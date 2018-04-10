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
    public class contracttypeController : Controller
    {
        private WFPEntities1 db = new WFPEntities1();

        // GET: /contracttype/
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = String.IsNullOrEmpty(sortOrder) ? "First_Name" : "";
            var students = from s in db.contract_type 
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

            var contract_type = db.contract_type.Include(s => s.staffs );

            if (!String.IsNullOrEmpty(searchString))
            {
                contract_type = contract_type.Where(s => s.contract_type_name .Contains(searchString)
                                       || s.contract_type_description .Contains(searchString));
                                       
            }

            switch (sortOrder)
            {
                case "First_Name":
                    contract_type = contract_type.OrderByDescending(s => s.contract_type_description );
                    break;
                case "name_desc":
                    contract_type = contract_type.OrderByDescending(s => s.contract_type_name );
                    break;

                default:  // Name ascending 
                    contract_type = contract_type.OrderBy(s => s.contract_type_description );
                    break;
            }

            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(contract_type.ToPagedList(pageNumber, pageSize));
            //return View(db.contract_type.ToList());
        }

        // GET: /contracttype/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            contract_type contract_type = db.contract_type.Find(id);
            if (contract_type == null)
            {
                return HttpNotFound();
            }
            return View(contract_type);
        }

        // GET: /contracttype/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /contracttype/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="contractTypeid,contract_Type_code,contract_type_name,contract_type_description")] contract_type contract_type)
        {
            if (ModelState.IsValid)
            {
                db.contract_type.Add(contract_type);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(contract_type);
        }

        // GET: /contracttype/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            contract_type contract_type = db.contract_type.Find(id);
            if (contract_type == null)
            {
                return HttpNotFound();
            }
            return View(contract_type);
        }

        // POST: /contracttype/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="contractTypeid,contract_Type_code,contract_type_name,contract_type_description")] contract_type contract_type)
        {
            if (ModelState.IsValid)
            {
                db.Entry(contract_type).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(contract_type);
        }

        // GET: /contracttype/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            contract_type contract_type = db.contract_type.Find(id);
            if (contract_type == null)
            {
                return HttpNotFound();
            }
            return View(contract_type);
        }

        // POST: /contracttype/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
            contract_type contract_type = db.contract_type.Find(id);
            db.contract_type.Remove(contract_type);
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
