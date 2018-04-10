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
    public class subofficeController : Controller
    {
        private WFPEntities1 db = new WFPEntities1();

        // GET: /suboffice/
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {

            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = String.IsNullOrEmpty(sortOrder) ? "First_Name" : "";
            var students = from s in db.staffs
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
            var sub_office = db.sub_office.Include(s => s.Country_office);

            if (!String.IsNullOrEmpty(searchString))
            {
                sub_office = sub_office.Where(s => s.office_abreviation_english .Contains(searchString)
                                       || s.office_description_english .Contains(searchString)
                                       || s.office_description_french .Contains(searchString)
                                       || s.Country_office .office_abreviation_english.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "First_Name":
                    sub_office = sub_office.OrderByDescending(s => s.office_description_english );
                    break;
                case "name_desc":
                    sub_office = sub_office.OrderByDescending(s => s.office_abreviation_english );
                    break;

                default:  // Name ascending 
                    sub_office = sub_office.OrderBy(s => s.office_description_english );
                    break;
            }

            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(sub_office.ToPagedList(pageNumber, pageSize));
            //return View(sub_office.ToList());
        }

        // GET: /suboffice/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            sub_office sub_office = db.sub_office.Find(id);
            if (sub_office == null)
            {
                return HttpNotFound();
            }
            return View(sub_office);
        }

        // GET: /suboffice/Create
        public ActionResult Create()
        {
            ViewBag.countryofficeid = new SelectList(db.Country_office, "countryofficeid", "office_abreviation_english");
            return View();
        }

        // POST: /suboffice/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="subofficeid,sub_office_id,office_description_english,office_abreviation_english,office_description_french,office_abreviation_french,office_status,office_created_by,office_created_datetime,office_last_modified_by,office_last_modified_datetime,office_deleted_by,office_deleted_datetime,countryofficeid")] sub_office sub_office)
        {
            if (ModelState.IsValid)
            {
                db.sub_office.Add(sub_office);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.countryofficeid = new SelectList(db.Country_office, "countryofficeid", "office_abreviation_english", sub_office.countryofficeid);
            return View(sub_office);
        }

        // GET: /suboffice/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            sub_office sub_office = db.sub_office.Find(id);
            if (sub_office == null)
            {
                return HttpNotFound();
            }
            ViewBag.countryofficeid = new SelectList(db.Country_office, "countryofficeid", "office_abreviation_english", sub_office.countryofficeid);
            return View(sub_office);
        }

        // POST: /suboffice/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="subofficeid,sub_office_id,office_description_english,office_abreviation_english,office_description_french,office_abreviation_french,office_status,office_created_by,office_created_datetime,office_last_modified_by,office_last_modified_datetime,office_deleted_by,office_deleted_datetime,countryofficeid")] sub_office sub_office)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sub_office).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.countryofficeid = new SelectList(db.Country_office, "countryofficeid", "office_abreviation_english", sub_office.countryofficeid);
            return View(sub_office);
        }

        // GET: /suboffice/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            sub_office sub_office = db.sub_office.Find(id);
            if (sub_office == null)
            {
                return HttpNotFound();
            }
            return View(sub_office);
        }

        // POST: /suboffice/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            sub_office sub_office = db.sub_office.Find(id);
            db.sub_office.Remove(sub_office);
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
