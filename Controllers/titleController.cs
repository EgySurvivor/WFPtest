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
    public class titleController : Controller
    {
        private WFPEntities1 db = new WFPEntities1();

        // GET: /title/
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {

            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = String.IsNullOrEmpty(sortOrder) ? "First_Name" : "";
            var students = from s in db.functional_title 
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

            var functional_title = db.functional_title.Include(s => s.staffs); 

            if (!String.IsNullOrEmpty(searchString))
            {
                functional_title = functional_title.Where(s => s.functional_title_abreviation_english.Contains(searchString)
                                       || s.functional_title_description_english .Contains(searchString)
                                       || s.functional_title_abreviation_french .Contains(searchString));
            }

            switch (sortOrder)
            {
                case "First_Name":
                    functional_title = functional_title.OrderByDescending(s => s.functional_title_description_english);
                    break;
                case "name_desc":
                    functional_title = functional_title.OrderByDescending(s => s.functional_title_abreviation_english );
                    break;

                default:  // Name ascending 
                    functional_title = functional_title.OrderBy(s => s.functional_title_description_english );
                    break;
            }

            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(functional_title.ToPagedList(pageNumber, pageSize));
            //return View(db.functional_title.ToList());
        }

        // GET: /title/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            functional_title functional_title = db.functional_title.Find(id);
            if (functional_title == null)
            {
                return HttpNotFound();
            }
            return View(functional_title);
        }

        // GET: /title/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /title/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="functionaltitleid,functional_title_id,functional_title_description_english,functional_title_abreviation_english,functional_title_description_french,functional_title_abreviation_french,functional_title_status,functional_title_created_by,functional_title_created_datetime,functional_title_last_modified_by,functional_title_last_modified_datetime,functional_title_deleted_by,functional_title_deleted_datetime")] functional_title functional_title)
        {
            if (ModelState.IsValid)
            {
                db.functional_title.Add(functional_title);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(functional_title);
        }

        // GET: /title/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            functional_title functional_title = db.functional_title.Find(id);
            if (functional_title == null)
            {
                return HttpNotFound();
            }
            return View(functional_title);
        }

        // POST: /title/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="functionaltitleid,functional_title_id,functional_title_description_english,functional_title_abreviation_english,functional_title_description_french,functional_title_abreviation_french,functional_title_status,functional_title_created_by,functional_title_created_datetime,functional_title_last_modified_by,functional_title_last_modified_datetime,functional_title_deleted_by,functional_title_deleted_datetime")] functional_title functional_title)
        {
            if (ModelState.IsValid)
            {
                db.Entry(functional_title).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(functional_title);
        }

        // GET: /title/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            functional_title functional_title = db.functional_title.Find(id);
            if (functional_title == null)
            {
                return HttpNotFound();
            }
            return View(functional_title);
        }

        // POST: /title/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            functional_title functional_title = db.functional_title.Find(id);
            db.functional_title.Remove(functional_title);
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
