using SIMS_CW.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace SIMS_CW.Controllers
{
    public class IdeaController : Controller
    {
        DbModel dbData = new DbModel();
        // GET: Idea
        public ActionResult Index()
        {

            return View();
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
            // 0 = true; 1 = false
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
            //idea.user_id = ((user)Session["loggedIn"]).user_id;
            //check anonymous
            if (Request.Form["isAnonymous"] != null)
            {
                idea.isAnonymous = 0;
            }
            else
            {
                idea.isAnonymous = 1;
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
            return View();
        }
    }
}
