using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using WFPtest.Models;
using Microsoft.AspNet.Identity;
using PagedList;
using System.Data.Entity.Validation;
using System.Diagnostics;


namespace WFPtest.Controllers
{

    public class MissionAuthorizationsController : Controller
    {
        private WFPEntities1 db = new WFPEntities1();

      

        // GET: MissionAuthorizations
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
           
           

            var staffs = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
            staffs = staffs.Where(s => s.staff_email.Contains(User.Identity.Name + "@wfp.org"));

            var staffsupervisor = staffs.First().staff_supervisorid;
            var supers = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
            supers = supers.Where(d => d.staffid == staffsupervisor);

            ViewBag.supervisor = (supers.First().staff_first_name + " " + supers.First().staff_last_name);
            ViewBag.supervisorEmail = supers.First().staff_email;

          

            //ViewBag.staffid = new SelectList(db.staffs, "staffs.First().staffid", staffs.First().staff_email);
            var superid = staffs.First().staffid  ;
            ViewBag.FirstName = staffs.First().staff_first_name;
            ViewBag.LastName = staffs.First().staff_last_name;

            var result = from p in db.staffs
                         join a in db.staffs 
                         on p.staffid  equals a.staff_supervisorid

                         
                         select new
                         {
                             staffSuperName = p.staff_first_name + " " + p.staff_last_name,
                             
                            
                         };

            ViewBag.newstaffSuperName = result;
           

            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = String.IsNullOrEmpty(sortOrder) ? "First_Name" : "";
           
            var students = from s in db.MissionAuthorizations
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

            //if REGIONAL DIRECTOR
            if (staffs.First().functional_title.functional_title_description_english == "REGIONAL DIRECTOR" || staffs.First().functional_title.functional_title_description_english == "Deputy Regional Director")
            {
                var missionAuthorizations = db.MissionAuthorizations.Include(m => m.staff);
                missionAuthorizations = missionAuthorizations.Where(s => s.staff.staff_supervisorid == (superid) || s.staff.staffid == (superid) || s.staff .Country_office .office_description_english == "RBC");

                ViewBag .loginid = staffs.First().staffid;
                
               


                if (!String.IsNullOrEmpty(searchString))
                {
                    missionAuthorizations = missionAuthorizations.Where(s => s.MissionItinerary.Equals(searchString)
                                           || s.MissionObjective.Equals(searchString));

                }

                switch (sortOrder)
                {
                    case "First_Name":
                        missionAuthorizations = missionAuthorizations.OrderByDescending(s => s.FromDate);
                        break;
                    case "name_desc":
                        missionAuthorizations = missionAuthorizations.OrderByDescending(s => s.staff.staff_email);
                        break;

                    default:  // Name ascending 
                        missionAuthorizations = missionAuthorizations.OrderBy(s => s.FromDate);
                        break;
                }


                ViewBag.rdDrd = 1;
              
                int pageSize = 10;
                int pageNumber = (page ?? 1);
                return View(missionAuthorizations.ToPagedList(pageNumber, pageSize));
            } //if REGIONAL DIRECTOR

             



            var missionAuthorizationss = db.MissionAuthorizations.Include(m => m.staff);
            missionAuthorizationss = missionAuthorizationss.Where(s => s.staff.staff_supervisorid == (superid) || s.staff.staffid  == (superid));

           
            //ViewBag.staffNName = Model.;

            if (!String.IsNullOrEmpty(searchString))
            {
                missionAuthorizationss = missionAuthorizationss.Where(s => s.MissionItinerary .Equals  (searchString)
                                       || s.MissionObjective .Equals (searchString));

            }

            switch (sortOrder)
            {
                case "First_Name":
                    missionAuthorizationss = missionAuthorizationss.OrderByDescending(s => s.FromDate );
                    break;
                case "name_desc":
                    missionAuthorizationss = missionAuthorizationss.OrderByDescending(s => s.staff .staff_email );
                    break;

                default:  // Name ascending 
                    missionAuthorizationss = missionAuthorizationss.OrderBy(s => s.FromDate);
                    break;
            }


            

