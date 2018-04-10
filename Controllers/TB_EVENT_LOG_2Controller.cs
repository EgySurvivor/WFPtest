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
    public class TB_EVENT_LOG_2Controller : Controller
    {
        private BioStarEntitiesWFP db = new BioStarEntitiesWFP();

        // GET: TB_EVENT_LOG_2
        public ActionResult Index()
        {

            return View(db.TB_EVENT_LOG_2.ToList().Where(l => l.nUserID == 8835203 && l.nTNAEvent == 1 && l.date.Value.Month == DateTime.Now.Month && l.date.Value.Year == DateTime.Now.Year).OrderBy(l => l.date));
           
        }

        // GET: TB_EVENT_LOG_2/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TB_EVENT_LOG_2 tB_EVENT_LOG_2 = db.TB_EVENT_LOG_2.Find(id);
            if (tB_EVENT_LOG_2 == null)
            {
                return HttpNotFound();
            }
            return View(tB_EVENT_LOG_2);
        }

        // GET: TB_EVENT_LOG_2/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TB_EVENT_LOG_2/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "nEventLogIdn,nDateTime,nReaderIdn,nEventIdn,nUserID,nIsLog,nTNAEvent,nIsUseTA,nType,date")] TB_EVENT_LOG_2 tB_EVENT_LOG_2)
        {
            if (ModelState.IsValid)
            {
                db.TB_EVENT_LOG_2.Add(tB_EVENT_LOG_2);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tB_EVENT_LOG_2);
        }

        // GET: TB_EVENT_LOG_2/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TB_EVENT_LOG_2 tB_EVENT_LOG_2 = db.TB_EVENT_LOG_2.Find(id);
            if (tB_EVENT_LOG_2 == null)
            {
                return HttpNotFound();
            }
            return View(tB_EVENT_LOG_2);
        }

        // POST: TB_EVENT_LOG_2/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "nEventLogIdn,nDateTime,nReaderIdn,nEventIdn,nUserID,nIsLog,nTNAEvent,nIsUseTA,nType,date")] TB_EVENT_LOG_2 tB_EVENT_LOG_2)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tB_EVENT_LOG_2).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tB_EVENT_LOG_2);
        }

        // GET: TB_EVENT_LOG_2/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TB_EVENT_LOG_2 tB_EVENT_LOG_2 = db.TB_EVENT_LOG_2.Find(id);
            if (tB_EVENT_LOG_2 == null)
            {
                return HttpNotFound();
            }
            return View(tB_EVENT_LOG_2);
        }

        // POST: TB_EVENT_LOG_2/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TB_EVENT_LOG_2 tB_EVENT_LOG_2 = db.TB_EVENT_LOG_2.Find(id);
            db.TB_EVENT_LOG_2.Remove(tB_EVENT_LOG_2);
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
