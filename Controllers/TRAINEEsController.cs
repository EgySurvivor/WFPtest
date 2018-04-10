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
    public class TRAINEEsController : Controller
    {
        private WFPEntities1 db = new WFPEntities1();

        // GET: TRAINEEs
        public ActionResult Index()
        {
            var tRAINEES = db.TRAINEES.Include(t => t.TRAINING_PLAN_TRAINING_CLASSES).Include(t => t.TRAINING);
            return View(tRAINEES.ToList());
        }

        // GET: TRAINEEs/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TRAINEE tRAINEE = db.TRAINEES.Find(id);
            if (tRAINEE == null)
            {
                return HttpNotFound();
            }
            return View(tRAINEE);
        }

        // GET: TRAINEEs/Create
        public ActionResult Create()
        {
            ViewBag.PLAN_ID = new SelectList(db.TRAINING_PLAN_TRAINING_CLASSES, "PLAN_ID", "CLASS_NAME");
            ViewBag.TRAINING_ID = new SelectList(db.TRAININGS, "TRAINING_ID", "TRAINING_NAME");
            ViewBag.EMP_ID = new SelectList(db.staffs, "staffid", "staff_email");
            return View();
        }

        // POST: TRAINEEs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PLAN_ID,TRAINING_ID,EMP_ID,ATTENDED,PASSED,TRAINING_EVALUATION,TRAINER_EVALUATION,CLASS_ID")] TRAINEE tRAINEE)
        {
            if (ModelState.IsValid)
            {
                db.TRAINEES.Add(tRAINEE);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PLAN_ID = new SelectList(db.TRAINING_PLAN_TRAINING_CLASSES, "PLAN_ID", "CLASS_NAME", tRAINEE.PLAN_ID);
            ViewBag.TRAINING_ID = new SelectList(db.TRAININGS, "TRAINING_ID", "TRAINING_NAME", tRAINEE.TRAINING_ID);
            return View(tRAINEE);
        }

        // GET: TRAINEEs/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TRAINEE tRAINEE = db.TRAINEES.Find(id);
            if (tRAINEE == null)
            {
                return HttpNotFound();
            }
            ViewBag.PLAN_ID = new SelectList(db.TRAINING_PLAN_TRAINING_CLASSES, "PLAN_ID", "CLASS_NAME", tRAINEE.PLAN_ID);
            ViewBag.TRAINING_ID = new SelectList(db.TRAININGS, "TRAINING_ID", "TRAINING_NAME", tRAINEE.TRAINING_ID);
            return View(tRAINEE);
        }

        // POST: TRAINEEs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PLAN_ID,TRAINING_ID,EMP_ID,ATTENDED,PASSED,TRAINING_EVALUATION,TRAINER_EVALUATION,CLASS_ID")] TRAINEE tRAINEE)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tRAINEE).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PLAN_ID = new SelectList(db.TRAINING_PLAN_TRAINING_CLASSES, "PLAN_ID", "CLASS_NAME", tRAINEE.PLAN_ID);
            ViewBag.TRAINING_ID = new SelectList(db.TRAININGS, "TRAINING_ID", "TRAINING_NAME", tRAINEE.TRAINING_ID);
            return View(tRAINEE);
        }

        // GET: TRAINEEs/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TRAINEE tRAINEE = db.TRAINEES.Find(id);
            if (tRAINEE == null)
            {
                return HttpNotFound();
            }
            return View(tRAINEE);
        }

        // POST: TRAINEEs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            TRAINEE tRAINEE = db.TRAINEES.Find(id);
            db.TRAINEES.Remove(tRAINEE);
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