            int pageSizes = 10;
            int pageNumbers = (page ?? 1);
            return View(missionAuthorizationss.ToPagedList(pageNumbers, pageSizes));

            //return View(missionAuthorizations.ToList());
        }

        // GET: MissionAuthorizations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MissionAuthorization missionAuthorization = db.MissionAuthorizations.Find(id);
            if (missionAuthorization == null)
            {
                return HttpNotFound();
            }
            return View(missionAuthorization);
        }

        // GET: MissionAuthorizations/Create
        public ActionResult Create()
        {
            //ViewBag.staffid = new SelectList(User.Identity.GetUserName() + "wfp.org");
            //var username = User.Identity.GetUserName();

            var staffs = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
            staffs = staffs.Where(s => s.staff_email.Contains(User.Identity.Name + "@wfp.org"));

            //ViewBag.staffid = new SelectList(db.staffs, "staffs.First().staffid", staffs.First().staff_email);
            ViewBag.staffid = staffs.First().staffid ;
            ViewBag.staffid2 = staffs.First().staff_first_name;
            ViewBag.FirstName = staffs.First().staff_first_name;
            ViewBag.LastName = staffs.First().staff_last_name;
            ViewBag.Unit = staffs.First().unit .unit_description_english  ;
            var staffsupervisor = staffs.First().staff_supervisorid;

            var supers = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
            supers = supers.Where(d => d.staffid == staffsupervisor);

            ViewBag.supervisor = (supers.First().staff_first_name + " " + supers.First().staff_last_name);
            ViewBag.supervisorEmail = supers.First().staff_email;
            ViewBag.SignatureDate = DateTime.Now;

            return View();
        }

        // POST: MissionAuthorizations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Models.MailModel objModelMail, [Bind(Include = "MissionID,staffid,FromDate,ToDate,MissionItinerary,funding,ClearedBySupervisor,MissionObjective,ExpectedOutputs,FollowUp,SignatureDate")] MissionAuthorization missionAuthorization)
        {


            if (ModelState.IsValid)
            {

                db.MissionAuthorizations.Add(missionAuthorization);
                try

        {

                db.SaveChanges();
        }
                catch (DbEntityValidationException ex)

        {

            // Retrieve the error messages as a list of strings.

            var errorMessages = ex.EntityValidationErrors

                    .SelectMany(x => x.ValidationErrors)

                    .Select(x => x.ErrorMessage);

 

            // Join the list to a single string.

            var fullErrorMessage = string.Join("; ", errorMessages);

 

            // Combine the original exception message with the new one.

            var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

 

            // Throw a new DbEntityValidationException with the improved exception message.

            throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);

        }
                var staffs = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                staffs = staffs.Where(s => s.staff_email.Contains(User.Identity.Name + "@wfp.org"));

                //ViewBag.staffid = new SelectList(db.staffs, "staffs.First().staffid", staffs.First().staff_email);
                ViewBag.staffid = staffs.First().staffid;
                ViewBag.staffid2 = staffs.First().staff_first_name;
                ViewBag.FirstName = staffs.First().staff_first_name;
                ViewBag.LastName = staffs.First().staff_last_name;
                ViewBag.Unit = staffs.First().unit.unit_description_english;
                var staffsupervisor = staffs.First().staff_supervisorid;

                var supers = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                supers = supers.Where(d => d.staffid == staffsupervisor);

              
                var to = supers.First().staff_email;
               

                MailMessage mail = new MailMessage();
                mail.To.Add(to);
                mail.From = new MailAddress("Mission_Authorization_Form@wfp.org");
                mail.Subject = "Mission Authorization Form";
                string Body = "Request : Mission &nbsp;  &nbsp;  &nbsp; &nbsp;  &nbsp; &nbsp; &nbsp; &nbsp;  Mission ID : " + missionAuthorization.MissionID + " <br> " + " From &nbsp; &nbsp; &nbsp;:" + staffs.First().staff_first_name + "&nbsp;" + staffs.First().staff_last_name + "<br><br>" + " Mission Information" + "<br><br>" + " From :" + missionAuthorization.FromDate + "&nbsp; &nbsp; &nbsp; &nbsp; To :" + missionAuthorization.ToDate +
                    "<br>Funding :" + missionAuthorization.funding + "&nbsp;  &nbsp; &nbsp;  &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;" + "Objective :" + missionAuthorization.MissionObjective;
                mail.Body = Body;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtprelay.global.wfp.org";
                smtp.Port = 25;
                smtp.UseDefaultCredentials = true;
                //smtp.Credentials = new System.Net.NetworkCredential
                //("ahmed.badr", "Survivor2323");// Enter seders User name and password  
                //smtp.EnableSsl = true;
                smtp.Send(mail);
               

                //string from = "ahmed.badr@wfp.org"; 
                ////Replace this with your own correct Gmail Address

                //string to = "ahmed.badr@wfp.org"; 
                ////Replace this with the Email Address 
                ////to whom you want to send the mail

                //System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                //mail.To.Add(to);
                //mail.From = new MailAddress(from);
                //mail.Subject = "This is a test mail";
                //mail.SubjectEncoding = System.Text.Encoding.UTF8;
                //mail.Body = "This is Email Body Text";
                //mail.BodyEncoding = System.Text.Encoding.UTF8;
                //mail.IsBodyHtml = true;
                //mail.Priority = MailPriority.High;

                //SmtpClient client = new SmtpClient("smtp.WFPEGSIMSP01.global.wfp.org", 25);

                ////Add the Creddentials- use your own email id and password
                //System.Net.NetworkCredential nt =
                //new System.Net.NetworkCredential("ahmed.badr", "Survivor2323");

                //client.Port = 25; // Gmail works on this port
                //client.EnableSsl = false; //Gmail works on Server Secured Layer
                //client.UseDefaultCredentials = true;
                //client.Credentials = nt;
                //client.Send(mail);


                //    var body = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";
                //    var message = new MailMessage();
                //    message.To.Add(new MailAddress("ahmed.badr@wfp.org"));  // replace with valid value 
                //    message.From = new MailAddress("ahmed.badr@wfp.org");  // replace with valid value
                //    message.Subject = "Your email subject";
                //    message.Body = string.Format(body,"hhh","hjkjh","kjlkjl");
                //    message.IsBodyHtml = true;

                //    using (var smtp = new SmtpClient())
                //    {
                //        var credential = new NetworkCredential
                //        {
                //            UserName = "ahmed.badr",  // replace with valid value
                //            Password = "Survivor2323"  // replace with valid value
                //        };
                //        smtp.Credentials = credential;
                //        smtp.Host = "smtprelay.global.wfp.org";
                //        smtp.Port = 25;
                //        smtp.EnableSsl = true;
                //        smtp.SendMailAsync(message);


                    return RedirectToAction("Index");


                //    }

                //}

            }

            //ViewBag.staffid = User.Identity.GetUserName() + "@wfp.org";
            return RedirectToAction("Create");
        }

        // GET: MissionAuthorizations/Edit/5
        public ActionResult Edit(int? id ,int? id2,int?drd,int? loginid)
        {

            var staffs = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
            staffs = staffs.Where(s => s.staff_email.Contains(User.Identity.Name + "@wfp.org"));

            //ViewBag.staffid = new SelectList(db.staffs, "staffs.First().staffid", staffs.First().staff_email);
            ViewBag.staffid22 = staffs.First().staffid;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                //string passingURL = "/MissionAuthorizations/Index/";
                //return Redirect(passingURL);
            }

            if (id2 != ViewBag.staffid22)
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                //string passingURL = "/MissionAuthorizations/SuperEdit/";
                //return Redirect(passingURL? id);

                return RedirectToAction("SuperEdit", "MissionAuthorizations", new { id = id ,id2 = id2,drd =drd,loginid=loginid});
            }


            

            MissionAuthorization missionAuthorization = db.MissionAuthorizations.Find(id);
            if (missionAuthorization == null)
            {
                return HttpNotFound();
            }


            ViewBag.staffid = staffs.First().staffid;
           
            return View(missionAuthorization);
        }

        // POST: MissionAuthorizations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MissionID,staffid,FromDate,ToDate,MissionItinerary,funding,ClearedBySupervisor,MissionObjective,ExpectedOutputs,Achievements,FollowUp,SignatureDate")] MissionAuthorization missionAuthorization)
        {
           if (ModelState.IsValid)
            {
           
                db.Entry(missionAuthorization).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");

            }
          
            var staffs = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
            staffs = staffs.Where(s => s.staff_email.Contains(User.Identity.Name + "@wfp.org"));

            ViewBag.staffid = staffs.First().staffid;
            ViewBag.FirstNme = staffs.First().staff_first_name ;
            ViewBag.LastName = staffs.First().staff_last_name;

            return View(missionAuthorization);
        }



        // GET: MissionAuthorizations/SuperEdit/5
        public ActionResult SuperEdit(int? id, int? id2, int? drd)
        {

            var staffs = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
            staffs = staffs.Where(s => s.staff_email.Contains(User.Identity.Name + "@wfp.org"));

            //ViewBag.staffid = new SelectList(db.staffs, "staffs.First().staffid", staffs.First().staff_email);
            ViewBag.staffid22 = staffs.First().staffid;
            //ViewBag.RDORDRDDate = DateTime.Now;
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                //string passingURL = "/MissionAuthorizations/Index/";
                //return Redirect(passingURL);
            }

            //if (id2 != ViewBag.staffid22)
            //{
            //    //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //    string passingURL = "/MissionAuthorizations/Index/";
            //    return Redirect(passingURL);
            //}


            MissionAuthorization missionAuthorization = db.MissionAuthorizations.Find(id);
           
           
            if (missionAuthorization == null)
            {
                return HttpNotFound();
            }
            if (drd.ToString () == "1")
            {
                ViewBag.RDORDRDDate = DateTime.Now;
                ViewBag.RDORDRDSignature = missionAuthorization.RDORDRDSignature;
                ViewBag.staffid = staffs.First().staffid;
                return View(missionAuthorization);
            }
           
            ViewBag.RDORDRDSignature = missionAuthorization.RDORDRDSignature;
            ViewBag.RDORDRDDate = missionAuthorization.RDORDRDDate;
            ViewBag.staffid = staffs.First().staffid;
            return View(missionAuthorization);
        }

        // POST: MissionAuthorizations/SuperEdit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SuperEdit([Bind(Include = "MissionID,staffid,FromDate,ToDate,MissionItinerary,funding,ClearedBySupervisor,MissionObjective,ExpectedOutputs,Achievements,FollowUp,StaffSignature,SignatureDate,RDORDRDSignature,RDORDRDDate")] MissionAuthorization missionAuthorization)
        {
           
            if (ModelState.IsValid)
            {
                db.Entry(missionAuthorization).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

           

            var staffs = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
            staffs = staffs.Where(s => s.staff_email.Contains(User.Identity.Name + "@wfp.org"));

            ViewBag.staffid = staffs.First().staffid;
            ViewBag.FirstNme = staffs.First().staff_first_name;
            ViewBag.LastName = staffs.First().staff_last_name;

            return RedirectToAction("SuperEdit");

            //return View(missionAuthorization);
        }










        // GET: MissionAuthorizations/Delete/5
        public ActionResult Delete(int? id,int? id2)

        {
            var staffs = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
            staffs = staffs.Where(s => s.staff_email.Contains(User.Identity.Name + "@wfp.org"));

            //ViewBag.staffid = new SelectList(db.staffs, "staffs.First().staffid", staffs.First().staff_email);
            ViewBag.staffid22 = staffs.First().staffid;

            if (id == null || id2 != ViewBag.staffid22)
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                string passingURL = "/MissionAuthorizations/Index/";
                return Redirect(passingURL);
            }
            MissionAuthorization missionAuthorization = db.MissionAuthorizations.Find(id);
            if (missionAuthorization == null)
            {
                return HttpNotFound();
            }
            return View(missionAuthorization);
        }

        // POST: MissionAuthorizations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MissionAuthorization missionAuthorization = db.MissionAuthorizations.Find(id);
            db.MissionAuthorizations.Remove(missionAuthorization);
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
