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
        // GET: Idea
        public ActionResult Index(int? page)
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
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(display_Ideas.ToPagedList(pageNumber, pageSize));
        }

        [HttpGet]
        public ActionResult Create()
        {
            List<category> categories = dbData.categories.ToList();
            SelectList listItems = new SelectList(categories, "category_id", "category_name");
            Session["cateCbb"] = listItems;
            return View();
        }

        [HttpPost]
        public ActionResult Create(idea idea, HttpPostedFileBase[] files)
        {
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
            return Redirect(Url.Action("Index", "Idea", new { page = 1}));
        }

        [HttpGet]
        public ActionResult Details(int idea_id)
        {
            user loggedIn = (user)Session["loggedIn"];

            idea idea = dbData.ideas.Where(i => i.idea_id == idea_id).First();
            int uid = Convert.ToInt32(idea.user_id);
            List<comment> comments = comments = dbData.comments.Where(c => c.idea_id == idea_id).ToList();
            List<comment> temp = new List<comment>();
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

            dynamic mymodel = new ExpandoObject();
            mymodel.Idea = idea;
            mymodel.Idea_user = dbData.users.Where(u => u.user_id == uid).First();
            mymodel.Comments = comments;
            mymodel.Comment_users = comment_users;

            return View(mymodel);
        }

        [HttpPost]
        public ActionResult Details()
        {
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
    }
}
