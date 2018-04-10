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
    public class TRAINING_TYPEController : Controller
    {
        private WFPEntities1 db = new WFPEntities1();

        // GET: TRAINING_TYPE
        public ActionResult Index()
        {
            return View(db.TRAINING_TYPE.ToList());
        }

        // GET: TRAINING_TYPE/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TRAINING_TYPE tRAINING_TYPE = db.TRAINING_TYPE.Find(id);
            if (tRAINING_TYPE == null)
            {
                return HttpNotFound();
            }
            return View(tRAINING_TYPE);
        }

        // GET: TRAINING_TYPE/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TRAINING_TYPE/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "type_id,type_name,comment,typ01,type02,type22")] TRAINING_TYPE tRAINING_TYPE)
        {
            if (ModelState.IsValid)
            {
                db.TRAINING_TYPE.Add(tRAINING_TYPE);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tRAINING_TYPE);
        }

        // GET: TRAINING_TYPE/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TRAINING_TYPE tRAINING_TYPE = db.TRAINING_TYPE.Find(id);
            if (tRAINING_TYPE == null)
            {
                return HttpNotFound();
            }
            return View(tRAINING_TYPE);
        }

        // POST: TRAINING_TYPE/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "type_id,type_name,comment,typ01,type02,type22")] TRAINING_TYPE tRAINING_TYPE)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tRAINING_TYPE).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tRAINING_TYPE);
        }

        // GET: TRAINING_TYPE/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TRAINING_TYPE tRAINING_TYPE = db.TRAINING_TYPE.Find(id);
            if (tRAINING_TYPE == null)
            {
                return HttpNotFound();
            }
            return View(tRAINING_TYPE);
        }

        // POST: TRAINING_TYPE/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            TRAINING_TYPE tRAINING_TYPE = db.TRAINING_TYPE.Find(id);
            db.TRAINING_TYPE.Remove(tRAINING_TYPE);
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
