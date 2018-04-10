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
    public class officeController : Controller
    {
        private WFPEntities1 db = new WFPEntities1();

        // GET: /office/
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = String.IsNullOrEmpty(sortOrder) ? "First_Name" : "";
            var students = from s in db.Country_office 
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
            var country_office = db.Country_office.Include(c => c.country);



            if (!String.IsNullOrEmpty(searchString))
            {
                country_office = country_office.Where(s => s.office_abreviation_english .Contains(searchString)
                                       || s.office_description_english.Contains(searchString)
                                       || s.office_description_french.Contains(searchString)
                                       || s.country .country_name .Contains(searchString));
            }

            switch (sortOrder)
            {
                case "First_Name":
                    country_office = country_office.OrderByDescending(s => s.office_description_english );
                    break;
                case "name_desc":
                    country_office = country_office.OrderByDescending(s => s.office_abreviation_english );
                    break;

                default:  // Name ascending 
                    country_office = country_office.OrderBy(s => s.office_description_english );
                    break;
            }

            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(country_office.ToPagedList(pageNumber, pageSize));
            //return View(country_office.ToList());
        }

        // GET: /office/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Country_office country_office = db.Country_office.Find(id);
            if (country_office == null)
            {
                return HttpNotFound();
            }
            return View(country_office);
        }

        // GET: /office/Create
        public ActionResult Create()
        {
            ViewBag.country_code = new SelectList(db.countries, "countryid", "country_name");
            return View();
        }

        // POST: /office/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="countryofficeid,office_id,office_description_english,office_abreviation_english,office_description_french,office_abreviation_french,office_status,office_created_by,office_created_datetime,office_last_modified_by,office_last_modified_datetime,office_deleted_by,office_deleted_datetime,country_code")] Country_office country_office)
        {
            if (ModelState.IsValid)
            {
                db.Country_office.Add(country_office);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.country_code = new SelectList(db.countries, "countryid", "country_name", country_office.country_code);
            return View(country_office);
        }



        // GET: /office/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Country_office country_office = db.Country_office.Find(id);
            if (country_office == null)
            {
                return HttpNotFound();
            }
            ViewBag.country_code = new SelectList(db.countries, "countryid", "country_name", country_office.country_code);
            return View(country_office);
        }

        // POST: /office/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="countryofficeid,office_id,office_description_english,office_abreviation_english,office_description_french,office_abreviation_french,office_status,office_created_by,office_created_datetime,office_last_modified_by,office_last_modified_datetime,office_deleted_by,office_deleted_datetime,country_code")] Country_office country_office)
        {
            if (ModelState.IsValid)
            {
                db.Entry(country_office).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.country_code = new SelectList(db.countries, "countryid", "country_name", country_office.country_code);
            return View(country_office);
        }

        // GET: /office/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Country_office country_office = db.Country_office.Find(id);
            if (country_office == null)
            {
                return HttpNotFound();
            }
            return View(country_office);
        }

        // POST: /office/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Country_office country_office = db.Country_office.Find(id);
            db.Country_office.Remove(country_office);
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
