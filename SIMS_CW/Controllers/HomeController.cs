using SIMS_CW.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SIMS_CW.Controllers
{
    public class HomeController : Controller
    {
        DbModel dbModel = new DbModel();

      
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult Error()
        {
            return View();
        }

        [HttpGet]
        public ActionResult LoginPage()
        {
            return View("Login");
        }

        [HttpPost]
        public ActionResult Login()
        {
            String email = Request.Form["email"].ToString();
            String password = Request.Form["password"].ToString();

            List<user> users = dbModel.users.ToList();
            foreach (user user in users)
            {
                if(user.email.ToLower().Equals(email.ToLower()) && user.password.Equals(password))
                {
                    Session["loggedIn"] = user;
                    Session["liRole"] = user.role_id;
                    switch (user.role_id)
                    {
                        case 1:
                            //admin
                            return Redirect(Url.Action("Index", "AdminAccount"));
                        case 2:
                            //QA Manager
                            return Redirect(Url.Action("Index", "Manager"));
                        case 3:
                            // QA Coordinator
                            return Redirect(Url.Action("Index", "Manager"));
                        case 4:
                            //Staff
                            return Redirect(Url.Action("Index", "Idea"));
                        case 5:
                            //Student
                            return Redirect(Url.Action("Index", "Idea"));
                    }
                }
                
            }
            ViewBag.error = "Incorrect email or password. Please check again!";
            return View("Login");
        }

        [HttpGet]
        public ActionResult Logout()
        {
            Session.Clear();
            return View("Index");
        }
    }
}