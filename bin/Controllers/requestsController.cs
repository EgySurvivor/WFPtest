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
//using CodeEffects.Rule;
//using CodeEffects.Rule.Attributes;
//using CodeEffects.Rule.Core;
//using CodeEffects.Rule.Models;


namespace WFPtest.Controllers
{
    
    public class requestsController : Controller
    {
        private WFPEntities1 db = new WFPEntities1();

        // GET: requests
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {

            var result = from p in db.requests 
                         join a in db.requests 
                         on p.first_name  equals a.first_name 


                         select new
                         {
                             staffSuperName = p.first_name  + " " + p.first_name ,


                         };

            ViewBag.newstaffSuperName = result;


            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = String.IsNullOrEmpty(sortOrder) ? "First_Name" : "";

            var students = from s in db.requests 
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


            var requests = db.requests.Include(r => r.functional_title).Include(r => r.staff).Include(r => r.staff1).Include(r => r.unit1);
            requests = requests.Where(r => r.requested_by == (staffid) || r.supervisor_email == (staffid));


            
           
            if (!String.IsNullOrEmpty(searchString))
            {
                requests = requests.Where(s => s.first_name .Equals(searchString)
                                       || s.last_name .Equals(searchString));

            }

            switch (sortOrder)
            {
                case "First_Name":
                    requests = requests.OrderByDescending(s => s.first_name );
                    break;
                case "name_desc":
                    requests = requests.OrderByDescending(s => s.last_name );
                    break;

                default:  // Name ascending 
                    requests = requests.OrderBy(s => s.first_name );
                    break;
            }


          

            int pageSize = 10;
            int pageNumber = (page ?? 1);
          

            return View(requests.ToPagedList(pageNumber, pageSize));
        }

        ///////////////// all requests


        public ActionResult IndexAll(string sortOrder, string currentFilter, string searchString, int? page)
        {

            var result = from p in db.requests
                         join a in db.requests
                         on p.first_name equals a.first_name


                         select new
                         {
                             staffSuperName = p.first_name + " " + p.first_name,


                         };

            ViewBag.newstaffSuperName = result;


            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = String.IsNullOrEmpty(sortOrder) ? "First_Name" : "";

            var students = from s in db.requests
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


            var requests = db.requests.Include(r => r.functional_title).Include(r => r.staff).Include(r => r.staff1).Include(r => r.unit1);
          




            if (!String.IsNullOrEmpty(searchString))
            {
                requests = requests.Where(s => s.first_name.Equals(searchString)
                                       || s.last_name.Equals(searchString));

            }

            switch (sortOrder)
            {
                case "First_Name":
                    requests = requests.OrderByDescending(s => s.first_name);
                    break;
                case "name_desc":
                    requests = requests.OrderByDescending(s => s.last_name);
                    break;

                default:  // Name ascending 
                    requests = requests.OrderBy(s => s.first_name);
                    break;
            }




            int pageSize = 10;
            int pageNumber = (page ?? 1);


            return View(requests.ToPagedList(pageNumber, pageSize));
        }

        //////////////// all requests end

        // GET: requests/Details/5
        public ActionResult Details(int? id)
        {
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            request request = db.requests.Find(id);
            if (request == null)
            {
                return HttpNotFound();
            }
            return View(request);
        }

