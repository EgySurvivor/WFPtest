using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WFPtest.Models;

namespace WFPtest.Views.EventRequest
{
    public class requestsController : Controller
    {
        private WFPEntities1 db = new WFPEntities1();

        // GET: requests
        public ActionResult Index()
        {
            var requests = db.requests.Include(r => r.functional_title).Include(r => r.staff).Include(r => r.staff1).Include(r => r.unit1);
            return View(requests.ToList());
        }

        // GET: requests/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            request request = db.requests.Find(id);
            if (request == null)
            {
                return HttpNotFound();
            }
            return View(request);
        }

        // GET: requests/Create
        public ActionResult Create()
        {
            ViewBag.job_title = new SelectList(db.functional_title, "functionaltitleid", "functional_title_id");
            ViewBag.requested_by = new SelectList(db.staffs, "staffid", "staff_id");
            ViewBag.requested_by = new SelectList(db.staffs, "staffid", "staff_id");
            ViewBag.unit = new SelectList(db.units, "unitid", "unit_id");
            return View();
        }

        // POST: requests/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "request_no,first_name,last_name,job_title,unit,budget_code,index_number,duty_station,supervisor_email,appointment_type,start_date,end_date,computerLaptop,computerDeskyop,email,access_P,telephone,pincode_ext,local_sim,international,roaming,SmartPhone,BasicPhone,usb_modem,color_printer,BlackberryService,IphoneService,mobile_phone,other,location,requested_by")] request request)
        {
            if (ModelState.IsValid)
            {
                db.requests.Add(request);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.job_title = new SelectList(db.functional_title, "functionaltitleid", "functional_title_id", request.job_title);
            ViewBag.requested_by = new SelectList(db.staffs, "staffid", "staff_id", request.requested_by);
            ViewBag.requested_by = new SelectList(db.staffs, "staffid", "staff_id", request.requested_by);
            ViewBag.unit = new SelectList(db.units, "unitid", "unit_id", request.unit);
            return View(request);
        }

        // GET: requests/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            request request = db.requests.Find(id);
            if (request == null)
            {
                return HttpNotFound();
            }
            ViewBag.job_title = new SelectList(db.functional_title, "functionaltitleid", "functional_title_id", request.job_title);
            ViewBag.requested_by = new SelectList(db.staffs, "staffid", "staff_id", request.requested_by);
            ViewBag.requested_by = new SelectList(db.staffs, "staffid", "staff_id", request.requested_by);
            ViewBag.unit = new SelectList(db.units, "unitid", "unit_id", request.unit);
            return View(request);
        }

        // POST: requests/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "request_no,first_name,last_name,job_title,unit,budget_code,index_number,duty_station,supervisor_email,appointment_type,start_date,end_date,computerLaptop,computerDeskyop,email,access_P,telephone,pincode_ext,local_sim,international,roaming,SmartPhone,BasicPhone,usb_modem,color_printer,BlackberryService,IphoneService,mobile_phone,other,location,requested_by")] request request)
        {
            if (ModelState.IsValid)
            {
                db.Entry(request).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.job_title = new SelectList(db.functional_title, "functionaltitleid", "functional_title_id", request.job_title);
            ViewBag.requested_by = new SelectList(db.staffs, "staffid", "staff_id", request.requested_by);
            ViewBag.requested_by = new SelectList(db.staffs, "staffid", "staff_id", request.requested_by);
            ViewBag.unit = new SelectList(db.units, "unitid", "unit_id", request.unit);
            return View(request);
        }

        // GET: requests/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            request request = db.requests.Find(id);
            if (request == null)
            {
                return HttpNotFound();
            }
            return View(request);
        }

        // POST: requests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            request request = db.requests.Find(id);
            db.requests.Remove(request);
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
