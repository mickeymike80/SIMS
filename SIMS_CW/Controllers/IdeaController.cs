using SIMS_CW.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using PagedList;

namespace SIMS_CW.Controllers
{
    public class IdeaController : Controller
    {
        DbModel dbData = new DbModel();

        
        public ActionResult Index(int? page, string idea_title, string name, string categoryID, string time_order, string pubFrom, string pubTo)
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
            List<display_idea> display_Ideas = getAllDisplayIdeas();

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

            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(display_Ideas.ToPagedList(pageNumber, pageSize));
        }

        private List<display_idea> getAllDisplayIdeas()
        {
            List<display_idea> display_Ideas = new List<display_idea>();
            List<idea> ideas = dbData.ideas.ToList();
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
                display_Ideas.Add(display_Idea);
            }

            return display_Ideas;
        }

        [HttpGet]
        public ActionResult Create()
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
            
            return View();
        }

        [HttpPost]
        public ActionResult Create(idea idea, HttpPostedFileBase[] files)
        {
            //check logged in?
            if (Session["loggedIn"] == null)
            {
                return Redirect("~/Home/LoginPage");
            }

            // 0 = false; 1 = true
            idea.category_id = Convert.ToInt32(Request.Form["categoryID"].ToString());
            idea.isEnabled = 1;
            idea.status = 0;
            idea.viewed_count = 0;
            idea.created_at = DateTime.Now;
            List<academic_years> list = dbData.academic_years.ToList();
            for (int i = 0; i < list.Count; i++)
            {
                if (DateTime.Now < list[i].ended_at)
                {
                    idea.academic_year_id = list[i].academic_year_id;
                }
            }
            idea.user_id = ((user)Session["loggedIn"]).user_id;
            //check anonymous
            if (Request.Form["isAnonymous"] != null)
            {
                idea.isAnonymous = 1;
            }
            else
            {
                idea.isAnonymous = 0;
            }

            dbData.ideas.Add(idea);
            //file upload
            foreach (HttpPostedFileBase file in files)
            {
                //Checking file is available to save.  
                if (file != null)
                {
                    string oldfileName = Path.GetFileName(file.FileName);
                    string sessionID = HttpContext.Session.SessionID;
                    string newfilename = sessionID + Guid.NewGuid().ToString() + oldfileName;
                    string path = Path.Combine(Server.MapPath("~/UploadedFiles"), newfilename);
                    file.SaveAs(path);
                    document document = new document();
                    document.new_file_name = newfilename;
                    document.old_file_name = oldfileName;
                    document.created_at = DateTime.Now;
                    document.idea_id = idea.idea_id;
                    dbData.documents.Add(document);
                }
            }

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
            String ToEmail = "koolkido1412@gmail.com";
            String EmailSubject = "abc";
            String EMailBody = "lol";
            //Send email  
            WebMail.Send(to: ToEmail, subject: EmailSubject, body: EMailBody, isBodyHtml: true);

            dbData.SaveChanges();
            return Redirect(Url.Action("Index", "Idea", new { page = 1 }));
        }

        [HttpGet]
        public ActionResult Details(int idea_id)
        {
            //check logged in?
            if (Session["loggedIn"] == null)
            {
                return Redirect("~/Home/LoginPage");
            }

            user loggedIn = (user)Session["loggedIn"];

            idea idea = dbData.ideas.Where(i => i.idea_id == idea_id).First();
            int uid = Convert.ToInt32(idea.user_id);
            List<comment> comments = comments = dbData.comments.Where(c => c.idea_id == idea_id).ToList();
            List<comment> temp = new List<comment>();
            idea.viewed_count += 1;
            dbData.SaveChanges();
            //student can't see staff comments
            if (loggedIn.role_id == 5)
            {
                foreach (comment c in comments)
                {
                    int user_id = Convert.ToInt32(c.user_id);
                    user user = dbData.users.Where(u => u.user_id == user_id).First();
                    if (user.role_id != 5)
                    {
                        temp.Add(c);
                    }
                }
                foreach (comment c in temp)
                {
                    comments.Remove(c);
                }
            }
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
            IQueryable<rate> rates = dbData.rates.Where(r => r.idea_id == idea_id).Where(r => r.user_id == loggedIn.user_id);
            if (rates.Any())
            {
                ViewBag.rate = rates.First().rate_point;
            }


            ViewBag.Idea = idea;
            if (idea.isAnonymous == 0)
            {
                ViewBag.Idea_user = dbData.users.Where(u => u.user_id == uid).First();
            }

            dynamic mymodel = new ExpandoObject();
            mymodel.Comments = comments;
            mymodel.Comment_users = comment_users;

            
            return View(mymodel);
        }

        [HttpPost]
        public ActionResult Details()
        {
            //check logged in?
            if (Session["loggedIn"] == null)
            {
                return Redirect("~/Home/LoginPage");
            }

            comment comment = new comment();
            int idea_id = Convert.ToInt32(Request.Form["idea_id"].ToString());
            comment.idea_id = idea_id;
            comment.user_id = ((user)Session["loggedIn"]).user_id;
            comment.comment_content = Request.Form["comment_content"].ToString();
            comment.isEnabled = 1;
            comment.created_at = DateTime.Now;
            //check anonymous
            // 0 = false; 1 = true
            if (Request.Form["isAnonymous"] != null)
            {
                comment.isAnonymous = 1;
            }
            else
            {
                comment.isAnonymous = 0;
            }
            dbData.comments.Add(comment);
            dbData.SaveChanges();

            return Redirect(Url.Action("Details", "Idea", new { idea_id = idea_id }));
        }

        [HttpPost]
        public ActionResult Like()
        {
            //check logged in?
            if (Session["loggedIn"] == null)
            {
                return Redirect("~/Home/LoginPage");
            }

            int idea_id = Convert.ToInt32(Request.Form["idea_id"].ToString());
            int user_id = ((user)Session["loggedIn"]).user_id;
            rate rate;
            IQueryable<rate> rates = dbData.rates.Where(r => r.idea_id == idea_id).Where(r => r.user_id == user_id);
            if (rates.Any())
            {
                rate = rates.First();
                rate.rate_point = 1;
                rate.created_at = DateTime.Now;
            }
            else
            {
                rate = new rate();
                rate.user_id = user_id;
                rate.idea_id = idea_id;
                rate.rate_point = 1;
                rate.created_at = DateTime.Now;
                dbData.rates.Add(rate);
            }

            dbData.SaveChanges();
            return Redirect("~/Idea/Details?idea_id=" + idea_id);
        }

        [HttpPost]
        public ActionResult Dislike()
        {
            //check logged in?
            if (Session["loggedIn"] == null)
            {
                return Redirect("~/Home/LoginPage");
            }

            int idea_id = Convert.ToInt32(Request.Form["idea_id"].ToString());
            int user_id = ((user)Session["loggedIn"]).user_id;
            rate rate;
            IQueryable<rate> rates = dbData.rates.Where(r => r.idea_id == idea_id).Where(r => r.user_id == user_id);
            if (rates.Any())
            {
                rate = rates.First();
                rate.rate_point = -1;
                rate.created_at = DateTime.Now;
            }
            else
            {
                rate = new rate();
                rate.user_id = user_id;
                rate.idea_id = idea_id;
                rate.rate_point = -1;
                rate.created_at = DateTime.Now;
                dbData.rates.Add(rate);
            }
            dbData.SaveChanges();
            return Redirect("~/Idea/Details?idea_id=" + idea_id);
        }

        public FileResult DownloadAttachment(string new_file_name, string old_file_name)
        {
            string currentFile = "~/UploadedFiles/" + new_file_name;
            string contentType = "application/" + old_file_name.Split('.')[1];
            return File(currentFile, contentType, old_file_name);
        }


        public ActionResult Filter(int ? page, string idea_title)
        {
            //int category_id = Convert.ToInt32(Request.Form["categoryID"].ToString());
            List<display_idea> display_Ideas = getAllDisplayIdeas();
            if (idea_title != null)
            {
                List<display_idea> temp = new List<display_idea>();
                foreach(display_idea item in display_Ideas)
                {
                    if (!item.idea.idea_title.Contains(idea_title))
                    {
                        temp.Add(item);
                    }
                }
                foreach(display_idea item in temp)
                {
                    display_Ideas.Remove(item);
                }
                ViewBag.idea_title = idea_title;
            }
           
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View("Filter", display_Ideas.ToPagedList(pageNumber, pageSize));
        }
    }
}
