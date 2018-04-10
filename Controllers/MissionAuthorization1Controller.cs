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
using System.IO;
using Hangfire;
using System.Collections;
using System.DirectoryServices.AccountManagement;

namespace WFPtest.Controllers
{
    public class MissionAuthorization1Controller : Controller
    {
        private WFPEntities1 db = new WFPEntities1();

        // GET: MissionAuthorization1
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, string node)
        {
            //var missionAuthorization1 = db.MissionAuthorization1.Include(m => m.staff);
            //return View(missionAuthorization1.ToList());

          

            Debug.WriteLine("id=" + node);
            var list = new List<Node>();
            if (node == null)
            {
                var items = Directory.GetFileSystemEntries("C:/");
                foreach (var item in items)
                {
                    list.Add(new Node(Path.GetFileName(item), item, Directory.Exists(item)));
                }
            }
            else
            {
                var items = Directory.GetFileSystemEntries(node);
                foreach (var item in items)
                {
                      list.Add(new Node(Path.GetFileName(item), item, Directory.Exists(item)));
                      
                   
                }
            }

            ViewBag.nnode = list;
          
            var staffs = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
            staffs = staffs.Where(s => s.staff_email.Contains(User.Identity.Name + "@wfp.org"));

           
            var superid = staffs.First().staffid; 
            
            if (staffs.First().staff_supervisorid == null)
            {
                staffs.First().staff_supervisorid = 1212;
            }

            if (staffs.First().staff_first_name  == null)
            {
                staffs.First().staff_first_name  = "no Name";
            }

            if (staffs.First().staff_last_name  == null)
            {
                staffs.First().staff_last_name  = "no Name";
            }

            if (staffs.First().unit_id  == null)
            {
                staffs.First().unit_id  = 15;
            }
            
            var staffsupervisor = staffs.First().staff_supervisorid;
            var supers = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
            supers = supers.Where(d => d.staffid == staffsupervisor);

            ViewBag.supervisor = (supers.First().staff_first_name + " " + supers.First().staff_last_name);
            ViewBag.supervisorEmail = supers.First().staff_email;

          

            //ViewBag.staffid = new SelectList(db.staffs, "staffs.First().staffid", staffs.First().staff_email);

           

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

            var students = from s in db.MissionAuthorization1
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
                   //(staffs.First().functional_title.functional_title_description_english == "REGIONAL DIRECTOR" || staffs.First().functional_title.functional_title_description_english == "Deputy Regional Director")


            var staffser = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
            staffser = staffser.Where(s => s.staff_email.Contains(User.Identity.Name + "@wfp.org"));
            var userUnit = staffser.First().unit_id;

            var unithou = db.units.Include(d => d.staffs);
            unithou = unithou.Where(u => u.unitid == userUnit);
            var houid = unithou.First().HOU_ID;

            if (staffser.First().staffid == houid)
            {
                var missionAuthorizations = db.MissionAuthorization1.Include(m => m.staff);
                missionAuthorizations = missionAuthorizations.Where(s => s.staff.staff_supervisorid == (superid) || s.staff.staffid == (superid) || s.staff.unit_id == userUnit).OrderByDescending(s => s.MissionID); 

               
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



            var missionAuthorizationss = db.MissionAuthorization1.Include(m => m.staff);
            missionAuthorizationss = missionAuthorizationss.Where(s => s.staff.staff_supervisorid == (superid) || s.staff.staffid == (superid) || s.staffonbehalf == s.staff .staffid );

            var missionAuthorizationssOnbehalf = db.MissionAuthorization1.Include(m => m.staff);
            missionAuthorizationssOnbehalf = missionAuthorizationssOnbehalf.Where(s => s.staffonbehalf == s.staff.staffid);

            var onbehafname = "";


            if (missionAuthorizationssOnbehalf.Any())
            {
                onbehafname = missionAuthorizationssOnbehalf.First().staff.staff_first_name + " " + missionAuthorizationssOnbehalf.First().staff.staff_last_name;
            }

            onbehafname = "";
           
            //ViewBag.staffNName = Model.;

            if (!String.IsNullOrEmpty(searchString))
            {
                missionAuthorizationss = missionAuthorizationss.Where(s => s.MissionItinerary.Equals(searchString)
                                       || s.MissionObjective.Contains(searchString)
                                       || s.staff.staff_first_name.Contains(searchString)
                                       || s.staff.staff_last_name.Contains(searchString));
                                       
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

       


        ///////////////////////////////////weekly report

        public ActionResult WeeklyReport(string sortOrder, string currentFilter, string searchString, int? page, string node)
        {

            Debug.WriteLine("id=" + node);
            var list = new List<Node>();
            if (node == null)
            {
                var items = Directory.GetFileSystemEntries("C:/");
                foreach (var item in items)
                {
                    list.Add(new Node(Path.GetFileName(item), item, Directory.Exists(item)));
                }
            }
            else
            {
                var items = Directory.GetFileSystemEntries(node);
                foreach (var item in items)
                {
                    list.Add(new Node(Path.GetFileName(item), item, Directory.Exists(item)));


                }
            }

            ViewBag.nnode = list;

          


       

            var result = from p in db.staffs
                         join a in db.staffs
                         on p.staffid equals a.staff_supervisorid


                         select new
                         {
                             staffSuperName = p.staff_first_name + " " + p.staff_last_name,


                         };

            ViewBag.newstaffSuperName = result;


            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = String.IsNullOrEmpty(sortOrder) ? "First_Name" : "";

            var students = from s in db.MissionAuthorization1
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
            //(staffs.First().functional_title.functional_title_description_english == "REGIONAL DIRECTOR" || staffs.First().functional_title.functional_title_description_english == "Deputy Regional Director")



            var missionAuthorizations = db.MissionAuthorization1.Include(m => m.staff);
                //missionAuthorizations = missionAuthorizations.Where(s => s.staff.staff_supervisorid == (superid) || s.staff.staffid == (superid) || s.staff.unit_id == userUnit);

               




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


                   

            int pageSizes = 10;
            int pageNumbers = (page ?? 1);
            return View(missionAuthorizations.ToPagedList(pageNumbers, pageSizes));

            //return View(missionAuthorizations.ToList());
        
        }

        // GET: MissionAuthorization1/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MissionAuthorization1 missionAuthorization1 = db.MissionAuthorization1.Find(id);
            if (missionAuthorization1 == null)
            {
                return HttpNotFound();
            }
            return View(missionAuthorization1);
        }

        // GET: MissionAuthorization1/Create
        public ActionResult Create()
        {
            //ViewBag.staffid = new SelectList(db.staffs, "staffid", "staff_id");
            //return View();


            if (!Request.IsAuthenticated)
            {
                Response.Redirect("/RBC-SMS/account/login");
            }
            //ViewBag.staffid = new SelectList(User.Identity.GetUserName() + "wfp.org");
            //var username = User.Identity.GetUserName();


            var staffs = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
            staffs = staffs.Where(s => s.staff_email == User.Identity.Name + "@wfp.org");
            var stafemal11 = staffs.First().staff_email;
            var staffforhouforsuper = staffs.First().staffid;


            if (staffs.First().staff_supervisorid == null || staffs.First().staff_first_name == null || staffs.First().unit_id == null || staffs.First().unit_id == null)
            {

                ///////////////////////

                var selectmanagerindex = db.EMPLOYEES.Include(emp => emp.staff);
                selectmanagerindex = selectmanagerindex.Where(emp => emp.EMAIL_ADDRESS == stafemal11);
                //var empEmail = selectmanagerindex.First().EMAIL_ADDRESS;
                var managerindex = selectmanagerindex.FirstOrDefault().MANAGER;
                var Depid = selectmanagerindex.First().DEPID;
                var firstname = selectmanagerindex.First().EMP_FIRST_NAME;
                var lastname = selectmanagerindex.First().EMP_LAST_NAME;
                var IndexNumber = selectmanagerindex.First().EMP_ID;


                var updatefirslastname = db.staffs.Single(u => u.staff_email == stafemal11);
                updatefirslastname.staff_first_name = firstname;
                updatefirslastname.staff_id = IndexNumber;
                updatefirslastname.staff_index_number = IndexNumber;
                if (lastname == null)
                {
                    updatefirslastname.staff_last_name = lastname;
                }

                db.SaveChanges();

                var SuperForEmpM = db.EMPLOYEES.Include(s => s.staff);
                SuperForEmpM = SuperForEmpM.Where(s => s.EMP_ID == managerindex);
                var mangerEmail = SuperForEmpM.First().EMAIL_ADDRESS;



                var staffscompmid = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                staffscompmid = staffscompmid.Where(s => s.staff_email == mangerEmail);
                var managerid = staffscompmid.First().staffid;


                var staffmanagerupdate1 = db.staffs.Single(u => u.staff_email == stafemal11);
                staffmanagerupdate1.staff_supervisorid = managerid;
                db.SaveChanges();

                var UNITForEmpM = db.DEPARTMENTS.Include(s => s.EMPLOYEES);
                UNITForEmpM = UNITForEmpM.Where(s => s.DEP_ID == Depid);
                var unitnameforemp = UNITForEmpM.First().DEP_NAME;
                var dephouindex = UNITForEmpM.First().DEP_MANAGER;

                var emailstaffforhou = db.EMPLOYEES.Include(s => s.staff);
                emailstaffforhou = emailstaffforhou.Where(s => s.EMP_ID == dephouindex);
                var mailhouemp = emailstaffforhou.First().EMAIL_ADDRESS;

                var staffhouid = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                staffhouid = staffhouid.Where(s => s.staff_email == mailhouemp);
                var houidEmp = staffscompmid.First().staffid;

                var staffscompmidfornameunit = db.units.Include(s => s.Country_office);
                staffscompmidfornameunit = staffscompmidfornameunit.Where(s => s.unit_description_english == unitnameforemp);

                if (staffscompmidfornameunit.Any())
                {
                    int unitid = staffscompmidfornameunit.First().unitid;
                    var staffmanagerupdate = db.staffs.Single(u => u.staff_email == stafemal11);
                    staffmanagerupdate.unit_id = unitid;
                    db.SaveChanges();

                }
                else
                {

                    var addunit = db.units.Add(new unit { unit_description_english = unitnameforemp }).unit_description_english;
                    db.SaveChanges();



                    var selectnewunit = db.units.Include(s => s.Country_office);
                    selectnewunit = selectnewunit.Where(s => s.unit_description_english == unitnameforemp);
                    var newunitid = selectnewunit.First().unitid;

                    var houfornewunit = db.units.Single(u => u.unitid == newunitid);
                    houfornewunit.HOU_ID = houidEmp;
                    db.SaveChanges();

                    var staffudateunitid = db.staffs.Single(u => u.staff_email == stafemal11);
                    staffudateunitid.unit_id = newunitid;
                    db.SaveChanges();

                }


                /////////////////////

                //ModelState.AddModelError("staffid2", "Name is required");
                //ViewBag.staffid2 = "Name is required";

                //var staffemail = staffs.First().staff_email;
                //var stafffullname = staffs.First ().staff_email  ;
                //var staffindex = staffs.First().staff_index_number;



                //MailMessage mailhou = new MailMessage();
                //mailhou.To.Add("ahmed.badr@wfp.org"); // odc ict
                //mailhou.CC.Add(staffemail);
                //mailhou.Bcc.Add("ahmed.badr@wfp.org");
                //mailhou.From = new MailAddress("RBS-no-replay@wfp.org");
                //mailhou.Subject = "Missing Information";

                //string Bodyhou = "<br>We missing Important information from " + stafffullname 
                //    + " index number " + staffindex
                //    + " &nbsp; record Please  update the data Like First Name, Last Name , Unit , Supervisor <br><br><font color='red'><font size='2' color='blue'>RBC SMS</font> This is an automatically generated email, please do not reply.</font>";

                ////string Body = "Request : Mission &nbsp;  &nbsp;  &nbsp; &nbsp;  &nbsp; &nbsp; &nbsp; &nbsp;  Mission ID : " + missionAuthorization.MissionID + " <br> " + " From &nbsp; &nbsp; &nbsp;:" + staffs.First().staff_first_name + "&nbsp;" + staffs.First().staff_last_name + "<br><br>" + " Mission Information" + "<br><br>" + " From :" + missionAuthorization.FromDate + "<br>To :" + missionAuthorization.ToDate +
                ////    "<br>Funding :" + missionAuthorization.funding + "<br>Objective :" + missionAuthorization.MissionObjective + "<br> Mission Itinerary :" + missionAuthorization.MissionItinerary + "<br> Can This meeting Be Done Over Voice/Video ? :" + missionAuthorization.canbedone  + " <br><br> <a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor=" + true + "&FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + " &to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Acept</a>&nbsp;&nbsp;&nbsp;<a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor= " + false + "& FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + "&to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Reject</a> ";

                //mailhou.Body = Bodyhou;
                //mailhou.IsBodyHtml = true;
                //SmtpClient smtphou = new SmtpClient();
                //smtphou.Host = "smtprelay.global.wfp.org";
                //smtphou.Port = 25;
                //smtphou.UseDefaultCredentials = true;
                ////smtp.Credentials = new System.Net.NetworkCredential
                ////("ahmed.badr", "Survivor2323");// Enter seders User name and password  
                ////smtp.EnableSsl = true;
                //smtphou.Send(mailhou);




            }

            var superid = staffs.First().staffid;


            if (staffs.First().staff_supervisorid == null)
            {
                staffs.First().staff_supervisorid = 1212;
                ViewBag.error = 1;
            }

            if (staffs.First().staff_first_name == null)
            {
                staffs.First().staff_first_name = "You must have First name !!!";
                ViewBag.error = 1;
            }

            if (staffs.First().staff_last_name == null)
            {
                staffs.First().staff_last_name = "You must have Last name !!!";
                ViewBag.error = 1;
            }


            //ViewBag.staffid = new SelectList(db.staffs, "staffs.First().staffid", staffs.First().staff_email);
            ViewBag.staffid = staffs.First().staffid;
            ViewBag.staffid2 = staffs.First().staff_first_name;
            ViewBag.FirstName = staffs.First().staff_first_name;
            ViewBag.LastName = staffs.First().staff_last_name;

            if (staffs.First().unit_id == null)
            {
                ViewBag.Unit = "You must have a Unit !!!!";
                staffs.First().unit_id = 15;
                ViewBag.error = 1;

            }
            else
            {

                ViewBag.Unit = staffs.First().unit.unit_description_english;
            }
            var staffsupervisor = staffs.First().staff_supervisorid;

            var userUnit = staffs.First().unit_id;

            var supers = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
            supers = supers.Where(d => d.staffid == staffsupervisor);





            ViewBag.supervisor = (supers.First().staff_first_name + " " + supers.First().staff_last_name);


            var supervisoremailif = supers.First().staff_email;
            if (supervisoremailif == "muhannad.hadi@wfp.org" || supervisoremailif == "carlo.scaramella@wfp.org")
            {
                supervisoremailif = "rbc.management@wfp.org"; // rbc.management@wfp.org

            }

            ViewBag.supervisorEmail = supervisoremailif;
            ViewBag.SignatureDate = DateTime.Now;



            var unithou = db.units.Include(d => d.staffs);
            unithou = unithou.Where(u => u.unitid == userUnit);
            var houid = unithou.First().HOU_ID;

            var staffshou = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
            staffshou = staffshou.Where(h => h.staffid == houid);

            //////////////////////////////
            var superforhounitsname = "";
            var houmailUPDATE = "";
            var houidforSuper = staffshou.First().staffid;
            if (houidforSuper == staffforhouforsuper)
            {
                var supersforhou = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                supersforhou = supersforhou.Where(d => d.staffid == staffsupervisor);
                superforhounitsname = supersforhou.First().staff_first_name + " " + staffshou.First().staff_last_name;
                houmailUPDATE = supersforhou.First().staff_email;

            }
            ///////////////////////////////

            var to = staffshou.First().staff_email;

            superforhounitsname = staffshou.First().staff_first_name + " " + staffshou.First().staff_last_name;
            ViewBag.houname = superforhounitsname;

            houmailUPDATE = staffshou.First().staff_email;

            if (houmailUPDATE == "muhannad.hadi@wfp.org" || houmailUPDATE == "carlo.scaramella@wfp.org")
            {
                houmailUPDATE = "rbc.management@wfp.org"; //rbc.management@wfp.org

            }


            ViewBag.houmail = houmailUPDATE;



            return View();
        }

        // POST: MissionAuthorization1/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MissionID,staffid,FromDate,ToDate,MissionItinerary,funding,ClearedBySupervisor,MissionObjective,ExpectedOutputs,Achievements,FollowUp,StaffSignature,SignatureDate,RDORDRDSignature,RDORDRDDate,canbedone,staffonbehalf,nnon1,non2,non3,non4,non5,Signature1,Signature1Date,Signature2,Signature2Date,Signature3,Signature3Date,BSITF,Bexpiredate,ASITF,Aexpiredate,SSAFE,Sexpiredate")] MissionAuthorization1 missionAuthorization1)
        {
            if (ModelState.IsValid)
            {
                //db.MissionAuthorization1.Add(missionAuthorization1);
                //db.SaveChanges();
                //return RedirectToAction("Index");



                if (ModelState.IsValid)
                {

                    db.MissionAuthorization1.Add(missionAuthorization1);
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
                    var userUnit = staffs.First().unit_id;


                    var from = staffs.First().staff_email;
                    var supers = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                    supers = supers.Where(d => d.staffid == staffsupervisor);


                    var unithou = db.units.Include(d => d.staffs);
                    unithou = unithou.Where(u => u.unitid == userUnit);
                    var houid = unithou.First().HOU_ID;

                    var staffshou = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                    staffshou = staffshou.Where(h => h.staffid == houid);
                    var to = staffshou.First().staff_email;
                    //var houname = staffshou.First().staff_first_name + " " + staffshou.First().staff_last_name;


                    var cc = supers.First().staff_email;
                    var supername = supers.First().staff_first_name + " " + supers.First().staff_last_name;


                    if (staffs.First().staffid == houid)
                    {
                        var canbedone = "";
                        if (missionAuthorization1.canbedone == true)
                        {
                            canbedone = "Yes";
                        }
                        else
                        {
                            canbedone = "No";
                        }

                        var CleredBySupervisor = "";
                        if (missionAuthorization1.canbedone == true)
                        {
                            CleredBySupervisor = "Yes";
                        }
                        else
                        {
                            CleredBySupervisor = "No";
                        }

                        var BSITF2 = "";
                        if (missionAuthorization1.BSITF == true)
                        {
                            BSITF2 = "Yes";
                        }
                        else
                        {
                            BSITF2 = "No";
                        }
                        ////////  
                        var ASITF2 = "";
                        if (missionAuthorization1.ASITF == true)
                        {
                            ASITF2 = "Yes";
                        }
                        else
                        {
                            ASITF2 = "No";
                        }
                        ////////
                        var SSAFE2 = "";
                        if (missionAuthorization1.SSAFE == true)
                        {
                            SSAFE2 = "Yes";
                        }
                        else
                        {
                            SSAFE2 = "No";
                        }

                        //////////////////////////////


                        var supersforhou = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                        supersforhou = supersforhou.Where(d => d.staffid == staffsupervisor);
                        var superforhounitsname = supersforhou.First().staff_first_name + " " + staffshou.First().staff_last_name;
                        var houmailUPDATE = supersforhou.First().staff_email;

                        if (houmailUPDATE == "muhannad.hadi@wfp.org" || houmailUPDATE == "carlo.scaramella@wfp.org")
                        {
                            houmailUPDATE = "rbc.management@wfp.org"; //rbc.management@wfp.org

                        }
                        /////////////////////////

                        MailMessage mailhou = new MailMessage();
                        mailhou.To.Add("ahmed.badr@wfp.org"); // RD/DRD OFFICE/rbc.management@wfp.org
                        //mail.CC.Add(cc);
                        mailhou.Bcc.Add("ahmed.badr@wfp.org");
                        mailhou.From = new MailAddress(from);
                        mailhou.Subject = "Mission Authorization Form crEATE";

                        string Bodyhou = "<table><tr><td colspan='2'>" + "<hr/>New Mission<hr/></td></tr><tr><td><font color='blue'>Request : </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorization1.MissionID + "</td></tr>"
                            + "<tr><td>" + "<font color='blue'>From :</font></td><td>" + staffs.First().staff_first_name + " " + staffs.First().staff_last_name + "</td></tr>"
                            + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                            + "<tr><td><font color='blue'>From :</font></td><td>" + missionAuthorization1.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                            + "<tr><td><font color='blue'>To :</font></td><td>" + missionAuthorization1.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                            + "<tr><td><font color='blue'>Funding :</font></td><td>" + missionAuthorization1.funding + "</td></tr>"
                            + "<tr><td><font color='blue'>Objective :</font></td><td>" + missionAuthorization1.MissionObjective + "</td></tr>"
                            + "<tr><td><font color='blue'>Mission Itinerary :</font></td><td>" + missionAuthorization1.MissionItinerary + "</td></tr>"
                            + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video ? :</font></td><td>" + canbedone + "</td></tr>"
                            + "<tr><td><font color='blue'>Cleared By Supervisor :</font></td><td>" + CleredBySupervisor + "</td></tr>"

                            + "<tr><td><font color='blue'>	BSITF :</font></td><td>" + BSITF2 + "</td></tr>"
                            + "<tr><td><font color='blue'> ASITF :</font></td><td>" + ASITF2 + "</td></tr>"
                            + "<tr><td><font color='blue'> SSAFE :</font></td><td>" + SSAFE2 + "</td></tr>"
                            + "<tr><td><font color='blue'> DSS Approval:</font></td><td>" + "<a href='mailto: @Html.Display('houmail', new { @id = " + missionAuthorization1.non6 +"  })'> @Html.Display('houmail', new { @id = 'houmail' })</a>" + "</td></tr>"

                            + "<tr><td><a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid="
                            + missionAuthorization1.MissionID
                            + "&RDORDRDSignature=Yes&FromDate=" + missionAuthorization1.FromDate
                            + " &ToDate=" + missionAuthorization1.ToDate
                            + " &to=" + to + "&staffmail=" + staffs.First().staff_email
                            + "'>Accept</a></td><td><a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization1.MissionID
                            + "&RDORDRDSignature=NO & FromDate=" + missionAuthorization1.FromDate
                            + " &ToDate=" + missionAuthorization1.ToDate
                            + "&to=" + to
                            + "&staffmail=" + staffs.First().staff_email
                            + "'>Reject</a>" + "</td><td>" + "</td></tr></table>";

                        //string Body = "Request : Mission &nbsp;  &nbsp;  &nbsp; &nbsp;  &nbsp; &nbsp; &nbsp; &nbsp;  Mission ID : " + missionAuthorization.MissionID + " <br> " + " From &nbsp; &nbsp; &nbsp;:" + staffs.First().staff_first_name + "&nbsp;" + staffs.First().staff_last_name + "<br><br>" + " Mission Information" + "<br><br>" + " From :" + missionAuthorization.FromDate + "<br>To :" + missionAuthorization.ToDate +
                        //    "<br>Funding :" + missionAuthorization.funding + "<br>Objective :" + missionAuthorization.MissionObjective + "<br> Mission Itinerary :" + missionAuthorization.MissionItinerary + "<br> Can This meeting Be Done Over Voice/Video ? :" + missionAuthorization.canbedone  + " <br><br> <a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor=" + true + "&FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + " &to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Acept</a>&nbsp;&nbsp;&nbsp;<a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor= " + false + "& FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + "&to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Reject</a> ";

                        mailhou.Body = Bodyhou;
                        mailhou.IsBodyHtml = true;
                        SmtpClient smtphou = new SmtpClient();
                        smtphou.Host = "smtprelay.global.wfp.org";
                        smtphou.Port = 25;
                        smtphou.UseDefaultCredentials = true;
                        //smtp.Credentials = new System.Net.NetworkCredential
                        //("ahmed.badr", "Survivor2323");// Enter seders User name and password  
                        //smtp.EnableSsl = true;
                        //smtphou.Send(mailhou);hhhhhhhhhhhh


                        return RedirectToAction("UploadFilesM", new { id2 = missionAuthorization1.MissionID });


                    }



                    if (to == "muhannad.hadi@wfp.org" || to == "carlo.scaramella@wfp.org")
                    {
                        to = "rbc.management@wfp.org"; //rbc.management@wfp.org

                    }


                    var canbedoneee = "";
                    if (missionAuthorization1.canbedone == true)
                    {
                        canbedoneee = "Yes";
                    }
                    else
                    {
                        canbedoneee = "No";
                    }

                    var CleredBySuper = "";
                    if (missionAuthorization1.ClearedBySupervisor == true)
                    {
                        CleredBySuper = "Yes";
                    }
                    else
                    {
                        CleredBySuper = "No";
                    }

                    var BSITF = "";
                    if (missionAuthorization1.BSITF == true)
                    {
                        BSITF = "Yes";
                    }
                    else
                    {
                        BSITF = "No";
                    }
                    ////////  
                    var ASITF = "";
                    if (missionAuthorization1.ASITF == true)
                    {
                        ASITF = "Yes";
                    }
                    else
                    {
                        ASITF = "No";
                    }
                    ////////
                    var SSAFE = "";
                    if (missionAuthorization1.SSAFE == true)
                    {
                        SSAFE = "Yes";
                    }
                    else
                    {
                        SSAFE = "No";
                    }


                    MailMessage mail = new MailMessage();
                    mail.To.Add("ahmed.badr@wfp.org"); // var to
                    //mail.CC.Add(cc);
                    mail.Bcc.Add("ahmed.badr@wfp.org");
                    mail.From = new MailAddress(from);
                    mail.Subject = "Mission Authorization Form CREATE";

                    string Body = "<table><tr><td colspan='2'>" + "<hr/>New Mission<hr/></td></tr><tr><td><font color='blue'>Request : </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorization1.MissionID + "</td></tr>"
                            + "<tr><td>" + "<font color='blue'>From :</font></td><td>" + staffs.First().staff_first_name + " " + staffs.First().staff_last_name + "</td></tr>"
                            + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                            + "<tr><td><font color='blue'>From :</font></td><td>" + missionAuthorization1.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                            + "<tr><td><font color='blue'>To :</font></td><td>" + missionAuthorization1.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                            + "<tr><td><font color='blue'>Funding :</font></td><td>" + missionAuthorization1.funding + "</td></tr>"
                            + "<tr><td><font color='blue'>Objective :</font></td><td>" + missionAuthorization1.MissionObjective + "</td></tr>"
                            + "<tr><td><font color='blue'>Mission Itinerary :</font></td><td>" + missionAuthorization1.MissionItinerary + "</td></tr>"
                            + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video ? :</font></td><td>" + canbedoneee + "</td></tr>"
                            + "<tr><td><font color='blue'>Cleared By Supervisor :</font></td><td>" + CleredBySuper + "</td></tr>"

                             + "<tr><td><font color='blue'>	BSITF :</font></td><td>" + BSITF + "</td></tr>"
                            + "<tr><td><font color='blue'> ASITF :</font></td><td>" + ASITF + "</td></tr>"
                            + "<tr><td><font color='blue'> SSAFE :</font></td><td>" + SSAFE + "</td></tr>"
                            + "<tr><td><font color='blue'> DSS Approval:</font></td><td>" + "<a href='mailto: @Html.Display('houmail', new { @id = " + missionAuthorization1.non6 + "  })'> @Html.Display('houmail', new { @id = 'houmail' })</a>" + "</td></tr>"


                            + "<tr><td><a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid="
                            + missionAuthorization1.MissionID
                            + "&RDORDRDSignature=Yes&FromDate=" + missionAuthorization1.FromDate
                            + " &ToDate=" + missionAuthorization1.ToDate
                            + " &to=" + to + "&staffmail=" + staffs.First().staff_email
                            + "'>Accept</a></td></tr><tr><td><a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization1.MissionID
                            + "&RDORDRDSignature=NO & FromDate=" + missionAuthorization1.FromDate
                            + " &ToDate=" + missionAuthorization1.ToDate
                            + "&to=" + to
                            + "&staffmail=" + staffs.First().staff_email
                            + "'>Reject</a>" + "</td></tr><tr><td>" + "</td></tr></table>";
                    //string Body = "Request : Mission &nbsp;  &nbsp;  &nbsp; &nbsp;  &nbsp; &nbsp; &nbsp; &nbsp;  Mission ID : " + missionAuthorization.MissionID + " <br> " + " From &nbsp; &nbsp; &nbsp;:" + staffs.First().staff_first_name + "&nbsp;" + staffs.First().staff_last_name + "<br><br>" + " Mission Information" + "<br><br>" + " From :" + missionAuthorization.FromDate + "<br>To :" + missionAuthorization.ToDate +
                    //    "<br>Funding :" + missionAuthorization.funding + "<br>Objective :" + missionAuthorization.MissionObjective + "<br> Mission Itinerary :" + missionAuthorization.MissionItinerary + "<br> Can This meeting Be Done Over Voice/Video ? :" + missionAuthorization.canbedone  + " <br><br> <a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor=" + true + "&FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + " &to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Acept</a>&nbsp;&nbsp;&nbsp;<a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor= " + false + "& FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + "&to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Reject</a> ";

                    mail.Body = Body;
                    mail.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = "smtprelay.global.wfp.org";
                    smtp.Port = 25;
                    smtp.UseDefaultCredentials = true;
                    //smtp.Credentials = new System.Net.NetworkCredential
                    //("ahmed.badr", "Survivor2323");// Enter seders User name and password  
                    //smtp.EnableSsl = true;
                    //smtp.Send(mail);



                    //BackgroundJob.Schedule(() => smtp.Send(mail), TimeSpan.FromMinutes(10));
                    //RecurringJob.AddOrUpdate(() => GetADGroupUsers("ss"), Cron.Minutely);
                    //RecurringJob.AddOrUpdate(() => Console.WriteLine(), Cron.Minutely);
                    //RecurringJob.AddOrUpdate(mail,Cron.Minutely);
                    //RecurringJob.AddOrUpdate(() => Storage.PunchIt(), Cron.Minutely);

                    //RecurringJob.AddOrUpdate(() => Storage.PunchIt(), Cron.Minutely);

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


                    return RedirectToAction("UploadFilesM", new { id2 = missionAuthorization1.MissionID });


                    //    }

                    //}

                }

                //ViewBag.staffid = User.Identity.GetUserName() + "@wfp.org";
                return RedirectToAction("Create");
            }

            ViewBag.staffid = new SelectList(db.staffs, "staffid", "staff_id", missionAuthorization1.staffid);
            return View(missionAuthorization1);
        }

        // GET: MissionAuthorization1/Edit/5
        public ActionResult Edit(int? id, int? id2, int? drd, int? loginid, int? behalfOf)
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

            if (behalfOf != null)
            {
                return RedirectToAction("EditBehalfof", "MissionAuthorizations", new { id = id, id2 = id2, drd = drd, loginid = loginid });
            }
            if (id2 != ViewBag.staffid22)
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                //string passingURL = "/MissionAuthorizations/SuperEdit/";
                //return Redirect(passingURL? id);

                 RedirectToAction("SuperEdit", "MissionAuthorization1", new { id = id, id2 = id2, drd = drd, loginid = loginid });
            }




            MissionAuthorization1 missionAuthorization1 = db.MissionAuthorization1.Find(id);
            if (missionAuthorization1 == null)
            {
                return HttpNotFound();
            }


            ViewBag.staffid = staffs.First().staffid;
            return View(missionAuthorization1);
        }

        // POST: MissionAuthorization1/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MissionID,staffid,FromDate,ToDate,MissionItinerary,funding,ClearedBySupervisor,MissionObjective,ExpectedOutputs,Achievements,SignatureDate,FollowUp,StaffSignature,RDORDRDSignature,RDORDRDDate,canbedone,staffonbehalf,nnon1,non2,non3,non4,non5,Signature1,Signature1Date,Signature2,Signature2Date,Signature3,Signature3Date,BSITF,Bexpiredate,ASITF,Aexpiredate,SSAFE,Sexpiredate")] MissionAuthorization1 missionAuthorization1)
        {
            if (ModelState.IsValid)
            {


                db.Entry(missionAuthorization1).State = EntityState.Modified;
                db.SaveChanges();

                ////////////////////  emails /////////////////////


                var staffsedit = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                staffsedit = staffsedit.Where(s => s.staff_email.Contains(User.Identity.Name + "@wfp.org"));

                ViewBag.staffid = staffsedit.First().staffid;
                ViewBag.staffid2 = staffsedit.First().staff_first_name;
                ViewBag.FirstName = staffsedit.First().staff_first_name;
                ViewBag.LastName = staffsedit.First().staff_last_name;
                ViewBag.Unit = staffsedit.First().unit.unit_description_english;
                var staffsupervisor = staffsedit.First().staff_supervisorid;
                var userUnit = staffsedit.First().unit_id;


                var from = staffsedit.First().staff_email;
                var supers = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                supers = supers.Where(d => d.staffid == staffsupervisor);


                var unithou = db.units.Include(d => d.staffs);
                unithou = unithou.Where(u => u.unitid == userUnit);
                var houid = unithou.First().HOU_ID;

                var staffshou = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                staffshou = staffshou.Where(h => h.staffid == houid);
                var to = staffshou.First().staff_email;



                var cc = supers.First().staff_email;
                var supername = supers.First().staff_first_name + " " + supers.First().staff_last_name;


                if (staffsedit.First().staffid == houid)
                {
                    var canbedone = "";
                    if (missionAuthorization1.canbedone == true)
                    {
                        canbedone = "Yes";
                    }
                    else
                    {
                        canbedone = "No";
                    }
                    //////BSITF,Bexpiredate,ASITF,Aexpiredate,SSAFE,Sexpiredate
                    var BSITF = "";
                    if (missionAuthorization1.BSITF == true)
                    {
                        BSITF = "Yes";
                    }
                    else
                    {
                        BSITF = "No";
                    }
                    ////////  
                    var ASITF = "";
                    if (missionAuthorization1.ASITF == true)
                    {
                        ASITF = "Yes";
                    }
                    else
                    {
                        ASITF = "No";
                    }
                    ////////
                    var SSAFE = "";
                    if (missionAuthorization1.SSAFE == true)
                    {
                        SSAFE = "Yes";
                    }
                    else
                    {
                        SSAFE = "No";
                    }


                    var CleredBySupervisor = "";
                    if (missionAuthorization1.canbedone == true)
                    {
                        CleredBySupervisor = "Yes";
                    }
                    else
                    {
                        CleredBySupervisor = "No";
                    }

                    var supersforhou = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                    supersforhou = supersforhou.Where(d => d.staffid == staffsupervisor);
                    var superforhounitsname = supersforhou.First().staff_first_name + " " + staffshou.First().staff_last_name;
                    var houmailUPDATE = supersforhou.First().staff_email;

                    if (houmailUPDATE == "muhannad.hadi@wfp.org" || houmailUPDATE == "carlo.scaramella@wfp.org")
                    {
                        houmailUPDATE = "rbc.management@wfp.org";//rbc.management@wfp.org

                    }


                    MailMessage mailhou = new MailMessage();
                    mailhou.To.Add("ahmed.badr@wfp.org"); // RD/DRD OFFICE/rbc.management@wfp.org
                    mailhou.Bcc.Add("ahmed.badr@wfp.org");
                    mailhou.From = new MailAddress(from);
                    mailhou.Subject = "Mission Updated";

                    string Bodyhou = "<table><tr><td colspan='2'>" + "<hr/>Updated Mission<hr/></td></tr><tr><td><font color='blue'>Request : </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorization1.MissionID + "</td></tr>"
                        + "<tr><td>" + "<font color='blue'>From :</font></td><td>" + staffsedit.First().staff_first_name + " " + staffsedit.First().staff_last_name + "</td></tr>"
                        + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                        + "<tr><td><font color='blue'>From :</font></td><td>" + missionAuthorization1.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                        + "<tr><td><font color='blue'>To :</font></td><td>" + missionAuthorization1.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                        + "<tr><td><font color='blue'>Funding :</font></td><td>" + missionAuthorization1.funding + "</td></tr>"
                        + "<tr><td><font color='blue'>Objective :</font></td><td>" + missionAuthorization1.MissionObjective + "</td></tr>"
                        + "<tr><td><font color='blue'>Mission Itinerary :</font></td><td>" + missionAuthorization1.MissionItinerary + "</td></tr>"
                        + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video ? :</font></td><td>" + canbedone + "</td></tr>"
                        + "<tr><td><font color='blue'>Cleared By Supervisor :</font></td><td>" + CleredBySupervisor + "</td></tr>"
                        + "<tr><td><font color='blue'>Cleared By Supervisor :</font></td><td>" + BSITF + "</td></tr>"
                        + "<tr><td><font color='blue'>Cleared By Supervisor :</font></td><td>" + ASITF + "</td></tr>"
                        + "<tr><td><font color='blue'>Cleared By Supervisor :</font></td><td>" + SSAFE + "</td></tr>"
                        + "<tr><td><font color='blue'> DSS Approval:</font></td><td>" + "<a href='mailto: @Html.Display('houmail', new { @id = " + missionAuthorization1.non6 + "  })'> @Html.Display('houmail', new { @id = 'houmail' })</a>" + "</td></tr>"
                        + "<tr><td><a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid="
                        + missionAuthorization1.MissionID
                        + "&RDORDRDSignature=Yes&FromDate=" + missionAuthorization1.FromDate
                        + " &ToDate=" + missionAuthorization1.ToDate
                        + " &to=" + to + "&staffmail=" + staffsedit.First().staff_email
                        + "'>Accept</a></td></tr><tr><td><a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization1.MissionID
                        + "&RDORDRDSignature=NO & FromDate=" + missionAuthorization1.FromDate
                        + " &ToDate=" + missionAuthorization1.ToDate
                        + "&to=" + to
                        + "&staffmail=" + staffsedit.First().staff_email
                        + "'>Reject</a>" + "</td></tr><tr><td>" + "</td></tr></table>";

                    mailhou.Body = Bodyhou;
                    mailhou.IsBodyHtml = true;
                    SmtpClient smtphou = new SmtpClient();
                    smtphou.Host = "smtprelay.global.wfp.org";
                    smtphou.Port = 25;
                    smtphou.UseDefaultCredentials = true;
                    smtphou.Send(mailhou);


                    return RedirectToAction("UploadFilesM", new { id2 = missionAuthorization1.MissionID });


                }

                var canbedoneee = "";
                if (missionAuthorization1.canbedone == true)
                {
                    canbedoneee = "Yes";
                }
                else
                {
                    canbedoneee = "No";
                }

                var CleredBySupe2 = "";
                if (missionAuthorization1.canbedone == true)
                {
                    CleredBySupe2 = "Yes";
                }
                else
                {
                    CleredBySupe2 = "No";
                }

                var BSITF2 = "";
                if (missionAuthorization1.BSITF == true)
                {
                    BSITF2 = "Yes";
                }
                else
                {
                    BSITF2 = "No";
                }
                ////////  
                var ASITF2 = "";
                if (missionAuthorization1.ASITF == true)
                {
                    ASITF2 = "Yes";
                }
                else
                {
                    ASITF2 = "No";
                }
                ////////
                var SSAFE2 = "";
                if (missionAuthorization1.SSAFE == true)
                {
                    SSAFE2 = "Yes";
                }
                else
                {
                    SSAFE2 = "No";
                }

                MailMessage mail = new MailMessage();
                mail.To.Add("ahmed.badr@wfp.org");
                //mail.CC.Add(cc);
                mail.Bcc.Add("ahmed.badr@wfp.org");
                mail.From = new MailAddress(from);
                mail.Subject = "Mission Updated";

                string Body = "<table><tr><td colspan='2'>" + "<hr/>Updated Mission<hr/></td></tr><tr><td><font color='blue'>Request : </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorization1.MissionID + "</td></tr>"
                        + "<tr><td>" + "<font color='blue'>From :</font></td><td>" + staffsedit.First().staff_first_name + " " + staffsedit.First().staff_last_name + "</td></tr>"
                        + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                        + "<tr><td><font color='blue'>From :</font></td><td>" + missionAuthorization1.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                        + "<tr><td><font color='blue'>To :</font></td><td>" + missionAuthorization1.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                        + "<tr><td><font color='blue'>Funding :</font></td><td>" + missionAuthorization1.funding + "</td></tr>"
                        + "<tr><td><font color='blue'>Objective :</font></td><td>" + missionAuthorization1.MissionObjective + "</td></tr>"
                        + "<tr><td><font color='blue'>Mission Itinerary :</font></td><td>" + missionAuthorization1.MissionItinerary + "</td></tr>"
                        + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video ? :</font></td><td>" + canbedoneee + "</td></tr>"
                        + "<tr><td><font color='blue'>Cleared By Supervisor :</font></td><td>" + CleredBySupe2 + "</td></tr>"

                        + "<tr><td><font color='blue'>BSITF :</font></td><td>" + BSITF2 + "</td></tr>"
                        + "<tr><td><font color='blue'>ASITF :</font></td><td>" + ASITF2 + "</td></tr>"
                        + "<tr><td><font color='blue'>SSAFE2 :</font></td><td>" + SSAFE2 + "</td></tr>"
                        + "<tr><td><font color='blue'> DSS Approval:</font></td><td>" + "<a href='mailto: @Html.Display('houmail', new { @id = " + missionAuthorization1.non6 + "  })'> @Html.Display('houmail', new { @id = 'houmail' })</a>" + "</td></tr>"


                        + "<tr><td><a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid="
                        + missionAuthorization1.MissionID
                        + "&RDORDRDSignature=Yes&FromDate=" + missionAuthorization1.FromDate
                        + " &ToDate=" + missionAuthorization1.ToDate
                        + " &to=" + to + "&staffmail=" + staffsedit.First().staff_email
                        + "'>Accept</a></td></tr><tr><td><a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization1.MissionID
                        + "&RDORDRDSignature=NO & FromDate=" + missionAuthorization1.FromDate
                        + " &ToDate=" + missionAuthorization1.ToDate
                        + "&to=" + to
                        + "&staffmail=" + staffsedit.First().staff_email
                        + "'>Reject</a>" + "</td></tr><tr><td>" + "</td></tr></table>";



                mail.Body = Body;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtprelay.global.wfp.org";
                smtp.Port = 25;
                smtp.UseDefaultCredentials = true;
                smtp.Send(mail);

                return RedirectToAction("UploadFilesM", new { id2 = missionAuthorization1.MissionID });

            }

            var staffs = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
            staffs = staffs.Where(s => s.staff_email.Contains(User.Identity.Name + "@wfp.org"));

            ViewBag.staffid = staffs.First().staffid;
            ViewBag.FirstNme = staffs.First().staff_first_name;
            ViewBag.LastName = staffs.First().staff_last_name;






            //    }

            //}

           

            return View(missionAuthorization1);
        }

        // GET: MissionAuthorization1/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MissionAuthorization1 missionAuthorization1 = db.MissionAuthorization1.Find(id);
            if (missionAuthorization1 == null)
            {
                return HttpNotFound();
            }
            return View();
        }

        // POST: MissionAuthorization1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MissionAuthorization1 missionAuthorization1 = db.MissionAuthorization1.Find(id);
            db.MissionAuthorization1.Remove(missionAuthorization1);
            db.SaveChanges();



            // MissionAuthorization missionAuthorization = db.MissionAuthorizations.Find(id);
           
            //db.MissionAuthorizations.Remove(missionAuthorization);
            //db.SaveChanges();

            //////////////////////////////////////////////////////////////////////////////////////////////////////

            var staffs = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                staffs = staffs.Where(s => s.staff_email.Contains(User.Identity.Name + "@wfp.org"));
                

              
                //ViewBag.staffid = new SelectList(db.staffs, "staffs.First().staffid", staffs.First().staff_email);
                //ViewBag.staffid = staffs.First().staffid;
                //ViewBag.staffid2 = staffs.First().staff_first_name;
                //ViewBag.FirstName = staffs.First().staff_first_name;
                //ViewBag.LastName = staffs.First().staff_last_name;
                //ViewBag.Unit = staffs.First().unit.unit_description_english;
                //var staffsupervisor = staffs.First().staff_supervisorid;
                //var userUnit = staffs.First().unit_id;


                //var from = staffs.First().staff_email;
                //var supers = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                //supers = supers.Where(d => d.staffid == staffsupervisor);

               
                //var unithou = db.units.Include(d => d.staffs );
                //unithou = unithou.Where(u => u.unitid  == userUnit);
                //var houid = unithou.First().HOU_ID ;

                //var staffshou = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                //staffshou = staffshou.Where(h => h.staffid == houid);
                //var to = staffshou.First().staff_email;
                ////var houname = staffshou.First().staff_first_name + " " + staffshou.First().staff_last_name;
                

                //var cc = supers.First().staff_email;
                //var supername = supers.First().staff_first_name + " " + supers.First().staff_last_name ;


            var canbedoneee = "";
            if (missionAuthorization1.canbedone == true)
            {
                canbedoneee = "Yes";
            }
            else
            {
                canbedoneee = "No";
            }

            var CleredBySupe2 = "";
            if (missionAuthorization1.canbedone == true)
            {
                CleredBySupe2 = "Yes";
            }
            else
            {
                CleredBySupe2 = "No";
            }

            var BSITF2 = "";
            if (missionAuthorization1.BSITF == true)
            {
                BSITF2 = "Yes";
            }
            else
            {
                BSITF2 = "No";
            }
            ////////  
            var ASITF2 = "";
            if (missionAuthorization1.ASITF == true)
            {
                ASITF2 = "Yes";
            }
            else
            {
                ASITF2 = "No";
            }
            ////////
            var SSAFE2 = "";
            if (missionAuthorization1.SSAFE == true)
            {
                SSAFE2 = "Yes";
            }
            else
            {
                SSAFE2 = "No";
            }

            MailMessage mail = new MailMessage();
            mail.To.Add("ahmed.badr@wfp.org");
            //mail.CC.Add(cc);
            mail.Bcc.Add("ahmed.badr@wfp.org");
            mail.From = new MailAddress("RBC-SMS@wfp.org");
            mail.Subject = "Mission Deleted";

            string Body = "<table><tr><td colspan='2'>" + "<hr/>Updated Mission<hr/></td></tr><tr><td><font color='blue'>Request : </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorization1.MissionID + "</td></tr>"
                    + "<tr><td>" + "<font color='blue'>From :</font></td><td>" + staffs.First().staff_first_name + " " + staffs.First().staff_last_name + "</td></tr>"
                    + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                    + "<tr><td><font color='blue'>From :</font></td><td>" + missionAuthorization1.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                    + "<tr><td><font color='blue'>To :</font></td><td>" + missionAuthorization1.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                    + "<tr><td><font color='blue'>Funding :</font></td><td>" + missionAuthorization1.funding + "</td></tr>"
                    + "<tr><td><font color='blue'>Objective :</font></td><td>" + missionAuthorization1.MissionObjective + "</td></tr>"
                    + "<tr><td><font color='blue'>Mission Itinerary :</font></td><td>" + missionAuthorization1.MissionItinerary + "</td></tr>"
                    + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video ? :</font></td><td>" + canbedoneee + "</td></tr>"
                    + "<tr><td><font color='blue'>Cleared By Supervisor :</font></td><td>" + CleredBySupe2 + "</td></tr>"

                    + "<tr><td><font color='blue'>Cleared By Supervisor :</font></td><td>" + BSITF2 + "</td></tr>"
                    + "<tr><td><font color='blue'>Cleared By Supervisor :</font></td><td>" + ASITF2 + "</td></tr>"
                    + "<tr><td><font color='blue'>Cleared By Supervisor :</font></td><td>" + SSAFE2 + "</td></tr></table>";



            mail.Body = Body;
            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtprelay.global.wfp.org";
            smtp.Port = 25;
            smtp.UseDefaultCredentials = true;
            smtp.Send(mail);

            return RedirectToAction("Index");
        }

        /// <summary>
        /// ///upload file
        /// </summary>
        /// <param name="disposing"></param>
        public ActionResult UploadFilesM(int? id2)
        {
            ViewBag.missionID = id2;



            return View();
        }



        [HttpPost]
        public ActionResult UploadFilesM(IEnumerable<HttpPostedFileBase> files,int? non6)
        {
            foreach (var file in files)
            {
                if (files != null)
                {
                    


                    //int missionid = missionID;
                    //file.SaveAs(Path.Combine(Server.MapPath("/UploadedFiles"), Guid.NewGuid() + Path.GetExtension(file.FileName)));
                    string path = Path.Combine(Server.MapPath("~/UploadedFiles"), Path.GetFileName(file.FileName));
                    file.SaveAs(path);

                    var houfornewunit = db.MissionAuthorization1.Single(u => u.MissionID == non6);
                    houfornewunit.non6 = Path.GetFileName(file.FileName);
                    db.SaveChanges();



                    //////////////////////////mail//////////////

                    MissionAuthorization1 missionAuthorization1 = db.MissionAuthorization1.Find(non6);
                    var staffs = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                    staffs = staffs.Where(s => s.staff_email.Contains(User.Identity.Name + "@wfp.org"));


                    
                    //ViewBag.staffid = new SelectList(db.staffs, "staffs.First().staffid", staffs.First().staff_email);
                    ViewBag.staffid = staffs.First().staffid;
                    ViewBag.staffid2 = staffs.First().staff_first_name;
                    ViewBag.FirstName = staffs.First().staff_first_name;
                    ViewBag.LastName = staffs.First().staff_last_name;
                    ViewBag.Unit = staffs.First().unit.unit_description_english;
                    var staffsupervisor = staffs.First().staff_supervisorid;
                    var userUnit = staffs.First().unit_id;


                    var from = staffs.First().staff_email;
                    var supers = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                    supers = supers.Where(d => d.staffid == staffsupervisor);


                    var unithou = db.units.Include(d => d.staffs);
                    unithou = unithou.Where(u => u.unitid == userUnit);
                    var houid = unithou.First().HOU_ID;

                    var staffshou = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                    staffshou = staffshou.Where(h => h.staffid == houid);
                    var to = staffshou.First().staff_email;
                    //var houname = staffshou.First().staff_first_name + " " + staffshou.First().staff_last_name;


                    var cc = supers.First().staff_email;
                    var supername = supers.First().staff_first_name + " " + supers.First().staff_last_name;


                  
                        var canbedone = "";
                        if (missionAuthorization1.canbedone == true)
                        {
                            canbedone = "Yes";
                        }
                        else
                        {
                            canbedone = "No";
                        }

                        var CleredBySupervisor = "";
                        if (missionAuthorization1.canbedone == true)
                        {
                            CleredBySupervisor = "Yes";
                        }
                        else
                        {
                            CleredBySupervisor = "No";
                        }

                        var BSITF2 = "";
                        if (missionAuthorization1.BSITF == true)
                        {
                            BSITF2 = "Yes";
                        }
                        else
                        {
                            BSITF2 = "No";
                        }
                        ////////  
                        var ASITF2 = "";
                        if (missionAuthorization1.ASITF == true)
                        {
                            ASITF2 = "Yes";
                        }
                        else
                        {
                            ASITF2 = "No";
                        }
                        ////////
                        var SSAFE2 = "";
                        if (missionAuthorization1.SSAFE == true)
                        {
                            SSAFE2 = "Yes";
                        }
                        else
                        {
                            SSAFE2 = "No";
                        }

                        //////////////////////////////


                        var supersforhou = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                        supersforhou = supersforhou.Where(d => d.staffid == staffsupervisor);
                        var superforhounitsname = supersforhou.First().staff_first_name + " " + staffshou.First().staff_last_name;
                        var houmailUPDATE = supersforhou.First().staff_email;

                        if (houmailUPDATE == "muhannad.hadi@wfp.org" || houmailUPDATE == "carlo.scaramella@wfp.org")
                        {
                            houmailUPDATE = "rbc.management@wfp.org"; //rbc.management@wfp.org

                        }
                        /////////////////////////

                        MailMessage mailhou = new MailMessage();
                        mailhou.To.Add("ahmed.badr@wfp.org"); // RD/DRD OFFICE/rbc.management@wfp.org
                        //mail.CC.Add(cc);
                        mailhou.Bcc.Add("ahmed.badr@wfp.org");
                        mailhou.From = new MailAddress(from);
                        mailhou.Subject = "Mission Authorization Form crEATE";

                        string Bodyhou = "<table><tr><td colspan='2'>" + "<hr/>New Mission<hr/></td></tr><tr><td><font color='blue'>Request : </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorization1.MissionID + "</td></tr>"
                            + "<tr><td>" + "<font color='blue'>From :</font></td><td>" + staffs.First().staff_first_name + " " + staffs.First().staff_last_name + "</td></tr>"
                            + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                            + "<tr><td><font color='blue'>From :</font></td><td>" + missionAuthorization1.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                            + "<tr><td><font color='blue'>To :</font></td><td>" + missionAuthorization1.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                            + "<tr><td><font color='blue'>Funding :</font></td><td>" + missionAuthorization1.funding + "</td></tr>"
                            + "<tr><td><font color='blue'>Objective :</font></td><td>" + missionAuthorization1.MissionObjective + "</td></tr>"
                            + "<tr><td><font color='blue'>Mission Itinerary :</font></td><td>" + missionAuthorization1.MissionItinerary + "</td></tr>"
                            + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video ? :</font></td><td>" + canbedone + "</td></tr>"
                            + "<tr><td><font color='blue'>Cleared By Supervisor :</font></td><td>" + CleredBySupervisor + "</td></tr>"

                            + "<tr><td><font color='blue'>	BSITF :</font></td><td>" + BSITF2 + "</td></tr>"
                            + "<tr><td><font color='blue'> ASITF :</font></td><td>" + ASITF2 + "</td></tr>"
                            + "<tr><td><font color='blue'> SSAFE :</font></td><td>" + SSAFE2 + "</td></tr>"
                            + "<tr><td><font color='blue'> DSS Approval:</font></td><td><a href='http://10.11.135.254:8080/RBC-SMS/UploadedFiles/" + missionAuthorization1.non6 + "'>" + missionAuthorization1.non6 + "</a></td></tr>"
                           
                            
                            //<a download='custom-filename.jpg' href="/path/to/image" title="ImageName">     <img alt="ImageName" src="/path/to/image"> </a>


                            + "<tr><td><a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorization1/Approval2?Mid="
                            + missionAuthorization1.MissionID
                            + "&RDORDRDSignature=Yes&FromDate=" + missionAuthorization1.FromDate
                            + " &ToDate=" + missionAuthorization1.ToDate
                            + " &to=" + to + "&staffmail=" + staffs.First().staff_email
                            + "'>Accept</a></td><td><a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorization1/Approval2?Mid=" + missionAuthorization1.MissionID
                            + "&RDORDRDSignature=NO & FromDate=" + missionAuthorization1.FromDate
                            + " &ToDate=" + missionAuthorization1.ToDate
                            + "&to=" + to
                            + "&staffmail=" + staffs.First().staff_email +   "</td></tr></table>";

                        //string Body = "Request : Mission &nbsp;  &nbsp;  &nbsp; &nbsp;  &nbsp; &nbsp; &nbsp; &nbsp;  Mission ID : " + missionAuthorization.MissionID + " <br> " + " From &nbsp; &nbsp; &nbsp;:" + staffs.First().staff_first_name + "&nbsp;" + staffs.First().staff_last_name + "<br><br>" + " Mission Information" + "<br><br>" + " From :" + missionAuthorization.FromDate + "<br>To :" + missionAuthorization.ToDate +
                        //    "<br>Funding :" + missionAuthorization.funding + "<br>Objective :" + missionAuthorization.MissionObjective + "<br> Mission Itinerary :" + missionAuthorization.MissionItinerary + "<br> Can This meeting Be Done Over Voice/Video ? :" + missionAuthorization.canbedone  + " <br><br> <a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor=" + true + "&FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + " &to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Acept</a>&nbsp;&nbsp;&nbsp;<a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor= " + false + "& FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + "&to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Reject</a> ";

                        mailhou.Body = Bodyhou;
                        mailhou.IsBodyHtml = true;
                        SmtpClient smtphou = new SmtpClient();
                        smtphou.Host = "smtprelay.global.wfp.org";
                        smtphou.Port = 25;
                        smtphou.UseDefaultCredentials = true;
                        //smtp.Credentials = new System.Net.NetworkCredential
                        //("ahmed.badr", "Survivor2323");// Enter seders User name and password  
                        //smtp.EnableSsl = true;
                        //smtphou.Send(mailhou);////////////


                        //return RedirectToAction("UploadFilesM", new { id = missionAuthorization1.MissionID });
                        return RedirectToAction("Index");
                   
                }

                return View();
            }
            return View();
        }


/// <summary>
/// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/// </summary>
/// <param name="Mid"></param>
/// <param name="RDORDRDSignature"></param>
/// <param name="FromDate"></param>
/// <param name="ToDate"></param>
/// <param name="to"></param>
/// <param name="staffmail"></param>
/// <param name="supervisorname"></param>
/// <param name="onbehalfID"></param>
/// <param name="onbehalfSuperID"></param>
/// <param name="onbehalfHouID"></param>
/// <returns></returns>
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public ActionResult Approval2(int? i, int? MissionID2, string RDORDRDSignature2, DateTime? FromDate2, DateTime? ToDate2, string to2, string staffmail2, string supervisorname2, int? onbehalfID2, int? onbehalfSuperID2, int? onbehalfHouID2)
        {
            ViewBag.MissionID = MissionID2;
            ViewBag.RDORDRDSignature = RDORDRDSignature2;
            ViewBag.FromDate = FromDate2;
            ViewBag.ToDate = ToDate2;
            ViewBag.to = to2;
            ViewBag.staffmail = staffmail2;
            ViewBag.supervisorname = supervisorname2;
            ViewBag.onbehalfID = onbehalfID2;
            ViewBag.onbehalfSuperID = onbehalfSuperID2;
            ViewBag.onbehalfHouID = onbehalfHouID2;
            
            return View();
        }

        [HttpPost]
        public ActionResult Approval2(int? MissionID, string Comment1, string RDORDRDSignature, DateTime? FromDate, DateTime? ToDate, string to, string staff, string supervisorname, int? onbehalfID, int? onbehalfSuperID, int? onbehalfHouID)
        {


            if (!Request.IsAuthenticated)
            {
                Response.Redirect("/RBC-SMS/account/login");
            }

            ///////////////////////// start on behalf 

            if (onbehalfID != null && onbehalfSuperID != null && onbehalfHouID != null)
            {
                ViewBag.mid = Request.QueryString["MissionID"];
                ViewBag.RDORDRDSignature = Request.QueryString["RDORDRDSignature"];
                ViewBag.FromDate = Request.QueryString["FromDate"];
                ViewBag.ToDate = Request.QueryString["ToDate"];
                if (RDORDRDSignature == "Yes")
                {
                    try
                    {
                        // missionAuthorization = db.MissionAuthorizations.Find(Mid);
                        //db.Entry(missionAuthorization).State = EntityState.Modified ;
                        //db.SaveChanges();

                        var missionAuthorization11 = db.MissionAuthorization1.Single(u => u.MissionID == MissionID);
                        missionAuthorization11.Signature1 = true;
                        missionAuthorization11.Signature1Date = DateTime.Now;
                        missionAuthorization11.Comment1 = Comment1;
                        db.SaveChanges();



                        var staffs = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                        staffs = staffs.Where(s => s.staffid == onbehalfID);
                        var too = staffs.First().staff_email;
                        ////////////////////////////////////////////////////////////////////////////onbehalf
                        var staffsupervisor = staffs.First().staff_supervisorid;
                        var supers = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                        supers = supers.Where(d => d.staffid == onbehalfID);
                        var onbehalfemail = supers.First().staff_email;


                        var supersonbehalf = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                        supersonbehalf = supersonbehalf.Where(d => d.staffid == onbehalfSuperID);
                        var onbehafsuper = supersonbehalf.First().staff_email;
                        /////////////////////////////////////////////////////////////////////////////////////
                        ////////////////////////////////////////////////////
                        //var rd = db.staffs.Include(r => r.contract_details).Include(r => r.contract_type).Include(r => r.Country_office).Include(r => r.functional_title).Include(r => r.staff2).Include(r => r.sub_office).Include(r => r.unit).Include(r => r.country).Include(r => r.governorate);
                        //rd = rd.Where(r => r.functional_title.functional_title_description_english == "REGIONAL DIRECTOR");
                        //var rdmail = rd.First().staff_email;

                        //var drd = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                        //drd = drd.Where(s => s.functional_title.functional_title_description_english == "Deputy Regional Director");
                        //var drdmail = drd.First().staff_email;

                        var userUnit = staffs.First().unit_id;
                        var unithou = db.units.Include(d => d.staffs);
                        unithou = unithou.Where(u => u.unitid == userUnit);
                        var houid = unithou.First().HOU_ID;

                        var staffshou = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                        staffshou = staffshou.Where(h => h.staffid == houid);
                        var hou = staffshou.First().staff_email;
                        //var houname = staffshou.First().staff_first_name + " " + staffshou.First().staff_last_name;

                        var cc = supers.First().staff_email;
                        var supername = supers.First().staff_first_name + " " + supers.First().staff_last_name;
                        var superemail = supers.First().staff_email;

                        //
                        //
                        //hou email
                        if (onbehalfID == houid)
                        {
                            var canbedone = "";
                            if (missionAuthorization11.canbedone == true)
                            {
                                canbedone = "Yes";
                            }
                            else
                            {
                                canbedone = "No";
                            }

                            var CleredBySupervisor = "";
                            if (missionAuthorization11.canbedone == true)
                            {
                                CleredBySupervisor = "Yes";
                            }
                            else
                            {
                                CleredBySupervisor = "No";
                            }

                            var BSITF2 = "";
                            if (missionAuthorization11.BSITF == true)
                            {
                                BSITF2 = "Yes";
                            }
                            else
                            {
                                BSITF2 = "No";
                            }
                            ////////  
                            var ASITF2 = "";
                            if (missionAuthorization11.ASITF == true)
                            {
                                ASITF2 = "Yes";
                            }
                            else
                            {
                                ASITF2 = "No";
                            }
                            ////////
                            var SSAFE2 = "";
                            if (missionAuthorization11.SSAFE == true)
                            {
                                SSAFE2 = "Yes";
                            }
                            else
                            {
                                SSAFE2 = "No";
                            }
                            var Roming = "";
                            if (missionAuthorization11.StaffSignature == "True")
                            {
                                Roming = "Yes";
                            }
                            else
                            {
                                Roming = "No";
                            }
                            var odcitcc = "";
                            if (Roming == "Yes")
                            {
                                odcitcc = "cairo.itservicedesk@wfp.org";
                            }
                            else
                            {
                                odcitcc = "ahmed.badr@wfp.org";
                            }
                            //////////////////////////////


                            var supersforhou = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                            supersforhou = supersforhou.Where(d => d.staffid == staffsupervisor);
                            var superforhounitsname = supersforhou.First().staff_first_name + " " + staffshou.First().staff_last_name;
                            var houmailUPDATE = supersforhou.First().staff_email;

                            if (houmailUPDATE == "muhannad.hadi@wfp.org" || houmailUPDATE == "carlo.scaramella@wfp.org")
                            {
                                houmailUPDATE = "rbc.management@wfp.org"; //rbc.management@wfp.org

                            }
                            /////////////////////////

                            MailMessage mailhou = new MailMessage();
                            mailhou.To.Add(onbehalfemail);
                            mailhou.Bcc.Add("ahmed.badr@wfp.org");

                            //mailhou.CC.Add("rbc.travel@wfp.org"); //travel unit
                            //mailhou.CC.Add("mai.kenawi@wfp.org"); //travel unit
                            //mailhou.CC.Add("amal.mohamed@wfp.org"); //travel unit
                            //mailhou.CC.Add("ayten.el-sheikh@wfp.org"); //travel unit
                            //mailhou.CC.Add("essraa.soliman@wfp.org"); //travel unit
                            //mailhou.CC.Add("rbc.management@wfp.org"); //RD/DRD Office
                            mailhou.Bcc.Add(odcitcc); //travel unit
                            mailhou.From = new MailAddress("RBC-SMS-SSYSTEM@wfp.org");
                            mailhou.Subject = "HOU Approved";

                            string Bodyhou = "<table><tr><td colspan='2'>" + "<hr/>New Mission<hr/></td></tr><tr><td><font color='blue'>Request : </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorization11.MissionID + "</td></tr>"
                                + "<tr><td>" + "<font color='blue'>From :</font></td><td>" + staffs.First().staff_first_name + " " + staffs.First().staff_last_name + "</td></tr>"
                                + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                                + "<tr><td><font color='blue'>From :</font></td><td>" + missionAuthorization11.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                                + "<tr><td><font color='blue'>To :</font></td><td>" + missionAuthorization11.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                                + "<tr><td><font color='blue'>Funding :</font></td><td>" + missionAuthorization11.funding + "</td></tr>"
                                + "<tr><td><font color='blue'>Objective :</font></td><td>" + missionAuthorization11.MissionObjective + "</td></tr>"
                                + "<tr><td><font color='blue'>Mission Itinerary :</font></td><td>" + missionAuthorization11.MissionItinerary + "</td></tr>"
                                + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video ? :</font></td><td>" + canbedone + "</td></tr>"
                                + "<tr><td><font color='blue'>Cleared By Supervisor :</font></td><td>" + CleredBySupervisor + "</td></tr>"
                                + "<tr><td><font color='blue'>International Roaming :</font></td><td>" + Roming + "</td></tr>"
                                + "<tr><td><font color='blue'>HOU Comment :</font></td><td>" + missionAuthorization11.Comment1 + "</td></tr>"
                                + "<tr><td><font color='blue'>	BSITF :</font></td><td>" + BSITF2 + "</td></tr>"
                                + "<tr><td><font color='blue'> ASITF :</font></td><td>" + ASITF2 + "</td></tr>"
                                + "<tr><td><font color='blue'> SSAFE :</font></td><td>" + SSAFE2 + "</td></tr>"
                                + "<tr><td><font color='blue'> DSS Approval:</font></td><td><a href='http://10.11.135.254:8080/RBC-SMS/UploadedFiles/" + missionAuthorization11.non6 + "'>" + missionAuthorization11.non6 + "</a></td></tr></table>";

                            //string Body = "Request : Mission &nbsp;  &nbsp;  &nbsp; &nbsp;  &nbsp; &nbsp; &nbsp; &nbsp;  Mission ID : " + missionAuthorization.MissionID + " <br> " + " From &nbsp; &nbsp; &nbsp;:" + staffs.First().staff_first_name + "&nbsp;" + staffs.First().staff_last_name + "<br><br>" + " Mission Information" + "<br><br>" + " From :" + missionAuthorization.FromDate + "<br>To :" + missionAuthorization.ToDate +
                            //    "<br>Funding :" + missionAuthorization.funding + "<br>Objective :" + missionAuthorization.MissionObjective + "<br> Mission Itinerary :" + missionAuthorization.MissionItinerary + "<br> Can This meeting Be Done Over Voice/Video ? :" + missionAuthorization.canbedone  + " <br><br> <a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor=" + true + "&FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + " &to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Acept</a>&nbsp;&nbsp;&nbsp;<a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor= " + false + "& FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + "&to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Reject</a> ";

                            mailhou.Body = Bodyhou;
                            mailhou.IsBodyHtml = true;
                            SmtpClient smtphou = new SmtpClient();
                            smtphou.Host = "smtprelay.global.wfp.org";
                            smtphou.Port = 25;
                            smtphou.UseDefaultCredentials = true;
                            //smtp.Credentials = new System.Net.NetworkCredential
                            //("ahmed.badr", "Survivor2323");// Enter seders User name and password  
                            //smtp.EnableSsl = true;
                            smtphou.Send(mailhou);

                            ///////////////////////////// Security Approvall //////////////////////

                            MailMessage mailhou1 = new MailMessage();
                            mailhou1.To.Add("ahmed.badr@wfp.org");
                            mailhou1.Bcc.Add("ahmed.badr@wfp.org");

                            //mailhou.CC.Add("rbc.travel@wfp.org"); //travel unit
                            //mailhou.CC.Add("mai.kenawi@wfp.org"); //travel unit
                            //mailhou.CC.Add("amal.mohamed@wfp.org"); //travel unit
                            //mailhou.CC.Add("ayten.el-sheikh@wfp.org"); //travel unit
                            //mailhou.CC.Add("essraa.soliman@wfp.org"); //travel unit
                            //mailhou.CC.Add("rbc.management@wfp.org"); //RD/DRD Office
                            mailhou1.Bcc.Add(odcitcc); //travel unit
                            mailhou1.From = new MailAddress("RBC-SMS-SSYSTEM@wfp.org");//"rbc.management@wfp.org"
                            mailhou1.Subject = "For Security Approval";

                            string Bodyhou1 = "<table><tr><td colspan='2'>" + "<hr/>New Mission<hr/></td></tr><tr><td><font color='blue'>Request : </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorization11.MissionID + "</td></tr>"
                                + "<tr><td>" + "<font color='blue'>From :</font></td><td>" + staffs.First().staff_first_name + " " + staffs.First().staff_last_name + "</td></tr>"
                                + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                                + "<tr><td><font color='blue'>From :</font></td><td>" + missionAuthorization11.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                                + "<tr><td><font color='blue'>To :</font></td><td>" + missionAuthorization11.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                                + "<tr><td><font color='blue'>Funding :</font></td><td>" + missionAuthorization11.funding + "</td></tr>"
                                + "<tr><td><font color='blue'>Objective :</font></td><td>" + missionAuthorization11.MissionObjective + "</td></tr>"
                                + "<tr><td><font color='blue'>Mission Itinerary :</font></td><td>" + missionAuthorization11.MissionItinerary + "</td></tr>"
                                + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video ? :</font></td><td>" + canbedone + "</td></tr>"
                                + "<tr><td><font color='blue'>Cleared By Supervisor :</font></td><td>" + CleredBySupervisor + "</td></tr>"
                                + "<tr><td><font color='blue'>International Roaming :</font></td><td>" + Roming + "</td></tr>"
                                + "<tr><td><font color='blue'>HOU Comment :</font></td><td>" + missionAuthorization11.Comment1 + "</td></tr>"
                                + "<tr><td><font color='blue'>	BSITF :</font></td><td>" + BSITF2 + "</td></tr>"
                                + "<tr><td><font color='blue'> ASITF :</font></td><td>" + ASITF2 + "</td></tr>"
                                + "<tr><td><font color='blue'> SSAFE :</font></td><td>" + SSAFE2 + "</td></tr>"
                                + "<tr><td><font color='blue'> DSS Approval:</font></td><td><a href='http://10.11.135.254:8080/RBC-SMS/UploadedFiles/" + missionAuthorization11.non6 + "'>" + missionAuthorization11.non6
                            + "<tr><td><a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorization1/Approval2Security?MissionID2="
                            + missionAuthorization11.MissionID
                            + "&RDORDRDSignature2=Yes&FromDate2=" + missionAuthorization11.FromDate
                            + "&ToDate2=" + missionAuthorization11.ToDate
                            + "&to2=" + to + "&staffmail2=" + staffs.First().staff_email
                            + "'>Accept</a></td><td><a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorization1/Approval2Security?MissionID2=" + missionAuthorization11.MissionID
                            + "&RDORDRDSignature2=NO & FromDate2=" + missionAuthorization11.FromDate
                            + "&ToDate2=" + missionAuthorization11.ToDate
                            + "&to2=" + to
                            + "&staffmail2=" + staffs.First().staff_email + "'>Reject</a>" + "</a></td></tr></table>";

                            //string Body = "Request : Mission &nbsp;  &nbsp;  &nbsp; &nbsp;  &nbsp; &nbsp; &nbsp; &nbsp;  Mission ID : " + missionAuthorization.MissionID + " <br> " + " From &nbsp; &nbsp; &nbsp;:" + staffs.First().staff_first_name + "&nbsp;" + staffs.First().staff_last_name + "<br><br>" + " Mission Information" + "<br><br>" + " From :" + missionAuthorization.FromDate + "<br>To :" + missionAuthorization.ToDate +
                            //    "<br>Funding :" + missionAuthorization.funding + "<br>Objective :" + missionAuthorization.MissionObjective + "<br> Mission Itinerary :" + missionAuthorization.MissionItinerary + "<br> Can This meeting Be Done Over Voice/Video ? :" + missionAuthorization.canbedone  + " <br><br> <a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor=" + true + "&FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + " &to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Acept</a>&nbsp;&nbsp;&nbsp;<a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor= " + false + "& FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + "&to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Reject</a> ";

                            mailhou1.Body = Bodyhou1;
                            mailhou1.IsBodyHtml = true;
                            SmtpClient smtphou1 = new SmtpClient();
                            smtphou1.Host = "smtprelay.global.wfp.org";
                            smtphou1.Port = 25;
                            smtphou1.UseDefaultCredentials = true;
                            //smtp.Credentials = new System.Net.NetworkCredential
                            //("ahmed.badr", "Survivor2323");// Enter seders User name and password  
                            //smtp.EnableSsl = true;
                            smtphou1.Send(mailhou1);

                            ///////////////////////////// Security Approvall end //////////////////////
                            return RedirectToAction("Index");
                        }

                        else
                        {
                            var canbedone = "";
                            if (missionAuthorization11.canbedone == true)
                            {
                                canbedone = "Yes";
                            }
                            else
                            {
                                canbedone = "No";
                            }

                            var CleredBySupervisor = "";
                            if (missionAuthorization11.canbedone == true)
                            {
                                CleredBySupervisor = "Yes";
                            }
                            else
                            {
                                CleredBySupervisor = "No";
                            }

                            var BSITF2 = "";
                            if (missionAuthorization11.BSITF == true)
                            {
                                BSITF2 = "Yes";
                            }
                            else
                            {
                                BSITF2 = "No";
                            }
                            ////////  
                            var ASITF2 = "";
                            if (missionAuthorization11.ASITF == true)
                            {
                                ASITF2 = "Yes";
                            }
                            else
                            {
                                ASITF2 = "No";
                            }
                            ////////
                            var SSAFE2 = "";
                            if (missionAuthorization11.SSAFE == true)
                            {
                                SSAFE2 = "Yes";
                            }
                            else
                            {
                                SSAFE2 = "No";
                            }
                            var odcitcc = "";
                            if (missionAuthorization11.StaffSignature == "True")
                            {
                                odcitcc = "cairo.itservicedesk@wfp.org";
                            }
                            else
                            {
                                odcitcc = "ahmed.badr@wfp.org";
                            }
                            var Roming = "";
                            if (missionAuthorization11.StaffSignature == "True")
                            {
                                Roming = "Yes";
                            }
                            else
                            {
                                Roming = "No";
                            }
                            //////////////////////////////


                            var supersforhou = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                            supersforhou = supersforhou.Where(d => d.staffid == staffsupervisor);
                            var superforhounitsname = supersforhou.First().staff_first_name + " " + staffshou.First().staff_last_name;
                            var houmailUPDATE = supersforhou.First().staff_email;

                            if (houmailUPDATE == "muhannad.hadi@wfp.org" || houmailUPDATE == "carlo.scaramella@wfp.org")
                            {
                                houmailUPDATE = "rbc.management@wfp.org"; //rbc.management@wfp.org

                            }
                            /////////////////////////

                            MailMessage mailhou = new MailMessage();
                            mailhou.To.Add(onbehalfemail); //staffmail
                            mailhou.Bcc.Add("ahmed.badr@wfp.org");
                            mailhou.CC.Add(onbehafsuper);
                            mailhou.Bcc.Add(odcitcc); //travel unit
                            //mailhou.CC.Add("rbc.travel@wfp.org"); //travel unit
                            //mailhou.CC.Add("mai.kenawi@wfp.org"); //travel unit
                            //mailhou.CC.Add("amal.mohamed@wfp.org"); //travel unit
                            //mailhou.CC.Add("ayten.el-sheikh@wfp.org"); //travel unit
                            //mailhou.CC.Add("essraa.soliman@wfp.org"); //travel unit
                            //mailhou.CC.Add("rbc.management@wfp.org"); //RD/DRD Office
                            mailhou.From = new MailAddress(hou);
                            mailhou.Subject = "HOU Approved";

                            string Bodyhou = "<table><tr><td colspan='2'>" + "<hr/>New Mission<hr/></td></tr><tr><td><font color='blue'>Request : </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorization11.MissionID + "</td></tr>"
                                + "<tr><td>" + "<font color='blue'>From :</font></td><td>" + staffs.First().staff_first_name + " " + staffs.First().staff_last_name + "</td></tr>"
                                + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                                + "<tr><td><font color='blue'>From :</font></td><td>" + missionAuthorization11.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                                + "<tr><td><font color='blue'>To :</font></td><td>" + missionAuthorization11.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                                + "<tr><td><font color='blue'>Funding :</font></td><td>" + missionAuthorization11.funding + "</td></tr>"
                                + "<tr><td><font color='blue'>Objective :</font></td><td>" + missionAuthorization11.MissionObjective + "</td></tr>"
                                + "<tr><td><font color='blue'>Mission Itinerary :</font></td><td>" + missionAuthorization11.MissionItinerary + "</td></tr>"
                                + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video ? :</font></td><td>" + canbedone + "</td></tr>"
                                + "<tr><td><font color='blue'>Cleared By Supervisor :</font></td><td>" + CleredBySupervisor + "</td></tr>"
                                + "<tr><td><font color='blue'>International Roaming :</font></td><td>" + Roming + "</td></tr>"
                                 + "<tr><td><font color='blue'>HOU Comment :</font></td><td>" + missionAuthorization11.Comment1 + "</td></tr>"
                                + "<tr><td><font color='blue'>	BSITF :</font></td><td>" + BSITF2 + "</td></tr>"
                                + "<tr><td><font color='blue'> ASITF :</font></td><td>" + ASITF2 + "</td></tr>"
                                + "<tr><td><font color='blue'> SSAFE :</font></td><td>" + SSAFE2 + "</td></tr>"
                                + "<tr><td><font color='blue'> DSS Approval:</font></td><td><a href='http://10.11.135.254:8080/RBC-SMS/UploadedFiles/" + missionAuthorization11.non6 + "'>" + missionAuthorization11.non6 + "</a></td></tr></table>"; 

                            //string Body = "Request : Mission &nbsp;  &nbsp;  &nbsp; &nbsp;  &nbsp; &nbsp; &nbsp; &nbsp;  Mission ID : " + missionAuthorization.MissionID + " <br> " + " From &nbsp; &nbsp; &nbsp;:" + staffs.First().staff_first_name + "&nbsp;" + staffs.First().staff_last_name + "<br><br>" + " Mission Information" + "<br><br>" + " From :" + missionAuthorization.FromDate + "<br>To :" + missionAuthorization.ToDate +
                            //    "<br>Funding :" + missionAuthorization.funding + "<br>Objective :" + missionAuthorization.MissionObjective + "<br> Mission Itinerary :" + missionAuthorization.MissionItinerary + "<br> Can This meeting Be Done Over Voice/Video ? :" + missionAuthorization.canbedone  + " <br><br> <a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor=" + true + "&FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + " &to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Acept</a>&nbsp;&nbsp;&nbsp;<a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor= " + false + "& FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + "&to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Reject</a> ";

                            mailhou.Body = Bodyhou;
                            mailhou.IsBodyHtml = true;
                            SmtpClient smtphou = new SmtpClient();
                            smtphou.Host = "smtprelay.global.wfp.org";
                            smtphou.Port = 25;
                            smtphou.UseDefaultCredentials = true;
                            //smtp.Credentials = new System.Net.NetworkCredential
                            //("ahmed.badr", "Survivor2323");// Enter seders User name and password  
                            //smtp.EnableSsl = true;
                            smtphou.Send(mailhou);


                            ///////////////////////////// Security Approvall //////////////////////

                            MailMessage mailhou1 = new MailMessage();
                            mailhou1.To.Add("ahmed.badr@wfp.org");
                            mailhou1.Bcc.Add("ahmed.badr@wfp.org");

                            //mailhou.CC.Add("rbc.travel@wfp.org"); //travel unit
                            //mailhou.CC.Add("mai.kenawi@wfp.org"); //travel unit
                            //mailhou.CC.Add("amal.mohamed@wfp.org"); //travel unit
                            //mailhou.CC.Add("ayten.el-sheikh@wfp.org"); //travel unit
                            //mailhou.CC.Add("essraa.soliman@wfp.org"); //travel unit
                            //mailhou.CC.Add("rbc.management@wfp.org"); //RD/DRD Office
                            mailhou1.Bcc.Add(odcitcc); //travel unit
                            mailhou1.From = new MailAddress("RBC-SMS-SSYSTEM@wfp.org");//"rbc.management@wfp.org"
                            mailhou1.Subject = "For Security Approval";

                            string Bodyhou1 = "<table><tr><td colspan='2'>" + "<hr/>New Mission<hr/></td></tr><tr><td><font color='blue'>Request : </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorization11.MissionID + "</td></tr>"
                                + "<tr><td>" + "<font color='blue'>From :</font></td><td>" + staffs.First().staff_first_name + " " + staffs.First().staff_last_name + "</td></tr>"
                                + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                                + "<tr><td><font color='blue'>From :</font></td><td>" + missionAuthorization11.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                                + "<tr><td><font color='blue'>To :</font></td><td>" + missionAuthorization11.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                                + "<tr><td><font color='blue'>Funding :</font></td><td>" + missionAuthorization11.funding + "</td></tr>"
                                + "<tr><td><font color='blue'>Objective :</font></td><td>" + missionAuthorization11.MissionObjective + "</td></tr>"
                                + "<tr><td><font color='blue'>Mission Itinerary :</font></td><td>" + missionAuthorization11.MissionItinerary + "</td></tr>"
                                + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video ? :</font></td><td>" + canbedone + "</td></tr>"
                                + "<tr><td><font color='blue'>Cleared By Supervisor :</font></td><td>" + CleredBySupervisor + "</td></tr>"
                                + "<tr><td><font color='blue'>International Roaming :</font></td><td>" + Roming + "</td></tr>"
                                + "<tr><td><font color='blue'>HOU Comment :</font></td><td>" + missionAuthorization11.Comment1 + "</td></tr>"
                                + "<tr><td><font color='blue'>	BSITF :</font></td><td>" + BSITF2 + "</td></tr>"
                                + "<tr><td><font color='blue'> ASITF :</font></td><td>" + ASITF2 + "</td></tr>"
                                + "<tr><td><font color='blue'> SSAFE :</font></td><td>" + SSAFE2 + "</td></tr>"
                                + "<tr><td><font color='blue'> DSS Approval:</font></td><td><a href='http://10.11.135.254:8080/RBC-SMS/UploadedFiles/" + missionAuthorization11.non6 + "'>" + missionAuthorization11.non6

                           + "<tr><td><a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorization1/Approval2Security?MissionID2="
                            + missionAuthorization11.MissionID
                            + "&RDORDRDSignature2=Yes&FromDate2=" + missionAuthorization11.FromDate
                            + "&ToDate2=" + missionAuthorization11.ToDate
                            + "&to2=" + to + "&staffmail2=" + staffs.First().staff_email
                            + "'>Accept</a></td><td><a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorization1/Approval2Security?MissionID2=" + missionAuthorization11.MissionID
                            + "&RDORDRDSignature2=NO & FromDate2=" + missionAuthorization11.FromDate
                            + "&ToDate2=" + missionAuthorization11.ToDate
                            + "&to2=" + to
                            + "&staffmail2=" + staffs.First().staff_email + "'>Reject</a>" + "</a></td></tr></table>";


                            //string Body = "Request : Mission &nbsp;  &nbsp;  &nbsp; &nbsp;  &nbsp; &nbsp; &nbsp; &nbsp;  Mission ID : " + missionAuthorization.MissionID + " <br> " + " From &nbsp; &nbsp; &nbsp;:" + staffs.First().staff_first_name + "&nbsp;" + staffs.First().staff_last_name + "<br><br>" + " Mission Information" + "<br><br>" + " From :" + missionAuthorization.FromDate + "<br>To :" + missionAuthorization.ToDate +
                            //    "<br>Funding :" + missionAuthorization.funding + "<br>Objective :" + missionAuthorization.MissionObjective + "<br> Mission Itinerary :" + missionAuthorization.MissionItinerary + "<br> Can This meeting Be Done Over Voice/Video ? :" + missionAuthorization.canbedone  + " <br><br> <a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor=" + true + "&FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + " &to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Acept</a>&nbsp;&nbsp;&nbsp;<a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor= " + false + "& FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + "&to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Reject</a> ";

                            mailhou1.Body = Bodyhou1;
                            mailhou1.IsBodyHtml = true;
                            SmtpClient smtphou1 = new SmtpClient();
                            smtphou1.Host = "smtprelay.global.wfp.org";
                            smtphou1.Port = 25;
                            smtphou1.UseDefaultCredentials = true;
                            //smtp.Credentials = new System.Net.NetworkCredential
                            //("ahmed.badr", "Survivor2323");// Enter seders User name and password  
                            //smtp.EnableSsl = true;
                            smtphou1.Send(mailhou1);

                            ///////////////////////////// Security Approvall end //////////////////////

                            return RedirectToAction("Index");


                        }
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
                    return View();
                }
                else
                {
                    //var missionAuthorizationn = db.MissionAuthorizations.Single(u => u.MissionID == Mid);
                    //db.MissionAuthorizations.Remove(missionAuthorizationn);
                    //db.SaveChanges();
                    var missionAuthorization12 = db.MissionAuthorization1.Single(u => u.MissionID == MissionID);
                    missionAuthorization12.Signature1 = false;
                    missionAuthorization12.Signature1Date = DateTime.Now;
                    missionAuthorization12.Comment1 = Comment1;
                    db.SaveChanges();



                    var staffss = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                    staffss = staffss.Where(s => s.staffid == onbehalfID);
                    var too = staffss.First().staff_email;
                    var staffsupervisor = staffss.First().staff_supervisorid;
                    var supers = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                    supers = supers.Where(d => d.staffid == staffsupervisor);
                    var from = supers.First().staff_email;

                    //var rd = db.staffs.Include(r => r.contract_details).Include(r => r.contract_type).Include(r => r.Country_office).Include(r => r.functional_title).Include(r => r.staff2).Include(r => r.sub_office).Include(r => r.unit).Include(r => r.country).Include(r => r.governorate);
                    //rd = rd.Where(r => r.functional_title.functional_title_description_english == "REGIONAL DIRECTOR");
                    //var rdmail = rd.First().staff_email;

                    //var drd = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                    //drd = drd.Where(s => s.functional_title.functional_title_description_english == "Deputy Regional Director");
                    //var drdmail = drd.First().staff_email;

                    //////////////////////////////////////////////////////////////////////////////////////


                    var Sonbehafemail = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                    Sonbehafemail = Sonbehafemail.Where(d => d.staffid == onbehalfID);
                    var onbehalfemail = Sonbehafemail.First().staff_email;


                    var supersonbehalf = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                    supersonbehalf = supersonbehalf.Where(d => d.staffid == onbehalfSuperID);
                    var onbehafsuperemail = supersonbehalf.First().staff_email;

                    /////////////////////////////////////////////////////////////////////////////////////
                    //new
                    var userUnit = staffss.First().unit_id;
                    var unithou = db.units.Include(d => d.staffs);
                    unithou = unithou.Where(u => u.unitid == userUnit);
                    var houid = unithou.First().HOU_ID;

                    var staffshou = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                    staffshou = staffshou.Where(h => h.staffid == houid);
                    var hou = staffshou.First().staff_email;
                    //var houname = staffshou.First().staff_first_name + " " + staffshou.First().staff_last_name;

                    var cc = supers.First().staff_email;
                    var supername = supers.First().staff_first_name + " " + supers.First().staff_last_name;

                    //
                    //
                    //hou email
                    if (onbehalfID == houid)
                    {
                        var canbedone = "";
                        if (missionAuthorization12.canbedone == true)
                        {
                            canbedone = "Yes";
                        }
                        else
                        {
                            canbedone = "No";
                        }

                        var CleredBySupervisor = "";
                        if (missionAuthorization12.canbedone == true)
                        {
                            CleredBySupervisor = "Yes";
                        }
                        else
                        {
                            CleredBySupervisor = "No";
                        }

                        var BSITF2 = "";
                        if (missionAuthorization12.BSITF == true)
                        {
                            BSITF2 = "Yes";
                        }
                        else
                        {
                            BSITF2 = "No";
                        }
                        ////////  
                        var ASITF2 = "";
                        if (missionAuthorization12.ASITF == true)
                        {
                            ASITF2 = "Yes";
                        }
                        else
                        {
                            ASITF2 = "No";
                        }
                        ////////
                        var SSAFE2 = "";
                        if (missionAuthorization12.SSAFE == true)
                        {
                            SSAFE2 = "Yes";
                        }
                        else
                        {
                            SSAFE2 = "No";
                        }

                        //////////////////////////////


                        var supersforhou = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                        supersforhou = supersforhou.Where(d => d.staffid == staffsupervisor);
                        var superforhounitsname = supersforhou.First().staff_first_name + " " + staffshou.First().staff_last_name;
                        var houmailUPDATE = supersforhou.First().staff_email;

                        if (houmailUPDATE == "muhannad.hadi@wfp.org" || houmailUPDATE == "carlo.scaramella@wfp.org")
                        {
                            houmailUPDATE = "rbc.management@wfp.org"; //rbc.management@wfp.org

                        }
                        /////////////////////////

                        MailMessage mailhou = new MailMessage();
                        mailhou.To.Add(onbehalfemail);
                        mailhou.Bcc.Add("ahmed.badr@wfp.org");
                        //mail2.CC.Add(cc);
                        mailhou.From = new MailAddress("RBC-SMS-SSYSTEM@wfp.org");//"rbc.management@wfp.org"
                        mailhou.Subject = "HOU Rejected";

                        string Bodyhou = "<table><tr><td colspan='2'>" + "<hr/>New Mission<hr/></td></tr><tr><td><font color='blue'>Request : </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorization12.MissionID + "</td></tr>"
                            + "<tr><td>" + "<font color='blue'>From :</font></td><td>" + staffss.First().staff_first_name + " " + staffss.First().staff_last_name + "</td></tr>"
                            + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                            + "<tr><td><font color='blue'>From :</font></td><td>" + missionAuthorization12.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                            + "<tr><td><font color='blue'>To :</font></td><td>" + missionAuthorization12.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                            + "<tr><td><font color='blue'>Funding :</font></td><td>" + missionAuthorization12.funding + "</td></tr>"
                            + "<tr><td><font color='blue'>Objective :</font></td><td>" + missionAuthorization12.MissionObjective + "</td></tr>"
                            + "<tr><td><font color='blue'>Mission Itinerary :</font></td><td>" + missionAuthorization12.MissionItinerary + "</td></tr>"
                            + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video ? :</font></td><td>" + canbedone + "</td></tr>"
                            + "<tr><td><font color='blue'>Cleared By Supervisor :</font></td><td>" + CleredBySupervisor + "</td></tr>"
                             + "<tr><td><font color='blue'>HOU Comment :</font></td><td>" + missionAuthorization12.Comment1 + "</td></tr>"
                            + "<tr><td><font color='blue'>	BSITF :</font></td><td>" + BSITF2 + "</td></tr>"
                            + "<tr><td><font color='blue'> ASITF :</font></td><td>" + ASITF2 + "</td></tr>"
                            + "<tr><td><font color='blue'> SSAFE :</font></td><td>" + SSAFE2 + "</td></tr>"
                            + "<tr><td><font color='blue'> DSS Approval:</font></td><td><a href='http://10.11.135.254:8080/RBC-SMS/UploadedFiles/" + missionAuthorization12.non6 + "'>" + missionAuthorization12.non6 + "</a></td></tr></table>";

                        //string Body = "Request : Mission &nbsp;  &nbsp;  &nbsp; &nbsp;  &nbsp; &nbsp; &nbsp; &nbsp;  Mission ID : " + missionAuthorization.MissionID + " <br> " + " From &nbsp; &nbsp; &nbsp;:" + staffs.First().staff_first_name + "&nbsp;" + staffs.First().staff_last_name + "<br><br>" + " Mission Information" + "<br><br>" + " From :" + missionAuthorization.FromDate + "<br>To :" + missionAuthorization.ToDate +
                        //    "<br>Funding :" + missionAuthorization.funding + "<br>Objective :" + missionAuthorization.MissionObjective + "<br> Mission Itinerary :" + missionAuthorization.MissionItinerary + "<br> Can This meeting Be Done Over Voice/Video ? :" + missionAuthorization.canbedone  + " <br><br> <a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor=" + true + "&FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + " &to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Acept</a>&nbsp;&nbsp;&nbsp;<a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor= " + false + "& FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + "&to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Reject</a> ";

                        mailhou.Body = Bodyhou;
                        mailhou.IsBodyHtml = true;
                        SmtpClient smtphou = new SmtpClient();
                        smtphou.Host = "smtprelay.global.wfp.org";
                        smtphou.Port = 25;
                        smtphou.UseDefaultCredentials = true;
                        //smtp.Credentials = new System.Net.NetworkCredential
                        //("ahmed.badr", "Survivor2323");// Enter seders User name and password  
                        //smtp.EnableSsl = true;
                        smtphou.Send(mailhou);
                        return RedirectToAction("Index");
                    }

                    else
                    {
                        var canbedone = "";
                        if (missionAuthorization12.canbedone == true)
                        {
                            canbedone = "Yes";
                        }
                        else
                        {
                            canbedone = "No";
                        }

                        var CleredBySupervisor = "";
                        if (missionAuthorization12.canbedone == true)
                        {
                            CleredBySupervisor = "Yes";
                        }
                        else
                        {
                            CleredBySupervisor = "No";
                        }

                        var BSITF2 = "";
                        if (missionAuthorization12.BSITF == true)
                        {
                            BSITF2 = "Yes";
                        }
                        else
                        {
                            BSITF2 = "No";
                        }
                        ////////  
                        var ASITF2 = "";
                        if (missionAuthorization12.ASITF == true)
                        {
                            ASITF2 = "Yes";
                        }
                        else
                        {
                            ASITF2 = "No";
                        }
                        ////////
                        var SSAFE2 = "";
                        if (missionAuthorization12.SSAFE == true)
                        {
                            SSAFE2 = "Yes";
                        }
                        else
                        {
                            SSAFE2 = "No";
                        }

                        //////////////////////////////


                        var supersforhou = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                        supersforhou = supersforhou.Where(d => d.staffid == staffsupervisor);
                        var superforhounitsname = supersforhou.First().staff_first_name + " " + staffshou.First().staff_last_name;
                        var houmailUPDATE = supersforhou.First().staff_email;

                        if (houmailUPDATE == "muhannad.hadi@wfp.org" || houmailUPDATE == "carlo.scaramella@wfp.org")
                        {
                            houmailUPDATE = "rbc.management@wfp.org"; //rbc.management@wfp.org

                        }
                        /////////////////////////

                        MailMessage mailhou = new MailMessage();
                        mailhou.To.Add(onbehalfemail);
                        mailhou.Bcc.Add("ahmed.badr@wfp.org");
                        mailhou.CC.Add(onbehafsuperemail);
                        mailhou.From = new MailAddress(hou);
                        mailhou.Subject = "HOU Rejected";

                        string Bodyhou = "<table><tr><td colspan='2'>" + "<hr/>New Mission<hr/></td></tr><tr><td><font color='blue'>Request : </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorization12.MissionID + "</td></tr>"
                            + "<tr><td>" + "<font color='blue'>From :</font></td><td>" + staffss.First().staff_first_name + " " + staffss.First().staff_last_name + "</td></tr>"
                            + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                            + "<tr><td><font color='blue'>From :</font></td><td>" + missionAuthorization12.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                            + "<tr><td><font color='blue'>To :</font></td><td>" + missionAuthorization12.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                            + "<tr><td><font color='blue'>Funding :</font></td><td>" + missionAuthorization12.funding + "</td></tr>"
                            + "<tr><td><font color='blue'>Objective :</font></td><td>" + missionAuthorization12.MissionObjective + "</td></tr>"
                            + "<tr><td><font color='blue'>Mission Itinerary :</font></td><td>" + missionAuthorization12.MissionItinerary + "</td></tr>"
                            + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video ? :</font></td><td>" + canbedone + "</td></tr>"
                            + "<tr><td><font color='blue'>Cleared By Supervisor :</font></td><td>" + CleredBySupervisor + "</td></tr>"
                             + "<tr><td><font color='blue'>HOU Comment :</font></td><td>" + missionAuthorization12.Comment1 + "</td></tr>"
                            + "<tr><td><font color='blue'>	BSITF :</font></td><td>" + BSITF2 + "</td></tr>"
                            + "<tr><td><font color='blue'> ASITF :</font></td><td>" + ASITF2 + "</td></tr>"
                            + "<tr><td><font color='blue'> SSAFE :</font></td><td>" + SSAFE2 + "</td></tr>"
                           + "<tr><td><font color='blue'> DSS Approval:</font></td><td><a href='http://10.11.135.254:8080/RBC-SMS/UploadedFiles/" + missionAuthorization12.non6 + "'>" + missionAuthorization12.non6 + "</a></td></tr></table>";

                        //string Body = "Request : Mission &nbsp;  &nbsp;  &nbsp; &nbsp;  &nbsp; &nbsp; &nbsp; &nbsp;  Mission ID : " + missionAuthorization.MissionID + " <br> " + " From &nbsp; &nbsp; &nbsp;:" + staffs.First().staff_first_name + "&nbsp;" + staffs.First().staff_last_name + "<br><br>" + " Mission Information" + "<br><br>" + " From :" + missionAuthorization.FromDate + "<br>To :" + missionAuthorization.ToDate +
                        //    "<br>Funding :" + missionAuthorization.funding + "<br>Objective :" + missionAuthorization.MissionObjective + "<br> Mission Itinerary :" + missionAuthorization.MissionItinerary + "<br> Can This meeting Be Done Over Voice/Video ? :" + missionAuthorization.canbedone  + " <br><br> <a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor=" + true + "&FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + " &to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Acept</a>&nbsp;&nbsp;&nbsp;<a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor= " + false + "& FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + "&to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Reject</a> ";

                        mailhou.Body = Bodyhou;
                        mailhou.IsBodyHtml = true;
                        SmtpClient smtphou = new SmtpClient();
                        smtphou.Host = "smtprelay.global.wfp.org";
                        smtphou.Port = 25;
                        smtphou.UseDefaultCredentials = true;
                        //smtp.Credentials = new System.Net.NetworkCredential
                        //("ahmed.badr", "Survivor2323");// Enter seders User name and password  
                        //smtp.EnableSsl = true;
                       smtphou.Send(mailhou);
                        return RedirectToAction("Index");


                    }
                }



            }
            ///////////////////////// end on behalf 
            else
            {
                ViewBag.Mid = Request.QueryString["MissionID"];
                ViewBag.RDORDRDSignature = Request.QueryString["RDORDRDSignature"];
                ViewBag.FromDate = Request.QueryString["FromDate"];
                ViewBag.ToDate = Request.QueryString["ToDate"];
                if (RDORDRDSignature == "Yes")
                {
                    try
                    {
                        // missionAuthorization = db.MissionAuthorizations.Find(Mid);
                        //db.Entry(missionAuthorization).State = EntityState.Modified ;
                        //db.SaveChanges();

                        var missionAuthorization = db.MissionAuthorization1.Single(u => u.MissionID == MissionID);
                        missionAuthorization.Signature1 = true;
                        missionAuthorization.Signature1Date = DateTime.Now;
                        missionAuthorization.Comment1 = Comment1;
                        db.SaveChanges();



                        var staffs = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                        staffs = staffs.Where(s => s.staff_email.Contains(staff));
                        var too = staffs.First().staff_email;
                        var staffsupervisor = staffs.First().staff_supervisorid;
                        var supers = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                        supers = supers.Where(d => d.staffid == staffsupervisor);
                        var from = supers.First().staff_email;

                        //var rd = db.staffs.Include(r => r.contract_details).Include(r => r.contract_type).Include(r => r.Country_office).Include(r => r.functional_title).Include(r => r.staff2).Include(r => r.sub_office).Include(r => r.unit).Include(r => r.country).Include(r => r.governorate);
                        //rd = rd.Where(r => r.functional_title.functional_title_description_english == "REGIONAL DIRECTOR");
                        //var rdmail = rd.First().staff_email;

                        //var drd = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                        //drd = drd.Where(s => s.functional_title.functional_title_description_english == "Deputy Regional Director");
                        //var drdmail = drd.First().staff_email;

                        var userUnit = staffs.First().unit_id;
                        var unithou = db.units.Include(d => d.staffs);
                        unithou = unithou.Where(u => u.unitid == userUnit);
                        var houid = unithou.First().HOU_ID;

                        var staffshou = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                        staffshou = staffshou.Where(h => h.staffid == houid);
                        var hou = staffshou.First().staff_email;
                        //var houname = staffshou.First().staff_first_name + " " + staffshou.First().staff_last_name;

                        var cc = supers.First().staff_email;
                        var supername = supers.First().staff_first_name + " " + supers.First().staff_last_name;
                        var superemail = supers.First().staff_email;

                        //
                        //
                        //hou email
                        if (staffs.First().staffid == houid)
                        {
                            var canbedone = "";
                            if (missionAuthorization.canbedone == true)
                            {
                                canbedone = "Yes";
                            }
                            else
                            {
                                canbedone = "No";
                            }

                            var CleredBySupervisor = "";
                            if (missionAuthorization.canbedone == true)
                            {
                                CleredBySupervisor = "Yes";
                            }
                            else
                            {
                                CleredBySupervisor = "No";
                            }

                            var BSITF2 = "";
                            if (missionAuthorization.BSITF == true)
                            {
                                BSITF2 = "Yes";
                            }
                            else
                            {
                                BSITF2 = "No";
                            }
                            ////////  
                            var ASITF2 = "";
                            if (missionAuthorization.ASITF == true)
                            {
                                ASITF2 = "Yes";
                            }
                            else
                            {
                                ASITF2 = "No";
                            }
                            ////////
                            var SSAFE2 = "";
                            if (missionAuthorization.SSAFE == true)
                            {
                                SSAFE2 = "Yes";
                            }
                            else
                            {
                                SSAFE2 = "No";
                            }
                            var Roming = "";
                            if (missionAuthorization.StaffSignature == "True")
                            {
                                Roming = "Yes";
                            }
                            else
                            {
                                Roming = "No";
                            }
                            var odcitcc = "";
                            if (Roming == "Yes")
                            {
                                odcitcc = "cairo.itservicedesk@wfp.org";
                            }
                            else
                            {
                                odcitcc = "ahmed.badr@wfp.org";
                            }
                            //////////////////////////////


                            var supersforhou = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                            supersforhou = supersforhou.Where(d => d.staffid == staffsupervisor);
                            var superforhounitsname = supersforhou.First().staff_first_name + " " + staffshou.First().staff_last_name;
                            var houmailUPDATE = supersforhou.First().staff_email;

                            if (houmailUPDATE == "muhannad.hadi@wfp.org" || houmailUPDATE == "carlo.scaramella@wfp.org")
                            {
                                houmailUPDATE = "rbc.management@wfp.org"; //rbc.management@wfp.org

                            }
                            /////////////////////////

                            MailMessage mailhou = new MailMessage();
                            mailhou.To.Add(staff);
                            mailhou.Bcc.Add("ahmed.badr@wfp.org");
                            mailhou.Bcc.Add(odcitcc); //travel unit
                            //mailhou.CC.Add("rbc.travel@wfp.org"); //travel unit
                            //mailhou.CC.Add("mai.kenawi@wfp.org"); //travel unit
                            //mailhou.CC.Add("amal.mohamed@wfp.org"); //travel unit
                            //mailhou.CC.Add("ayten.el-sheikh@wfp.org"); //travel unit
                            //mailhou.CC.Add("essraa.soliman@wfp.org"); //travel unit
                            //mailhou.CC.Add("rbc.management@wfp.org"); //RD/DRD Office
                            mailhou.From = new MailAddress("RBC-SMS-SSYSTEM@wfp.org");//"rbc.management@wfp.org"
                            mailhou.Subject = "HOU Approved";

                            string Bodyhou = "<table><tr><td colspan='2'>" + "<hr/>New Mission<hr/></td></tr><tr><td><font color='blue'>Request : </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorization.MissionID + "</td></tr>"
                                + "<tr><td>" + "<font color='blue'>From :</font></td><td>" + staffs.First().staff_first_name + " " + staffs.First().staff_last_name + "</td></tr>"
                                + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                                + "<tr><td><font color='blue'>From :</font></td><td>" + missionAuthorization.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                                + "<tr><td><font color='blue'>To :</font></td><td>" + missionAuthorization.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                                + "<tr><td><font color='blue'>Funding :</font></td><td>" + missionAuthorization.funding + "</td></tr>"
                                + "<tr><td><font color='blue'>Objective :</font></td><td>" + missionAuthorization.MissionObjective + "</td></tr>"
                                + "<tr><td><font color='blue'>Mission Itinerary :</font></td><td>" + missionAuthorization.MissionItinerary + "</td></tr>"
                                + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video ? :</font></td><td>" + canbedone + "</td></tr>"
                                + "<tr><td><font color='blue'>Cleared By Supervisor :</font></td><td>" + CleredBySupervisor + "</td></tr>"
                                + "<tr><td><font color='blue'>International Roaming :</font></td><td>" + Roming + "</td></tr>"
                                 + "<tr><td><font color='blue'>HOU Comment :</font></td><td>" + missionAuthorization.Comment1 + "</td></tr>"
                                + "<tr><td><font color='blue'>	BSITF :</font></td><td>" + BSITF2 + "</td></tr>"
                                + "<tr><td><font color='blue'> ASITF :</font></td><td>" + ASITF2 + "</td></tr>"
                                + "<tr><td><font color='blue'> SSAFE :</font></td><td>" + SSAFE2 + "</td></tr>"
                               + "<tr><td><font color='blue'> DSS Approval:</font></td><td><a target='_blank' href='http://10.11.135.254:8080/RBC-SMS/UploadedFiles/" + missionAuthorization.non6 + "'>" + missionAuthorization.non6 + "</a></td></tr></table>";

                            //string Body = "Request : Mission &nbsp;  &nbsp;  &nbsp; &nbsp;  &nbsp; &nbsp; &nbsp; &nbsp;  Mission ID : " + missionAuthorization.MissionID + " <br> " + " From &nbsp; &nbsp; &nbsp;:" + staffs.First().staff_first_name + "&nbsp;" + staffs.First().staff_last_name + "<br><br>" + " Mission Information" + "<br><br>" + " From :" + missionAuthorization.FromDate + "<br>To :" + missionAuthorization.ToDate +
                            //    "<br>Funding :" + missionAuthorization.funding + "<br>Objective :" + missionAuthorization.MissionObjective + "<br> Mission Itinerary :" + missionAuthorization.MissionItinerary + "<br> Can This meeting Be Done Over Voice/Video ? :" + missionAuthorization.canbedone  + " <br><br> <a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor=" + true + "&FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + " &to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Acept</a>&nbsp;&nbsp;&nbsp;<a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor= " + false + "& FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + "&to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Reject</a> ";

                            mailhou.Body = Bodyhou;
                            mailhou.IsBodyHtml = true;
                            SmtpClient smtphou = new SmtpClient();
                            smtphou.Host = "smtprelay.global.wfp.org";
                            smtphou.Port = 25;
                            smtphou.UseDefaultCredentials = true;
                            //smtp.Credentials = new System.Net.NetworkCredential
                            //("ahmed.badr", "Survivor2323");// Enter seders User name and password  
                            //smtp.EnableSsl = true;
                           smtphou.Send(mailhou);


                            ///////////////////////////// Security Approvall //////////////////////

                            MailMessage mailhou1 = new MailMessage();
                            mailhou1.To.Add("ahmed.badr@wfp.org");
                            mailhou1.Bcc.Add("ahmed.badr@wfp.org");

                            //mailhou.CC.Add("rbc.travel@wfp.org"); //travel unit
                            //mailhou.CC.Add("mai.kenawi@wfp.org"); //travel unit
                            //mailhou.CC.Add("amal.mohamed@wfp.org"); //travel unit
                            //mailhou.CC.Add("ayten.el-sheikh@wfp.org"); //travel unit
                            //mailhou.CC.Add("essraa.soliman@wfp.org"); //travel unit
                            //mailhou.CC.Add("rbc.management@wfp.org"); //RD/DRD Office
                            mailhou1.Bcc.Add(odcitcc); //travel unit
                            mailhou1.From = new MailAddress("RBC-SMS-SSYSTEM@wfp.org");//"rbc.management@wfp.org"
                            mailhou1.Subject = "For Security Approval";

                            string Bodyhou1 = "<table><tr><td colspan='2'>" + "<hr/>New Mission<hr/></td></tr><tr><td><font color='blue'>Request : </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorization.MissionID + "</td></tr>"
                                + "<tr><td>" + "<font color='blue'>From :</font></td><td>" + staffs.First().staff_first_name + " " + staffs.First().staff_last_name + "</td></tr>"
                                + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                                + "<tr><td><font color='blue'>From :</font></td><td>" + missionAuthorization.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                                + "<tr><td><font color='blue'>To :</font></td><td>" + missionAuthorization.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                                + "<tr><td><font color='blue'>Funding :</font></td><td>" + missionAuthorization.funding + "</td></tr>"
                                + "<tr><td><font color='blue'>Objective :</font></td><td>" + missionAuthorization.MissionObjective + "</td></tr>"
                                + "<tr><td><font color='blue'>Mission Itinerary :</font></td><td>" + missionAuthorization.MissionItinerary + "</td></tr>"
                                + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video ? :</font></td><td>" + canbedone + "</td></tr>"
                                + "<tr><td><font color='blue'>Cleared By Supervisor :</font></td><td>" + CleredBySupervisor + "</td></tr>"
                                + "<tr><td><font color='blue'>International Roaming :</font></td><td>" + Roming + "</td></tr>"
                                + "<tr><td><font color='blue'>HOU Comment :</font></td><td>" + missionAuthorization.Comment1 + "</td></tr>"
                                + "<tr><td><font color='blue'>	BSITF :</font></td><td>" + BSITF2 + "</td></tr>"
                                + "<tr><td><font color='blue'> ASITF :</font></td><td>" + ASITF2 + "</td></tr>"
                                + "<tr><td><font color='blue'> SSAFE :</font></td><td>" + SSAFE2 + "</td></tr>"
                                + "<tr><td><font color='blue'> DSS Approval:</font></td><td><a target='_blank' href='http://10.11.135.254:8080/RBC-SMS/UploadedFiles/" + missionAuthorization.non6 + "'>" + missionAuthorization.non6

                             + "<tr><td><a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorization1/Approval2Security?MissionID2="
                            + missionAuthorization.MissionID
                            + "&RDORDRDSignature2=Yes&FromDate2=" + missionAuthorization.FromDate
                            + "&ToDate2=" + missionAuthorization.ToDate
                            + "&to2=" + to + "&staffmail2=" + staffs.First().staff_email
                            + "'>Accept</a></td><td><a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorization1/Approval2Security?MissionID2=" + missionAuthorization.MissionID
                            + "&RDORDRDSignature2=NO & FromDate2=" + missionAuthorization.FromDate
                            + "&ToDate2=" + missionAuthorization.ToDate
                            + "&to2=" + to
                            + "&staffmail2=" + staffs.First().staff_email + "'>Reject</a>" + "</a></td></tr></table>";

                            //string Body = "Request : Mission &nbsp;  &nbsp;  &nbsp; &nbsp;  &nbsp; &nbsp; &nbsp; &nbsp;  Mission ID : " + missionAuthorization.MissionID + " <br> " + " From &nbsp; &nbsp; &nbsp;:" + staffs.First().staff_first_name + "&nbsp;" + staffs.First().staff_last_name + "<br><br>" + " Mission Information" + "<br><br>" + " From :" + missionAuthorization.FromDate + "<br>To :" + missionAuthorization.ToDate +
                            //    "<br>Funding :" + missionAuthorization.funding + "<br>Objective :" + missionAuthorization.MissionObjective + "<br> Mission Itinerary :" + missionAuthorization.MissionItinerary + "<br> Can This meeting Be Done Over Voice/Video ? :" + missionAuthorization.canbedone  + " <br><br> <a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor=" + true + "&FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + " &to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Acept</a>&nbsp;&nbsp;&nbsp;<a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor= " + false + "& FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + "&to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Reject</a> ";

                            mailhou1.Body = Bodyhou1;
                            mailhou1.IsBodyHtml = true;
                            SmtpClient smtphou1 = new SmtpClient();
                            smtphou1.Host = "smtprelay.global.wfp.org";
                            smtphou1.Port = 25;
                            smtphou1.UseDefaultCredentials = true;
                            //smtp.Credentials = new System.Net.NetworkCredential
                            //("ahmed.badr", "Survivor2323");// Enter seders User name and password  
                            //smtp.EnableSsl = true;
                          smtphou1.Send(mailhou1);

                            ///////////////////////////// Security Approvall end //////////////////////

                            return RedirectToAction("Index");
                        }

                        else
                        {
                            var canbedone = "";
                            if (missionAuthorization.canbedone == true)
                            {
                                canbedone = "Yes";
                            }
                            else
                            {
                                canbedone = "No";
                            }

                            var CleredBySupervisor = "";
                            if (missionAuthorization.canbedone == true)
                            {
                                CleredBySupervisor = "Yes";
                            }
                            else
                            {
                                CleredBySupervisor = "No";
                            }

                            var BSITF2 = "";
                            if (missionAuthorization.BSITF == true)
                            {
                                BSITF2 = "Yes";
                            }
                            else
                            {
                                BSITF2 = "No";
                            }
                            ////////  
                            var ASITF2 = "";
                            if (missionAuthorization.ASITF == true)
                            {
                                ASITF2 = "Yes";
                            }
                            else
                            {
                                ASITF2 = "No";
                            }
                            ////////
                            var SSAFE2 = "";
                            if (missionAuthorization.SSAFE == true)
                            {
                                SSAFE2 = "Yes";
                            }
                            else
                            {
                                SSAFE2 = "No";
                            }

                            //////////////////////////////


                            var supersforhou = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                            supersforhou = supersforhou.Where(d => d.staffid == staffsupervisor);
                            var superforhounitsname = supersforhou.First().staff_first_name + " " + staffshou.First().staff_last_name;
                            var houmailUPDATE = supersforhou.First().staff_email;

                            if (houmailUPDATE == "muhannad.hadi@wfp.org" || houmailUPDATE == "carlo.scaramella@wfp.org")
                            {
                                houmailUPDATE = "rbc.management@wfp.org"; //rbc.management@wfp.org

                            }
                            var odcitcc = "";

                            var Roming = "";
                            if (missionAuthorization.StaffSignature == "True")
                            {
                                Roming = "Yes";
                            }
                            else
                            {
                                Roming = "No";
                            }
                            if (Roming == "Yes")
                            {
                                odcitcc = "cairo.itservicedesk@wfp.org";
                            }
                            else
                            {
                                odcitcc = "ahmed.badr@wfp.org";
                            }
                            /////////////////////////

                            MailMessage mailhou = new MailMessage();
                            mailhou.To.Add(staff); //staffmail
                            mailhou.Bcc.Add("ahmed.badr@wfp.org");
                            mailhou.Bcc.Add(odcitcc); //travel unit
                            mailhou.CC.Add(superemail);
                            //mailhou.CC.Add("rbc.travel@wfp.org"); //travel unit
                            //mailhou.CC.Add("mai.kenawi@wfp.org"); //travel unit
                            //mailhou.CC.Add("amal.mohamed@wfp.org"); //travel unit
                            //mailhou.CC.Add("ayten.el-sheikh@wfp.org"); //travel unit
                            //mailhou.CC.Add("essraa.soliman@wfp.org"); //travel unit
                            //mailhou.CC.Add("rbc.management@wfp.org"); //RD/DRD Office
                            mailhou.From = new MailAddress(hou);
                            mailhou.Subject = "HOU Approved";

                            string Bodyhou = "<table><tr><td colspan='2'>" + "<hr/>New Mission<hr/></td></tr><tr><td><font color='blue'>Request : </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorization.MissionID + "</td></tr>"
                                + "<tr><td>" + "<font color='blue'>From :</font></td><td>" + staffs.First().staff_first_name + " " + staffs.First().staff_last_name + "</td></tr>"
                                + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                                + "<tr><td><font color='blue'>From :</font></td><td>" + missionAuthorization.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                                + "<tr><td><font color='blue'>To :</font></td><td>" + missionAuthorization.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                                + "<tr><td><font color='blue'>Funding :</font></td><td>" + missionAuthorization.funding + "</td></tr>"
                                + "<tr><td><font color='blue'>Objective :</font></td><td>" + missionAuthorization.MissionObjective + "</td></tr>"
                                + "<tr><td><font color='blue'>Mission Itinerary :</font></td><td>" + missionAuthorization.MissionItinerary + "</td></tr>"
                                + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video ? :</font></td><td>" + canbedone + "</td></tr>"
                                + "<tr><td><font color='blue'>Cleared By Supervisor :</font></td><td>" + CleredBySupervisor + "</td></tr>"
                                + "<tr><td><font color='blue'>International Roaming :</font></td><td>" + Roming + "</td></tr>"
                                 + "<tr><td><font color='blue'>HOU Comment :</font></td><td>" + missionAuthorization.Comment1 + "</td></tr>"
                                + "<tr><td><font color='blue'>	BSITF :</font></td><td>" + BSITF2 + "</td></tr>"
                                + "<tr><td><font color='blue'> ASITF :</font></td><td>" + ASITF2 + "</td></tr>"
                                + "<tr><td><font color='blue'> SSAFE :</font></td><td>" + SSAFE2 + "</td></tr>"
                               + "<tr><td><font color='blue'> DSS Approval:</font></td><td><a target='_blank' href='http://10.11.135.254:8080/RBC-SMS/UploadedFiles/" + missionAuthorization.non6 + "'>" + missionAuthorization.non6 + "</a></td></tr></table>";

                            //string Body = "Request : Mission &nbsp;  &nbsp;  &nbsp; &nbsp;  &nbsp; &nbsp; &nbsp; &nbsp;  Mission ID : " + missionAuthorization.MissionID + " <br> " + " From &nbsp; &nbsp; &nbsp;:" + staffs.First().staff_first_name + "&nbsp;" + staffs.First().staff_last_name + "<br><br>" + " Mission Information" + "<br><br>" + " From :" + missionAuthorization.FromDate + "<br>To :" + missionAuthorization.ToDate +
                            //    "<br>Funding :" + missionAuthorization.funding + "<br>Objective :" + missionAuthorization.MissionObjective + "<br> Mission Itinerary :" + missionAuthorization.MissionItinerary + "<br> Can This meeting Be Done Over Voice/Video ? :" + missionAuthorization.canbedone  + " <br><br> <a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor=" + true + "&FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + " &to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Acept</a>&nbsp;&nbsp;&nbsp;<a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor= " + false + "& FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + "&to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Reject</a> ";

                            mailhou.Body = Bodyhou;
                            mailhou.IsBodyHtml = true;
                            SmtpClient smtphou = new SmtpClient();
                            smtphou.Host = "smtprelay.global.wfp.org";
                            smtphou.Port = 25;
                            smtphou.UseDefaultCredentials = true;
                            //smtp.Credentials = new System.Net.NetworkCredential
                            //("ahmed.badr", "Survivor2323");// Enter seders User name and password  
                            //smtp.EnableSsl = true;
                           smtphou.Send(mailhou);

                            ///////////////////////////// Security Approvall //////////////////////

                            MailMessage mailhou1 = new MailMessage();
                            mailhou1.To.Add("ahmed.badr@wfp.org");
                            mailhou1.Bcc.Add("ahmed.badr@wfp.org");

                            //mailhou.CC.Add("rbc.travel@wfp.org"); //travel unit
                            //mailhou.CC.Add("mai.kenawi@wfp.org"); //travel unit
                            //mailhou.CC.Add("amal.mohamed@wfp.org"); //travel unit
                            //mailhou.CC.Add("ayten.el-sheikh@wfp.org"); //travel unit
                            //mailhou.CC.Add("essraa.soliman@wfp.org"); //travel unit
                            //mailhou.CC.Add("rbc.management@wfp.org"); //RD/DRD Office
                            mailhou1.Bcc.Add(odcitcc); //travel unit
                            mailhou1.From = new MailAddress("RBC-SMS-SSYSTEM@wfp.org");//"rbc.management@wfp.org"
                            mailhou1.Subject = "For Security Approval";

                            string Bodyhou1 = "<table><tr><td colspan='2'>" + "<hr/>New Mission<hr/></td></tr><tr><td><font color='blue'>Request : </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorization.MissionID + "</td></tr>"
                                + "<tr><td>" + "<font color='blue'>From :</font></td><td>" + staffs.First().staff_first_name + " " + staffs.First().staff_last_name + "</td></tr>"
                                + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                                + "<tr><td><font color='blue'>From :</font></td><td>" + missionAuthorization.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                                + "<tr><td><font color='blue'>To :</font></td><td>" + missionAuthorization.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                                + "<tr><td><font color='blue'>Funding :</font></td><td>" + missionAuthorization.funding + "</td></tr>"
                                + "<tr><td><font color='blue'>Objective :</font></td><td>" + missionAuthorization.MissionObjective + "</td></tr>"
                                + "<tr><td><font color='blue'>Mission Itinerary :</font></td><td>" + missionAuthorization.MissionItinerary + "</td></tr>"
                                + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video ? :</font></td><td>" + canbedone + "</td></tr>"
                                + "<tr><td><font color='blue'>Cleared By Supervisor :</font></td><td>" + CleredBySupervisor + "</td></tr>"
                                + "<tr><td><font color='blue'>International Roaming :</font></td><td>" + Roming + "</td></tr>"
                                + "<tr><td><font color='blue'>HOU Comment :</font></td><td>" + missionAuthorization.Comment1 + "</td></tr>"
                                + "<tr><td><font color='blue'>	BSITF :</font></td><td>" + BSITF2 + "</td></tr>"
                                + "<tr><td><font color='blue'> ASITF :</font></td><td>" + ASITF2 + "</td></tr>"
                                + "<tr><td><font color='blue'> SSAFE :</font></td><td>" + SSAFE2 + "</td></tr>"
                                + "<tr><td><font color='blue'> DSS Approval:</font></td><td><a target='_blank' href='http://10.11.135.254:8080/RBC-SMS/UploadedFiles/" + missionAuthorization.non6 + "'>" + missionAuthorization.non6


                                 + "<tr><td><a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorization1/Approval2Security?MissionID2="
                            + missionAuthorization.MissionID
                            + "&RDORDRDSignature2=Yes&FromDate2=" + missionAuthorization.FromDate
                            + "&ToDate2=" + missionAuthorization.ToDate
                            + "&to2=" + to + "&staffmail2=" + staffs.First().staff_email
                            + "'>Accept</a></td><td><a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorization1/Approval2Security?MissionID2=" + missionAuthorization.MissionID
                            + "&RDORDRDSignature2=NO & FromDate2=" + missionAuthorization.FromDate
                            + "&ToDate2=" + missionAuthorization.ToDate
                            + "&to2=" + to
                            + "&staffmail2=" + staffs.First().staff_email  + "'>Reject</a>" + "</a></td></tr></table>";


                            //string Body = "Request : Mission &nbsp;  &nbsp;  &nbsp; &nbsp;  &nbsp; &nbsp; &nbsp; &nbsp;  Mission ID : " + missionAuthorization.MissionID + " <br> " + " From &nbsp; &nbsp; &nbsp;:" + staffs.First().staff_first_name + "&nbsp;" + staffs.First().staff_last_name + "<br><br>" + " Mission Information" + "<br><br>" + " From :" + missionAuthorization.FromDate + "<br>To :" + missionAuthorization.ToDate +
                            //    "<br>Funding :" + missionAuthorization.funding + "<br>Objective :" + missionAuthorization.MissionObjective + "<br> Mission Itinerary :" + missionAuthorization.MissionItinerary + "<br> Can This meeting Be Done Over Voice/Video ? :" + missionAuthorization.canbedone  + " <br><br> <a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor=" + true + "&FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + " &to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Acept</a>&nbsp;&nbsp;&nbsp;<a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor= " + false + "& FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + "&to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Reject</a> ";

                            mailhou1.Body = Bodyhou1;
                            mailhou1.IsBodyHtml = true;
                            SmtpClient smtphou1 = new SmtpClient();
                            smtphou1.Host = "smtprelay.global.wfp.org";
                            smtphou1.Port = 25;
                            smtphou1.UseDefaultCredentials = true;
                            //smtp.Credentials = new System.Net.NetworkCredential
                            //("ahmed.badr", "Survivor2323");// Enter seders User name and password  
                            //smtp.EnableSsl = true;
                           smtphou1.Send(mailhou1);

                            ///////////////////////////// Security Approvall end //////////////////////

                            return RedirectToAction("Index");


                        }
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
                    return View();
                }
                else
                {
                    //var missionAuthorizationn = db.MissionAuthorizations.Single(u => u.MissionID == Mid);
                    //db.MissionAuthorizations.Remove(missionAuthorizationn);
                    //db.SaveChanges();
                    var missionAuthorizationn = db.MissionAuthorization1.Single(u => u.MissionID == MissionID);
                    missionAuthorizationn.Signature1 = false;
                    missionAuthorizationn.Signature1Date = DateTime.Now;
                    missionAuthorizationn.Comment1 = Comment1;
                    db.SaveChanges();


                    var staffss = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                    staffss = staffss.Where(s => s.staff_email.Contains(staff));
                    var too = staffss.First().staff_email;
                    var staffsupervisor = staffss.First().staff_supervisorid;
                    var supers = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                    supers = supers.Where(d => d.staffid == staffsupervisor);
                    var from = supers.First().staff_email;

                    //var rd = db.staffs.Include(r => r.contract_details).Include(r => r.contract_type).Include(r => r.Country_office).Include(r => r.functional_title).Include(r => r.staff2).Include(r => r.sub_office).Include(r => r.unit).Include(r => r.country).Include(r => r.governorate);
                    //rd = rd.Where(r => r.functional_title.functional_title_description_english == "REGIONAL DIRECTOR");
                    //var rdmail = rd.First().staff_email;

                    //var drd = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                    //drd = drd.Where(s => s.functional_title.functional_title_description_english == "Deputy Regional Director");
                    //var drdmail = drd.First().staff_email;


                    //new
                    var userUnit = staffss.First().unit_id;
                    var unithou = db.units.Include(d => d.staffs);
                    unithou = unithou.Where(u => u.unitid == userUnit);
                    var houid = unithou.First().HOU_ID;

                    var staffshou = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                    staffshou = staffshou.Where(h => h.staffid == houid);
                    var hou = staffshou.First().staff_email;
                    //var houname = staffshou.First().staff_first_name + " " + staffshou.First().staff_last_name;

                    var cc = supers.First().staff_email;
                    var supername = supers.First().staff_first_name + " " + supers.First().staff_last_name;


                

                  

                 
                    var superemail = supers.First().staff_email;

                    //
                    //
                    //hou email
                    if (staffss.First().staffid == houid)
                    {
                        var canbedone = "";
                        if (missionAuthorizationn.canbedone == true)
                        {
                            canbedone = "Yes";
                        }
                        else
                        {
                            canbedone = "No";
                        }
                        var Roming = "";
                        if (missionAuthorizationn.StaffSignature == "True")
                        {
                            Roming = "Yes";
                        }
                        else
                        {
                            Roming = "No";
                        }
                        var CleredBySupervisor = "";
                        if (missionAuthorizationn.canbedone == true)
                        {
                            CleredBySupervisor = "Yes";
                        }
                        else
                        {
                            CleredBySupervisor = "No";
                        }

                        var BSITF2 = "";
                        if (missionAuthorizationn.BSITF == true)
                        {
                            BSITF2 = "Yes";
                        }
                        else
                        {
                            BSITF2 = "No";
                        }
                        ////////  
                        var ASITF2 = "";
                        if (missionAuthorizationn.ASITF == true)
                        {
                            ASITF2 = "Yes";
                        }
                        else
                        {
                            ASITF2 = "No";
                        }
                        ////////
                        var SSAFE2 = "";
                        if (missionAuthorizationn.SSAFE == true)
                        {
                            SSAFE2 = "Yes";
                        }
                        else
                        {
                            SSAFE2 = "No";
                        }
                        var odcitcc = "";
                        if (Roming == "Yes")
                        {
                            odcitcc = "cairo.itservicedesk@wfp.org";
                        }
                        else
                        {
                            odcitcc = "ahmed.badr@wfp.org";
                        }
                        //////////////////////////////


                        var supersforhou = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                        supersforhou = supersforhou.Where(d => d.staffid == staffsupervisor);
                        var superforhounitsname = supersforhou.First().staff_first_name + " " + staffshou.First().staff_last_name;
                        var houmailUPDATE = supersforhou.First().staff_email;

                        if (houmailUPDATE == "muhannad.hadi@wfp.org" || houmailUPDATE == "carlo.scaramella@wfp.org")
                        {
                            houmailUPDATE = "rbc.management@wfp.org"; //rbc.management@wfp.org

                        }
                        /////////////////////////

                        MailMessage mailhou = new MailMessage();
                        mailhou.To.Add(staff);
                        mailhou.Bcc.Add("ahmed.badr@wfp.org");
                        //mail2.CC.Add(cc);
                        mailhou.From = new MailAddress("RBC-SMS-SSYSTEM@wfp.org");//"rbc.management@wfp.org"
                        mailhou.Subject = "HOU Rejected";

                        string Bodyhou = "<table><tr><td colspan='2'>" + "<hr/>New Mission<hr/></td></tr><tr><td><font color='blue'>Request : </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorizationn.MissionID + "</td></tr>"
                            + "<tr><td>" + "<font color='blue'>From :</font></td><td>" + staffss.First().staff_first_name + " " + staffss.First().staff_last_name + "</td></tr>"
                            + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                            + "<tr><td><font color='blue'>From :</font></td><td>" + missionAuthorizationn.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                            + "<tr><td><font color='blue'>To :</font></td><td>" + missionAuthorizationn.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                            + "<tr><td><font color='blue'>Funding :</font></td><td>" + missionAuthorizationn.funding + "</td></tr>"
                            + "<tr><td><font color='blue'>Objective :</font></td><td>" + missionAuthorizationn.MissionObjective + "</td></tr>"
                            + "<tr><td><font color='blue'>Mission Itinerary :</font></td><td>" + missionAuthorizationn.MissionItinerary + "</td></tr>"
                            + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video ? :</font></td><td>" + canbedone + "</td></tr>"
                            + "<tr><td><font color='blue'>Cleared By Supervisor :</font></td><td>" + CleredBySupervisor + "</td></tr>"
                             + "<tr><td><font color='blue'>HOU Comment :</font></td><td>" + missionAuthorizationn.Comment1 + "</td></tr>"
                            + "<tr><td><font color='blue'>	BSITF :</font></td><td>" + BSITF2 + "</td></tr>"
                            + "<tr><td><font color='blue'> ASITF :</font></td><td>" + ASITF2 + "</td></tr>"
                            + "<tr><td><font color='blue'> SSAFE :</font></td><td>" + SSAFE2 + "</td></tr>"
                           + "<tr><td><font color='blue'> DSS Approval:</font></td><td><a target='_blank' href='http://10.11.135.254:8080/RBC-SMS/UploadedFiles/" + missionAuthorizationn.non6 + "'>" + missionAuthorizationn.non6 + "</a></td></tr></table>";

                        //string Body = "Request : Mission &nbsp;  &nbsp;  &nbsp; &nbsp;  &nbsp; &nbsp; &nbsp; &nbsp;  Mission ID : " + missionAuthorization.MissionID + " <br> " + " From &nbsp; &nbsp; &nbsp;:" + staffs.First().staff_first_name + "&nbsp;" + staffs.First().staff_last_name + "<br><br>" + " Mission Information" + "<br><br>" + " From :" + missionAuthorization.FromDate + "<br>To :" + missionAuthorization.ToDate +
                        //    "<br>Funding :" + missionAuthorization.funding + "<br>Objective :" + missionAuthorization.MissionObjective + "<br> Mission Itinerary :" + missionAuthorization.MissionItinerary + "<br> Can This meeting Be Done Over Voice/Video ? :" + missionAuthorization.canbedone  + " <br><br> <a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor=" + true + "&FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + " &to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Acept</a>&nbsp;&nbsp;&nbsp;<a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor= " + false + "& FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + "&to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Reject</a> ";

                        mailhou.Body = Bodyhou;
                        mailhou.IsBodyHtml = true;
                        SmtpClient smtphou = new SmtpClient();
                        smtphou.Host = "smtprelay.global.wfp.org";
                        smtphou.Port = 25;
                        smtphou.UseDefaultCredentials = true;
                        //smtp.Credentials = new System.Net.NetworkCredential
                        //("ahmed.badr", "Survivor2323");// Enter seders User name and password  
                        //smtp.EnableSsl = true;
                        smtphou.Send(mailhou);
                        return RedirectToAction("Index");
                    }

                    else
                    {
                        var canbedone = "";
                        if (missionAuthorizationn.canbedone == true)
                        {
                            canbedone = "Yes";
                        }
                        else
                        {
                            canbedone = "No";
                        }

                        var CleredBySupervisor = "";
                        if (missionAuthorizationn.canbedone == true)
                        {
                            CleredBySupervisor = "Yes";
                        }
                        else
                        {
                            CleredBySupervisor = "No";
                        }

                        var BSITF2 = "";
                        if (missionAuthorizationn.BSITF == true)
                        {
                            BSITF2 = "Yes";
                        }
                        else
                        {
                            BSITF2 = "No";
                        }
                        ////////  
                        var ASITF2 = "";
                        if (missionAuthorizationn.ASITF == true)
                        {
                            ASITF2 = "Yes";
                        }
                        else
                        {
                            ASITF2 = "No";
                        }
                        ////////
                        var SSAFE2 = "";
                        if (missionAuthorizationn.SSAFE == true)
                        {
                            SSAFE2 = "Yes";
                        }
                        else
                        {
                            SSAFE2 = "No";
                        }

                        //////////////////////////////


                        var supersforhou = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                        supersforhou = supersforhou.Where(d => d.staffid == staffsupervisor);
                        var superforhounitsname = supersforhou.First().staff_first_name + " " + staffshou.First().staff_last_name;
                        var houmailUPDATE = supersforhou.First().staff_email;

                        if (houmailUPDATE == "muhannad.hadi@wfp.org" || houmailUPDATE == "carlo.scaramella@wfp.org")
                        {
                            houmailUPDATE = "rbc.management@wfp.org"; //rbc.management@wfp.org

                        }
                        /////////////////////////


                        MailMessage mailhou = new MailMessage();
                        mailhou.To.Add(staff);
                        mailhou.Bcc.Add("ahmed.badr@wfp.org");
                        mailhou.CC.Add(cc);
                        mailhou.From = new MailAddress(hou);
                        mailhou.Subject = "HOU Rejected";

                        string Bodyhou = "<table><tr><td colspan='2'>" + "<hr/>New Mission<hr/></td></tr><tr><td><font color='blue'>Request : </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorizationn.MissionID + "</td></tr>"
                            + "<tr><td>" + "<font color='blue'>From :</font></td><td>" + staffss.First().staff_first_name + " " + staffss.First().staff_last_name + "</td></tr>"
                            + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                            + "<tr><td><font color='blue'>From :</font></td><td>" + missionAuthorizationn.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                            + "<tr><td><font color='blue'>To :</font></td><td>" + missionAuthorizationn.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                            + "<tr><td><font color='blue'>Funding :</font></td><td>" + missionAuthorizationn.funding + "</td></tr>"
                            + "<tr><td><font color='blue'>Objective :</font></td><td>" + missionAuthorizationn.MissionObjective + "</td></tr>"
                            + "<tr><td><font color='blue'>Mission Itinerary :</font></td><td>" + missionAuthorizationn.MissionItinerary + "</td></tr>"
                            + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video ? :</font></td><td>" + canbedone + "</td></tr>"
                            + "<tr><td><font color='blue'>Cleared By Supervisor :</font></td><td>" + CleredBySupervisor + "</td></tr>"
                             + "<tr><td><font color='blue'>HOU Comment :</font></td><td>" + missionAuthorizationn.Comment1 + "</td></tr>"
                            + "<tr><td><font color='blue'>	BSITF :</font></td><td>" + BSITF2 + "</td></tr>"
                            + "<tr><td><font color='blue'> ASITF :</font></td><td>" + ASITF2 + "</td></tr>"
                            + "<tr><td><font color='blue'> SSAFE :</font></td><td>" + SSAFE2 + "</td></tr>"
                           + "<tr><td><font color='blue'> DSS Approval:</font></td><td><a target='_blank' href='http://10.11.135.254:8080/RBC-SMS/UploadedFiles/" + missionAuthorizationn.non6 + "'>" + missionAuthorizationn.non6 + "</a></td></tr></table>";

                        //string Body = "Request : Mission &nbsp;  &nbsp;  &nbsp; &nbsp;  &nbsp; &nbsp; &nbsp; &nbsp;  Mission ID : " + missionAuthorization.MissionID + " <br> " + " From &nbsp; &nbsp; &nbsp;:" + staffs.First().staff_first_name + "&nbsp;" + staffs.First().staff_last_name + "<br><br>" + " Mission Information" + "<br><br>" + " From :" + missionAuthorization.FromDate + "<br>To :" + missionAuthorization.ToDate +
                        //    "<br>Funding :" + missionAuthorization.funding + "<br>Objective :" + missionAuthorization.MissionObjective + "<br> Mission Itinerary :" + missionAuthorization.MissionItinerary + "<br> Can This meeting Be Done Over Voice/Video ? :" + missionAuthorization.canbedone  + " <br><br> <a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor=" + true + "&FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + " &to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Acept</a>&nbsp;&nbsp;&nbsp;<a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor= " + false + "& FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + "&to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Reject</a> ";

                        mailhou.Body = Bodyhou;
                        mailhou.IsBodyHtml = true;
                        SmtpClient smtphou = new SmtpClient();
                        smtphou.Host = "smtprelay.global.wfp.org";
                        smtphou.Port = 25;
                        smtphou.UseDefaultCredentials = true;
                        //smtp.Credentials = new System.Net.NetworkCredential
                        //("ahmed.badr", "Survivor2323");// Enter seders User name and password  
                        //smtp.EnableSsl = true;
                       smtphou.Send(mailhou);
                        return RedirectToAction("Index");


                    }
                }
            }
            return View();

        }
        /////////////////////////////////////////////////////////////////////// start approval level 2 security //////////////////////////////////////////////////////////////////////////////////////////

        public ActionResult Approval2Security(int? i, int? MissionID2, string RDORDRDSignature2, DateTime? FromDate2, DateTime? ToDate2, string to2, string staffmail2, string supervisorname2, int? onbehalfID2, int? onbehalfSuperID2, int? onbehalfHouID2)
        {
            ViewBag.MissionID = MissionID2;
            ViewBag.RDORDRDSignature = RDORDRDSignature2;
            ViewBag.FromDate = FromDate2;
            ViewBag.ToDate = ToDate2;
            ViewBag.to = to2;
            ViewBag.staffmail = staffmail2;
            ViewBag.supervisorname = supervisorname2;
            ViewBag.onbehalfID = onbehalfID2;
            ViewBag.onbehalfSuperID = onbehalfSuperID2;
            ViewBag.onbehalfHouID = onbehalfHouID2;

            return View();
        }

        [HttpPost]
        public ActionResult Approval2Security(int? MissionID, string Comment2, string RDORDRDSignature, DateTime? FromDate, DateTime? ToDate, string non10, string staff, string supervisorname, int? onbehalfID, int? onbehalfSuperID, int? onbehalfHouID)
        {


            if (!Request.IsAuthenticated)
            {
                Response.Redirect("/RBC-SMS/account/login");
            }

            ///////////////////////// start on behalf 

            if (onbehalfID != null && onbehalfSuperID != null && onbehalfHouID != null)
            {
                ViewBag.mid = Request.QueryString["MissionID"];
                ViewBag.RDORDRDSignature = Request.QueryString["RDORDRDSignature"];
                ViewBag.FromDate = Request.QueryString["FromDate"];
                ViewBag.ToDate = Request.QueryString["ToDate"];
                if (RDORDRDSignature == "Yes")
                {
                    try
                    {
                        // missionAuthorization = db.MissionAuthorizations.Find(Mid);
                        //db.Entry(missionAuthorization).State = EntityState.Modified ;
                        //db.SaveChanges();

                        var missionAuthorization11 = db.MissionAuthorization1.Single(u => u.MissionID == MissionID);
                        missionAuthorization11.Signature2 = true;
                        missionAuthorization11.Signature2Date = DateTime.Now;
                        missionAuthorization11.Comment2 = Comment2;
                        db.SaveChanges();



                        var staffs = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                        staffs = staffs.Where(s => s.staffid == onbehalfID);
                        var too = staffs.First().staff_email;
                        ////////////////////////////////////////////////////////////////////////////onbehalf
                        var staffsupervisor = staffs.First().staff_supervisorid;
                        var supers = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                        supers = supers.Where(d => d.staffid == onbehalfID);
                        var onbehalfemail = supers.First().staff_email;


                        var supersonbehalf = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                        supersonbehalf = supersonbehalf.Where(d => d.staffid == onbehalfSuperID);
                        var onbehafsuper = supersonbehalf.First().staff_email;
                        /////////////////////////////////////////////////////////////////////////////////////
                        ////////////////////////////////////////////////////
                        //var rd = db.staffs.Include(r => r.contract_details).Include(r => r.contract_type).Include(r => r.Country_office).Include(r => r.functional_title).Include(r => r.staff2).Include(r => r.sub_office).Include(r => r.unit).Include(r => r.country).Include(r => r.governorate);
                        //rd = rd.Where(r => r.functional_title.functional_title_description_english == "REGIONAL DIRECTOR");
                        //var rdmail = rd.First().staff_email;

                        //var drd = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                        //drd = drd.Where(s => s.functional_title.functional_title_description_english == "Deputy Regional Director");
                        //var drdmail = drd.First().staff_email;

                        var userUnit = staffs.First().unit_id;
                        var unithou = db.units.Include(d => d.staffs);
                        unithou = unithou.Where(u => u.unitid == userUnit);
                        var houid = unithou.First().HOU_ID;

                        var staffshou = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                        staffshou = staffshou.Where(h => h.staffid == houid);
                        var hou = staffshou.First().staff_email;
                        //var houname = staffshou.First().staff_first_name + " " + staffshou.First().staff_last_name;

                        var cc = supers.First().staff_email;
                        var supername = supers.First().staff_first_name + " " + supers.First().staff_last_name;
                        var superemail = supers.First().staff_email;

                        //
                        //
                        //hou email
                        if (onbehalfID == houid)
                        {
                            var canbedone = "";
                            if (missionAuthorization11.canbedone == true)
                            {
                                canbedone = "Yes";
                            }
                            else
                            {
                                canbedone = "No";
                            }

                            var CleredBySupervisor = "";
                            if (missionAuthorization11.canbedone == true)
                            {
                                CleredBySupervisor = "Yes";
                            }
                            else
                            {
                                CleredBySupervisor = "No";
                            }

                            var BSITF2 = "";
                            if (missionAuthorization11.BSITF == true)
                            {
                                BSITF2 = "Yes";
                            }
                            else
                            {
                                BSITF2 = "No";
                            }
                            ////////  
                            var ASITF2 = "";
                            if (missionAuthorization11.ASITF == true)
                            {
                                ASITF2 = "Yes";
                            }
                            else
                            {
                                ASITF2 = "No";
                            }
                            ////////
                            var SSAFE2 = "";
                            if (missionAuthorization11.SSAFE == true)
                            {
                                SSAFE2 = "Yes";
                            }
                            else
                            {
                                SSAFE2 = "No";
                            }
                            var Roming = "";
                            if (missionAuthorization11.StaffSignature == "True")
                            {
                                Roming = "Yes";
                            }
                            else
                            {
                                Roming = "No";
                            }
                            var odcitcc = "";
                            if (Roming == "Yes")
                            {
                                odcitcc = "cairo.itservicedesk@wfp.org";
                            }
                            else
                            {
                                odcitcc = "ahmed.badr@wfp.org";
                            }
                            //////////////////////////////


                            var supersforhou = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                            supersforhou = supersforhou.Where(d => d.staffid == staffsupervisor);
                            var superforhounitsname = supersforhou.First().staff_first_name + " " + staffshou.First().staff_last_name;
                            var houmailUPDATE = supersforhou.First().staff_email;

                            if (houmailUPDATE == "muhannad.hadi@wfp.org" || houmailUPDATE == "carlo.scaramella@wfp.org")
                            {
                                houmailUPDATE = "rbc.management@wfp.org"; //rbc.management@wfp.org

                            }
                            /////////////////////////

                            MailMessage mailhou = new MailMessage();
                            mailhou.To.Add(onbehalfemail);
                            mailhou.Bcc.Add("ahmed.badr@wfp.org");

                            //mailhou.CC.Add("rbc.travel@wfp.org"); //travel unit
                            //mailhou.CC.Add("mai.kenawi@wfp.org"); //travel unit
                            //mailhou.CC.Add("amal.mohamed@wfp.org"); //travel unit
                            //mailhou.CC.Add("ayten.el-sheikh@wfp.org"); //travel unit
                            //mailhou.CC.Add("essraa.soliman@wfp.org"); //travel unit
                            //mailhou.CC.Add("rbc.management@wfp.org"); //RD/DRD Office
                            mailhou.Bcc.Add(odcitcc); //travel unit
                            mailhou.From = new MailAddress("RBC-SMS-SYSTEM@wfp.org");
                            mailhou.Subject = "Mission Security Approved";

                            string Bodyhou = "<table><tr><td colspan='2'>" + "<hr/>New Mission<hr/></td></tr><tr><td><font color='blue'>Request : </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorization11.MissionID + "</td></tr>"
                                + "<tr><td>" + "<font color='blue'>From :</font></td><td>" + staffs.First().staff_first_name + " " + staffs.First().staff_last_name + "</td></tr>"
                                + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                                + "<tr><td><font color='blue'>From :</font></td><td>" + missionAuthorization11.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                                + "<tr><td><font color='blue'>To :</font></td><td>" + missionAuthorization11.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                                + "<tr><td><font color='blue'>Funding :</font></td><td>" + missionAuthorization11.funding + "</td></tr>"
                                + "<tr><td><font color='blue'>Objective :</font></td><td>" + missionAuthorization11.MissionObjective + "</td></tr>"
                                + "<tr><td><font color='blue'>Mission Itinerary :</font></td><td>" + missionAuthorization11.MissionItinerary + "</td></tr>"
                                + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video ? :</font></td><td>" + canbedone + "</td></tr>"
                                + "<tr><td><font color='blue'>Cleared By Supervisor :</font></td><td>" + CleredBySupervisor + "</td></tr>"
                                + "<tr><td><font color='blue'>International Roaming :</font></td><td>" + Roming + "</td></tr>"
                                + "<tr><td><font color='blue'>HOU Comment :</font></td><td>" + missionAuthorization11.Comment1 + "</td></tr>"
                                + "<tr><td><font color='blue'>Security Comment :</font></td><td>" + missionAuthorization11.Comment2 + "</td></tr>"
                                + "<tr><td><font color='blue'>	BSITF :</font></td><td>" + BSITF2 + "</td></tr>"
                                + "<tr><td><font color='blue'> ASITF :</font></td><td>" + ASITF2 + "</td></tr>"
                                + "<tr><td><font color='blue'> SSAFE :</font></td><td>" + SSAFE2 + "</td></tr>"
                                + "<tr><td><font color='blue'> DSS Approval:</font></td><td><a href='http://10.11.135.254:8080/RBC-SMS/UploadedFiles/" + missionAuthorization11.non6 + "'>" + missionAuthorization11.non6 + "</a></td></tr></table>";

                            //string Body = "Request : Mission &nbsp;  &nbsp;  &nbsp; &nbsp;  &nbsp; &nbsp; &nbsp; &nbsp;  Mission ID : " + missionAuthorization.MissionID + " <br> " + " From &nbsp; &nbsp; &nbsp;:" + staffs.First().staff_first_name + "&nbsp;" + staffs.First().staff_last_name + "<br><br>" + " Mission Information" + "<br><br>" + " From :" + missionAuthorization.FromDate + "<br>To :" + missionAuthorization.ToDate +
                            //    "<br>Funding :" + missionAuthorization.funding + "<br>Objective :" + missionAuthorization.MissionObjective + "<br> Mission Itinerary :" + missionAuthorization.MissionItinerary + "<br> Can This meeting Be Done Over Voice/Video ? :" + missionAuthorization.canbedone  + " <br><br> <a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor=" + true + "&FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + " &to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Acept</a>&nbsp;&nbsp;&nbsp;<a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor= " + false + "& FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + "&to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Reject</a> ";

                            mailhou.Body = Bodyhou;
                            mailhou.IsBodyHtml = true;
                            SmtpClient smtphou = new SmtpClient();
                            smtphou.Host = "smtprelay.global.wfp.org";
                            smtphou.Port = 25;
                            smtphou.UseDefaultCredentials = true;
                            //smtp.Credentials = new System.Net.NetworkCredential
                            //("ahmed.badr", "Survivor2323");// Enter seders User name and password  
                            //smtp.EnableSsl = true;
                            smtphou.Send(mailhou);

                            ///////////////////////////// Finance/Admin Unit Approval //////////////////////

                            MailMessage mailhou1 = new MailMessage();
                            mailhou1.To.Add("ahmed.badr@wfp.org");
                            mailhou1.Bcc.Add("ahmed.badr@wfp.org");

                            //mailhou.CC.Add("rbc.travel@wfp.org"); //travel unit
                            //mailhou.CC.Add("mai.kenawi@wfp.org"); //travel unit
                            //mailhou.CC.Add("amal.mohamed@wfp.org"); //travel unit
                            //mailhou.CC.Add("ayten.el-sheikh@wfp.org"); //travel unit
                            //mailhou.CC.Add("essraa.soliman@wfp.org"); //travel unit
                            //mailhou.CC.Add("rbc.management@wfp.org"); //RD/DRD Office
                            mailhou1.Bcc.Add(odcitcc); //travel unit
                            mailhou1.From = new MailAddress("RBC-SMS-SYSTEM@wfp.org");
                            mailhou1.Subject = "For Finance Clearance";

                            string Bodyhou1 = "<table><tr><td colspan='2'>" + "<hr/>New Mission<hr/></td></tr><tr><td><font color='blue'>Request : </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorization11.MissionID + "</td></tr>"
                                + "<tr><td>" + "<font color='blue'>From :</font></td><td>" + staffs.First().staff_first_name + " " + staffs.First().staff_last_name + "</td></tr>"
                                + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                                + "<tr><td><font color='blue'>From :</font></td><td>" + missionAuthorization11.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                                + "<tr><td><font color='blue'>To :</font></td><td>" + missionAuthorization11.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                                + "<tr><td><font color='blue'>Funding :</font></td><td>" + missionAuthorization11.funding + "</td></tr>"
                                + "<tr><td><font color='blue'>Objective :</font></td><td>" + missionAuthorization11.MissionObjective + "</td></tr>"
                                + "<tr><td><font color='blue'>Mission Itinerary :</font></td><td>" + missionAuthorization11.MissionItinerary + "</td></tr>"
                                + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video ? :</font></td><td>" + canbedone + "</td></tr>"
                                + "<tr><td><font color='blue'>Cleared By Supervisor :</font></td><td>" + CleredBySupervisor + "</td></tr>"
                                + "<tr><td><font color='blue'>International Roaming :</font></td><td>" + Roming + "</td></tr>"
                                + "<tr><td><font color='blue'>HOU Comment :</font></td><td>" + missionAuthorization11.Comment1 + "</td></tr>"
                                + "<tr><td><font color='blue'>Security Comment :</font></td><td>" + missionAuthorization11.Comment2 + "</td></tr>"
                                + "<tr><td><font color='blue'>	BSITF :</font></td><td>" + BSITF2 + "</td></tr>"
                                + "<tr><td><font color='blue'> ASITF :</font></td><td>" + ASITF2 + "</td></tr>"
                                + "<tr><td><font color='blue'> SSAFE :</font></td><td>" + SSAFE2 + "</td></tr>"
                                + "<tr><td><font color='blue'> DSS Approval:</font></td><td><a href='http://10.11.135.254:8080/RBC-SMS/UploadedFiles/" + missionAuthorization11.non6 + "'>" + missionAuthorization11.non6
                            + "<tr><td><a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorization1/Approval2FinAdmin?MissionID2="
                            + missionAuthorization11.MissionID
                            + "&RDORDRDSignature2=Yes&FromDate2=" + missionAuthorization11.FromDate
                            + "&ToDate2=" + missionAuthorization11.ToDate
                            + "&to2=" + non10 + "&staffmail2=" + staffs.First().staff_email
                            + "'>Cleared</a></td><td><a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorization1/Approval2FinAdmin?MissionID2=" + missionAuthorization11.MissionID
                            + "&RDORDRDSignature2=NO & FromDate2=" + missionAuthorization11.FromDate
                            + "&ToDate2=" + missionAuthorization11.ToDate
                            + "&to2=" + non10
                            + "&staffmail2=" + staffs.First().staff_email + "'>Not Cleared</a>" + "</a></td></tr></table>";

                            //string Body = "Request : Mission &nbsp;  &nbsp;  &nbsp; &nbsp;  &nbsp; &nbsp; &nbsp; &nbsp;  Mission ID : " + missionAuthorization.MissionID + " <br> " + " From &nbsp; &nbsp; &nbsp;:" + staffs.First().staff_first_name + "&nbsp;" + staffs.First().staff_last_name + "<br><br>" + " Mission Information" + "<br><br>" + " From :" + missionAuthorization.FromDate + "<br>To :" + missionAuthorization.ToDate +
                            //    "<br>Funding :" + missionAuthorization.funding + "<br>Objective :" + missionAuthorization.MissionObjective + "<br> Mission Itinerary :" + missionAuthorization.MissionItinerary + "<br> Can This meeting Be Done Over Voice/Video ? :" + missionAuthorization.canbedone  + " <br><br> <a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor=" + true + "&FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + " &to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Acept</a>&nbsp;&nbsp;&nbsp;<a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor= " + false + "& FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + "&to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Reject</a> ";

                            mailhou1.Body = Bodyhou1;
                            mailhou1.IsBodyHtml = true;
                            SmtpClient smtphou1 = new SmtpClient();
                            smtphou1.Host = "smtprelay.global.wfp.org";
                            smtphou1.Port = 25;
                            smtphou1.UseDefaultCredentials = true;
                            //smtp.Credentials = new System.Net.NetworkCredential
                            //("ahmed.badr", "Survivor2323");// Enter seders User name and password  
                            //smtp.EnableSsl = true;
                            smtphou1.Send(mailhou1);

                            ///////////////////////////// Finance/Admin Unit Approval  end //////////////////////
                            return RedirectToAction("Index");
                        }

                        else
                        {
                            var canbedone = "";
                            if (missionAuthorization11.canbedone == true)
                            {
                                canbedone = "Yes";
                            }
                            else
                            {
                                canbedone = "No";
                            }

                            var CleredBySupervisor = "";
                            if (missionAuthorization11.canbedone == true)
                            {
                                CleredBySupervisor = "Yes";
                            }
                            else
                            {
                                CleredBySupervisor = "No";
                            }

                            var BSITF2 = "";
                            if (missionAuthorization11.BSITF == true)
                            {
                                BSITF2 = "Yes";
                            }
                            else
                            {
                                BSITF2 = "No";
                            }
                            ////////  
                            var ASITF2 = "";
                            if (missionAuthorization11.ASITF == true)
                            {
                                ASITF2 = "Yes";
                            }
                            else
                            {
                                ASITF2 = "No";
                            }
                            ////////
                            var SSAFE2 = "";
                            if (missionAuthorization11.SSAFE == true)
                            {
                                SSAFE2 = "Yes";
                            }
                            else
                            {
                                SSAFE2 = "No";
                            }
                            var odcitcc = "";
                            if (missionAuthorization11.StaffSignature == "True")
                            {
                                odcitcc = "cairo.itservicedesk@wfp.org";
                            }
                            else
                            {
                                odcitcc = "ahmed.badr@wfp.org";
                            }
                            var Roming = "";
                            if (missionAuthorization11.StaffSignature == "True")
                            {
                                Roming = "Yes";
                            }
                            else
                            {
                                Roming = "No";
                            }
                            //////////////////////////////


                            var supersforhou = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                            supersforhou = supersforhou.Where(d => d.staffid == staffsupervisor);
                            var superforhounitsname = supersforhou.First().staff_first_name + " " + staffshou.First().staff_last_name;
                            var houmailUPDATE = supersforhou.First().staff_email;

                            if (houmailUPDATE == "muhannad.hadi@wfp.org" || houmailUPDATE == "carlo.scaramella@wfp.org")
                            {
                                houmailUPDATE = "rbc.management@wfp.org"; //rbc.management@wfp.org

                            }
                            /////////////////////////

                            MailMessage mailhou = new MailMessage();
                            mailhou.To.Add(onbehalfemail); //staffmail
                            mailhou.Bcc.Add("ahmed.badr@wfp.org");
                            mailhou.CC.Add(onbehafsuper);
                            mailhou.Bcc.Add(odcitcc); //travel unit
                            //mailhou.CC.Add("rbc.travel@wfp.org"); //travel unit
                            //mailhou.CC.Add("mai.kenawi@wfp.org"); //travel unit
                            //mailhou.CC.Add("amal.mohamed@wfp.org"); //travel unit
                            //mailhou.CC.Add("ayten.el-sheikh@wfp.org"); //travel unit
                            //mailhou.CC.Add("essraa.soliman@wfp.org"); //travel unit
                            //mailhou.CC.Add("rbc.management@wfp.org"); //RD/DRD Office
                            mailhou.From = new MailAddress("RBC-SMS-SYSTEM@wfp.org");
                            mailhou.Subject = "Mission Security Approved";

                            string Bodyhou = "<table><tr><td colspan='2'>" + "<hr/>New Mission<hr/></td></tr><tr><td><font color='blue'>Request : </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorization11.MissionID + "</td></tr>"
                                + "<tr><td>" + "<font color='blue'>From :</font></td><td>" + staffs.First().staff_first_name + " " + staffs.First().staff_last_name + "</td></tr>"
                                + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                                + "<tr><td><font color='blue'>From :</font></td><td>" + missionAuthorization11.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                                + "<tr><td><font color='blue'>To :</font></td><td>" + missionAuthorization11.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                                + "<tr><td><font color='blue'>Funding :</font></td><td>" + missionAuthorization11.funding + "</td></tr>"
                                + "<tr><td><font color='blue'>Objective :</font></td><td>" + missionAuthorization11.MissionObjective + "</td></tr>"
                                + "<tr><td><font color='blue'>Mission Itinerary :</font></td><td>" + missionAuthorization11.MissionItinerary + "</td></tr>"
                                + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video ? :</font></td><td>" + canbedone + "</td></tr>"
                                + "<tr><td><font color='blue'>Cleared By Supervisor :</font></td><td>" + CleredBySupervisor + "</td></tr>"
                                + "<tr><td><font color='blue'>International Roaming :</font></td><td>" + Roming + "</td></tr>"
                                + "<tr><td><font color='blue'>HOU Comment :</font></td><td>" + missionAuthorization11.Comment1 + "</td></tr>"
                                + "<tr><td><font color='blue'>Security Comment :</font></td><td>" + missionAuthorization11.Comment2 + "</td></tr>"
                                + "<tr><td><font color='blue'>	BSITF :</font></td><td>" + BSITF2 + "</td></tr>"
                                + "<tr><td><font color='blue'> ASITF :</font></td><td>" + ASITF2 + "</td></tr>"
                                + "<tr><td><font color='blue'> SSAFE :</font></td><td>" + SSAFE2 + "</td></tr>"
                                + "<tr><td><font color='blue'> DSS Approval:</font></td><td><a href='http://10.11.135.254:8080/RBC-SMS/UploadedFiles/" + missionAuthorization11.non6 + "'>" + missionAuthorization11.non6 + "</a></td></tr></table>";

                            //string Body = "Request : Mission &nbsp;  &nbsp;  &nbsp; &nbsp;  &nbsp; &nbsp; &nbsp; &nbsp;  Mission ID : " + missionAuthorization.MissionID + " <br> " + " From &nbsp; &nbsp; &nbsp;:" + staffs.First().staff_first_name + "&nbsp;" + staffs.First().staff_last_name + "<br><br>" + " Mission Information" + "<br><br>" + " From :" + missionAuthorization.FromDate + "<br>To :" + missionAuthorization.ToDate +
                            //    "<br>Funding :" + missionAuthorization.funding + "<br>Objective :" + missionAuthorization.MissionObjective + "<br> Mission Itinerary :" + missionAuthorization.MissionItinerary + "<br> Can This meeting Be Done Over Voice/Video ? :" + missionAuthorization.canbedone  + " <br><br> <a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor=" + true + "&FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + " &to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Acept</a>&nbsp;&nbsp;&nbsp;<a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor= " + false + "& FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + "&to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Reject</a> ";

                            mailhou.Body = Bodyhou;
                            mailhou.IsBodyHtml = true;
                            SmtpClient smtphou = new SmtpClient();
                            smtphou.Host = "smtprelay.global.wfp.org";
                            smtphou.Port = 25;
                            smtphou.UseDefaultCredentials = true;
                            //smtp.Credentials = new System.Net.NetworkCredential
                            //("ahmed.badr", "Survivor2323");// Enter seders User name and password  
                            //smtp.EnableSsl = true;
                            smtphou.Send(mailhou);


                            ///////////////////////////// Finance/Admin Unit Approval //////////////////////

                            MailMessage mailhou1 = new MailMessage();
                            mailhou1.To.Add("ahmed.badr@wfp.org");
                            mailhou1.Bcc.Add("ahmed.badr@wfp.org");

                            //mailhou.CC.Add("rbc.travel@wfp.org"); //travel unit
                            //mailhou.CC.Add("mai.kenawi@wfp.org"); //travel unit
                            //mailhou.CC.Add("amal.mohamed@wfp.org"); //travel unit
                            //mailhou.CC.Add("ayten.el-sheikh@wfp.org"); //travel unit
                            //mailhou.CC.Add("essraa.soliman@wfp.org"); //travel unit
                            //mailhou.CC.Add("rbc.management@wfp.org"); //RD/DRD Office
                            mailhou1.Bcc.Add(odcitcc); //travel unit
                            mailhou1.From = new MailAddress("RBC-SMS-SYSTEM@wfp.org");
                            mailhou1.Subject = "For Finance Clearance";

                            string Bodyhou1 = "<table><tr><td colspan='2'>" + "<hr/>New Mission<hr/></td></tr><tr><td><font color='blue'>Request : </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorization11.MissionID + "</td></tr>"
                                + "<tr><td>" + "<font color='blue'>From :</font></td><td>" + staffs.First().staff_first_name + " " + staffs.First().staff_last_name + "</td></tr>"
                                + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                                + "<tr><td><font color='blue'>From :</font></td><td>" + missionAuthorization11.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                                + "<tr><td><font color='blue'>To :</font></td><td>" + missionAuthorization11.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                                + "<tr><td><font color='blue'>Funding :</font></td><td>" + missionAuthorization11.funding + "</td></tr>"
                                + "<tr><td><font color='blue'>Objective :</font></td><td>" + missionAuthorization11.MissionObjective + "</td></tr>"
                                + "<tr><td><font color='blue'>Mission Itinerary :</font></td><td>" + missionAuthorization11.MissionItinerary + "</td></tr>"
                                + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video ? :</font></td><td>" + canbedone + "</td></tr>"
                                + "<tr><td><font color='blue'>Cleared By Supervisor :</font></td><td>" + CleredBySupervisor + "</td></tr>"
                                + "<tr><td><font color='blue'>International Roaming :</font></td><td>" + Roming + "</td></tr>"
                                + "<tr><td><font color='blue'>HOU Comment :</font></td><td>" + missionAuthorization11.Comment1 + "</td></tr>"
                                + "<tr><td><font color='blue'>Security Comment :</font></td><td>" + missionAuthorization11.Comment2 + "</td></tr>"
                                + "<tr><td><font color='blue'>	BSITF :</font></td><td>" + BSITF2 + "</td></tr>"
                                + "<tr><td><font color='blue'> ASITF :</font></td><td>" + ASITF2 + "</td></tr>"
                                + "<tr><td><font color='blue'> SSAFE :</font></td><td>" + SSAFE2 + "</td></tr>"
                                + "<tr><td><font color='blue'> DSS Approval:</font></td><td><a href='http://10.11.135.254:8080/RBC-SMS/UploadedFiles/" + missionAuthorization11.non6 + "'>" + missionAuthorization11.non6

                           + "<tr><td><a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorization1/Approval2FinAdmin?MissionID2="
                            + missionAuthorization11.MissionID
                            + "&RDORDRDSignature2=Yes&FromDate2=" + missionAuthorization11.FromDate
                            + "&ToDate2=" + missionAuthorization11.ToDate
                            + "&to2=" + non10 + "&staffmail2=" + staffs.First().staff_email
                            + "'>Cleared</a></td><td><a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorization1/Approval2FinAdmin?MissionID2=" + missionAuthorization11.MissionID
                            + "&RDORDRDSignature2=NO & FromDate2=" + missionAuthorization11.FromDate
                            + "&ToDate2=" + missionAuthorization11.ToDate
                            + "&to2=" + non10
                            + "&staffmail2=" + staffs.First().staff_email + "'>Not Cleared</a>" + "</a></td></tr></table>";


                            //string Body = "Request : Mission &nbsp;  &nbsp;  &nbsp; &nbsp;  &nbsp; &nbsp; &nbsp; &nbsp;  Mission ID : " + missionAuthorization.MissionID + " <br> " + " From &nbsp; &nbsp; &nbsp;:" + staffs.First().staff_first_name + "&nbsp;" + staffs.First().staff_last_name + "<br><br>" + " Mission Information" + "<br><br>" + " From :" + missionAuthorization.FromDate + "<br>To :" + missionAuthorization.ToDate +
                            //    "<br>Funding :" + missionAuthorization.funding + "<br>Objective :" + missionAuthorization.MissionObjective + "<br> Mission Itinerary :" + missionAuthorization.MissionItinerary + "<br> Can This meeting Be Done Over Voice/Video ? :" + missionAuthorization.canbedone  + " <br><br> <a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor=" + true + "&FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + " &to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Acept</a>&nbsp;&nbsp;&nbsp;<a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor= " + false + "& FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + "&to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Reject</a> ";

                            mailhou1.Body = Bodyhou1;
                            mailhou1.IsBodyHtml = true;
                            SmtpClient smtphou1 = new SmtpClient();
                            smtphou1.Host = "smtprelay.global.wfp.org";
                            smtphou1.Port = 25;
                            smtphou1.UseDefaultCredentials = true;
                            //smtp.Credentials = new System.Net.NetworkCredential
                            //("ahmed.badr", "Survivor2323");// Enter seders User name and password  
                            //smtp.EnableSsl = true;
                            smtphou1.Send(mailhou1);

                            ///////////////////////////// Finance/Admin Unit Approval end //////////////////////

                            return RedirectToAction("Index");


                        }
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
                    return View();
                }
                else
                {
                    //var missionAuthorizationn = db.MissionAuthorizations.Single(u => u.MissionID == Mid);
                    //db.MissionAuthorizations.Remove(missionAuthorizationn);
                    //db.SaveChanges();
                    var missionAuthorization12 = db.MissionAuthorization1.Single(u => u.MissionID == MissionID);
                    missionAuthorization12.Signature2 = false;
                    missionAuthorization12.Signature2Date = DateTime.Now;
                    missionAuthorization12.Comment2 = Comment2;
                    db.SaveChanges();



                    var staffss = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                    staffss = staffss.Where(s => s.staffid == onbehalfID);
                    var too = staffss.First().staff_email;
                    var staffsupervisor = staffss.First().staff_supervisorid;
                    var supers = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                    supers = supers.Where(d => d.staffid == staffsupervisor);
                    var from = supers.First().staff_email;

                    //var rd = db.staffs.Include(r => r.contract_details).Include(r => r.contract_type).Include(r => r.Country_office).Include(r => r.functional_title).Include(r => r.staff2).Include(r => r.sub_office).Include(r => r.unit).Include(r => r.country).Include(r => r.governorate);
                    //rd = rd.Where(r => r.functional_title.functional_title_description_english == "REGIONAL DIRECTOR");
                    //var rdmail = rd.First().staff_email;

                    //var drd = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                    //drd = drd.Where(s => s.functional_title.functional_title_description_english == "Deputy Regional Director");
                    //var drdmail = drd.First().staff_email;

                    //////////////////////////////////////////////////////////////////////////////////////


                    var Sonbehafemail = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                    Sonbehafemail = Sonbehafemail.Where(d => d.staffid == onbehalfID);
                    var onbehalfemail = Sonbehafemail.First().staff_email;


                    var supersonbehalf = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                    supersonbehalf = supersonbehalf.Where(d => d.staffid == onbehalfSuperID);
                    var onbehafsuperemail = supersonbehalf.First().staff_email;

                    /////////////////////////////////////////////////////////////////////////////////////
                    //new
                    var userUnit = staffss.First().unit_id;
                    var unithou = db.units.Include(d => d.staffs);
                    unithou = unithou.Where(u => u.unitid == userUnit);
                    var houid = unithou.First().HOU_ID;

                    var staffshou = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                    staffshou = staffshou.Where(h => h.staffid == houid);
                    var hou = staffshou.First().staff_email;
                    //var houname = staffshou.First().staff_first_name + " " + staffshou.First().staff_last_name;

                    var cc = supers.First().staff_email;
                    var supername = supers.First().staff_first_name + " " + supers.First().staff_last_name;

                    //
                    //
                    //hou email
                    if (onbehalfID == houid)
                    {
                        var canbedone = "";
                        if (missionAuthorization12.canbedone == true)
                        {
                            canbedone = "Yes";
                        }
                        else
                        {
                            canbedone = "No";
                        }

                        var CleredBySupervisor = "";
                        if (missionAuthorization12.canbedone == true)
                        {
                            CleredBySupervisor = "Yes";
                        }
                        else
                        {
                            CleredBySupervisor = "No";
                        }

                        var BSITF2 = "";
                        if (missionAuthorization12.BSITF == true)
                        {
                            BSITF2 = "Yes";
                        }
                        else
                        {
                            BSITF2 = "No";
                        }
                        ////////  
                        var ASITF2 = "";
                        if (missionAuthorization12.ASITF == true)
                        {
                            ASITF2 = "Yes";
                        }
                        else
                        {
                            ASITF2 = "No";
                        }
                        ////////
                        var SSAFE2 = "";
                        if (missionAuthorization12.SSAFE == true)
                        {
                            SSAFE2 = "Yes";
                        }
                        else
                        {
                            SSAFE2 = "No";
                        }

                        //////////////////////////////


                        var supersforhou = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                        supersforhou = supersforhou.Where(d => d.staffid == staffsupervisor);
                        var superforhounitsname = supersforhou.First().staff_first_name + " " + staffshou.First().staff_last_name;
                        var houmailUPDATE = supersforhou.First().staff_email;

                        if (houmailUPDATE == "muhannad.hadi@wfp.org" || houmailUPDATE == "carlo.scaramella@wfp.org")
                        {
                            houmailUPDATE = "rbc.management@wfp.org"; //rbc.management@wfp.org

                        }
                        /////////////////////////

                        MailMessage mailhou = new MailMessage();
                        mailhou.To.Add(onbehalfemail);
                        mailhou.Bcc.Add("ahmed.badr@wfp.org");
                        //mail2.CC.Add(cc);
                        mailhou.From = new MailAddress("RBC-SMS-SYSTEM@wfp.org");
                        mailhou.Subject = "Mission Security Rejected";

                        string Bodyhou = "<table><tr><td colspan='2'>" + "<hr/>New Mission<hr/></td></tr><tr><td><font color='blue'>Request : </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorization12.MissionID + "</td></tr>"
                            + "<tr><td>" + "<font color='blue'>From :</font></td><td>" + staffss.First().staff_first_name + " " + staffss.First().staff_last_name + "</td></tr>"
                            + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                            + "<tr><td><font color='blue'>From :</font></td><td>" + missionAuthorization12.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                            + "<tr><td><font color='blue'>To :</font></td><td>" + missionAuthorization12.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                            + "<tr><td><font color='blue'>Funding :</font></td><td>" + missionAuthorization12.funding + "</td></tr>"
                            + "<tr><td><font color='blue'>Objective :</font></td><td>" + missionAuthorization12.MissionObjective + "</td></tr>"
                            + "<tr><td><font color='blue'>Mission Itinerary :</font></td><td>" + missionAuthorization12.MissionItinerary + "</td></tr>"
                            + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video ? :</font></td><td>" + canbedone + "</td></tr>"
                            + "<tr><td><font color='blue'>Cleared By Supervisor :</font></td><td>" + CleredBySupervisor + "</td></tr>"
                             + "<tr><td><font color='blue'>HOU Comment :</font></td><td>" + missionAuthorization12.Comment1 + "</td></tr>"
                             + "<tr><td><font color='blue'>Security Comment :</font></td><td>" + missionAuthorization12.Comment2 + "</td></tr>"
                            + "<tr><td><font color='blue'>	BSITF :</font></td><td>" + BSITF2 + "</td></tr>"
                            + "<tr><td><font color='blue'> ASITF :</font></td><td>" + ASITF2 + "</td></tr>"
                            + "<tr><td><font color='blue'> SSAFE :</font></td><td>" + SSAFE2 + "</td></tr>"
                            + "<tr><td><font color='blue'> DSS Approval:</font></td><td><a href='http://10.11.135.254:8080/RBC-SMS/UploadedFiles/" + missionAuthorization12.non6 + "'>" + missionAuthorization12.non6 + "</a></td></tr></table>";

                        //string Body = "Request : Mission &nbsp;  &nbsp;  &nbsp; &nbsp;  &nbsp; &nbsp; &nbsp; &nbsp;  Mission ID : " + missionAuthorization.MissionID + " <br> " + " From &nbsp; &nbsp; &nbsp;:" + staffs.First().staff_first_name + "&nbsp;" + staffs.First().staff_last_name + "<br><br>" + " Mission Information" + "<br><br>" + " From :" + missionAuthorization.FromDate + "<br>To :" + missionAuthorization.ToDate +
                        //    "<br>Funding :" + missionAuthorization.funding + "<br>Objective :" + missionAuthorization.MissionObjective + "<br> Mission Itinerary :" + missionAuthorization.MissionItinerary + "<br> Can This meeting Be Done Over Voice/Video ? :" + missionAuthorization.canbedone  + " <br><br> <a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor=" + true + "&FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + " &to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Acept</a>&nbsp;&nbsp;&nbsp;<a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor= " + false + "& FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + "&to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Reject</a> ";

                        mailhou.Body = Bodyhou;
                        mailhou.IsBodyHtml = true;
                        SmtpClient smtphou = new SmtpClient();
                        smtphou.Host = "smtprelay.global.wfp.org";
                        smtphou.Port = 25;
                        smtphou.UseDefaultCredentials = true;
                        //smtp.Credentials = new System.Net.NetworkCredential
                        //("ahmed.badr", "Survivor2323");// Enter seders User name and password  
                        //smtp.EnableSsl = true;
                        smtphou.Send(mailhou);
                        return RedirectToAction("Index");
                    }

                    else
                    {
                        var canbedone = "";
                        if (missionAuthorization12.canbedone == true)
                        {
                            canbedone = "Yes";
                        }
                        else
                        {
                            canbedone = "No";
                        }

                        var CleredBySupervisor = "";
                        if (missionAuthorization12.canbedone == true)
                        {
                            CleredBySupervisor = "Yes";
                        }
                        else
                        {
                            CleredBySupervisor = "No";
                        }

                        var BSITF2 = "";
                        if (missionAuthorization12.BSITF == true)
                        {
                            BSITF2 = "Yes";
                        }
                        else
                        {
                            BSITF2 = "No";
                        }
                        ////////  
                        var ASITF2 = "";
                        if (missionAuthorization12.ASITF == true)
                        {
                            ASITF2 = "Yes";
                        }
                        else
                        {
                            ASITF2 = "No";
                        }
                        ////////
                        var SSAFE2 = "";
                        if (missionAuthorization12.SSAFE == true)
                        {
                            SSAFE2 = "Yes";
                        }
                        else
                        {
                            SSAFE2 = "No";
                        }

                        //////////////////////////////


                        var supersforhou = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                        supersforhou = supersforhou.Where(d => d.staffid == staffsupervisor);
                        var superforhounitsname = supersforhou.First().staff_first_name + " " + staffshou.First().staff_last_name;
                        var houmailUPDATE = supersforhou.First().staff_email;

                        if (houmailUPDATE == "muhannad.hadi@wfp.org" || houmailUPDATE == "carlo.scaramella@wfp.org")
                        {
                            houmailUPDATE = "rbc.management@wfp.org"; //rbc.management@wfp.org

                        }
                        /////////////////////////

                        MailMessage mailhou = new MailMessage();
                        mailhou.To.Add(onbehalfemail);
                        mailhou.Bcc.Add("ahmed.badr@wfp.org");
                        mailhou.CC.Add(onbehafsuperemail);
                        mailhou.From = new MailAddress(hou);
                        mailhou.Subject = "Mission Security Rejected";

                        string Bodyhou = "<table><tr><td colspan='2'>" + "<hr/>New Mission<hr/></td></tr><tr><td><font color='blue'>Request : </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorization12.MissionID + "</td></tr>"
                            + "<tr><td>" + "<font color='blue'>From :</font></td><td>" + staffss.First().staff_first_name + " " + staffss.First().staff_last_name + "</td></tr>"
                            + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                            + "<tr><td><font color='blue'>From :</font></td><td>" + missionAuthorization12.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                            + "<tr><td><font color='blue'>To :</font></td><td>" + missionAuthorization12.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                            + "<tr><td><font color='blue'>Funding :</font></td><td>" + missionAuthorization12.funding + "</td></tr>"
                            + "<tr><td><font color='blue'>Objective :</font></td><td>" + missionAuthorization12.MissionObjective + "</td></tr>"
                            + "<tr><td><font color='blue'>Mission Itinerary :</font></td><td>" + missionAuthorization12.MissionItinerary + "</td></tr>"
                            + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video ? :</font></td><td>" + canbedone + "</td></tr>"
                            + "<tr><td><font color='blue'>Cleared By Supervisor :</font></td><td>" + CleredBySupervisor + "</td></tr>"
                             + "<tr><td><font color='blue'>HOU Comment :</font></td><td>" + missionAuthorization12.Comment1 + "</td></tr>"
                             + "<tr><td><font color='blue'>Security Comment :</font></td><td>" + missionAuthorization12.Comment2 + "</td></tr>"
                            + "<tr><td><font color='blue'>	BSITF :</font></td><td>" + BSITF2 + "</td></tr>"
                            + "<tr><td><font color='blue'> ASITF :</font></td><td>" + ASITF2 + "</td></tr>"
                            + "<tr><td><font color='blue'> SSAFE :</font></td><td>" + SSAFE2 + "</td></tr>"
                           + "<tr><td><font color='blue'> DSS Approval:</font></td><td><a href='http://10.11.135.254:8080/RBC-SMS/UploadedFiles/" + missionAuthorization12.non6 + "'>" + missionAuthorization12.non6 + "</a></td></tr></table>";

                        //string Body = "Request : Mission &nbsp;  &nbsp;  &nbsp; &nbsp;  &nbsp; &nbsp; &nbsp; &nbsp;  Mission ID : " + missionAuthorization.MissionID + " <br> " + " From &nbsp; &nbsp; &nbsp;:" + staffs.First().staff_first_name + "&nbsp;" + staffs.First().staff_last_name + "<br><br>" + " Mission Information" + "<br><br>" + " From :" + missionAuthorization.FromDate + "<br>To :" + missionAuthorization.ToDate +
                        //    "<br>Funding :" + missionAuthorization.funding + "<br>Objective :" + missionAuthorization.MissionObjective + "<br> Mission Itinerary :" + missionAuthorization.MissionItinerary + "<br> Can This meeting Be Done Over Voice/Video ? :" + missionAuthorization.canbedone  + " <br><br> <a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor=" + true + "&FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + " &to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Acept</a>&nbsp;&nbsp;&nbsp;<a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor= " + false + "& FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + "&to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Reject</a> ";

                        mailhou.Body = Bodyhou;
                        mailhou.IsBodyHtml = true;
                        SmtpClient smtphou = new SmtpClient();
                        smtphou.Host = "smtprelay.global.wfp.org";
                        smtphou.Port = 25;
                        smtphou.UseDefaultCredentials = true;
                        //smtp.Credentials = new System.Net.NetworkCredential
                        //("ahmed.badr", "Survivor2323");// Enter seders User name and password  
                        //smtp.EnableSsl = true;
                        smtphou.Send(mailhou);
                        return RedirectToAction("Index");


                    }
                }



            }
            ///////////////////////// end on behalf 
            else
            {
                ViewBag.Mid = Request.QueryString["MissionID"];
                ViewBag.RDORDRDSignature = Request.QueryString["RDORDRDSignature"];
                ViewBag.FromDate = Request.QueryString["FromDate"];
                ViewBag.ToDate = Request.QueryString["ToDate"];
                if (RDORDRDSignature == "Yes")
                {
                    try
                    {
                        // missionAuthorization = db.MissionAuthorizations.Find(Mid);
                        //db.Entry(missionAuthorization).State = EntityState.Modified ;
                        //db.SaveChanges();

                        var missionAuthorization = db.MissionAuthorization1.Single(u => u.MissionID == MissionID);
                        missionAuthorization.Signature2 = true;
                        missionAuthorization.Signature2Date = DateTime.Now;
                        missionAuthorization.Comment2 = Comment2;
                        db.SaveChanges();



                        var staffs = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                        staffs = staffs.Where(s => s.staff_email.Contains(staff));
                        var too = staffs.First().staff_email;
                        var staffsupervisor = staffs.First().staff_supervisorid;
                        var supers = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                        supers = supers.Where(d => d.staffid == staffsupervisor);
                        var from = supers.First().staff_email;

                        //var rd = db.staffs.Include(r => r.contract_details).Include(r => r.contract_type).Include(r => r.Country_office).Include(r => r.functional_title).Include(r => r.staff2).Include(r => r.sub_office).Include(r => r.unit).Include(r => r.country).Include(r => r.governorate);
                        //rd = rd.Where(r => r.functional_title.functional_title_description_english == "REGIONAL DIRECTOR");
                        //var rdmail = rd.First().staff_email;

                        //var drd = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                        //drd = drd.Where(s => s.functional_title.functional_title_description_english == "Deputy Regional Director");
                        //var drdmail = drd.First().staff_email;

                        var userUnit = staffs.First().unit_id;
                        var unithou = db.units.Include(d => d.staffs);
                        unithou = unithou.Where(u => u.unitid == userUnit);
                        var houid = unithou.First().HOU_ID;

                        var staffshou = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                        staffshou = staffshou.Where(h => h.staffid == houid);
                        var hou = staffshou.First().staff_email;
                        //var houname = staffshou.First().staff_first_name + " " + staffshou.First().staff_last_name;

                        var cc = supers.First().staff_email;
                        var supername = supers.First().staff_first_name + " " + supers.First().staff_last_name;
                        var superemail = supers.First().staff_email;

                        //
                        //
                        //hou email
                        if (staffs.First().staffid == houid)
                        {
                            var canbedone = "";
                            if (missionAuthorization.canbedone == true)
                            {
                                canbedone = "Yes";
                            }
                            else
                            {
                                canbedone = "No";
                            }

                            var CleredBySupervisor = "";
                            if (missionAuthorization.canbedone == true)
                            {
                                CleredBySupervisor = "Yes";
                            }
                            else
                            {
                                CleredBySupervisor = "No";
                            }

                            var BSITF2 = "";
                            if (missionAuthorization.BSITF == true)
                            {
                                BSITF2 = "Yes";
                            }
                            else
                            {
                                BSITF2 = "No";
                            }
                            ////////  
                            var ASITF2 = "";
                            if (missionAuthorization.ASITF == true)
                            {
                                ASITF2 = "Yes";
                            }
                            else
                            {
                                ASITF2 = "No";
                            }
                            ////////
                            var SSAFE2 = "";
                            if (missionAuthorization.SSAFE == true)
                            {
                                SSAFE2 = "Yes";
                            }
                            else
                            {
                                SSAFE2 = "No";
                            }
                            var Roming = "";
                            if (missionAuthorization.StaffSignature == "True")
                            {
                                Roming = "Yes";
                            }
                            else
                            {
                                Roming = "No";
                            }
                            var odcitcc = "";
                            if (Roming == "Yes")
                            {
                                odcitcc = "cairo.itservicedesk@wfp.org";
                            }
                            else
                            {
                                odcitcc = "ahmed.badr@wfp.org";
                            }
                            //////////////////////////////


                            var supersforhou = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                            supersforhou = supersforhou.Where(d => d.staffid == staffsupervisor);
                            var superforhounitsname = supersforhou.First().staff_first_name + " " + staffshou.First().staff_last_name;
                            var houmailUPDATE = supersforhou.First().staff_email;

                            if (houmailUPDATE == "muhannad.hadi@wfp.org" || houmailUPDATE == "carlo.scaramella@wfp.org")
                            {
                                houmailUPDATE = "rbc.management@wfp.org"; //rbc.management@wfp.org

                            }
                            /////////////////////////

                            MailMessage mailhou = new MailMessage();
                            mailhou.To.Add(staff);
                            mailhou.Bcc.Add("ahmed.badr@wfp.org");
                            mailhou.Bcc.Add(odcitcc); //travel unit
                            //mailhou.CC.Add("rbc.travel@wfp.org"); //travel unit
                            //mailhou.CC.Add("mai.kenawi@wfp.org"); //travel unit
                            //mailhou.CC.Add("amal.mohamed@wfp.org"); //travel unit
                            //mailhou.CC.Add("ayten.el-sheikh@wfp.org"); //travel unit
                            //mailhou.CC.Add("essraa.soliman@wfp.org"); //travel unit
                            //mailhou.CC.Add("rbc.management@wfp.org"); //RD/DRD Office
                            mailhou.From = new MailAddress("RBC-SMS-SYSTEM@wfp.org");
                            mailhou.Subject = "Mission Security Approved";

                            string Bodyhou = "<table><tr><td colspan='2'>" + "<hr/>New Mission<hr/></td></tr><tr><td><font color='blue'>Request : </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorization.MissionID + "</td></tr>"
                                + "<tr><td>" + "<font color='blue'>From :</font></td><td>" + staffs.First().staff_first_name + " " + staffs.First().staff_last_name + "</td></tr>"
                                + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                                + "<tr><td><font color='blue'>From :</font></td><td>" + missionAuthorization.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                                + "<tr><td><font color='blue'>To :</font></td><td>" + missionAuthorization.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                                + "<tr><td><font color='blue'>Funding :</font></td><td>" + missionAuthorization.funding + "</td></tr>"
                                + "<tr><td><font color='blue'>Objective :</font></td><td>" + missionAuthorization.MissionObjective + "</td></tr>"
                                + "<tr><td><font color='blue'>Mission Itinerary :</font></td><td>" + missionAuthorization.MissionItinerary + "</td></tr>"
                                + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video ? :</font></td><td>" + canbedone + "</td></tr>"
                                + "<tr><td><font color='blue'>Cleared By Supervisor :</font></td><td>" + CleredBySupervisor + "</td></tr>"
                                + "<tr><td><font color='blue'>International Roaming :</font></td><td>" + Roming + "</td></tr>"
                                 + "<tr><td><font color='blue'>HOU Comment :</font></td><td>" + missionAuthorization.Comment1 + "</td></tr>"
                                 + "<tr><td><font color='blue'>Security Comment :</font></td><td>" + missionAuthorization.Comment2 + "</td></tr>"
                                + "<tr><td><font color='blue'>	BSITF :</font></td><td>" + BSITF2 + "</td></tr>"
                                + "<tr><td><font color='blue'> ASITF :</font></td><td>" + ASITF2 + "</td></tr>"
                                + "<tr><td><font color='blue'> SSAFE :</font></td><td>" + SSAFE2 + "</td></tr>"
                               + "<tr><td><font color='blue'> DSS Approval:</font></td><td><a target='_blank' href='http://10.11.135.254:8080/RBC-SMS/UploadedFiles/" + missionAuthorization.non6 + "'>" + missionAuthorization.non6 + "</a></td></tr></table>";

                            //string Body = "Request : Mission &nbsp;  &nbsp;  &nbsp; &nbsp;  &nbsp; &nbsp; &nbsp; &nbsp;  Mission ID : " + missionAuthorization.MissionID + " <br> " + " From &nbsp; &nbsp; &nbsp;:" + staffs.First().staff_first_name + "&nbsp;" + staffs.First().staff_last_name + "<br><br>" + " Mission Information" + "<br><br>" + " From :" + missionAuthorization.FromDate + "<br>To :" + missionAuthorization.ToDate +
                            //    "<br>Funding :" + missionAuthorization.funding + "<br>Objective :" + missionAuthorization.MissionObjective + "<br> Mission Itinerary :" + missionAuthorization.MissionItinerary + "<br> Can This meeting Be Done Over Voice/Video ? :" + missionAuthorization.canbedone  + " <br><br> <a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor=" + true + "&FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + " &to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Acept</a>&nbsp;&nbsp;&nbsp;<a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor= " + false + "& FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + "&to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Reject</a> ";

                            mailhou.Body = Bodyhou;
                            mailhou.IsBodyHtml = true;
                            SmtpClient smtphou = new SmtpClient();
                            smtphou.Host = "smtprelay.global.wfp.org";
                            smtphou.Port = 25;
                            smtphou.UseDefaultCredentials = true;
                            //smtp.Credentials = new System.Net.NetworkCredential
                            //("ahmed.badr", "Survivor2323");// Enter seders User name and password  
                            //smtp.EnableSsl = true;
                            smtphou.Send(mailhou);


                            ///////////////////////////// Finance/Admin Unit Approval //////////////////////

                            MailMessage mailhou1 = new MailMessage();
                            mailhou1.To.Add("ahmed.badr@wfp.org");
                            mailhou1.Bcc.Add("ahmed.badr@wfp.org");

                            //mailhou.CC.Add("rbc.travel@wfp.org"); //travel unit
                            //mailhou.CC.Add("mai.kenawi@wfp.org"); //travel unit
                            //mailhou.CC.Add("amal.mohamed@wfp.org"); //travel unit
                            //mailhou.CC.Add("ayten.el-sheikh@wfp.org"); //travel unit
                            //mailhou.CC.Add("essraa.soliman@wfp.org"); //travel unit
                            //mailhou.CC.Add("rbc.management@wfp.org"); //RD/DRD Office
                            mailhou1.Bcc.Add(odcitcc); //travel unit
                            mailhou1.From = new MailAddress("RBC-SMS-SYSTEM@wfp.org");
                            mailhou1.Subject = "For Finance Clearance";

                            string Bodyhou1 = "<table><tr><td colspan='2'>" + "<hr/>New Mission<hr/></td></tr><tr><td><font color='blue'>Request : </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorization.MissionID + "</td></tr>"
                                + "<tr><td>" + "<font color='blue'>From :</font></td><td>" + staffs.First().staff_first_name + " " + staffs.First().staff_last_name + "</td></tr>"
                                + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                                + "<tr><td><font color='blue'>From :</font></td><td>" + missionAuthorization.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                                + "<tr><td><font color='blue'>To :</font></td><td>" + missionAuthorization.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                                + "<tr><td><font color='blue'>Funding :</font></td><td>" + missionAuthorization.funding + "</td></tr>"
                                + "<tr><td><font color='blue'>Objective :</font></td><td>" + missionAuthorization.MissionObjective + "</td></tr>"
                                + "<tr><td><font color='blue'>Mission Itinerary :</font></td><td>" + missionAuthorization.MissionItinerary + "</td></tr>"
                                + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video ? :</font></td><td>" + canbedone + "</td></tr>"
                                + "<tr><td><font color='blue'>Cleared By Supervisor :</font></td><td>" + CleredBySupervisor + "</td></tr>"
                                + "<tr><td><font color='blue'>International Roaming :</font></td><td>" + Roming + "</td></tr>"
                                + "<tr><td><font color='blue'>HOU Comment :</font></td><td>" + missionAuthorization.Comment1 + "</td></tr>"
                                + "<tr><td><font color='blue'>Security Comment :</font></td><td>" + missionAuthorization.Comment2 + "</td></tr>"
                                + "<tr><td><font color='blue'>	BSITF :</font></td><td>" + BSITF2 + "</td></tr>"
                                + "<tr><td><font color='blue'> ASITF :</font></td><td>" + ASITF2 + "</td></tr>"
                                + "<tr><td><font color='blue'> SSAFE :</font></td><td>" + SSAFE2 + "</td></tr>"
                                + "<tr><td><font color='blue'> DSS Approval:</font></td><td><a target='_blank' href='http://10.11.135.254:8080/RBC-SMS/UploadedFiles/" + missionAuthorization.non6 + "'>" + missionAuthorization.non6

                             + "<tr><td><a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorization1/Approval2FinAdmin?MissionID2="
                            + missionAuthorization.MissionID
                            + "&RDORDRDSignature2=Yes&FromDate2=" + missionAuthorization.FromDate
                            + "&ToDate2=" + missionAuthorization.ToDate
                            + "&to2=" + non10 + "&staffmail2=" + staffs.First().staff_email
                            + "'>Cleared</a></td><td><a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorization1/Approval2FinAdmin?MissionID2=" + missionAuthorization.MissionID
                            + "&RDORDRDSignature2=NO & FromDate2=" + missionAuthorization.FromDate
                            + "&ToDate2=" + missionAuthorization.ToDate
                            + "&to2=" + non10
                            + "&staffmail2=" + staffs.First().staff_email + "'>Not Cleared</a>" + "</a></td></tr></table>";

                            //string Body = "Request : Mission &nbsp;  &nbsp;  &nbsp; &nbsp;  &nbsp; &nbsp; &nbsp; &nbsp;  Mission ID : " + missionAuthorization.MissionID + " <br> " + " From &nbsp; &nbsp; &nbsp;:" + staffs.First().staff_first_name + "&nbsp;" + staffs.First().staff_last_name + "<br><br>" + " Mission Information" + "<br><br>" + " From :" + missionAuthorization.FromDate + "<br>To :" + missionAuthorization.ToDate +
                            //    "<br>Funding :" + missionAuthorization.funding + "<br>Objective :" + missionAuthorization.MissionObjective + "<br> Mission Itinerary :" + missionAuthorization.MissionItinerary + "<br> Can This meeting Be Done Over Voice/Video ? :" + missionAuthorization.canbedone  + " <br><br> <a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor=" + true + "&FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + " &to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Acept</a>&nbsp;&nbsp;&nbsp;<a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor= " + false + "& FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + "&to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Reject</a> ";

                            mailhou1.Body = Bodyhou1;
                            mailhou1.IsBodyHtml = true;
                            SmtpClient smtphou1 = new SmtpClient();
                            smtphou1.Host = "smtprelay.global.wfp.org";
                            smtphou1.Port = 25;
                            smtphou1.UseDefaultCredentials = true;
                            //smtp.Credentials = new System.Net.NetworkCredential
                            //("ahmed.badr", "Survivor2323");// Enter seders User name and password  
                            //smtp.EnableSsl = true;
                            smtphou1.Send(mailhou1);

                            ///////////////////////////// Finance/Admin Unit Approval end //////////////////////

                            return RedirectToAction("Index");
                        }

                        else
                        {
                            var canbedone = "";
                            if (missionAuthorization.canbedone == true)
                            {
                                canbedone = "Yes";
                            }
                            else
                            {
                                canbedone = "No";
                            }

                            var CleredBySupervisor = "";
                            if (missionAuthorization.canbedone == true)
                            {
                                CleredBySupervisor = "Yes";
                            }
                            else
                            {
                                CleredBySupervisor = "No";
                            }

                            var BSITF2 = "";
                            if (missionAuthorization.BSITF == true)
                            {
                                BSITF2 = "Yes";
                            }
                            else
                            {
                                BSITF2 = "No";
                            }
                            ////////  
                            var ASITF2 = "";
                            if (missionAuthorization.ASITF == true)
                            {
                                ASITF2 = "Yes";
                            }
                            else
                            {
                                ASITF2 = "No";
                            }
                            ////////
                            var SSAFE2 = "";
                            if (missionAuthorization.SSAFE == true)
                            {
                                SSAFE2 = "Yes";
                            }
                            else
                            {
                                SSAFE2 = "No";
                            }

                            //////////////////////////////


                            var supersforhou = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                            supersforhou = supersforhou.Where(d => d.staffid == staffsupervisor);
                            var superforhounitsname = supersforhou.First().staff_first_name + " " + staffshou.First().staff_last_name;
                            var houmailUPDATE = supersforhou.First().staff_email;

                            if (houmailUPDATE == "muhannad.hadi@wfp.org" || houmailUPDATE == "carlo.scaramella@wfp.org")
                            {
                                houmailUPDATE = "rbc.management@wfp.org"; //rbc.management@wfp.org

                            }
                            var odcitcc = "";

                            var Roming = "";
                            if (missionAuthorization.StaffSignature == "True")
                            {
                                Roming = "Yes";
                            }
                            else
                            {
                                Roming = "No";
                            }
                            if (Roming == "Yes")
                            {
                                odcitcc = "cairo.itservicedesk@wfp.org";
                            }
                            else
                            {
                                odcitcc = "ahmed.badr@wfp.org";
                            }
                            /////////////////////////

                            MailMessage mailhou = new MailMessage();
                            mailhou.To.Add(staff); //staffmail
                            mailhou.Bcc.Add("ahmed.badr@wfp.org");
                            mailhou.Bcc.Add(odcitcc); //travel unit
                            mailhou.CC.Add(superemail);
                            //mailhou.CC.Add("rbc.travel@wfp.org"); //travel unit
                            //mailhou.CC.Add("mai.kenawi@wfp.org"); //travel unit
                            //mailhou.CC.Add("amal.mohamed@wfp.org"); //travel unit
                            //mailhou.CC.Add("ayten.el-sheikh@wfp.org"); //travel unit
                            //mailhou.CC.Add("essraa.soliman@wfp.org"); //travel unit
                            //mailhou.CC.Add("rbc.management@wfp.org"); //RD/DRD Office
                            mailhou.From = new MailAddress("RBC-SMS-SYSTEM@wfp.org");
                            mailhou.Subject = "Mission Security Approved";

                            string Bodyhou = "<table><tr><td colspan='2'>" + "<hr/>New Mission<hr/></td></tr><tr><td><font color='blue'>Request : </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorization.MissionID + "</td></tr>"
                                + "<tr><td>" + "<font color='blue'>From :</font></td><td>" + staffs.First().staff_first_name + " " + staffs.First().staff_last_name + "</td></tr>"
                                + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                                + "<tr><td><font color='blue'>From :</font></td><td>" + missionAuthorization.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                                + "<tr><td><font color='blue'>To :</font></td><td>" + missionAuthorization.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                                + "<tr><td><font color='blue'>Funding :</font></td><td>" + missionAuthorization.funding + "</td></tr>"
                                + "<tr><td><font color='blue'>Objective :</font></td><td>" + missionAuthorization.MissionObjective + "</td></tr>"
                                + "<tr><td><font color='blue'>Mission Itinerary :</font></td><td>" + missionAuthorization.MissionItinerary + "</td></tr>"
                                + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video ? :</font></td><td>" + canbedone + "</td></tr>"
                                + "<tr><td><font color='blue'>Cleared By Supervisor :</font></td><td>" + CleredBySupervisor + "</td></tr>"
                                + "<tr><td><font color='blue'>International Roaming :</font></td><td>" + Roming + "</td></tr>"
                                 + "<tr><td><font color='blue'>HOU Comment :</font></td><td>" + missionAuthorization.Comment1 + "</td></tr>"
                                 + "<tr><td><font color='blue'>Security Comment :</font></td><td>" + missionAuthorization.Comment2 + "</td></tr>"
                                + "<tr><td><font color='blue'>	BSITF :</font></td><td>" + BSITF2 + "</td></tr>"
                                + "<tr><td><font color='blue'> ASITF :</font></td><td>" + ASITF2 + "</td></tr>"
                                + "<tr><td><font color='blue'> SSAFE :</font></td><td>" + SSAFE2 + "</td></tr>"
                               + "<tr><td><font color='blue'> DSS Approval:</font></td><td><a target='_blank' href='http://10.11.135.254:8080/RBC-SMS/UploadedFiles/" + missionAuthorization.non6 + "'>" + missionAuthorization.non6 + "</a></td></tr></table>";

                            //string Body = "Request : Mission &nbsp;  &nbsp;  &nbsp; &nbsp;  &nbsp; &nbsp; &nbsp; &nbsp;  Mission ID : " + missionAuthorization.MissionID + " <br> " + " From &nbsp; &nbsp; &nbsp;:" + staffs.First().staff_first_name + "&nbsp;" + staffs.First().staff_last_name + "<br><br>" + " Mission Information" + "<br><br>" + " From :" + missionAuthorization.FromDate + "<br>To :" + missionAuthorization.ToDate +
                            //    "<br>Funding :" + missionAuthorization.funding + "<br>Objective :" + missionAuthorization.MissionObjective + "<br> Mission Itinerary :" + missionAuthorization.MissionItinerary + "<br> Can This meeting Be Done Over Voice/Video ? :" + missionAuthorization.canbedone  + " <br><br> <a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor=" + true + "&FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + " &to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Acept</a>&nbsp;&nbsp;&nbsp;<a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor= " + false + "& FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + "&to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Reject</a> ";

                            mailhou.Body = Bodyhou;
                            mailhou.IsBodyHtml = true;
                            SmtpClient smtphou = new SmtpClient();
                            smtphou.Host = "smtprelay.global.wfp.org";
                            smtphou.Port = 25;
                            smtphou.UseDefaultCredentials = true;
                            //smtp.Credentials = new System.Net.NetworkCredential
                            //("ahmed.badr", "Survivor2323");// Enter seders User name and password  
                            //smtp.EnableSsl = true;
                            smtphou.Send(mailhou);

                            ///////////////////////////// Finance/Admin Unit Approval //////////////////////

                            MailMessage mailhou1 = new MailMessage();
                            mailhou1.To.Add("ahmed.badr@wfp.org");
                            mailhou1.Bcc.Add("ahmed.badr@wfp.org");

                            //mailhou.CC.Add("rbc.travel@wfp.org"); //travel unit
                            //mailhou.CC.Add("mai.kenawi@wfp.org"); //travel unit
                            //mailhou.CC.Add("amal.mohamed@wfp.org"); //travel unit
                            //mailhou.CC.Add("ayten.el-sheikh@wfp.org"); //travel unit
                            //mailhou.CC.Add("essraa.soliman@wfp.org"); //travel unit
                            //mailhou.CC.Add("rbc.management@wfp.org"); //RD/DRD Office
                            mailhou1.Bcc.Add(odcitcc); //travel unit
                            mailhou1.From = new MailAddress("RBC-SMS-SYSTEM@WFP.ORG");
                            mailhou1.Subject = "For Finance Clearance";

                            string Bodyhou1 = "<table><tr><td colspan='2'>" + "<hr/>New Mission<hr/></td></tr><tr><td><font color='blue'>Request : </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorization.MissionID + "</td></tr>"
                                + "<tr><td>" + "<font color='blue'>From :</font></td><td>" + staffs.First().staff_first_name + " " + staffs.First().staff_last_name + "</td></tr>"
                                + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                                + "<tr><td><font color='blue'>From :</font></td><td>" + missionAuthorization.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                                + "<tr><td><font color='blue'>To :</font></td><td>" + missionAuthorization.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                                + "<tr><td><font color='blue'>Funding :</font></td><td>" + missionAuthorization.funding + "</td></tr>"
                                + "<tr><td><font color='blue'>Objective :</font></td><td>" + missionAuthorization.MissionObjective + "</td></tr>"
                                + "<tr><td><font color='blue'>Mission Itinerary :</font></td><td>" + missionAuthorization.MissionItinerary + "</td></tr>"
                                + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video ? :</font></td><td>" + canbedone + "</td></tr>"
                                + "<tr><td><font color='blue'>Cleared By Supervisor :</font></td><td>" + CleredBySupervisor + "</td></tr>"
                                + "<tr><td><font color='blue'>International Roaming :</font></td><td>" + Roming + "</td></tr>"
                                + "<tr><td><font color='blue'>HOU Comment :</font></td><td>" + missionAuthorization.Comment1 + "</td></tr>"
                                + "<tr><td><font color='blue'>Security Comment :</font></td><td>" + missionAuthorization.Comment2 + "</td></tr>"
                                + "<tr><td><font color='blue'>	BSITF :</font></td><td>" + BSITF2 + "</td></tr>"
                                + "<tr><td><font color='blue'> ASITF :</font></td><td>" + ASITF2 + "</td></tr>"
                                + "<tr><td><font color='blue'> SSAFE :</font></td><td>" + SSAFE2 + "</td></tr>"
                                + "<tr><td><font color='blue'> DSS Approval:</font></td><td><a target='_blank' href='http://10.11.135.254:8080/RBC-SMS/UploadedFiles/" + missionAuthorization.non6 + "'>" + missionAuthorization.non6


                                 + "<tr><td><a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorization1/Approval2FinAdmin?MissionID2="
                            + missionAuthorization.MissionID
                            + "&RDORDRDSignature2=Yes&FromDate2=" + missionAuthorization.FromDate
                            + "&ToDate2=" + missionAuthorization.ToDate
                            + "&to2=" + non10 + "&staffmail2=" + staffs.First().staff_email
                            + "'>Cleared</a></td><td><a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorization1/Approval2FinAdmin?MissionID2=" + missionAuthorization.MissionID
                            + "&RDORDRDSignature2=NO & FromDate2=" + missionAuthorization.FromDate
                            + "&ToDate2=" + missionAuthorization.ToDate
                            + "&to2=" + non10
                            + "&staffmail2=" + staffs.First().staff_email + "'>Not Cleared</a>" + "</a></td></tr></table>";


                            //string Body = "Request : Mission &nbsp;  &nbsp;  &nbsp; &nbsp;  &nbsp; &nbsp; &nbsp; &nbsp;  Mission ID : " + missionAuthorization.MissionID + " <br> " + " From &nbsp; &nbsp; &nbsp;:" + staffs.First().staff_first_name + "&nbsp;" + staffs.First().staff_last_name + "<br><br>" + " Mission Information" + "<br><br>" + " From :" + missionAuthorization.FromDate + "<br>To :" + missionAuthorization.ToDate +
                            //    "<br>Funding :" + missionAuthorization.funding + "<br>Objective :" + missionAuthorization.MissionObjective + "<br> Mission Itinerary :" + missionAuthorization.MissionItinerary + "<br> Can This meeting Be Done Over Voice/Video ? :" + missionAuthorization.canbedone  + " <br><br> <a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor=" + true + "&FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + " &to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Acept</a>&nbsp;&nbsp;&nbsp;<a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor= " + false + "& FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + "&to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Reject</a> ";

                            mailhou1.Body = Bodyhou1;
                            mailhou1.IsBodyHtml = true;
                            SmtpClient smtphou1 = new SmtpClient();
                            smtphou1.Host = "smtprelay.global.wfp.org";
                            smtphou1.Port = 25;
                            smtphou1.UseDefaultCredentials = true;
                            //smtp.Credentials = new System.Net.NetworkCredential
                            //("ahmed.badr", "Survivor2323");// Enter seders User name and password  
                            //smtp.EnableSsl = true;
                            smtphou1.Send(mailhou1);

                            ///////////////////////////// Finance/Admin Unit Approval //////////////////////

                            return RedirectToAction("Index");


                        }
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
                    return View();
                }
                else
                {
                    //var missionAuthorizationn = db.MissionAuthorizations.Single(u => u.MissionID == Mid);
                    //db.MissionAuthorizations.Remove(missionAuthorizationn);
                    //db.SaveChanges();
                    var missionAuthorizationn = db.MissionAuthorization1.Single(u => u.MissionID == MissionID);
                    missionAuthorizationn.Signature2 = false;
                    missionAuthorizationn.Signature2Date = DateTime.Now;
                    missionAuthorizationn.Comment2 = Comment2;
                    db.SaveChanges();


                    var staffss = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                    staffss = staffss.Where(s => s.staff_email.Contains(staff));
                    var too = staffss.First().staff_email;
                    var staffsupervisor = staffss.First().staff_supervisorid;
                    var supers = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                    supers = supers.Where(d => d.staffid == staffsupervisor);
                    var from = supers.First().staff_email;

                    //var rd = db.staffs.Include(r => r.contract_details).Include(r => r.contract_type).Include(r => r.Country_office).Include(r => r.functional_title).Include(r => r.staff2).Include(r => r.sub_office).Include(r => r.unit).Include(r => r.country).Include(r => r.governorate);
                    //rd = rd.Where(r => r.functional_title.functional_title_description_english == "REGIONAL DIRECTOR");
                    //var rdmail = rd.First().staff_email;

                    //var drd = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                    //drd = drd.Where(s => s.functional_title.functional_title_description_english == "Deputy Regional Director");
                    //var drdmail = drd.First().staff_email;


                    //new
                    var userUnit = staffss.First().unit_id;
                    var unithou = db.units.Include(d => d.staffs);
                    unithou = unithou.Where(u => u.unitid == userUnit);
                    var houid = unithou.First().HOU_ID;

                    var staffshou = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                    staffshou = staffshou.Where(h => h.staffid == houid);
                    var hou = staffshou.First().staff_email;
                    //var houname = staffshou.First().staff_first_name + " " + staffshou.First().staff_last_name;

                    var cc = supers.First().staff_email;
                    var supername = supers.First().staff_first_name + " " + supers.First().staff_last_name;







                    var superemail = supers.First().staff_email;

                    //
                    //
                    //hou email
                    if (staffss.First().staffid == houid)
                    {
                        var canbedone = "";
                        if (missionAuthorizationn.canbedone == true)
                        {
                            canbedone = "Yes";
                        }
                        else
                        {
                            canbedone = "No";
                        }
                        var Roming = "";
                        if (missionAuthorizationn.StaffSignature == "True")
                        {
                            Roming = "Yes";
                        }
                        else
                        {
                            Roming = "No";
                        }
                        var CleredBySupervisor = "";
                        if (missionAuthorizationn.canbedone == true)
                        {
                            CleredBySupervisor = "Yes";
                        }
                        else
                        {
                            CleredBySupervisor = "No";
                        }

                        var BSITF2 = "";
                        if (missionAuthorizationn.BSITF == true)
                        {
                            BSITF2 = "Yes";
                        }
                        else
                        {
                            BSITF2 = "No";
                        }
                        ////////  
                        var ASITF2 = "";
                        if (missionAuthorizationn.ASITF == true)
                        {
                            ASITF2 = "Yes";
                        }
                        else
                        {
                            ASITF2 = "No";
                        }
                        ////////
                        var SSAFE2 = "";
                        if (missionAuthorizationn.SSAFE == true)
                        {
                            SSAFE2 = "Yes";
                        }
                        else
                        {
                            SSAFE2 = "No";
                        }
                        var odcitcc = "";
                        if (Roming == "Yes")
                        {
                            odcitcc = "cairo.itservicedesk@wfp.org";
                        }
                        else
                        {
                            odcitcc = "ahmed.badr@wfp.org";
                        }
                        //////////////////////////////


                        var supersforhou = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                        supersforhou = supersforhou.Where(d => d.staffid == staffsupervisor);
                        var superforhounitsname = supersforhou.First().staff_first_name + " " + staffshou.First().staff_last_name;
                        var houmailUPDATE = supersforhou.First().staff_email;

                        if (houmailUPDATE == "muhannad.hadi@wfp.org" || houmailUPDATE == "carlo.scaramella@wfp.org")
                        {
                            houmailUPDATE = "rbc.management@wfp.org"; //rbc.management@wfp.org

                        }
                        /////////////////////////

                        MailMessage mailhou = new MailMessage();
                        mailhou.To.Add(staff);
                        mailhou.Bcc.Add("ahmed.badr@wfp.org");
                        //mail2.CC.Add(cc);
                        mailhou.From = new MailAddress("RBC-SMS-SYSTEM@WFP.ORG");
                        mailhou.Subject = "Mission Security Rejected";

                        string Bodyhou = "<table><tr><td colspan='2'>" + "<hr/>New Mission<hr/></td></tr><tr><td><font color='blue'>Request : </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorizationn.MissionID + "</td></tr>"
                            + "<tr><td>" + "<font color='blue'>From :</font></td><td>" + staffss.First().staff_first_name + " " + staffss.First().staff_last_name + "</td></tr>"
                            + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                            + "<tr><td><font color='blue'>From :</font></td><td>" + missionAuthorizationn.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                            + "<tr><td><font color='blue'>To :</font></td><td>" + missionAuthorizationn.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                            + "<tr><td><font color='blue'>Funding :</font></td><td>" + missionAuthorizationn.funding + "</td></tr>"
                            + "<tr><td><font color='blue'>Objective :</font></td><td>" + missionAuthorizationn.MissionObjective + "</td></tr>"
                            + "<tr><td><font color='blue'>Mission Itinerary :</font></td><td>" + missionAuthorizationn.MissionItinerary + "</td></tr>"
                            + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video ? :</font></td><td>" + canbedone + "</td></tr>"
                            + "<tr><td><font color='blue'>Cleared By Supervisor :</font></td><td>" + CleredBySupervisor + "</td></tr>"
                             + "<tr><td><font color='blue'>HOU Comment :</font></td><td>" + missionAuthorizationn.Comment1 + "</td></tr>"
                             + "<tr><td><font color='blue'>Security Comment :</font></td><td>" + missionAuthorizationn.Comment2 + "</td></tr>"
                            + "<tr><td><font color='blue'>	BSITF :</font></td><td>" + BSITF2 + "</td></tr>"
                            + "<tr><td><font color='blue'> ASITF :</font></td><td>" + ASITF2 + "</td></tr>"
                            + "<tr><td><font color='blue'> SSAFE :</font></td><td>" + SSAFE2 + "</td></tr>"
                           + "<tr><td><font color='blue'> DSS Approval:</font></td><td><a target='_blank' href='http://10.11.135.254:8080/RBC-SMS/UploadedFiles/" + missionAuthorizationn.non6 + "'>" + missionAuthorizationn.non6 + "</a></td></tr></table>";

                        //string Body = "Request : Mission &nbsp;  &nbsp;  &nbsp; &nbsp;  &nbsp; &nbsp; &nbsp; &nbsp;  Mission ID : " + missionAuthorization.MissionID + " <br> " + " From &nbsp; &nbsp; &nbsp;:" + staffs.First().staff_first_name + "&nbsp;" + staffs.First().staff_last_name + "<br><br>" + " Mission Information" + "<br><br>" + " From :" + missionAuthorization.FromDate + "<br>To :" + missionAuthorization.ToDate +
                        //    "<br>Funding :" + missionAuthorization.funding + "<br>Objective :" + missionAuthorization.MissionObjective + "<br> Mission Itinerary :" + missionAuthorization.MissionItinerary + "<br> Can This meeting Be Done Over Voice/Video ? :" + missionAuthorization.canbedone  + " <br><br> <a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor=" + true + "&FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + " &to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Acept</a>&nbsp;&nbsp;&nbsp;<a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor= " + false + "& FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + "&to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Reject</a> ";

                        mailhou.Body = Bodyhou;
                        mailhou.IsBodyHtml = true;
                        SmtpClient smtphou = new SmtpClient();
                        smtphou.Host = "smtprelay.global.wfp.org";
                        smtphou.Port = 25;
                        smtphou.UseDefaultCredentials = true;
                        //smtp.Credentials = new System.Net.NetworkCredential
                        //("ahmed.badr", "Survivor2323");// Enter seders User name and password  
                        //smtp.EnableSsl = true;
                        smtphou.Send(mailhou);
                        return RedirectToAction("Index");
                    }

                    else
                    {
                        var canbedone = "";
                        if (missionAuthorizationn.canbedone == true)
                        {
                            canbedone = "Yes";
                        }
                        else
                        {
                            canbedone = "No";
                        }

                        var CleredBySupervisor = "";
                        if (missionAuthorizationn.canbedone == true)
                        {
                            CleredBySupervisor = "Yes";
                        }
                        else
                        {
                            CleredBySupervisor = "No";
                        }

                        var BSITF2 = "";
                        if (missionAuthorizationn.BSITF == true)
                        {
                            BSITF2 = "Yes";
                        }
                        else
                        {
                            BSITF2 = "No";
                        }
                        ////////  
                        var ASITF2 = "";
                        if (missionAuthorizationn.ASITF == true)
                        {
                            ASITF2 = "Yes";
                        }
                        else
                        {
                            ASITF2 = "No";
                        }
                        ////////
                        var SSAFE2 = "";
                        if (missionAuthorizationn.SSAFE == true)
                        {
                            SSAFE2 = "Yes";
                        }
                        else
                        {
                            SSAFE2 = "No";
                        }

                        //////////////////////////////


                        var supersforhou = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                        supersforhou = supersforhou.Where(d => d.staffid == staffsupervisor);
                        var superforhounitsname = supersforhou.First().staff_first_name + " " + staffshou.First().staff_last_name;
                        var houmailUPDATE = supersforhou.First().staff_email;

                        if (houmailUPDATE == "muhannad.hadi@wfp.org" || houmailUPDATE == "carlo.scaramella@wfp.org")
                        {
                            houmailUPDATE = "rbc.management@wfp.org"; //rbc.management@wfp.org

                        }
                        /////////////////////////


                        MailMessage mailhou = new MailMessage();
                        mailhou.To.Add(staff);
                        mailhou.Bcc.Add("ahmed.badr@wfp.org");
                        mailhou.CC.Add(cc);
                        mailhou.From = new MailAddress(hou);
                        mailhou.Subject = "Mission Security Rejected";

                        string Bodyhou = "<table><tr><td colspan='2'>" + "<hr/>New Mission<hr/></td></tr><tr><td><font color='blue'>Request : </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorizationn.MissionID + "</td></tr>"
                            + "<tr><td>" + "<font color='blue'>From :</font></td><td>" + staffss.First().staff_first_name + " " + staffss.First().staff_last_name + "</td></tr>"
                            + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                            + "<tr><td><font color='blue'>From :</font></td><td>" + missionAuthorizationn.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                            + "<tr><td><font color='blue'>To :</font></td><td>" + missionAuthorizationn.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                            + "<tr><td><font color='blue'>Funding :</font></td><td>" + missionAuthorizationn.funding + "</td></tr>"
                            + "<tr><td><font color='blue'>Objective :</font></td><td>" + missionAuthorizationn.MissionObjective + "</td></tr>"
                            + "<tr><td><font color='blue'>Mission Itinerary :</font></td><td>" + missionAuthorizationn.MissionItinerary + "</td></tr>"
                            + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video ? :</font></td><td>" + canbedone + "</td></tr>"
                            + "<tr><td><font color='blue'>Cleared By Supervisor :</font></td><td>" + CleredBySupervisor + "</td></tr>"
                             + "<tr><td><font color='blue'>HOU Comment :</font></td><td>" + missionAuthorizationn.Comment1 + "</td></tr>"
                             + "<tr><td><font color='blue'>Security Comment :</font></td><td>" + missionAuthorizationn.Comment2 + "</td></tr>"
                            + "<tr><td><font color='blue'>	BSITF :</font></td><td>" + BSITF2 + "</td></tr>"
                            + "<tr><td><font color='blue'> ASITF :</font></td><td>" + ASITF2 + "</td></tr>"
                            + "<tr><td><font color='blue'> SSAFE :</font></td><td>" + SSAFE2 + "</td></tr>"
                           + "<tr><td><font color='blue'> DSS Approval:</font></td><td><a target='_blank' href='http://10.11.135.254:8080/RBC-SMS/UploadedFiles/" + missionAuthorizationn.non6 + "'>" + missionAuthorizationn.non6 + "</a></td></tr></table>";

                        //string Body = "Request : Mission &nbsp;  &nbsp;  &nbsp; &nbsp;  &nbsp; &nbsp; &nbsp; &nbsp;  Mission ID : " + missionAuthorization.MissionID + " <br> " + " From &nbsp; &nbsp; &nbsp;:" + staffs.First().staff_first_name + "&nbsp;" + staffs.First().staff_last_name + "<br><br>" + " Mission Information" + "<br><br>" + " From :" + missionAuthorization.FromDate + "<br>To :" + missionAuthorization.ToDate +
                        //    "<br>Funding :" + missionAuthorization.funding + "<br>Objective :" + missionAuthorization.MissionObjective + "<br> Mission Itinerary :" + missionAuthorization.MissionItinerary + "<br> Can This meeting Be Done Over Voice/Video ? :" + missionAuthorization.canbedone  + " <br><br> <a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor=" + true + "&FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + " &to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Acept</a>&nbsp;&nbsp;&nbsp;<a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor= " + false + "& FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + "&to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Reject</a> ";

                        mailhou.Body = Bodyhou;
                        mailhou.IsBodyHtml = true;
                        SmtpClient smtphou = new SmtpClient();
                        smtphou.Host = "smtprelay.global.wfp.org";
                        smtphou.Port = 25;
                        smtphou.UseDefaultCredentials = true;
                        //smtp.Credentials = new System.Net.NetworkCredential
                        //("ahmed.badr", "Survivor2323");// Enter seders User name and password  
                        //smtp.EnableSsl = true;
                        smtphou.Send(mailhou);
                        return RedirectToAction("Index");


                    }
                }
            }
            return View();

        }


        ////////////////////////////////////////////////////////////////////////////////// end approval level 2 security //////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



        /////////////////////////////////////////////////////////////////////// start approval level 3 Finance //////////////////////////////////////////////////////////////////////////////////////////

        public ActionResult Approval2FinAdmin(int? i, int? MissionID2, string RDORDRDSignature2, DateTime? FromDate2, DateTime? ToDate2, string to2, string staffmail2, string supervisorname2, int? onbehalfID2, int? onbehalfSuperID2, int? onbehalfHouID2)
        {
            ViewBag.MissionID = MissionID2;
            ViewBag.RDORDRDSignature = RDORDRDSignature2;
            ViewBag.FromDate = FromDate2;
            ViewBag.ToDate = ToDate2;
            ViewBag.to = to2;
            ViewBag.staffmail = staffmail2;
            ViewBag.supervisorname = supervisorname2;
            ViewBag.onbehalfID = onbehalfID2;
            ViewBag.onbehalfSuperID = onbehalfSuperID2;
            ViewBag.onbehalfHouID = onbehalfHouID2;

            return View();
        }

        [HttpPost]
        public ActionResult Approval2FinAdmin(int? MissionID,string funding , string Comment3, string RDORDRDSignature, DateTime? FromDate, DateTime? ToDate, string non10, string staff, string supervisorname, int? onbehalfID, int? onbehalfSuperID, int? onbehalfHouID)
        {


            if (!Request.IsAuthenticated)
            {
                Response.Redirect("/RBC-SMS/account/login");
            }

            ///////////////////////// start on behalf 

            if (onbehalfID != null && onbehalfSuperID != null && onbehalfHouID != null)
            {
                ViewBag.mid = Request.QueryString["MissionID"];
                ViewBag.RDORDRDSignature = Request.QueryString["RDORDRDSignature"];
                ViewBag.FromDate = Request.QueryString["FromDate"];
                ViewBag.ToDate = Request.QueryString["ToDate"];
                if (RDORDRDSignature == "Yes")
                {
                    try
                    {
                        // missionAuthorization = db.MissionAuthorizations.Find(Mid);
                        //db.Entry(missionAuthorization).State = EntityState.Modified ;
                        //db.SaveChanges();

                        var missionAuthorization11 = db.MissionAuthorization1.Single(u => u.MissionID == MissionID);
                        missionAuthorization11.Signature3 = true;
                        missionAuthorization11.Signature3Date = DateTime.Now;
                        missionAuthorization11.Comment3 = Comment3;
                        if (funding != "")
                        {
                            missionAuthorization11.funding = funding;
                        }
                        db.SaveChanges();



                        var staffs = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                        staffs = staffs.Where(s => s.staffid == onbehalfID);
                        var too = staffs.First().staff_email;
                        ////////////////////////////////////////////////////////////////////////////onbehalf
                        var staffsupervisor = staffs.First().staff_supervisorid;
                        var supers = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                        supers = supers.Where(d => d.staffid == onbehalfID);
                        var onbehalfemail = supers.First().staff_email;


                        var supersonbehalf = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                        supersonbehalf = supersonbehalf.Where(d => d.staffid == onbehalfSuperID);
                        var onbehafsuper = supersonbehalf.First().staff_email;
                        /////////////////////////////////////////////////////////////////////////////////////
                        ////////////////////////////////////////////////////
                        //var rd = db.staffs.Include(r => r.contract_details).Include(r => r.contract_type).Include(r => r.Country_office).Include(r => r.functional_title).Include(r => r.staff2).Include(r => r.sub_office).Include(r => r.unit).Include(r => r.country).Include(r => r.governorate);
                        //rd = rd.Where(r => r.functional_title.functional_title_description_english == "REGIONAL DIRECTOR");
                        //var rdmail = rd.First().staff_email;

                        //var drd = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                        //drd = drd.Where(s => s.functional_title.functional_title_description_english == "Deputy Regional Director");
                        //var drdmail = drd.First().staff_email;

                        var userUnit = staffs.First().unit_id;
                        var unithou = db.units.Include(d => d.staffs);
                        unithou = unithou.Where(u => u.unitid == userUnit);
                        var houid = unithou.First().HOU_ID;

                        var staffshou = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                        staffshou = staffshou.Where(h => h.staffid == houid);
                        var hou = staffshou.First().staff_email;
                        //var houname = staffshou.First().staff_first_name + " " + staffshou.First().staff_last_name;

                        var cc = supers.First().staff_email;
                        var supername = supers.First().staff_first_name + " " + supers.First().staff_last_name;
                        var superemail = supers.First().staff_email;

                        //
                        //
                        //hou email
                        if (onbehalfID == houid)
                        {
                            var canbedone = "";
                            if (missionAuthorization11.canbedone == true)
                            {
                                canbedone = "Yes";
                            }
                            else
                            {
                                canbedone = "No";
                            }

                            var CleredBySupervisor = "";
                            if (missionAuthorization11.canbedone == true)
                            {
                                CleredBySupervisor = "Yes";
                            }
                            else
                            {
                                CleredBySupervisor = "No";
                            }

                            var BSITF2 = "";
                            if (missionAuthorization11.BSITF == true)
                            {
                                BSITF2 = "Yes";
                            }
                            else
                            {
                                BSITF2 = "No";
                            }
                            ////////  
                            var ASITF2 = "";
                            if (missionAuthorization11.ASITF == true)
                            {
                                ASITF2 = "Yes";
                            }
                            else
                            {
                                ASITF2 = "No";
                            }
                            ////////
                            var SSAFE2 = "";
                            if (missionAuthorization11.SSAFE == true)
                            {
                                SSAFE2 = "Yes";
                            }
                            else
                            {
                                SSAFE2 = "No";
                            }
                            var Roming = "";
                            if (missionAuthorization11.StaffSignature == "True")
                            {
                                Roming = "Yes";
                            }
                            else
                            {
                                Roming = "No";
                            }
                            var odcitcc = "";
                            if (Roming == "Yes")
                            {
                                odcitcc = "cairo.itservicedesk@wfp.org";
                            }
                            else
                            {
                                odcitcc = "ahmed.badr@wfp.org";
                            }
                            //////////////////////////////


                            var supersforhou = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                            supersforhou = supersforhou.Where(d => d.staffid == staffsupervisor);
                            var superforhounitsname = supersforhou.First().staff_first_name + " " + staffshou.First().staff_last_name;
                            var houmailUPDATE = supersforhou.First().staff_email;

                            if (houmailUPDATE == "muhannad.hadi@wfp.org" || houmailUPDATE == "carlo.scaramella@wfp.org")
                            {
                                houmailUPDATE = "rbc.management@wfp.org"; //rbc.management@wfp.org

                            }
                            /////////////////////////

                            MailMessage mailhou = new MailMessage();
                            mailhou.To.Add(onbehalfemail);
                            mailhou.Bcc.Add("ahmed.badr@wfp.org");

                            //mailhou.CC.Add("rbc.travel@wfp.org"); //travel unit
                            //mailhou.CC.Add("mai.kenawi@wfp.org"); //travel unit
                            //mailhou.CC.Add("amal.mohamed@wfp.org"); //travel unit
                            //mailhou.CC.Add("ayten.el-sheikh@wfp.org"); //travel unit
                            //mailhou.CC.Add("essraa.soliman@wfp.org"); //travel unit
                            //mailhou.CC.Add("rbc.management@wfp.org"); //RD/DRD Office
                            mailhou.Bcc.Add(odcitcc); //travel unit
                            mailhou.From = new MailAddress("RBC-SMS-SYSTEM@wfp.org");
                            mailhou.Subject = "Finance Cleared";

                            string Bodyhou = "<table><tr><td colspan='2'>" + "<hr/>New Mission<hr/></td></tr><tr><td><font color='blue'>Request : </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorization11.MissionID + "</td></tr>"
                                + "<tr><td>" + "<font color='blue'>From :</font></td><td>" + staffs.First().staff_first_name + " " + staffs.First().staff_last_name + "</td></tr>"
                                + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                                + "<tr><td><font color='blue'>From :</font></td><td>" + missionAuthorization11.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                                + "<tr><td><font color='blue'>To :</font></td><td>" + missionAuthorization11.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                                + "<tr><td><font color='blue'>Funding :</font></td><td>" + missionAuthorization11.funding + "</td></tr>"
                                + "<tr><td><font color='blue'>Objective :</font></td><td>" + missionAuthorization11.MissionObjective + "</td></tr>"
                                + "<tr><td><font color='blue'>Mission Itinerary :</font></td><td>" + missionAuthorization11.MissionItinerary + "</td></tr>"
                                + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video ? :</font></td><td>" + canbedone + "</td></tr>"
                                + "<tr><td><font color='blue'>Cleared By Supervisor :</font></td><td>" + CleredBySupervisor + "</td></tr>"
                                + "<tr><td><font color='blue'>International Roaming :</font></td><td>" + Roming + "</td></tr>"
                                + "<tr><td><font color='blue'>HOU Comment :</font></td><td>" + missionAuthorization11.Comment1 + "</td></tr>"
                                + "<tr><td><font color='blue'>Security Comment :</font></td><td>" + missionAuthorization11.Comment2 + "</td></tr>"
                                + "<tr><td><font color='blue'>Finance Comment :</font></td><td>" + missionAuthorization11.Comment3 + "</td></tr>"
                                + "<tr><td><font color='blue'>	BSITF :</font></td><td>" + BSITF2 + "</td></tr>"
                                + "<tr><td><font color='blue'> ASITF :</font></td><td>" + ASITF2 + "</td></tr>"
                                + "<tr><td><font color='blue'> SSAFE :</font></td><td>" + SSAFE2 + "</td></tr>"
                                + "<tr><td><font color='blue'> DSS Approval:</font></td><td><a href='http://10.11.135.254:8080/RBC-SMS/UploadedFiles/" + missionAuthorization11.non6 + "'>" + missionAuthorization11.non6 + "</a></td></tr></table>";

                            //string Body = "Request : Mission &nbsp;  &nbsp;  &nbsp; &nbsp;  &nbsp; &nbsp; &nbsp; &nbsp;  Mission ID : " + missionAuthorization.MissionID + " <br> " + " From &nbsp; &nbsp; &nbsp;:" + staffs.First().staff_first_name + "&nbsp;" + staffs.First().staff_last_name + "<br><br>" + " Mission Information" + "<br><br>" + " From :" + missionAuthorization.FromDate + "<br>To :" + missionAuthorization.ToDate +
                            //    "<br>Funding :" + missionAuthorization.funding + "<br>Objective :" + missionAuthorization.MissionObjective + "<br> Mission Itinerary :" + missionAuthorization.MissionItinerary + "<br> Can This meeting Be Done Over Voice/Video ? :" + missionAuthorization.canbedone  + " <br><br> <a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor=" + true + "&FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + " &to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Acept</a>&nbsp;&nbsp;&nbsp;<a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor= " + false + "& FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + "&to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Reject</a> ";

                            mailhou.Body = Bodyhou;
                            mailhou.IsBodyHtml = true;
                            SmtpClient smtphou = new SmtpClient();
                            smtphou.Host = "smtprelay.global.wfp.org";
                            smtphou.Port = 25;
                            smtphou.UseDefaultCredentials = true;
                            //smtp.Credentials = new System.Net.NetworkCredential
                            //("ahmed.badr", "Survivor2323");// Enter seders User name and password  
                            //smtp.EnableSsl = true;
                            smtphou.Send(mailhou);

                            ///////////////////////////// CD  Approval //////////////////////

                            MailMessage mailhou1 = new MailMessage();
                            mailhou1.To.Add("ahmed.badr@wfp.org");
                            mailhou1.Bcc.Add("ahmed.badr@wfp.org");

                            //mailhou.CC.Add("rbc.travel@wfp.org"); //travel unit
                            //mailhou.CC.Add("mai.kenawi@wfp.org"); //travel unit
                            //mailhou.CC.Add("amal.mohamed@wfp.org"); //travel unit
                            //mailhou.CC.Add("ayten.el-sheikh@wfp.org"); //travel unit
                            //mailhou.CC.Add("essraa.soliman@wfp.org"); //travel unit
                            //mailhou.CC.Add("rbc.management@wfp.org"); //RD/DRD Office
                            mailhou1.Bcc.Add(odcitcc); //travel unit
                            mailhou1.From = new MailAddress("RBC-SMS-SYSTEM@wfp.org");
                            mailhou1.Subject = "For CD Approval";

                            string Bodyhou1 = "<table><tr><td colspan='2'>" + "<hr/>New Mission<hr/></td></tr><tr><td><font color='blue'>Request : </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorization11.MissionID + "</td></tr>"
                                + "<tr><td>" + "<font color='blue'>From :</font></td><td>" + staffs.First().staff_first_name + " " + staffs.First().staff_last_name + "</td></tr>"
                                + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                                + "<tr><td><font color='blue'>From :</font></td><td>" + missionAuthorization11.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                                + "<tr><td><font color='blue'>To :</font></td><td>" + missionAuthorization11.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                                + "<tr><td><font color='blue'>Funding :</font></td><td>" + missionAuthorization11.funding + "</td></tr>"
                                + "<tr><td><font color='blue'>Objective :</font></td><td>" + missionAuthorization11.MissionObjective + "</td></tr>"
                                + "<tr><td><font color='blue'>Mission Itinerary :</font></td><td>" + missionAuthorization11.MissionItinerary + "</td></tr>"
                                + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video ? :</font></td><td>" + canbedone + "</td></tr>"
                                + "<tr><td><font color='blue'>Cleared By Supervisor :</font></td><td>" + CleredBySupervisor + "</td></tr>"
                                + "<tr><td><font color='blue'>International Roaming :</font></td><td>" + Roming + "</td></tr>"
                                + "<tr><td><font color='blue'>HOU Comment :</font></td><td>" + missionAuthorization11.Comment1 + "</td></tr>"
                                   + "<tr><td><font color='blue'>Security Comment :</font></td><td>" + missionAuthorization11.Comment2 + "</td></tr>"
                                      + "<tr><td><font color='blue'>FInance Comment :</font></td><td>" + missionAuthorization11.Comment3 + "</td></tr>"
                                + "<tr><td><font color='blue'>	BSITF :</font></td><td>" + BSITF2 + "</td></tr>"
                                + "<tr><td><font color='blue'> ASITF :</font></td><td>" + ASITF2 + "</td></tr>"
                                + "<tr><td><font color='blue'> SSAFE :</font></td><td>" + SSAFE2 + "</td></tr>"
                                + "<tr><td><font color='blue'> DSS Approval:</font></td><td><a href='http://10.11.135.254:8080/RBC-SMS/UploadedFiles/" + missionAuthorization11.non6 + "'>" + missionAuthorization11.non6
                            + "<tr><td><a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorization1/Approval2CD?MissionID2="
                            + missionAuthorization11.MissionID
                            + "&RDORDRDSignature2=Yes&FromDate2=" + missionAuthorization11.FromDate
                            + "&ToDate2=" + missionAuthorization11.ToDate
                            + "&to2=" + non10 + "&staffmail2=" + staffs.First().staff_email
                            + "'>Accept</a></td><td><a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorization1/Approval2CD?MissionID2=" + missionAuthorization11.MissionID
                            + "&RDORDRDSignature2=NO & FromDate2=" + missionAuthorization11.FromDate
                            + "&ToDate2=" + missionAuthorization11.ToDate
                            + "&to2=" + non10
                            + "&staffmail2=" + staffs.First().staff_email + "'>Reject</a>" + "</a></td></tr></table>";

                            //string Body = "Request : Mission &nbsp;  &nbsp;  &nbsp; &nbsp;  &nbsp; &nbsp; &nbsp; &nbsp;  Mission ID : " + missionAuthorization.MissionID + " <br> " + " From &nbsp; &nbsp; &nbsp;:" + staffs.First().staff_first_name + "&nbsp;" + staffs.First().staff_last_name + "<br><br>" + " Mission Information" + "<br><br>" + " From :" + missionAuthorization.FromDate + "<br>To :" + missionAuthorization.ToDate +
                            //    "<br>Funding :" + missionAuthorization.funding + "<br>Objective :" + missionAuthorization.MissionObjective + "<br> Mission Itinerary :" + missionAuthorization.MissionItinerary + "<br> Can This meeting Be Done Over Voice/Video ? :" + missionAuthorization.canbedone  + " <br><br> <a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor=" + true + "&FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + " &to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Acept</a>&nbsp;&nbsp;&nbsp;<a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor= " + false + "& FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + "&to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Reject</a> ";

                            mailhou1.Body = Bodyhou1;
                            mailhou1.IsBodyHtml = true;
                            SmtpClient smtphou1 = new SmtpClient();
                            smtphou1.Host = "smtprelay.global.wfp.org";
                            smtphou1.Port = 25;
                            smtphou1.UseDefaultCredentials = true;
                            //smtp.Credentials = new System.Net.NetworkCredential
                            //("ahmed.badr", "Survivor2323");// Enter seders User name and password  
                            //smtp.EnableSsl = true;
                            smtphou1.Send(mailhou1);

                            ///////////////////////////// CD Approval  end //////////////////////
                            return RedirectToAction("Index");
                        }

                        else
                        {
                            var canbedone = "";
                            if (missionAuthorization11.canbedone == true)
                            {
                                canbedone = "Yes";
                            }
                            else
                            {
                                canbedone = "No";
                            }

                            var CleredBySupervisor = "";
                            if (missionAuthorization11.canbedone == true)
                            {
                                CleredBySupervisor = "Yes";
                            }
                            else
                            {
                                CleredBySupervisor = "No";
                            }

                            var BSITF2 = "";
                            if (missionAuthorization11.BSITF == true)
                            {
                                BSITF2 = "Yes";
                            }
                            else
                            {
                                BSITF2 = "No";
                            }
                            ////////  
                            var ASITF2 = "";
                            if (missionAuthorization11.ASITF == true)
                            {
                                ASITF2 = "Yes";
                            }
                            else
                            {
                                ASITF2 = "No";
                            }
                            ////////
                            var SSAFE2 = "";
                            if (missionAuthorization11.SSAFE == true)
                            {
                                SSAFE2 = "Yes";
                            }
                            else
                            {
                                SSAFE2 = "No";
                            }
                            var odcitcc = "";
                            if (missionAuthorization11.StaffSignature == "True")
                            {
                                odcitcc = "cairo.itservicedesk@wfp.org";
                            }
                            else
                            {
                                odcitcc = "ahmed.badr@wfp.org";
                            }
                            var Roming = "";
                            if (missionAuthorization11.StaffSignature == "True")
                            {
                                Roming = "Yes";
                            }
                            else
                            {
                                Roming = "No";
                            }
                            //////////////////////////////


                            var supersforhou = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                            supersforhou = supersforhou.Where(d => d.staffid == staffsupervisor);
                            var superforhounitsname = supersforhou.First().staff_first_name + " " + staffshou.First().staff_last_name;
                            var houmailUPDATE = supersforhou.First().staff_email;

                            if (houmailUPDATE == "muhannad.hadi@wfp.org" || houmailUPDATE == "carlo.scaramella@wfp.org")
                            {
                                houmailUPDATE = "rbc.management@wfp.org"; //rbc.management@wfp.org

                            }
                            /////////////////////////

                            MailMessage mailhou = new MailMessage();
                            mailhou.To.Add(onbehalfemail); //staffmail
                            mailhou.Bcc.Add("ahmed.badr@wfp.org");
                            mailhou.CC.Add(onbehafsuper);
                            mailhou.Bcc.Add(odcitcc); //travel unit
                            //mailhou.CC.Add("rbc.travel@wfp.org"); //travel unit
                            //mailhou.CC.Add("mai.kenawi@wfp.org"); //travel unit
                            //mailhou.CC.Add("amal.mohamed@wfp.org"); //travel unit
                            //mailhou.CC.Add("ayten.el-sheikh@wfp.org"); //travel unit
                            //mailhou.CC.Add("essraa.soliman@wfp.org"); //travel unit
                            //mailhou.CC.Add("rbc.management@wfp.org"); //RD/DRD Office
                            mailhou.From = new MailAddress(hou);
                            mailhou.Subject = "Finance Cleared";

                            string Bodyhou = "<table><tr><td colspan='2'>" + "<hr/>New Mission<hr/></td></tr><tr><td><font color='blue'>Request : </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorization11.MissionID + "</td></tr>"
                                + "<tr><td>" + "<font color='blue'>From :</font></td><td>" + staffs.First().staff_first_name + " " + staffs.First().staff_last_name + "</td></tr>"
                                + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                                + "<tr><td><font color='blue'>From :</font></td><td>" + missionAuthorization11.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                                + "<tr><td><font color='blue'>To :</font></td><td>" + missionAuthorization11.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                                + "<tr><td><font color='blue'>Funding :</font></td><td>" + missionAuthorization11.funding + "</td></tr>"
                                + "<tr><td><font color='blue'>Objective :</font></td><td>" + missionAuthorization11.MissionObjective + "</td></tr>"
                                + "<tr><td><font color='blue'>Mission Itinerary :</font></td><td>" + missionAuthorization11.MissionItinerary + "</td></tr>"
                                + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video ? :</font></td><td>" + canbedone + "</td></tr>"
                                + "<tr><td><font color='blue'>Cleared By Supervisor :</font></td><td>" + CleredBySupervisor + "</td></tr>"
                                + "<tr><td><font color='blue'>International Roaming :</font></td><td>" + Roming + "</td></tr>"
                                + "<tr><td><font color='blue'>HOU Comment :</font></td><td>" + missionAuthorization11.Comment1 + "</td></tr>"
                                + "<tr><td><font color='blue'>Security Comment :</font></td><td>" + missionAuthorization11.Comment2 + "</td></tr>"
                                + "<tr><td><font color='blue'>Finance Comment :</font></td><td>" + missionAuthorization11.Comment3 + "</td></tr>"
                                + "<tr><td><font color='blue'>	BSITF :</font></td><td>" + BSITF2 + "</td></tr>"
                                + "<tr><td><font color='blue'> ASITF :</font></td><td>" + ASITF2 + "</td></tr>"
                                + "<tr><td><font color='blue'> SSAFE :</font></td><td>" + SSAFE2 + "</td></tr>"
                                + "<tr><td><font color='blue'> DSS Approval:</font></td><td><a href='http://10.11.135.254:8080/RBC-SMS/UploadedFiles/" + missionAuthorization11.non6 + "'>" + missionAuthorization11.non6 + "</a></td></tr></table>";

                            //string Body = "Request : Mission &nbsp;  &nbsp;  &nbsp; &nbsp;  &nbsp; &nbsp; &nbsp; &nbsp;  Mission ID : " + missionAuthorization.MissionID + " <br> " + " From &nbsp; &nbsp; &nbsp;:" + staffs.First().staff_first_name + "&nbsp;" + staffs.First().staff_last_name + "<br><br>" + " Mission Information" + "<br><br>" + " From :" + missionAuthorization.FromDate + "<br>To :" + missionAuthorization.ToDate +
                            //    "<br>Funding :" + missionAuthorization.funding + "<br>Objective :" + missionAuthorization.MissionObjective + "<br> Mission Itinerary :" + missionAuthorization.MissionItinerary + "<br> Can This meeting Be Done Over Voice/Video ? :" + missionAuthorization.canbedone  + " <br><br> <a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor=" + true + "&FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + " &to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Acept</a>&nbsp;&nbsp;&nbsp;<a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor= " + false + "& FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + "&to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Reject</a> ";

                            mailhou.Body = Bodyhou;
                            mailhou.IsBodyHtml = true;
                            SmtpClient smtphou = new SmtpClient();
                            smtphou.Host = "smtprelay.global.wfp.org";
                            smtphou.Port = 25;
                            smtphou.UseDefaultCredentials = true;
                            //smtp.Credentials = new System.Net.NetworkCredential
                            //("ahmed.badr", "Survivor2323");// Enter seders User name and password  
                            //smtp.EnableSsl = true;
                            smtphou.Send(mailhou);


                            ///////////////////////////// CD  Approval //////////////////////

                            MailMessage mailhou1 = new MailMessage();
                            mailhou1.To.Add("ahmed.badr@wfp.org");
                            mailhou1.Bcc.Add("ahmed.badr@wfp.org");

                            //mailhou.CC.Add("rbc.travel@wfp.org"); //travel unit
                            //mailhou.CC.Add("mai.kenawi@wfp.org"); //travel unit
                            //mailhou.CC.Add("amal.mohamed@wfp.org"); //travel unit
                            //mailhou.CC.Add("ayten.el-sheikh@wfp.org"); //travel unit
                            //mailhou.CC.Add("essraa.soliman@wfp.org"); //travel unit
                            //mailhou.CC.Add("rbc.management@wfp.org"); //RD/DRD Office
                            mailhou1.Bcc.Add(odcitcc); //travel unit
                            mailhou1.From = new MailAddress("RBC-SMS-SYSTEM@wfp.org");
                            mailhou1.Subject = "For CD Approval";

                            string Bodyhou1 = "<table><tr><td colspan='2'>" + "<hr/>New Mission<hr/></td></tr><tr><td><font color='blue'>Request : </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorization11.MissionID + "</td></tr>"
                                + "<tr><td>" + "<font color='blue'>From :</font></td><td>" + staffs.First().staff_first_name + " " + staffs.First().staff_last_name + "</td></tr>"
                                + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                                + "<tr><td><font color='blue'>From :</font></td><td>" + missionAuthorization11.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                                + "<tr><td><font color='blue'>To :</font></td><td>" + missionAuthorization11.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                                + "<tr><td><font color='blue'>Funding :</font></td><td>" + missionAuthorization11.funding + "</td></tr>"
                                + "<tr><td><font color='blue'>Objective :</font></td><td>" + missionAuthorization11.MissionObjective + "</td></tr>"
                                + "<tr><td><font color='blue'>Mission Itinerary :</font></td><td>" + missionAuthorization11.MissionItinerary + "</td></tr>"
                                + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video ? :</font></td><td>" + canbedone + "</td></tr>"
                                + "<tr><td><font color='blue'>Cleared By Supervisor :</font></td><td>" + CleredBySupervisor + "</td></tr>"
                                + "<tr><td><font color='blue'>International Roaming :</font></td><td>" + Roming + "</td></tr>"
                                + "<tr><td><font color='blue'>HOU Comment :</font></td><td>" + missionAuthorization11.Comment1 + "</td></tr>"
                                + "<tr><td><font color='blue'>Security Comment :</font></td><td>" + missionAuthorization11.Comment2 + "</td></tr>"
                                + "<tr><td><font color='blue'>Finance Comment :</font></td><td>" + missionAuthorization11.Comment3 + "</td></tr>"
                                + "<tr><td><font color='blue'>	BSITF :</font></td><td>" + BSITF2 + "</td></tr>"
                                + "<tr><td><font color='blue'> ASITF :</font></td><td>" + ASITF2 + "</td></tr>"
                                + "<tr><td><font color='blue'> SSAFE :</font></td><td>" + SSAFE2 + "</td></tr>"
                                + "<tr><td><font color='blue'> DSS Approval:</font></td><td><a href='http://10.11.135.254:8080/RBC-SMS/UploadedFiles/" + missionAuthorization11.non6 + "'>" + missionAuthorization11.non6

                           + "<tr><td><a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorization1/Approval2CD?MissionID2="
                            + missionAuthorization11.MissionID
                            + "&RDORDRDSignature2=Yes&FromDate2=" + missionAuthorization11.FromDate
                            + "&ToDate2=" + missionAuthorization11.ToDate
                            + "&to2=" + non10 + "&staffmail2=" + staffs.First().staff_email
                            + "'>Accept</a></td><td><a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorization1/Approval2CD?MissionID2=" + missionAuthorization11.MissionID
                            + "&RDORDRDSignature2=NO & FromDate2=" + missionAuthorization11.FromDate
                            + "&ToDate2=" + missionAuthorization11.ToDate
                            + "&to2=" + non10
                            + "&staffmail2=" + staffs.First().staff_email + "'>Reject</a>" + "</a></td></tr></table>";


                            //string Body = "Request : Mission &nbsp;  &nbsp;  &nbsp; &nbsp;  &nbsp; &nbsp; &nbsp; &nbsp;  Mission ID : " + missionAuthorization.MissionID + " <br> " + " From &nbsp; &nbsp; &nbsp;:" + staffs.First().staff_first_name + "&nbsp;" + staffs.First().staff_last_name + "<br><br>" + " Mission Information" + "<br><br>" + " From :" + missionAuthorization.FromDate + "<br>To :" + missionAuthorization.ToDate +
                            //    "<br>Funding :" + missionAuthorization.funding + "<br>Objective :" + missionAuthorization.MissionObjective + "<br> Mission Itinerary :" + missionAuthorization.MissionItinerary + "<br> Can This meeting Be Done Over Voice/Video ? :" + missionAuthorization.canbedone  + " <br><br> <a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor=" + true + "&FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + " &to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Acept</a>&nbsp;&nbsp;&nbsp;<a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor= " + false + "& FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + "&to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Reject</a> ";

                            mailhou1.Body = Bodyhou1;
                            mailhou1.IsBodyHtml = true;
                            SmtpClient smtphou1 = new SmtpClient();
                            smtphou1.Host = "smtprelay.global.wfp.org";
                            smtphou1.Port = 25;
                            smtphou1.UseDefaultCredentials = true;
                            //smtp.Credentials = new System.Net.NetworkCredential
                            //("ahmed.badr", "Survivor2323");// Enter seders User name and password  
                            //smtp.EnableSsl = true;
                            smtphou1.Send(mailhou1);

                            ///////////////////////////// CD Approval end //////////////////////

                            return RedirectToAction("Index");


                        }
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
                    return View();
                }
                else
                {
                    //var missionAuthorizationn = db.MissionAuthorizations.Single(u => u.MissionID == Mid);
                    //db.MissionAuthorizations.Remove(missionAuthorizationn);
                    //db.SaveChanges();
                    var missionAuthorization12 = db.MissionAuthorization1.Single(u => u.MissionID == MissionID);
                    missionAuthorization12.Signature3 = false;
                    missionAuthorization12.Signature3Date = DateTime.Now;
                    missionAuthorization12.Comment3 = Comment3;
                    if (funding != "")
                    {
                        missionAuthorization12.funding = funding;
                    }
                    db.SaveChanges();



                    var staffss = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                    staffss = staffss.Where(s => s.staffid == onbehalfID);
                    var too = staffss.First().staff_email;
                    var staffsupervisor = staffss.First().staff_supervisorid;
                    var supers = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                    supers = supers.Where(d => d.staffid == staffsupervisor);
                    var from = supers.First().staff_email;

                    //var rd = db.staffs.Include(r => r.contract_details).Include(r => r.contract_type).Include(r => r.Country_office).Include(r => r.functional_title).Include(r => r.staff2).Include(r => r.sub_office).Include(r => r.unit).Include(r => r.country).Include(r => r.governorate);
                    //rd = rd.Where(r => r.functional_title.functional_title_description_english == "REGIONAL DIRECTOR");
                    //var rdmail = rd.First().staff_email;

                    //var drd = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                    //drd = drd.Where(s => s.functional_title.functional_title_description_english == "Deputy Regional Director");
                    //var drdmail = drd.First().staff_email;

                    //////////////////////////////////////////////////////////////////////////////////////


                    var Sonbehafemail = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                    Sonbehafemail = Sonbehafemail.Where(d => d.staffid == onbehalfID);
                    var onbehalfemail = Sonbehafemail.First().staff_email;


                    var supersonbehalf = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                    supersonbehalf = supersonbehalf.Where(d => d.staffid == onbehalfSuperID);
                    var onbehafsuperemail = supersonbehalf.First().staff_email;

                    /////////////////////////////////////////////////////////////////////////////////////
                    //new
                    var userUnit = staffss.First().unit_id;
                    var unithou = db.units.Include(d => d.staffs);
                    unithou = unithou.Where(u => u.unitid == userUnit);
                    var houid = unithou.First().HOU_ID;

                    var staffshou = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                    staffshou = staffshou.Where(h => h.staffid == houid);
                    var hou = staffshou.First().staff_email;
                    //var houname = staffshou.First().staff_first_name + " " + staffshou.First().staff_last_name;

                    var cc = supers.First().staff_email;
                    var supername = supers.First().staff_first_name + " " + supers.First().staff_last_name;

                    //
                    //
                    //hou email
                    if (onbehalfID == houid)
                    {
                        var canbedone = "";
                        if (missionAuthorization12.canbedone == true)
                        {
                            canbedone = "Yes";
                        }
                        else
                        {
                            canbedone = "No";
                        }

                        var CleredBySupervisor = "";
                        if (missionAuthorization12.canbedone == true)
                        {
                            CleredBySupervisor = "Yes";
                        }
                        else
                        {
                            CleredBySupervisor = "No";
                        }

                        var BSITF2 = "";
                        if (missionAuthorization12.BSITF == true)
                        {
                            BSITF2 = "Yes";
                        }
                        else
                        {
                            BSITF2 = "No";
                        }
                        ////////  
                        var ASITF2 = "";
                        if (missionAuthorization12.ASITF == true)
                        {
                            ASITF2 = "Yes";
                        }
                        else
                        {
                            ASITF2 = "No";
                        }
                        ////////
                        var SSAFE2 = "";
                        if (missionAuthorization12.SSAFE == true)
                        {
                            SSAFE2 = "Yes";
                        }
                        else
                        {
                            SSAFE2 = "No";
                        }

                        //////////////////////////////


                        var supersforhou = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                        supersforhou = supersforhou.Where(d => d.staffid == staffsupervisor);
                        var superforhounitsname = supersforhou.First().staff_first_name + " " + staffshou.First().staff_last_name;
                        var houmailUPDATE = supersforhou.First().staff_email;

                        if (houmailUPDATE == "muhannad.hadi@wfp.org" || houmailUPDATE == "carlo.scaramella@wfp.org")
                        {
                            houmailUPDATE = "rbc.management@wfp.org"; //rbc.management@wfp.org

                        }
                        /////////////////////////

                        MailMessage mailhou = new MailMessage();
                        mailhou.To.Add(onbehalfemail);
                        mailhou.Bcc.Add("ahmed.badr@wfp.org");
                        //mail2.CC.Add(cc);
                        mailhou.From = new MailAddress("RBC-SMS-SYSTEM@wfp.org");
                        mailhou.Subject = "Finance Not Cleared";

                        string Bodyhou = "<table><tr><td colspan='2'>" + "<hr/>New Mission<hr/></td></tr><tr><td><font color='blue'>Request : </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorization12.MissionID + "</td></tr>"
                            + "<tr><td>" + "<font color='blue'>From :</font></td><td>" + staffss.First().staff_first_name + " " + staffss.First().staff_last_name + "</td></tr>"
                            + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                            + "<tr><td><font color='blue'>From :</font></td><td>" + missionAuthorization12.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                            + "<tr><td><font color='blue'>To :</font></td><td>" + missionAuthorization12.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                            + "<tr><td><font color='blue'>Funding :</font></td><td>" + missionAuthorization12.funding + "</td></tr>"
                            + "<tr><td><font color='blue'>Objective :</font></td><td>" + missionAuthorization12.MissionObjective + "</td></tr>"
                            + "<tr><td><font color='blue'>Mission Itinerary :</font></td><td>" + missionAuthorization12.MissionItinerary + "</td></tr>"
                            + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video ? :</font></td><td>" + canbedone + "</td></tr>"
                            + "<tr><td><font color='blue'>Cleared By Supervisor :</font></td><td>" + CleredBySupervisor + "</td></tr>"
                             + "<tr><td><font color='blue'>HOU Comment :</font></td><td>" + missionAuthorization12.Comment1 + "</td></tr>"
                             + "<tr><td><font color='blue'>Security Comment :</font></td><td>" + missionAuthorization12.Comment2 + "</td></tr>"
                             + "<tr><td><font color='blue'>Finance Comment :</font></td><td>" + missionAuthorization12.Comment3 + "</td></tr>"
                            + "<tr><td><font color='blue'>	BSITF :</font></td><td>" + BSITF2 + "</td></tr>"
                            + "<tr><td><font color='blue'> ASITF :</font></td><td>" + ASITF2 + "</td></tr>"
                            + "<tr><td><font color='blue'> SSAFE :</font></td><td>" + SSAFE2 + "</td></tr>"
                            + "<tr><td><font color='blue'> DSS Approval:</font></td><td><a href='http://10.11.135.254:8080/RBC-SMS/UploadedFiles/" + missionAuthorization12.non6 + "'>" + missionAuthorization12.non6 + "</a></td></tr></table>";

                        //string Body = "Request : Mission &nbsp;  &nbsp;  &nbsp; &nbsp;  &nbsp; &nbsp; &nbsp; &nbsp;  Mission ID : " + missionAuthorization.MissionID + " <br> " + " From &nbsp; &nbsp; &nbsp;:" + staffs.First().staff_first_name + "&nbsp;" + staffs.First().staff_last_name + "<br><br>" + " Mission Information" + "<br><br>" + " From :" + missionAuthorization.FromDate + "<br>To :" + missionAuthorization.ToDate +
                        //    "<br>Funding :" + missionAuthorization.funding + "<br>Objective :" + missionAuthorization.MissionObjective + "<br> Mission Itinerary :" + missionAuthorization.MissionItinerary + "<br> Can This meeting Be Done Over Voice/Video ? :" + missionAuthorization.canbedone  + " <br><br> <a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor=" + true + "&FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + " &to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Acept</a>&nbsp;&nbsp;&nbsp;<a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor= " + false + "& FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + "&to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Reject</a> ";

                        mailhou.Body = Bodyhou;
                        mailhou.IsBodyHtml = true;
                        SmtpClient smtphou = new SmtpClient();
                        smtphou.Host = "smtprelay.global.wfp.org";
                        smtphou.Port = 25;
                        smtphou.UseDefaultCredentials = true;
                        //smtp.Credentials = new System.Net.NetworkCredential
                        //("ahmed.badr", "Survivor2323");// Enter seders User name and password  
                        //smtp.EnableSsl = true;
                        smtphou.Send(mailhou);
                        return RedirectToAction("Index");
                    }

                    else
                    {
                        var canbedone = "";
                        if (missionAuthorization12.canbedone == true)
                        {
                            canbedone = "Yes";
                        }
                        else
                        {
                            canbedone = "No";
                        }

                        var CleredBySupervisor = "";
                        if (missionAuthorization12.canbedone == true)
                        {
                            CleredBySupervisor = "Yes";
                        }
                        else
                        {
                            CleredBySupervisor = "No";
                        }

                        var BSITF2 = "";
                        if (missionAuthorization12.BSITF == true)
                        {
                            BSITF2 = "Yes";
                        }
                        else
                        {
                            BSITF2 = "No";
                        }
                        ////////  
                        var ASITF2 = "";
                        if (missionAuthorization12.ASITF == true)
                        {
                            ASITF2 = "Yes";
                        }
                        else
                        {
                            ASITF2 = "No";
                        }
                        ////////
                        var SSAFE2 = "";
                        if (missionAuthorization12.SSAFE == true)
                        {
                            SSAFE2 = "Yes";
                        }
                        else
                        {
                            SSAFE2 = "No";
                        }

                        //////////////////////////////


                        var supersforhou = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                        supersforhou = supersforhou.Where(d => d.staffid == staffsupervisor);
                        var superforhounitsname = supersforhou.First().staff_first_name + " " + staffshou.First().staff_last_name;
                        var houmailUPDATE = supersforhou.First().staff_email;

                        if (houmailUPDATE == "muhannad.hadi@wfp.org" || houmailUPDATE == "carlo.scaramella@wfp.org")
                        {
                            houmailUPDATE = "rbc.management@wfp.org"; //rbc.management@wfp.org

                        }
                        /////////////////////////

                        MailMessage mailhou = new MailMessage();
                        mailhou.To.Add(onbehalfemail);
                        mailhou.Bcc.Add("ahmed.badr@wfp.org");
                        mailhou.CC.Add(onbehafsuperemail);
                        mailhou.From = new MailAddress(hou);
                        mailhou.Subject = "Finance Not Cleared";

                        string Bodyhou = "<table><tr><td colspan='2'>" + "<hr/>New Mission<hr/></td></tr><tr><td><font color='blue'>Request : </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorization12.MissionID + "</td></tr>"
                            + "<tr><td>" + "<font color='blue'>From :</font></td><td>" + staffss.First().staff_first_name + " " + staffss.First().staff_last_name + "</td></tr>"
                            + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                            + "<tr><td><font color='blue'>From :</font></td><td>" + missionAuthorization12.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                            + "<tr><td><font color='blue'>To :</font></td><td>" + missionAuthorization12.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                            + "<tr><td><font color='blue'>Funding :</font></td><td>" + missionAuthorization12.funding + "</td></tr>"
                            + "<tr><td><font color='blue'>Objective :</font></td><td>" + missionAuthorization12.MissionObjective + "</td></tr>"
                            + "<tr><td><font color='blue'>Mission Itinerary :</font></td><td>" + missionAuthorization12.MissionItinerary + "</td></tr>"
                            + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video ? :</font></td><td>" + canbedone + "</td></tr>"
                            + "<tr><td><font color='blue'>Cleared By Supervisor :</font></td><td>" + CleredBySupervisor + "</td></tr>"
                             + "<tr><td><font color='blue'>HOU Comment :</font></td><td>" + missionAuthorization12.Comment1 + "</td></tr>"
                             + "<tr><td><font color='blue'>HOU Comment :</font></td><td>" + missionAuthorization12.Comment2 + "</td></tr>"
                             + "<tr><td><font color='blue'>HOU Comment :</font></td><td>" + missionAuthorization12.Comment3 + "</td></tr>"
                            + "<tr><td><font color='blue'>	BSITF :</font></td><td>" + BSITF2 + "</td></tr>"
                            + "<tr><td><font color='blue'> ASITF :</font></td><td>" + ASITF2 + "</td></tr>"
                            + "<tr><td><font color='blue'> SSAFE :</font></td><td>" + SSAFE2 + "</td></tr>"
                           + "<tr><td><font color='blue'> DSS Approval:</font></td><td><a href='http://10.11.135.254:8080/RBC-SMS/UploadedFiles/" + missionAuthorization12.non6 + "'>" + missionAuthorization12.non6 + "</a></td></tr></table>";

                        //string Body = "Request : Mission &nbsp;  &nbsp;  &nbsp; &nbsp;  &nbsp; &nbsp; &nbsp; &nbsp;  Mission ID : " + missionAuthorization.MissionID + " <br> " + " From &nbsp; &nbsp; &nbsp;:" + staffs.First().staff_first_name + "&nbsp;" + staffs.First().staff_last_name + "<br><br>" + " Mission Information" + "<br><br>" + " From :" + missionAuthorization.FromDate + "<br>To :" + missionAuthorization.ToDate +
                        //    "<br>Funding :" + missionAuthorization.funding + "<br>Objective :" + missionAuthorization.MissionObjective + "<br> Mission Itinerary :" + missionAuthorization.MissionItinerary + "<br> Can This meeting Be Done Over Voice/Video ? :" + missionAuthorization.canbedone  + " <br><br> <a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor=" + true + "&FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + " &to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Acept</a>&nbsp;&nbsp;&nbsp;<a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor= " + false + "& FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + "&to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Reject</a> ";

                        mailhou.Body = Bodyhou;
                        mailhou.IsBodyHtml = true;
                        SmtpClient smtphou = new SmtpClient();
                        smtphou.Host = "smtprelay.global.wfp.org";
                        smtphou.Port = 25;
                        smtphou.UseDefaultCredentials = true;
                        //smtp.Credentials = new System.Net.NetworkCredential
                        //("ahmed.badr", "Survivor2323");// Enter seders User name and password  
                        //smtp.EnableSsl = true;
                        smtphou.Send(mailhou);
                        return RedirectToAction("Index");


                    }
                }



            }
            ///////////////////////// end on behalf 
            else
            {
                ViewBag.Mid = Request.QueryString["MissionID"];
                ViewBag.RDORDRDSignature = Request.QueryString["RDORDRDSignature"];
                ViewBag.FromDate = Request.QueryString["FromDate"];
                ViewBag.ToDate = Request.QueryString["ToDate"];
                if (RDORDRDSignature == "Yes")
                {
                    try
                    {
                        // missionAuthorization = db.MissionAuthorizations.Find(Mid);
                        //db.Entry(missionAuthorization).State = EntityState.Modified ;
                        //db.SaveChanges();

                        var missionAuthorization = db.MissionAuthorization1.Single(u => u.MissionID == MissionID);
                        missionAuthorization.Signature3 = true;
                        missionAuthorization.Signature3Date = DateTime.Now;
                        missionAuthorization.Comment3 = Comment3;
                        if (funding != "")
                        { 
                        missionAuthorization.funding = funding;
                        }
                        db.SaveChanges();



                        var staffs = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                        staffs = staffs.Where(s => s.staff_email.Contains(staff));
                        var too = staffs.First().staff_email;
                        var staffsupervisor = staffs.First().staff_supervisorid;
                        var supers = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                        supers = supers.Where(d => d.staffid == staffsupervisor);
                        var from = supers.First().staff_email;

                        //var rd = db.staffs.Include(r => r.contract_details).Include(r => r.contract_type).Include(r => r.Country_office).Include(r => r.functional_title).Include(r => r.staff2).Include(r => r.sub_office).Include(r => r.unit).Include(r => r.country).Include(r => r.governorate);
                        //rd = rd.Where(r => r.functional_title.functional_title_description_english == "REGIONAL DIRECTOR");
                        //var rdmail = rd.First().staff_email;

                        //var drd = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                        //drd = drd.Where(s => s.functional_title.functional_title_description_english == "Deputy Regional Director");
                        //var drdmail = drd.First().staff_email;

                        var userUnit = staffs.First().unit_id;
                        var unithou = db.units.Include(d => d.staffs);
                        unithou = unithou.Where(u => u.unitid == userUnit);
                        var houid = unithou.First().HOU_ID;

                        var staffshou = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                        staffshou = staffshou.Where(h => h.staffid == houid);
                        var hou = staffshou.First().staff_email;
                        //var houname = staffshou.First().staff_first_name + " " + staffshou.First().staff_last_name;

                        var cc = supers.First().staff_email;
                        var supername = supers.First().staff_first_name + " " + supers.First().staff_last_name;
                        var superemail = supers.First().staff_email;

                        //
                        //
                        //hou email
                        if (staffs.First().staffid == houid)
                        {
                            var canbedone = "";
                            if (missionAuthorization.canbedone == true)
                            {
                                canbedone = "Yes";
                            }
                            else
                            {
                                canbedone = "No";
                            }

                            var CleredBySupervisor = "";
                            if (missionAuthorization.canbedone == true)
                            {
                                CleredBySupervisor = "Yes";
                            }
                            else
                            {
                                CleredBySupervisor = "No";
                            }

                            var BSITF2 = "";
                            if (missionAuthorization.BSITF == true)
                            {
                                BSITF2 = "Yes";
                            }
                            else
                            {
                                BSITF2 = "No";
                            }
                            ////////  
                            var ASITF2 = "";
                            if (missionAuthorization.ASITF == true)
                            {
                                ASITF2 = "Yes";
                            }
                            else
                            {
                                ASITF2 = "No";
                            }
                            ////////
                            var SSAFE2 = "";
                            if (missionAuthorization.SSAFE == true)
                            {
                                SSAFE2 = "Yes";
                            }
                            else
                            {
                                SSAFE2 = "No";
                            }
                            var Roming = "";
                            if (missionAuthorization.StaffSignature == "True")
                            {
                                Roming = "Yes";
                            }
                            else
                            {
                                Roming = "No";
                            }
                            var odcitcc = "";
                            if (Roming == "Yes")
                            {
                                odcitcc = "cairo.itservicedesk@wfp.org";
                            }
                            else
                            {
                                odcitcc = "ahmed.badr@wfp.org";
                            }
                            //////////////////////////////


                            var supersforhou = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                            supersforhou = supersforhou.Where(d => d.staffid == staffsupervisor);
                            var superforhounitsname = supersforhou.First().staff_first_name + " " + staffshou.First().staff_last_name;
                            var houmailUPDATE = supersforhou.First().staff_email;

                            if (houmailUPDATE == "muhannad.hadi@wfp.org" || houmailUPDATE == "carlo.scaramella@wfp.org")
                            {
                                houmailUPDATE = "rbc.management@wfp.org"; //rbc.management@wfp.org

                            }
                            /////////////////////////

                            MailMessage mailhou = new MailMessage();
                            mailhou.To.Add(staff);
                            mailhou.Bcc.Add("ahmed.badr@wfp.org");
                            mailhou.Bcc.Add(odcitcc); //travel unit
                            //mailhou.CC.Add("rbc.travel@wfp.org"); //travel unit
                            //mailhou.CC.Add("mai.kenawi@wfp.org"); //travel unit
                            //mailhou.CC.Add("amal.mohamed@wfp.org"); //travel unit
                            //mailhou.CC.Add("ayten.el-sheikh@wfp.org"); //travel unit
                            //mailhou.CC.Add("essraa.soliman@wfp.org"); //travel unit
                            //mailhou.CC.Add("rbc.management@wfp.org"); //RD/DRD Office
                            mailhou.From = new MailAddress("RBC-SMS-SYSTEM@wfp.org");
                            mailhou.Subject = "Finance Cleared";

                            string Bodyhou = "<table><tr><td colspan='2'>" + "<hr/>New Mission<hr/></td></tr><tr><td><font color='blue'>Request : </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorization.MissionID + "</td></tr>"
                                + "<tr><td>" + "<font color='blue'>From :</font></td><td>" + staffs.First().staff_first_name + " " + staffs.First().staff_last_name + "</td></tr>"
                                + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                                + "<tr><td><font color='blue'>From :</font></td><td>" + missionAuthorization.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                                + "<tr><td><font color='blue'>To :</font></td><td>" + missionAuthorization.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                                + "<tr><td><font color='blue'>Funding :</font></td><td>" + missionAuthorization.funding + "</td></tr>"
                                + "<tr><td><font color='blue'>Objective :</font></td><td>" + missionAuthorization.MissionObjective + "</td></tr>"
                                + "<tr><td><font color='blue'>Mission Itinerary :</font></td><td>" + missionAuthorization.MissionItinerary + "</td></tr>"
                                + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video ? :</font></td><td>" + canbedone + "</td></tr>"
                                + "<tr><td><font color='blue'>Cleared By Supervisor :</font></td><td>" + CleredBySupervisor + "</td></tr>"
                                + "<tr><td><font color='blue'>International Roaming :</font></td><td>" + Roming + "</td></tr>"
                                 + "<tr><td><font color='blue'>HOU Comment :</font></td><td>" + missionAuthorization.Comment1 + "</td></tr>"
                                  + "<tr><td><font color='blue'>Security Comment :</font></td><td>" + missionAuthorization.Comment2 + "</td></tr>"
                                   + "<tr><td><font color='blue'>Finance Comment :</font></td><td>" + missionAuthorization.Comment3 + "</td></tr>"
                                + "<tr><td><font color='blue'>	BSITF :</font></td><td>" + BSITF2 + "</td></tr>"
                                + "<tr><td><font color='blue'> ASITF :</font></td><td>" + ASITF2 + "</td></tr>"
                                + "<tr><td><font color='blue'> SSAFE :</font></td><td>" + SSAFE2 + "</td></tr>"
                               + "<tr><td><font color='blue'> DSS Approval:</font></td><td><a target='_blank' href='http://10.11.135.254:8080/RBC-SMS/UploadedFiles/" + missionAuthorization.non6 + "'>" + missionAuthorization.non6 + "</a></td></tr></table>";

                            //string Body = "Request : Mission &nbsp;  &nbsp;  &nbsp; &nbsp;  &nbsp; &nbsp; &nbsp; &nbsp;  Mission ID : " + missionAuthorization.MissionID + " <br> " + " From &nbsp; &nbsp; &nbsp;:" + staffs.First().staff_first_name + "&nbsp;" + staffs.First().staff_last_name + "<br><br>" + " Mission Information" + "<br><br>" + " From :" + missionAuthorization.FromDate + "<br>To :" + missionAuthorization.ToDate +
                            //    "<br>Funding :" + missionAuthorization.funding + "<br>Objective :" + missionAuthorization.MissionObjective + "<br> Mission Itinerary :" + missionAuthorization.MissionItinerary + "<br> Can This meeting Be Done Over Voice/Video ? :" + missionAuthorization.canbedone  + " <br><br> <a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor=" + true + "&FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + " &to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Acept</a>&nbsp;&nbsp;&nbsp;<a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor= " + false + "& FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + "&to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Reject</a> ";

                            mailhou.Body = Bodyhou;
                            mailhou.IsBodyHtml = true;
                            SmtpClient smtphou = new SmtpClient();
                            smtphou.Host = "smtprelay.global.wfp.org";
                            smtphou.Port = 25;
                            smtphou.UseDefaultCredentials = true;
                            //smtp.Credentials = new System.Net.NetworkCredential
                            //("ahmed.badr", "Survivor2323");// Enter seders User name and password  
                            //smtp.EnableSsl = true;
                            smtphou.Send(mailhou);


                            ///////////////////////////// CD  Approval //////////////////////

                            MailMessage mailhou1 = new MailMessage();
                            mailhou1.To.Add("ahmed.badr@wfp.org");
                            mailhou1.Bcc.Add("ahmed.badr@wfp.org");

                            //mailhou.CC.Add("rbc.travel@wfp.org"); //travel unit
                            //mailhou.CC.Add("mai.kenawi@wfp.org"); //travel unit
                            //mailhou.CC.Add("amal.mohamed@wfp.org"); //travel unit
                            //mailhou.CC.Add("ayten.el-sheikh@wfp.org"); //travel unit
                            //mailhou.CC.Add("essraa.soliman@wfp.org"); //travel unit
                            //mailhou.CC.Add("rbc.management@wfp.org"); //RD/DRD Office
                            mailhou1.Bcc.Add(odcitcc); //travel unit
                            mailhou1.From = new MailAddress("RBC-SMS-SYSTEM@wfp.org");
                            mailhou1.Subject = "For CD Approval";

                            string Bodyhou1 = "<table><tr><td colspan='2'>" + "<hr/>New Mission<hr/></td></tr><tr><td><font color='blue'>Request : </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorization.MissionID + "</td></tr>"
                                + "<tr><td>" + "<font color='blue'>From :</font></td><td>" + staffs.First().staff_first_name + " " + staffs.First().staff_last_name + "</td></tr>"
                                + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                                + "<tr><td><font color='blue'>From :</font></td><td>" + missionAuthorization.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                                + "<tr><td><font color='blue'>To :</font></td><td>" + missionAuthorization.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                                + "<tr><td><font color='blue'>Funding :</font></td><td>" + missionAuthorization.funding + "</td></tr>"
                                + "<tr><td><font color='blue'>Objective :</font></td><td>" + missionAuthorization.MissionObjective + "</td></tr>"
                                + "<tr><td><font color='blue'>Mission Itinerary :</font></td><td>" + missionAuthorization.MissionItinerary + "</td></tr>"
                                + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video ? :</font></td><td>" + canbedone + "</td></tr>"
                                + "<tr><td><font color='blue'>Cleared By Supervisor :</font></td><td>" + CleredBySupervisor + "</td></tr>"
                                + "<tr><td><font color='blue'>International Roaming :</font></td><td>" + Roming + "</td></tr>"
                                + "<tr><td><font color='blue'>HOU Comment :</font></td><td>" + missionAuthorization.Comment1 + "</td></tr>"
                                 + "<tr><td><font color='blue'>Security Comment :</font></td><td>" + missionAuthorization.Comment2 + "</td></tr>"
                                  + "<tr><td><font color='blue'>Finance Comment :</font></td><td>" + missionAuthorization.Comment3 + "</td></tr>"
                                + "<tr><td><font color='blue'>	BSITF :</font></td><td>" + BSITF2 + "</td></tr>"
                                + "<tr><td><font color='blue'> ASITF :</font></td><td>" + ASITF2 + "</td></tr>"
                                + "<tr><td><font color='blue'> SSAFE :</font></td><td>" + SSAFE2 + "</td></tr>"
                                + "<tr><td><font color='blue'> DSS Approval:</font></td><td><a target='_blank' href='http://10.11.135.254:8080/RBC-SMS/UploadedFiles/" + missionAuthorization.non6 + "'>" + missionAuthorization.non6

                             + "<tr><td><a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorization1/Approval2CD?MissionID2="
                            + missionAuthorization.MissionID
                            + "&RDORDRDSignature2=Yes&FromDate2=" + missionAuthorization.FromDate
                            + "&ToDate2=" + missionAuthorization.ToDate
                            + "&to2=" + non10 + "&staffmail2=" + staffs.First().staff_email
                            + "'>Accept</a></td><td><a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorization1/Approval2CD?MissionID2=" + missionAuthorization.MissionID
                            + "&RDORDRDSignature2=NO & FromDate2=" + missionAuthorization.FromDate
                            + "&ToDate2=" + missionAuthorization.ToDate
                            + "&to2=" + non10
                            + "&staffmail2=" + staffs.First().staff_email + "'>Reject</a>" + "</a></td></tr></table>";

                            //string Body = "Request : Mission &nbsp;  &nbsp;  &nbsp; &nbsp;  &nbsp; &nbsp; &nbsp; &nbsp;  Mission ID : " + missionAuthorization.MissionID + " <br> " + " From &nbsp; &nbsp; &nbsp;:" + staffs.First().staff_first_name + "&nbsp;" + staffs.First().staff_last_name + "<br><br>" + " Mission Information" + "<br><br>" + " From :" + missionAuthorization.FromDate + "<br>To :" + missionAuthorization.ToDate +
                            //    "<br>Funding :" + missionAuthorization.funding + "<br>Objective :" + missionAuthorization.MissionObjective + "<br> Mission Itinerary :" + missionAuthorization.MissionItinerary + "<br> Can This meeting Be Done Over Voice/Video ? :" + missionAuthorization.canbedone  + " <br><br> <a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor=" + true + "&FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + " &to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Acept</a>&nbsp;&nbsp;&nbsp;<a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor= " + false + "& FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + "&to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Reject</a> ";

                            mailhou1.Body = Bodyhou1;
                            mailhou1.IsBodyHtml = true;
                            SmtpClient smtphou1 = new SmtpClient();
                            smtphou1.Host = "smtprelay.global.wfp.org";
                            smtphou1.Port = 25;
                            smtphou1.UseDefaultCredentials = true;
                            //smtp.Credentials = new System.Net.NetworkCredential
                            //("ahmed.badr", "Survivor2323");// Enter seders User name and password  
                            //smtp.EnableSsl = true;
                            smtphou1.Send(mailhou1);

                            ///////////////////////////// CD Approval end //////////////////////

                            return RedirectToAction("Index");
                        }

                        else
                        {
                            var canbedone = "";
                            if (missionAuthorization.canbedone == true)
                            {
                                canbedone = "Yes";
                            }
                            else
                            {
                                canbedone = "No";
                            }

                            var CleredBySupervisor = "";
                            if (missionAuthorization.canbedone == true)
                            {
                                CleredBySupervisor = "Yes";
                            }
                            else
                            {
                                CleredBySupervisor = "No";
                            }

                            var BSITF2 = "";
                            if (missionAuthorization.BSITF == true)
                            {
                                BSITF2 = "Yes";
                            }
                            else
                            {
                                BSITF2 = "No";
                            }
                            ////////  
                            var ASITF2 = "";
                            if (missionAuthorization.ASITF == true)
                            {
                                ASITF2 = "Yes";
                            }
                            else
                            {
                                ASITF2 = "No";
                            }
                            ////////
                            var SSAFE2 = "";
                            if (missionAuthorization.SSAFE == true)
                            {
                                SSAFE2 = "Yes";
                            }
                            else
                            {
                                SSAFE2 = "No";
                            }

                            //////////////////////////////


                            var supersforhou = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                            supersforhou = supersforhou.Where(d => d.staffid == staffsupervisor);
                            var superforhounitsname = supersforhou.First().staff_first_name + " " + staffshou.First().staff_last_name;
                            var houmailUPDATE = supersforhou.First().staff_email;

                            if (houmailUPDATE == "muhannad.hadi@wfp.org" || houmailUPDATE == "carlo.scaramella@wfp.org")
                            {
                                houmailUPDATE = "rbc.management@wfp.org"; //rbc.management@wfp.org

                            }
                            var odcitcc = "";

                            var Roming = "";
                            if (missionAuthorization.StaffSignature == "True")
                            {
                                Roming = "Yes";
                            }
                            else
                            {
                                Roming = "No";
                            }
                            if (Roming == "Yes")
                            {
                                odcitcc = "cairo.itservicedesk@wfp.org";
                            }
                            else
                            {
                                odcitcc = "ahmed.badr@wfp.org";
                            }
                            /////////////////////////

                            MailMessage mailhou = new MailMessage();
                            mailhou.To.Add(staff); //staffmail
                            mailhou.Bcc.Add("ahmed.badr@wfp.org");
                            mailhou.Bcc.Add(odcitcc); //travel unit
                            mailhou.CC.Add(superemail);
                            //mailhou.CC.Add("rbc.travel@wfp.org"); //travel unit
                            //mailhou.CC.Add("mai.kenawi@wfp.org"); //travel unit
                            //mailhou.CC.Add("amal.mohamed@wfp.org"); //travel unit
                            //mailhou.CC.Add("ayten.el-sheikh@wfp.org"); //travel unit
                            //mailhou.CC.Add("essraa.soliman@wfp.org"); //travel unit
                            //mailhou.CC.Add("rbc.management@wfp.org"); //RD/DRD Office
                            mailhou.From = new MailAddress(hou);
                            mailhou.Subject = "Finance Cleared";

                            string Bodyhou = "<table><tr><td colspan='2'>" + "<hr/>New Mission<hr/></td></tr><tr><td><font color='blue'>Request : </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorization.MissionID + "</td></tr>"
                                + "<tr><td>" + "<font color='blue'>From :</font></td><td>" + staffs.First().staff_first_name + " " + staffs.First().staff_last_name + "</td></tr>"
                                + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                                + "<tr><td><font color='blue'>From :</font></td><td>" + missionAuthorization.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                                + "<tr><td><font color='blue'>To :</font></td><td>" + missionAuthorization.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                                + "<tr><td><font color='blue'>Funding :</font></td><td>" + missionAuthorization.funding + "</td></tr>"
                                + "<tr><td><font color='blue'>Objective :</font></td><td>" + missionAuthorization.MissionObjective + "</td></tr>"
                                + "<tr><td><font color='blue'>Mission Itinerary :</font></td><td>" + missionAuthorization.MissionItinerary + "</td></tr>"
                                + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video ? :</font></td><td>" + canbedone + "</td></tr>"
                                + "<tr><td><font color='blue'>Cleared By Supervisor :</font></td><td>" + CleredBySupervisor + "</td></tr>"
                                + "<tr><td><font color='blue'>International Roaming :</font></td><td>" + Roming + "</td></tr>"
                                 + "<tr><td><font color='blue'>HOU Comment :</font></td><td>" + missionAuthorization.Comment1 + "</td></tr>"
                                  + "<tr><td><font color='blue'>Security Comment :</font></td><td>" + missionAuthorization.Comment2 + "</td></tr>"
                                   + "<tr><td><font color='blue'>Finance Comment :</font></td><td>" + missionAuthorization.Comment3 + "</td></tr>"
                                + "<tr><td><font color='blue'>	BSITF :</font></td><td>" + BSITF2 + "</td></tr>"
                                + "<tr><td><font color='blue'> ASITF :</font></td><td>" + ASITF2 + "</td></tr>"
                                + "<tr><td><font color='blue'> SSAFE :</font></td><td>" + SSAFE2 + "</td></tr>"
                               + "<tr><td><font color='blue'> DSS Approval:</font></td><td><a target='_blank' href='http://10.11.135.254:8080/RBC-SMS/UploadedFiles/" + missionAuthorization.non6 + "'>" + missionAuthorization.non6 + "</a></td></tr></table>";

                            //string Body = "Request : Mission &nbsp;  &nbsp;  &nbsp; &nbsp;  &nbsp; &nbsp; &nbsp; &nbsp;  Mission ID : " + missionAuthorization.MissionID + " <br> " + " From &nbsp; &nbsp; &nbsp;:" + staffs.First().staff_first_name + "&nbsp;" + staffs.First().staff_last_name + "<br><br>" + " Mission Information" + "<br><br>" + " From :" + missionAuthorization.FromDate + "<br>To :" + missionAuthorization.ToDate +
                            //    "<br>Funding :" + missionAuthorization.funding + "<br>Objective :" + missionAuthorization.MissionObjective + "<br> Mission Itinerary :" + missionAuthorization.MissionItinerary + "<br> Can This meeting Be Done Over Voice/Video ? :" + missionAuthorization.canbedone  + " <br><br> <a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor=" + true + "&FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + " &to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Acept</a>&nbsp;&nbsp;&nbsp;<a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor= " + false + "& FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + "&to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Reject</a> ";

                            mailhou.Body = Bodyhou;
                            mailhou.IsBodyHtml = true;
                            SmtpClient smtphou = new SmtpClient();
                            smtphou.Host = "smtprelay.global.wfp.org";
                            smtphou.Port = 25;
                            smtphou.UseDefaultCredentials = true;
                            //smtp.Credentials = new System.Net.NetworkCredential
                            //("ahmed.badr", "Survivor2323");// Enter seders User name and password  
                            //smtp.EnableSsl = true;
                            smtphou.Send(mailhou);

                            ///////////////////////////// CD  Approval //////////////////////

                            MailMessage mailhou1 = new MailMessage();
                            mailhou1.To.Add("ahmed.badr@wfp.org");
                            mailhou1.Bcc.Add("ahmed.badr@wfp.org");

                            //mailhou.CC.Add("rbc.travel@wfp.org"); //travel unit
                            //mailhou.CC.Add("mai.kenawi@wfp.org"); //travel unit
                            //mailhou.CC.Add("amal.mohamed@wfp.org"); //travel unit
                            //mailhou.CC.Add("ayten.el-sheikh@wfp.org"); //travel unit
                            //mailhou.CC.Add("essraa.soliman@wfp.org"); //travel unit
                            //mailhou.CC.Add("rbc.management@wfp.org"); //RD/DRD Office
                            mailhou1.Bcc.Add(odcitcc); //travel unit
                            mailhou1.From = new MailAddress("RBC-SMS-SYSTEM@WFP.ORG");
                            mailhou1.Subject = "For CD Approval";

                            string Bodyhou1 = "<table><tr><td colspan='2'>" + "<hr/>New Mission<hr/></td></tr><tr><td><font color='blue'>Request : </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorization.MissionID + "</td></tr>"
                                + "<tr><td>" + "<font color='blue'>From :</font></td><td>" + staffs.First().staff_first_name + " " + staffs.First().staff_last_name + "</td></tr>"
                                + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                                + "<tr><td><font color='blue'>From :</font></td><td>" + missionAuthorization.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                                + "<tr><td><font color='blue'>To :</font></td><td>" + missionAuthorization.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                                + "<tr><td><font color='blue'>Funding :</font></td><td>" + missionAuthorization.funding + "</td></tr>"
                                + "<tr><td><font color='blue'>Objective :</font></td><td>" + missionAuthorization.MissionObjective + "</td></tr>"
                                + "<tr><td><font color='blue'>Mission Itinerary :</font></td><td>" + missionAuthorization.MissionItinerary + "</td></tr>"
                                + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video ? :</font></td><td>" + canbedone + "</td></tr>"
                                + "<tr><td><font color='blue'>Cleared By Supervisor :</font></td><td>" + CleredBySupervisor + "</td></tr>"
                                + "<tr><td><font color='blue'>International Roaming :</font></td><td>" + Roming + "</td></tr>"
                                + "<tr><td><font color='blue'>HOU Comment :</font></td><td>" + missionAuthorization.Comment1 + "</td></tr>"
                                + "<tr><td><font color='blue'>Security Comment :</font></td><td>" + missionAuthorization.Comment2 + "</td></tr>"
                                + "<tr><td><font color='blue'>Finance Comment :</font></td><td>" + missionAuthorization.Comment3 + "</td></tr>"
                                + "<tr><td><font color='blue'>	BSITF :</font></td><td>" + BSITF2 + "</td></tr>"
                                + "<tr><td><font color='blue'> ASITF :</font></td><td>" + ASITF2 + "</td></tr>"
                                + "<tr><td><font color='blue'> SSAFE :</font></td><td>" + SSAFE2 + "</td></tr>"
                                + "<tr><td><font color='blue'> DSS Approval:</font></td><td><a target='_blank' href='http://10.11.135.254:8080/RBC-SMS/UploadedFiles/" + missionAuthorization.non6 + "'>" + missionAuthorization.non6


                                 + "<tr><td><a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorization1/Approval2CD?MissionID2="
                            + missionAuthorization.MissionID
                            + "&RDORDRDSignature2=Yes&FromDate2=" + missionAuthorization.FromDate
                            + "&ToDate2=" + missionAuthorization.ToDate
                            + "&to2=" + non10 + "&staffmail2=" + staffs.First().staff_email
                            + "'>Accept</a></td><td><a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorization1/Approval2CD?MissionID2=" + missionAuthorization.MissionID
                            + "&RDORDRDSignature2=NO & FromDate2=" + missionAuthorization.FromDate
                            + "&ToDate2=" + missionAuthorization.ToDate
                            + "&to2=" + non10
                            + "&staffmail2=" + staffs.First().staff_email + "'>Reject</a>" + "</a></td></tr></table>";


                            //string Body = "Request : Mission &nbsp;  &nbsp;  &nbsp; &nbsp;  &nbsp; &nbsp; &nbsp; &nbsp;  Mission ID : " + missionAuthorization.MissionID + " <br> " + " From &nbsp; &nbsp; &nbsp;:" + staffs.First().staff_first_name + "&nbsp;" + staffs.First().staff_last_name + "<br><br>" + " Mission Information" + "<br><br>" + " From :" + missionAuthorization.FromDate + "<br>To :" + missionAuthorization.ToDate +
                            //    "<br>Funding :" + missionAuthorization.funding + "<br>Objective :" + missionAuthorization.MissionObjective + "<br> Mission Itinerary :" + missionAuthorization.MissionItinerary + "<br> Can This meeting Be Done Over Voice/Video ? :" + missionAuthorization.canbedone  + " <br><br> <a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor=" + true + "&FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + " &to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Acept</a>&nbsp;&nbsp;&nbsp;<a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor= " + false + "& FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + "&to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Reject</a> ";

                            mailhou1.Body = Bodyhou1;
                            mailhou1.IsBodyHtml = true;
                            SmtpClient smtphou1 = new SmtpClient();
                            smtphou1.Host = "smtprelay.global.wfp.org";
                            smtphou1.Port = 25;
                            smtphou1.UseDefaultCredentials = true;
                            //smtp.Credentials = new System.Net.NetworkCredential
                            //("ahmed.badr", "Survivor2323");// Enter seders User name and password  
                            //smtp.EnableSsl = true;
                            smtphou1.Send(mailhou1);

                            ///////////////////////////// CD Approval //////////////////////

                            return RedirectToAction("Index");


                        }
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
                    return View();
                }
                else
                {
                    //var missionAuthorizationn = db.MissionAuthorizations.Single(u => u.MissionID == Mid);
                    //db.MissionAuthorizations.Remove(missionAuthorizationn);
                    //db.SaveChanges();
                    var missionAuthorizationn = db.MissionAuthorization1.Single(u => u.MissionID == MissionID);
                    missionAuthorizationn.Signature3 = false;
                    missionAuthorizationn.Signature3Date = DateTime.Now;
                    missionAuthorizationn.Comment3 = Comment3;
                    if (funding != "")
                    {
                        missionAuthorizationn.funding = funding;
                    }
                    db.SaveChanges();


                    var staffss = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                    staffss = staffss.Where(s => s.staff_email.Contains(staff));
                    var too = staffss.First().staff_email;
                    var staffsupervisor = staffss.First().staff_supervisorid;
                    var supers = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                    supers = supers.Where(d => d.staffid == staffsupervisor);
                    var from = supers.First().staff_email;

                    //var rd = db.staffs.Include(r => r.contract_details).Include(r => r.contract_type).Include(r => r.Country_office).Include(r => r.functional_title).Include(r => r.staff2).Include(r => r.sub_office).Include(r => r.unit).Include(r => r.country).Include(r => r.governorate);
                    //rd = rd.Where(r => r.functional_title.functional_title_description_english == "REGIONAL DIRECTOR");
                    //var rdmail = rd.First().staff_email;

                    //var drd = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                    //drd = drd.Where(s => s.functional_title.functional_title_description_english == "Deputy Regional Director");
                    //var drdmail = drd.First().staff_email;


                    //new
                    var userUnit = staffss.First().unit_id;
                    var unithou = db.units.Include(d => d.staffs);
                    unithou = unithou.Where(u => u.unitid == userUnit);
                    var houid = unithou.First().HOU_ID;

                    var staffshou = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                    staffshou = staffshou.Where(h => h.staffid == houid);
                    var hou = staffshou.First().staff_email;
                    //var houname = staffshou.First().staff_first_name + " " + staffshou.First().staff_last_name;

                    var cc = supers.First().staff_email;
                    var supername = supers.First().staff_first_name + " " + supers.First().staff_last_name;







                    var superemail = supers.First().staff_email;

                    //
                    //
                    //hou email
                    if (staffss.First().staffid == houid)
                    {
                        var canbedone = "";
                        if (missionAuthorizationn.canbedone == true)
                        {
                            canbedone = "Yes";
                        }
                        else
                        {
                            canbedone = "No";
                        }
                        var Roming = "";
                        if (missionAuthorizationn.StaffSignature == "True")
                        {
                            Roming = "Yes";
                        }
                        else
                        {
                            Roming = "No";
                        }
                        var CleredBySupervisor = "";
                        if (missionAuthorizationn.canbedone == true)
                        {
                            CleredBySupervisor = "Yes";
                        }
                        else
                        {
                            CleredBySupervisor = "No";
                        }

                        var BSITF2 = "";
                        if (missionAuthorizationn.BSITF == true)
                        {
                            BSITF2 = "Yes";
                        }
                        else
                        {
                            BSITF2 = "No";
                        }
                        ////////  
                        var ASITF2 = "";
                        if (missionAuthorizationn.ASITF == true)
                        {
                            ASITF2 = "Yes";
                        }
                        else
                        {
                            ASITF2 = "No";
                        }
                        ////////
                        var SSAFE2 = "";
                        if (missionAuthorizationn.SSAFE == true)
                        {
                            SSAFE2 = "Yes";
                        }
                        else
                        {
                            SSAFE2 = "No";
                        }
                        var odcitcc = "";
                        if (Roming == "Yes")
                        {
                            odcitcc = "cairo.itservicedesk@wfp.org";
                        }
                        else
                        {
                            odcitcc = "ahmed.badr@wfp.org";
                        }
                        //////////////////////////////


                        var supersforhou = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                        supersforhou = supersforhou.Where(d => d.staffid == staffsupervisor);
                        var superforhounitsname = supersforhou.First().staff_first_name + " " + staffshou.First().staff_last_name;
                        var houmailUPDATE = supersforhou.First().staff_email;

                        if (houmailUPDATE == "muhannad.hadi@wfp.org" || houmailUPDATE == "carlo.scaramella@wfp.org")
                        {
                            houmailUPDATE = "rbc.management@wfp.org"; //rbc.management@wfp.org

                        }
                        /////////////////////////

                        MailMessage mailhou = new MailMessage();
                        mailhou.To.Add(staff);
                        mailhou.Bcc.Add("ahmed.badr@wfp.org");
                        //mail2.CC.Add(cc);
                        mailhou.From = new MailAddress("RBC-SMS-SYSTEM@WFP.ORG");
                        mailhou.Subject = "Finance Not Cleared";

                        string Bodyhou = "<table><tr><td colspan='2'>" + "<hr/>New Mission<hr/></td></tr><tr><td><font color='blue'>Request : </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorizationn.MissionID + "</td></tr>"
                            + "<tr><td>" + "<font color='blue'>From :</font></td><td>" + staffss.First().staff_first_name + " " + staffss.First().staff_last_name + "</td></tr>"
                            + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                            + "<tr><td><font color='blue'>From :</font></td><td>" + missionAuthorizationn.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                            + "<tr><td><font color='blue'>To :</font></td><td>" + missionAuthorizationn.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                            + "<tr><td><font color='blue'>Funding :</font></td><td>" + missionAuthorizationn.funding + "</td></tr>"
                            + "<tr><td><font color='blue'>Objective :</font></td><td>" + missionAuthorizationn.MissionObjective + "</td></tr>"
                            + "<tr><td><font color='blue'>Mission Itinerary :</font></td><td>" + missionAuthorizationn.MissionItinerary + "</td></tr>"
                            + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video ? :</font></td><td>" + canbedone + "</td></tr>"
                            + "<tr><td><font color='blue'>Cleared By Supervisor :</font></td><td>" + CleredBySupervisor + "</td></tr>"
                             + "<tr><td><font color='blue'>HOU Comment :</font></td><td>" + missionAuthorizationn.Comment1 + "</td></tr>"
                             + "<tr><td><font color='blue'>Security Comment :</font></td><td>" + missionAuthorizationn.Comment2 + "</td></tr>"
                             + "<tr><td><font color='blue'>Finance Comment :</font></td><td>" + missionAuthorizationn.Comment3 + "</td></tr>"
                            + "<tr><td><font color='blue'>	BSITF :</font></td><td>" + BSITF2 + "</td></tr>"
                            + "<tr><td><font color='blue'> ASITF :</font></td><td>" + ASITF2 + "</td></tr>"
                            + "<tr><td><font color='blue'> SSAFE :</font></td><td>" + SSAFE2 + "</td></tr>"
                           + "<tr><td><font color='blue'> DSS Approval:</font></td><td><a target='_blank' href='http://10.11.135.254:8080/RBC-SMS/UploadedFiles/" + missionAuthorizationn.non6 + "'>" + missionAuthorizationn.non6 + "</a></td></tr></table>";

                        //string Body = "Request : Mission &nbsp;  &nbsp;  &nbsp; &nbsp;  &nbsp; &nbsp; &nbsp; &nbsp;  Mission ID : " + missionAuthorization.MissionID + " <br> " + " From &nbsp; &nbsp; &nbsp;:" + staffs.First().staff_first_name + "&nbsp;" + staffs.First().staff_last_name + "<br><br>" + " Mission Information" + "<br><br>" + " From :" + missionAuthorization.FromDate + "<br>To :" + missionAuthorization.ToDate +
                        //    "<br>Funding :" + missionAuthorization.funding + "<br>Objective :" + missionAuthorization.MissionObjective + "<br> Mission Itinerary :" + missionAuthorization.MissionItinerary + "<br> Can This meeting Be Done Over Voice/Video ? :" + missionAuthorization.canbedone  + " <br><br> <a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor=" + true + "&FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + " &to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Acept</a>&nbsp;&nbsp;&nbsp;<a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor= " + false + "& FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + "&to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Reject</a> ";

                        mailhou.Body = Bodyhou;
                        mailhou.IsBodyHtml = true;
                        SmtpClient smtphou = new SmtpClient();
                        smtphou.Host = "smtprelay.global.wfp.org";
                        smtphou.Port = 25;
                        smtphou.UseDefaultCredentials = true;
                        //smtp.Credentials = new System.Net.NetworkCredential
                        //("ahmed.badr", "Survivor2323");// Enter seders User name and password  
                        //smtp.EnableSsl = true;
                        smtphou.Send(mailhou);
                        return RedirectToAction("Index");
                    }

                    else
                    {
                        var canbedone = "";
                        if (missionAuthorizationn.canbedone == true)
                        {
                            canbedone = "Yes";
                        }
                        else
                        {
                            canbedone = "No";
                        }

                        var CleredBySupervisor = "";
                        if (missionAuthorizationn.canbedone == true)
                        {
                            CleredBySupervisor = "Yes";
                        }
                        else
                        {
                            CleredBySupervisor = "No";
                        }

                        var BSITF2 = "";
                        if (missionAuthorizationn.BSITF == true)
                        {
                            BSITF2 = "Yes";
                        }
                        else
                        {
                            BSITF2 = "No";
                        }
                        ////////  
                        var ASITF2 = "";
                        if (missionAuthorizationn.ASITF == true)
                        {
                            ASITF2 = "Yes";
                        }
                        else
                        {
                            ASITF2 = "No";
                        }
                        ////////
                        var SSAFE2 = "";
                        if (missionAuthorizationn.SSAFE == true)
                        {
                            SSAFE2 = "Yes";
                        }
                        else
                        {
                            SSAFE2 = "No";
                        }

                        //////////////////////////////


                        var supersforhou = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                        supersforhou = supersforhou.Where(d => d.staffid == staffsupervisor);
                        var superforhounitsname = supersforhou.First().staff_first_name + " " + staffshou.First().staff_last_name;
                        var houmailUPDATE = supersforhou.First().staff_email;

                        if (houmailUPDATE == "muhannad.hadi@wfp.org" || houmailUPDATE == "carlo.scaramella@wfp.org")
                        {
                            houmailUPDATE = "rbc.management@wfp.org"; //rbc.management@wfp.org

                        }
                        /////////////////////////


                        MailMessage mailhou = new MailMessage();
                        mailhou.To.Add(staff);
                        mailhou.Bcc.Add("ahmed.badr@wfp.org");
                        mailhou.CC.Add(cc);
                        mailhou.From = new MailAddress(hou);
                        mailhou.Subject = "Finance Not Cleared";

                        string Bodyhou = "<table><tr><td colspan='2'>" + "<hr/>New Mission<hr/></td></tr><tr><td><font color='blue'>Request : </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorizationn.MissionID + "</td></tr>"
                            + "<tr><td>" + "<font color='blue'>From :</font></td><td>" + staffss.First().staff_first_name + " " + staffss.First().staff_last_name + "</td></tr>"
                            + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                            + "<tr><td><font color='blue'>From :</font></td><td>" + missionAuthorizationn.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                            + "<tr><td><font color='blue'>To :</font></td><td>" + missionAuthorizationn.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                            + "<tr><td><font color='blue'>Funding :</font></td><td>" + missionAuthorizationn.funding + "</td></tr>"
                            + "<tr><td><font color='blue'>Objective :</font></td><td>" + missionAuthorizationn.MissionObjective + "</td></tr>"
                            + "<tr><td><font color='blue'>Mission Itinerary :</font></td><td>" + missionAuthorizationn.MissionItinerary + "</td></tr>"
                            + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video ? :</font></td><td>" + canbedone + "</td></tr>"
                            + "<tr><td><font color='blue'>Cleared By Supervisor :</font></td><td>" + CleredBySupervisor + "</td></tr>"
                             + "<tr><td><font color='blue'>HOU Comment :</font></td><td>" + missionAuthorizationn.Comment1 + "</td></tr>"
                             + "<tr><td><font color='blue'>Security Comment :</font></td><td>" + missionAuthorizationn.Comment2 + "</td></tr>"
                             + "<tr><td><font color='blue'>Finance Comment :</font></td><td>" + missionAuthorizationn.Comment3 + "</td></tr>"
                            + "<tr><td><font color='blue'>	BSITF :</font></td><td>" + BSITF2 + "</td></tr>"
                            + "<tr><td><font color='blue'> ASITF :</font></td><td>" + ASITF2 + "</td></tr>"
                            + "<tr><td><font color='blue'> SSAFE :</font></td><td>" + SSAFE2 + "</td></tr>"
                           + "<tr><td><font color='blue'> DSS Approval:</font></td><td><a target='_blank' href='http://10.11.135.254:8080/RBC-SMS/UploadedFiles/" + missionAuthorizationn.non6 + "'>" + missionAuthorizationn.non6 + "</a></td></tr></table>";

                        //string Body = "Request : Mission &nbsp;  &nbsp;  &nbsp; &nbsp;  &nbsp; &nbsp; &nbsp; &nbsp;  Mission ID : " + missionAuthorization.MissionID + " <br> " + " From &nbsp; &nbsp; &nbsp;:" + staffs.First().staff_first_name + "&nbsp;" + staffs.First().staff_last_name + "<br><br>" + " Mission Information" + "<br><br>" + " From :" + missionAuthorization.FromDate + "<br>To :" + missionAuthorization.ToDate +
                        //    "<br>Funding :" + missionAuthorization.funding + "<br>Objective :" + missionAuthorization.MissionObjective + "<br> Mission Itinerary :" + missionAuthorization.MissionItinerary + "<br> Can This meeting Be Done Over Voice/Video ? :" + missionAuthorization.canbedone  + " <br><br> <a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor=" + true + "&FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + " &to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Acept</a>&nbsp;&nbsp;&nbsp;<a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor= " + false + "& FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + "&to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Reject</a> ";

                        mailhou.Body = Bodyhou;
                        mailhou.IsBodyHtml = true;
                        SmtpClient smtphou = new SmtpClient();
                        smtphou.Host = "smtprelay.global.wfp.org";
                        smtphou.Port = 25;
                        smtphou.UseDefaultCredentials = true;
                        //smtp.Credentials = new System.Net.NetworkCredential
                        //("ahmed.badr", "Survivor2323");// Enter seders User name and password  
                        //smtp.EnableSsl = true;
                        smtphou.Send(mailhou);
                        return RedirectToAction("Index");


                    }
                }
            }
            return View();

        }


        ////////////////////////////////////////////////////////////////////////////////// end approval level 3 finance //////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        /////////////////////////////////////////////////////////////////////// start approval level 4 CD //////////////////////////////////////////////////////////////////////////////////////////

        public ActionResult Approval2CD(int? i, int? MissionID2, string RDORDRDSignature2, DateTime? FromDate2, DateTime? ToDate2, string to2, string staffmail2, string supervisorname2, int? onbehalfID2, int? onbehalfSuperID2, int? onbehalfHouID2)
        {
            ViewBag.MissionID = MissionID2;
            ViewBag.RDORDRDSignature = RDORDRDSignature2;
            ViewBag.FromDate = FromDate2;
            ViewBag.ToDate = ToDate2;
            ViewBag.to = to2;
            ViewBag.staffmail = staffmail2;
            ViewBag.supervisorname = supervisorname2;
            ViewBag.onbehalfID = onbehalfID2;
            ViewBag.onbehalfSuperID = onbehalfSuperID2;
            ViewBag.onbehalfHouID = onbehalfHouID2;

            return View();
        }

        [HttpPost]
        public ActionResult Approval2CD(int? MissionID, string non14, string RDORDRDSignature, DateTime? FromDate, DateTime? ToDate, string non10, string staff, string supervisorname, int? onbehalfID, int? onbehalfSuperID, int? onbehalfHouID)
        {


            if (!Request.IsAuthenticated)
            {
                Response.Redirect("/RBC-SMS/account/login");
            }

            ///////////////////////// start on behalf 

            if (onbehalfID != null && onbehalfSuperID != null && onbehalfHouID != null)
            {
                ViewBag.mid = Request.QueryString["MissionID"];
                ViewBag.RDORDRDSignature = Request.QueryString["RDORDRDSignature"];
                ViewBag.FromDate = Request.QueryString["FromDate"];
                ViewBag.ToDate = Request.QueryString["ToDate"];
                if (RDORDRDSignature == "Yes")
                {
                    try
                    {
                        // missionAuthorization = db.MissionAuthorizations.Find(Mid);
                        //db.Entry(missionAuthorization).State = EntityState.Modified ;
                        //db.SaveChanges();

                        var missionAuthorization11 = db.MissionAuthorization1.Single(u => u.MissionID == MissionID);
                        missionAuthorization11.non12  = true;
                        missionAuthorization11.non13 = DateTime.Now;
                        missionAuthorization11.non14 = non14;
                        db.SaveChanges();



                        var staffs = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                        staffs = staffs.Where(s => s.staffid == onbehalfID);
                        var too = staffs.First().staff_email;
                        ////////////////////////////////////////////////////////////////////////////onbehalf
                        var staffsupervisor = staffs.First().staff_supervisorid;
                        var supers = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                        supers = supers.Where(d => d.staffid == onbehalfID);
                        var onbehalfemail = supers.First().staff_email;


                        var supersonbehalf = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                        supersonbehalf = supersonbehalf.Where(d => d.staffid == onbehalfSuperID);
                        var onbehafsuper = supersonbehalf.First().staff_email;
                        /////////////////////////////////////////////////////////////////////////////////////
                        ////////////////////////////////////////////////////
                        //var rd = db.staffs.Include(r => r.contract_details).Include(r => r.contract_type).Include(r => r.Country_office).Include(r => r.functional_title).Include(r => r.staff2).Include(r => r.sub_office).Include(r => r.unit).Include(r => r.country).Include(r => r.governorate);
                        //rd = rd.Where(r => r.functional_title.functional_title_description_english == "REGIONAL DIRECTOR");
                        //var rdmail = rd.First().staff_email;

                        //var drd = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                        //drd = drd.Where(s => s.functional_title.functional_title_description_english == "Deputy Regional Director");
                        //var drdmail = drd.First().staff_email;

                        var userUnit = staffs.First().unit_id;
                        var unithou = db.units.Include(d => d.staffs);
                        unithou = unithou.Where(u => u.unitid == userUnit);
                        var houid = unithou.First().HOU_ID;

                        var staffshou = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                        staffshou = staffshou.Where(h => h.staffid == houid);
                        var hou = staffshou.First().staff_email;
                        //var houname = staffshou.First().staff_first_name + " " + staffshou.First().staff_last_name;

                        var cc = supers.First().staff_email;
                        var supername = supers.First().staff_first_name + " " + supers.First().staff_last_name;
                        var superemail = supers.First().staff_email;

                        //
                        //
                        //hou email
                        if (onbehalfID == houid)
                        {
                            var canbedone = "";
                            if (missionAuthorization11.canbedone == true)
                            {
                                canbedone = "Yes";
                            }
                            else
                            {
                                canbedone = "No";
                            }

                            var CleredBySupervisor = "";
                            if (missionAuthorization11.canbedone == true)
                            {
                                CleredBySupervisor = "Yes";
                            }
                            else
                            {
                                CleredBySupervisor = "No";
                            }

                            var BSITF2 = "";
                            if (missionAuthorization11.BSITF == true)
                            {
                                BSITF2 = "Yes";
                            }
                            else
                            {
                                BSITF2 = "No";
                            }
                            ////////  
                            var ASITF2 = "";
                            if (missionAuthorization11.ASITF == true)
                            {
                                ASITF2 = "Yes";
                            }
                            else
                            {
                                ASITF2 = "No";
                            }
                            ////////
                            var SSAFE2 = "";
                            if (missionAuthorization11.SSAFE == true)
                            {
                                SSAFE2 = "Yes";
                            }
                            else
                            {
                                SSAFE2 = "No";
                            }
                            var Roming = "";
                            if (missionAuthorization11.StaffSignature == "True")
                            {
                                Roming = "Yes";
                            }
                            else
                            {
                                Roming = "No";
                            }
                            var odcitcc = "";
                            if (Roming == "Yes")
                            {
                                odcitcc = "cairo.itservicedesk@wfp.org";
                            }
                            else
                            {
                                odcitcc = "ahmed.badr@wfp.org";
                            }
                            //////////////////////////////


                            var supersforhou = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                            supersforhou = supersforhou.Where(d => d.staffid == staffsupervisor);
                            var superforhounitsname = supersforhou.First().staff_first_name + " " + staffshou.First().staff_last_name;
                            var houmailUPDATE = supersforhou.First().staff_email;

                            if (houmailUPDATE == "muhannad.hadi@wfp.org" || houmailUPDATE == "carlo.scaramella@wfp.org")
                            {
                                houmailUPDATE = "rbc.management@wfp.org"; //rbc.management@wfp.org

                            }
                            /////////////////////////

                            MailMessage mailhou = new MailMessage();
                            mailhou.To.Add(onbehalfemail);
                            mailhou.Bcc.Add("ahmed.badr@wfp.org");

                            //mailhou.CC.Add("rbc.travel@wfp.org"); //travel unit
                            //mailhou.CC.Add("mai.kenawi@wfp.org"); //travel unit
                            //mailhou.CC.Add("amal.mohamed@wfp.org"); //travel unit
                            //mailhou.CC.Add("ayten.el-sheikh@wfp.org"); //travel unit
                            //mailhou.CC.Add("essraa.soliman@wfp.org"); //travel unit
                            //mailhou.CC.Add("rbc.management@wfp.org"); //RD/DRD Office
                            mailhou.Bcc.Add(odcitcc); //travel unit
                            mailhou.From = new MailAddress("RBC-SMS-SYSTEM@wfp.org");
                            mailhou.Subject = "CD Approved";

                            string Bodyhou = "<table><tr><td colspan='2'>" + "<hr/>New Mission<hr/></td></tr><tr><td><font color='blue'>Request : </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorization11.MissionID + "</td></tr>"
                                + "<tr><td>" + "<font color='blue'>From :</font></td><td>" + staffs.First().staff_first_name + " " + staffs.First().staff_last_name + "</td></tr>"
                                + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                                + "<tr><td><font color='blue'>From :</font></td><td>" + missionAuthorization11.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                                + "<tr><td><font color='blue'>To :</font></td><td>" + missionAuthorization11.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                                + "<tr><td><font color='blue'>Funding :</font></td><td>" + missionAuthorization11.funding + "</td></tr>"
                                + "<tr><td><font color='blue'>Objective :</font></td><td>" + missionAuthorization11.MissionObjective + "</td></tr>"
                                + "<tr><td><font color='blue'>Mission Itinerary :</font></td><td>" + missionAuthorization11.MissionItinerary + "</td></tr>"
                                + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video ? :</font></td><td>" + canbedone + "</td></tr>"
                                + "<tr><td><font color='blue'>Cleared By Supervisor :</font></td><td>" + CleredBySupervisor + "</td></tr>"
                                + "<tr><td><font color='blue'>International Roaming :</font></td><td>" + Roming + "</td></tr>"
                                + "<tr><td><font color='blue'>HOU Comment :</font></td><td>" + missionAuthorization11.Comment1 + "</td></tr>"
                                + "<tr><td><font color='blue'>Security Comment :</font></td><td>" + missionAuthorization11.Comment2 + "</td></tr>"
                                + "<tr><td><font color='blue'>Finance Comment :</font></td><td>" + missionAuthorization11.Comment3 + "</td></tr>"
                                + "<tr><td><font color='blue'>	BSITF :</font></td><td>" + BSITF2 + "</td></tr>"
                                + "<tr><td><font color='blue'> ASITF :</font></td><td>" + ASITF2 + "</td></tr>"
                                + "<tr><td><font color='blue'> SSAFE :</font></td><td>" + SSAFE2 + "</td></tr>"
                                + "<tr><td><font color='blue'> DSS Approval:</font></td><td><a href='http://10.11.135.254:8080/RBC-SMS/UploadedFiles/" + missionAuthorization11.non6 + "'>" + missionAuthorization11.non6 + "</a></td></tr></table>";

                            //string Body = "Request : Mission &nbsp;  &nbsp;  &nbsp; &nbsp;  &nbsp; &nbsp; &nbsp; &nbsp;  Mission ID : " + missionAuthorization.MissionID + " <br> " + " From &nbsp; &nbsp; &nbsp;:" + staffs.First().staff_first_name + "&nbsp;" + staffs.First().staff_last_name + "<br><br>" + " Mission Information" + "<br><br>" + " From :" + missionAuthorization.FromDate + "<br>To :" + missionAuthorization.ToDate +
                            //    "<br>Funding :" + missionAuthorization.funding + "<br>Objective :" + missionAuthorization.MissionObjective + "<br> Mission Itinerary :" + missionAuthorization.MissionItinerary + "<br> Can This meeting Be Done Over Voice/Video ? :" + missionAuthorization.canbedone  + " <br><br> <a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor=" + true + "&FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + " &to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Acept</a>&nbsp;&nbsp;&nbsp;<a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor= " + false + "& FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + "&to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Reject</a> ";

                            mailhou.Body = Bodyhou;
                            mailhou.IsBodyHtml = true;
                            SmtpClient smtphou = new SmtpClient();
                            smtphou.Host = "smtprelay.global.wfp.org";
                            smtphou.Port = 25;
                            smtphou.UseDefaultCredentials = true;
                            //smtp.Credentials = new System.Net.NetworkCredential
                            //("ahmed.badr", "Survivor2323");// Enter seders User name and password  
                            //smtp.EnableSsl = true;
                            smtphou.Send(mailhou);

                            ///////////////////////////// CD  Approval //////////////////////

                            MailMessage mailhou1 = new MailMessage();
                            mailhou1.To.Add("ahmed.badr@wfp.org");
                            mailhou1.Bcc.Add("ahmed.badr@wfp.org");

                            //mailhou.CC.Add("rbc.travel@wfp.org"); //travel unit
                            //mailhou.CC.Add("mai.kenawi@wfp.org"); //travel unit
                            //mailhou.CC.Add("amal.mohamed@wfp.org"); //travel unit
                            //mailhou.CC.Add("ayten.el-sheikh@wfp.org"); //travel unit
                            //mailhou.CC.Add("essraa.soliman@wfp.org"); //travel unit
                            //mailhou.CC.Add("rbc.management@wfp.org"); //RD/DRD Office
                            mailhou1.Bcc.Add(odcitcc); //travel unit
                            mailhou1.From = new MailAddress("RBC-SMS-SYSTEM@wfp.org");
                            mailhou1.Subject = "For CD Approval";

                            string Bodyhou1 = "<table><tr><td colspan='2'>" + "<hr/>New Mission<hr/></td></tr><tr><td><font color='blue'>Request : </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorization11.MissionID + "</td></tr>"
                                + "<tr><td>" + "<font color='blue'>From :</font></td><td>" + staffs.First().staff_first_name + " " + staffs.First().staff_last_name + "</td></tr>"
                                + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                                + "<tr><td><font color='blue'>From :</font></td><td>" + missionAuthorization11.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                                + "<tr><td><font color='blue'>To :</font></td><td>" + missionAuthorization11.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                                + "<tr><td><font color='blue'>Funding :</font></td><td>" + missionAuthorization11.funding + "</td></tr>"
                                + "<tr><td><font color='blue'>Objective :</font></td><td>" + missionAuthorization11.MissionObjective + "</td></tr>"
                                + "<tr><td><font color='blue'>Mission Itinerary :</font></td><td>" + missionAuthorization11.MissionItinerary + "</td></tr>"
                                + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video ? :</font></td><td>" + canbedone + "</td></tr>"
                                + "<tr><td><font color='blue'>Cleared By Supervisor :</font></td><td>" + CleredBySupervisor + "</td></tr>"
                                + "<tr><td><font color='blue'>International Roaming :</font></td><td>" + Roming + "</td></tr>"
                                + "<tr><td><font color='blue'>HOU Comment :</font></td><td>" + missionAuthorization11.Comment1 + "</td></tr>"
                                   + "<tr><td><font color='blue'>Security Comment :</font></td><td>" + missionAuthorization11.Comment2 + "</td></tr>"
                                      + "<tr><td><font color='blue'>FInance Comment :</font></td><td>" + missionAuthorization11.Comment3 + "</td></tr>"
                                + "<tr><td><font color='blue'>	BSITF :</font></td><td>" + BSITF2 + "</td></tr>"
                                + "<tr><td><font color='blue'> ASITF :</font></td><td>" + ASITF2 + "</td></tr>"
                                + "<tr><td><font color='blue'> SSAFE :</font></td><td>" + SSAFE2 + "</td></tr>"
                                + "<tr><td><font color='blue'> DSS Approval:</font></td><td><a href='http://10.11.135.254:8080/RBC-SMS/UploadedFiles/" + missionAuthorization11.non6 + "'>" + missionAuthorization11.non6
                            + "<tr><td><a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorization1/Approval2FinAdmin?MissionID2="
                            + missionAuthorization11.MissionID
                            + "&RDORDRDSignature2=Yes&FromDate2=" + missionAuthorization11.FromDate
                            + "&ToDate2=" + missionAuthorization11.ToDate
                            + "&to2=" + non10 + "&staffmail2=" + staffs.First().staff_email
                            + "'>Cleared</a></td><td><a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorization1/Approval2FinAdmin?MissionID2=" + missionAuthorization11.MissionID
                            + "&RDORDRDSignature2=NO & FromDate2=" + missionAuthorization11.FromDate
                            + "&ToDate2=" + missionAuthorization11.ToDate
                            + "&to2=" + non10
                            + "&staffmail2=" + staffs.First().staff_email + "'>Not Cleared</a>" + "</a></td></tr></table>";

                            //string Body = "Request : Mission &nbsp;  &nbsp;  &nbsp; &nbsp;  &nbsp; &nbsp; &nbsp; &nbsp;  Mission ID : " + missionAuthorization.MissionID + " <br> " + " From &nbsp; &nbsp; &nbsp;:" + staffs.First().staff_first_name + "&nbsp;" + staffs.First().staff_last_name + "<br><br>" + " Mission Information" + "<br><br>" + " From :" + missionAuthorization.FromDate + "<br>To :" + missionAuthorization.ToDate +
                            //    "<br>Funding :" + missionAuthorization.funding + "<br>Objective :" + missionAuthorization.MissionObjective + "<br> Mission Itinerary :" + missionAuthorization.MissionItinerary + "<br> Can This meeting Be Done Over Voice/Video ? :" + missionAuthorization.canbedone  + " <br><br> <a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor=" + true + "&FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + " &to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Acept</a>&nbsp;&nbsp;&nbsp;<a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor= " + false + "& FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + "&to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Reject</a> ";

                            mailhou1.Body = Bodyhou1;
                            mailhou1.IsBodyHtml = true;
                            SmtpClient smtphou1 = new SmtpClient();
                            smtphou1.Host = "smtprelay.global.wfp.org";
                            smtphou1.Port = 25;
                            smtphou1.UseDefaultCredentials = true;
                            //smtp.Credentials = new System.Net.NetworkCredential
                            //("ahmed.badr", "Survivor2323");// Enter seders User name and password  
                            //smtp.EnableSsl = true;
                            //smtphou1.Send(mailhou1);

                            ///////////////////////////// CD Approval  end //////////////////////
                            return RedirectToAction("Index");
                        }

                        else
                        {
                            var canbedone = "";
                            if (missionAuthorization11.canbedone == true)
                            {
                                canbedone = "Yes";
                            }
                            else
                            {
                                canbedone = "No";
                            }

                            var CleredBySupervisor = "";
                            if (missionAuthorization11.canbedone == true)
                            {
                                CleredBySupervisor = "Yes";
                            }
                            else
                            {
                                CleredBySupervisor = "No";
                            }

                            var BSITF2 = "";
                            if (missionAuthorization11.BSITF == true)
                            {
                                BSITF2 = "Yes";
                            }
                            else
                            {
                                BSITF2 = "No";
                            }
                            ////////  
                            var ASITF2 = "";
                            if (missionAuthorization11.ASITF == true)
                            {
                                ASITF2 = "Yes";
                            }
                            else
                            {
                                ASITF2 = "No";
                            }
                            ////////
                            var SSAFE2 = "";
                            if (missionAuthorization11.SSAFE == true)
                            {
                                SSAFE2 = "Yes";
                            }
                            else
                            {
                                SSAFE2 = "No";
                            }
                            var odcitcc = "";
                            if (missionAuthorization11.StaffSignature == "True")
                            {
                                odcitcc = "cairo.itservicedesk@wfp.org";
                            }
                            else
                            {
                                odcitcc = "ahmed.badr@wfp.org";
                            }
                            var Roming = "";
                            if (missionAuthorization11.StaffSignature == "True")
                            {
                                Roming = "Yes";
                            }
                            else
                            {
                                Roming = "No";
                            }
                            //////////////////////////////


                            var supersforhou = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                            supersforhou = supersforhou.Where(d => d.staffid == staffsupervisor);
                            var superforhounitsname = supersforhou.First().staff_first_name + " " + staffshou.First().staff_last_name;
                            var houmailUPDATE = supersforhou.First().staff_email;

                            if (houmailUPDATE == "muhannad.hadi@wfp.org" || houmailUPDATE == "carlo.scaramella@wfp.org")
                            {
                                houmailUPDATE = "rbc.management@wfp.org"; //rbc.management@wfp.org

                            }
                            /////////////////////////

                            MailMessage mailhou = new MailMessage();
                            mailhou.To.Add(onbehalfemail); //staffmail
                            mailhou.Bcc.Add("ahmed.badr@wfp.org");
                            mailhou.CC.Add(onbehafsuper);
                            mailhou.Bcc.Add(odcitcc); //travel unit
                            //mailhou.CC.Add("rbc.travel@wfp.org"); //travel unit
                            //mailhou.CC.Add("mai.kenawi@wfp.org"); //travel unit
                            //mailhou.CC.Add("amal.mohamed@wfp.org"); //travel unit
                            //mailhou.CC.Add("ayten.el-sheikh@wfp.org"); //travel unit
                            //mailhou.CC.Add("essraa.soliman@wfp.org"); //travel unit
                            //mailhou.CC.Add("rbc.management@wfp.org"); //RD/DRD Office
                            mailhou.From = new MailAddress("RBC-SMS-SYSTEM@wfp.org");
                            mailhou.Subject = "CD Approved";

                            string Bodyhou = "<table><tr><td colspan='2'>" + "<hr/>New Mission<hr/></td></tr><tr><td><font color='blue'>Request : </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorization11.MissionID + "</td></tr>"
                                + "<tr><td>" + "<font color='blue'>From :</font></td><td>" + staffs.First().staff_first_name + " " + staffs.First().staff_last_name + "</td></tr>"
                                + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                                + "<tr><td><font color='blue'>From :</font></td><td>" + missionAuthorization11.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                                + "<tr><td><font color='blue'>To :</font></td><td>" + missionAuthorization11.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                                + "<tr><td><font color='blue'>Funding :</font></td><td>" + missionAuthorization11.funding + "</td></tr>"
                                + "<tr><td><font color='blue'>Objective :</font></td><td>" + missionAuthorization11.MissionObjective + "</td></tr>"
                                + "<tr><td><font color='blue'>Mission Itinerary :</font></td><td>" + missionAuthorization11.MissionItinerary + "</td></tr>"
                                + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video ? :</font></td><td>" + canbedone + "</td></tr>"
                                + "<tr><td><font color='blue'>Cleared By Supervisor :</font></td><td>" + CleredBySupervisor + "</td></tr>"
                                + "<tr><td><font color='blue'>International Roaming :</font></td><td>" + Roming + "</td></tr>"
                                + "<tr><td><font color='blue'>HOU Comment :</font></td><td>" + missionAuthorization11.Comment1 + "</td></tr>"
                                + "<tr><td><font color='blue'>Security Comment :</font></td><td>" + missionAuthorization11.Comment2 + "</td></tr>"
                                + "<tr><td><font color='blue'>Finance Comment :</font></td><td>" + missionAuthorization11.Comment3 + "</td></tr>"
                                + "<tr><td><font color='blue'>	BSITF :</font></td><td>" + BSITF2 + "</td></tr>"
                                + "<tr><td><font color='blue'> ASITF :</font></td><td>" + ASITF2 + "</td></tr>"
                                + "<tr><td><font color='blue'> SSAFE :</font></td><td>" + SSAFE2 + "</td></tr>"
                                + "<tr><td><font color='blue'> DSS Approval:</font></td><td><a href='http://10.11.135.254:8080/RBC-SMS/UploadedFiles/" + missionAuthorization11.non6 + "'>" + missionAuthorization11.non6 + "</a></td></tr></table>";

                            //string Body = "Request : Mission &nbsp;  &nbsp;  &nbsp; &nbsp;  &nbsp; &nbsp; &nbsp; &nbsp;  Mission ID : " + missionAuthorization.MissionID + " <br> " + " From &nbsp; &nbsp; &nbsp;:" + staffs.First().staff_first_name + "&nbsp;" + staffs.First().staff_last_name + "<br><br>" + " Mission Information" + "<br><br>" + " From :" + missionAuthorization.FromDate + "<br>To :" + missionAuthorization.ToDate +
                            //    "<br>Funding :" + missionAuthorization.funding + "<br>Objective :" + missionAuthorization.MissionObjective + "<br> Mission Itinerary :" + missionAuthorization.MissionItinerary + "<br> Can This meeting Be Done Over Voice/Video ? :" + missionAuthorization.canbedone  + " <br><br> <a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor=" + true + "&FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + " &to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Acept</a>&nbsp;&nbsp;&nbsp;<a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor= " + false + "& FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + "&to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Reject</a> ";

                            mailhou.Body = Bodyhou;
                            mailhou.IsBodyHtml = true;
                            SmtpClient smtphou = new SmtpClient();
                            smtphou.Host = "smtprelay.global.wfp.org";
                            smtphou.Port = 25;
                            smtphou.UseDefaultCredentials = true;
                            //smtp.Credentials = new System.Net.NetworkCredential
                            //("ahmed.badr", "Survivor2323");// Enter seders User name and password  
                            //smtp.EnableSsl = true;
                            smtphou.Send(mailhou);


                            ///////////////////////////// CD Unit Approval //////////////////////

                            MailMessage mailhou1 = new MailMessage();
                            mailhou1.To.Add("ahmed.badr@wfp.org");
                            mailhou1.Bcc.Add("ahmed.badr@wfp.org");

                            //mailhou.CC.Add("rbc.travel@wfp.org"); //travel unit
                            //mailhou.CC.Add("mai.kenawi@wfp.org"); //travel unit
                            //mailhou.CC.Add("amal.mohamed@wfp.org"); //travel unit
                            //mailhou.CC.Add("ayten.el-sheikh@wfp.org"); //travel unit
                            //mailhou.CC.Add("essraa.soliman@wfp.org"); //travel unit
                            //mailhou.CC.Add("rbc.management@wfp.org"); //RD/DRD Office
                            mailhou1.Bcc.Add(odcitcc); //travel unit
                            mailhou1.From = new MailAddress("RBC-SMS-SYSTEM@wfp.org");
                            mailhou1.Subject = "For CD Approval";

                            string Bodyhou1 = "<table><tr><td colspan='2'>" + "<hr/>New Mission<hr/></td></tr><tr><td><font color='blue'>Request : </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorization11.MissionID + "</td></tr>"
                                + "<tr><td>" + "<font color='blue'>From :</font></td><td>" + staffs.First().staff_first_name + " " + staffs.First().staff_last_name + "</td></tr>"
                                + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                                + "<tr><td><font color='blue'>From :</font></td><td>" + missionAuthorization11.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                                + "<tr><td><font color='blue'>To :</font></td><td>" + missionAuthorization11.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                                + "<tr><td><font color='blue'>Funding :</font></td><td>" + missionAuthorization11.funding + "</td></tr>"
                                + "<tr><td><font color='blue'>Objective :</font></td><td>" + missionAuthorization11.MissionObjective + "</td></tr>"
                                + "<tr><td><font color='blue'>Mission Itinerary :</font></td><td>" + missionAuthorization11.MissionItinerary + "</td></tr>"
                                + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video ? :</font></td><td>" + canbedone + "</td></tr>"
                                + "<tr><td><font color='blue'>Cleared By Supervisor :</font></td><td>" + CleredBySupervisor + "</td></tr>"
                                + "<tr><td><font color='blue'>International Roaming :</font></td><td>" + Roming + "</td></tr>"
                                + "<tr><td><font color='blue'>HOU Comment :</font></td><td>" + missionAuthorization11.Comment1 + "</td></tr>"
                                + "<tr><td><font color='blue'>Security Comment :</font></td><td>" + missionAuthorization11.Comment2 + "</td></tr>"
                                + "<tr><td><font color='blue'>Finance Comment :</font></td><td>" + missionAuthorization11.Comment3 + "</td></tr>"
                                + "<tr><td><font color='blue'>	BSITF :</font></td><td>" + BSITF2 + "</td></tr>"
                                + "<tr><td><font color='blue'> ASITF :</font></td><td>" + ASITF2 + "</td></tr>"
                                + "<tr><td><font color='blue'> SSAFE :</font></td><td>" + SSAFE2 + "</td></tr>"
                                + "<tr><td><font color='blue'> DSS Approval:</font></td><td><a href='http://10.11.135.254:8080/RBC-SMS/UploadedFiles/" + missionAuthorization11.non6 + "'>" + missionAuthorization11.non6

                           + "<tr><td><a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorization1/Approval2FinAdmin?MissionID2="
                            + missionAuthorization11.MissionID
                            + "&RDORDRDSignature2=Yes&FromDate2=" + missionAuthorization11.FromDate
                            + "&ToDate2=" + missionAuthorization11.ToDate
                            + "&to2=" + non10 + "&staffmail2=" + staffs.First().staff_email
                            + "'>Cleared</a></td><td><a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorization1/Approval2FinAdmin?MissionID2=" + missionAuthorization11.MissionID
                            + "&RDORDRDSignature2=NO & FromDate2=" + missionAuthorization11.FromDate
                            + "&ToDate2=" + missionAuthorization11.ToDate
                            + "&to2=" + non10
                            + "&staffmail2=" + staffs.First().staff_email + "'>Not Cleared</a>" + "</a></td></tr></table>";


                            //string Body = "Request : Mission &nbsp;  &nbsp;  &nbsp; &nbsp;  &nbsp; &nbsp; &nbsp; &nbsp;  Mission ID : " + missionAuthorization.MissionID + " <br> " + " From &nbsp; &nbsp; &nbsp;:" + staffs.First().staff_first_name + "&nbsp;" + staffs.First().staff_last_name + "<br><br>" + " Mission Information" + "<br><br>" + " From :" + missionAuthorization.FromDate + "<br>To :" + missionAuthorization.ToDate +
                            //    "<br>Funding :" + missionAuthorization.funding + "<br>Objective :" + missionAuthorization.MissionObjective + "<br> Mission Itinerary :" + missionAuthorization.MissionItinerary + "<br> Can This meeting Be Done Over Voice/Video ? :" + missionAuthorization.canbedone  + " <br><br> <a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor=" + true + "&FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + " &to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Acept</a>&nbsp;&nbsp;&nbsp;<a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor= " + false + "& FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + "&to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Reject</a> ";

                            mailhou1.Body = Bodyhou1;
                            mailhou1.IsBodyHtml = true;
                            SmtpClient smtphou1 = new SmtpClient();
                            smtphou1.Host = "smtprelay.global.wfp.org";
                            smtphou1.Port = 25;
                            smtphou1.UseDefaultCredentials = true;
                            //smtp.Credentials = new System.Net.NetworkCredential
                            //("ahmed.badr", "Survivor2323");// Enter seders User name and password  
                            //smtp.EnableSsl = true;
                            //smtphou1.Send(mailhou1);

                            ///////////////////////////// CD Approval end //////////////////////

                            return RedirectToAction("Index");


                        }
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
                    return View();
                }
                else
                {
                    //var missionAuthorizationn = db.MissionAuthorizations.Single(u => u.MissionID == Mid);
                    //db.MissionAuthorizations.Remove(missionAuthorizationn);
                    //db.SaveChanges();
                    var missionAuthorization12 = db.MissionAuthorization1.Single(u => u.MissionID == MissionID);
                    missionAuthorization12.non12 = false;
                    missionAuthorization12.non13 = DateTime.Now;
                    missionAuthorization12.non14 = non14;
                    db.SaveChanges();



                    var staffss = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                    staffss = staffss.Where(s => s.staffid == onbehalfID);
                    var too = staffss.First().staff_email;
                    var staffsupervisor = staffss.First().staff_supervisorid;
                    var supers = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                    supers = supers.Where(d => d.staffid == staffsupervisor);
                    var from = supers.First().staff_email;

                    //var rd = db.staffs.Include(r => r.contract_details).Include(r => r.contract_type).Include(r => r.Country_office).Include(r => r.functional_title).Include(r => r.staff2).Include(r => r.sub_office).Include(r => r.unit).Include(r => r.country).Include(r => r.governorate);
                    //rd = rd.Where(r => r.functional_title.functional_title_description_english == "REGIONAL DIRECTOR");
                    //var rdmail = rd.First().staff_email;

                    //var drd = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                    //drd = drd.Where(s => s.functional_title.functional_title_description_english == "Deputy Regional Director");
                    //var drdmail = drd.First().staff_email;

                    //////////////////////////////////////////////////////////////////////////////////////


                    var Sonbehafemail = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                    Sonbehafemail = Sonbehafemail.Where(d => d.staffid == onbehalfID);
                    var onbehalfemail = Sonbehafemail.First().staff_email;


                    var supersonbehalf = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                    supersonbehalf = supersonbehalf.Where(d => d.staffid == onbehalfSuperID);
                    var onbehafsuperemail = supersonbehalf.First().staff_email;

                    /////////////////////////////////////////////////////////////////////////////////////
                    //new
                    var userUnit = staffss.First().unit_id;
                    var unithou = db.units.Include(d => d.staffs);
                    unithou = unithou.Where(u => u.unitid == userUnit);
                    var houid = unithou.First().HOU_ID;

                    var staffshou = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                    staffshou = staffshou.Where(h => h.staffid == houid);
                    var hou = staffshou.First().staff_email;
                    //var houname = staffshou.First().staff_first_name + " " + staffshou.First().staff_last_name;

                    var cc = supers.First().staff_email;
                    var supername = supers.First().staff_first_name + " " + supers.First().staff_last_name;

                    //
                    //
                    //hou email
                    if (onbehalfID == houid)
                    {
                        var canbedone = "";
                        if (missionAuthorization12.canbedone == true)
                        {
                            canbedone = "Yes";
                        }
                        else
                        {
                            canbedone = "No";
                        }

                        var CleredBySupervisor = "";
                        if (missionAuthorization12.canbedone == true)
                        {
                            CleredBySupervisor = "Yes";
                        }
                        else
                        {
                            CleredBySupervisor = "No";
                        }

                        var BSITF2 = "";
                        if (missionAuthorization12.BSITF == true)
                        {
                            BSITF2 = "Yes";
                        }
                        else
                        {
                            BSITF2 = "No";
                        }
                        ////////  
                        var ASITF2 = "";
                        if (missionAuthorization12.ASITF == true)
                        {
                            ASITF2 = "Yes";
                        }
                        else
                        {
                            ASITF2 = "No";
                        }
                        ////////
                        var SSAFE2 = "";
                        if (missionAuthorization12.SSAFE == true)
                        {
                            SSAFE2 = "Yes";
                        }
                        else
                        {
                            SSAFE2 = "No";
                        }

                        //////////////////////////////


                        var supersforhou = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                        supersforhou = supersforhou.Where(d => d.staffid == staffsupervisor);
                        var superforhounitsname = supersforhou.First().staff_first_name + " " + staffshou.First().staff_last_name;
                        var houmailUPDATE = supersforhou.First().staff_email;

                        if (houmailUPDATE == "muhannad.hadi@wfp.org" || houmailUPDATE == "carlo.scaramella@wfp.org")
                        {
                            houmailUPDATE = "rbc.management@wfp.org"; //rbc.management@wfp.org

                        }
                        /////////////////////////

                        MailMessage mailhou = new MailMessage();
                        mailhou.To.Add(onbehalfemail);
                        mailhou.Bcc.Add("ahmed.badr@wfp.org");
                        //mail2.CC.Add(cc);
                        mailhou.From = new MailAddress("RBC-SMS-SYSTEM@wfp.org");
                        mailhou.Subject = "CD Rejected";

                        string Bodyhou = "<table><tr><td colspan='2'>" + "<hr/>New Mission<hr/></td></tr><tr><td><font color='blue'>Request : </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorization12.MissionID + "</td></tr>"
                            + "<tr><td>" + "<font color='blue'>From :</font></td><td>" + staffss.First().staff_first_name + " " + staffss.First().staff_last_name + "</td></tr>"
                            + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                            + "<tr><td><font color='blue'>From :</font></td><td>" + missionAuthorization12.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                            + "<tr><td><font color='blue'>To :</font></td><td>" + missionAuthorization12.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                            + "<tr><td><font color='blue'>Funding :</font></td><td>" + missionAuthorization12.funding + "</td></tr>"
                            + "<tr><td><font color='blue'>Objective :</font></td><td>" + missionAuthorization12.MissionObjective + "</td></tr>"
                            + "<tr><td><font color='blue'>Mission Itinerary :</font></td><td>" + missionAuthorization12.MissionItinerary + "</td></tr>"
                            + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video ? :</font></td><td>" + canbedone + "</td></tr>"
                            + "<tr><td><font color='blue'>Cleared By Supervisor :</font></td><td>" + CleredBySupervisor + "</td></tr>"
                             + "<tr><td><font color='blue'>HOU Comment :</font></td><td>" + missionAuthorization12.Comment1 + "</td></tr>"
                             + "<tr><td><font color='blue'>Security Comment :</font></td><td>" + missionAuthorization12.Comment2 + "</td></tr>"
                             + "<tr><td><font color='blue'>Finance Comment :</font></td><td>" + missionAuthorization12.Comment3 + "</td></tr>"
                            + "<tr><td><font color='blue'>	BSITF :</font></td><td>" + BSITF2 + "</td></tr>"
                            + "<tr><td><font color='blue'> ASITF :</font></td><td>" + ASITF2 + "</td></tr>"
                            + "<tr><td><font color='blue'> SSAFE :</font></td><td>" + SSAFE2 + "</td></tr>"
                            + "<tr><td><font color='blue'> DSS Approval:</font></td><td><a href='http://10.11.135.254:8080/RBC-SMS/UploadedFiles/" + missionAuthorization12.non6 + "'>" + missionAuthorization12.non6 + "</a></td></tr></table>";

                        //string Body = "Request : Mission &nbsp;  &nbsp;  &nbsp; &nbsp;  &nbsp; &nbsp; &nbsp; &nbsp;  Mission ID : " + missionAuthorization.MissionID + " <br> " + " From &nbsp; &nbsp; &nbsp;:" + staffs.First().staff_first_name + "&nbsp;" + staffs.First().staff_last_name + "<br><br>" + " Mission Information" + "<br><br>" + " From :" + missionAuthorization.FromDate + "<br>To :" + missionAuthorization.ToDate +
                        //    "<br>Funding :" + missionAuthorization.funding + "<br>Objective :" + missionAuthorization.MissionObjective + "<br> Mission Itinerary :" + missionAuthorization.MissionItinerary + "<br> Can This meeting Be Done Over Voice/Video ? :" + missionAuthorization.canbedone  + " <br><br> <a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor=" + true + "&FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + " &to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Acept</a>&nbsp;&nbsp;&nbsp;<a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor= " + false + "& FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + "&to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Reject</a> ";

                        mailhou.Body = Bodyhou;
                        mailhou.IsBodyHtml = true;
                        SmtpClient smtphou = new SmtpClient();
                        smtphou.Host = "smtprelay.global.wfp.org";
                        smtphou.Port = 25;
                        smtphou.UseDefaultCredentials = true;
                        //smtp.Credentials = new System.Net.NetworkCredential
                        //("ahmed.badr", "Survivor2323");// Enter seders User name and password  
                        //smtp.EnableSsl = true;
                        smtphou.Send(mailhou);
                        return RedirectToAction("Index");
                    }

                    else
                    {
                        var canbedone = "";
                        if (missionAuthorization12.canbedone == true)
                        {
                            canbedone = "Yes";
                        }
                        else
                        {
                            canbedone = "No";
                        }

                        var CleredBySupervisor = "";
                        if (missionAuthorization12.canbedone == true)
                        {
                            CleredBySupervisor = "Yes";
                        }
                        else
                        {
                            CleredBySupervisor = "No";
                        }

                        var BSITF2 = "";
                        if (missionAuthorization12.BSITF == true)
                        {
                            BSITF2 = "Yes";
                        }
                        else
                        {
                            BSITF2 = "No";
                        }
                        ////////  
                        var ASITF2 = "";
                        if (missionAuthorization12.ASITF == true)
                        {
                            ASITF2 = "Yes";
                        }
                        else
                        {
                            ASITF2 = "No";
                        }
                        ////////
                        var SSAFE2 = "";
                        if (missionAuthorization12.SSAFE == true)
                        {
                            SSAFE2 = "Yes";
                        }
                        else
                        {
                            SSAFE2 = "No";
                        }

                        //////////////////////////////


                        var supersforhou = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                        supersforhou = supersforhou.Where(d => d.staffid == staffsupervisor);
                        var superforhounitsname = supersforhou.First().staff_first_name + " " + staffshou.First().staff_last_name;
                        var houmailUPDATE = supersforhou.First().staff_email;

                        if (houmailUPDATE == "muhannad.hadi@wfp.org" || houmailUPDATE == "carlo.scaramella@wfp.org")
                        {
                            houmailUPDATE = "rbc.management@wfp.org"; //rbc.management@wfp.org

                        }
                        /////////////////////////

                        MailMessage mailhou = new MailMessage();
                        mailhou.To.Add(onbehalfemail);
                        mailhou.Bcc.Add("ahmed.badr@wfp.org");
                        mailhou.CC.Add(onbehafsuperemail);
                        mailhou.From = new MailAddress(hou);
                        mailhou.Subject = "CD Rejected";

                        string Bodyhou = "<table><tr><td colspan='2'>" + "<hr/>New Mission<hr/></td></tr><tr><td><font color='blue'>Request : </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorization12.MissionID + "</td></tr>"
                            + "<tr><td>" + "<font color='blue'>From :</font></td><td>" + staffss.First().staff_first_name + " " + staffss.First().staff_last_name + "</td></tr>"
                            + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                            + "<tr><td><font color='blue'>From :</font></td><td>" + missionAuthorization12.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                            + "<tr><td><font color='blue'>To :</font></td><td>" + missionAuthorization12.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                            + "<tr><td><font color='blue'>Funding :</font></td><td>" + missionAuthorization12.funding + "</td></tr>"
                            + "<tr><td><font color='blue'>Objective :</font></td><td>" + missionAuthorization12.MissionObjective + "</td></tr>"
                            + "<tr><td><font color='blue'>Mission Itinerary :</font></td><td>" + missionAuthorization12.MissionItinerary + "</td></tr>"
                            + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video ? :</font></td><td>" + canbedone + "</td></tr>"
                            + "<tr><td><font color='blue'>Cleared By Supervisor :</font></td><td>" + CleredBySupervisor + "</td></tr>"
                             + "<tr><td><font color='blue'>HOU Comment :</font></td><td>" + missionAuthorization12.Comment1 + "</td></tr>"
                             + "<tr><td><font color='blue'>HOU Comment :</font></td><td>" + missionAuthorization12.Comment2 + "</td></tr>"
                             + "<tr><td><font color='blue'>HOU Comment :</font></td><td>" + missionAuthorization12.Comment3 + "</td></tr>"
                            + "<tr><td><font color='blue'>	BSITF :</font></td><td>" + BSITF2 + "</td></tr>"
                            + "<tr><td><font color='blue'> ASITF :</font></td><td>" + ASITF2 + "</td></tr>"
                            + "<tr><td><font color='blue'> SSAFE :</font></td><td>" + SSAFE2 + "</td></tr>"
                           + "<tr><td><font color='blue'> DSS Approval:</font></td><td><a href='http://10.11.135.254:8080/RBC-SMS/UploadedFiles/" + missionAuthorization12.non6 + "'>" + missionAuthorization12.non6 + "</a></td></tr></table>";

                        //string Body = "Request : Mission &nbsp;  &nbsp;  &nbsp; &nbsp;  &nbsp; &nbsp; &nbsp; &nbsp;  Mission ID : " + missionAuthorization.MissionID + " <br> " + " From &nbsp; &nbsp; &nbsp;:" + staffs.First().staff_first_name + "&nbsp;" + staffs.First().staff_last_name + "<br><br>" + " Mission Information" + "<br><br>" + " From :" + missionAuthorization.FromDate + "<br>To :" + missionAuthorization.ToDate +
                        //    "<br>Funding :" + missionAuthorization.funding + "<br>Objective :" + missionAuthorization.MissionObjective + "<br> Mission Itinerary :" + missionAuthorization.MissionItinerary + "<br> Can This meeting Be Done Over Voice/Video ? :" + missionAuthorization.canbedone  + " <br><br> <a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor=" + true + "&FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + " &to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Acept</a>&nbsp;&nbsp;&nbsp;<a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor= " + false + "& FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + "&to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Reject</a> ";

                        mailhou.Body = Bodyhou;
                        mailhou.IsBodyHtml = true;
                        SmtpClient smtphou = new SmtpClient();
                        smtphou.Host = "smtprelay.global.wfp.org";
                        smtphou.Port = 25;
                        smtphou.UseDefaultCredentials = true;
                        //smtp.Credentials = new System.Net.NetworkCredential
                        //("ahmed.badr", "Survivor2323");// Enter seders User name and password  
                        //smtp.EnableSsl = true;
                        smtphou.Send(mailhou);
                        return RedirectToAction("Index");


                    }
                }



            }
            ///////////////////////// end on behalf 
            else
            {
                ViewBag.Mid = Request.QueryString["MissionID"];
                ViewBag.RDORDRDSignature = Request.QueryString["RDORDRDSignature"];
                ViewBag.FromDate = Request.QueryString["FromDate"];
                ViewBag.ToDate = Request.QueryString["ToDate"];
                if (RDORDRDSignature == "Yes")
                {
                    try
                    {
                        // missionAuthorization = db.MissionAuthorizations.Find(Mid);
                        //db.Entry(missionAuthorization).State = EntityState.Modified ;
                        //db.SaveChanges();

                        var missionAuthorization = db.MissionAuthorization1.Single(u => u.MissionID == MissionID);
                        missionAuthorization.non12 = true;
                        missionAuthorization.non13 = DateTime.Now;
                        missionAuthorization.non14 = non14;
                        db.SaveChanges();



                        var staffs = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                        staffs = staffs.Where(s => s.staff_email.Contains(staff));
                        var too = staffs.First().staff_email;
                        var staffsupervisor = staffs.First().staff_supervisorid;
                        var supers = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                        supers = supers.Where(d => d.staffid == staffsupervisor);
                        var from = supers.First().staff_email;

                        //var rd = db.staffs.Include(r => r.contract_details).Include(r => r.contract_type).Include(r => r.Country_office).Include(r => r.functional_title).Include(r => r.staff2).Include(r => r.sub_office).Include(r => r.unit).Include(r => r.country).Include(r => r.governorate);
                        //rd = rd.Where(r => r.functional_title.functional_title_description_english == "REGIONAL DIRECTOR");
                        //var rdmail = rd.First().staff_email;

                        //var drd = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                        //drd = drd.Where(s => s.functional_title.functional_title_description_english == "Deputy Regional Director");
                        //var drdmail = drd.First().staff_email;

                        var userUnit = staffs.First().unit_id;
                        var unithou = db.units.Include(d => d.staffs);
                        unithou = unithou.Where(u => u.unitid == userUnit);
                        var houid = unithou.First().HOU_ID;

                        var staffshou = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                        staffshou = staffshou.Where(h => h.staffid == houid);
                        var hou = staffshou.First().staff_email;
                        //var houname = staffshou.First().staff_first_name + " " + staffshou.First().staff_last_name;

                        var cc = supers.First().staff_email;
                        var supername = supers.First().staff_first_name + " " + supers.First().staff_last_name;
                        var superemail = supers.First().staff_email;

                        //
                        //
                        //hou email
                        if (staffs.First().staffid == houid)
                        {
                            var canbedone = "";
                            if (missionAuthorization.canbedone == true)
                            {
                                canbedone = "Yes";
                            }
                            else
                            {
                                canbedone = "No";
                            }

                            var CleredBySupervisor = "";
                            if (missionAuthorization.canbedone == true)
                            {
                                CleredBySupervisor = "Yes";
                            }
                            else
                            {
                                CleredBySupervisor = "No";
                            }

                            var BSITF2 = "";
                            if (missionAuthorization.BSITF == true)
                            {
                                BSITF2 = "Yes";
                            }
                            else
                            {
                                BSITF2 = "No";
                            }
                            ////////  
                            var ASITF2 = "";
                            if (missionAuthorization.ASITF == true)
                            {
                                ASITF2 = "Yes";
                            }
                            else
                            {
                                ASITF2 = "No";
                            }
                            ////////
                            var SSAFE2 = "";
                            if (missionAuthorization.SSAFE == true)
                            {
                                SSAFE2 = "Yes";
                            }
                            else
                            {
                                SSAFE2 = "No";
                            }
                            var Roming = "";
                            if (missionAuthorization.StaffSignature == "True")
                            {
                                Roming = "Yes";
                            }
                            else
                            {
                                Roming = "No";
                            }
                            var odcitcc = "";
                            if (Roming == "Yes")
                            {
                                odcitcc = "cairo.itservicedesk@wfp.org";
                            }
                            else
                            {
                                odcitcc = "ahmed.badr@wfp.org";
                            }
                            //////////////////////////////


                            var supersforhou = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                            supersforhou = supersforhou.Where(d => d.staffid == staffsupervisor);
                            var superforhounitsname = supersforhou.First().staff_first_name + " " + staffshou.First().staff_last_name;
                            var houmailUPDATE = supersforhou.First().staff_email;

                            if (houmailUPDATE == "muhannad.hadi@wfp.org" || houmailUPDATE == "carlo.scaramella@wfp.org")
                            {
                                houmailUPDATE = "rbc.management@wfp.org"; //rbc.management@wfp.org

                            }
                            /////////////////////////

                            MailMessage mailhou = new MailMessage();
                            mailhou.To.Add(staff);
                            mailhou.Bcc.Add("ahmed.badr@wfp.org");
                            mailhou.Bcc.Add(odcitcc); //travel unit
                            //mailhou.CC.Add("rbc.travel@wfp.org"); //travel unit
                            //mailhou.CC.Add("mai.kenawi@wfp.org"); //travel unit
                            //mailhou.CC.Add("amal.mohamed@wfp.org"); //travel unit
                            //mailhou.CC.Add("ayten.el-sheikh@wfp.org"); //travel unit
                            //mailhou.CC.Add("essraa.soliman@wfp.org"); //travel unit
                            //mailhou.CC.Add("rbc.management@wfp.org"); //RD/DRD Office
                            mailhou.From = new MailAddress("RBC-SMS-SYSTEM@wfp.org");
                            mailhou.Subject = "CD Approved";

                            string Bodyhou = "<table><tr><td colspan='2'>" + "<hr/>New Mission<hr/></td></tr><tr><td><font color='blue'>Request : </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorization.MissionID + "</td></tr>"
                                + "<tr><td>" + "<font color='blue'>From :</font></td><td>" + staffs.First().staff_first_name + " " + staffs.First().staff_last_name + "</td></tr>"
                                + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                                + "<tr><td><font color='blue'>From :</font></td><td>" + missionAuthorization.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                                + "<tr><td><font color='blue'>To :</font></td><td>" + missionAuthorization.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                                + "<tr><td><font color='blue'>Funding :</font></td><td>" + missionAuthorization.funding + "</td></tr>"
                                + "<tr><td><font color='blue'>Objective :</font></td><td>" + missionAuthorization.MissionObjective + "</td></tr>"
                                + "<tr><td><font color='blue'>Mission Itinerary :</font></td><td>" + missionAuthorization.MissionItinerary + "</td></tr>"
                                + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video ? :</font></td><td>" + canbedone + "</td></tr>"
                                + "<tr><td><font color='blue'>Cleared By Supervisor :</font></td><td>" + CleredBySupervisor + "</td></tr>"
                                + "<tr><td><font color='blue'>International Roaming :</font></td><td>" + Roming + "</td></tr>"
                                 + "<tr><td><font color='blue'>HOU Comment :</font></td><td>" + missionAuthorization.Comment1 + "</td></tr>"
                                  + "<tr><td><font color='blue'>Security Comment :</font></td><td>" + missionAuthorization.Comment2 + "</td></tr>"
                                   + "<tr><td><font color='blue'>Finance Comment :</font></td><td>" + missionAuthorization.Comment3 + "</td></tr>"
                                + "<tr><td><font color='blue'>	BSITF :</font></td><td>" + BSITF2 + "</td></tr>"
                                + "<tr><td><font color='blue'> ASITF :</font></td><td>" + ASITF2 + "</td></tr>"
                                + "<tr><td><font color='blue'> SSAFE :</font></td><td>" + SSAFE2 + "</td></tr>"
                               + "<tr><td><font color='blue'> DSS Approval:</font></td><td><a target='_blank' href='http://10.11.135.254:8080/RBC-SMS/UploadedFiles/" + missionAuthorization.non6 + "'>" + missionAuthorization.non6 + "</a></td></tr></table>";

                            //string Body = "Request : Mission &nbsp;  &nbsp;  &nbsp; &nbsp;  &nbsp; &nbsp; &nbsp; &nbsp;  Mission ID : " + missionAuthorization.MissionID + " <br> " + " From &nbsp; &nbsp; &nbsp;:" + staffs.First().staff_first_name + "&nbsp;" + staffs.First().staff_last_name + "<br><br>" + " Mission Information" + "<br><br>" + " From :" + missionAuthorization.FromDate + "<br>To :" + missionAuthorization.ToDate +
                            //    "<br>Funding :" + missionAuthorization.funding + "<br>Objective :" + missionAuthorization.MissionObjective + "<br> Mission Itinerary :" + missionAuthorization.MissionItinerary + "<br> Can This meeting Be Done Over Voice/Video ? :" + missionAuthorization.canbedone  + " <br><br> <a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor=" + true + "&FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + " &to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Acept</a>&nbsp;&nbsp;&nbsp;<a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor= " + false + "& FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + "&to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Reject</a> ";

                            mailhou.Body = Bodyhou;
                            mailhou.IsBodyHtml = true;
                            SmtpClient smtphou = new SmtpClient();
                            smtphou.Host = "smtprelay.global.wfp.org";
                            smtphou.Port = 25;
                            smtphou.UseDefaultCredentials = true;
                            //smtp.Credentials = new System.Net.NetworkCredential
                            //("ahmed.badr", "Survivor2323");// Enter seders User name and password  
                            //smtp.EnableSsl = true;
                            smtphou.Send(mailhou);


                            ///////////////////////////// CD  Approval //////////////////////

                            MailMessage mailhou1 = new MailMessage();
                            mailhou1.To.Add("ahmed.badr@wfp.org");
                            mailhou1.Bcc.Add("ahmed.badr@wfp.org");

                            //mailhou.CC.Add("rbc.travel@wfp.org"); //travel unit
                            //mailhou.CC.Add("mai.kenawi@wfp.org"); //travel unit
                            //mailhou.CC.Add("amal.mohamed@wfp.org"); //travel unit
                            //mailhou.CC.Add("ayten.el-sheikh@wfp.org"); //travel unit
                            //mailhou.CC.Add("essraa.soliman@wfp.org"); //travel unit
                            //mailhou.CC.Add("rbc.management@wfp.org"); //RD/DRD Office
                            mailhou1.Bcc.Add(odcitcc); //travel unit
                            mailhou1.From = new MailAddress("RBC-SMS-SYSTEM@wfp.org");
                            mailhou1.Subject = "For CD Approval";

                            string Bodyhou1 = "<table><tr><td colspan='2'>" + "<hr/>New Mission<hr/></td></tr><tr><td><font color='blue'>Request : </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorization.MissionID + "</td></tr>"
                                + "<tr><td>" + "<font color='blue'>From :</font></td><td>" + staffs.First().staff_first_name + " " + staffs.First().staff_last_name + "</td></tr>"
                                + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                                + "<tr><td><font color='blue'>From :</font></td><td>" + missionAuthorization.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                                + "<tr><td><font color='blue'>To :</font></td><td>" + missionAuthorization.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                                + "<tr><td><font color='blue'>Funding :</font></td><td>" + missionAuthorization.funding + "</td></tr>"
                                + "<tr><td><font color='blue'>Objective :</font></td><td>" + missionAuthorization.MissionObjective + "</td></tr>"
                                + "<tr><td><font color='blue'>Mission Itinerary :</font></td><td>" + missionAuthorization.MissionItinerary + "</td></tr>"
                                + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video ? :</font></td><td>" + canbedone + "</td></tr>"
                                + "<tr><td><font color='blue'>Cleared By Supervisor :</font></td><td>" + CleredBySupervisor + "</td></tr>"
                                + "<tr><td><font color='blue'>International Roaming :</font></td><td>" + Roming + "</td></tr>"
                                + "<tr><td><font color='blue'>HOU Comment :</font></td><td>" + missionAuthorization.Comment1 + "</td></tr>"
                                 + "<tr><td><font color='blue'>Security Comment :</font></td><td>" + missionAuthorization.Comment2 + "</td></tr>"
                                  + "<tr><td><font color='blue'>Finance Comment :</font></td><td>" + missionAuthorization.Comment3 + "</td></tr>"
                                + "<tr><td><font color='blue'>	BSITF :</font></td><td>" + BSITF2 + "</td></tr>"
                                + "<tr><td><font color='blue'> ASITF :</font></td><td>" + ASITF2 + "</td></tr>"
                                + "<tr><td><font color='blue'> SSAFE :</font></td><td>" + SSAFE2 + "</td></tr>"
                                + "<tr><td><font color='blue'> DSS Approval:</font></td><td><a target='_blank' href='http://10.11.135.254:8080/RBC-SMS/UploadedFiles/" + missionAuthorization.non6 + "'>" + missionAuthorization.non6

                             + "<tr><td><a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorization1/Approval2FinAdmin?MissionID2="
                            + missionAuthorization.MissionID
                            + "&RDORDRDSignature2=Yes&FromDate2=" + missionAuthorization.FromDate
                            + "&ToDate2=" + missionAuthorization.ToDate
                            + "&to2=" + non10 + "&staffmail2=" + staffs.First().staff_email
                            + "'>Cleared</a></td><td><a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorization1/Approval2FinAdmin?MissionID2=" + missionAuthorization.MissionID
                            + "&RDORDRDSignature2=NO & FromDate2=" + missionAuthorization.FromDate
                            + "&ToDate2=" + missionAuthorization.ToDate
                            + "&to2=" + non10
                            + "&staffmail2=" + staffs.First().staff_email + "'>Not Cleared</a>" + "</a></td></tr></table>";

                            //string Body = "Request : Mission &nbsp;  &nbsp;  &nbsp; &nbsp;  &nbsp; &nbsp; &nbsp; &nbsp;  Mission ID : " + missionAuthorization.MissionID + " <br> " + " From &nbsp; &nbsp; &nbsp;:" + staffs.First().staff_first_name + "&nbsp;" + staffs.First().staff_last_name + "<br><br>" + " Mission Information" + "<br><br>" + " From :" + missionAuthorization.FromDate + "<br>To :" + missionAuthorization.ToDate +
                            //    "<br>Funding :" + missionAuthorization.funding + "<br>Objective :" + missionAuthorization.MissionObjective + "<br> Mission Itinerary :" + missionAuthorization.MissionItinerary + "<br> Can This meeting Be Done Over Voice/Video ? :" + missionAuthorization.canbedone  + " <br><br> <a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor=" + true + "&FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + " &to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Acept</a>&nbsp;&nbsp;&nbsp;<a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor= " + false + "& FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + "&to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Reject</a> ";

                            mailhou1.Body = Bodyhou1;
                            mailhou1.IsBodyHtml = true;
                            SmtpClient smtphou1 = new SmtpClient();
                            smtphou1.Host = "smtprelay.global.wfp.org";
                            smtphou1.Port = 25;
                            smtphou1.UseDefaultCredentials = true;
                            //smtp.Credentials = new System.Net.NetworkCredential
                            //("ahmed.badr", "Survivor2323");// Enter seders User name and password  
                            //smtp.EnableSsl = true;
                            //smtphou1.Send(mailhou1);

                            ///////////////////////////// CD Approval end //////////////////////

                            return RedirectToAction("Index");
                        }

                        else
                        {
                            var canbedone = "";
                            if (missionAuthorization.canbedone == true)
                            {
                                canbedone = "Yes";
                            }
                            else
                            {
                                canbedone = "No";
                            }

                            var CleredBySupervisor = "";
                            if (missionAuthorization.canbedone == true)
                            {
                                CleredBySupervisor = "Yes";
                            }
                            else
                            {
                                CleredBySupervisor = "No";
                            }

                            var BSITF2 = "";
                            if (missionAuthorization.BSITF == true)
                            {
                                BSITF2 = "Yes";
                            }
                            else
                            {
                                BSITF2 = "No";
                            }
                            ////////  
                            var ASITF2 = "";
                            if (missionAuthorization.ASITF == true)
                            {
                                ASITF2 = "Yes";
                            }
                            else
                            {
                                ASITF2 = "No";
                            }
                            ////////
                            var SSAFE2 = "";
                            if (missionAuthorization.SSAFE == true)
                            {
                                SSAFE2 = "Yes";
                            }
                            else
                            {
                                SSAFE2 = "No";
                            }

                            //////////////////////////////


                            var supersforhou = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                            supersforhou = supersforhou.Where(d => d.staffid == staffsupervisor);
                            var superforhounitsname = supersforhou.First().staff_first_name + " " + staffshou.First().staff_last_name;
                            var houmailUPDATE = supersforhou.First().staff_email;

                            if (houmailUPDATE == "muhannad.hadi@wfp.org" || houmailUPDATE == "carlo.scaramella@wfp.org")
                            {
                                houmailUPDATE = "rbc.management@wfp.org"; //rbc.management@wfp.org

                            }
                            var odcitcc = "";

                            var Roming = "";
                            if (missionAuthorization.StaffSignature == "True")
                            {
                                Roming = "Yes";
                            }
                            else
                            {
                                Roming = "No";
                            }
                            if (Roming == "Yes")
                            {
                                odcitcc = "cairo.itservicedesk@wfp.org";
                            }
                            else
                            {
                                odcitcc = "ahmed.badr@wfp.org";
                            }
                            /////////////////////////

                            MailMessage mailhou = new MailMessage();
                            mailhou.To.Add(staff); //staffmail
                            mailhou.Bcc.Add("ahmed.badr@wfp.org");
                            mailhou.Bcc.Add(odcitcc); //travel unit
                            mailhou.CC.Add(superemail);
                            //mailhou.CC.Add("rbc.travel@wfp.org"); //travel unit
                            //mailhou.CC.Add("mai.kenawi@wfp.org"); //travel unit
                            //mailhou.CC.Add("amal.mohamed@wfp.org"); //travel unit
                            //mailhou.CC.Add("ayten.el-sheikh@wfp.org"); //travel unit
                            //mailhou.CC.Add("essraa.soliman@wfp.org"); //travel unit
                            //mailhou.CC.Add("rbc.management@wfp.org"); //RD/DRD Office
                            mailhou.From = new MailAddress("RBC-SMS-SYSTEM@wfp.org");
                            mailhou.Subject = "CD Approved";

                            string Bodyhou = "<table><tr><td colspan='2'>" + "<hr/>New Mission<hr/></td></tr><tr><td><font color='blue'>Request : </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorization.MissionID + "</td></tr>"
                                + "<tr><td>" + "<font color='blue'>From :</font></td><td>" + staffs.First().staff_first_name + " " + staffs.First().staff_last_name + "</td></tr>"
                                + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                                + "<tr><td><font color='blue'>From :</font></td><td>" + missionAuthorization.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                                + "<tr><td><font color='blue'>To :</font></td><td>" + missionAuthorization.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                                + "<tr><td><font color='blue'>Funding :</font></td><td>" + missionAuthorization.funding + "</td></tr>"
                                + "<tr><td><font color='blue'>Objective :</font></td><td>" + missionAuthorization.MissionObjective + "</td></tr>"
                                + "<tr><td><font color='blue'>Mission Itinerary :</font></td><td>" + missionAuthorization.MissionItinerary + "</td></tr>"
                                + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video ? :</font></td><td>" + canbedone + "</td></tr>"
                                + "<tr><td><font color='blue'>Cleared By Supervisor :</font></td><td>" + CleredBySupervisor + "</td></tr>"
                                + "<tr><td><font color='blue'>International Roaming :</font></td><td>" + Roming + "</td></tr>"
                                 + "<tr><td><font color='blue'>HOU Comment :</font></td><td>" + missionAuthorization.Comment1 + "</td></tr>"
                                  + "<tr><td><font color='blue'>Security Comment :</font></td><td>" + missionAuthorization.Comment2 + "</td></tr>"
                                   + "<tr><td><font color='blue'>Finance Comment :</font></td><td>" + missionAuthorization.Comment3 + "</td></tr>"
                                + "<tr><td><font color='blue'>	BSITF :</font></td><td>" + BSITF2 + "</td></tr>"
                                + "<tr><td><font color='blue'> ASITF :</font></td><td>" + ASITF2 + "</td></tr>"
                                + "<tr><td><font color='blue'> SSAFE :</font></td><td>" + SSAFE2 + "</td></tr>"
                               + "<tr><td><font color='blue'> DSS Approval:</font></td><td><a target='_blank' href='http://10.11.135.254:8080/RBC-SMS/UploadedFiles/" + missionAuthorization.non6 + "'>" + missionAuthorization.non6 + "</a></td></tr></table>";

                            //string Body = "Request : Mission &nbsp;  &nbsp;  &nbsp; &nbsp;  &nbsp; &nbsp; &nbsp; &nbsp;  Mission ID : " + missionAuthorization.MissionID + " <br> " + " From &nbsp; &nbsp; &nbsp;:" + staffs.First().staff_first_name + "&nbsp;" + staffs.First().staff_last_name + "<br><br>" + " Mission Information" + "<br><br>" + " From :" + missionAuthorization.FromDate + "<br>To :" + missionAuthorization.ToDate +
                            //    "<br>Funding :" + missionAuthorization.funding + "<br>Objective :" + missionAuthorization.MissionObjective + "<br> Mission Itinerary :" + missionAuthorization.MissionItinerary + "<br> Can This meeting Be Done Over Voice/Video ? :" + missionAuthorization.canbedone  + " <br><br> <a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor=" + true + "&FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + " &to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Acept</a>&nbsp;&nbsp;&nbsp;<a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor= " + false + "& FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + "&to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Reject</a> ";

                            mailhou.Body = Bodyhou;
                            mailhou.IsBodyHtml = true;
                            SmtpClient smtphou = new SmtpClient();
                            smtphou.Host = "smtprelay.global.wfp.org";
                            smtphou.Port = 25;
                            smtphou.UseDefaultCredentials = true;
                            //smtp.Credentials = new System.Net.NetworkCredential
                            //("ahmed.badr", "Survivor2323");// Enter seders User name and password  
                            //smtp.EnableSsl = true;
                            smtphou.Send(mailhou);

                            ///////////////////////////// CD  Approval //////////////////////

                            MailMessage mailhou1 = new MailMessage();
                            mailhou1.To.Add("ahmed.badr@wfp.org");
                            mailhou1.Bcc.Add("ahmed.badr@wfp.org");

                            //mailhou.CC.Add("rbc.travel@wfp.org"); //travel unit
                            //mailhou.CC.Add("mai.kenawi@wfp.org"); //travel unit
                            //mailhou.CC.Add("amal.mohamed@wfp.org"); //travel unit
                            //mailhou.CC.Add("ayten.el-sheikh@wfp.org"); //travel unit
                            //mailhou.CC.Add("essraa.soliman@wfp.org"); //travel unit
                            //mailhou.CC.Add("rbc.management@wfp.org"); //RD/DRD Office
                            mailhou1.Bcc.Add(odcitcc); //travel unit
                            mailhou1.From = new MailAddress("RBC-SMS-SYSTEM@WFP.ORG");
                            mailhou1.Subject = "For CD Approval";

                            string Bodyhou1 = "<table><tr><td colspan='2'>" + "<hr/>New Mission<hr/></td></tr><tr><td><font color='blue'>Request : </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorization.MissionID + "</td></tr>"
                                + "<tr><td>" + "<font color='blue'>From :</font></td><td>" + staffs.First().staff_first_name + " " + staffs.First().staff_last_name + "</td></tr>"
                                + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                                + "<tr><td><font color='blue'>From :</font></td><td>" + missionAuthorization.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                                + "<tr><td><font color='blue'>To :</font></td><td>" + missionAuthorization.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                                + "<tr><td><font color='blue'>Funding :</font></td><td>" + missionAuthorization.funding + "</td></tr>"
                                + "<tr><td><font color='blue'>Objective :</font></td><td>" + missionAuthorization.MissionObjective + "</td></tr>"
                                + "<tr><td><font color='blue'>Mission Itinerary :</font></td><td>" + missionAuthorization.MissionItinerary + "</td></tr>"
                                + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video ? :</font></td><td>" + canbedone + "</td></tr>"
                                + "<tr><td><font color='blue'>Cleared By Supervisor :</font></td><td>" + CleredBySupervisor + "</td></tr>"
                                + "<tr><td><font color='blue'>International Roaming :</font></td><td>" + Roming + "</td></tr>"
                                + "<tr><td><font color='blue'>HOU Comment :</font></td><td>" + missionAuthorization.Comment1 + "</td></tr>"
                                + "<tr><td><font color='blue'>Security Comment :</font></td><td>" + missionAuthorization.Comment2 + "</td></tr>"
                                + "<tr><td><font color='blue'>Finance Comment :</font></td><td>" + missionAuthorization.Comment3 + "</td></tr>"
                                + "<tr><td><font color='blue'>	BSITF :</font></td><td>" + BSITF2 + "</td></tr>"
                                + "<tr><td><font color='blue'> ASITF :</font></td><td>" + ASITF2 + "</td></tr>"
                                + "<tr><td><font color='blue'> SSAFE :</font></td><td>" + SSAFE2 + "</td></tr>"
                                + "<tr><td><font color='blue'> DSS Approval:</font></td><td><a target='_blank' href='http://10.11.135.254:8080/RBC-SMS/UploadedFiles/" + missionAuthorization.non6 + "'>" + missionAuthorization.non6


                                 + "<tr><td><a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorization1/Approval2FinAdmin?MissionID2="
                            + missionAuthorization.MissionID
                            + "&RDORDRDSignature2=Yes&FromDate2=" + missionAuthorization.FromDate
                            + "&ToDate2=" + missionAuthorization.ToDate
                            + "&to2=" + non10 + "&staffmail2=" + staffs.First().staff_email
                            + "'>Cleared</a></td><td><a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorization1/Approval2FinAdmin?MissionID2=" + missionAuthorization.MissionID
                            + "&RDORDRDSignature2=NO & FromDate2=" + missionAuthorization.FromDate
                            + "&ToDate2=" + missionAuthorization.ToDate
                            + "&to2=" + non10
                            + "&staffmail2=" + staffs.First().staff_email + "'>Not Cleared</a>" + "</a></td></tr></table>";


                            //string Body = "Request : Mission &nbsp;  &nbsp;  &nbsp; &nbsp;  &nbsp; &nbsp; &nbsp; &nbsp;  Mission ID : " + missionAuthorization.MissionID + " <br> " + " From &nbsp; &nbsp; &nbsp;:" + staffs.First().staff_first_name + "&nbsp;" + staffs.First().staff_last_name + "<br><br>" + " Mission Information" + "<br><br>" + " From :" + missionAuthorization.FromDate + "<br>To :" + missionAuthorization.ToDate +
                            //    "<br>Funding :" + missionAuthorization.funding + "<br>Objective :" + missionAuthorization.MissionObjective + "<br> Mission Itinerary :" + missionAuthorization.MissionItinerary + "<br> Can This meeting Be Done Over Voice/Video ? :" + missionAuthorization.canbedone  + " <br><br> <a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor=" + true + "&FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + " &to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Acept</a>&nbsp;&nbsp;&nbsp;<a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor= " + false + "& FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + "&to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Reject</a> ";

                            mailhou1.Body = Bodyhou1;
                            mailhou1.IsBodyHtml = true;
                            SmtpClient smtphou1 = new SmtpClient();
                            smtphou1.Host = "smtprelay.global.wfp.org";
                            smtphou1.Port = 25;
                            smtphou1.UseDefaultCredentials = true;
                            //smtp.Credentials = new System.Net.NetworkCredential
                            //("ahmed.badr", "Survivor2323");// Enter seders User name and password  
                            //smtp.EnableSsl = true;
                            //smtphou1.Send(mailhou1);

                            ///////////////////////////// CD Approval //////////////////////

                            return RedirectToAction("Index");


                        }
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
                    return View();
                }
                else
                {
                    //var missionAuthorizationn = db.MissionAuthorizations.Single(u => u.MissionID == Mid);
                    //db.MissionAuthorizations.Remove(missionAuthorizationn);
                    //db.SaveChanges();
                    var missionAuthorizationn = db.MissionAuthorization1.Single(u => u.MissionID == MissionID);
                    missionAuthorizationn.non12 = false;
                    missionAuthorizationn.non13 = DateTime.Now;
                    missionAuthorizationn.non14 = non14;
                    db.SaveChanges();


                    var staffss = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                    staffss = staffss.Where(s => s.staff_email.Contains(staff));
                    var too = staffss.First().staff_email;
                    var staffsupervisor = staffss.First().staff_supervisorid;
                    var supers = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                    supers = supers.Where(d => d.staffid == staffsupervisor);
                    var from = supers.First().staff_email;

                    //var rd = db.staffs.Include(r => r.contract_details).Include(r => r.contract_type).Include(r => r.Country_office).Include(r => r.functional_title).Include(r => r.staff2).Include(r => r.sub_office).Include(r => r.unit).Include(r => r.country).Include(r => r.governorate);
                    //rd = rd.Where(r => r.functional_title.functional_title_description_english == "REGIONAL DIRECTOR");
                    //var rdmail = rd.First().staff_email;

                    //var drd = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                    //drd = drd.Where(s => s.functional_title.functional_title_description_english == "Deputy Regional Director");
                    //var drdmail = drd.First().staff_email;


                    //new
                    var userUnit = staffss.First().unit_id;
                    var unithou = db.units.Include(d => d.staffs);
                    unithou = unithou.Where(u => u.unitid == userUnit);
                    var houid = unithou.First().HOU_ID;

                    var staffshou = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                    staffshou = staffshou.Where(h => h.staffid == houid);
                    var hou = staffshou.First().staff_email;
                    //var houname = staffshou.First().staff_first_name + " " + staffshou.First().staff_last_name;

                    var cc = supers.First().staff_email;
                    var supername = supers.First().staff_first_name + " " + supers.First().staff_last_name;







                    var superemail = supers.First().staff_email;

                    //
                    //
                    //hou email
                    if (staffss.First().staffid == houid)
                    {
                        var canbedone = "";
                        if (missionAuthorizationn.canbedone == true)
                        {
                            canbedone = "Yes";
                        }
                        else
                        {
                            canbedone = "No";
                        }
                        var Roming = "";
                        if (missionAuthorizationn.StaffSignature == "True")
                        {
                            Roming = "Yes";
                        }
                        else
                        {
                            Roming = "No";
                        }
                        var CleredBySupervisor = "";
                        if (missionAuthorizationn.canbedone == true)
                        {
                            CleredBySupervisor = "Yes";
                        }
                        else
                        {
                            CleredBySupervisor = "No";
                        }

                        var BSITF2 = "";
                        if (missionAuthorizationn.BSITF == true)
                        {
                            BSITF2 = "Yes";
                        }
                        else
                        {
                            BSITF2 = "No";
                        }
                        ////////  
                        var ASITF2 = "";
                        if (missionAuthorizationn.ASITF == true)
                        {
                            ASITF2 = "Yes";
                        }
                        else
                        {
                            ASITF2 = "No";
                        }
                        ////////
                        var SSAFE2 = "";
                        if (missionAuthorizationn.SSAFE == true)
                        {
                            SSAFE2 = "Yes";
                        }
                        else
                        {
                            SSAFE2 = "No";
                        }
                        var odcitcc = "";
                        if (Roming == "Yes")
                        {
                            odcitcc = "cairo.itservicedesk@wfp.org";
                        }
                        else
                        {
                            odcitcc = "ahmed.badr@wfp.org";
                        }
                        //////////////////////////////


                        var supersforhou = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                        supersforhou = supersforhou.Where(d => d.staffid == staffsupervisor);
                        var superforhounitsname = supersforhou.First().staff_first_name + " " + staffshou.First().staff_last_name;
                        var houmailUPDATE = supersforhou.First().staff_email;

                        if (houmailUPDATE == "muhannad.hadi@wfp.org" || houmailUPDATE == "carlo.scaramella@wfp.org")
                        {
                            houmailUPDATE = "rbc.management@wfp.org"; //rbc.management@wfp.org

                        }
                        /////////////////////////

                        MailMessage mailhou = new MailMessage();
                        mailhou.To.Add(staff);
                        mailhou.Bcc.Add("ahmed.badr@wfp.org");
                        //mail2.CC.Add(cc);
                        mailhou.From = new MailAddress("RBC-SMS-SYSTEM@WFP.ORG");
                        mailhou.Subject = "CD Rejected";

                        string Bodyhou = "<table><tr><td colspan='2'>" + "<hr/>New Mission<hr/></td></tr><tr><td><font color='blue'>Request : </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorizationn.MissionID + "</td></tr>"
                            + "<tr><td>" + "<font color='blue'>From :</font></td><td>" + staffss.First().staff_first_name + " " + staffss.First().staff_last_name + "</td></tr>"
                            + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                            + "<tr><td><font color='blue'>From :</font></td><td>" + missionAuthorizationn.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                            + "<tr><td><font color='blue'>To :</font></td><td>" + missionAuthorizationn.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                            + "<tr><td><font color='blue'>Funding :</font></td><td>" + missionAuthorizationn.funding + "</td></tr>"
                            + "<tr><td><font color='blue'>Objective :</font></td><td>" + missionAuthorizationn.MissionObjective + "</td></tr>"
                            + "<tr><td><font color='blue'>Mission Itinerary :</font></td><td>" + missionAuthorizationn.MissionItinerary + "</td></tr>"
                            + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video ? :</font></td><td>" + canbedone + "</td></tr>"
                            + "<tr><td><font color='blue'>Cleared By Supervisor :</font></td><td>" + CleredBySupervisor + "</td></tr>"
                             + "<tr><td><font color='blue'>HOU Comment :</font></td><td>" + missionAuthorizationn.Comment1 + "</td></tr>"
                             + "<tr><td><font color='blue'>Security Comment :</font></td><td>" + missionAuthorizationn.Comment2 + "</td></tr>"
                             + "<tr><td><font color='blue'>Finance Comment :</font></td><td>" + missionAuthorizationn.Comment3 + "</td></tr>"
                            + "<tr><td><font color='blue'>	BSITF :</font></td><td>" + BSITF2 + "</td></tr>"
                            + "<tr><td><font color='blue'> ASITF :</font></td><td>" + ASITF2 + "</td></tr>"
                            + "<tr><td><font color='blue'> SSAFE :</font></td><td>" + SSAFE2 + "</td></tr>"
                           + "<tr><td><font color='blue'> DSS Approval:</font></td><td><a target='_blank' href='http://10.11.135.254:8080/RBC-SMS/UploadedFiles/" + missionAuthorizationn.non6 + "'>" + missionAuthorizationn.non6 + "</a></td></tr></table>";

                        //string Body = "Request : Mission &nbsp;  &nbsp;  &nbsp; &nbsp;  &nbsp; &nbsp; &nbsp; &nbsp;  Mission ID : " + missionAuthorization.MissionID + " <br> " + " From &nbsp; &nbsp; &nbsp;:" + staffs.First().staff_first_name + "&nbsp;" + staffs.First().staff_last_name + "<br><br>" + " Mission Information" + "<br><br>" + " From :" + missionAuthorization.FromDate + "<br>To :" + missionAuthorization.ToDate +
                        //    "<br>Funding :" + missionAuthorization.funding + "<br>Objective :" + missionAuthorization.MissionObjective + "<br> Mission Itinerary :" + missionAuthorization.MissionItinerary + "<br> Can This meeting Be Done Over Voice/Video ? :" + missionAuthorization.canbedone  + " <br><br> <a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor=" + true + "&FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + " &to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Acept</a>&nbsp;&nbsp;&nbsp;<a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor= " + false + "& FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + "&to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Reject</a> ";

                        mailhou.Body = Bodyhou;
                        mailhou.IsBodyHtml = true;
                        SmtpClient smtphou = new SmtpClient();
                        smtphou.Host = "smtprelay.global.wfp.org";
                        smtphou.Port = 25;
                        smtphou.UseDefaultCredentials = true;
                        //smtp.Credentials = new System.Net.NetworkCredential
                        //("ahmed.badr", "Survivor2323");// Enter seders User name and password  
                        //smtp.EnableSsl = true;
                        smtphou.Send(mailhou);
                        return RedirectToAction("Index");
                    }

                    else
                    {
                        var canbedone = "";
                        if (missionAuthorizationn.canbedone == true)
                        {
                            canbedone = "Yes";
                        }
                        else
                        {
                            canbedone = "No";
                        }

                        var CleredBySupervisor = "";
                        if (missionAuthorizationn.canbedone == true)
                        {
                            CleredBySupervisor = "Yes";
                        }
                        else
                        {
                            CleredBySupervisor = "No";
                        }

                        var BSITF2 = "";
                        if (missionAuthorizationn.BSITF == true)
                        {
                            BSITF2 = "Yes";
                        }
                        else
                        {
                            BSITF2 = "No";
                        }
                        ////////  
                        var ASITF2 = "";
                        if (missionAuthorizationn.ASITF == true)
                        {
                            ASITF2 = "Yes";
                        }
                        else
                        {
                            ASITF2 = "No";
                        }
                        ////////
                        var SSAFE2 = "";
                        if (missionAuthorizationn.SSAFE == true)
                        {
                            SSAFE2 = "Yes";
                        }
                        else
                        {
                            SSAFE2 = "No";
                        }

                        //////////////////////////////


                        var supersforhou = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                        supersforhou = supersforhou.Where(d => d.staffid == staffsupervisor);
                        var superforhounitsname = supersforhou.First().staff_first_name + " " + staffshou.First().staff_last_name;
                        var houmailUPDATE = supersforhou.First().staff_email;

                        if (houmailUPDATE == "muhannad.hadi@wfp.org" || houmailUPDATE == "carlo.scaramella@wfp.org")
                        {
                            houmailUPDATE = "rbc.management@wfp.org"; //rbc.management@wfp.org

                        }
                        /////////////////////////


                        MailMessage mailhou = new MailMessage();
                        mailhou.To.Add(staff);
                        mailhou.Bcc.Add("ahmed.badr@wfp.org");
                        mailhou.CC.Add(cc);
                        mailhou.From = new MailAddress(hou);
                        mailhou.Subject = "CD Rejected";

                        string Bodyhou = "<table><tr><td colspan='2'>" + "<hr/>New Mission<hr/></td></tr><tr><td><font color='blue'>Request : </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorizationn.MissionID + "</td></tr>"
                            + "<tr><td>" + "<font color='blue'>From :</font></td><td>" + staffss.First().staff_first_name + " " + staffss.First().staff_last_name + "</td></tr>"
                            + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                            + "<tr><td><font color='blue'>From :</font></td><td>" + missionAuthorizationn.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                            + "<tr><td><font color='blue'>To :</font></td><td>" + missionAuthorizationn.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                            + "<tr><td><font color='blue'>Funding :</font></td><td>" + missionAuthorizationn.funding + "</td></tr>"
                            + "<tr><td><font color='blue'>Objective :</font></td><td>" + missionAuthorizationn.MissionObjective + "</td></tr>"
                            + "<tr><td><font color='blue'>Mission Itinerary :</font></td><td>" + missionAuthorizationn.MissionItinerary + "</td></tr>"
                            + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video ? :</font></td><td>" + canbedone + "</td></tr>"
                            + "<tr><td><font color='blue'>Cleared By Supervisor :</font></td><td>" + CleredBySupervisor + "</td></tr>"
                             + "<tr><td><font color='blue'>HOU Comment :</font></td><td>" + missionAuthorizationn.Comment1 + "</td></tr>"
                             + "<tr><td><font color='blue'>Security Comment :</font></td><td>" + missionAuthorizationn.Comment2 + "</td></tr>"
                             + "<tr><td><font color='blue'>Finance Comment :</font></td><td>" + missionAuthorizationn.Comment3 + "</td></tr>"
                            + "<tr><td><font color='blue'>	BSITF :</font></td><td>" + BSITF2 + "</td></tr>"
                            + "<tr><td><font color='blue'> ASITF :</font></td><td>" + ASITF2 + "</td></tr>"
                            + "<tr><td><font color='blue'> SSAFE :</font></td><td>" + SSAFE2 + "</td></tr>"
                           + "<tr><td><font color='blue'> DSS Approval:</font></td><td><a target='_blank' href='http://10.11.135.254:8080/RBC-SMS/UploadedFiles/" + missionAuthorizationn.non6 + "'>" + missionAuthorizationn.non6 + "</a></td></tr></table>";

                        //string Body = "Request : Mission &nbsp;  &nbsp;  &nbsp; &nbsp;  &nbsp; &nbsp; &nbsp; &nbsp;  Mission ID : " + missionAuthorization.MissionID + " <br> " + " From &nbsp; &nbsp; &nbsp;:" + staffs.First().staff_first_name + "&nbsp;" + staffs.First().staff_last_name + "<br><br>" + " Mission Information" + "<br><br>" + " From :" + missionAuthorization.FromDate + "<br>To :" + missionAuthorization.ToDate +
                        //    "<br>Funding :" + missionAuthorization.funding + "<br>Objective :" + missionAuthorization.MissionObjective + "<br> Mission Itinerary :" + missionAuthorization.MissionItinerary + "<br> Can This meeting Be Done Over Voice/Video ? :" + missionAuthorization.canbedone  + " <br><br> <a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor=" + true + "&FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + " &to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Acept</a>&nbsp;&nbsp;&nbsp;<a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor= " + false + "& FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + "&to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Reject</a> ";

                        mailhou.Body = Bodyhou;
                        mailhou.IsBodyHtml = true;
                        SmtpClient smtphou = new SmtpClient();
                        smtphou.Host = "smtprelay.global.wfp.org";
                        smtphou.Port = 25;
                        smtphou.UseDefaultCredentials = true;
                        //smtp.Credentials = new System.Net.NetworkCredential
                        //("ahmed.badr", "Survivor2323");// Enter seders User name and password  
                        //smtp.EnableSsl = true;
                        smtphou.Send(mailhou);
                        return RedirectToAction("Index");


                    }
                }
            }
            return View();

        }


        ////////////////////////////////////////////////////////////////////////////////// end approval level 4 CD //////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ActionResult Approval(int? Mid, bool? ClearedBySupervisor, DateTime? FromDate, DateTime? ToDate, string staffmail, string supervisorname, int? onbehalfID, int? onbehalfSuperID, int? onbehalfHouID)
        {



            /////////////////////////////// start on behalf
            if (onbehalfID != null && onbehalfSuperID != null && onbehalfHouID != null)
            {


                var staffser = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                staffser = staffser.Where(s => s.staff_email.Contains(User.Identity.Name + "@wfp.org"));
                if (staffser.First().staff_supervisorid == null || staffser.First().staff_first_name == null || staffser.First().staff_last_name == null || staffser.First().unit_id == null)
                {

                    ModelState.AddModelError("staffid2", "Name is required");
                    ViewBag.staffid2 = "Name is required";

                }


                if (ModelState.IsValid)
                {

                    //db.MissionAuthorizations.Add(missionAuthorization);
                    try
                    {



                        //db.SaveChanges();
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
                    staffs = staffs.Where(s => s.staffid == onbehalfID);

                    var missionAuthorization = db.MissionAuthorization1.Single(u => u.MissionID == Mid);
                    missionAuthorization.non4 = "Yes";
                    db.SaveChanges();

                    //ViewBag.staffid = new SelectList(db.staffs, "staffs.First().staffid", staffs.First().staff_email);
                    ViewBag.staffid = staffs.First().staffid;
                    ViewBag.staffid2 = staffs.First().staff_first_name;
                    ViewBag.FirstName = staffs.First().staff_first_name;
                    ViewBag.LastName = staffs.First().staff_last_name;
                    ViewBag.Unit = staffs.First().unit.unit_description_english;
                    var staffsupervisor = staffs.First().staff_supervisorid;
                    var userUnit = staffs.First().unit_id;


                    /////////////sender
                    var sender = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                    sender = sender.Where(s => s.staff_email.Contains(User.Identity.Name + "@wfp.org"));
                    var from = sender.First().staff_email;
                    /////////////sender

                    var supers = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                    supers = supers.Where(d => d.staffid == staffsupervisor);


                    var unithou = db.units.Include(d => d.staffs);
                    unithou = unithou.Where(u => u.unitid == userUnit);
                    var houid = onbehalfHouID;

                    var staffshou = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                    staffshou = staffshou.Where(h => h.staffid == onbehalfHouID);
                    var to = staffshou.First().staff_email;
                    //var houname = staffshou.First().staff_first_name + " " + staffshou.First().staff_last_name;


                    var cc = supers.First().staff_email;
                    var supername = supers.First().staff_first_name + " " + supers.First().staff_last_name;


                    if (onbehalfID == houid)
                    {
                        var canbedone = "";
                        if (missionAuthorization.canbedone == true)
                        {
                            canbedone = "Yes";
                        }
                        else
                        {
                            canbedone = "No";
                        }

                        var CleredBySupervisor = "";
                        if (missionAuthorization.canbedone == true)
                        {
                            CleredBySupervisor = "Yes";
                        }
                        else
                        {
                            CleredBySupervisor = "No";
                        }

                        var BSITF2 = "";
                        if (missionAuthorization.BSITF == true)
                        {
                            BSITF2 = "Yes";
                        }
                        else
                        {
                            BSITF2 = "No";
                        }
                        ////////  
                        var ASITF2 = "";
                        if (missionAuthorization.ASITF == true)
                        {
                            ASITF2 = "Yes";
                        }
                        else
                        {
                            ASITF2 = "No";
                        }
                        ////////
                        var SSAFE2 = "";
                        if (missionAuthorization.SSAFE == true)
                        {
                            SSAFE2 = "Yes";
                        }
                        else
                        {
                            SSAFE2 = "No";
                        }

                        //////////////////////////////


                        var supersforhou = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                        supersforhou = supersforhou.Where(d => d.staffid == staffsupervisor);
                        var superforhounitsname = supersforhou.First().staff_first_name + " " + staffshou.First().staff_last_name;
                        var houmailUPDATE = supersforhou.First().staff_email;

                        if (houmailUPDATE == "muhannad.hadi@wfp.org" || houmailUPDATE == "carlo.scaramella@wfp.org")
                        {
                            houmailUPDATE = "rbc.management@wfp.org"; //rbc.management@wfp.org

                        }
                        /////////////////////////
                        var onbehalfof = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                        onbehalfof = onbehalfof.Where(d => d.staffid == onbehalfID);
                        var onbehalfofName = onbehalfof.First().staff_first_name + " " + onbehalfof.First().staff_last_name;
                        var onbehalfofEmail = onbehalfof.First().staff_email;
                        ///////////////////////////////////////////////////
                        MailMessage mailhou = new MailMessage();
                        mailhou.To.Add(houmailUPDATE); // RD/DRD OFFICE/rbc.management@wfp.org
                        //mail.CC.Add(cc);
                        mailhou.Bcc.Add("ahmed.badr@wfp.org");
                        mailhou.From = new MailAddress(from);
                        mailhou.Subject = "Mission Authorization Form" + " " + "on behalf of" + " " + onbehalfofName;

                        string Bodyhou = "<table><tr><td colspan='2'>" + "<hr/>New Mission<hr/></td></tr><tr><td><font color='blue'>Request : </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorization.MissionID + "</td></tr>"
                            + "<tr><td>" + "<font color='blue'>From :</font></td><td>" + staffs.First().staff_first_name + " " + staffs.First().staff_last_name + "</td></tr>"
                            + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                            + "<tr><td><font color='blue'>From :</font></td><td>" + missionAuthorization.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                            + "<tr><td><font color='blue'>To :</font></td><td>" + missionAuthorization.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                            + "<tr><td><font color='blue'>Funding :</font></td><td>" + missionAuthorization.funding + "</td></tr>"
                            + "<tr><td><font color='blue'>Objective :</font></td><td>" + missionAuthorization.MissionObjective + "</td></tr>"
                            + "<tr><td><font color='blue'>Mission Itinerary :</font></td><td>" + missionAuthorization.MissionItinerary + "</td></tr>"
                            + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video ? :</font></td><td>" + canbedone + "</td></tr>"
                            + "<tr><td><font color='blue'>Cleared By Supervisor :</font></td><td>" + CleredBySupervisor + "</td></tr>"

                            + "<tr><td><font color='blue'>	BSITF :</font></td><td>" + BSITF2 + "</td></tr>"
                            + "<tr><td><font color='blue'> ASITF :</font></td><td>" + ASITF2 + "</td></tr>"
                            + "<tr><td><font color='blue'> SSAFE :</font></td><td>" + SSAFE2 + "</td></tr>"
                            + "<tr><td><font color='blue'> DSS Approval:</font></td><td><a target='_blank' href='http://10.11.135.254:8080/RBC-SMS/UploadedFiles/" + missionAuthorization.non6 + "'>" + missionAuthorization.non6 + "</a></td></tr>"


                            //<a download='custom-filename.jpg' href="/path/to/image" title="ImageName">     <img alt="ImageName" src="/path/to/image"> </a>


                            + "<tr><td><a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorization1/Approval2?MissionID2="
                            + missionAuthorization.MissionID
                            + "&RDORDRDSignature2=Yes&FromDate2=" + missionAuthorization.FromDate
                            + " &ToDate2=" + missionAuthorization.ToDate
                            + " &to2=" + to + "&staffmail2=" + staffs.First().staff_email
                            + "'>Accept</a></td><td><a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorization1/Approval2?MissionID2=" + missionAuthorization.MissionID
                            + "&RDORDRDSignature2=NO & FromDate2=" + missionAuthorization.FromDate
                            + " &ToDate2=" + missionAuthorization.ToDate
                            + "&to2=" + to
                            + "&staffmail2=" + staffs.First().staff_email
                            + "'>Reject</a>" + "</td><td>" + "</td></tr></table>";

                        //string Body = "Request : Mission &nbsp;  &nbsp;  &nbsp; &nbsp;  &nbsp; &nbsp; &nbsp; &nbsp;  Mission ID : " + missionAuthorization.MissionID + " <br> " + " From &nbsp; &nbsp; &nbsp;:" + staffs.First().staff_first_name + "&nbsp;" + staffs.First().staff_last_name + "<br><br>" + " Mission Information" + "<br><br>" + " From :" + missionAuthorization.FromDate + "<br>To :" + missionAuthorization.ToDate +
                        //    "<br>Funding :" + missionAuthorization.funding + "<br>Objective :" + missionAuthorization.MissionObjective + "<br> Mission Itinerary :" + missionAuthorization.MissionItinerary + "<br> Can This meeting Be Done Over Voice/Video ? :" + missionAuthorization.canbedone  + " <br><br> <a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor=" + true + "&FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + " &to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Acept</a>&nbsp;&nbsp;&nbsp;<a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor= " + false + "& FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + "&to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Reject</a> ";

                        mailhou.Body = Bodyhou;
                        mailhou.IsBodyHtml = true;
                        SmtpClient smtphou = new SmtpClient();
                        smtphou.Host = "smtprelay.global.wfp.org";
                        smtphou.Port = 25;
                        smtphou.UseDefaultCredentials = true;
                        //smtp.Credentials = new System.Net.NetworkCredential
                        //("ahmed.badr", "Survivor2323");// Enter seders User name and password  
                        //smtp.EnableSsl = true;
                        smtphou.Send(mailhou);

                        return RedirectToAction("Index");


                    }

                    /////////////////////////////////////////////
                    var onbehalfof2 = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                    onbehalfof2 = onbehalfof2.Where(d => d.staffid == onbehalfID);
                    var onbehalfofName2 = onbehalfof2.First().staff_first_name + " " + onbehalfof2.First().staff_last_name;
                    var onbehalfofEmail2 = onbehalfof2.First().staff_email;
                    ///////////////////////////////////////////////

                    if (to == "muhannad.hadi@wfp.org" || to == "carlo.scaramella@wfp.org")
                    {
                        to = "rbc.management@wfp.org";//rbc.management@wfp.org

                    }


                    var canbedone1 = "";
                    if (missionAuthorization.canbedone == true)
                    {
                        canbedone1 = "Yes";
                    }
                    else
                    {
                        canbedone1 = "No";
                    }

                    var CleredBySupervisor1 = "";
                    if (missionAuthorization.canbedone == true)
                    {
                        CleredBySupervisor1 = "Yes";
                    }
                    else
                    {
                        CleredBySupervisor1 = "No";
                    }

                    var BSITF21 = "";
                    if (missionAuthorization.BSITF == true)
                    {
                        BSITF21 = "Yes";
                    }
                    else
                    {
                        BSITF21 = "No";
                    }
                    ////////  
                    var ASITF21 = "";
                    if (missionAuthorization.ASITF == true)
                    {
                        ASITF21 = "Yes";
                    }
                    else
                    {
                        ASITF21 = "No";
                    }
                    ////////
                    var SSAFE21 = "";
                    if (missionAuthorization.SSAFE == true)
                    {
                        SSAFE21 = "Yes";
                    }
                    else
                    {
                        SSAFE21 = "No";
                    }

                    //////////////////////////////


                    var supersforhou1 = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                    supersforhou1 = supersforhou1.Where(d => d.staffid == staffsupervisor);
                    var superforhounitsname1 = supersforhou1.First().staff_first_name + " " + staffshou.First().staff_last_name;
                    var houmailUPDATE1 = supersforhou1.First().staff_email;

                    if (houmailUPDATE1 == "muhannad.hadi@wfp.org" || houmailUPDATE1 == "carlo.scaramella@wfp.org")
                    {
                        houmailUPDATE1 = "rbc.management@wfp.org"; //rbc.management@wfp.org

                    }
                    /////////////////////////
                    var onbehalfof1 = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                    onbehalfof1 = onbehalfof1.Where(d => d.staffid == onbehalfID);
                    var onbehalfofName1 = onbehalfof1.First().staff_first_name + " " + onbehalfof1.First().staff_last_name;
                    var onbehalfofEmail1 = onbehalfof1.First().staff_email;
                    ///////////////////////////////////////////////////
                    MailMessage mailhou1 = new MailMessage();
                    mailhou1.To.Add(to); // var to
                    //mail.CC.Add(cc);
                    mailhou1.Bcc.Add("ahmed.badr@wfp.org");
                    mailhou1.From = new MailAddress(from);
                    mailhou1.Subject = "Mission Authorization Form on behalf of" + " " + onbehalfofName2;

                    string Bodyhou1 = "<table><tr><td colspan='2'>" + "<hr/>New Mission<hr/></td></tr><tr><td><font color='blue'>Request : </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorization.MissionID + "</td></tr>"
                        + "<tr><td>" + "<font color='blue'>From :</font></td><td>" + staffs.First().staff_first_name + " " + staffs.First().staff_last_name + "</td></tr>"
                        + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                        + "<tr><td><font color='blue'>From :</font></td><td>" + missionAuthorization.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                        + "<tr><td><font color='blue'>To :</font></td><td>" + missionAuthorization.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                        + "<tr><td><font color='blue'>Funding :</font></td><td>" + missionAuthorization.funding + "</td></tr>"
                        + "<tr><td><font color='blue'>Objective :</font></td><td>" + missionAuthorization.MissionObjective + "</td></tr>"
                        + "<tr><td><font color='blue'>Mission Itinerary :</font></td><td>" + missionAuthorization.MissionItinerary + "</td></tr>"
                        + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video ? :</font></td><td>" + canbedone1 + "</td></tr>"
                        + "<tr><td><font color='blue'>Cleared By Supervisor :</font></td><td>" + CleredBySupervisor1 + "</td></tr>"

                        + "<tr><td><font color='blue'>	BSITF :</font></td><td>" + BSITF21 + "</td></tr>"
                        + "<tr><td><font color='blue'> ASITF :</font></td><td>" + ASITF21 + "</td></tr>"
                        + "<tr><td><font color='blue'> SSAFE :</font></td><td>" + SSAFE21 + "</td></tr>"
                        + "<tr><td><font color='blue'> DSS Approval:</font></td><td><a target='_blank' href='http://10.11.135.254:8080/RBC-SMS/UploadedFiles/" + missionAuthorization.non6 + "'>" + missionAuthorization.non6 + "</a></td></tr>"


                        //<a download='custom-filename.jpg' href="/path/to/image" title="ImageName">     <img alt="ImageName" src="/path/to/image"> </a>


                        + "<tr><td><a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorization1/Approval2?MissionID2="
                        + missionAuthorization.MissionID
                        + "&RDORDRDSignature2=Yes&FromDate2=" + missionAuthorization.FromDate
                        + " &ToDate2=" + missionAuthorization.ToDate
                        + " &to2=" + to + "&staffmail2=" + staffs.First().staff_email
                        + "'>Accept</a></td><td><a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorization1/Approval2?MissionID2=" + missionAuthorization.MissionID
                        + "&RDORDRDSignature2=NO & FromDate2=" + missionAuthorization.FromDate
                        + " &ToDate2=" + missionAuthorization.ToDate
                        + "&to2=" + to
                        + "&staffmail2=" + staffs.First().staff_email
                        + "'>Reject</a>" + "</td><td>" + "</td></tr></table>";

                    //string Body = "Request : Mission &nbsp;  &nbsp;  &nbsp; &nbsp;  &nbsp; &nbsp; &nbsp; &nbsp;  Mission ID : " + missionAuthorization.MissionID + " <br> " + " From &nbsp; &nbsp; &nbsp;:" + staffs.First().staff_first_name + "&nbsp;" + staffs.First().staff_last_name + "<br><br>" + " Mission Information" + "<br><br>" + " From :" + missionAuthorization.FromDate + "<br>To :" + missionAuthorization.ToDate +
                    //    "<br>Funding :" + missionAuthorization.funding + "<br>Objective :" + missionAuthorization.MissionObjective + "<br> Mission Itinerary :" + missionAuthorization.MissionItinerary + "<br> Can This meeting Be Done Over Voice/Video ? :" + missionAuthorization.canbedone  + " <br><br> <a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor=" + true + "&FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + " &to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Acept</a>&nbsp;&nbsp;&nbsp;<a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor= " + false + "& FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + "&to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Reject</a> ";

                    mailhou1.Body = Bodyhou1;
                    mailhou1.IsBodyHtml = true;
                    SmtpClient smtphou1 = new SmtpClient();
                    smtphou1.Host = "smtprelay.global.wfp.org";
                    smtphou1.Port = 25;
                    smtphou1.UseDefaultCredentials = true;
                    //smtp.Credentials = new System.Net.NetworkCredential
                    //("ahmed.badr", "Survivor2323");// Enter seders User name and password  
                    //smtp.EnableSsl = true;
                    smtphou1.Send(mailhou1);


                    return RedirectToAction("Index");


                }



            }
            //////////////////////////////end if     onbehalfid .....   


            else
            {
                //ViewBag.mid = Request.QueryString ["Mid"];
                //ViewBag.ClearedBySupervisor = Request.QueryString["ClearedBySupervisor"];
                //ViewBag.FromDate = Request.QueryString["FromDate"];
                //ViewBag.ToDate = Request.QueryString["ToDate"];
                //if (ClearedBySupervisor == true)
                //{
                //    try
                //    {
                //        // missionAuthorization = db.MissionAuthorizations.Find(Mid);
                //        //db.Entry(missionAuthorization).State = EntityState.Modified ;
                //        //db.SaveChanges();

                //        var missionAuthorization = db.MissionAuthorizations.Single(u => u.MissionID == Mid);
                //        missionAuthorization.ClearedBySupervisor = ClearedBySupervisor;
                //        db.SaveChanges();



                //        var staffs = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                //        staffs = staffs.Where(s => s.staff_email.Contains(staffmail));
                //        var too = staffs.First().staff_email;


                //        var staffsupervisor = staffs.First().staff_supervisorid;

                //        var supers = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                //        supers = supers.Where(d => d.staffid == staffsupervisor);


                //        var from = supers.First().staff_email;

                //        var canbedone = "";
                //        if (missionAuthorization.canbedone == true)
                //        {
                //            canbedone = "Yes";
                //        }
                //        else
                //        {
                //            canbedone = "No";
                //        }


                //        MailMessage mail = new MailMessage();
                //        mail.To.Add(staffmail);
                //        mail.From = new MailAddress(to);
                //        mail.Subject = "Your mission Accepted from your supervisor";
                //        string Body = "<h4>Your mission Accepted from your supervisor<h4><br>Request : Mission &nbsp;  &nbsp;  &nbsp; &nbsp;  &nbsp; &nbsp; &nbsp; &nbsp;  Mission ID : " + missionAuthorization.MissionID + " <br> " + " From &nbsp; &nbsp; &nbsp;:" + staffs.First().staff_first_name + "&nbsp;" + staffs.First().staff_last_name + "<br><br>" + " Mission Information" + "<br><br>" + " From :" + missionAuthorization.FromDate + "<br>To :" + missionAuthorization.ToDate +
                //            "<br>Funding :" + missionAuthorization.funding + "<br>Objective :" + missionAuthorization.MissionObjective + "<br> Mission Itinerary :" + missionAuthorization.MissionItinerary ;

                //        mail.Body = Body;
                //        mail.IsBodyHtml = true;
                //        SmtpClient smtp = new SmtpClient();
                //        smtp.Host = "smtprelay.global.wfp.org";
                //        smtp.Port = 25;
                //        smtp.UseDefaultCredentials = true;
                //        //smtp.Credentials = new System.Net.NetworkCredential
                //        //("ahmed.badr", "Survivor2323");// Enter seders User name and password  
                //        //smtp.EnableSsl = true;
                //        smtp.Send(mail);

                //        var rd = db.staffs.Include(r => r.contract_details).Include(r => r.contract_type).Include(r => r.Country_office).Include(r => r.functional_title).Include(r => r.staff2).Include(r => r.sub_office).Include(r => r.unit).Include(r => r.country).Include(r => r.governorate);
                //        rd = rd.Where(r => r.functional_title.functional_title_description_english == "REGIONAL DIRECTOR");
                //        var rdmail = rd.First ().staff_email ;

                //        var drd = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                //        drd = drd.Where(s => s.functional_title.functional_title_description_english == "Deputy Regional Director");
                //        var drdmail = drd.First().staff_email ;

                //        MailMessage mailrd = new MailMessage();
                //        mailrd.To.Add(rdmail);
                //        mailrd.CC.Add(drdmail);
                //        mailrd.Bcc.Add("ahmed.badr@wfp.org");
                //        mailrd.From = new MailAddress(staffmail);
                //        mailrd.Subject = "Mission Authorization Request";
                //        string Bodyrd = "<h4>Staff Supervisor " + supervisorname + "  accept this mission</h4><br>Request : Mission &nbsp;  &nbsp;  &nbsp; &nbsp;  &nbsp; &nbsp; &nbsp; &nbsp;  Mission ID : " + missionAuthorization.MissionID + " <br> " + " From &nbsp; &nbsp; &nbsp;:" + staffs.First().staff_first_name + "&nbsp;" + staffs.First().staff_last_name + "<br><br>" + " Mission Information" + "<br><br>" + " From :" + missionAuthorization.FromDate + "<br>To :" + missionAuthorization.ToDate +
                //            "<br>Funding :" + missionAuthorization.funding + "<br>Objective :" + missionAuthorization.MissionObjective + "<br> Mission Itinerary :" + missionAuthorization.MissionItinerary + " <br><br> <a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization.MissionID + "&RDORDRDSignature=Yes&FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + " &to=" + to + "&staffmail=" + staffs.First().staff_email + "'>Acept</a>&nbsp;&nbsp;&nbsp;<a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization.MissionID + "&RDORDRDSignature=NO & FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + "&to=" + to + "&staffmail=" + staffs.First().staff_email + "'>Reject</a>  ";

                //        mailrd.Body = Bodyrd;
                //        mailrd.IsBodyHtml = true;
                //        SmtpClient smtprd = new SmtpClient();
                //        smtprd.Host = "smtprelay.global.wfp.org";
                //        smtprd.Port = 25;
                //        smtprd.UseDefaultCredentials = true;
                //        //smtp.Credentials = new System.Net.NetworkCredential
                //        //("ahmed.badr", "Survivor2323");// Enter seders User name and password  
                //        //smtp.EnableSsl = true;
                //        smtprd.Send(mailrd);

                //    }
                //    catch (DbEntityValidationException ex)
                //    {

                //        // Retrieve the error messages as a list of strings.

                //        var errorMessages = ex.EntityValidationErrors

                //                .SelectMany(x => x.ValidationErrors)

                //                .Select(x => x.ErrorMessage);



                //        // Join the list to a single string.

                //        var fullErrorMessage = string.Join("; ", errorMessages);



                //        // Combine the original exception message with the new one.

                //        var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);



                //        // Throw a new DbEntityValidationException with the improved exception message.

                //        throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);

                //    }
                //    return View();
                //}
                //else
                //{ 
                ////var missionAuthorizationn = db.MissionAuthorizations.Single(u => u.MissionID == Mid);
                ////db.MissionAuthorizations.Remove(missionAuthorizationn);
                ////db.SaveChanges();
                //var missionAuthorizationn = db.MissionAuthorizations.Single(u => u.MissionID == Mid);
                //missionAuthorizationn.ClearedBySupervisor = ClearedBySupervisor;
                //db.SaveChanges();


                //var staffss = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                //staffss = staffss.Where(s => s.staff_email.Contains(staffmail));
                //var too = staffss.First().staff_email;
                //var staffsupervisor = staffss.First().staff_supervisorid;
                //var supers = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                //supers = supers.Where(d => d.staffid == staffsupervisor);
                //var from = supers.First().staff_email;




                //MailMessage mail2 = new MailMessage();
                //mail2.To.Add(staffmail);
                //mail2.From = new MailAddress(to);
                //mail2.Subject = "Your mission Rejected from your Supervisor ";
                //string Body2 = "<h4>Your mission Rejected from your supervisor<h4><br>Request : Mission &nbsp;  &nbsp;  &nbsp; &nbsp;  &nbsp; &nbsp; &nbsp; &nbsp;  Mission ID : " + missionAuthorizationn.MissionID + " <br> " + " From &nbsp; &nbsp; &nbsp;:" + staffss.First().staff_first_name + "&nbsp;" + staffss.First().staff_last_name + "<br><br>" + " Mission Information" + "<br><br>" + " From :" + missionAuthorizationn.FromDate + "<br>To :" + missionAuthorizationn.ToDate +
                //    "<br>Funding :" + missionAuthorizationn.funding + "<br>Objective :" + missionAuthorizationn.MissionObjective + "<br> Mission Itinerary :" + missionAuthorizationn.MissionItinerary;

                //mail2.Body = Body2;
                //mail2.IsBodyHtml = true;
                //SmtpClient smtp2 = new SmtpClient();
                //smtp2.Host = "smtprelay.global.wfp.org";
                //smtp2.Port = 25;
                //smtp2.UseDefaultCredentials = true;
                ////smtp.Credentials = new System.Net.NetworkCredential
                ////("ahmed.badr", "Survivor2323");// Enter seders User name and password  
                ////smtp.EnableSsl = true;
                //smtp2.Send(mail2);


                var staffser = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                staffser = staffser.Where(s => s.staff_email.Contains(User.Identity.Name + "@wfp.org"));
                if (staffser.First().staff_supervisorid == null || staffser.First().staff_first_name == null || staffser.First().staff_last_name == null || staffser.First().unit_id == null)
                {

                    ModelState.AddModelError("staffid2", "Name is required");
                    ViewBag.staffid2 = "Name is required";

                }


                if (ModelState.IsValid)
                {

                    //db.MissionAuthorizations.Add(missionAuthorization);
                    try
                    {



                        //db.SaveChanges();
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

                    var missionAuthorization = db.MissionAuthorization1.Single(u => u.MissionID == Mid);
                    missionAuthorization.non4 = "Yes";
                    //missionAuthorization.RDORDRDDate = DateTime.Now;
                    db.SaveChanges();

                    //ViewBag.staffid = new SelectList(db.staffs, "staffs.First().staffid", staffs.First().staff_email);
                    ViewBag.staffid = staffs.First().staffid;
                    ViewBag.staffid2 = staffs.First().staff_first_name;
                    ViewBag.FirstName = staffs.First().staff_first_name;
                    ViewBag.LastName = staffs.First().staff_last_name;
                    ViewBag.Unit = staffs.First().unit.unit_description_english;
                    var staffsupervisor = staffs.First().staff_supervisorid;
                    var userUnit = staffs.First().unit_id;


                    var from = staffs.First().staff_email;
                    var supers = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                    supers = supers.Where(d => d.staffid == staffsupervisor);


                    var unithou = db.units.Include(d => d.staffs);
                    unithou = unithou.Where(u => u.unitid == userUnit);
                    var houid = unithou.First().HOU_ID;

                    var staffshou = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                    staffshou = staffshou.Where(h => h.staffid == houid);
                    var to = staffshou.First().staff_email;
                    //var houname = staffshou.First().staff_first_name + " " + staffshou.First().staff_last_name;


                    var cc = supers.First().staff_email;
                    var supername = supers.First().staff_first_name + " " + supers.First().staff_last_name;


                    if (staffs.First().staffid == houid)
                    {
                        var canbedone = "";
                        if (missionAuthorization.canbedone == true)
                        {
                            canbedone = "Yes";
                        }
                        else
                        {
                            canbedone = "No";
                        }

                        var CleredBySupervisor = "";
                        if (missionAuthorization.canbedone == true)
                        {
                            CleredBySupervisor = "Yes";
                        }
                        else
                        {
                            CleredBySupervisor = "No";
                        }

                        var BSITF2 = "";
                        if (missionAuthorization.BSITF == true)
                        {
                            BSITF2 = "Yes";
                        }
                        else
                        {
                            BSITF2 = "No";
                        }
                        ////////  
                        var ASITF2 = "";
                        if (missionAuthorization.ASITF == true)
                        {
                            ASITF2 = "Yes";
                        }
                        else
                        {
                            ASITF2 = "No";
                        }
                        ////////
                        var SSAFE2 = "";
                        if (missionAuthorization.SSAFE == true)
                        {
                            SSAFE2 = "Yes";
                        }
                        else
                        {
                            SSAFE2 = "No";
                        }
                        var Roming = "";
                        if (missionAuthorization.StaffSignature == "True")
                        {
                            Roming = "Yes";
                        }
                        else
                        {
                            Roming = "No";
                        }
                        //////////////////////////////


                        var supersforhou = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                        supersforhou = supersforhou.Where(d => d.staffid == staffsupervisor);
                        var superforhounitsname = supersforhou.First().staff_first_name + " " + staffshou.First().staff_last_name;
                        var houmailUPDATE = supersforhou.First().staff_email;

                        if (houmailUPDATE == "muhannad.hadi@wfp.org" || houmailUPDATE == "carlo.scaramella@wfp.org")
                        {
                            houmailUPDATE = "rbc.management@wfp.org"; //rbc.management@wfp.org

                        }
                        /////////////////////////
                        //var onbehalfof = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                        //onbehalfof = onbehalfof.Where(d => d.staffid == onbehalfID);
                        //var onbehalfofName = onbehalfof.First().staff_first_name + " " + onbehalfof.First().staff_last_name;
                        //var onbehalfofEmail = onbehalfof.First().staff_email;
                        ///////////////////////////////////////////////////
                        MailMessage mailhou = new MailMessage();
                        mailhou.To.Add(houmailUPDATE); // RD/DRD OFFICE/rbc.management@wfp.org
                        //mail.CC.Add(cc);
                        mailhou.Bcc.Add("ahmed.badr@wfp.org");
                        mailhou.From = new MailAddress(from);
                        mailhou.Subject = "Mission Authorization Form";
                        string Bodyhou = "<table><tr><td colspan='2'>" + "<hr/>New Mission<hr/></td></tr><tr><td><font color='blue'>Request : </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorization.MissionID + "</td></tr>"
                            + "<tr><td>" + "<font color='blue'>From :</font></td><td>" + staffs.First().staff_first_name + " " + staffs.First().staff_last_name + "</td></tr>"
                            + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                            + "<tr><td><font color='blue'>From :</font></td><td>" + missionAuthorization.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                            + "<tr><td><font color='blue'>To :</font></td><td>" + missionAuthorization.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                            + "<tr><td><font color='blue'>Funding :</font></td><td>" + missionAuthorization.funding + "</td></tr>"
                            + "<tr><td><font color='blue'>Objective :</font></td><td>" + missionAuthorization.MissionObjective + "</td></tr>"
                            + "<tr><td><font color='blue'>Mission Itinerary :</font></td><td>" + missionAuthorization.MissionItinerary + "</td></tr>"
                            + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video ? :</font></td><td>" + canbedone + "</td></tr>"
                            + "<tr><td><font color='blue'>Cleared By Supervisor :</font></td><td>" + CleredBySupervisor + "</td></tr>"
                             + "<tr><td><font color='blue'>International Roaming :</font></td><td>" + Roming + "</td></tr>"
                            + "<tr><td><font color='blue'>	BSITF :</font></td><td>" + BSITF2 + "</td></tr>"
                            + "<tr><td><font color='blue'> ASITF :</font></td><td>" + ASITF2 + "</td></tr>"
                            + "<tr><td><font color='blue'> SSAFE :</font></td><td>" + SSAFE2 + "</td></tr>"
                            + "<tr><td><font color='blue'> DSS Approval:</font></td><td><a target='_blank' href='http://10.11.135.254:8080/RBC-SMS/UploadedFiles/" + missionAuthorization.non6 + "'>" + missionAuthorization.non6 + "</a></td></tr>"


                            //<a download='custom-filename.jpg' href="/path/to/image" title="ImageName">     <img alt="ImageName" src="/path/to/image"> </a>


                            + "<tr><td><a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorization1/Approval2?MissionID2="
                            + missionAuthorization.MissionID
                            + "&RDORDRDSignature2=Yes&FromDate2=" + missionAuthorization.FromDate
                            + " &ToDate2=" + missionAuthorization.ToDate
                            + " &to2=" + to + "&staffmail2=" + staffs.First().staff_email
                            + "'>Accept</a></td><td><a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorization1/Approval2?MissionID2=" + missionAuthorization.MissionID
                            + "&RDORDRDSignature2=NO & FromDate2=" + missionAuthorization.FromDate
                            + " &ToDate2=" + missionAuthorization.ToDate
                            + "&to2=" + to
                            + "&staffmail2=" + staffs.First().staff_email
                            + "'>Reject</a>" + "</td><td>" + "</td></tr></table>";

                        //string Body = "Request : Mission &nbsp;  &nbsp;  &nbsp; &nbsp;  &nbsp; &nbsp; &nbsp; &nbsp;  Mission ID : " + missionAuthorization.MissionID + " <br> " + " From &nbsp; &nbsp; &nbsp;:" + staffs.First().staff_first_name + "&nbsp;" + staffs.First().staff_last_name + "<br><br>" + " Mission Information" + "<br><br>" + " From :" + missionAuthorization.FromDate + "<br>To :" + missionAuthorization.ToDate +
                        //    "<br>Funding :" + missionAuthorization.funding + "<br>Objective :" + missionAuthorization.MissionObjective + "<br> Mission Itinerary :" + missionAuthorization.MissionItinerary + "<br> Can This meeting Be Done Over Voice/Video ? :" + missionAuthorization.canbedone  + " <br><br> <a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor=" + true + "&FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + " &to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Acept</a>&nbsp;&nbsp;&nbsp;<a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor= " + false + "& FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + "&to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Reject</a> ";

                        mailhou.Body = Bodyhou;
                        mailhou.IsBodyHtml = true;
                        SmtpClient smtphou = new SmtpClient();
                        smtphou.Host = "smtprelay.global.wfp.org";
                        smtphou.Port = 25;
                        smtphou.UseDefaultCredentials = true;
                        //smtp.Credentials = new System.Net.NetworkCredential
                        //("ahmed.badr", "Survivor2323");// Enter seders User name and password  
                        //smtp.EnableSsl = true;
                        smtphou.Send(mailhou);


                        return RedirectToAction("Index");


                    }



                    if (to == "muhannad.hadi@wfp.org" || to == "carlo.scaramella@wfp.org")
                    {
                        to = "rbc.management@wfp.org";//rbc.management@wfp.org

                    }


                    var canbedone4 = "";
                    if (missionAuthorization.canbedone == true)
                    {
                        canbedone4 = "Yes";
                    }
                    else
                    {
                        canbedone4 = "No";
                    }

                    var CleredBySupervisor4 = "";
                    if (missionAuthorization.canbedone == true)
                    {
                        CleredBySupervisor4 = "Yes";
                    }
                    else
                    {
                        CleredBySupervisor4 = "No";
                    }

                    var BSITF24 = "";
                    if (missionAuthorization.BSITF == true)
                    {
                        BSITF24 = "Yes";
                    }
                    else
                    {
                        BSITF24 = "No";
                    }
                    ////////  
                    var ASITF24 = "";
                    if (missionAuthorization.ASITF == true)
                    {
                        ASITF24 = "Yes";
                    }
                    else
                    {
                        ASITF24 = "No";
                    }
                    ////////
                    var SSAFE24 = "";
                    if (missionAuthorization.SSAFE == true)
                    {
                        SSAFE24 = "Yes";
                    }
                    else
                    {
                        SSAFE24 = "No";
                    }
                    var Roming33 = "";
                    if (missionAuthorization.StaffSignature == "True")
                    {
                        Roming33 = "Yes";
                    }
                    else
                    {
                        Roming33= "No";
                    }
                    //////////////////////////////


                    var supersforhou33 = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                    supersforhou33 = supersforhou33.Where(d => d.staffid == staffsupervisor);
                    var superforhounitsname33 = supersforhou33.First().staff_first_name + " " + staffshou.First().staff_last_name;
                    var houmailUPDATE33 = supersforhou33.First().staff_email;

                    if (houmailUPDATE33 == "muhannad.hadi@wfp.org" || houmailUPDATE33 == "carlo.scaramella@wfp.org")
                    {
                        houmailUPDATE33 = "rbc.management@wfp.org"; //rbc.management@wfp.org

                    }
                    /////////////////////////
                    //var onbehalfof33 = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                    //onbehalfof33 = onbehalfof33.Where(d => d.staffid == onbehalfID);
                    //var onbehalfofName33 = onbehalfof33.First().staff_first_name + " " + onbehalfof33.First().staff_last_name;
                    //var onbehalfofEmail33 = onbehalfof33.First().staff_email;
                    ///////////////////////////////////////////////////
                    MailMessage mailhou33 = new MailMessage();
                    mailhou33.To.Add(to); // var to
                    //mail.CC.Add(cc);
                    mailhou33.Bcc.Add("ahmed.badr@wfp.org");
                    mailhou33.From = new MailAddress(from);
                    mailhou33.Subject = "Mission Authorization Form";

                    string Bodyhou33 = "<table><tr><td colspan='2'>" + "<hr/>New Mission<hr/></td></tr><tr><td><font color='blue'>Request : </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorization.MissionID + "</td></tr>"
                        + "<tr><td>" + "<font color='blue'>From :</font></td><td>" + staffs.First().staff_first_name + " " + staffs.First().staff_last_name + "</td></tr>"
                        + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                        + "<tr><td><font color='blue'>From :</font></td><td>" + missionAuthorization.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                        + "<tr><td><font color='blue'>To :</font></td><td>" + missionAuthorization.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                        + "<tr><td><font color='blue'>Funding :</font></td><td>" + missionAuthorization.funding + "</td></tr>"
                        + "<tr><td><font color='blue'>Objective :</font></td><td>" + missionAuthorization.MissionObjective + "</td></tr>"
                        + "<tr><td><font color='blue'>Mission Itinerary :</font></td><td>" + missionAuthorization.MissionItinerary + "</td></tr>"
                        + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video ? :</font></td><td>" + canbedone4 + "</td></tr>"
                        + "<tr><td><font color='blue'>Cleared By Supervisor :</font></td><td>" + CleredBySupervisor4 + "</td></tr>"
                         + "<tr><td><font color='blue'>International Roaming :</font></td><td>" + Roming33 + "</td></tr>"
                        + "<tr><td><font color='blue'>	BSITF :</font></td><td>" + BSITF24 + "</td></tr>"
                        + "<tr><td><font color='blue'> ASITF :</font></td><td>" + ASITF24 + "</td></tr>"
                        + "<tr><td><font color='blue'> SSAFE :</font></td><td>" + SSAFE24 + "</td></tr>"
                        + "<tr><td><font color='blue'> DSS Approval:</font></td><td><a target='_blank' href='http://10.11.135.254:8080/RBC-SMS/UploadedFiles/" + missionAuthorization.non6 + "'>" + missionAuthorization.non6 + "</a></td></tr>"


                        //<a download='custom-filename.jpg' href="/path/to/image" title="ImageName">     <img alt="ImageName" src="/path/to/image"> </a>


                        + "<tr><td><a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorization1/Approval2?MissionID2="
                        + missionAuthorization.MissionID
                        + "&RDORDRDSignature2=Yes&FromDate2=" + missionAuthorization.FromDate
                        + " &ToDate2=" + missionAuthorization.ToDate
                        + " &to2=" + to + "&staffmail2=" + staffs.First().staff_email
                        + "'>Accept</a></td><td><a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorization1/Approval2?MissionID2=" + missionAuthorization.MissionID
                        + "&RDORDRDSignature2=NO & FromDate2=" + missionAuthorization.FromDate
                        + " &ToDate2=" + missionAuthorization.ToDate
                        + "&to2=" + to
                        + "&staffmail2=" + staffs.First().staff_email
                        + "'>Reject</a>" + "</td><td>" + "</td></tr></table>";

                    //string Body = "Request : Mission &nbsp;  &nbsp;  &nbsp; &nbsp;  &nbsp; &nbsp; &nbsp; &nbsp;  Mission ID : " + missionAuthorization.MissionID + " <br> " + " From &nbsp; &nbsp; &nbsp;:" + staffs.First().staff_first_name + "&nbsp;" + staffs.First().staff_last_name + "<br><br>" + " Mission Information" + "<br><br>" + " From :" + missionAuthorization.FromDate + "<br>To :" + missionAuthorization.ToDate +
                    //    "<br>Funding :" + missionAuthorization.funding + "<br>Objective :" + missionAuthorization.MissionObjective + "<br> Mission Itinerary :" + missionAuthorization.MissionItinerary + "<br> Can This meeting Be Done Over Voice/Video ? :" + missionAuthorization.canbedone  + " <br><br> <a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor=" + true + "&FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + " &to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Acept</a>&nbsp;&nbsp;&nbsp;<a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor= " + false + "& FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + "&to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Reject</a> ";

                    mailhou33.Body = Bodyhou33;
                    mailhou33.IsBodyHtml = true;
                    SmtpClient smtphou33 = new SmtpClient();
                    smtphou33.Host = "smtprelay.global.wfp.org";
                    smtphou33.Port = 25;
                    smtphou33.UseDefaultCredentials = true;
                    //smtp.Credentials = new System.Net.NetworkCredential
                    //("ahmed.badr", "Survivor2323");// Enter seders User name and password  
                    //smtp.EnableSsl = true;
                    smtphou33.Send(mailhou33);



                    //BackgroundJob.Schedule(() => smtp.Send(mail), TimeSpan.FromMinutes(10));
                    //RecurringJob.AddOrUpdate(() => GetADGroupUsers("ss"), Cron.Minutely);
                    //RecurringJob.AddOrUpdate(() => Console.WriteLine(), Cron.Minutely);
                    //RecurringJob.AddOrUpdate(mail,Cron.Minutely);
                    //RecurringJob.AddOrUpdate(() => Storage.PunchIt(), Cron.Minutely);

                    //RecurringJob.AddOrUpdate(() => Storage.PunchIt(), Cron.Minutely);

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
            }

            //ViewBag.staffid = User.Identity.GetUserName() + "@wfp.org";
            return RedirectToAction("Create");
        }
        /// <summary>
        /// ////////////
        /// </summary>
        /// <param name="disposing"></param>
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
