using PagedList;
using SIMS_CW.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace SIMS_CW.Controllers
{
    public class ManagerController : Controller
    {
        DbModel dbData = new DbModel();
        private static academic_years current_year;

        public ActionResult Index(int? page, string idea_title, string name, string categoryID, string departmentID, string ideaStatus, string time_order, string pubFrom, string pubTo)
        {

            user loggedIn = (user)Session["loggedIn"];

            //check logged in?
            if (loggedIn == null)
            {
                return Redirect("~/Home/LoginPage");
            }
            if (Session["cateCbb"] == null)
            {
                List<category> categories = dbData.categories.ToList();
                SelectList listItems = new SelectList(categories, "category_id", "category_name");
                Session["cateCbb"] = listItems;
            }
            if (Session["deptCbb"] == null)
            {
                List<department> departments = dbData.departments.ToList();
                SelectList listItems = new SelectList(departments, "department_id", "department_name");
                Session["deptCbb"] = listItems;
            }
            if (loggedIn.role.role_id < 1 || loggedIn.role.role_id > 3)
            {
                return Redirect("~/Home/DeniedAccess");
            }

            //store page to return to proper page when visited Details page
            Session["previousPage"] = Url.Action("Index", "Manager");

            current_year = dbData.academic_years.Where(item => item.started_at <= DateTime.Now).Where(item => item.ended_at >= DateTime.Now).Single();
            /*List<display_idea> display_Ideas = getAllDisplayIdeas().Where(item => item.idea.isEnabled == 1).ToList();*/
            List<display_idea> display_Ideas = getAllDisplayIdeas().ToList();
            



            //--------filtering-------

            //filter with title
            display_Ideas = TitleFilter(display_Ideas, idea_title);

            // filter with published user name
            display_Ideas = UsernameFilter(display_Ideas, name);

            //filter with category
            display_Ideas = CategoryFilter(display_Ideas, categoryID);

            //filter with department
            display_Ideas = DepartmentFilter(display_Ideas, departmentID);

            // filter for status
            display_Ideas = StatusFilter(display_Ideas, ideaStatus);

            // filter publish from to
            display_Ideas = PublishFromFilter(display_Ideas, pubFrom, pubTo);

            // filter for order
            display_Ideas = OrderFilter(display_Ideas, time_order);


            //--------End filtering-------

            ViewBag.currentUser = loggedIn;

            DateTime dt = current_year.deadline_comments ?? DateTime.Now;
            ViewBag.closure_date = dt.ToString("yyyy/MM/dd");
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(display_Ideas.ToPagedList(pageNumber, pageSize));
        }

        private List<display_idea> getAllDisplayIdeas()
        {
            List<display_idea> display_Ideas = new List<display_idea>();
            List<idea> ideas = dbData.ideas.Where(item => item.academic_year_id == current_year.academic_year_id).ToList();
            /*List<idea> ideas = dbData.ideas.ToList();*/
            for (int i = 0; i < ideas.Count; i++)
            {
                idea idea = ideas[i];
                int user_id = Convert.ToInt32(idea.user_id);
                int isAnonymous = Convert.ToInt32(idea.isAnonymous);

                user user = dbData.users.Where(u => u.user_id == user_id).First();
                display_idea display_Idea = new display_idea();

                display_Idea.idea = idea;
                display_Idea.user = user;
                /*if (isAnonymous == 1)
                {
                    //true
                    user u = new user();
                    u.user_id = user.user_id;
                    u.user_name = "Anonymous";
                    display_Idea.user = u;
                }*/


                var rates = dbData.rates.Where(item => item.idea_id == idea.idea_id);
                int rate_point = 0;
                foreach (rate rate in rates)
                {
                    rate_point += Convert.ToInt32(rate.rate_point);
                }
                display_Idea.rate_point = rate_point;

                int rate_point_up = 0;
                int rate_point_down = 0;
                foreach (rate rate in rates)
                {
                    if (rate.rate_point == 1)
                    {
                        rate_point_up += Convert.ToInt32(rate.rate_point);
                    }
                    else if (rate.rate_point == -1)
                    {
                        rate_point_down += Convert.ToInt32(rate.rate_point);
                    }
                }
                display_Idea.rate_point_up = rate_point_up;
                display_Idea.rate_point_down = rate_point_down;

                //get number of comments of idea for sorting purposes
                var comments = dbData.comments.Where(c => c.idea_id == idea.idea_id);
                int numberOfComments = 0;
                foreach (comment comment in comments)
                {
                    numberOfComments++;
                }
                display_Idea.number_of_comments = numberOfComments;

                display_Ideas.Add(display_Idea);
            }
            display_Ideas.OrderByDescending(item => item.idea.created_at);
            return display_Ideas;
        }

        private List<display_comment> getAllDisplayComments()
        {
            List<display_comment> display_Comments = new List<display_comment>();
            /*List<comment> comments = dbData.comments.ToList();*/
            List<comment> comments = dbData.comments.Where(item => item.idea.academic_year_id == current_year.academic_year_id).ToList();
            for (int i = 0; i < comments.Count; i++)
            {
                comment comment = comments[i];
                int user_id = Convert.ToInt32(comment.user_id);

                user user = dbData.users.Where(u => u.user_id == user_id).First();
                display_comment display_Comment = new display_comment();

                display_Comment.comment = comment;
                display_Comment.user = user;
                
                display_Comments.Add(display_Comment);
            }
            display_Comments.OrderByDescending(item => item.comment.created_at);
            return display_Comments;
        }


        //GET
        public ActionResult ApproveIdeas(int? page, string idea_title, string name, string categoryID, string departmentID, string time_order, string pubFrom, string pubTo)
        {
            user loggedIn = (user)Session["loggedIn"];

            //check logged in?
            if (loggedIn == null)
            {
                return Redirect("~/Home/LoginPage");
            }
            if (Session["cateCbb"] == null)
            {
                List<category> categories = dbData.categories.ToList();
                SelectList listItems = new SelectList(categories, "category_id", "category_name");
                Session["cateCbb"] = listItems;
            }
            if (Session["deptCbb"] == null)
            {
                List<department> departments = dbData.departments.ToList();
                SelectList listItems = new SelectList(departments, "department_id", "department_name");
                Session["deptCbb"] = listItems;
            }
            if (loggedIn.role.role_id < 1 || loggedIn.role.role_id > 3)
            {
                return Redirect("~/Home/DeniedAccess");
            }

            //store page to return to proper page when visited Details page
            Session["previousPage"] = Url.Action("ApproveIdeas", "Manager");

            current_year = dbData.academic_years.Where(item => item.started_at <= DateTime.Now).Where(item => item.ended_at >= DateTime.Now).Single();

            List<display_idea> display_Ideas;
            //show all records to QA Manager
            if (loggedIn.role_id == 1 || loggedIn.role_id == 2)
            {
                display_Ideas = getAllDisplayIdeas().Where(item => item.idea.status == 0).ToList();
            }
            else
            {
                display_Ideas = getAllDisplayIdeas().Where(item => item.idea.status == 0).Where(item => item.user.department_id == loggedIn.department_id).ToList();
            }
            
            //--------filtering-------

            //filter with title
            display_Ideas = TitleFilter(display_Ideas, idea_title);

            // filter with published user name
            display_Ideas = UsernameFilter(display_Ideas, name);

            //filter with category
            display_Ideas = CategoryFilter(display_Ideas, categoryID);

            //filter with department
            display_Ideas = DepartmentFilter(display_Ideas, departmentID);

            // filter publish from to
            display_Ideas = PublishFromFilter(display_Ideas, pubFrom, pubTo);

            // filter for order
            if (time_order == null)
            {
                display_Ideas = display_Ideas.OrderByDescending(di => di.idea.created_at).ToList();
            }
            else
            {
                if (time_order.Equals("Newest"))
                {
                    display_Ideas = display_Ideas.OrderByDescending(di => di.idea.created_at).ToList();
                    ViewBag.time_order = time_order;
                }
                else if (time_order.Equals("Oldest"))
                {
                    display_Ideas = display_Ideas.OrderBy(di => di.idea.created_at).ToList();
                    ViewBag.time_order = time_order;
                }
            }

            //--------End filtering-------

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(display_Ideas.ToPagedList(pageNumber, pageSize));
        }


        //GET
        public ActionResult MostViewed(int? page, string idea_title, string name, string categoryID, string departmentID, string time_order, string pubFrom, string pubTo)
        {
            user loggedIn = (user)Session["loggedIn"];

            //check logged in?
            if (loggedIn == null)
            {
                return Redirect("~/Home/LoginPage");
            }
            if (Session["cateCbb"] == null)
            {
                List<category> categories = dbData.categories.ToList();
                SelectList listItems = new SelectList(categories, "category_id", "category_name");
                Session["cateCbb"] = listItems;
            }
            if (Session["deptCbb"] == null)
            {
                List<department> departments = dbData.departments.ToList();
                SelectList listItems = new SelectList(departments, "department_id", "department_name");
                Session["deptCbb"] = listItems;
            }
            if (loggedIn.role.role_id < 1 || loggedIn.role.role_id > 4)
            {
                return Redirect("~/Home/DeniedAccess");
            }

            //store page to return to proper page when visited Details page
            Session["previousPage"] = Url.Action("MostViewed", "Manager");

            current_year = dbData.academic_years.Where(item => item.started_at <= DateTime.Now).Where(item => item.ended_at >= DateTime.Now).Single();
            List<display_idea> display_Ideas = getAllDisplayIdeas().Where(item => item.idea.viewed_count > 0).ToList();

            //--------filtering-------

            //filter with title
            display_Ideas = TitleFilter(display_Ideas, idea_title);

            // filter with published user name
            display_Ideas = UsernameFilter(display_Ideas, name);

            //filter with category
            display_Ideas = CategoryFilter(display_Ideas, categoryID);

            //filter with department
            display_Ideas = DepartmentFilter(display_Ideas, departmentID);

            // filter publish from to
            display_Ideas = PublishFromFilter(display_Ideas, pubFrom, pubTo);

            // filter for order
            if (time_order == null)
            {
                display_Ideas = display_Ideas.OrderByDescending(di => di.idea.viewed_count).ToList();
            }
            else
            {
                if (time_order.Equals("Newest"))
                {
                    display_Ideas = display_Ideas.OrderByDescending(di => di.idea.created_at).ToList();
                    ViewBag.time_order = time_order;
                }
                else if (time_order.Equals("Oldest"))
                {
                    display_Ideas = display_Ideas.OrderBy(di => di.idea.created_at).ToList();
                    ViewBag.time_order = time_order;
                }
            }

            //--------End filtering-------

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(display_Ideas.ToPagedList(pageNumber, pageSize));
        }


        //GET
        public ActionResult MostPopular(int? page, string idea_title, string name, string categoryID, string departmentID, string time_order, string pubFrom, string pubTo)
        {
            user loggedIn = (user)Session["loggedIn"];

            //check logged in?
            if (loggedIn == null)
            {
                return Redirect("~/Home/LoginPage");
            }
            if (Session["cateCbb"] == null)
            {
                List<category> categories = dbData.categories.ToList();
                SelectList listItems = new SelectList(categories, "category_id", "category_name");
                Session["cateCbb"] = listItems;
            }
            if (Session["deptCbb"] == null)
            {
                List<department> departments = dbData.departments.ToList();
                SelectList listItems = new SelectList(departments, "department_id", "department_name");
                Session["deptCbb"] = listItems;
            }
            if (loggedIn.role.role_id < 1 || loggedIn.role.role_id > 4)
            {
                return Redirect("~/Home/DeniedAccess");
            }

            //store page to return to proper page when visited Details page
            Session["previousPage"] = Url.Action("MostPopular", "Manager");

            current_year = dbData.academic_years.Where(item => item.started_at <= DateTime.Now).Where(item => item.ended_at >= DateTime.Now).Single();
            List<display_idea> display_Ideas = getAllDisplayIdeas().Where(di => di.idea.status == 1).ToList();

            //--------filtering-------

            //filter with title
            display_Ideas = TitleFilter(display_Ideas, idea_title);

            // filter with published user name
            display_Ideas = UsernameFilter(display_Ideas, name);

            //filter with category
            display_Ideas = CategoryFilter(display_Ideas, categoryID);

            //filter with department
            display_Ideas = DepartmentFilter(display_Ideas, departmentID);

            // filter publish from to
            display_Ideas = PublishFromFilter(display_Ideas, pubFrom, pubTo);


            // order
            if (time_order == null)
            {
                display_Ideas = display_Ideas.OrderByDescending(di => di.rate_point_up).ToList();
            }
            else
            {
                if (time_order.Equals("Newest"))
                {
                    display_Ideas = display_Ideas.OrderByDescending(di => di.idea.created_at).ToList();
                    ViewBag.time_order = time_order;
                }
                else if (time_order.Equals("Oldest"))
                {
                    display_Ideas = display_Ideas.OrderBy(di => di.idea.created_at).ToList();
                    ViewBag.time_order = time_order;
                }
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(display_Ideas.ToPagedList(pageNumber, pageSize));
        }


        //GET
        public ActionResult IdeasWithoutComments(int? page, string idea_title, string name, string categoryID, string departmentID, string ideaStatus, string time_order, string pubFrom, string pubTo)
        {
            user loggedIn = (user)Session["loggedIn"];

            //check logged in?
            if (loggedIn == null)
            {
                return Redirect("~/Home/LoginPage");
            }
            if (Session["cateCbb"] == null)
            {
                List<category> categories = dbData.categories.ToList();
                SelectList listItems = new SelectList(categories, "category_id", "category_name");
                Session["cateCbb"] = listItems;
            }
            if (Session["deptCbb"] == null)
            {
                List<department> departments = dbData.departments.ToList();
                SelectList listItems = new SelectList(departments, "department_id", "department_name");
                Session["deptCbb"] = listItems;
            }
            if (loggedIn.role.role_id < 1 || loggedIn.role.role_id > 3)
            {
                return Redirect("~/Home/DeniedAccess");
            }

            //store page to return to proper page when visited Details page
            Session["previousPage"] = Url.Action("IdeasWithoutComments", "Manager");

            current_year = dbData.academic_years.Where(item => item.started_at <= DateTime.Now).Where(item => item.ended_at >= DateTime.Now).Single();
            List<display_idea> display_Ideas = getAllDisplayIdeas().Where(di => di.number_of_comments == 0).ToList();

            //--------filtering-------

            //filter with title
            display_Ideas = TitleFilter(display_Ideas, idea_title);

            // filter with published user name
            display_Ideas = UsernameFilter(display_Ideas, name);

            //filter with category
            display_Ideas = CategoryFilter(display_Ideas, categoryID);

            //filter with department
            display_Ideas = DepartmentFilter(display_Ideas, departmentID);

            // filter publish from to
            display_Ideas = PublishFromFilter(display_Ideas, pubFrom, pubTo);

            // filter for order
            display_Ideas = OrderFilter(display_Ideas, time_order);

            //--------End filtering-------

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(display_Ideas.ToPagedList(pageNumber, pageSize));
        }


        //GET
        public ActionResult AnonymousIdeas(int? page, string idea_title, string name, string categoryID, string departmentID, string time_order, string pubFrom, string pubTo)
        {
            user loggedIn = (user)Session["loggedIn"];

            //check logged in?
            if (loggedIn == null)
            {
                return Redirect("~/Home/LoginPage");
            }
            if (Session["cateCbb"] == null)
            {
                List<category> categories = dbData.categories.ToList();
                SelectList listItems = new SelectList(categories, "category_id", "category_name");
                Session["cateCbb"] = listItems;
            }
            if (Session["deptCbb"] == null)
            {
                List<department> departments = dbData.departments.ToList();
                SelectList listItems = new SelectList(departments, "department_id", "department_name");
                Session["deptCbb"] = listItems;
            }
            if (loggedIn.role.role_id < 1 || loggedIn.role.role_id > 3)
            {
                return Redirect("~/Home/DeniedAccess");
            }

            //store page to return to proper page when visited Details page
            Session["previousPage"] = Url.Action("AnonymousIdeas", "Manager");

            current_year = dbData.academic_years.Where(item => item.started_at <= DateTime.Now).Where(item => item.ended_at >= DateTime.Now).Single();
            List<display_idea> display_Ideas = getAllDisplayIdeas().Where(di => di.idea.isAnonymous == 0).Where(di => di.idea.status == 1).ToList();



            //--------filtering-------

            //filter with title
            display_Ideas = TitleFilter(display_Ideas, idea_title);

            // filter with published user name
            display_Ideas = UsernameFilter(display_Ideas, name);

            //filter with category
            display_Ideas = CategoryFilter(display_Ideas, categoryID);

            //filter with department
            display_Ideas = DepartmentFilter(display_Ideas, departmentID);

            // filter publish from to
            display_Ideas = PublishFromFilter(display_Ideas, pubFrom, pubTo);

            // filter for order
            display_Ideas = OrderFilter(display_Ideas, time_order);

            //--------End filtering-------

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(display_Ideas.ToPagedList(pageNumber, pageSize));
        }


        //GET
        public ActionResult LatestIdeas(int? page, string idea_title, string name, string categoryID, string departmentID, string time_order, string pubFrom, string pubTo)
        {
            user loggedIn = (user)Session["loggedIn"];

            //check logged in?
            if (loggedIn == null)
            {
                return Redirect("~/Home/LoginPage");
            }
            if (Session["cateCbb"] == null)
            {
                List<category> categories = dbData.categories.ToList();
                SelectList listItems = new SelectList(categories, "category_id", "category_name");
                Session["cateCbb"] = listItems;
            }
            if (Session["deptCbb"] == null)
            {
                List<department> departments = dbData.departments.ToList();
                SelectList listItems = new SelectList(departments, "department_id", "department_name");
                Session["deptCbb"] = listItems;
            }
            if (loggedIn.role.role_id < 1 || loggedIn.role.role_id > 4)
            {
                return Redirect("~/Home/DeniedAccess");
            }

            //store page to return to proper page when visited Details page
            Session["previousPage"] = Url.Action("LatestIdeas", "Manager");

            //get date two weeks earlier
            DateTime today = DateTime.Today;
            DateTime twoWeeks_Earlier = today.AddDays(-14);

            current_year = dbData.academic_years.Where(item => item.started_at <= DateTime.Now).Where(item => item.ended_at >= DateTime.Now).Single();
            List<display_idea> display_Ideas = getAllDisplayIdeas().Where(di => di.idea.status == 1).Where(dc => dc.idea.created_at > twoWeeks_Earlier).ToList();



            //--------filtering-------

            //filter with title
            display_Ideas = TitleFilter(display_Ideas, idea_title);

            // filter with published user name
            display_Ideas = UsernameFilter(display_Ideas, name);

            //filter with category
            display_Ideas = CategoryFilter(display_Ideas, categoryID);

            //filter with department
            display_Ideas = DepartmentFilter(display_Ideas, departmentID);

            // filter publish from to
            display_Ideas = PublishFromFilter(display_Ideas, pubFrom, pubTo);

            // filter for order
            display_Ideas = OrderFilter(display_Ideas, time_order);

            //--------End filtering-------

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(display_Ideas.ToPagedList(pageNumber, pageSize));
        }


        //GET
        public ActionResult AnonymousComments(int? page, string name, string roleID, string departmentID, string time_order, string pubFrom, string pubTo)
        {
            user loggedIn = (user)Session["loggedIn"];

            //check logged in?
            if (loggedIn == null)
            {
                return Redirect("~/Home/LoginPage");
            }
            if (Session["roleCbb"] == null)
            {
                List<role> roles = dbData.roles.ToList();
                SelectList listItems = new SelectList(roles, "role_id", "role_name");
                Session["roleCbb"] = listItems;
            }
            if (Session["deptCbb"] == null)
            {
                List<department> departments = dbData.departments.ToList();
                SelectList listItems = new SelectList(departments, "department_id", "department_name");
                Session["deptCbb"] = listItems;
            }
            if (loggedIn.role.role_id < 1 || loggedIn.role.role_id > 3)
            {
                return Redirect("~/Home/DeniedAccess");
            }

            //store page to return to proper page when visited Details page
            Session["previousPage"] = Url.Action("AnonymousComments", "Manager");

            current_year = dbData.academic_years.Where(item => item.started_at <= DateTime.Now).Where(item => item.ended_at >= DateTime.Now).Single();
            List<display_comment> display_Comments = getAllDisplayComments().Where(dc => dc.comment.isAnonymous == 1).ToList();


            //--------filtering-------

            //filter for username
            display_Comments = UsernameFilterComment(display_Comments, name);

            display_Comments = RoleFilterComment(display_Comments, roleID);

            //filter for departments
            display_Comments = DepartmentFilterComment(display_Comments, departmentID);

            //filter for 'Publish from to ..'
            display_Comments = PublishFromFilterComment(display_Comments, pubFrom, pubTo);
            
            // filter for order
            if (time_order == null)
            {
                display_Comments = display_Comments.OrderByDescending(dc => dc.comment.created_at).ToList();
            }
            else
            {
                if (time_order.Equals("Newest"))
                {
                    display_Comments = display_Comments.OrderByDescending(dc => dc.comment.created_at).ToList();
                    ViewBag.time_order = time_order;
                }
                else if (time_order.Equals("Oldest"))
                {
                    display_Comments = display_Comments.OrderBy(dc => dc.comment.created_at).ToList();
                    ViewBag.time_order = time_order;
                }
            }

            //--------End filtering-------


            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(display_Comments.ToPagedList(pageNumber, pageSize));
        }


        //GET
        public ActionResult LatestComments(int? page, string name, string roleID, string departmentID, string time_order, string pubFrom, string pubTo)
        {
            user loggedIn = (user)Session["loggedIn"];

            //check logged in?
            if (loggedIn == null)
            {
                return Redirect("~/Home/LoginPage");
            }
            if (Session["roleCbb"] == null)
            {
                List<role> roles = dbData.roles.ToList();
                SelectList listItems = new SelectList(roles, "role_id", "role_name");
                Session["roleCbb"] = listItems;
            }
            if (Session["deptCbb"] == null)
            {
                List<department> departments = dbData.departments.ToList();
                SelectList listItems = new SelectList(departments, "department_id", "department_name");
                Session["deptCbb"] = listItems;
            }
            if (loggedIn.role.role_id < 1 || loggedIn.role.role_id > 4)
            {
                return Redirect("~/Home/DeniedAccess");
            }

            //store page to return to proper page when visited Details page
            Session["previousPage"] = Url.Action("LatestComments", "Manager");

            //get date two weeks earlier
            DateTime today = DateTime.Today;
            DateTime twoWeeks_Earlier = today.AddDays(-14);

            current_year = dbData.academic_years.Where(item => item.started_at <= DateTime.Now).Where(item => item.ended_at >= DateTime.Now).Single();
            List<display_comment> display_Comments = getAllDisplayComments().Where(dc => dc.comment.created_at > twoWeeks_Earlier).ToList();


            //--------filtering-------

            //filter for username
            display_Comments = UsernameFilterComment(display_Comments, name);

            display_Comments = RoleFilterComment(display_Comments, roleID);

            //filter for departments
            display_Comments = DepartmentFilterComment(display_Comments, departmentID);

            //filter for 'Publish from to ..'
            display_Comments = PublishFromFilterComment(display_Comments, pubFrom, pubTo);

            // filter for order
            if (time_order == null)
            {
                display_Comments = display_Comments.OrderByDescending(dc => dc.comment.created_at).ToList();
            }
            else
            {
                if (time_order.Equals("Newest"))
                {
                    display_Comments = display_Comments.OrderByDescending(dc => dc.comment.created_at).ToList();
                    ViewBag.time_order = time_order;
                }
                else if (time_order.Equals("Oldest"))
                {
                    display_Comments = display_Comments.OrderBy(dc => dc.comment.created_at).ToList();
                    ViewBag.time_order = time_order;
                }
            }

            //--------End filtering-------


            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(display_Comments.ToPagedList(pageNumber, pageSize));
        }


        //GET
        public ActionResult Details(int idea_id, string mode)
        {
            user loggedIn = (user)Session["loggedIn"];

            //check logged in?
            if (loggedIn == null)
            {
                return Redirect("~/Home/LoginPage");
            }
            if (loggedIn.role.role_id < 1 || loggedIn.role.role_id > 4)
            {
                return Redirect("~/Home/DeniedAccess");
            }

            ViewBag.currentUser = loggedIn;
            idea idea = dbData.ideas.Find(idea_id);
            int uid = Convert.ToInt32(idea.user_id);
            List<comment> comments = comments = dbData.comments.Where(c => c.idea_id == idea_id).OrderByDescending(c=>c.created_at).ToList();
            List<comment> temp = new List<comment>();
            dbData.SaveChanges();
            
            //get all comments
            List<user> comment_users = new List<user>();
            for (int i = 0; i < comments.Count; i++)
            {
                comment comment = comments[i];

                int user_id = Convert.ToInt32(comment.user_id);
                int isAnonymous = Convert.ToInt32(comment.isAnonymous);

                user user = dbData.users.Where(u => u.user_id == user_id).First();
                comment_users.Add(user);
                if (isAnonymous == 1)
                {
                    //true
                    user u = new user();
                    u.user_id = user.user_id;
                    u.user_name = comment.user.user_name + " (Anonymous)";
                    comment_users[i] = u;
                }
            }

            //get attachment
            List<document> documents = dbData.documents.Where(d => d.idea_id == idea_id).ToList();
            ViewBag.documents = documents;

            //get rate
            int cnt_likes = 0;
            int cnt_dislikes = 0;
            IQueryable<rate> rates = dbData.rates.Where(r => r.idea_id == idea_id);
            foreach(rate item in rates)
            {
                if (item.rate_point == 1)
                {
                    cnt_likes++;
                }
                else
                {
                    cnt_dislikes++;
                }
            }

            ViewBag.likes = cnt_likes;
            ViewBag.dislikes = cnt_dislikes;
            ViewBag.Idea = idea;
            if (idea.isAnonymous == 0)
            {
                ViewBag.Idea_user = dbData.users.Where(u => u.user_id == uid).First();
            }
            if(mode == "approve")
            {
                ViewBag.mode = "approve";
            }
            else
            {
                ViewBag.mode = "viewall";
            }
            dynamic mymodel = new ExpandoObject();
            mymodel.Comments = comments;
            mymodel.Comment_users = comment_users;

            return View(mymodel);
        }


        [HttpPost]
        public ActionResult Approve(int idea_id)
        {
            idea idea = dbData.ideas.Find(idea_id);
            idea.status = 1;
            dbData.SaveChanges();
            return RedirectToAction("ApproveIdeas", new { page = 1 });
        }


        [HttpPost]
        public ActionResult Deny(int idea_id, string reason)
        {
            idea idea = dbData.ideas.Find(idea_id);
            idea.status = 2;
            dbData.SaveChanges();

            //Configuring webMail class to send emails  
            //gmail smtp server  
            WebMail.SmtpServer = "smtp.gmail.com";
            //gmail port to send emails  
            WebMail.SmtpPort = 587;
            WebMail.SmtpUseDefaultCredentials = true;
            //sending emails with secure protocol  
            WebMail.EnableSsl = true;
            //EmailId used to send emails from application  
            WebMail.UserName = "simscw2018@gmail.com";
            WebMail.Password = "abc123xyz";


            IQueryable<user> userDepartment = dbData.users.Where(q => q.department_id == idea.user.department_id);
            user qaCoordinator = userDepartment.Where(q => q.role_id == 3).Single();

            //Sender email address.  
            WebMail.From = "simscw2018@gmail.com";
            String ToEmail = idea.user.email;
            String EmailSubject = "Your idea submission has been rejected!";
            String EMailBody = "<h3>Your idea submission has been rejected!</h3>" + "<br/><br/>"
                                    + "<b>Idea title: </b>" + idea.idea_title + "<br/>"
                                    + "<b>Idea Content: </b>" + idea.idea_content + "<br/><br/>"
                                    + "<b>Reason: </b>" + reason + "<br/><br/>"
                                    + "<a href ='onlineexamproject2018.somee.com/Idea/Details?idea_id=" + idea.idea_id + "'>Click here</a>" + " to access your submitted idea." + "<br/><br/>"

                                    + "For more information, please contact the QA Coordinator at " + qaCoordinator.email + "." + "<br/>"
                                    + "Best regards," + "<br/><br/>"
                                    + "Quality Assurance team";
            //Send email  
            WebMail.Send(to: ToEmail, subject: EmailSubject, body: EMailBody, isBodyHtml: true);

            return RedirectToAction("ApproveIdeas", new { page = 1 });
        }


        // GET: Manager
        public ActionResult LineChart()
        {
            return View();
        }


        public FileResult Download()
        {
            string fileSavePath = System.Web.Hosting.HostingEnvironment.MapPath("~/UploadedFiles");
            DirectoryInfo dirInfo = new DirectoryInfo(fileSavePath);
            
            var documents = dbData.documents;
            using (var memoryStream = new MemoryStream())
            {
                using (var ziparchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach(document document in documents)
                    {
                        ziparchive.CreateEntryFromFile(dirInfo.FullName +"/" + document.new_file_name, document.old_file_name);
                    }
                }
                return File(memoryStream.ToArray(), "application/zip", "Attachments.zip");
            }
        }







        //-----------Filter Ideas----------

        //filter for title
        private List<display_idea> TitleFilter(List<display_idea> display_Ideas, string idea_title)
        {
            if (idea_title != null)
            {
                List<display_idea> temp = new List<display_idea>();
                foreach (display_idea item in display_Ideas)
                {
                    if (!item.idea.idea_title.ToLower().Contains(idea_title.ToLower()))
                    {
                        temp.Add(item);
                    }
                }
                foreach (display_idea item in temp)
                {
                    display_Ideas.Remove(item);
                }
                ViewBag.idea_title = idea_title;
            }
            return display_Ideas;
        }

        //filter for username
        private List<display_idea> UsernameFilter(List<display_idea> display_Ideas, string name)
        {
            if (name != null)
            {
                List<display_idea> temp = new List<display_idea>();
                foreach (display_idea item in display_Ideas)
                {
                    if (!item.user.user_name.ToLower().Contains(name.ToLower()))
                    {
                        temp.Add(item);
                    }
                }
                foreach (display_idea item in temp)
                {
                    display_Ideas.Remove(item);
                }
                ViewBag.name = name;
            }
            return display_Ideas;
        }

        //filter for caregory
        private List<display_idea> CategoryFilter(List<display_idea> display_Ideas, string categoryID)
        {
            try
            {
                int filter_categoryID = Convert.ToInt32(categoryID);
                if (filter_categoryID != 0)
                {
                    List<display_idea> temp = new List<display_idea>();
                    foreach (display_idea item in display_Ideas)
                    {
                        if (!(item.idea.category_id == filter_categoryID))
                        {
                            temp.Add(item);
                        }
                    }
                    foreach (display_idea item in temp)
                    {
                        display_Ideas.Remove(item);
                    }
                    ViewBag.categoryID = filter_categoryID;
                }
            }
            catch (FormatException) { }
            return display_Ideas;
        }

        //filter for departments
        private List<display_idea> DepartmentFilter(List<display_idea> display_Ideas, string departmentID)
        {
            try
            {
                int filter_departmentID = Convert.ToInt32(departmentID);
                if (filter_departmentID != 0)
                {
                    List<display_idea> temp = new List<display_idea>();
                    foreach (display_idea item in display_Ideas)
                    {
                        if (!(item.idea.user.department_id == filter_departmentID))
                        {
                            temp.Add(item);
                        }
                    }
                    foreach (display_idea item in temp)
                    {
                        display_Ideas.Remove(item);
                    }
                    ViewBag.departmentID = filter_departmentID;
                }
            }
            catch (FormatException) { }
            return display_Ideas;
        }

        //filter for status
        private List<display_idea> StatusFilter(List<display_idea> display_Ideas, string ideaStatus)
        {
            if (ideaStatus == null)
            {
                display_Ideas = display_Ideas.OrderByDescending(di => di.idea.created_at).ToList();
            }
            else
            {
                if (ideaStatus == "Pending")
                {
                    display_Ideas = display_Ideas.Where(di => di.idea.status == 0).ToList();
                    ViewBag.ideaStatus = ideaStatus;
                }
                else if (ideaStatus == "Approved")
                {
                    display_Ideas = display_Ideas.Where(di => di.idea.status == 1).ToList();
                    ViewBag.ideaStatus = ideaStatus;
                }
                else if (ideaStatus == "Rejected")
                {
                    display_Ideas = display_Ideas.Where(di => di.idea.status == 2).ToList();
                    ViewBag.ideaStatus = ideaStatus;
                }
            }
            return display_Ideas;
        }

        //filter for order
        private List<display_idea> OrderFilter(List<display_idea> display_Ideas, string time_order)
        {
            if (time_order == null)
            {
                display_Ideas = display_Ideas.OrderByDescending(di => di.idea.created_at).ToList();
            }
            else
            {
                if (time_order.Equals("Newest"))
                {
                    display_Ideas = display_Ideas.OrderByDescending(di => di.idea.created_at).ToList();
                    ViewBag.time_order = time_order;
                }
                else if (time_order.Equals("Oldest"))
                {
                    display_Ideas = display_Ideas.OrderBy(di => di.idea.created_at).ToList();
                    ViewBag.time_order = time_order;
                }
            }
            return display_Ideas;
        }

        //filter for 'Publish from to ..'
        private List<display_idea> PublishFromFilter(List<display_idea> display_Ideas, string pubFrom, string pubTo)
        {
            try
            {
                DateTime pubFromTime = DateTime.Parse(pubFrom);
                DateTime pubToTime = DateTime.Parse(pubTo);
                if (pubFromTime > pubToTime)
                {
                    ViewBag.error = "To time must be after 'From Time' ";

                }
                else
                {
                    List<display_idea> temp = new List<display_idea>();
                    foreach (display_idea item in display_Ideas)
                    {
                        if (item.idea.created_at < pubFromTime || item.idea.created_at > pubToTime)
                        {
                            temp.Add(item);
                        }
                    }
                    foreach (display_idea item in temp)
                    {
                        display_Ideas.Remove(item);
                    }
                    ViewBag.pubFrom = pubFrom;
                    ViewBag.pubTo = pubTo;
                }

            }
            catch (ArgumentNullException) { }
            catch (FormatException)
            {
                if (pubTo != pubFrom)
                    ViewBag.error = "Both 'From Time' and 'To Time' are required";
            }
            return display_Ideas;
        }

        //filter for other arguments
        private List<display_idea> OtherFilter(List<display_idea> display_Ideas, string other_filter)
        {
            if (other_filter != null)
            {
                if (other_filter.Equals("Most Popular"))
                {
                    display_Ideas = display_Ideas.OrderByDescending(di => di.rate_point).ToList();
                    ViewBag.time_order = "None";
                    ViewBag.other_filter = other_filter;
                }
                else if (other_filter.Equals("Most Viewed"))
                {
                    display_Ideas = display_Ideas.OrderByDescending(di => di.idea.viewed_count).ToList();
                    ViewBag.time_order = "None";
                    ViewBag.other_filter = other_filter;
                }
            }
            return display_Ideas;
        }


        //-----------Filter Comments----------

        //filter for username
        private List<display_comment> UsernameFilterComment(List<display_comment> display_Comments, string name)
        {
            if (name != null)
            {
                List<display_comment> temp = new List<display_comment>();
                foreach (display_comment item in display_Comments)
                {
                    if (!item.user.user_name.ToLower().Contains(name.ToLower()))
                    {
                        temp.Add(item);
                    }
                }
                foreach (display_comment item in temp)
                {
                    display_Comments.Remove(item);
                }
                ViewBag.name = name;
            }
            return display_Comments;
        }

        //filter for departments
        private List<display_comment> RoleFilterComment(List<display_comment> display_Comments, string roleID)
        {
            try
            {
                int filter_roleID = Convert.ToInt32(roleID);
                if (filter_roleID != 0)
                {
                    List<display_comment> temp = new List<display_comment>();
                    foreach (display_comment item in display_Comments)
                    {
                        if (!(item.comment.user.role_id == filter_roleID))
                        {
                            temp.Add(item);
                        }
                    }
                    foreach (display_comment item in temp)
                    {
                        display_Comments.Remove(item);
                    }
                    ViewBag.roleID = filter_roleID;
                }
            }
            catch (FormatException) { }
            return display_Comments;
        }

        //filter for departments
        private List<display_comment> DepartmentFilterComment(List<display_comment> display_Comments, string departmentID)
        {
            try
            {
                int filter_departmentID = Convert.ToInt32(departmentID);
                if (filter_departmentID != 0)
                {
                    List<display_comment> temp = new List<display_comment>();
                    foreach (display_comment item in display_Comments)
                    {
                        if (!(item.comment.user.department_id == filter_departmentID))
                        {
                            temp.Add(item);
                        }
                    }
                    foreach (display_comment item in temp)
                    {
                        display_Comments.Remove(item);
                    }
                    ViewBag.departmentID = filter_departmentID;
                }
            }
            catch (FormatException) { }
            return display_Comments;
        }

        //filter for 'Publish from to ..'
        private List<display_comment> PublishFromFilterComment(List<display_comment> display_Comments, string pubFrom, string pubTo)
        {
            try
            {
                DateTime pubFromTime = DateTime.Parse(pubFrom);
                DateTime pubToTime = DateTime.Parse(pubTo);
                if (pubFromTime > pubToTime)
                {
                    ViewBag.error = "To time must be after 'From Time' ";

                }
                else
                {
                    List<display_comment> temp = new List<display_comment>();
                    foreach (display_comment item in display_Comments)
                    {
                        if (item.comment.created_at < pubFromTime || item.comment.created_at > pubToTime)
                        {
                            temp.Add(item);
                        }
                    }
                    foreach (display_comment item in temp)
                    {
                        display_Comments.Remove(item);
                    }
                    ViewBag.pubFrom = pubFrom;
                    ViewBag.pubTo = pubTo;
                }

            }
            catch (ArgumentNullException) { }
            catch (FormatException)
            {
                if (pubTo != pubFrom)
                    ViewBag.error = "Both 'From Time' and 'To Time' are required";
            }
            return display_Comments;
        }

    }
}