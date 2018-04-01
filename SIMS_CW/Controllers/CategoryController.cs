using SIMS_CW.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SIMS_CW.Controllers
{
    public class CategoryController : Controller
    {
        DbModel dbData = new DbModel();

        public ActionResult GetCategory()
        {
            List<category> categories = dbData.categories.ToList();
            return View(categories);

        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(category category)
        {
            if (ModelState.IsValid)
            {
                category.created_at = DateTime.Now;
                dbData.categories.Add(category);
                dbData.SaveChanges();
                return Redirect(Url.Action("Index", "Manager"));
            }
            else
            {
                ViewBag.error_message = "woops, error occured";
                return View();
            }
        }

        public ActionResult Category(string searchString)
        {

            var categories = from c in dbData.categories
                             select c;

            if (!String.IsNullOrEmpty(searchString))
            {
                categories = categories.Where(s => s.category_name.Contains(searchString));
            }
            return View(categories);

        }

        //
        // GET: /Movies/Edit/5

        public ActionResult Edit(int id = 0)
        {
            category category = dbData.categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return Redirect(Url.Action("Index", "Manager"));
        }

        //
        // POST: /Movies/Edit/5

        [HttpPost]
        public ActionResult Edit(category category)
        {
            if (ModelState.IsValid)
            {
                dbData.Entry(category).State = EntityState.Modified;
                dbData.SaveChanges();
                return Redirect(Url.Action("Index", "Manager"));
            }
            return View(category);
        }

    }
}
