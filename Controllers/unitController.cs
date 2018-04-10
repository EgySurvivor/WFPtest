using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WFPtest.Models;
using PagedList;

namespace WFPtest.Controllers
{
    public class unitController : Controller
    {
        private WFPEntities1 db = new WFPEntities1();

        // GET: /unit/
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {

            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = String.IsNullOrEmpty(sortOrder) ? "First_Name" : "";
            var students = from s in db.units
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
            var units = db.units.Include(s => s.Country_office);
            units = units.Where(s => s.unit_description_english != "NO UNIT");

            if (!String.IsNullOrEmpty(searchString))
            {
                units = units.Where(s => s.unit_abreviation_english .Contains(searchString)
                                       || s.unit_description_english .Contains(searchString)
                                       || s.unit_abreviation_french .Contains(searchString)
                                       || s.unit_description_french .Contains(searchString)
                                       || s.Country_office .office_abreviation_english .Contains(searchString));

            }

            switch (sortOrder)
            {
                case "First_Name":
                    units = units.OrderByDescending(s => s.unit_description_english );
                    break;
                case "name_desc":
                    units = units.OrderByDescending(s => s.unit_abreviation_english );
                    break;

                default:  // Name ascending 
                    units = units.OrderBy(s => s.unit_description_english );
                    break;
            }

            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(units.ToPagedList(pageNumber, pageSize));

            //return View(db.units.ToList());
        }

        // GET: /unit/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            unit unit = db.units.Find(id);
            if (unit == null)
            {
                return HttpNotFound();
            }
            return View(unit);
        }

        // GET: /unit/Create
        public ActionResult Create()
        {
            ViewBag.office_id = new SelectList(db.Country_office, "countryofficeid", "office_description_english");
            ViewBag.HOU_ID = new SelectList(db.staffs.OrderBy(staffs => staffs.staff_email), "staffid", "staff_email");
            return View();
        }

        // POST: /unit/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "unitid,unit_id,unit_description_english,unit_abreviation_english,unit_description_french,unit_abreviation_french,unit_status,unit_created_by,unit_created_datetime,unit_last_modified_by,unit_last_modified_datetime,unit_deleted_by,unit_deleted_datetime,unit_short_abreviation,office_id,HOU_ID")] unit unit)
        {
            if (ModelState.IsValid)
            {
                db.units.Add(unit);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.office_id = new SelectList(db.Country_office, "countryofficeid", "office_description_english",unit.office_id );
            ViewBag.HOU_ID = new SelectList(db.Country_office, "staffid", "staff_email", unit.HOU_ID );
            return View(unit);
        }

        // GET: /unit/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            unit unit = db.units.Find(id);
            if (unit == null)
            {
                return HttpNotFound();
            }
            ViewBag.office_id = new SelectList(db.Country_office, "countryofficeid", "office_description_english", unit.office_id);
            ViewBag.HOU_ID = new SelectList(db.staffs, "staffid", "staff_email",unit.HOU_ID );
            return View(unit);
        }

        // POST: /unit/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "unitid,unit_id,unit_description_english,unit_abreviation_english,unit_description_french,unit_abreviation_french,unit_status,unit_created_by,unit_created_datetime,unit_last_modified_by,unit_last_modified_datetime,unit_deleted_by,unit_deleted_datetime,unit_short_abreviation,office_id,HOU_ID")] unit unit)
        {
            if (ModelState.IsValid)
            {
                db.Entry(unit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.office_id = new SelectList(db.Country_office, "countryofficeid", "office_description_english", unit.office_id);
            ViewBag.HOU_ID = new SelectList(db.Country_office, "staffid", "staff_email", unit.HOU_ID);
            return View(unit);
        }

        // GET: /unit/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            unit unit = db.units.Find(id);
            if (unit == null)
            {
                return HttpNotFound();
            }
            return View(unit);
        }

        // POST: /unit/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            unit unit = db.units.Find(id);
            db.units.Remove(unit);
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
