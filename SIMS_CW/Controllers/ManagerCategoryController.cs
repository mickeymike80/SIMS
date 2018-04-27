using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SIMS_CW.Models;

namespace SIMS_CW.Controllers
{
    public class ManagerCategoryController : Controller
    {
        private DbModel db = new DbModel();

        // GET: ManagerCategory
        public ActionResult Index()
        {
            user loggedIn = (user)Session["loggedIn"];

            //check logged in?
            if (loggedIn == null)
            {
                return Redirect("~/Home/LoginPage");
            }
            if (loggedIn.role.role_id < 1 || loggedIn.role.role_id > 2)
            {
                return Redirect("~/Home/DeniedAccess");
            }

            return View(db.categories.ToList());
        }

        // GET: ManagerCategory/Details/5
        public ActionResult Details(int? id)
        {
            user loggedIn = (user)Session["loggedIn"];

            //check logged in?
            if (loggedIn == null)
            {
                return Redirect("~/Home/LoginPage");
            }
            if (loggedIn.role.role_id < 1 || loggedIn.role.role_id > 2)
            {
                return Redirect("~/Home/DeniedAccess");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            category category = db.categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // GET: ManagerCategory/Create
        public ActionResult Create()
        {
            user loggedIn = (user)Session["loggedIn"];

            //check logged in?
            if (loggedIn == null)
            {
                return Redirect("~/Home/LoginPage");
            }
            if (loggedIn.role.role_id < 1 || loggedIn.role.role_id > 2)
            {
                return Redirect("~/Home/DeniedAccess");
            }

            return View();
        }

        // POST: ManagerCategory/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "category_id,category_name")] category category)
        {
            user loggedIn = (user)Session["loggedIn"];

            //check logged in?
            if (loggedIn == null)
            {
                return Redirect("~/Home/LoginPage");
            }
            if (loggedIn.role.role_id < 1 || loggedIn.role.role_id > 2)
            {
                return Redirect("~/Home/DeniedAccess");
            }

            if (ModelState.IsValid)
            {
                category.created_at = DateTime.Now;
                db.categories.Add(category);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(category);
        }

        // GET: ManagerCategory/Edit/5
        public ActionResult Edit(int? id)
        {
            user loggedIn = (user)Session["loggedIn"];

            //check logged in?
            if (loggedIn == null)
            {
                return Redirect("~/Home/LoginPage");
            }
            if (loggedIn.role.role_id < 1 || loggedIn.role.role_id > 2)
            {
                return Redirect("~/Home/DeniedAccess");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            category category = db.categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // POST: ManagerCategory/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "category_id,category_name,created_at")] category category)
        {
            user loggedIn = (user)Session["loggedIn"];

            //check logged in?
            if (loggedIn == null)
            {
                return Redirect("~/Home/LoginPage");
            }
            if (loggedIn.role.role_id < 1 || loggedIn.role.role_id > 2)
            {
                return Redirect("~/Home/DeniedAccess");
            }

            if (ModelState.IsValid)
            {
                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(category);
        }

        // GET: ManagerCategory/Delete/5
        public ActionResult Delete(int? id)
        {
            user loggedIn = (user)Session["loggedIn"];

            //check logged in?
            if (loggedIn == null)
            {
                return Redirect("~/Home/LoginPage");
            }
            if (loggedIn.role.role_id < 1 || loggedIn.role.role_id > 2)
            {
                return Redirect("~/Home/DeniedAccess");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            category category = db.categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // POST: ManagerCategory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            user loggedIn = (user)Session["loggedIn"];

            //check logged in?
            if (loggedIn == null)
            {
                return Redirect("~/Home/LoginPage");
            }
            if (loggedIn.role.role_id < 1 || loggedIn.role.role_id > 2)
            {
                return Redirect("~/Home/DeniedAccess");
            }

            category category = db.categories.Find(id);
            db.categories.Remove(category);
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
