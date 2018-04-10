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
    public class requests11Controller : Controller
    {
        private WFPEntities1 db = new WFPEntities1();

        // GET: requests11
        public ActionResult Index()
        {
            return View(db.requests1.ToList());
        }

        // GET: requests11/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            requests1 requests1 = db.requests1.Find(id);
            if (requests1 == null)
            {
                return HttpNotFound();
            }
            return View(requests1);
        }

        // GET: requests11/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: requests11/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,RequstingBy,RequestingDate,RequestingUnit,FocalPoint,Purpose,FromDate,ToDate,NumOFP_CO,NumOFP_RB,NumOFP_HQ,NumOFP_Other,Country,GOV,District,OtherPlace,Budget_TR,Number_Roms,Check_inDate,Check_OutDate,Rom_Type,Microphone,HiSpeedInternet,Conf_Call_Device,MMR_Days,BOR_Days,COffeBreak,COffeBreak_UNM,Lunch,Dinner,Coctail_Reception,Dedicated_IT_SUPP,DescripDaysORHours,Transportstion_ForLocal_Staff,Airport_Picup_Services,Other_Services,non1,non2,non3")] requests1 requests1)
        {
            if (ModelState.IsValid)
            {
                db.requests1.Add(requests1);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(requests1);
        }

        // GET: requests11/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            requests1 requests1 = db.requests1.Find(id);
            if (requests1 == null)
            {
                return HttpNotFound();
            }
            return View(requests1);
        }

        // POST: requests11/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,RequstingBy,RequestingDate,RequestingUnit,FocalPoint,Purpose,FromDate,ToDate,NumOFP_CO,NumOFP_RB,NumOFP_HQ,NumOFP_Other,Country,GOV,District,OtherPlace,Budget_TR,Number_Roms,Check_inDate,Check_OutDate,Rom_Type,Microphone,HiSpeedInternet,Conf_Call_Device,MMR_Days,BOR_Days,COffeBreak,COffeBreak_UNM,Lunch,Dinner,Coctail_Reception,Dedicated_IT_SUPP,DescripDaysORHours,Transportstion_ForLocal_Staff,Airport_Picup_Services,Other_Services,non1,non2,non3")] requests1 requests1)
        {
            if (ModelState.IsValid)
            {
                db.Entry(requests1).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(requests1);
        }

        // GET: requests11/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            requests1 requests1 = db.requests1.Find(id);
            if (requests1 == null)
            {
                return HttpNotFound();
            }
            return View(requests1);
        }

        // POST: requests11/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            requests1 requests1 = db.requests1.Find(id);
            db.requests1.Remove(requests1);
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
