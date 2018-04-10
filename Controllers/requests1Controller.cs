using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WFPtest.Models;
using Microsoft.AspNet.Identity;
using PagedList;
using System.Diagnostics;
using System.Net.Mail;

namespace WFPtest.Controllers
{
    public class requests1Controller : Controller
    {
        private WFPEntities1 db = new WFPEntities1();

        // GET: requests1
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            var result = from p in db.requests1
                         join a in db.requests1
                         on p.Purpose  equals a.Purpose 


                         select new
                         {
                             staffSuperName = p.Purpose  + " " + p.Purpose,


                         };

            ViewBag.newstaffSuperName = result;


            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = String.IsNullOrEmpty(sortOrder) ? "First_Name" : "";

            var students = from s in db.requests1
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

            var staffser = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
            staffser = staffser.Where(s => s.staff_email.Contains(User.Identity.Name + "@wfp.org"));
            var staffid = staffser.First().staffid;


            var requests1 = db.requests1.Include(r => r.).Include(r => r.staff).Include(r => r.staff1).Include(r => r.unit1);
            requests1 = requests1.Where(r => r.requested_by == (staffid) || r.supervisor_email == (staffid));




            if (!String.IsNullOrEmpty(searchString))
            {
                requests = requests.Where(s => s.first_name.Equals(searchString)
                                       || s.last_name.Equals(searchString));

            }

            switch (sortOrder)
            {
                case "First_Name":
                    requests = requests.OrderByDescending(s => s.first_name);
                    break;
                case "name_desc":
                    requests = requests.OrderByDescending(s => s.last_name);
                    break;

                default:  // Name ascending 
                    requests = requests.OrderBy(s => s.first_name);
                    break;
            }




            int pageSize = 10;
            int pageNumber = (page ?? 1);


            return View(requests.ToPagedList(pageNumber, pageSize));
        }

        // GET: requests1/Details/5
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

        // GET: requests1/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: requests1/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,RequestingDate,RequestingUnit,FocalPoint,Purpose,FromDate,ToDate,NumOFP_CO,NumOFP_RB,NumOFP_HQ,NumOFP_Other,Country,GOV,District,OtherPlace,Budget_TR,Number_Roms,Check_inDate,Check_OutDate,Rom_Type,Microphone,HiSpeedInternet,Conf_Call_Device,MMR_Days,BOR_Days,COffeBreak,COffeBreak_UNM,Lunch,Dinner,Coctail_Reception,Dedicated_IT_SUPP,Transportstion_ForLocal_Staff,Airport_Picup_Services,Other_Services,non1,non2,non3")] requests1 requests1)
        {
            if (ModelState.IsValid)
            {
                db.requests1.Add(requests1);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(requests1);
        }

        // GET: requests1/Edit/5
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

        // POST: requests1/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,RequestingDate,RequestingUnit,FocalPoint,Purpose,FromDate,ToDate,NumOFP_CO,NumOFP_RB,NumOFP_HQ,NumOFP_Other,Country,GOV,District,OtherPlace,Budget_TR,Number_Roms,Check_inDate,Check_OutDate,Rom_Type,Microphone,HiSpeedInternet,Conf_Call_Device,MMR_Days,BOR_Days,COffeBreak,COffeBreak_UNM,Lunch,Dinner,Coctail_Reception,Dedicated_IT_SUPP,Transportstion_ForLocal_Staff,Airport_Picup_Services,Other_Services,non1,non2,non3")] requests1 requests1)
        {
            if (ModelState.IsValid)
            {
                db.Entry(requests1).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(requests1);
        }

        // GET: requests1/Delete/5
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

        // POST: requests1/Delete/5
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
