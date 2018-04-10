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
    public class governorateController : Controller
    {
        private WFPEntities1 db = new WFPEntities1();

        // GET: /governorate/
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = String.IsNullOrEmpty(sortOrder) ? "First_Name" : "";
            var students = from s in db.governorates 
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

            var governorates = db.governorates.Include(g => g.country);

            if (!String.IsNullOrEmpty(searchString))
            {
                governorates = governorates.Where(s => s.governorates_description .Contains(searchString)
                                       || s.governorates_name .Contains(searchString));


            }

            switch (sortOrder)
            {
                case "First_Name":
                    governorates = governorates.OrderByDescending(s => s.governorates_name );
                    break;
                case "name_desc":
                    governorates = governorates.OrderByDescending(s => s.governorates_description );
                    break;

                default:  // Name ascending 
                    governorates = governorates.OrderBy(s => s.governorates_name );
                    break;
            }

            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(governorates.ToPagedList(pageNumber, pageSize));
        }

        // GET: /governorate/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            governorate governorate = db.governorates.Find(id);
            if (governorate == null)
            {
                return HttpNotFound();
            }
            return View(governorate);
        }

        // GET: /governorate/Create
        public ActionResult Create()
        {
            ViewBag.country_code = new SelectList(db.countries, "countryid", "country_name");
            return View();
        }

        // POST: /governorate/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="governorates_code,governorates_name,governorates_description,country_code,staff_id")] governorate governorate)
        {
            if (ModelState.IsValid)
            {
                db.governorates.Add(governorate);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.country_code = new SelectList(db.countries, "countryid", "country_name", governorate.country_code);
            return View(governorate);
        }

        // GET: /governorate/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            governorate governorate = db.governorates.Find(id);
            if (governorate == null)
            {
                return HttpNotFound();
            }
            ViewBag.country_code = new SelectList(db.countries, "countryid", "country_name", governorate.country_code);
            return View(governorate);
        }

        // POST: /governorate/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="governorates_code,governorates_name,governorates_description,country_code,staff_id")] governorate governorate)
        {
            if (ModelState.IsValid)
            {
                db.Entry(governorate).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.country_code = new SelectList(db.countries, "countryid", "country_name", governorate.country_code);
            return View(governorate);
        }

        // GET: /governorate/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            governorate governorate = db.governorates.Find(id);
            if (governorate == null)
            {
                return HttpNotFound();
            }
            return View(governorate);
        }

        // POST: /governorate/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            governorate governorate = db.governorates.Find(id);
            db.governorates.Remove(governorate);
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
