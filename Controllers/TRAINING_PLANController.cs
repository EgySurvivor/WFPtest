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
    public class TRAINING_PLANController : Controller
    {
        private WFPEntities1 db = new WFPEntities1();

        // GET: TRAINING_PLAN
        public ActionResult Index()
        {
            return View(db.TRAINING_PLAN.ToList());
        }

        // GET: TRAINING_PLAN/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TRAINING_PLAN tRAINING_PLAN = db.TRAINING_PLAN.Find(id);
            if (tRAINING_PLAN == null)
            {
                return HttpNotFound();
            }
            return View(tRAINING_PLAN);
        }

        // GET: TRAINING_PLAN/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TRAINING_PLAN/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PLAN_ID,TRAINING_PLACE,PERIOD_FROM,PERIOD_TO")] TRAINING_PLAN tRAINING_PLAN)
        {
            if (ModelState.IsValid)
            {
                db.TRAINING_PLAN.Add(tRAINING_PLAN);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tRAINING_PLAN);
        }

        // GET: TRAINING_PLAN/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TRAINING_PLAN tRAINING_PLAN = db.TRAINING_PLAN.Find(id);
            if (tRAINING_PLAN == null)
            {
                return HttpNotFound();
            }
            return View(tRAINING_PLAN);
        }

        // POST: TRAINING_PLAN/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PLAN_ID,TRAINING_PLACE,PERIOD_FROM,PERIOD_TO")] TRAINING_PLAN tRAINING_PLAN)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tRAINING_PLAN).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tRAINING_PLAN);
        }

        // GET: TRAINING_PLAN/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TRAINING_PLAN tRAINING_PLAN = db.TRAINING_PLAN.Find(id);
            if (tRAINING_PLAN == null)
            {
                return HttpNotFound();
            }
            return View(tRAINING_PLAN);
        }

        // POST: TRAINING_PLAN/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            TRAINING_PLAN tRAINING_PLAN = db.TRAINING_PLAN.Find(id);
            db.TRAINING_PLAN.Remove(tRAINING_PLAN);
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
