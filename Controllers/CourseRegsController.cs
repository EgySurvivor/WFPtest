using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WFPtest.Models;
using PagedList;

namespace WFPtest.Controllers
{
    public class CourseRegsController : Controller
    {
        private WFPEntities1 db = new WFPEntities1();

        // GET: CourseRegs
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {


            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = String.IsNullOrEmpty(sortOrder) ? "First_Name" : "";
            var students = from s in db.CourseRegs
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
            var courseRegs = db.CourseRegs.Include(c => c.Cours).Include(c => c.CourseSession).Include(c => c.staff);

            if (!String.IsNullOrEmpty(searchString))
            {
                courseRegs = courseRegs.Where(s => s.Cours.CourseName.Contains(searchString)
                                       || s.staff.staff_email.Contains(searchString));
                                       

            }

            switch (sortOrder)
            {
                case "First_Name":
                    courseRegs = courseRegs.OrderByDescending(s => s.Cours .CourseName);
                    break;
                case "name_desc":
                    courseRegs = courseRegs.OrderByDescending(s => s.Cours.CourseName );
                    break;

                default:  // Name ascending 
                    courseRegs = courseRegs.OrderBy(s => s.Cours.CourseName );
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(courseRegs.ToPagedList(pageNumber, pageSize));
            //return View(await courseRegs.ToListAsync());
        }

        // GET: CourseRegs/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseReg courseReg = await db.CourseRegs.FindAsync(id);
            if (courseReg == null)
            {
                return HttpNotFound();
            }
            return View(courseReg);
        }

        // GET: CourseRegs/Create
        public ActionResult Create()
        {
            ViewBag.courseid = new SelectList(db.Courses, "CourseID", "CourseName");
            ViewBag.sessionid = new SelectList(db.CourseSessions, "Sessionid", "SessionName");
            //ViewBag.staffid = new SelectList(db.staffs.OrderBy(staffs => staffs.staff_email ), "staffid", "staff_email");

             var staffsc = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
            staffsc = staffsc.Where(s => s.staff_email == User.Identity.Name + "@wfp.org");
            var stafemal11 = staffsc.First().staff_email;

            ViewBag.staffemail = stafemal11;
            ViewBag.staffid  = staffsc.First().staffid; 

            return View();
        }

        // POST: CourseRegs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "RegID,staffid,courseid,sessionid,LMSPre,SupervisorApprove")] CourseReg courseReg)
        {


            if (ModelState.IsValid)
            {
                db.CourseRegs.Add(courseReg);
                await db.SaveChangesAsync();
                return RedirectToAction("Create");
            }

          
            ViewBag.courseid = new SelectList(db.Courses, "CourseID", "CourseName", courseReg.courseid);
            ViewBag.sessionid = new SelectList(db.CourseSessions, "Sessionid", "CourseDuration", courseReg.sessionid);
            ViewBag.staffid = new SelectList(db.staffs, "staffid", "staff_id", courseReg.staffid);
            return View(courseReg);
        }

        // GET: CourseRegs/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseReg courseReg = await db.CourseRegs.FindAsync(id);
            if (courseReg == null)
            {
                return HttpNotFound();
            }
            ViewBag.courseid = new SelectList(db.Courses, "CourseID", "CourseName", courseReg.courseid);
            ViewBag.sessionid = new SelectList(db.CourseSessions, "Sessionid", "CourseDuration", courseReg.sessionid);
            ViewBag.staffid = new SelectList(db.staffs, "staffid", "staff_id", courseReg.staffid);
            return View(courseReg);
        }

        // POST: CourseRegs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "RegID,staffid,courseid,sessionid,LMSPre,,SupervisorApprove")] CourseReg courseReg)
        {
            if (ModelState.IsValid)
            {
                db.Entry(courseReg).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.courseid = new SelectList(db.Courses, "CourseID", "CourseName", courseReg.courseid);
            ViewBag.sessionid = new SelectList(db.CourseSessions, "Sessionid", "CourseDuration", courseReg.sessionid);
            ViewBag.staffid = new SelectList(db.staffs, "staffid", "staff_id", courseReg.staffid);
            return View(courseReg);
        }

        // GET: CourseRegs/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseReg courseReg = await db.CourseRegs.FindAsync(id);
            if (courseReg == null)
            {
                return HttpNotFound();
            }
            return View(courseReg);
        }

        // POST: CourseRegs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            CourseReg courseReg = await db.CourseRegs.FindAsync(id);
            db.CourseRegs.Remove(courseReg);
            await db.SaveChangesAsync();
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
