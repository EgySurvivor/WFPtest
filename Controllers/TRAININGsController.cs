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
    public class TRAININGsController : Controller
    {
        private WFPEntities1 db = new WFPEntities1();

        // GET: TRAININGs
        public ActionResult Index()
        {
            var tRAININGS = db.TRAININGS.Include(t => t.TRAINING1);
            return View(tRAININGS.ToList());
        }

        // GET: TRAININGs/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TRAINING tRAINING = db.TRAININGS.Find(id);
            if (tRAINING == null)
            {
                return HttpNotFound();
            }
            return View(tRAINING);
        }

        // GET: TRAININGs/Create
        public ActionResult Create()
        {
            ViewBag.PARENT_TRAINING_ID = new SelectList(db.TRAININGS, "TRAINING_ID", "TRAINING_NAME");
            return View();
        }

        // POST: TRAININGs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TRAINING_ID,TRAINING_NAME,TRAINING_DESC,TRAINING_PLACE,TRAINING_PERIOD,PARENT_TRAINING_ID")] TRAINING tRAINING)
        {
            if (ModelState.IsValid)
            {
                db.TRAININGS.Add(tRAINING);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PARENT_TRAINING_ID = new SelectList(db.TRAININGS, "TRAINING_ID", "TRAINING_NAME", tRAINING.PARENT_TRAINING_ID);
            return View(tRAINING);
        }

        // GET: TRAININGs/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TRAINING tRAINING = db.TRAININGS.Find(id);
            if (tRAINING == null)
            {
                return HttpNotFound();
            }
            ViewBag.PARENT_TRAINING_ID = new SelectList(db.TRAININGS, "TRAINING_ID", "TRAINING_NAME", tRAINING.PARENT_TRAINING_ID);
            return View(tRAINING);
        }

        // POST: TRAININGs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TRAINING_ID,TRAINING_NAME,TRAINING_DESC,TRAINING_PLACE,TRAINING_PERIOD,PARENT_TRAINING_ID")] TRAINING tRAINING)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tRAINING).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PARENT_TRAINING_ID = new SelectList(db.TRAININGS, "TRAINING_ID", "TRAINING_NAME", tRAINING.PARENT_TRAINING_ID);
            return View(tRAINING);
        }

        // GET: TRAININGs/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TRAINING tRAINING = db.TRAININGS.Find(id);
            if (tRAINING == null)
            {
                return HttpNotFound();
            }
            return View(tRAINING);
        }

        // POST: TRAININGs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            TRAINING tRAINING = db.TRAININGS.Find(id);
            db.TRAININGS.Remove(tRAINING);
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
