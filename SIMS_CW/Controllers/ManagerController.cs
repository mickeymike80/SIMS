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

        public ActionResult Index(int? page, string idea_title, string name, string categoryID, string time_order, string pubFrom, string pubTo, string other_filter)
        {
            //check logged in?
            if (Session["loggedIn"] == null)
            {
                return Redirect("~/Home/LoginPage");
            }
            if (Session["cateCbb"] == null)
            {
                List<category> categories = dbData.categories.ToList();
                SelectList listItems = new SelectList(categories, "category_id", "category_name");
                Session["cateCbb"] = listItems;
            }
            current_year = dbData.academic_years.Where(item => item.started_at <= DateTime.Now && item.ended_at >= DateTime.Now).Single();
            List<display_idea> display_Ideas = getAllDisplayIdeas().Where(item => item.idea.isEnabled == 1).ToList();

            //filter with title
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
            // filter with published user name
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

            //filter with category
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

            // filter publish from to
            try
            {
                DateTime pubFromTime = DateTime.Parse(pubFrom);
                DateTime pubToTime = DateTime.Parse(pubTo);
                if (pubFromTime > pubToTime)
                {
                    ViewBag.error = "To time must be after from time ";

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
                    ViewBag.error = "Both from time and to time are required";
            }


            // order
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

            //other filter
            if (other_filter != null)
            {
                if(other_filter.Equals("Most Popular"))
                {
                    display_Ideas = display_Ideas.OrderByDescending(di => di.rate_point).ToList();
                    ViewBag.time_order = "None";
                    ViewBag.other_filter = other_filter;
                }
                else if(other_filter.Equals("Most Viewed"))
                {
                    display_Ideas = display_Ideas.OrderByDescending(di => di.idea.viewed_count).ToList();
                    ViewBag.time_order = "None";
                    ViewBag.other_filter = other_filter;
                }
            }
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
            
            for (int i = 0; i < ideas.Count; i++)
            {
                idea idea = ideas[i];
                int user_id = Convert.ToInt32(idea.user_id);
                int isAnonymous = Convert.ToInt32(idea.isAnonymous);

                user user = dbData.users.Where(u => u.user_id == user_id).First();
                display_idea display_Idea = new display_idea();

                display_Idea.idea = idea;
                display_Idea.user = user;
                if (isAnonymous == 1)
                {
                    //true
                    user u = new user();
                    u.user_id = user.user_id;
                    u.user_name = "Anonymous";
                    display_Idea.user = u;
                }

                var rates = dbData.rates.Where(item => item.idea_id == idea.idea_id);
                int rate_point = 0;
                foreach(rate rate in rates)
                {
                    rate_point += Convert.ToInt32( rate.rate_point);
                }
                display_Idea.rate_point = rate_point;
                display_Ideas.Add(display_Idea);
            }
            display_Ideas.OrderByDescending(item => item.idea.created_at);
            return display_Ideas;
        }

        //GET
        public ActionResult ApproveIdeas(int? page, string idea_title, string name, string categoryID, string time_order, string pubFrom, string pubTo)
        {
            //check logged in?
            if (Session["loggedIn"] == null)
            {
                return Redirect("~/Home/LoginPage");
            }
            if (Session["cateCbb"] == null)
            {
                List<category> categories = dbData.categories.ToList();
                SelectList listItems = new SelectList(categories, "category_id", "category_name");
                Session["cateCbb"] = listItems;
            }
            current_year = dbData.academic_years.Where(item => item.started_at <= DateTime.Now).Where(item => item.ended_at >= DateTime.Now).Single();
            List<display_idea> display_Ideas = getAllDisplayIdeas().Where(item => item.idea.status == 0 || item.idea.status == 2).ToList();

            //filter with title
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
            // filter with published user name
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

            //filter with category
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

            // filter publish from to
            try
            {
                DateTime pubFromTime = DateTime.Parse(pubFrom);
                DateTime pubToTime = DateTime.Parse(pubTo);
                if (pubFromTime > pubToTime)
                {
                    ViewBag.error = "To time must be after from time ";

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
                    ViewBag.error = "Both from time and to time are required";
            }


            // order
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

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(display_Ideas.ToPagedList(pageNumber, pageSize));
        }

        //GET
        public ActionResult Details(int idea_id, string mode)
        {
            //check logged in?
            if (Session["loggedIn"] == null)
            {
                return Redirect("~/Home/LoginPage");
            }

            user loggedIn = (user)Session["loggedIn"];

            idea idea = dbData.ideas.Find(idea_id);
            int uid = Convert.ToInt32(idea.user_id);
            List<comment> comments = comments = dbData.comments.Where(c => c.idea_id == idea_id).OrderByDescending(c=>c.created_at).ToList();
            List<comment> temp = new List<comment>();
            idea.viewed_count += 1;
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
                    u.user_name = "Anonymous";
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
            idea.isEnabled = 1;
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

            //Sender email address.  
            WebMail.From = "simscw2018@gmail.com";
            String ToEmail = idea.user.email;
            String EmailSubject = "One of your ideas have been denied!";
            String EMailBody = "<h3>An idea has been denied!</h3> Idea title: " 
                + idea.idea_title + "<br/>"
                +"Reason: "+reason +"<br/>"
                + "Idea link: <a href ='onlineexamproject2018.somee.com/Idea/Details?idea_id=" + idea.idea_id + "'>click here</a> <br/>" +
                                            "For more information please contact the QA Manger.";
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
    }
}