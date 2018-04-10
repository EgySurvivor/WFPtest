using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WFPtest.Models;

namespace WFPtest.Controllers
{
    public class TRAINING_PLAN_TRAININGSController : Controller
    {
        private WFPEntities1 db = new WFPEntities1();

        // GET: TRAINING_PLAN_TRAININGS
        public ActionResult Index()
        {
            var tRAINING_PLAN_TRAININGS = db.TRAINING_PLAN_TRAININGS.Include(t => t.TRAINING_PLAN).Include(t => t.TRAINING_TYPE1);
            return View(tRAINING_PLAN_TRAININGS.ToList());
        }

        // GET: TRAINING_PLAN_TRAININGS/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TRAINING_PLAN_TRAININGS tRAINING_PLAN_TRAININGS = db.TRAINING_PLAN_TRAININGS.Find(id);
            if (tRAINING_PLAN_TRAININGS == null)
            {
                return HttpNotFound();
            }
            return View(tRAINING_PLAN_TRAININGS);
        }

        // GET: TRAINING_PLAN_TRAININGS/Create
        public ActionResult Create()
        {
            ViewBag.PLAN_ID = new SelectList(db.TRAINING_PLAN, "PLAN_ID", "TRAINING_PLACE");
            ViewBag.TRAINING_TYPE = new SelectList(db.TRAINING_TYPE, "type_id", "type_name");
            ViewBag.TRAINING_ID = new SelectList(db.TRAININGS, "TRAINING_ID", "TRAINING_ID");
            return View();
        }

        // POST: TRAINING_PLAN_TRAININGS/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PLAN_ID,TRAINING_ID,REQUIRE_CERTIFICATE,CERTIFICATE_ID,COASTPEREMP,EMPLPERCENTAGE,FAILPASSDEPENDANCY,IFFAILPERCENTAGE,IFPASSPERCENTAGE,TRAINING_TYPE")] TRAINING_PLAN_TRAININGS tRAINING_PLAN_TRAININGS)
        {
            if (ModelState.IsValid)
            {
                db.TRAINING_PLAN_TRAININGS.Add(tRAINING_PLAN_TRAININGS);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PLAN_ID = new SelectList(db.TRAINING_PLAN, "PLAN_ID", "TRAINING_PLACE", tRAINING_PLAN_TRAININGS.PLAN_ID);
            ViewBag.TRAINING_TYPE = new SelectList(db.TRAINING_TYPE, "type_id", "type_name", tRAINING_PLAN_TRAININGS.TRAINING_TYPE);
            return View(tRAINING_PLAN_TRAININGS);
        }

        // GET: TRAINING_PLAN_TRAININGS/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TRAINING_PLAN_TRAININGS tRAINING_PLAN_TRAININGS = db.TRAINING_PLAN_TRAININGS.Find(id);
            if (tRAINING_PLAN_TRAININGS == null)
            {
                return HttpNotFound();
            }
            ViewBag.PLAN_ID = new SelectList(db.TRAINING_PLAN, "PLAN_ID", "TRAINING_PLACE", tRAINING_PLAN_TRAININGS.PLAN_ID);
            ViewBag.TRAINING_TYPE = new SelectList(db.TRAINING_TYPE, "type_id", "type_name", tRAINING_PLAN_TRAININGS.TRAINING_TYPE);
            return View(tRAINING_PLAN_TRAININGS);
        }

        // POST: TRAINING_PLAN_TRAININGS/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PLAN_ID,TRAINING_ID,REQUIRE_CERTIFICATE,CERTIFICATE_ID,COASTPEREMP,EMPLPERCENTAGE,FAILPASSDEPENDANCY,IFFAILPERCENTAGE,IFPASSPERCENTAGE,TRAINING_TYPE")] TRAINING_PLAN_TRAININGS tRAINING_PLAN_TRAININGS)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tRAINING_PLAN_TRAININGS).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PLAN_ID = new SelectList(db.TRAINING_PLAN, "PLAN_ID", "TRAINING_PLACE", tRAINING_PLAN_TRAININGS.PLAN_ID);
            ViewBag.TRAINING_TYPE = new SelectList(db.TRAINING_TYPE, "type_id", "type_name", tRAINING_PLAN_TRAININGS.TRAINING_TYPE);
            return View(tRAINING_PLAN_TRAININGS);
        }

        // GET: TRAINING_PLAN_TRAININGS/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TRAINING_PLAN_TRAININGS tRAINING_PLAN_TRAININGS = db.TRAINING_PLAN_TRAININGS.Find(id);
            if (tRAINING_PLAN_TRAININGS == null)
            {
                return HttpNotFound();
            }
            return View(tRAINING_PLAN_TRAININGS);
        }

        // POST: TRAINING_PLAN_TRAININGS/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            TRAINING_PLAN_TRAININGS tRAINING_PLAN_TRAININGS = db.TRAINING_PLAN_TRAININGS.Find(id);
            db.TRAINING_PLAN_TRAININGS.Remove(tRAINING_PLAN_TRAININGS);
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
