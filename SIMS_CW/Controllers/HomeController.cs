using SIMS_CW.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
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

        public ActionResult MockedGraph()
        {
            List<string> department = new List<string>();
            List<int> idealCount = new List<int>();

            using (DbModel db = new DbModel())
            {
                foreach(department departmant in db.departments){
                    department.Add(departmant.department_name);
                    int numberofideas = 0;
                    foreach(user user in departmant.users)
                    {
                        numberofideas += user.ideas.Count;
                    }
                    idealCount.Add(numberofideas);
                }
            }
                var chart = new Chart(800, 400)
                    .AddTitle("title")
                    .AddSeries(
                        name: "Mocked Graph",
                        //chartType:"pie",
                        xValue: department.ToArray(),
                        yValues: idealCount.ToArray()
                    ).Write("bmp");
            return null;
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
                    switch (user.role_id)
                    {
                        case 1:
                            //admin
                            break;
                        case 2:
                            //QA Manager
                            break;
                        case 3:
                            // QA Coordinator
                            break;
                        case 4:
                            //Staff
                            return Redirect(Url.Action("Index", "Idea"));
                        case 5:
                            //Student
                            return Redirect(Url.Action("Index", "Idea"));
                    }
                }
                
            }
            return View();


        }
    }
}