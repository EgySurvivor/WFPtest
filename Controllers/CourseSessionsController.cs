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
    public class CourseSessionsController : Controller
    {
        private WFPEntities1 db = new WFPEntities1();

        // GET: CourseSessions
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {

            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = String.IsNullOrEmpty(sortOrder) ? "First_Name" : "";
            var students = from s in db.CourseSessions
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
            var courseSessions = db.CourseSessions.Include(c => c.Cours);

            if (!String.IsNullOrEmpty(searchString))
            {
                courseSessions = courseSessions.Where(s => s.Cours .CourseName .Contains(searchString)
                                       || s.CourseLocation .Contains(searchString)
                                       || s.CourseDuration .Contains(searchString));

            }

            switch (sortOrder)
            {
                case "First_Name":
                    courseSessions = courseSessions.OrderByDescending(s => s.Cours.CourseName);
                    break;
                case "name_desc":
                    courseSessions = courseSessions.OrderByDescending(s => s.CourseStartDate );
                    break;

                default:  // Name ascending 
                    courseSessions = courseSessions.OrderBy(s => s.Cours .CourseName );
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(courseSessions.ToPagedList(pageNumber, pageSize));
            //return View(courseSessions.ToList());
        }

        // GET: CourseSessions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseSession courseSession = db.CourseSessions.Find(id);
            if (courseSession == null)
            {
                return HttpNotFound();
            }
            return View(courseSession);
        }





        public ActionResult IndexPartial(string sortOrder, string currentFilter, string searchString, int? page)
        {

            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = String.IsNullOrEmpty(sortOrder) ? "First_Name" : "";
            var students = from s in db.CourseSessions
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
            var courseSessions = db.CourseSessions.Include(c => c.Cours);

            if (!String.IsNullOrEmpty(searchString))
            {
                courseSessions = courseSessions.Where(s => s.Cours.CourseName.Contains(searchString)
                                       || s.CourseLocation.Contains(searchString)
                                       || s.CourseDuration.Contains(searchString));

            }

            switch (sortOrder)
            {
                case "First_Name":
                    courseSessions = courseSessions.OrderByDescending(s => s.Cours.CourseName);
                    break;
                case "name_desc":
                    courseSessions = courseSessions.OrderByDescending(s => s.CourseStartDate);
                    break;

                default:  // Name ascending 
                    courseSessions = courseSessions.OrderBy(s => s.Cours.CourseID);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(courseSessions.ToPagedList(pageNumber, pageSize) );
            //return View(courseSessions.ToList());
        }

  

        // GET: CourseSessions/Create
        public ActionResult Create()
        {
            ViewBag.CourseID = new SelectList(db.Courses, "CourseID", "CourseName");
            return View();
        }

        // POST: CourseSessions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Sessionid,CourseID,CourseDuration,CourseStartTime,CourseEndTime,CourseStartDate,CourseEndDate,CourseLocation,SessionName,CoursePrerequisite")] CourseSession courseSession)
        {
            if (ModelState.IsValid)
            {
                db.CourseSessions.Add(courseSession);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CourseID = new SelectList(db.Courses, "CourseID", "CourseName", courseSession.CourseID);
            return View(courseSession);
        }

        // GET: CourseSessions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseSession courseSession = db.CourseSessions.Find(id);
            if (courseSession == null)
            {
                return HttpNotFound();
            }
            ViewBag.CourseID = new SelectList(db.Courses, "CourseID", "CourseName", courseSession.CourseID);
            return View(courseSession);
        }

        // POST: CourseSessions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Sessionid,CourseID,CourseDuration,CourseStartTime,CourseEndTime,CourseStartDate,CourseEndDate,CourseLocation,SessionName,CoursePrerequisite")] CourseSession courseSession)
        {
            if (ModelState.IsValid)
            {
                db.Entry(courseSession).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CourseID = new SelectList(db.Courses, "CourseID", "CourseName", courseSession.CourseID);
            return View(courseSession);
        }

        // GET: CourseSessions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseSession courseSession = db.CourseSessions.Find(id);
            if (courseSession == null)
            {
                return HttpNotFound();
            }
            return View(courseSession);
        }

        // POST: CourseSessions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CourseSession courseSession = db.CourseSessions.Find(id);
            db.CourseSessions.Remove(courseSession);
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
