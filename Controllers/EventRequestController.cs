using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WFPtest.Models;
using Microsoft.AspNet.Identity;
using PagedList;
using System.Diagnostics;
using System.Net.Mail;

namespace WFPtest.Controllers
{
    public class EventRequestController : Controller
    {
        private WFPEntities1 db = new WFPEntities1();

        // GET: EventRequest
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {


            var result = from p in db.requests1
                         join a in db.requests1
                         on p.Purpose equals a.Purpose


                         select new
                         {
                             staffSuperName = p.Purpose + " " + p.Purpose,


                         };

            ViewBag.newstaffSuperName = result;


            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = String.IsNullOrEmpty(sortOrder) ? "First_Name" : "";

            var students = from s in db.requests1
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

            var staffser = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
            staffser = staffser.Where(s => s.staff_email.Contains(User.Identity.Name + "@wfp.org"));
            var staffid = staffser.First().staffid;

            if (staffser.First().staff_email == "ahmed.moussa@wfp.org")
            {
                var requests1 = db.requests1.Include(r => r.Attends);
                

                if (requests1.Count() != 0)
                {

                    var requestingunitID = requests1.First().RequestingUnit;
                    var focalpointID = requests1.First().FocalPoint;
                    var requestingbyID = requests1.First().RequstingBy;

                    var requestsunit = db.units.Include(r => r.Country_office);
                    requestsunit = requestsunit.Where(r => r.unitid == (requestingunitID));
                    var unitname = requestsunit.First().unit_description_english;

                    var focalpointName = db.staffs.Include(s => s.Attends);
                    focalpointName = focalpointName.Where(s => s.staffid == focalpointID);
                    var focalpointFullName = focalpointName.First().staff_first_name + " " + focalpointName.First().staff_last_name;

                    var requestingbyName = db.staffs.Include(s => s.Attends);
                    requestingbyName = requestingbyName.Where(s => s.staffid == requestingbyID);
                    var requestingbyFullName = requestingbyName.First().staff_first_name + " " + requestingbyName.First().staff_last_name; ;

                    ViewBag.unitname = unitname;
                    ViewBag.focalpointFullName = focalpointFullName;
                    ViewBag.requestingbyFullName = requestingbyFullName;
                }

                if (!String.IsNullOrEmpty(searchString))
                {
                    requests1 = requests1.Where(s => s.Purpose.Equals(searchString)
                                           || s.Purpose.Equals(searchString));

                }

                switch (sortOrder)
                {
                    case "First_Name":
                        requests1 = requests1.OrderByDescending(s => s.Purpose);
                        break;
                    case "name_desc":
                        requests1= requests1.OrderByDescending(s => s.Purpose);
                        break;

                    default:  // Name ascending 
                        requests1 = requests1.OrderBy(s => s.Purpose);
                        break;
                }




                int pageSize = 10;
                int pageNumber = (page ?? 1);


                return View(requests1.ToPagedList(pageNumber, pageSize));

            }

            var requests2= db.requests1.Include(r => r.Attends);

            if (requests2.Count () != 0)
            {

                var requestingunitID = requests2.First().RequestingUnit;
                var focalpointID = requests2.First().FocalPoint;
                var requestingbyID = requests2.First().RequstingBy;

                var requestsunit = db.units.Include(r => r.Country_office);
                requestsunit = requestsunit.Where(r => r.unitid == (requestingunitID));
                var unitname = requestsunit.First().unit_description_english;

                var focalpointName = db.staffs.Include(s => s.Attends);
                focalpointName = focalpointName.Where(s => s.staffid == focalpointID);
                var focalpointFullName = focalpointName.First().staff_first_name + " " + focalpointName.First().staff_last_name;

                var requestingbyName = db.staffs.Include(s => s.Attends);
                requestingbyName = requestingbyName.Where(s => s.staffid == requestingbyID);
                var requestingbyFullName = requestingbyName.First().staff_first_name + " " + requestingbyName.First().staff_last_name; ;

                ViewBag.unitname = unitname;
                ViewBag.focalpointFullName = focalpointFullName;
                ViewBag.requestingbyFullName = requestingbyFullName;
            }

                if (!String.IsNullOrEmpty(searchString))
                {
                    requests2= requests2.Where(s => s.Purpose.Equals(searchString)
                                           || s.Purpose.Equals(searchString));

                }

                switch (sortOrder)
                {
                    case "First_Name":
                        requests2 = requests2.OrderByDescending(s => s.Purpose);
                        break;
                    case "name_desc":
                        requests2 = requests2.OrderByDescending(s => s.Purpose);
                        break;

                    default:  // Name ascending 
                        requests2 = requests2.OrderBy(s => s.Purpose);
                        break;
                }




                int pageSize1 = 10;
                int pageNumber1 = (page ?? 1);


                return View(requests2.ToPagedList(pageNumber1, pageSize1));
            }
       
        // GET: EventRequest/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            requests1 requests1 = db.requests1.Find(id);
            if (requests1 == null)
            {
                return HttpNotFound();
            }
            ViewBag.Focalpoint = new SelectList(db.staffs.OrderBy(staffs => staffs.staff_email), "staffid", "staff_email");
            if (!Request.IsAuthenticated)
            {
                Response.Redirect("/RBC-SMS/account/login");
            }
            //ViewBag.staffid = new SelectList(User.Identity.GetUserName() + "wfp.org");
            //var username = User.Identity.GetUserName();


            var staffs3 = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
            staffs3 = staffs3.Where(s => s.staff_email == User.Identity.Name + "@wfp.org");
            var stafemal11 = staffs3.First().staff_email;
            var staffforhouforsuper = staffs3.First().staffid;




            if (staffs3.First().staff_supervisorid == null || staffs3.First().staff_first_name == null || staffs3.First().unit_id == null || staffs3.First().unit_id == null)
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

            var superid = staffs3.First().staffid;


            if (staffs3.First().staff_supervisorid == null)
            {
                staffs3.First().staff_supervisorid = 1212;
                ViewBag.error = 1;
            }

            if (staffs3.First().staff_first_name == null)
            {
                staffs3.First().staff_first_name = "You must have First name !!!";
                ViewBag.error = 1;
            }

            if (staffs3.First().staff_last_name == null)
            {
                staffs3.First().staff_last_name = "You must have Last name !!!";
                ViewBag.error = 1;
            }


            //ViewBag.staffid = new SelectList(db.staffs, "staffs.First().staffid", staffs.First().staff_email);
            ViewBag.staffid = staffs3.First().staffid;
            ViewBag.staffid2 = staffs3.First().staff_first_name;
            ViewBag.FirstName = staffs3.First().staff_first_name;
            ViewBag.LastName = staffs3.First().staff_last_name;
            ViewBag.unitid = staffs3.First().unit_id;
            ViewBag.currentDate = DateTime.Now;
            if (staffs3.First().unit_id == null)
            {
                ViewBag.Unit = "You must have a Unit !!!!";
                staffs3.First().unit_id = 15;
                ViewBag.error = 1;

            }
            else
            {

                ViewBag.Unit = staffs3.First().unit.unit_description_english;
            }
            var staffsupervisor = staffs3.First().staff_supervisorid;

            var userUnit = staffs3.First().unit_id;

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





            return View(requests1);
        }

