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
    public class districtController : Controller
    {
        private WFPEntities1 db = new WFPEntities1();

        // GET: /district/
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = String.IsNullOrEmpty(sortOrder) ? "First_Name" : "";
            var students = from s in db.districts 
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

            var districts = db.districts.Include(d => d.country).Include(d => d.governorate);

            if (!String.IsNullOrEmpty(searchString))
            {
                districts = districts.Where(s => s.area_name.Contains(searchString)
                                       || s.area_name  .Contains(searchString));


            }

            switch (sortOrder)
            {
                case "First_Name":
                    districts = districts.OrderByDescending(s => s.area_name );
                    break;
                case "name_desc":
                    districts = districts.OrderByDescending(s => s.area_name);
                    break;

                default:  // Name ascending 
                    districts = districts.OrderBy(s => s.area_name );
                    break;
            }

            int pageSize = 5;
            int pageNumber = (page ?? 1);

            return View(districts.ToPagedList(pageNumber, pageSize));
        }

        // GET: /district/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            district district = db.districts.Find(id);
            if (district == null)
            {
                return HttpNotFound();
            }
            return View(district);
        }

        // GET: /district/Create
        public ActionResult Create()
        {
            ViewBag.countryid = new SelectList(db.countries, "countryid", "country_name");
            ViewBag.governorates_code = new SelectList(db.governorates, "governorates_code", "governorates_name");
            return View();
        }

        // POST: /district/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="areacode,area_name,countryid,governorates_code")] district district)
        {
            if (ModelState.IsValid)
            {
                db.districts.Add(district);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.countryid = new SelectList(db.countries, "countryid", "country_name", district.countryid);
            ViewBag.governorates_code = new SelectList(db.governorates, "governorates_code", "governorates_name", district.governorates_code);
            return View(district);
        }

        // GET: /district/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            district district = db.districts.Find(id);
            if (district == null)
            {
                return HttpNotFound();
            }
            ViewBag.countryid = new SelectList(db.countries, "countryid", "country_name", district.countryid);
            ViewBag.governorates_code = new SelectList(db.governorates, "governorates_code", "governorates_name", district.governorates_code);
            return View(district);
        }

        // POST: /district/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="areacode,area_name,countryid,governorates_code")] district district)
        {
            if (ModelState.IsValid)
            {
                db.Entry(district).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.countryid = new SelectList(db.countries, "countryid", "country_name", district.countryid);
            ViewBag.governorates_code = new SelectList(db.governorates, "governorates_code", "governorates_name", district.governorates_code);
            return View(district);
        }

        // GET: /district/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            district district = db.districts.Find(id);
            if (district == null)
            {
                return HttpNotFound();
            }
            return View(district);
        }

        // POST: /district/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            district district = db.districts.Find(id);
            db.districts.Remove(district);
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