        // GET: requests/Create
        public ActionResult Create()
        {
            ViewBag.job_title = new SelectList(db.functional_title, "functionaltitleid", "functional_title_description_english");
            //ViewBag.requested_by = new SelectList(db.staffs, "staffid", "staff_id");
            //ViewBag.requested_by = new SelectList(db.staffs, "staffid", "staff_id");
            ViewBag.unit = new SelectList(db.units, "unitid", "unit_description_english");
            ViewBag.supervisor_email = new SelectList(db.staffs.OrderBy(staffs => staffs.staff_email), "staffid", "staff_email");
            //ViewBag.duty_station = new SelectList(db.countries, "countryid", "country_name");
            //ViewBag.requested_by = new SelectList(db.staffs.OrderBy(staffs => staffs.staff_email), "staffid", "staff_email");

            var staffs5 = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
            staffs5 = staffs5.Where(s => s.staff_email.Contains(User.Identity.Name + "@wfp.org"));

            ////ViewBag.staffid = new SelectList(db.staffs, "staffs.First().staffid", staffs.First().staff_email);
            //ViewBag.staffid = staffs5.First().staffid;
            //ViewBag.staffid2 = staffs5.First().staff_first_name;
            ViewBag.FirstName = staffs5.First().staff_first_name;
            ViewBag.LastName = staffs5.First().staff_last_name;
            ViewBag.requested_byName = staffs5.First().staff_first_name + " " + staffs5.First().staff_last_name;
            ViewBag.requested_byID = staffs5.First().staffid ;
            //ViewBag.Unit = staffs.First().unit.unit_description_english;
            //var staffsupervisor = staffs.First().staff_supervisorid;
            //var userUnit = staffs.First().unit_id;
            ViewBag.userID = new SelectList(User.Identity.Name);
             return View();
        }

