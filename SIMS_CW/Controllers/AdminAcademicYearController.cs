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
    public class AdminAcademicYearController : Controller
    {
        private DbModel db = new DbModel();

        // GET: AdminAcademicYear
        public ActionResult Index()
        {
            return View(db.academic_years.ToList());
        }

        // GET: AdminAcademicYear/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            academic_years academic_years = db.academic_years.Find(id);
            if (academic_years == null)
            {
                return HttpNotFound();
            }
            return View(academic_years);
        }

        // GET: AdminAcademicYear/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AdminAcademicYear/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "academic_year_id,academic_year_name,started_at,ended_at,deadline_ideas,deadline_comments")] academic_years academic_years)
        {
            if (ModelState.IsValid)
            {
                db.academic_years.Add(academic_years);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(academic_years);
        }

        // GET: AdminAcademicYear/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            academic_years academic_years = db.academic_years.Find(id);
            if (academic_years == null)
            {
                return HttpNotFound();
            }
            return View(academic_years);
        }

        // POST: AdminAcademicYear/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "academic_year_id,academic_year_name,started_at,ended_at,deadline_ideas,deadline_comments")] academic_years academic_years)
        {
            if (ModelState.IsValid)
            {
                db.Entry(academic_years).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(academic_years);
        }

        // GET: AdminAcademicYear/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            academic_years academic_years = db.academic_years.Find(id);
            if (academic_years == null)
            {
                return HttpNotFound();
            }
            return View(academic_years);
        }

        // POST: AdminAcademicYear/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            academic_years academic_years = db.academic_years.Find(id);
            db.academic_years.Remove(academic_years);
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
