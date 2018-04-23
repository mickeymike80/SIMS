using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using SIMS_CW.Models;

namespace SIMS_CW.Controllers
{
    public class AdminAccountController : Controller
    {
        private DbModel db = new DbModel();

        // GET: AdminAccount
        public ActionResult Index()
        {
            var users = db.users.Include(u => u.department).Include(u => u.role);
            return View(users.ToList());
        }

        // GET: AdminAccount/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            user user = db.users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: AdminAccount/Create
        public ActionResult Create()
        {
            ViewBag.department_id = new SelectList(db.departments, "department_id", "department_name");
            ViewBag.role_id = new SelectList(db.roles, "role_id", "role_name");
            return View();
        }

        // POST: AdminAccount/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(user user)
        {
            if (ModelState.IsValid)
            {
                user.created_at = DateTime.Now;
                string guid = Guid.NewGuid().ToString();
                string password = guid.Split('-')[0];
                user.password = password;
                db.users.Add(user);
                db.SaveChanges();

                // send email about account details
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
                String EmailSubject = "Account Details";
                String EMailBody = "Email: " + user.email +"\nPassword: "+ password + "\nLink: <a href='http://localhost:52547/Home/LoginPage'>Click here</a>";
                //Send email  
                WebMail.Send(to: ToEmail, subject: EmailSubject, body: EMailBody, isBodyHtml: true);
                return RedirectToAction("Index");
            }

            ViewBag.department_id = new SelectList(db.departments, "department_id", "department_name", user.department_id);
            ViewBag.role_id = new SelectList(db.roles, "role_id", "role_name", user.role_id);
            return View(user);
        }

        // GET: AdminAccount/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            user user = db.users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.department_id = new SelectList(db.departments, "department_id", "department_name", user.department_id);
            ViewBag.role_id = new SelectList(db.roles, "role_id", "role_name", user.role_id);
            return View(user);
        }

        // POST: AdminAccount/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "user_id,user_university_id,department_id,role_id,user_name,email,password,created_at")] user user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.department_id = new SelectList(db.departments, "department_id", "department_name", user.department_id);
            ViewBag.role_id = new SelectList(db.roles, "role_id", "role_name", user.role_id);
            return View(user);
        }

        // GET: AdminAccount/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            user user = db.users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: AdminAccount/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            user user = db.users.Find(id);
            db.users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
