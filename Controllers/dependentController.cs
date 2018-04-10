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
    public class dependentController : Controller
    {
        private WFPEntities1 db = new WFPEntities1();

        // GET: /dependent/
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = String.IsNullOrEmpty(sortOrder) ? "First_Name" : "";
            var students = from s in db.dependents
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
            var dependents = db.dependents.Include(d => d.staff);


            if (!String.IsNullOrEmpty(searchString))
            {
                dependents = dependents.Where(s => s.dependents_last_name.Contains(searchString)
                                       || s.dependents_first_name.Contains(searchString)
                                       || s.staff .staff_email .Contains(searchString));
            }

            switch (sortOrder)
            {
                case "First_Name":
                    dependents = dependents.OrderByDescending(s => s.dependents_first_name );
                    break;
                case "name_desc":
                    dependents = dependents.OrderByDescending(s => s.dependents_last_name );
                    break;

                default:  // Name ascending 
                    dependents = dependents.OrderBy(s => s.dependents_first_name );
                    break;
            }

            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(dependents.ToPagedList(pageNumber, pageSize));
        }

        // GET: /dependent/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            dependent dependent = db.dependents.Find(id);
            if (dependent == null)
            {
                return HttpNotFound();
            }
            return View(dependent);
        }

        // GET: /dependent/Create
        public ActionResult Create()
        {
            ViewBag.staffid = new SelectList(db.staffs, "staffid", "staff_email");
            return View();
        }

        // POST: /dependent/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="dependentsid,dependents_code,staffid,dependents_first_name,dependents_last_name,dependents_gender,dependents_phone_num,dependents_blood_group,dependents_passport_num,dependents_passport_expiry_date,dependents_notes,dependents_medical_condition")] dependent dependent)
        {
            if (ModelState.IsValid)
            {
                db.dependents.Add(dependent);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.staffid = new SelectList(db.staffs, "staffid", "staff_email", dependent.staffid);
            return View(dependent);
        }

        // GET: /dependent/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            dependent dependent = db.dependents.Find(id);
            if (dependent == null)
            {
                return HttpNotFound();
            }
            ViewBag.staffid = new SelectList(db.staffs, "staffid", "staff_email", dependent.staffid);
            return View(dependent);
        }

        // POST: /dependent/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="dependentsid,dependents_code,staffid,dependents_first_name,dependents_last_name,dependents_gender,dependents_phone_num,dependents_blood_group,dependents_passport_num,dependents_passport_expiry_date,dependents_notes,dependents_medical_condition")] dependent dependent)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dependent).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.staffid = new SelectList(db.staffs, "staffid", "staff_email", dependent.staffid);
            return View(dependent);
        }

        // GET: /dependent/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            dependent dependent = db.dependents.Find(id);
            if (dependent == null)
            {
                return HttpNotFound();
            }
            return View(dependent);
        }

        // POST: /dependent/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
             try
  {
            dependent dependent = db.dependents.Find(id);
            db.dependents.Remove(dependent);
            db.SaveChanges();
            return RedirectToAction("Index");
  }
             catch (Exception ex)
             {
                 return View(ex);
             }
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
