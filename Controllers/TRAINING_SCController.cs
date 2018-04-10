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
    public class TRAINING_SCController : Controller
    {
        private WFPEntities1 db = new WFPEntities1();

        // GET: TRAINING_SC
        public ActionResult Index()
        {
            var tRAINING_SC = db.TRAINING_SC.Include(t => t.TRAINING_PLAN);
            return View(tRAINING_SC.ToList());
        }

        // GET: TRAINING_SC/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TRAINING_SC tRAINING_SC = db.TRAINING_SC.Find(id);
            if (tRAINING_SC == null)
            {
                return HttpNotFound();
            }
            return View(tRAINING_SC);
        }

        // GET: TRAINING_SC/Create
        public ActionResult Create()
        {
            ViewBag.PLAN_ID = new SelectList(db.TRAINING_PLAN, "PLAN_ID", "TRAINING_PLACE");
            return View();
        }

        // POST: TRAINING_SC/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "sc_id,from_time,to_time,EMP_ID,attend,PLAN_ID,time_open,extra1,extra2,extra3,time_closed,comment")] TRAINING_SC tRAINING_SC)
        {
            if (ModelState.IsValid)
            {
                db.TRAINING_SC.Add(tRAINING_SC);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PLAN_ID = new SelectList(db.TRAINING_PLAN, "PLAN_ID", "TRAINING_PLACE", tRAINING_SC.PLAN_ID);
            return View(tRAINING_SC);
        }

        // GET: TRAINING_SC/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TRAINING_SC tRAINING_SC = db.TRAINING_SC.Find(id);
            if (tRAINING_SC == null)
            {
                return HttpNotFound();
            }
            ViewBag.PLAN_ID = new SelectList(db.TRAINING_PLAN, "PLAN_ID", "TRAINING_PLACE", tRAINING_SC.PLAN_ID);
            return View(tRAINING_SC);
        }

        // POST: TRAINING_SC/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "sc_id,from_time,to_time,EMP_ID,attend,PLAN_ID,time_open,extra1,extra2,extra3,time_closed,comment")] TRAINING_SC tRAINING_SC)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tRAINING_SC).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PLAN_ID = new SelectList(db.TRAINING_PLAN, "PLAN_ID", "TRAINING_PLACE", tRAINING_SC.PLAN_ID);
            return View(tRAINING_SC);
        }

        // GET: TRAINING_SC/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TRAINING_SC tRAINING_SC = db.TRAINING_SC.Find(id);
            if (tRAINING_SC == null)
            {
                return HttpNotFound();
            }
            return View(tRAINING_SC);
        }

        // POST: TRAINING_SC/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            TRAINING_SC tRAINING_SC = db.TRAINING_SC.Find(id);
            db.TRAINING_SC.Remove(tRAINING_SC);
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