        // POST: requests/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "request_no,first_name,last_name,job_title,unit,budget_code,index_number,duty_station,supervisor_email,appointment_type,start_date,end_date,computerLaptop,computerDeskyop,email,access_P,telephone,pincode_ext,local_sim,international,roaming,SmartPhone,BasicPhone,usb_modem,color_printer,IphoneService,IphoneService,mobile_phone,other,location,requested_by")] request request)
        {
            if(request .computerDeskyop  == false)

            { request.computerLaptop = true; }
            


           

                db.requests.Add(request);
           
                try
                {
                    ///////////////////// staff
                    var addunit = db.staffs.Add(new staff { staff_id = request.index_number, staff_first_name = request.first_name, staff_last_name = request.last_name, unit_id = request.unit, staff_supervisorid = request.supervisor_email, functional_title_id = request.job_title , staff_email = request .first_name + "." + request .last_name + "@wfp.org"}).staff_index_number;
                    db.SaveChanges();
                    /////////////////////// staff
                    db.SaveChanges();

                    var email = "";
                    var PDrive = "";
                    var telephoneext = "";
                    var pincode = "";
                    var localsim = "";
                    var international = "";
                    var roaming = "";
                    var USBMODEM = "";
                    var color_printer = "";

                    if (request.email == true)
                    {
                         email = "Yes";
                    }
                    else {
                         email = "No";
                    }

                    if (request.access_P  == true)
                    {
                        PDrive = "Yes";
                    }
                    else
                    {
                        PDrive = "No";
                    }

                    if (request.telephone  == true)
                    {
                        telephoneext = "Yes";
                    }
                    else
                    {
                        telephoneext = "No";
                    }

                    if (request.pincode_ext  == true)
                    {
                        pincode = "Yes";
                    }
                    else
                    {
                        pincode = "No";
                    }

                    if (request.local_sim  == true)
                    {
                        localsim = "Yes";
                    }
                    else
                    {
                        localsim = "No";
                    }

                    if (request.international == true)
                    {
                        international = "Yes";
                    }
                    else
                    {
                        international = "No";
                    }

                    if (request.roaming == true)
                    {
                        roaming = "Yes";
                    }
                    else
                    {
                        roaming = "No";
                    }

                    if (request.usb_modem  == true)
                    {
                        USBMODEM = "Yes";
                    }
                    else
                    {
                        USBMODEM = "No";
                    }
                    if (request.color_printer == true)
                    {
                        color_printer = "Yes";
                    }
                    else
                    {
                        color_printer = "No";
                    }

                    var staffs = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                    staffs = staffs.Where(s => s.staffid == request .requested_by );
                    var requestedby = staffs.First().staff_first_name + " " + staffs.First().staff_last_name;
                    var requestedbyemail = staffs.First().staff_email;

                    var title = db.functional_title.Include(s => s.staffs );
                    title = title.Where(t => t.functionaltitleid  == request .job_title);
                    var jobtitle = title.First().functional_title_description_english;

                    var unitselect = db.units.Include(s => s.staffs);
                    unitselect = unitselect.Where(u => u.unitid == request.unit);
                    var unit = unitselect.First().unit_description_english ;

                    var Superemail = db.staffs .Include(s => s.district );
                    Superemail = Superemail.Where(Su => Su.staffid == request.supervisor_email );
                    var supervisor_email = Superemail.First().staff_email ;

                   

                    MailMessage mail = new MailMessage();
                    mail.To.Add("cairo.itservicedesk@wfp.org");
                    mail.CC.Add(supervisor_email);
                    mail.CC.Add("cairo.itservicedesk@wfp.org");
                           
                    mail.CC.Add(requestedbyemail);
                    //mail.CC.Add("mahmoud.abdelfattah@wfp.org");
                    //mail.CC.Add("amal.mohamed@wfp.org");
                    mail.Bcc.Add("ahmed.badr@wfp.org");
                    mail.From = new MailAddress(requestedbyemail);
                    mail.Subject = "IT Services Request for :" +  request.first_name + " " + request.last_name;

                    string Body = "<table border='0'><tr><td colspan='2'><hr/>Request Information<hr/></td><hr/></tr><tr><td><font color='blue'>Request Number: </font></td><td>" + request.request_no + "</td></tr>"
                     + "<tr><td><font color='blue'>" + " Requested By :</font></td><td>" + requestedby + "</td></tr>"
                     + "<tr><td colspan='2'><hr/>PERSONAL INFORMATION<hr/></td></tr>"
                     + "<tr><td><font color='blue'>" + "New Staff :</font></td><td>" + request.first_name
                     + " " + request.last_name + "</td></tr>"
                     + "<tr><td><font color='blue'> JOB TITLE :</font></td><td>" + jobtitle + "</td></tr>"
                     + "<tr><td><font color='blue'>UNIT :</font></td><td>" + unit + "</td></tr>"
                     + "<tr><td><font color='blue'>BUDGET CODE :</font></td><td>" + request.budget_code + "</td></tr>"
                     + "<tr><td><font color='blue'>INDEX NUMBER :</font></td><td>" + request.index_number + "</td></tr>"
                     + "<tr><td><font color='blue'>DUTY STATION :</font></td><td>" + request.duty_station + "</td></tr>"
                     + "<tr><td><font color='blue'>SUPERVISOR EMAIL :</font></td><td>" + supervisor_email + "</td></tr>"
                     + "<tr><td colspan='2'><hr/>Contract Information<hr/></td></tr><tr><td><font color='blue'>Appoinment Type :</font></td><td>" + request.appointment_type + "</td></tr>"
                     + "<tr><td><font color='blue'>START DATE :</font></td><td>" + request.start_date.Value.ToString("dd/MMM/yy") + "</td></tr>"
                     + "<tr><td><font color='blue'>END DATE :</font></td><td>" + request.end_date.Value.ToString("dd/MMM/yy") + "</td></tr>"
                     + "<tr><td colspan='2'><hr/>SERVICES AND EQUIPMENT<hr/></td></tr>" + "<tr><td><font color='blue'>COMPUTER TYPE :</font></td><td>" + request.IphoneService + "</td></tr>"
                     + "<tr><td><font color='blue'>E-MAIL :</font></td><td>" + email + "</td></tr>"
                     + "<tr><td><font color='blue'>ACCESS P DRIVE :</font></td><td>" + PDrive + "</td></tr>"
                     + "<tr><td><font color='blue'>TELEPHONE EXT :</font></td><td>" + telephoneext + "</td></tr>"
                     + "<tr><td><font color='blue'>PINCODE FOR EXTERNAL CALLS :</font></td><td>" + pincode + "</td></tr>"
                     + "<tr><td><font color='blue'>LOCAL MOBILE SIM :</font></td><td>" + localsim + "</td></tr>"
                     + "<tr><td><font color='blue'>INTERNATIONAL :</font></td><td>" + international + "</td></tr>"
                     + "<tr><td><font color='blue'>ROAMING :</font></td><td>" + roaming + "</td></tr>"
                     + "<tr><td><font color='blue'>USB Modem :</font></td><td>" + USBMODEM + "</td></tr>"
                     + "<tr><td><font color='blue'>PHONES :</font></td><td>" + request.mobile_phone + "</td></tr>"
                     + "<tr><td><font color='blue'>COLOR_PRINTER :</font></td><td>" + color_printer + "</td></tr>"
                     + "<tr><td><font color='blue'>OTHER REQUIREMENTS :</font></td><td>" + request.other + "</td></tr>"
                     + "<tr><td><font color='blue'>SITTING LOCATION :</font></td><td>" + request.location + "</td></tr></table>";

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

             //MailMessage mail = new MailMessage();
             //mail.To.Add("ahmed.badr@wfp.org");
             //   //mail.CC.Add(cc);
             //   //mail.Bcc.Add("ahmed.badr@wfp.org");
             //   mail.From = new MailAddress("RBC_SMS@wfp.org");
             //   mail.Subject = "IT Services Request for New staff";

             //   string Body = "<br>New Request<br><br> PERSONAL INFORMATION <br><br>  Request Number : " + request.request_no + " <br> " + " From &nbsp; &nbsp; &nbsp;:" + request.requested_by + "<br><br>" + "New Staff :" + request.first_name + " " + request.last_name + "<br> JOB TITLE :" + request.job_title + "<br>UNIT :" + request.unit +
             //           "<br>BUDGET CODE :" + request.budget_code + "<br>INDEX NUMBER :" + request.index_number + "<br> DUTY STATION :" + request.duty_station + "<br> SUPERVISOR EMAIL :" + request.supervisor_email + "<br><br>Contract Information <br><br> Appoinment Type :" + request.appointment_type + "<br> START DATE :" + request.start_date + "<br> END DATE :" + request.end_date + " <br><br>SERVICES AND EQUIPMENT<br>" + "<br>COMPUTER TYPE :" + request.IphoneService + " <br> E-MAIL :" + email + " <br> ACCESS P DRIVE :" + request.access_P + " <br> TELEPHONE EXT :" + request.telephone + "<br> PINCODE FOR EXTERNAL CALLS :" + request.pincode_ext + "<br> LOCAL MOBILE SIM :" + request.local_sim + "<br> INTERNATIONAL :" + request.international + "<br> ROAMING :" + request.roaming + "<br> PHONES :" + request.mobile_phone + "<br> COLOR_PRINTER :" + request.color_printer + "<br> OTHER REQUIREMENTS :" + request.other + "<br> SITTING LOCATION :" + request.location + "<br> REQUESTED BY :" + request.requested_by;

             //   //string Body = "Request : Mission &nbsp;  &nbsp;  &nbsp; &nbsp;  &nbsp; &nbsp; &nbsp; &nbsp;  Mission ID : " + missionAuthorization.MissionID + " <br> " + " From &nbsp; &nbsp; &nbsp;:" + staffs.First().staff_first_name + "&nbsp;" + staffs.First().staff_last_name + "<br><br>" + " Mission Information" + "<br><br>" + " From :" + missionAuthorization.FromDate + "<br>To :" + missionAuthorization.ToDate +
             //   //    "<br>Funding :" + missionAuthorization.funding + "<br>Objective :" + missionAuthorization.MissionObjective + "<br> Mission Itinerary :" + missionAuthorization.MissionItinerary + "<br> Can This meeting Be Done Over Voice/Video ? :" + missionAuthorization.canbedone  + " <br><br> <a href='http://10.11.135.254:8080/WFPSMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor=" + true + "&FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + " &to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Acept</a>&nbsp;&nbsp;&nbsp;<a href='http://10.11.135.254:8080/WFPSMS/MissionAuthorizations/Approval?Mid=" + missionAuthorization.MissionID + "&ClearedBySupervisor= " + false + "& FromDate=" + missionAuthorization.FromDate + " &ToDate=" + missionAuthorization.ToDate + "&to=" + to + "&staffmail=" + staffs.First().staff_email + "&supervisorname=" + supername + "'>Reject</a> ";

             //   mail.Body = Body;
             //   mail.IsBodyHtml = true;
             //   SmtpClient smtp = new SmtpClient();
             //   smtp.Host = "smtprelay.global.wfp.org";
             //   smtp.Port = 25;
             //   smtp.UseDefaultCredentials = true;
             //   //smtp.Credentials = new System.Net.NetworkCredential
             //   //("ahmed.badr", "Survivor2323");// Enter seders User name and password  
             //   //smtp.EnableSsl = true;
             //   smtp.Send(mail);
               
             //   return RedirectToAction("Index");

           

            ViewBag.job_title = new SelectList(db.functional_title, "functionaltitleid", "functional_title_description_english", request.job_title);
            //ViewBag.requested_by = new SelectList(db.staffs, "staffid", "staff_id", request.requested_by);
            //ViewBag.requested_by = new SelectList(db.staffs, "staffid", "staff_id", request.requested_by);
            ViewBag.unit = new SelectList(db.units, "unitid", "unit_description_english", request.unit);
            ViewBag.supervisor_email = new SelectList(db.staffs.OrderBy(staffs => staffs.staff_email), "staffid", "staff_email",request .supervisor_email );
            return View(request);
        }

        // GET: requests/Edit/5
        public ActionResult Edit(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            request request = db.requests.Find(id);
            if (request == null)
            {
                return HttpNotFound();
            }

            ViewBag.job_title = new SelectList(db.functional_title, "functionaltitleid", "functional_title_description_english", request.job_title);
            //ViewBag.requested_by = new SelectList(db.staffs, "staffid", "staff_id");
            //ViewBag.requested_by = new SelectList(db.staffs, "staffid", "staff_id");
            ViewBag.unit = new SelectList(db.units, "unitid", "unit_description_english",request.unit);
            ViewBag.supervisor_email = new SelectList(db.staffs.OrderBy(staffs => staffs.staff_email), "staffid", "staff_email");
            //ViewBag.duty_station = new SelectList(db.countries, "countryid", "country_name");
            //ViewBag.requested_by = new SelectList(db.staffs.OrderBy(staffs => staffs.staff_email), "staffid", "staff_email");

            var staffs5 = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
            staffs5 = staffs5.Where(s => s.staffid == (request .requested_by ));

            ////ViewBag.staffid = new SelectList(db.staffs, "staffs.First().staffid", staffs.First().staff_email);
            //ViewBag.staffid = staffs5.First().staffid;
            //ViewBag.staffid2 = staffs5.First().staff_first_name;
            ViewBag.FirstName = staffs5.First().staff_first_name;
            ViewBag.LastName = staffs5.First().staff_last_name;
            ViewBag.requested_byName = staffs5.First().staff_first_name + " " + staffs5.First().staff_last_name;
            ViewBag.requested_byID = staffs5.First().staffid;
            //ViewBag.Unit = staffs.First().unit.unit_description_english;
            //var staffsupervisor = staffs.First().staff_supervisorid;
            //var userUnit = staffs.First().unit_id;
            ViewBag.userID = new SelectList(User.Identity.Name);

            ViewBag.supervisor_email = new SelectList(db.staffs.OrderBy(staffs => staffs.staff_email), "staffid", "staff_email", request.supervisor_email);
            //ViewBag.job_title = new SelectList(db.functional_title, "functionaltitleid", "functional_title_id", request.job_title);
            //ViewBag.requested_by = new SelectList(db.staffs, "staffid", "staff_id", request.requested_by);
            //ViewBag.requested_by = new SelectList(db.staffs, "staffid", "staff_id", request.requested_by);
            //ViewBag.unit = new SelectList(db.units, "unitid", "unit_id", request.unit);
            return View(request);
        }

        // POST: requests/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "request_no,first_name,last_name,job_title,unit,budget_code,index_number,duty_station,supervisor_email,appointment_type,start_date,end_date,computerLaptop,computerDeskyop,IphoneService,email,access_P,telephone,pincode_ext,local_sim,international,roaming,SmartPhone,BasicPhone,usb_modem,color_printer,BlackberryService,IphoneService,mobile_phone,other,location,requested_by")] request request)
        {

            if (request.computerDeskyop == false)

            { request.computerLaptop = true; }

            if (ModelState.IsValid)
           {
            try
            {
                db.Entry(request).State = EntityState.Modified;
                db.SaveChanges();

                //////////////////// update email
                var email = "";
                var PDrive = "";
                var telephoneext = "";
                var pincode = "";
                var localsim = "";
                var international = "";
                var roaming = "";
                var USBMODEM = "";
                var color_printer = "";

                if (request.email == true)
                {
                    email = "Yes";
                }
                else
                {
                    email = "No";
                }

                if (request.access_P == true)
                {
                    PDrive = "Yes";
                }
                else
                {
                    PDrive = "No";
                }

                if (request.telephone == true)
                {
                    telephoneext = "Yes";
                }
                else
                {
                    telephoneext = "No";
                }

                if (request.pincode_ext == true)
                {
                    pincode = "Yes";
                }
                else
                {
                    pincode = "No";
                }

                if (request.local_sim == true)
                {
                    localsim = "Yes";
                }
                else
                {
                    localsim = "No";
                }

                if (request.international == true)
                {
                    international = "Yes";
                }
                else
                {
                    international = "No";
                }

                if (request.roaming == true)
                {
                    roaming = "Yes";
                }
                else
                {
                    roaming = "No";
                }

                if (request.usb_modem == true)
                {
                    USBMODEM = "Yes";
                }
                else
                {
                    USBMODEM = "No";
                }
                if (request.color_printer == true)
                {
                    color_printer = "Yes";
                }
                else
                {
                    color_printer = "No";
                }

                var staffs = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                staffs = staffs.Where(s => s.staffid == request.requested_by);
                var requestedby = staffs.First().staff_first_name + " " + staffs.First().staff_last_name;
                var requestedbyemail = staffs.First().staff_email;

                var title = db.functional_title.Include(s => s.staffs);
                title = title.Where(t => t.functionaltitleid == request.job_title);
                var jobtitle = title.First().functional_title_description_english;

                var unitselect = db.units.Include(s => s.staffs);
                unitselect = unitselect.Where(u => u.unitid == request.unit);
                var unit = unitselect.First().unit_description_english;

                var Superemail = db.staffs.Include(s => s.district);
                Superemail = Superemail.Where(Su => Su.staffid == request.supervisor_email);
                var supervisor_email = Superemail.First().staff_email;



                MailMessage mail = new MailMessage();
                mail.To.Add("cairo.itservicedesk@wfp.org");
                mail.CC.Add(supervisor_email);
                mail.CC.Add(requestedbyemail);
                mail.CC.Add("cairo.itservicedesk@wfp.org");
                //mail.CC.Add("amal.mohamed@wfp.org");
                //mail.CC.Add(supervisor_email);
                mail.Bcc.Add("ahmed.badr@wfp.org");
                mail.From = new MailAddress(requestedbyemail);
                mail.Subject = "IT Services New Edit for :" + request.first_name + " " + request.last_name;

                string Body = "<table border='0'><tr><td colspan='2'><hr/>Request Information<hr/></td><hr/></tr><tr><td><font color='blue'>Request Number: </font></td><td>" + request.request_no + "</td></tr>"
                     + "<tr><td><font color='blue'>" + " Requested By :</font></td><td>" + requestedby + "</td></tr>"
                     + "<tr><td colspan='2'><hr/>PERSONAL INFORMATION<hr/></td></tr>"
                     + "<tr><td><font color='blue'>" + "New Staff :</font></td><td>" + request.first_name
                     + " " + request.last_name + "</td></tr>"
                     + "<tr><td><font color='blue'> JOB TITLE :</font></td><td>" + jobtitle + "</td></tr>"
                     + "<tr><td><font color='blue'>UNIT :</font></td><td>" + unit + "</td></tr>"
                     + "<tr><td><font color='blue'>BUDGET CODE :</font></td><td>" + request.budget_code + "</td></tr>"
                     + "<tr><td><font color='blue'>INDEX NUMBER :</font></td><td>" + request.index_number + "</td></tr>"
                     + "<tr><td><font color='blue'>DUTY STATION :</font></td><td>" + request.duty_station + "</td></tr>"
                     + "<tr><td><font color='blue'>SUPERVISOR EMAIL :</font></td><td>" + supervisor_email + "</td></tr>"
                     + "<tr><td colspan='2'><hr/>Contract Information<hr/></td></tr><tr><td><font color='blue'>Appoinment Type :</font></td><td>" + request.appointment_type + "</td></tr>"
                     + "<tr><td><font color='blue'>START DATE :</font></td><td>" + request.start_date.Value.ToString("dd/MMM/yy") + "</td></tr>"
                     + "<tr><td><font color='blue'>END DATE :</font></td><td>" + request.end_date.Value.ToString("dd/MMM/yy") + "</td></tr>"
                     + "<tr><td colspan='2'><hr/>SERVICES AND EQUIPMENT<hr/></td></tr>" + "<tr><td><font color='blue'>COMPUTER TYPE :</font></td><td>" + request.IphoneService + "</td></tr>"
                     + "<tr><td><font color='blue'>E-MAIL :</font></td><td>" + email + "</td></tr>"
                     + "<tr><td><font color='blue'>ACCESS P DRIVE :</font></td><td>" + PDrive + "</td></tr>"
                     + "<tr><td><font color='blue'>TELEPHONE EXT :</font></td><td>" + telephoneext + "</td></tr>"
                     + "<tr><td><font color='blue'>PINCODE FOR EXTERNAL CALLS :</font></td><td>" + pincode + "</td></tr>"
                     + "<tr><td><font color='blue'>LOCAL MOBILE SIM :</font></td><td>" + localsim + "</td></tr>"
                     + "<tr><td><font color='blue'>INTERNATIONAL :</font></td><td>" + international + "</td></tr>"
                     + "<tr><td><font color='blue'>ROAMING :</font></td><td>" + roaming + "</td></tr>"
                     + "<tr><td><font color='blue'>USB Modem :</font></td><td>" + USBMODEM + "</td></tr>"
                     + "<tr><td><font color='blue'>PHONES :</font></td><td>" + request.mobile_phone + "</td></tr>"
                     + "<tr><td><font color='blue'>COLOR_PRINTER :</font></td><td>" + color_printer + "</td></tr>"
                     + "<tr><td><font color='blue'>OTHER REQUIREMENTS :</font></td><td>" + request.other + "</td></tr>"
                     + "<tr><td><font color='blue'>SITTING LOCATION :</font></td><td>" + request.location + "</td></tr></table>";

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
                /////////////////// update email end
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

            }

            ViewBag.job_title = new SelectList(db.functional_title, "functionaltitleid", "functional_title_id", request.job_title);
            ViewBag.requested_by = new SelectList(db.staffs, "staffid", "staff_id", request.requested_by);
            ViewBag.requested_by = new SelectList(db.staffs, "staffid", "staff_id", request.requested_by);
            ViewBag.unit = new SelectList(db.units, "unitid", "unit_id", request.unit);
            ViewBag.supervisor_email = new SelectList(db.staffs.OrderBy(staffs => staffs.staff_email), "staffid", "staff_email", request.supervisor_email);
            return View(request);
        }

        // GET: requests/Delete/5
        public ActionResult Delete(int? id)
        {
           

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            request request = db.requests.Find(id);
            if (request == null)
            {
                return HttpNotFound();
            }
           
            return View(request);
        }

        // POST: requests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            request request = db.requests.Find(id);
            db.requests.Remove(request);
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


        ////////////////////////////////////////////

        //[HttpPost]
        //public ActionResult Evaluate(request  req, RuleModel ruleEditor)
        //{
        //    // Tell Code Effects which type to use as its source object
        //    ruleEditor.BindSource(typeof(request));

        //    // We are not saving the rule, just evaluating it. Tell the model not to enforce the rule name validation
        //    ruleEditor.SkipNameValidation = true;
        //    ViewBag.Rule = ruleEditor;

        //    if (ruleEditor.IsEmpty() || !ruleEditor.IsValid(StorageService.LoadRuleXml))
        //    {
        //        ViewBag.Message = "The rule is empty or invalid";
        //        return View("Index", req);
        //    }

          
          
        //}


        /////////////////////////////////////////////
    }
}
