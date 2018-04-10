using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using WFPtest.Models;
using Microsoft.AspNet.Identity;
using PagedList;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.IO;
using Hangfire;
using System.Collections;
using System.DirectoryServices.AccountManagement;

namespace WFPtest.Controllers
{
    public class MissionItinerariesController : Controller
    {
        private WFPEntities1 db = new WFPEntities1();

        // GET: MissionItineraries
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, string node)
        {
            var result = from p in db.staffs
                         join a in db.staffs
                         on p.staffid equals a.staff_supervisorid


                         select new
                         {
                             staffSuperName = p.staff_first_name + " " + p.staff_last_name,


                         };

            ViewBag.newstaffSuperName = result;


            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = String.IsNullOrEmpty(sortOrder) ? "First_Name" : "";

            var students = from s in db.MissionItineraries
                           select s;


            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }


            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentSort = sortOrder;

            var missionItineraries = db.MissionItineraries.Include(m => m.country).Include(m => m.country1).Include(m => m.MissionAuthorization);
            //return View(missionItineraries.ToList());

            if (!String.IsNullOrEmpty(searchString))
            {
                missionItineraries = missionItineraries.Where(s => s .Equals(searchString)
                                       || s.country .Equals(searchString));

            }

            switch (sortOrder)
            {
                case "First_Name":
                    missionItineraries = missionItineraries.OrderByDescending(s => s.FromDate);
                    break;
                case "name_desc":
                    missionItineraries = missionItineraries.OrderByDescending(s => s.FromDate);
                    break;

                default:  // Name ascending 
                    missionItineraries = missionItineraries.OrderBy(s => s.FromDate);
                    break;
            }


            ViewBag.rdDrd = 1;
            ViewBag.FromID = new SelectList(db.countries, "countryid", "country_name");
            ViewBag.ToID = new SelectList(db.countries, "countryid", "country_name");
            ViewBag.MissionID = new SelectList(db.MissionAuthorizations, "MissionID", "MissionID");


            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(missionItineraries.ToPagedList(pageNumber, pageSize));

        }

        // GET: MissionItineraries/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MissionItinerary missionItinerary = db.MissionItineraries.Find(id);
            if (missionItinerary == null)
            {
                return HttpNotFound();
            }
            return View(missionItinerary);
        }

        // GET: MissionItineraries/Create
        public ActionResult Create()
        {
            ViewBag.FromID = new SelectList(db.countries, "countryid", "country_name");
            ViewBag.ToID = new SelectList(db.countries, "countryid", "country_name");


            var staffser = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
            staffser = staffser.Where(s => s.staff_email.Contains(User.Identity.Name + "@wfp.org"));
            var staffid = staffser.First().staffid;

            var staffmissions = db.MissionAuthorizations.Include(d => d.staff);
            staffmissions = staffmissions.Where(u => u.staffid == staffid);



            ViewBag.MissionID = new SelectList(staffmissions, "MissionID", "MissionID");
            //ViewBag.MissionID = 1117;
            return View();
        }

        public ActionResult Createitinary()
        {
            ViewBag.FromID = new SelectList(db.countries, "countryid", "country_name");
            ViewBag.ToID = new SelectList(db.countries, "countryid", "country_name");

            var staffser = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
            staffser = staffser.Where(s => s.staff_email.Contains(User.Identity.Name + "@wfp.org"));
            var staffid = staffser.First().staffid;

            var staffmissions = db.MissionAuthorizations.Include(d => d.staff);
            staffmissions = staffmissions.Where(u => u.staffid == staffid);



            ViewBag.MissionID = new SelectList(staffmissions, "MissionID", "MissionID");
            ViewBag.non3 = staffser.First().staffid;

            return View();
        }

        // POST: MissionItineraries/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleError]
        public ActionResult Create([Bind(Include = "ID,MissionID,Description,FromID,FromDate,ToID,ToDate,non1,non2,non3,non4,Fromtime,Totime")] MissionItinerary missionItinerary)
        {
            if (ModelState.IsValid)
            {
                db.MissionItineraries.Add(missionItinerary);
                db.SaveChanges();
                return RedirectToAction("Index", "MissionAuthorizations");
            }

            ViewBag.FromID = new SelectList(db.countries, "countryid", "country_name", missionItinerary.FromID);
            ViewBag.ToID = new SelectList(db.countries, "countryid", "country_name", missionItinerary.ToID);
            ViewBag.MissionID = new SelectList(db.MissionAuthorizations, "MissionID", "MissionID", missionItinerary.MissionID);
           //return View(missionItinerary);
           return RedirectToAction("Createitinary", "MissionAuthorizations");
            //return PartialView("_createiti");
        }

        // GET: MissionItineraries/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MissionItinerary missionItinerary = db.MissionItineraries.Find(id);
            if (missionItinerary == null)
            {
                return HttpNotFound();
            }
            ViewBag.FromID = new SelectList(db.countries, "countryid", "country_name", missionItinerary.FromID);
            ViewBag.ToID = new SelectList(db.countries, "countryid", "country_name", missionItinerary.ToID );
            ViewBag.MissionID = new SelectList(db.MissionAuthorizations, "MissionID", "MissionID", missionItinerary.MissionID);
            return View(missionItinerary);
           
        }

        // POST: MissionItineraries/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,MissionID,Description,FromID,FromDate,ToID,ToDate,non1,non2,non3,non4,Fromtime,Totime")] MissionItinerary missionItinerary)
        {
            if (ModelState.IsValid)
            {
                db.Entry(missionItinerary).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.FromID = new SelectList(db.countries, "countryid", "country_name", missionItinerary.FromID);
            ViewBag.ToID = new SelectList(db.countries, "countryid", "country_name", missionItinerary.ToID);
            ViewBag.MissionID = new SelectList(db.MissionAuthorizations, "MissionID", "MissionID", missionItinerary.MissionID);
            return View(missionItinerary);
        }

        // GET: MissionItineraries/Delete/5
        public ActionResult Delete(int id)
        {

            var staffser = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
            staffser = staffser.Where(s => s.staff_email.Contains(User.Identity.Name + "@wfp.org"));
            var staffid = staffser.First().staffid;

            var staffmissions = db.MissionItineraries.Include(d => d.MissionAuthorization);
            //staffmissions = staffmissions.Where(u => u.staffid == staffid);



            //ViewBag.MissionID = new SelectList(staffmissions, "MissionID", "MissionID");
            //if (missionItinerary == null)
            //{
            //    return HttpNotFound();
            //}
            return View(staffmissions);
        }

        // POST: MissionItineraries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MissionItinerary missionItinerary = db.MissionItineraries.Find(id);
            db.MissionItineraries.Remove(missionItinerary);
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
