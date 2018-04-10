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
    public class contractdetailsController : Controller
    {
        private WFPEntities1 db = new WFPEntities1();

        // GET: /contractdetails/
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            
            
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = String.IsNullOrEmpty(sortOrder) ? "First_Name" : "";
            var students = from s in db.contract_details 
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
            var contract_details = db.contract_details.Include(c => c.Fund);
           ViewBag.staffid = new SelectList(db.staffs, "staffid", "staff_email");
            if (!String.IsNullOrEmpty(searchString))
            {
                contract_details = contract_details.Where(s => s.contract_details_abreviation_french.Contains(searchString)
                                       || s.contract_details_abreviation_english.Contains(searchString));
                
                                     
            }

            switch (sortOrder)
            {
                case "First_Name":
                    contract_details = contract_details.OrderByDescending(s => s.contract_details_abreviation_english );
                    break;
                case "name_desc":
                    contract_details = contract_details.OrderByDescending(s => s.contract_details_abreviation_french );
                    break;

                default:  // Name ascending 
                    contract_details = contract_details.OrderBy(s => s.contract_details_abreviation_english );
                    break;
            }

            int pageSize = 5;
            int pageNumber = (page ?? 1);

          
          

            return View(contract_details.ToPagedList(pageNumber, pageSize));
            
            //return View(contract_details.ToList());
        }

        // GET: /contractdetails/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            contract_details contract_details = db.contract_details.Find(id);

         
           
            if (contract_details == null)
            {
                return HttpNotFound();
            }
           
            return View(contract_details);
        }

        // GET: /contractdetails/Create
        public ActionResult Create()
        {
            ViewBag.fundid = new SelectList(db.Funds, "fundid", "resource");
            ViewBag.staffid = new SelectList(db.staffs  , "staffid", "staff_email");
            return View();
        }

        // POST: /contractdetails/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="contractdetailsid,contract_details_id,contract_details_description_english,contract_details_abreviation_english,contract_details_description_french,contract_details_abreviation_french,contract_details_status,contract_details_created_by,contract_details_created_datetime,contract_details_last_modified_by,contract_details_last_modified_datetime,contract_details_deleted_by,contract_details_deleted_datetime,contract_type_code,fundid,staffid")] contract_details contract_details)
        {
            if (ModelState.IsValid)
            {
                db.contract_details.Add(contract_details);
                db.SaveChanges();


                //staff staffs = new staff()
                //{
                //    stuff_contract_details = contract_details.contractdetailsid,

                //};




                //selectedItem.End_Date = kendo.parseDate(data, "MM/dd/yyyy")
                ////selectedItem.set("End_Date", kendo.parseDate(data, "MM/dd/yyyy"))
                ////db.Entry(staffs).State = EntityState.Modified;
                ////db.staffs.Add(staffs);
                //var res = db.staffs .Include(c => c.staffid );
                var res = (from c in db.staffs
                           where c.staffid == contract_details.staffid 
                           select c).SingleOrDefault();


                res.stuff_contract_details = contract_details.contractdetailsid ;
                db.SaveChanges();

                return RedirectToAction("Create");
            }


            ViewBag.fundid = new SelectList(db.Funds, "fundid", "resource", contract_details.fundid);
            ViewBag.staffid = new SelectList(db.staffs, "staffid", "staff_email", contract_details.staffid );
            return View(contract_details);
        }

        // GET: /contractdetails/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            contract_details contract_details = db.contract_details.Find(id);
            if (contract_details == null)
            {
                return HttpNotFound();
            }
            ViewBag.fundid = new SelectList(db.Funds, "fundid", "resource", contract_details.fundid);
            ViewBag.staffid = new SelectList(db.staffs, "staffid", "staff_email", contract_details.staffid);
         
            
           
            db.SaveChanges();

            return View(contract_details);
        
         
    
        }

        // POST: /contractdetails/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="contractdetailsid,contract_details_id,contract_details_description_english,contract_details_abreviation_english,contract_details_description_french,contract_details_abreviation_french,contract_details_status,contract_details_created_by,contract_details_created_datetime,contract_details_last_modified_by,contract_details_last_modified_datetime,contract_details_deleted_by,contract_details_deleted_datetime,contract_type_code,fundid,staffid")] contract_details contract_details)
        {
            if (ModelState.IsValid)
            {
                db.Entry(contract_details).State = EntityState.Modified;
                db.SaveChanges();

                //var res = (from c in db.staffs
                //           where c.staffid == contract_details.staffid
                //           select c).SingleOrDefault();
                
                //res.stuff_contract_details = contract_details.contractdetailsid;
                               
                //db.SaveChanges();

                //var res = (from c in db.staffs
                //           where c.staffid == contract_details.staffid
                //           select c).SingleOrDefault();

                //res.stuff_contract_details = null;


                return RedirectToAction("Index");


                
            }
            ViewBag.fundid = new SelectList(db.Funds, "fundid", "fund_code");
            ViewBag.staffid = new SelectList(db.staffs, "staffid", "staff_email");
            return View(contract_details);
        }

        // GET: /contractdetails/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            contract_details contract_details = db.contract_details.Find(id);
            if (contract_details == null)
            {
                return HttpNotFound();
            }
            return View(contract_details);
        }

        // POST: /contractdetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {

            contract_details contract_details = db.contract_details.Find(id);
            db.contract_details.Remove(contract_details);
            db.SaveChanges();
            return RedirectToAction("Index");
            }
  catch(Exception ex)
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
