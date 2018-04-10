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
    public class MissionTypesController : Controller
    {
        private WFPEntities1 db = new WFPEntities1();

        // GET: MissionTypes
        public ActionResult Index()
        {
            return View(db.MissionTypes.ToList());
        }

        // GET: MissionTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MissionType missionType = db.MissionTypes.Find(id);
            if (missionType == null)
            {
                return HttpNotFound();
            }
            return View(missionType);
        }

        // GET: MissionTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MissionTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,Description,Non1,Non2,Non3")] MissionType missionType)
        {
            if (ModelState.IsValid)
            {
                db.MissionTypes.Add(missionType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(missionType);
        }

        // GET: MissionTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MissionType missionType = db.MissionTypes.Find(id);
            if (missionType == null)
            {
                return HttpNotFound();
            }
            return View(missionType);
        }

        // POST: MissionTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,Description,Non1,Non2,Non3")] MissionType missionType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(missionType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(missionType);
        }

        // GET: MissionTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MissionType missionType = db.MissionTypes.Find(id);
            if (missionType == null)
            {
                return HttpNotFound();
            }
            return View(missionType);
        }

        // POST: MissionTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MissionType missionType = db.MissionTypes.Find(id);
            db.MissionTypes.Remove(missionType);
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