        // GET: EventRequest/Create
        public ActionResult Create()
        {

            ViewBag.Focalpoint = new SelectList(db.staffs.OrderBy(staffs => staffs.staff_email), "staffid", "staff_email");
            if (!Request.IsAuthenticated)
            {
                Response.Redirect("/RBC-SMS/account/login");
            }
            //ViewBag.staffid = new SelectList(User.Identity.GetUserName() + "wfp.org");
            //var username = User.Identity.GetUserName();
           

            var staffs3 = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
            staffs3 = staffs3.Where(s => s.staff_email == User.Identity.Name + "@wfp.org");
            var stafemal11 = staffs3.First().staff_email;
            var staffforhouforsuper = staffs3.First().staffid;


           

            if (staffs3.First().staff_supervisorid == null || staffs3.First().staff_first_name == null || staffs3.First().unit_id == null || staffs3.First().unit_id == null)
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

            var superid = staffs3.First().staffid;


            if (staffs3.First().staff_supervisorid == null)
            {
                staffs3.First().staff_supervisorid = 1212;
                ViewBag.error = 1;
            }

            if (staffs3.First().staff_first_name == null)
            {
                staffs3.First().staff_first_name = "You must have First name !!!";
                ViewBag.error = 1;
            }

            if (staffs3.First().staff_last_name == null)
            {
                staffs3.First().staff_last_name = "You must have Last name !!!";
                ViewBag.error = 1;
            }


            //ViewBag.staffid = new SelectList(db.staffs, "staffs.First().staffid", staffs.First().staff_email);
            ViewBag.staffid = staffs3.First().staffid;
            ViewBag.staffid2 = staffs3.First().staff_first_name;
            ViewBag.FirstName = staffs3.First().staff_first_name;
            ViewBag.LastName = staffs3.First().staff_last_name;
            ViewBag.unitid = staffs3.First().unit_id;
            ViewBag.currentDate = DateTime.Now;
            ViewBag.non2 = new SelectList(db.staffs.OrderBy(staffs => staffs.staff_email), "staffid", "staff_email");
            if (staffs3.First().unit_id == null)
            {
                ViewBag.Unit = "You must have a Unit !!!!";
                staffs3.First().unit_id = 15;
                ViewBag.error = 1;

            }
            else
            {

                ViewBag.Unit = staffs3.First().unit.unit_description_english;
            }
            var staffsupervisor = staffs3.First().staff_supervisorid;

            var userUnit = staffs3.First().unit_id;

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

        // POST: EventRequest/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,RequstingBy,RequestingDate,RequestingUnit,FocalPoint,Purpose,FromDate,ToDate,NumOFP_CO,NumOFP_RB,NumOFP_HQ,NumOFP_Other,Country,GOV,District,OtherPlace,Budget_TR,Number_Roms,Check_inDate,Check_OutDate,Rom_Type,Microphone,HiSpeedInternet,Conf_Call_Device,MMR_Days,BOR_Days,COffeBreak,COffeBreak_UNM,Lunch,Dinner,Coctail_Reception,Dedicated_IT_SUPP,DescripDaysORHours,Transportstion_ForLocal_Staff,Airport_Picup_Services,Other_Services,non1,non2,non3,Stationary,NameTags,Writingpads,ServicesReqOthers")] requests1 requests1)
        {
            if (ModelState.IsValid)
            {
                db.requests1.Add(requests1);
                db.SaveChanges();
                //////////// email////////////


                var staffs3 = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                staffs3 = staffs3.Where(s => s.staff_email == User.Identity.Name + "@wfp.org");
                var stafemal11 = staffs3.First().staff_email;
                var staffforhouforsuper = staffs3.First().staffid;


                ViewBag.non2 = new SelectList(db.staffs.OrderBy(staffs => staffs.staff_email), "staffid", "staff_email");

                if (staffs3.First().staff_supervisorid == null || staffs3.First().staff_first_name == null || staffs3.First().unit_id == null || staffs3.First().unit_id == null)
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

                var superid = staffs3.First().staffid;


                if (staffs3.First().staff_supervisorid == null)
                {
                    staffs3.First().staff_supervisorid = 1212;
                    ViewBag.error = 1;
                }

                if (staffs3.First().staff_first_name == null)
                {
                    staffs3.First().staff_first_name = "You must have First name !!!";
                    ViewBag.error = 1;
                }

                if (staffs3.First().staff_last_name == null)
                {
                    staffs3.First().staff_last_name = "You must have Last name !!!";
                    ViewBag.error = 1;
                }


                //ViewBag.staffid = new SelectList(db.staffs, "staffs.First().staffid", staffs.First().staff_email);
                ViewBag.staffid = staffs3.First().staffid;
                ViewBag.staffid2 = staffs3.First().staff_first_name;
                ViewBag.FirstName = staffs3.First().staff_first_name;
                ViewBag.LastName = staffs3.First().staff_last_name;
                ViewBag.unitid = staffs3.First().unit_id;
                ViewBag.currentDate = DateTime.Now;
                ViewBag.non2 = new SelectList(db.staffs.OrderBy(staffs => staffs.staff_email), "staffid", "staff_email");
                if (staffs3.First().unit_id == null)
                {
                    ViewBag.Unit = "You must have a Unit !!!!";
                    staffs3.First().unit_id = 15;
                    ViewBag.error = 1;

                }
                else
                {

                    ViewBag.Unit = staffs3.First().unit.unit_description_english;
                }
                var staffsupervisor = staffs3.First().staff_supervisorid;

                var userUnit = staffs3.First().unit_id;

                var supers = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                supers = supers.Where(d => d.staffid == staffsupervisor);





                ViewBag.supervisor = (supers.First().staff_first_name + " " + supers.First().staff_last_name);



                var staffsforapprovall = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                staffsforapprovall = staffsforapprovall.Where(s => s.staffid == requests1.non2);
                var forapprovall = staffsforapprovall.First().staff_first_name + " " + staffsforapprovall.First().staff_last_name;
                var forapprovallEmail = staffsforapprovall.First().staff_email;


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

                try
                {

                    var Microphone = "";
                    var HiSpeedInternet = "";
                    var Conf_Call_Device = "";
                    var COffeBreak = "";
                    var Dedicated_IT_SUPP = "";
                    var Lunch = "";
                    var Dinner = "";
                    var Coctail_Reception = "";
                    var Transportstion_ForLocal_Staff = "";
                    var Airport_Picup_Services = "";
                    //var Stationary = "";
                    var NameTags = "";
                    var Writingpads = "";

                    if (requests1.Microphone == true)
                    {
                        Microphone = "Yes";
                    }
                    else
                    {
                        Microphone = "No";
                    }

                    if (requests1.HiSpeedInternet == true)
                    {
                        HiSpeedInternet = "Yes";
                    }
                    else
                    {
                        HiSpeedInternet = "No";
                    }

                    if (requests1.Conf_Call_Device == true)
                    {
                        Conf_Call_Device = "Yes";
                    }
                    else
                    {
                        Conf_Call_Device = "No";
                    }

                    if (requests1.COffeBreak == true)
                    {
                        COffeBreak = "Yes";
                    }
                    else
                    {
                        COffeBreak = "No";
                    }

                    if (requests1.Dedicated_IT_SUPP == true)
                    {
                        Dedicated_IT_SUPP = "Yes";
                    }
                    else
                    {
                        Dedicated_IT_SUPP = "No";
                    }

                    if (requests1.Lunch == true)
                    {
                        Lunch = "Yes";
                    }
                    else
                    {
                        Lunch = "No";
                    }

                    if (requests1.Dinner == true)
                    {
                        Dinner = "Yes";
                    }
                    else
                    {
                        Dinner = "No";
                    }

                    if (requests1.Coctail_Reception == true)
                    {
                        Coctail_Reception = "Yes";
                    }
                    else
                    {
                        Coctail_Reception = "No";
                    }
                    if (requests1.Transportstion_ForLocal_Staff == true)
                    {
                        Transportstion_ForLocal_Staff = "Yes";
                    }
                    else
                    {
                        Transportstion_ForLocal_Staff = "No";
                    }
                    if (requests1.Airport_Picup_Services == true)
                    {
                        Airport_Picup_Services = "Yes";
                    }
                    else
                    {
                        Airport_Picup_Services = "No";
                    }
                   
                    if (requests1.NameTags == true)
                    {
                        NameTags = "Yes";
                    }
                    else
                    {
                        NameTags = "No";
                    }
                    if (requests1.Writingpads == true)
                    {
                        Writingpads = "Yes";
                    }
                    else
                    {
                        Writingpads = "No";
                    }
                    var staffs = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                    staffs = staffs.Where(s => s.staffid == requests1.RequstingBy);
                    var requestedby = staffs.First().staff_first_name + " " + staffs.First().staff_last_name;
                    var requestedbyemail = staffs.First().staff_email;

                    //var title = db.functional_title.Include(s => s.staffs);
                    //title = title.Where(t => t.functionaltitleid == requests1.job_title);
                    //var jobtitle = title.First().functional_title_description_english;

                    var unitselect = db.units.Include(s => s.staffs);
                    unitselect = unitselect.Where(u => u.unitid == requests1.RequestingUnit);
                    var unit = unitselect.First().unit_description_english;

                    //var Superemail = db.staffs.Include(s => s.district);
                    //Superemail = Superemail.Where(Su => Su.staffid == requests1.supervisor_email);
                    //var supervisor_email = Superemail.First().staff_email;



                    MailMessage mail = new MailMessage();
                    mail.To.Add(forapprovallEmail);
                    //mail.CC.Add("cairo.itservicedesk@wfp.org");

                    mail.CC.Add("hanaa.ibrahim@wfp.org");
                    //mail.CC.Add("mahmoud.abdelfattah@wfp.org");
                    //mail.CC.Add("amal.mohamed@wfp.org");
                    mail.Bcc.Add("ahmed.badr@wfp.org");
                    mail.From = new MailAddress(requestedbyemail);
                    mail.Subject = " Event Request : " + requests1.Purpose;

                    string Body = "<table border='0'><tr><td colspan='2'><hr/>Request Information<hr/></td><hr/></tr><tr><td><font color='blue'>Request Number: </font></td><td>" + requests1.ID + "</td></tr>"
                     + "<tr><td><font color='blue'>" + " Requested By :</font></td><td>" + requestedby + "</td></tr>"
                     + "<tr><td><font color='blue'>Requesting UNIT :</font></td><td>" + unit + "</td></tr>"
                     + "<tr><td><font color='blue'>FOCAL POINT:</font></td><td>" + staffs3.First().staff_first_name + " " + staffs3.First().staff_last_name + "</td></tr>"
                     + "<tr><td><font color='blue'>Requesting Date :</font></td><td>" + requests1.RequestingDate + "</td></tr>"
                     + "<tr><td><font color='blue'>Purpose :</font></td><td>" + requests1.Purpose + "</td></tr>"
                     + "<tr><td><font color='blue'>From DATE  :</font></td><td>" + requests1.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                     + "<tr><td><font color='blue'>TO DATE :</font></td><td>" + requests1.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                     + "<tr><td colspan='2'><hr/>Participates Information <hr/></td></tr>"
                     + "<tr><td><font color='blue'>Number OF Persons EGYCO:</font></td><td>" + requests1.NumOFP_CO + "</td></tr>"
                     + "<tr><td><font color='blue'>Number OF Persons RB :</font></td><td>" + requests1.NumOFP_RB + "</td></tr>"
                     + "<tr><td><font color='blue'>Number OF Persons HQ :</font></td><td>" + requests1.NumOFP_HQ + "</td></tr>"
                     + "<tr><td><font color='blue'>Others :</font></td><td>" + requests1.NumOFP_Other + "</td></tr>"
                     + "<tr><td colspan='2'><hr/>Location Information <hr/></td></tr>"
                     + "<tr><td><font color='blue'>Country :</font></td><td>" + requests1.Country + "</td></tr>"
                     + "<tr><td><font color='blue'>District :</font></td><td>" + requests1.District + "</td></tr>"
                     + "<tr><td><font color='blue'>Other Place :</font></td><td>" + requests1.OtherPlace + "</td></tr>"
                     + "<tr><td colspan='2'><hr/>Hotel Rooms  Information <hr/></td></tr>"
                     + "<tr><td><font color='blue'>Number of Rooms :</font></td><td>" + requests1.Number_Roms + "</td></tr>"
                     + "<tr><td><font color='blue'>Check in Date :</font></td><td>" + requests1.Check_inDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                     + "<tr><td><font color='blue'>Check Out Date :</font></td><td>" + requests1.Check_OutDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                     + "<tr><td><font color='blue'>Main Meeting Room:</font></td><td>" + requests1.MMR_Days + "</td></tr>"
                     + "<tr><td><font color='blue'>SITTING LOCATION :</font></td><td>" + requests1.BOR_Days + "</td></tr>"
                     + "<tr><td colspan='2'><hr/>SERVICES REQUIRED <hr/></td></tr>"
                     + "<tr><td><font color='blue'>Microphone:</font></td><td>" + Microphone + "</td></tr>"
                     + "<tr><td><font color='blue'>high Speed Internet:</font></td><td>" + HiSpeedInternet + "</td></tr>"
                     + "<tr><td><font color='blue'>Confrance Call Device :</font></td><td>" + Conf_Call_Device + "</td></tr>"
                     + "<tr><td><font color='blue'>Dedicated IT Support :</font></td><td>" + Dedicated_IT_SUPP + "</td></tr>"
                     + "<tr><td><font color='blue'>Descrip Days/Hours:</font></td><td>" + requests1.DescripDaysORHours  + "</td></tr>"
                     //+ "<tr><td><font color='blue'>Confrance Call Device :</font></td><td>" + Conf_Call_Device + "</td></tr>"
                     //+ "<tr><td><font color='blue'>Dedicated IT Support :</font></td><td>" + Dedicated_IT_SUPP + "</td></tr>"
                     //+ "<tr><td><font color='blue'>Descrip Days/Hours:</font></td><td>" + requests1.DescripDaysORHours + "</td></tr>"
                      + "<tr><td><font color='blue'>STATIONARY:</font></td><td>" + requests1.Stationary + "</td></tr>"
                     + "<tr><td><font color='blue'>Name Tags:</font></td><td>" + NameTags + "</td></tr>"
                     + "<tr><td><font color='blue'>Writing Pads:</font></td><td>" + Writingpads + "</td></tr>"
                     + "<tr><td><font color='blue'>Others Services:</font></td><td>" + requests1.ServicesReqOthers + "</td></tr>"
                     + "<tr><td colspan='2'><hr/>Catering Information <hr/></td></tr>"
                     + "<tr><td><font color='blue'>COffee Break:</font></td><td>" + COffeBreak + "</td></tr>"
                     + "<tr><td><font color='blue'>COffee Break Number:</font></td><td>" + requests1.COffeBreak_UNM + "</td></tr>"
                     + "<tr><td><font color='blue'>COCKTAIL RECEPTION :</font></td><td>" + Coctail_Reception + "</td></tr>"
                     + "<tr><td><font color='blue'>Lunch:</font></td><td>" + Lunch + "</td></tr>"
                     + "<tr><td><font color='blue'>Dinner :</font></td><td>" + Dinner + "</td></tr>"
                     + "<tr><td colspan='2'><hr/>Transportstion <hr/></td></tr>"
                      + "<tr><td><font color='blue'>TRANSPORTATION FOR LOCAL STAFF:</font></td><td>" + Transportstion_ForLocal_Staff + "</td></tr>"
                     + "<tr><td><font color='blue'>AIRPORT PICK UP SERVICES :</font></td><td>" + Airport_Picup_Services + "</td></tr>"
                      + "<tr><td><a href='http://10.11.135.254:8080/RBC-SMS/EventRequest/Approval2?Mid="
                        + requests1.ID
                        + "&RDORDRDSignature=Yes&FromDate=" + requests1.FromDate
                        + " &ToDate=" + requests1.ToDate
                        + " &to=" + to + "&staffmail=" + forapprovall + "&forapprovallEmail=" + forapprovallEmail
                        + "&requestedbyemail=" + requestedbyemail
                      + "'>Accept</a></td></tr><tr><td><a href='http://10.11.135.254:8080/RBC-SMS/EventRequest/Approval2?Mid=" + requests1.ID
                        + "&RDORDRDSignature=NO & FromDate=" + requests1.FromDate
                        + " &ToDate=" + requests1.ToDate
                        + "&to=" + to
                        + "&staffmail=" + forapprovall
                        + "&forapprovallEmail=" + forapprovallEmail
                        + "&requestedbyemail=" + requestedbyemail
                        + "'>Reject</a></td></tr></table>"
                     ;

                    //string Body = "Request : Mission &nbsp;  &nbsp;  &nbsp; &nbsp;  &nbsp; &nbsp; &nbsp; &nbsp;  Mission ID : " + missionAuthorization.MissionID + " <br> " + " From &nbsp; &nbsp; &nbsp;:" + staffs.First().staff_first_name + "&nbsp;" + staffs.First().staff_last_name + "<br><br>" + " Mission Information" + "<br><br>" + " From :" + missionAuthorization.FromDate + "<br>To :" + missionAuthorization.ToDate +
                    //    "<br>Funding :" + missionAuthorization.funding + "<br>Objective :" + missionAuthorization.MissionObjective + "<br> Mission Itinerary :" + missionAuthorization.MissionItinerary + "<br> Can This meeting Be Done Over Voice/Video ? :" + missionAuthorization.canbedone  + " <br><br> <a href='http://10.11.135.254:8080/WFPSMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor=" + true + "&FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + " &to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Acept</a>&nbsp;&nbsp;&nbsp;<a href='http://10.11.135.254:8080/WFPSMS/MissionAuthorizations/Approval?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor= " + false + "& FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + "&to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Reject</a> ";

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

                    return RedirectToAction("Index");
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
                ///////////email /////////////
                return RedirectToAction("Index");
            }

            ViewBag.Focalpoint = new SelectList(db.staffs.OrderBy(staffs => staffs.staff_email), "staffid", "staff_email");
            if (!Request.IsAuthenticated)
            {
                Response.Redirect("/RBC-SMS/account/login");
            }
            //ViewBag.staffid = new SelectList(User.Identity.GetUserName() + "wfp.org");
            //var username = User.Identity.GetUserName();

            var staffs33 = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
            staffs33 = staffs33.Where(s => s.staff_email == User.Identity.Name + "@wfp.org");
            var stafemal113 = staffs33.First().staff_email;
            var staffforhouforsuper3 = staffs33.First().staffid;

            ViewBag.staffid = staffs33.First().staffid;
            ViewBag.staffid2 = staffs33.First().staff_first_name;
            ViewBag.FirstName = staffs33.First().staff_first_name;
            ViewBag.LastName = staffs33.First().staff_last_name;
            ViewBag.unitid = staffs33.First().unit_id;
            ViewBag.currentDate = DateTime.Now;

            if (staffs33.First().unit_id == null)
            {
                ViewBag.Unit = "You must have a Unit !!!!";
                staffs33.First().unit_id = 15;
                ViewBag.error = 1;

            }
            else
            {

                ViewBag.Unit = staffs33.First().unit.unit_description_english;
            }
            var staffsupervisor3 = staffs33.First().staff_supervisorid;

            var userUnit3 = staffs33.First().unit_id;

            var supers3 = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
            supers3 = supers3.Where(d => d.staffid == staffsupervisor3);





            ViewBag.supervisor = (supers3.First().staff_first_name + " " + supers3.First().staff_last_name);


            var supervisoremailif3 = supers3.First().staff_email;
            if (supervisoremailif3 == "muhannad.hadi@wfp.org" || supervisoremailif3 == "carlo.scaramella@wfp.org")
            {
                supervisoremailif3 = "rbc.management@wfp.org"; // rbc.management@wfp.org

            }

            ViewBag.supervisorEmail = supervisoremailif3;
            ViewBag.SignatureDate = DateTime.Now;



            var unithou3 = db.units.Include(d => d.staffs);
            unithou3 = unithou3.Where(u => u.unitid == userUnit3);
            var houid3 = unithou3.First().HOU_ID;

            var staffshou3 = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
            staffshou3 = staffshou3.Where(h => h.staffid == houid3);

            //////////////////////////////
            var superforhounitsname3 = "";
            var houmailUPDATE3 = "";
            var houidforSuper3 = staffshou3.First().staffid;
            if (houidforSuper3 == staffforhouforsuper3)
            {
                var supersforhou = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                supersforhou = supersforhou.Where(d => d.staffid == staffsupervisor3);
                superforhounitsname3 = supersforhou.First().staff_first_name + " " + staffshou3.First().staff_last_name;
                houmailUPDATE3 = supersforhou.First().staff_email;

            }
            ///////////////////////////////

            var to3 = staffshou3.First().staff_email;

            superforhounitsname3 = staffshou3.First().staff_first_name + " " + staffshou3.First().staff_last_name;
            ViewBag.houname = superforhounitsname3;

            houmailUPDATE3 = staffshou3.First().staff_email;

            if (houmailUPDATE3 == "muhannad.hadi@wfp.org" || houmailUPDATE3 == "carlo.scaramella@wfp.org")
            {
                houmailUPDATE3 = "rbc.management@wfp.org"; //rbc.management@wfp.org

            }


            ViewBag.houmail = houmailUPDATE3;
            return View();
        }

        //    var staffs3 = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
        //    staffs3 = staffs3.Where(s => s.staff_email == User.Identity.Name + "@wfp.org");
        //    var stafemal11 = staffs3.First().staff_email;
        //    var staffforhouforsuper = staffs3.First().staffid;




        //    if (staffs3.First().staff_supervisorid == null || staffs3.First().staff_first_name == null || staffs3.First().unit_id == null || staffs3.First().unit_id == null)
        //    {

        //        ///////////////////////

        //        var selectmanagerindex = db.EMPLOYEES.Include(emp => emp.staff);
        //        selectmanagerindex = selectmanagerindex.Where(emp => emp.EMAIL_ADDRESS == stafemal11);
        //        //var empEmail = selectmanagerindex.First().EMAIL_ADDRESS;
        //        var managerindex = selectmanagerindex.FirstOrDefault().MANAGER;
        //        var Depid = selectmanagerindex.First().DEPID;
        //        var firstname = selectmanagerindex.First().EMP_FIRST_NAME;
        //        var lastname = selectmanagerindex.First().EMP_LAST_NAME;
        //        var IndexNumber = selectmanagerindex.First().EMP_ID;


        //        var updatefirslastname = db.staffs.Single(u => u.staff_email == stafemal11);
        //        updatefirslastname.staff_first_name = firstname;
        //        updatefirslastname.staff_id = IndexNumber;
        //        updatefirslastname.staff_index_number = IndexNumber;
        //        if (lastname == null)
        //        {
        //            updatefirslastname.staff_last_name = lastname;
        //        }

        //        db.SaveChanges();

        //        var SuperForEmpM = db.EMPLOYEES.Include(s => s.staff);
        //        SuperForEmpM = SuperForEmpM.Where(s => s.EMP_ID == managerindex);
        //        var mangerEmail = SuperForEmpM.First().EMAIL_ADDRESS;



        //        var staffscompmid = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
        //        staffscompmid = staffscompmid.Where(s => s.staff_email == mangerEmail);
        //        var managerid = staffscompmid.First().staffid;


        //        var staffmanagerupdate1 = db.staffs.Single(u => u.staff_email == stafemal11);
        //        staffmanagerupdate1.staff_supervisorid = managerid;
        //        db.SaveChanges();

        //        var UNITForEmpM = db.DEPARTMENTS.Include(s => s.EMPLOYEES);
        //        UNITForEmpM = UNITForEmpM.Where(s => s.DEP_ID == Depid);
        //        var unitnameforemp = UNITForEmpM.First().DEP_NAME;
        //        var dephouindex = UNITForEmpM.First().DEP_MANAGER;

        //        var emailstaffforhou = db.EMPLOYEES.Include(s => s.staff);
        //        emailstaffforhou = emailstaffforhou.Where(s => s.EMP_ID == dephouindex);
        //        var mailhouemp = emailstaffforhou.First().EMAIL_ADDRESS;

        //        var staffhouid = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
        //        staffhouid = staffhouid.Where(s => s.staff_email == mailhouemp);
        //        var houidEmp = staffscompmid.First().staffid;

        //        var staffscompmidfornameunit = db.units.Include(s => s.Country_office);
        //        staffscompmidfornameunit = staffscompmidfornameunit.Where(s => s.unit_description_english == unitnameforemp);

        //        if (staffscompmidfornameunit.Any())
        //        {
        //            int unitid = staffscompmidfornameunit.First().unitid;
        //            var staffmanagerupdate = db.staffs.Single(u => u.staff_email == stafemal11);
        //            staffmanagerupdate.unit_id = unitid;
        //            db.SaveChanges();

        //        }
        //        else
        //        {

        //            var addunit = db.units.Add(new unit { unit_description_english = unitnameforemp }).unit_description_english;
        //            db.SaveChanges();



        //            var selectnewunit = db.units.Include(s => s.Country_office);
        //            selectnewunit = selectnewunit.Where(s => s.unit_description_english == unitnameforemp);
        //            var newunitid = selectnewunit.First().unitid;

        //            var houfornewunit = db.units.Single(u => u.unitid == newunitid);
        //            houfornewunit.HOU_ID = houidEmp;
        //            db.SaveChanges();

        //            var staffudateunitid = db.staffs.Single(u => u.staff_email == stafemal11);
        //            staffudateunitid.unit_id = newunitid;
        //            db.SaveChanges();

        //        }


        //        /////////////////////

        //        //ModelState.AddModelError("staffid2", "Name is required");
        //        //ViewBag.staffid2 = "Name is required";

        //        //var staffemail = staffs.First().staff_email;
        //        //var stafffullname = staffs.First ().staff_email  ;
        //        //var staffindex = staffs.First().staff_index_number;



        //        //MailMessage mailhou = new MailMessage();
        //        //mailhou.To.Add("ahmed.badr@wfp.org"); // odc ict
        //        //mailhou.CC.Add(staffemail);
        //        //mailhou.Bcc.Add("ahmed.badr@wfp.org");
        //        //mailhou.From = new MailAddress("RBS-no-replay@wfp.org");
        //        //mailhou.Subject = "Missing Information";

        //        //string Bodyhou = "<br>We missing Important information from " + stafffullname 
        //        //    + " index number " + staffindex
        //        //    + " &nbsp; record Please  update the data Like First Name, Last Name , Unit , Supervisor <br><br><font color='red'><font size='2' color='blue'>RBC SMS</font> This is an automatically generated email, please do not reply.</font>";

        //        ////string Body = "Request : Mission &nbsp;  &nbsp;  &nbsp; &nbsp;  &nbsp; &nbsp; &nbsp; &nbsp;  Mission ID : " + missionAuthorization.MissionID + " <br> " + " From &nbsp; &nbsp; &nbsp;:" + staffs.First().staff_first_name + "&nbsp;" + staffs.First().staff_last_name + "<br><br>" + " Mission Information" + "<br><br>" + " From :" + missionAuthorization.FromDate + "<br>To :" + missionAuthorization.ToDate +
        //        ////    "<br>Funding :" + missionAuthorization.funding + "<br>Objective :" + missionAuthorization.MissionObjective + "<br> Mission Itinerary :" + missionAuthorization.MissionItinerary + "<br> Can This meeting Be Done Over Voice/Video ? :" + missionAuthorization.canbedone  + " <br><br> <a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor=" + true + "&FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + " &to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Acept</a>&nbsp;&nbsp;&nbsp;<a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor= " + false + "& FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + "&to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Reject</a> ";

        //        //mailhou.Body = Bodyhou;
        //        //mailhou.IsBodyHtml = true;
        //        //SmtpClient smtphou = new SmtpClient();
        //        //smtphou.Host = "smtprelay.global.wfp.org";
        //        //smtphou.Port = 25;
        //        //smtphou.UseDefaultCredentials = true;
        //        ////smtp.Credentials = new System.Net.NetworkCredential
        //        ////("ahmed.badr", "Survivor2323");// Enter seders User name and password  
        //        ////smtp.EnableSsl = true;
        //        //smtphou.Send(mailhou);




        //    }

        //    var superid = staffs3.First().staffid;


        //    if (staffs3.First().staff_supervisorid == null)
        //    {
        //        staffs3.First().staff_supervisorid = 1212;
        //        ViewBag.error = 1;
        //    }

        //    if (staffs3.First().staff_first_name == null)
        //    {
        //        staffs3.First().staff_first_name = "You must have First name !!!";
        //        ViewBag.error = 1;
        //    }

        //    if (staffs3.First().staff_last_name == null)
        //    {
        //        staffs3.First().staff_last_name = "You must have Last name !!!";
        //        ViewBag.error = 1;
        //    }


        //    //ViewBag.staffid = new SelectList(db.staffs, "staffs.First().staffid", staffs.First().staff_email);
        //    ViewBag.staffid = staffs3.First().staffid;
        //    ViewBag.staffid2 = staffs3.First().staff_first_name;
        //    ViewBag.FirstName = staffs3.First().staff_first_name;
        //    ViewBag.LastName = staffs3.First().staff_last_name;
        //    ViewBag.unitid = staffs3.First().unit_id;
        //    ViewBag.currentDate = DateTime.Now;
        //    if (staffs3.First().unit_id == null)
        //    {
        //        ViewBag.Unit = "You must have a Unit !!!!";
        //        staffs3.First().unit_id = 15;
        //        ViewBag.error = 1;

        //    }
        //    else
        //    {

        //        ViewBag.Unit = staffs3.First().unit.unit_description_english;
        //    }
        //    var staffsupervisor = staffs3.First().staff_supervisorid;

        //    var userUnit = staffs3.First().unit_id;

        //    var supers = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
        //    supers = supers.Where(d => d.staffid == staffsupervisor);





        //    ViewBag.supervisor = (supers.First().staff_first_name + " " + supers.First().staff_last_name);


        //    var supervisoremailif = supers.First().staff_email;
        //    if (supervisoremailif == "muhannad.hadi@wfp.org" || supervisoremailif == "carlo.scaramella@wfp.org")
        //    {
        //        supervisoremailif = "rbc.management@wfp.org"; // rbc.management@wfp.org

        //    }

        //    ViewBag.supervisorEmail = supervisoremailif;
        //    ViewBag.SignatureDate = DateTime.Now;



        //    var unithou = db.units.Include(d => d.staffs);
        //    unithou = unithou.Where(u => u.unitid == userUnit);
        //    var houid = unithou.First().HOU_ID;

        //    var staffshou = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
        //    staffshou = staffshou.Where(h => h.staffid == houid);

        //    //////////////////////////////
        //    var superforhounitsname = "";
        //    var houmailUPDATE = "";
        //    var houidforSuper = staffshou.First().staffid;
        //    if (houidforSuper == staffforhouforsuper)
        //    {
        //        var supersforhou = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
        //        supersforhou = supersforhou.Where(d => d.staffid == staffsupervisor);
        //        superforhounitsname = supersforhou.First().staff_first_name + " " + staffshou.First().staff_last_name;
        //        houmailUPDATE = supersforhou.First().staff_email;

        //    }
        //    ///////////////////////////////

        //    var to = staffshou.First().staff_email;

        //    superforhounitsname = staffshou.First().staff_first_name + " " + staffshou.First().staff_last_name;
        //    ViewBag.houname = superforhounitsname;

        //    houmailUPDATE = staffshou.First().staff_email;

        //    if (houmailUPDATE == "muhannad.hadi@wfp.org" || houmailUPDATE == "carlo.scaramella@wfp.org")
        //    {
        //        houmailUPDATE = "rbc.management@wfp.org"; //rbc.management@wfp.org

        //    }


        //    ViewBag.houmail = houmailUPDATE;





        //    return View(requests1);
        //}
        public ActionResult NotAuthorized()
        {
            return View();
        }
        // GET: EventRequest/Edit/5
        public ActionResult Edit(int? id , int? staffid , String staffname)
        {


           
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            requests1 requests1 = db.requests1.Find(id);
            if (requests1 == null)
            {
                return HttpNotFound();
            }


             ViewBag.Focalpoint = new SelectList(db.staffs.OrderBy(staffs => staffs.staff_email), "staffid", "staff_email");
            if (!Request.IsAuthenticated)
            {
                Response.Redirect("/RBC-SMS/account/login");
            }
            //ViewBag.staffid = new SelectList(User.Identity.GetUserName() + "wfp.org");
            //var username = User.Identity.GetUserName();
           

            var staffs3 = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
            staffs3 = staffs3.Where(s => s.staff_email == User.Identity.Name + "@wfp.org");
            var stafemal11 = staffs3.First().staff_email;
            var staffforhouforsuper = staffs3.First().staffid;


           

            if (staffs3.First().staff_supervisorid == null || staffs3.First().staff_first_name == null || staffs3.First().unit_id == null || staffs3.First().unit_id == null)
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

            var superid = staffs3.First().staffid;


            if (staffs3.First().staff_supervisorid == null)
            {
                staffs3.First().staff_supervisorid = 1212;
                ViewBag.error = 1;
            }

            if (staffs3.First().staff_first_name == null)
            {
                staffs3.First().staff_first_name = "You must have First name !!!";
                ViewBag.error = 1;
            }

            if (staffs3.First().staff_last_name == null)
            {
                staffs3.First().staff_last_name = "You must have Last name !!!";
                ViewBag.error = 1;
            }


            //ViewBag.staffid = new SelectList(db.staffs, "staffs.First().staffid", staffs.First().staff_email);
            ViewBag.staffid = staffs3.First().staffid;
            ViewBag.staffid2 = staffs3.First().staff_first_name;
            ViewBag.FirstName = staffs3.First().staff_first_name;
            ViewBag.LastName = staffs3.First().staff_last_name;
            ViewBag.unitid = staffs3.First().unit_id;
            ViewBag.currentDate = DateTime.Now;
            ViewBag.non2 = new SelectList((db.staffs) , "staffid", "staff_email", requests1.non2);
           
            if (staffs3.First().unit_id == null)
            {
                ViewBag.Unit = "You must have a Unit !!!!";
                staffs3.First().unit_id = 15;
                ViewBag.error = 1;

            }
            else
            {

                ViewBag.Unit = staffs3.First().unit.unit_description_english;
            }
            var staffsupervisor = staffs3.First().staff_supervisorid;

            var userUnit = staffs3.First().unit_id;

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



         

            return View(requests1);
        }

        // POST: EventRequest/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,RequstingBy,RequestingDate,RequestingUnit,FocalPoint,Purpose,FromDate,ToDate,NumOFP_CO,NumOFP_RB,NumOFP_HQ,NumOFP_Other,Country,GOV,District,OtherPlace,Budget_TR,Number_Roms,Check_inDate,Check_OutDate,Rom_Type,Microphone,HiSpeedInternet,Conf_Call_Device,MMR_Days,BOR_Days,COffeBreak,COffeBreak_UNM,Lunch,Dinner,Coctail_Reception,Dedicated_IT_SUPP,DescripDaysORHours,Transportstion_ForLocal_Staff,Airport_Picup_Services,Other_Services,non1,non2,non3,Stationary,NameTags,Writingpads,ServicesReqOthers")] requests1 requests1)
        {
            if (ModelState.IsValid)
            {

                db.Entry(requests1).State = EntityState.Modified;
                db.SaveChanges();
                //////////////////////email////////////////////

                 //////////email////////////

                
                var staffs3 = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                staffs3 = staffs3.Where(s => s.staff_email == User.Identity.Name + "@wfp.org");
                var stafemal11 = staffs3.First().staff_email;
                var staffforhouforsuper = staffs3.First().staffid;




                if (staffs3.First().staff_supervisorid == null || staffs3.First().staff_first_name == null || staffs3.First().unit_id == null || staffs3.First().unit_id == null)
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

                var superid = staffs3.First().staffid;


                if (staffs3.First().staff_supervisorid == null)
                {
                    staffs3.First().staff_supervisorid = 1212;
                    ViewBag.error = 1;
                }

                if (staffs3.First().staff_first_name == null)
                {
                    staffs3.First().staff_first_name = "You must have First name !!!";
                    ViewBag.error = 1;
                }

                if (staffs3.First().staff_last_name == null)
                {
                    staffs3.First().staff_last_name = "You must have Last name !!!";
                    ViewBag.error = 1;
                }


                //ViewBag.staffid = new SelectList(db.staffs, "staffs.First().staffid", staffs.First().staff_email);
                ViewBag.staffid = staffs3.First().staffid;
                ViewBag.staffid2 = staffs3.First().staff_first_name;
                ViewBag.FirstName = staffs3.First().staff_first_name;
                ViewBag.LastName = staffs3.First().staff_last_name;
                ViewBag.unitid = staffs3.First().unit_id;
                ViewBag.currentDate = DateTime.Now;
                if (staffs3.First().unit_id == null)
                {
                    ViewBag.Unit = "You must have a Unit !!!!";
                    staffs3.First().unit_id = 15;
                    ViewBag.error = 1;

                }
                else
                {

                    ViewBag.Unit = staffs3.First().unit.unit_description_english;
                }
                var staffsupervisor = staffs3.First().staff_supervisorid;

                var userUnit = staffs3.First().unit_id;

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

             

                    var Microphone = "";
                    var HiSpeedInternet = "";
                    var Conf_Call_Device = "";
                    var COffeBreak = "";
                    var Dedicated_IT_SUPP = "";
                    var Lunch = "";
                    var Dinner = "";
                    var Coctail_Reception = "";
                    var Transportstion_ForLocal_Staff = "";
                    var Airport_Picup_Services = "";
                    //var Stationary = "";
                    var NameTags = "";
                    var Writingpads = "";

                    if (requests1.Microphone == true)
                    {
                        Microphone = "Yes";
                    }
                    else
                    {
                        Microphone = "No";
                    }

                    if (requests1.HiSpeedInternet == true)
                    {
                        HiSpeedInternet = "Yes";
                    }
                    else
                    {
                        HiSpeedInternet = "No";
                    }

                    if (requests1.Conf_Call_Device == true)
                    {
                        Conf_Call_Device = "Yes";
                    }
                    else
                    {
                        Conf_Call_Device = "No";
                    }

                    if (requests1.COffeBreak == true)
                    {
                        COffeBreak = "Yes";
                    }
                    else
                    {
                        COffeBreak = "No";
                    }

                    if (requests1.Dedicated_IT_SUPP == true)
                    {
                        Dedicated_IT_SUPP = "Yes";
                    }
                    else
                    {
                        Dedicated_IT_SUPP = "No";
                    }

                    if (requests1.Lunch == true)
                    {
                        Lunch = "Yes";
                    }
                    else
                    {
                        Lunch = "No";
                    }

                    if (requests1.Dinner == true)
                    {
                        Dinner = "Yes";
                    }
                    else
                    {
                        Dinner = "No";
                    }

                    if (requests1.Coctail_Reception == true)
                    {
                        Coctail_Reception = "Yes";
                    }
                    else
                    {
                        Coctail_Reception = "No";
                    }
                    if (requests1.Transportstion_ForLocal_Staff == true)
                    {
                        Transportstion_ForLocal_Staff = "Yes";
                    }
                    else
                    {
                        Transportstion_ForLocal_Staff = "No";
                    }
                    if (requests1.Airport_Picup_Services == true)
                    {
                        Airport_Picup_Services = "Yes";
                    }
                    else
                    {
                        Airport_Picup_Services = "No";
                    }
                   
                    if (requests1.NameTags == true)
                    {
                        NameTags = "Yes";
                    }
                    else
                    {
                        NameTags = "No";
                    }
                    if (requests1.Writingpads == true)
                    {
                        Writingpads = "Yes";
                    }
                    else
                    {
                        Writingpads = "No";
                    }
                    var staffs = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                    staffs = staffs.Where(s => s.staffid == requests1.RequstingBy);
                    var requestedby = staffs.First().staff_first_name + " " + staffs.First().staff_last_name;
                    var requestedbyemail = staffs.First().staff_email;

                 var staffsforapprovall = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                 staffsforapprovall = staffsforapprovall.Where(s => s.staffid == requests1.non2);
                 var forapprovall = staffsforapprovall.First().staff_first_name + " " + staffsforapprovall.First().staff_last_name;
                    var forapprovallEmail = staffsforapprovall.First().staff_email;

                    //var title = db.functional_title.Include(s => s.staffs);
                    //title = title.Where(t => t.functionaltitleid == requests1.job_title);
                    //var jobtitle = title.First().functional_title_description_english;

                    var unitselect = db.units.Include(s => s.staffs);
                    unitselect = unitselect.Where(u => u.unitid == requests1.RequestingUnit);
                    var unit = unitselect.First().unit_description_english;

                    //var Superemail = db.staffs.Include(s => s.district);
                    //Superemail = Superemail.Where(Su => Su.staffid == requests1.supervisor_email);
                    //var supervisor_email = Superemail.First().staff_email;



                    MailMessage mail = new MailMessage();
                    mail.To.Add(forapprovallEmail);
                    //mail.CC.Add("cairo.itservicedesk@wfp.org");
                    mail.CC.Add("hanaa.ibrahim@wfp.org");
                    mail.CC.Add(requestedbyemail);
                    //mail.CC.Add("mahmoud.abdelfattah@wfp.org");
                    //mail.CC.Add("amal.mohamed@wfp.org");
                    mail.Bcc.Add("ahmed.badr@wfp.org");
                    mail.From = new MailAddress(requestedbyemail);
                    mail.Subject = " Event Update : " + requests1.Purpose;

                    string Body = "<table border='0'><tr><td colspan='2'><hr/>Request Information<hr/></td><hr/></tr><tr><td><font color='blue'>Request Number: </font></td><td>" + requests1.ID + "</td></tr>"
                     + "<tr><td><font color='blue'>" + " Requested By :</font></td><td>" + requestedby + "</td></tr>"
                     + "<tr><td><font color='blue'>Requesting UNIT :</font></td><td>" + unit + "</td></tr>"
                     + "<tr><td><font color='blue'>FOCAL POINT:</font></td><td>" + staffs3.First().staff_first_name + " " + staffs3.First().staff_last_name + "</td></tr>"
                     + "<tr><td><font color='blue'>Requesting Date :</font></td><td>" + requests1.RequestingDate + "</td></tr>"
                     + "<tr><td><font color='blue'>Purpose :</font></td><td>" + requests1.Purpose + "</td></tr>"
                     + "<tr><td><font color='blue'>FROM DATE  :</font></td><td>" + requests1.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                     + "<tr><td><font color='blue'>TO DATE :</font></td><td>" + requests1.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                     + "<tr><td colspan='2'><hr/>Participates Information <hr/></td></tr>"
                     + "<tr><td><font color='blue'>Number OF Persons EGYCO:</font></td><td>" + requests1.NumOFP_CO + "</td></tr>"
                     + "<tr><td><font color='blue'>Number OF Persons RB :</font></td><td>" + requests1.NumOFP_RB + "</td></tr>"
                     + "<tr><td><font color='blue'>Number OF Persons HQ :</font></td><td>" + requests1.NumOFP_HQ + "</td></tr>"
                     + "<tr><td><font color='blue'>Others :</font></td><td>" + requests1.NumOFP_Other + "</td></tr>"
                     + "<tr><td colspan='2'><hr/>Location Information <hr/></td></tr>"
                     + "<tr><td><font color='blue'>Country :</font></td><td>" + requests1.Country + "</td></tr>"
                     + "<tr><td><font color='blue'>District :</font></td><td>" + requests1.District + "</td></tr>"
                     + "<tr><td><font color='blue'>Other Place :</font></td><td>" + requests1.OtherPlace + "</td></tr>"
                     + "<tr><td colspan='2'><hr/>Hotel Rooms  Information <hr/></td></tr>"
                     + "<tr><td><font color='blue'>Number of Rooms :</font></td><td>" + requests1.Number_Roms + "</td></tr>"
                     + "<tr><td><font color='blue'>Check in Date :</font></td><td>" + requests1.Check_inDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                     + "<tr><td><font color='blue'>Check Out Date :</font></td><td>" + requests1.Check_OutDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                     + "<tr><td><font color='blue'>Main Meeting Room:</font></td><td>" + requests1.MMR_Days + "</td></tr>"
                     + "<tr><td><font color='blue'>SITTING LOCATION :</font></td><td>" + requests1.BOR_Days + "</td></tr>"
                     + "<tr><td colspan='2'><hr/>SERVICES REQUIRED <hr/></td></tr>"
                     + "<tr><td><font color='blue'>Microphone:</font></td><td>" + Microphone + "</td></tr>"
                     + "<tr><td><font color='blue'>High Speed Internet:</font></td><td>" + HiSpeedInternet  + "</td></tr>"
                   
                     //+ "<tr><td><font color='blue'>Dedicated IT Support :</font></td><td>" + Dedicated_IT_SUPP + "</td></tr>"
                     //+ "<tr><td><font color='blue'>Descrip Days/Hours:</font></td><td>" + requests1.DescripDaysORHours  + "</td></tr>"
                     + "<tr><td><font color='blue'>Confrance Call Device :</font></td><td>" + Conf_Call_Device + "</td></tr>"
                     + "<tr><td><font color='blue'>Dedicated IT Support :</font></td><td>" + Dedicated_IT_SUPP + "</td></tr>"
                     + "<tr><td><font color='blue'>Descrip Days/Hours:</font></td><td>" + requests1.DescripDaysORHours + "</td></tr>"
                      + "<tr><td><font color='blue'>STATIONARY:</font></td><td>" + requests1.Stationary + "</td></tr>"
                     + "<tr><td><font color='blue'>Name Tags:</font></td><td>" + NameTags + "</td></tr>"
                     + "<tr><td><font color='blue'>Writing Pads:</font></td><td>" + Writingpads + "</td></tr>"
                     + "<tr><td><font color='blue'>Others Services:</font></td><td>" + requests1.ServicesReqOthers + "</td></tr>"
                     + "<tr><td colspan='2'><hr/>Catering Information <hr/></td></tr>"
                     + "<tr><td><font color='blue'>COffee Break:</font></td><td>" + COffeBreak + "</td></tr>"
                     + "<tr><td><font color='blue'>COffee Break Number:</font></td><td>" + requests1.COffeBreak_UNM + "</td></tr>"
                     + "<tr><td><font color='blue'>COCKTAIL RECEPTION :</font></td><td>" + Coctail_Reception + "</td></tr>"
                     + "<tr><td><font color='blue'>Lunch:</font></td><td>" + Lunch + "</td></tr>"
                     + "<tr><td><font color='blue'>Dinner :</font></td><td>" + Dinner + "</td></tr>"
                     + "<tr><td colspan='2'><hr/>Transportstion <hr/></td></tr>"
                      + "<tr><td><font color='blue'>TRANSPORTATION FOR LOCAL STAFF:</font></td><td>" + Transportstion_ForLocal_Staff + "</td></tr>"
                     + "<tr><td><font color='blue'>AIRPORT PICK UP SERVICES :</font></td><td>" + Airport_Picup_Services + "</td></tr>"
                      + "<tr><td><a href='http://10.11.135.254:8080/RBC-SMS/EventRequest/Approval2?Mid="
                        + requests1.ID
                        + "&RDORDRDSignature=Yes&FromDate=" + requests1.FromDate
                        + " &ToDate=" + requests1.ToDate
                        + " &to=" + to + "&staffmail=" + forapprovall + "&forapprovallEmail=" + forapprovallEmail
                        + "&requestedbyemail=" + requestedbyemail
                      + "'>Accept</a></td></tr><tr><td><a href='http://10.11.135.254:8080/RBC-SMS/EventRequest/Approval2?Mid=" + requests1.ID 
                        + "&RDORDRDSignature=NO & FromDate=" + requests1.FromDate
                        + " &ToDate=" + requests1.ToDate
                        + "&to=" + to
                        + "&staffmail=" + forapprovall
                         + "&forapprovallEmail=" + forapprovallEmail
                        + "&requestedbyemail=" + requestedbyemail
                        + "'>Reject</a>"  ;

                    //string Body = "Request : Mission &nbsp;  &nbsp;  &nbsp; &nbsp;  &nbsp; &nbsp; &nbsp; &nbsp;  Mission ID : " + missionAuthorization.MissionID + " <br> " + " From &nbsp; &nbsp; &nbsp;:" + staffs.First().staff_first_name + "&nbsp;" + staffs.First().staff_last_name + "<br><br>" + " Mission Information" + "<br><br>" + " From :" + missionAuthorization.FromDate + "<br>To :" + missionAuthorization.ToDate +
                    //    "<br>Funding :" + missionAuthorization.funding + "<br>Objective :" + missionAuthorization.MissionObjective + "<br> Mission Itinerary :" + missionAuthorization.MissionItinerary + "<br> Can This meeting Be Done Over Voice/Video ? :" + missionAuthorization.canbedone  + " <br><br> <a href='http://10.11.135.254:8080/WFPSMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor=" + true + "&FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + " &to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Acept</a>&nbsp;&nbsp;&nbsp;<a href='http://10.11.135.254:8080/WFPSMS/MissionAuthorizations/Approval?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor= " + false + "& FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + "&to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Reject</a> ";

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

                    return RedirectToAction("Index");


                //////////end email//////////////
                

///////////////////////////////////////////email/////////////








            }
            return View(requests1);
        }





        ////////////////////////////////////////////////////// approval ///////////////////////////////////////////

        public ActionResult Approval2(int? Mid, string RDORDRDSignature, DateTime? FromDate, DateTime? ToDate, string to, string staffmail, string supervisorname, int? onbehalfID, int? onbehalfSuperID, int? onbehalfHouID, string forapprovallEmail, string requestedbyemail)
        {


           

            ///////////////////////// start on behalf 

           
                        // missionAuthorization = db.MissionAuthorizations.Find(Mid);
                        //db.Entry(missionAuthorization).State = EntityState.Modified ;
                        //db.SaveChanges();

            if (RDORDRDSignature == "Yes")
            {

                var eventreques = db.requests1.Single(u => u.ID == Mid);
                eventreques.non1 = "Yes";
                eventreques.non3 = DateTime.Now;
                db.SaveChanges();



                var staffs = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                staffs = staffs.Where(s => s.staffid == eventreques.RequstingBy);
                var too = staffs.First().staff_email;
                ////////////////////////////////////////////////////////////////////////////onbehalf
                var staffsupervisor = staffs.First().staff_supervisorid;
                var supers = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                supers = supers.Where(d => d.staffid == eventreques.RequstingBy);
                var onbehalfemail = supers.First().staff_email;


                var supersonbehalf = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                supersonbehalf = supersonbehalf.Where(d => d.staffid == eventreques.RequstingBy);
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
                var Microphone = "";
                var HiSpeedInternet = "";
                var Conf_Call_Device = "";
                var COffeBreak = "";
                var Dedicated_IT_SUPP = "";
                var Lunch = "";
                var Dinner = "";
                var Coctail_Reception = "";
                var Transportstion_ForLocal_Staff = "";
                var Airport_Picup_Services = "";
                //var Stationary = "";
                var NameTags = "";
                var Writingpads = "";

                if (eventreques.Microphone == true)
                {
                    Microphone = "Yes";
                }
                else
                {
                    Microphone = "No";
                }

                if (eventreques.HiSpeedInternet == true)
                {
                    HiSpeedInternet = "Yes";
                }
                else
                {
                    HiSpeedInternet = "No";
                }

                if (eventreques.Conf_Call_Device == true)
                {
                    Conf_Call_Device = "Yes";
                }
                else
                {
                    Conf_Call_Device = "No";
                }

                if (eventreques.COffeBreak == true)
                {
                    COffeBreak = "Yes";
                }
                else
                {
                    COffeBreak = "No";
                }

                if (eventreques.Dedicated_IT_SUPP == true)
                {
                    Dedicated_IT_SUPP = "Yes";
                }
                else
                {
                    Dedicated_IT_SUPP = "No";
                }

                if (eventreques.Lunch == true)
                {
                    Lunch = "Yes";
                }
                else
                {
                    Lunch = "No";
                }

                if (eventreques.Dinner == true)
                {
                    Dinner = "Yes";
                }
                else
                {
                    Dinner = "No";
                }

                if (eventreques.Coctail_Reception == true)
                {
                    Coctail_Reception = "Yes";
                }
                else
                {
                    Coctail_Reception = "No";
                }
                if (eventreques.Transportstion_ForLocal_Staff == true)
                {
                    Transportstion_ForLocal_Staff = "Yes";
                }
                else
                {
                    Transportstion_ForLocal_Staff = "No";
                }
                if (eventreques.Airport_Picup_Services == true)
                {
                    Airport_Picup_Services = "Yes";
                }
                else
                {
                    Airport_Picup_Services = "No";
                }

                if (eventreques.NameTags == true)
                {
                    NameTags = "Yes";
                }
                else
                {
                    NameTags = "No";
                }
                if (eventreques.Writingpads == true)
                {
                    Writingpads = "Yes";
                }
                else
                {
                    Writingpads = "No";
                }



                var requestedby = staffs.First().staff_first_name + " " + staffs.First().staff_last_name;
                var requestedbyemail1 = staffs.First().staff_email;

                //var staffsforapprovall = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                //staffsforapprovall = staffsforapprovall.Where(s => s.staffid == requests1.non);
                //var forapprovall = staffsforapprovall.First().staff_first_name + " " + staffsforapprovall.First().staff_last_name;
                //   var forapprovallEmail = staffsforapprovall.First().staff_email;

                //var title = db.functional_title.Include(s => s.staffs);
                //title = title.Where(t => t.functionaltitleid == requests1.job_title);
                //var jobtitle = title.First().functional_title_description_english;

                var unitselect = db.units.Include(s => s.staffs);
                unitselect = unitselect.Where(u => u.unitid == eventreques.RequestingUnit);
                var unit = unitselect.First().unit_description_english;

                //var Superemail = db.staffs.Include(s => s.district);
                //Superemail = Superemail.Where(Su => Su.staffid == requests1.supervisor_email);
                //var supervisor_email = Superemail.First().staff_email;



                MailMessage mail = new MailMessage();
                mail.To.Add(requestedbyemail);
                mail.CC.Add("hanaa.ibrahim@wfp.org");

                mail.CC.Add(forapprovallEmail);
                //mail.CC.Add("mahmoud.abdelfattah@wfp.org");
                //mail.CC.Add("amal.mohamed@wfp.org");
                mail.Bcc.Add("ahmed.badr@wfp.org");
                mail.From = new MailAddress(forapprovallEmail);
                mail.Subject = " Approval For Event : " + eventreques.Purpose;

                string Body = "<table border='0'><tr><td colspan='2'><hr/>Request Information<hr/></td><hr/></tr><tr><td><font color='blue'>Request Number: </font></td><td>" + eventreques.ID + "</td></tr>"
                 + "<tr><td><font color='blue'>" + " Requested By :</font></td><td>" + requestedby + "</td></tr>"
                 + "<tr><td><font color='blue'>Requesting UNIT :</font></td><td>" + unit + "</td></tr>"
                 + "<tr><td><font color='blue'>FOCAL POINT:</font></td><td>" + requestedbyemail + "</td></tr>"
                 + "<tr><td><font color='blue'>Requesting Date :</font></td><td>" + eventreques.RequestingDate + "</td></tr>"
                 + "<tr><td><font color='blue'>Purpose :</font></td><td>" + eventreques.Purpose + "</td></tr>"
                 + "<tr><td><font color='blue'>FROM DATE  :</font></td><td>" + eventreques.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                 + "<tr><td><font color='blue'>TO DATE :</font></td><td>" + eventreques.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                 + "<tr><td colspan='2'><hr/>Participates Information <hr/></td></tr>"
                 + "<tr><td><font color='blue'>Number OF Persons EGYCO:</font></td><td>" + eventreques.NumOFP_CO + "</td></tr>"
                 + "<tr><td><font color='blue'>Number OF Persons RB :</font></td><td>" + eventreques.NumOFP_RB + "</td></tr>"
                 + "<tr><td><font color='blue'>Number OF Persons HQ :</font></td><td>" + eventreques.NumOFP_HQ + "</td></tr>"
                 + "<tr><td><font color='blue'>Others :</font></td><td>" + eventreques.NumOFP_Other + "</td></tr>"
                 + "<tr><td colspan='2'><hr/>Location Information <hr/></td></tr>"
                 + "<tr><td><font color='blue'>Country :</font></td><td>" + eventreques.Country + "</td></tr>"
                 + "<tr><td><font color='blue'>District :</font></td><td>" + eventreques.District + "</td></tr>"
                 + "<tr><td><font color='blue'>Other Place :</font></td><td>" + eventreques.OtherPlace + "</td></tr>"
                 + "<tr><td colspan='2'><hr/>Hotel Rooms  Information <hr/></td></tr>"
                 + "<tr><td><font color='blue'>Number of Rooms :</font></td><td>" + eventreques.Number_Roms + "</td></tr>"
                 + "<tr><td><font color='blue'>Check in Date :</font></td><td>" + eventreques.Check_inDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                 + "<tr><td><font color='blue'>Check Out Date :</font></td><td>" + eventreques.Check_OutDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                 + "<tr><td><font color='blue'>Main Meeting Room:</font></td><td>" + eventreques.MMR_Days + "</td></tr>"
                 + "<tr><td><font color='blue'>SITTING LOCATION :</font></td><td>" + eventreques.BOR_Days + "</td></tr>"
                 + "<tr><td colspan='2'><hr/>SERVICES REQUIRED <hr/></td></tr>"
                 + "<tr><td><font color='blue'>Microphone:</font></td><td>" + Microphone + "</td></tr>"
                 + "<tr><td><font color='blue'>High Speed Internet:</font></td><td>" + HiSpeedInternet + "</td></tr>"
                 + "<tr><td><font color='blue'>Confrance Call Device :</font></td><td>" + Conf_Call_Device + "</td></tr>"
                 + "<tr><td><font color='blue'>Dedicated IT Support :</font></td><td>" + Dedicated_IT_SUPP + "</td></tr>"
                 + "<tr><td><font color='blue'>Descrip Days/Hours:</font></td><td>" + eventreques.DescripDaysORHours + "</td></tr>"
                 //+ "<tr><td><font color='blue'>Confrance Call Device :</font></td><td>" + Conf_Call_Device + "</td></tr>"
                 ////+ "<tr><td><font color='blue'>Dedicated IT Support :</font></td><td>" + Dedicated_IT_SUPP + "</td></tr>"
                 //+ "<tr><td><font color='blue'>Descrip Days/Hours:</font></td><td>" + eventreques.DescripDaysORHours + "</td></tr>"
                  + "<tr><td><font color='blue'>STATIONARY:</font></td><td>" + eventreques.Stationary + "</td></tr>"
                 + "<tr><td><font color='blue'>Name Tags:</font></td><td>" + NameTags + "</td></tr>"
                 + "<tr><td><font color='blue'>Writing Pads:</font></td><td>" + Writingpads + "</td></tr>"
                 + "<tr><td><font color='blue'>Others Services:</font></td><td>" + eventreques.ServicesReqOthers + "</td></tr>"
                 + "<tr><td colspan='2'><hr/>Catering Information <hr/></td></tr>"
                 + "<tr><td><font color='blue'>COffee Break:</font></td><td>" + COffeBreak + "</td></tr>"
                 + "<tr><td><font color='blue'>COffee Break Number:</font></td><td>" + eventreques.COffeBreak_UNM + "</td></tr>"
                 + "<tr><td><font color='blue'>COCKTAIL RECEPTION :</font></td><td>" + Coctail_Reception + "</td></tr>"
                 + "<tr><td><font color='blue'>Lunch:</font></td><td>" + Lunch + "</td></tr>"
                 + "<tr><td><font color='blue'>Dinner :</font></td><td>" + Dinner + "</td></tr>"
                 + "<tr><td colspan='2'><hr/>Transportstion <hr/></td></tr>"
                  + "<tr><td><font color='blue'>TRANSPORTATION FOR LOCAL STAFF:</font></td><td>" + Transportstion_ForLocal_Staff + "</td></tr>"
                 + "<tr><td><font color='blue'>AIRPORT PICK UP SERVICES :</font></td><td>" + Airport_Picup_Services + "</td></tr></table>"
                  ;

                //string Body = "Request : Mission &nbsp;  &nbsp;  &nbsp; &nbsp;  &nbsp; &nbsp; &nbsp; &nbsp;  Mission ID : " + missionAuthorization.MissionID + " <br> " + " From &nbsp; &nbsp; &nbsp;:" + staffs.First().staff_first_name + "&nbsp;" + staffs.First().staff_last_name + "<br><br>" + " Mission Information" + "<br><br>" + " From :" + missionAuthorization.FromDate + "<br>To :" + missionAuthorization.ToDate +
                //    "<br>Funding :" + missionAuthorization.funding + "<br>Objective :" + missionAuthorization.MissionObjective + "<br> Mission Itinerary :" + missionAuthorization.MissionItinerary + "<br> Can This meeting Be Done Over Voice/Video ? :" + missionAuthorization.canbedone  + " <br><br> <a href='http://10.11.135.254:8080/WFPSMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor=" + true + "&FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + " &to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Acept</a>&nbsp;&nbsp;&nbsp;<a href='http://10.11.135.254:8080/WFPSMS/MissionAuthorizations/Approval?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor= " + false + "& FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + "&to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Reject</a> ";

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
                return RedirectToAction("Index");
            }
            else
            {

                var eventreques = db.requests1.Single(u => u.ID == Mid);
                eventreques.non1 = "NO";
                eventreques.non3 = DateTime.Now;
                db.SaveChanges();



                var staffs = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                staffs = staffs.Where(s => s.staffid == eventreques.RequstingBy);
                var too = staffs.First().staff_email;
                ////////////////////////////////////////////////////////////////////////////onbehalf
                var staffsupervisor = staffs.First().staff_supervisorid;
                var supers = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                supers = supers.Where(d => d.staffid == eventreques.RequstingBy);
                var onbehalfemail = supers.First().staff_email;


                var supersonbehalf = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                supersonbehalf = supersonbehalf.Where(d => d.staffid == eventreques.RequstingBy);
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
                var Microphone = "";
                var HiSpeedInternet = "";
                var Conf_Call_Device = "";
                var COffeBreak = "";
                var Dedicated_IT_SUPP = "";
                var Lunch = "";
                var Dinner = "";
                var Coctail_Reception = "";
                var Transportstion_ForLocal_Staff = "";
                var Airport_Picup_Services = "";
                //var Stationary = "";
                var NameTags = "";
                var Writingpads = "";

                if (eventreques.Microphone == true)
                {
                    Microphone = "Yes";
                }
                else
                {
                    Microphone = "No";
                }

                if (eventreques.HiSpeedInternet == true)
                {
                    HiSpeedInternet = "Yes";
                }
                else
                {
                    HiSpeedInternet = "No";
                }

                if (eventreques.Conf_Call_Device == true)
                {
                    Conf_Call_Device = "Yes";
                }
                else
                {
                    Conf_Call_Device = "No";
                }

                if (eventreques.COffeBreak == true)
                {
                    COffeBreak = "Yes";
                }
                else
                {
                    COffeBreak = "No";
                }

                if (eventreques.Dedicated_IT_SUPP == true)
                {
                    Dedicated_IT_SUPP = "Yes";
                }
                else
                {
                    Dedicated_IT_SUPP = "No";
                }

                if (eventreques.Lunch == true)
                {
                    Lunch = "Yes";
                }
                else
                {
                    Lunch = "No";
                }

                if (eventreques.Dinner == true)
                {
                    Dinner = "Yes";
                }
                else
                {
                    Dinner = "No";
                }

                if (eventreques.Coctail_Reception == true)
                {
                    Coctail_Reception = "Yes";
                }
                else
                {
                    Coctail_Reception = "No";
                }
                if (eventreques.Transportstion_ForLocal_Staff == true)
                {
                    Transportstion_ForLocal_Staff = "Yes";
                }
                else
                {
                    Transportstion_ForLocal_Staff = "No";
                }
                if (eventreques.Airport_Picup_Services == true)
                {
                    Airport_Picup_Services = "Yes";
                }
                else
                {
                    Airport_Picup_Services = "No";
                }

                if (eventreques.NameTags == true)
                {
                    NameTags = "Yes";
                }
                else
                {
                    NameTags = "No";
                }
                if (eventreques.Writingpads == true)
                {
                    Writingpads = "Yes";
                }
                else
                {
                    Writingpads = "No";
                }



                var requestedby = staffs.First().staff_first_name + " " + staffs.First().staff_last_name;
                var requestedbyemail1 = staffs.First().staff_email;

                //var staffsforapprovall = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                //staffsforapprovall = staffsforapprovall.Where(s => s.staffid == requests1.non);
                //var forapprovall = staffsforapprovall.First().staff_first_name + " " + staffsforapprovall.First().staff_last_name;
                //   var forapprovallEmail = staffsforapprovall.First().staff_email;

                //var title = db.functional_title.Include(s => s.staffs);
                //title = title.Where(t => t.functionaltitleid == requests1.job_title);
                //var jobtitle = title.First().functional_title_description_english;

                var unitselect = db.units.Include(s => s.staffs);
                unitselect = unitselect.Where(u => u.unitid == eventreques.RequestingUnit);
                var unit = unitselect.First().unit_description_english;

                //var Superemail = db.staffs.Include(s => s.district);
                //Superemail = Superemail.Where(Su => Su.staffid == requests1.supervisor_email);
                //var supervisor_email = Superemail.First().staff_email;



                MailMessage mail = new MailMessage();
                mail.To.Add(forapprovallEmail);
                //mail.CC.Add("cairo.itservicedesk@wfp.org");
                mail.CC.Add("hanaa.ibrahim@wfp.org");
                mail.CC.Add(requestedbyemail);
                //mail.CC.Add("mahmoud.abdelfattah@wfp.org");
                //mail.CC.Add("amal.mohamed@wfp.org");
                mail.Bcc.Add("ahmed.badr@wfp.org");
                mail.From = new MailAddress(forapprovallEmail);
                mail.Subject = " Reject For Event : " + eventreques.Purpose;

                string Body = "<table border='0'><tr><td colspan='2'><hr/>Request Information<hr/></td><hr/></tr><tr><td><font color='blue'>Request Number: </font></td><td>" + eventreques.ID + "</td></tr>"
                 + "<tr><td><font color='blue'>" + " Requested By :</font></td><td>" + requestedby + "</td></tr>"
                 + "<tr><td><font color='blue'>Requesting UNIT :</font></td><td>" + unit + "</td></tr>"
                 + "<tr><td><font color='blue'>FOCAL POINT:</font></td><td>" + requestedbyemail + "</td></tr>"
                 + "<tr><td><font color='blue'>Requesting Date :</font></td><td>" + eventreques.RequestingDate + "</td></tr>"
                 + "<tr><td><font color='blue'>Purpose :</font></td><td>" + eventreques.Purpose + "</td></tr>"
                 + "<tr><td><font color='blue'>FROM DATE  :</font></td><td>" + eventreques.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                 + "<tr><td><font color='blue'>TO DATE :</font></td><td>" + eventreques.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                 + "<tr><td colspan='2'><hr/>Participates Information <hr/></td></tr>"
                 + "<tr><td><font color='blue'>Number OF Persons EGYCO:</font></td><td>" + eventreques.NumOFP_CO + "</td></tr>"
                 + "<tr><td><font color='blue'>Number OF Persons RB :</font></td><td>" + eventreques.NumOFP_RB + "</td></tr>"
                 + "<tr><td><font color='blue'>Number OF Persons HQ :</font></td><td>" + eventreques.NumOFP_HQ + "</td></tr>"
                 + "<tr><td><font color='blue'>Others :</font></td><td>" + eventreques.NumOFP_Other + "</td></tr>"
                 + "<tr><td colspan='2'><hr/>Location Information <hr/></td></tr>"
                 + "<tr><td><font color='blue'>Country :</font></td><td>" + eventreques.Country + "</td></tr>"
                 + "<tr><td><font color='blue'>District :</font></td><td>" + eventreques.District + "</td></tr>"
                 + "<tr><td><font color='blue'>Other Place :</font></td><td>" + eventreques.OtherPlace + "</td></tr>"
                 + "<tr><td colspan='2'><hr/>Hotel Rooms  Information <hr/></td></tr>"
                 + "<tr><td><font color='blue'>Number of Rooms :</font></td><td>" + eventreques.Number_Roms + "</td></tr>"
                 + "<tr><td><font color='blue'>Check in Date :</font></td><td>" + eventreques.Check_inDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                 + "<tr><td><font color='blue'>Check Out Date :</font></td><td>" + eventreques.Check_OutDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                 + "<tr><td><font color='blue'>Main Meeting Room:</font></td><td>" + eventreques.MMR_Days + "</td></tr>"
                 + "<tr><td><font color='blue'>SITTING LOCATION :</font></td><td>" + eventreques.BOR_Days + "</td></tr>"
                 + "<tr><td colspan='2'><hr/>SERVICES REQUIRED <hr/></td></tr>"
                 + "<tr><td><font color='blue'>Microphone:</font></td><td>" + Microphone + "</td></tr>"
                 + "<tr><td><font color='blue'>High Speed Internet:</font></td><td>" + HiSpeedInternet + "</td></tr>"
                 + "<tr><td><font color='blue'>Confrance Call Device :</font></td><td>" + Conf_Call_Device + "</td></tr>"
                 + "<tr><td><font color='blue'>Dedicated IT Support :</font></td><td>" + Dedicated_IT_SUPP + "</td></tr>"
                 + "<tr><td><font color='blue'>Descrip Days/Hours:</font></td><td>" + eventreques.DescripDaysORHours + "</td></tr>"
                 //+ "<tr><td><font color='blue'>Confrance Call Device :</font></td><td>" + Conf_Call_Device + "</td></tr>"
                 //+ "<tr><td><font color='blue'>Dedicated IT Support :</font></td><td>" + Dedicated_IT_SUPP + "</td></tr>"
                 //+ "<tr><td><font color='blue'>Descrip Days/Hours:</font></td><td>" + eventreques.DescripDaysORHours + "</td></tr>"
                  + "<tr><td><font color='blue'>STATIONARY:</font></td><td>" + eventreques.Stationary + "</td></tr>"
                 + "<tr><td><font color='blue'>Name Tags:</font></td><td>" + NameTags + "</td></tr>"
                 + "<tr><td><font color='blue'>Writing Pads:</font></td><td>" + Writingpads + "</td></tr>"
                 + "<tr><td><font color='blue'>Others Services:</font></td><td>" + eventreques.ServicesReqOthers + "</td></tr>"
                 + "<tr><td colspan='2'><hr/>Catering Information <hr/></td></tr>"
                 + "<tr><td><font color='blue'>COffee Break:</font></td><td>" + COffeBreak + "</td></tr>"
                 + "<tr><td><font color='blue'>COffee Break Number:</font></td><td>" + eventreques.COffeBreak_UNM + "</td></tr>"
                 + "<tr><td><font color='blue'>COCKTAIL RECEPTION :</font></td><td>" + Coctail_Reception + "</td></tr>"
                 + "<tr><td><font color='blue'>Lunch:</font></td><td>" + Lunch + "</td></tr>"
                 + "<tr><td><font color='blue'>Dinner :</font></td><td>" + Dinner + "</td></tr>"
                 + "<tr><td colspan='2'><hr/>Transportstion <hr/></td></tr>"
                  + "<tr><td><font color='blue'>TRANSPORTATION FOR LOCAL STAFF:</font></td><td>" + Transportstion_ForLocal_Staff + "</td></tr>"
                 + "<tr><td><font color='blue'>AIRPORT PICK UP SERVICES :</font></td><td>" + Airport_Picup_Services + "</td></tr></table>"
                  ;

                //string Body = "Request : Mission &nbsp;  &nbsp;  &nbsp; &nbsp;  &nbsp; &nbsp; &nbsp; &nbsp;  Mission ID : " + missionAuthorization.MissionID + " <br> " + " From &nbsp; &nbsp; &nbsp;:" + staffs.First().staff_first_name + "&nbsp;" + staffs.First().staff_last_name + "<br><br>" + " Mission Information" + "<br><br>" + " From :" + missionAuthorization.FromDate + "<br>To :" + missionAuthorization.ToDate +
                //    "<br>Funding :" + missionAuthorization.funding + "<br>Objective :" + missionAuthorization.MissionObjective + "<br> Mission Itinerary :" + missionAuthorization.MissionItinerary + "<br> Can This meeting Be Done Over Voice/Video ? :" + missionAuthorization.canbedone  + " <br><br> <a href='http://10.11.135.254:8080/WFPSMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor=" + true + "&FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + " &to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Acept</a>&nbsp;&nbsp;&nbsp;<a href='http://10.11.135.254:8080/WFPSMS/MissionAuthorizations/Approval?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor= " + false + "& FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + "&to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Reject</a> ";

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
                return RedirectToAction("Index");
            
            
            }



            return View();

        }

        ////////////////////////////////////////////////////// approval ///////////////////////////////////////////

           
        // GET: EventRequest/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            requests1 requests1 = db.requests1.Find(id);
            if (requests1 == null)
            {
                return HttpNotFound();
            }
            return View(requests1);
        }

        // POST: EventRequest/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            requests1 requests1 = db.requests1.Find(id);
            db.requests1.Remove(requests1);
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
