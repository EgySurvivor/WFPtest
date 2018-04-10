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

    public class MissionAuthorizationsController : Controller
    {
        private WFPEntities1 db = new WFPEntities1();

        ///////////////////////////////////////////////////////////
        ///////////////////////////////AD sync /////////////////////

        ArrayList GetADGroupUsersADD(string groupName)
        {
            ArrayList myItems = new ArrayList();
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain, "global");
            GroupPrincipal group = GroupPrincipal.FindByIdentity(ctx, "GG-EGRB Members");
            PrincipalSearchResult<Principal> members = group.GetMembers();

            foreach (Principal user in members)
            {
                myItems.Add(user.UserPrincipalName);
                //myItems.Add(user.SamAccountName );
                //myItems.Add(user.UserPrincipalName); //E-mail
                //myItems.Add(user.Name); //Full Name (Second//First)
                //myItems.Add(user.DisplayName);  //Full Name (Second//First)
                //myItems.Add(user.Description); //empty
                //myItems.Add(user.ContextType ); //Domain

            }
            return myItems;

            //SearchResult result;
            //DirectorySearcher search = new DirectorySearcher();
            //search.Filter = String.Format("(cn={0})", groupName);
            //search.PropertiesToLoad.Add("member");
            //search.PropertiesToLoad.Add("mail");
            //result = search.FindOne();
            //ArrayList usersinfo = new ArrayList();
            //usersinfo  = members;
            //return members;
        }
        //////////////////////////////////////// AD Sync //////////////////////////////////////////////////////////////

        //ArrayList GetADGroupUsers(string groupName)
        //{

        //    var myItems = new ArrayList();

        //    var ctx = new PrincipalContext(ContextType.Domain, "global", "EG");

        //    // define a "query-by-example" principal - here, we search for a GroupPrincipal 
        //    var qbeGroup = new GroupPrincipal(ctx);

        //    // create your principal searcher passing in the QBE principal    
        //    var srch = new PrincipalSearcher(qbeGroup);

        //    // find all matches
        //    foreach (Principal found in srch.FindAll())
        //    {
        //        var foundGroup = found as GroupPrincipal;

        //        if (foundGroup != null)
        //        {
        //            myItems.Add(foundGroup.Name);
        //        }
        //    }
        //    return myItems;
        //}

        ////////////////////////////////////////////////// AD Sync call /////////////////////////////////////////////////////////////////////////////
        public void GetADGroupUsersADDCall(string groupName)
        {
            ArrayList newuser = GetADGroupUsersADD("GG-EGRB Members");
            //ViewBag.ADusers = GetADGroupUsers("GG-EGRB ICT").IndexOf ("user") ;
            foreach (object el in newuser)
            {
                string ff = newuser.Capacity.ToString();
                string username = el.ToString();
                //string start = "=";
                //string end = ",";
                //int startIndex = username.IndexOf("=");
                //int endIndex = username.LastIndexOf(",");
                //ViewBag.ADusers = username.Substring(username .IndexOf ("="), username .IndexOf (","));

                //ViewBag.ADusers = (el);

                char[] commaSeparator = new char[] { ',' };
                string[] authors = username.Split(commaSeparator, StringSplitOptions.None);
                foreach (string author in authors)
                {
                    var staffemailcom = author;
                    var staffscomp = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                    staffscomp = staffscomp.Where(s => s.staff_email == staffemailcom);
                    //var newstaff = staffscomp.First().staff_email.DefaultIfEmpty();
                    if (staffscomp.Count() == 0)
                    {
                        //staff staffemailnew = null ;
                        //staffemailnew.staff_email = author;
                        //db.staffs.Add(staffemailnew) ;
                        var staffemailnew = db.staffs.Add(
                         new staff { staff_email = author }).staff_email;
                        db.SaveChanges();

                    }

                    //ViewBag.ADusers += author + "      ";

                }

            }
            ////////////////////////////////////////////////////////// AD Sync ////////////////////////////////////////////////////////////////////////         


            //staff NewuUser = new staff();
            //NewuUser.staff_first_name = ADStaff.;
            //NewuUser.Stone = Model.Stone;
            //NewuUser.Pound = Model.Pound;
            //NewuUser.Date = System.DateTime.Now;

            //db.Weights.Add(Model);
            //db.SaveChanges();

        }


        ///////////////////////////////AD sync call end /////////////////////
        ////////////////////////////Links/////////////////////////
        //////////////////////////////////

        public void GetADGroupUsers(string groupName)
        {

            int monht = DateTime.Now.Month;
            int year = DateTime.Now.Year;
            int day = DateTime.DaysInMonth(year, monht);
            RecurringJob.AddOrUpdate(() => GetADGroupUsersM("ss"), Cron.Monthly(day, 14));

            DateTime date1 = DateTime.Now.Subtract(new TimeSpan(8, 0, 0, 0));
            RecurringJob.AddOrUpdate(() => leaveMonthly("ss"), Cron.Monthly(day, 14));
            RecurringJob.AddOrUpdate(() => leaveMonthly("sjjjs"), Cron.Monthly(day, 14));

            var WeeklyMission = db.MissionAuthorizations.Include(s => s.staff);
            WeeklyMission = WeeklyMission.Where(s => s.SignatureDate >= date1 && s.SignatureDate <= DateTime.Now || s.FromDate >= date1 && s.FromDate <= DateTime.Now).OrderBy(s => s.staff.unit.unit_description_english);








            //DateTime MDate = WeeklyMission.First().SignatureDate.Value;
            //DateTime NDate = DateTime.Now;
            //var weeks = (NDate.Subtract(MDate)).TotalDays / 7;
            //if (weeks == 0)
            //{


            MailMessage mail = new MailMessage();
            mail.To.Add("rbc.travel@wfp.org"); //rbc.travel@wfp.org
            mail.CC.Add("mai.kenawi@wfp.org"); //mai.kenawi@wfp.org
            mail.CC.Add("rbc.management@wfp.org"); //mai.kenawi@wfp.org 
            mail.CC.Add("lina.youssef@wfp.org");
            mail.CC.Add("essraa.soliman@wfp.org");
            mail.CC.Add("youssef.yassin@wfp.org");
            mail.Bcc.Add("ahmed.badr@wfp.org");
            mail.From = new MailAddress("RBC-no-reply@wfp.org");
            mail.Subject = "Mission Authorization Weekly Report";

            string Body = "<table cellpadding='10'><tr><td align='center' style='border:1px solid red;'><font color='blue'>Index No.</font></td>"
                        + "<td align='center' style='border:1px solid red;'><font color='blue'>Staff/On Behalf</font></td>"
                        + "<td align='center' style='border:1px solid red;'><font color='blue'>Unit</font></td>"
                        + "<td align='center' style='border:1px solid red;'><font color='blue'>From</font></td>"
                        + "<td align='center' style='border:1px solid red;'><font color='blue'>To</font></td>"
                        + "<td align='center' style='border:1px solid red;'><font color='blue'>Number of Dayes</font></td>"
                        + "<td align='center' style='border:1px solid red;'><font color='blue'>Issued Date</font></td>"
                        + "<td align='center' style='border:1px solid red;'><font color='blue'>Funding</font></td>"
                        + "<td align='center' style='border:1px solid red;'><font color='blue'>Objective</font></td>"
                        + "<td align='center' style='border:1px solid red;'><font color='blue'>Approved</font></td>"
                        + "<td align='center' style='border:1px solid red;'><font color='blue'>Itinerary</font></td></tr>";


            foreach (var item in WeeklyMission)
            {

                var staffser = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                staffser = staffser.Where(s => s.staffid == item.staffid);
                var staffemailforIndex = staffser.First().staff_email;

                var leave = db.EMPLOYEES.Include(l => l.staff);
                leave = leave.Where(l => l.EMAIL_ADDRESS == staffemailforIndex);
                var empindex = "";
                if (leave.Any())
                {
                    empindex = leave.First().EMP_ID;
                }
                else
                {
                    var staffserindex = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                    staffserindex = staffserindex.Where(s => s.staff_email == staffemailforIndex);
                    if (staffserindex.Any())
                    {
                        empindex = staffserindex.First().staff_id;
                    }
                    else
                    {
                        empindex = " ";
                    }

                }

                var onbehalfFullName = "";
                if (item.staffonbehalf != null)
                {

                    var staffseronbehalf = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                    staffseronbehalf = staffseronbehalf.Where(s => s.staffid == item.staffonbehalf);
                    onbehalfFullName = staffseronbehalf.First().staff_first_name + " " + staffseronbehalf.First().staff_last_name;

                }


                var canbedoneee = "";
                if (item.canbedone == true)
                {
                    canbedoneee = "Yes";
                }
                else
                {
                    canbedoneee = "No";
                }

                var CleredBySupe2 = "";
                if (item.ClearedBySupervisor == true)
                {
                    CleredBySupe2 = "Yes";
                }
                else
                {
                    CleredBySupe2 = "No";
                }

                var FirstDate = item.FromDate;
                var SecondDate = item.ToDate;
                var dayesNumber = (SecondDate.Value - FirstDate.Value).TotalDays;


                Body = Body + "<tr><td align='center' style='border:1px solid blue;'>" + empindex + "</td>"
                        + "<td align='center' style='border:1px solid blue;'>" + staffser.First().staff_first_name + " " + staffser.First().staff_last_name + " / " + onbehalfFullName + "</td>"
                        + "<td align='center' style='border:1px solid blue;'>" + item.staff.unit.unit_description_english + "</td>"
                        + "<td align='center' style='border:1px solid blue;'>" + item.FromDate.Value.ToString("dd/MMM/yy") + "</td>"
                        + "<td align='center' style='border:1px solid blue;'>" + item.ToDate.Value.ToString("dd/MMM/yy") + "</td>"
                        + "<td align='center' style='border:1px solid blue;'>" + dayesNumber + "</td>"
                        + "<td align='center' style='border:1px solid blue;'>" + item.SignatureDate.Value.ToString("dd/MMM/yy") + "</td>"
                        + "<td align='center' style='border:1px solid blue;'>" + item.funding + "</td>"
                        + "<td align='center' style='border:1px solid blue;'>" + item.MissionObjective + "</td>"
                        + "<td align='center' style='border:1px solid blue;'>" + item.RDORDRDSignature + "</td>"
                        + "<td align='center' style='border:1px solid blue;'>" + item.MissionItinerary + "</td></tr>";


            }

            mail.Body = Body + "</table> <br><font size='2' color='blue'>RBC SMS</font><b> This is an automatically generated email, please do not reply.</b>";

            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtprelay.global.wfp.org";
            smtp.Port = 25;
            smtp.UseDefaultCredentials = true;
            smtp.Send(mail);

            //}//end if

        }

        /// <summary>
        /// //// report
        /// </summary>
        /// // weekly report leave
        public void LeaveWeeklyReport(string groupName)
        {

            int monht = DateTime.Now.Month;
            int year = DateTime.Now.Year;
            int day = DateTime.DaysInMonth(year, monht);
            RecurringJob.AddOrUpdate(() => leaveMonthly("ss"), Cron.Monthly(day, 14));

            DateTime date1 = DateTime.Now.Subtract(new TimeSpan(8, 0, 0, 0));
            RecurringJob.AddOrUpdate(() => leaveMonthly("ss"), Cron.Monthly(day, 14));
            //RecurringJob.AddOrUpdate(() => leaveMonthly("sjjjs"), Cron.Monthly(day, 14));

            var WeeklyLeave = db.VAC_TRANS.Include(s => s.staff);
            WeeklyLeave = WeeklyLeave.Where(s => s.ISSUE_DATE >= date1 && s.ISSUE_DATE <= DateTime.Now || s.FROM_DATE >= date1 && s.FROM_DATE <= DateTime.Now).OrderBy(s => s.staff.unit.unit_description_english);








            //DateTime MDate = WeeklyMission.First().SignatureDate.Value;
            //DateTime NDate = DateTime.Now;
            //var weeks = (NDate.Subtract(MDate)).TotalDays / 7;
            //if (weeks == 0)
            //{


            MailMessage mail = new MailMessage();
            mail.To.Add("lina.youssef@wfp.org");//lina.youssef@wfp.org
            mail.CC.Add("youssef.yassin@wfp.org");//youssef.yassin@wfp.org
            //mail.CC.Add(cc);
            mail.Bcc.Add("ahmed.badr@wfp.org");
            mail.From = new MailAddress("RBC-no-reply@wfp.org"); //
            mail.Subject = "Leave weekly Report";

            string Body = "<table cellpadding='10'><tr><td align='center' style='border:1px solid red;'><font color='blue'>Index No.</font></td>"
                       + "<td align='center' style='border:1px solid red;'><font color='blue'>Staff Name</font></td>"
                       + "<td align='center' style='border:1px solid red;'><font color='blue'>Unit</font></td>"
                       + "<td align='center' style='border:1px solid red;'><font color='blue'>Leave Type </font></td>"
                       + "<td align='center' style='border:1px solid red;'><font color='blue'>From</font></td>"
                       + "<td align='center' style='border:1px solid red;'><font color='blue'>To</font></td>"
                       + "<td align='center' style='border:1px solid red;'><font color='blue'>Number of Days</font></td>"
                       + "<td align='center' style='border:1px solid red;'><font color='blue'>Issue Date</font></td>"
                       + "<td align='center' style='border:1px solid red;'><font color='blue'>Description</font></td></tr>";




            foreach (var item in WeeklyLeave)
            {

                var leaverr = db.EMPLOYEES.Include(l => l.staff);
                leaverr = leaverr.Where(l => l.EMP_ID == item.EMP_ID);

                var leaveid = item.VACATION_TYPE_ID;
                var empidformail = item.EMP_ID;

                var empidforemail = db.EMPLOYEES.Include(l => l.staff);
                empidforemail = empidforemail.Where(l => l.EMP_ID == empidformail);
                var emailforunit = empidforemail.First().EMAIL_ADDRESS;
                var deptidforemp2 = empidforemail.First().DEPID;

                var unitIDfiremp = db.staffs.Include(l => l.unit);
                unitIDfiremp = unitIDfiremp.Where(l => l.staff_email == emailforunit);
                int? unitIDforUnitname = 1;
                if (unitIDfiremp.Any())
                {
                    unitIDforUnitname = (int?)unitIDfiremp.First().unit_id;
                }
                else
                {
                    unitIDforUnitname = 1;
                }

                var unitnameforemp = db.units.Include(l => l.staffs);
                unitnameforemp = unitnameforemp.Where(l => l.unitid == unitIDforUnitname);
                var unitfullname = "";

                if (unitnameforemp.Any())
                {
                    unitfullname = unitnameforemp.First().unit_description_english;
                }
                else
                {
                    var DepartForEmp = db.DEPARTMENTS.Include(s => s.EMPLOYEES);
                    DepartForEmp = DepartForEmp.Where(s => s.DEP_ID == deptidforemp2);
                    if (DepartForEmp.Any())
                    {
                        unitfullname = DepartForEmp.First().DEP_NAME;
                    }
                    else
                    {
                        unitfullname = " ";
                    }

                }

                var leavetype = db.VACATION_TYPE_LIST.Include(l => l.VAC_TRANS);
                leavetype = leavetype.Where(l => l.VACATION_TYPE_ID == leaveid);

                var leavetypename = leavetype.First().VACATION_TYPE_NAME;



                Body = Body + "<tr><td align='center' style='border:1px dotted blue;'>" + item.EMP_ID + "</td>"
                               + "<td align='center' style='border:1px dotted blue;'>" + leaverr.First().EMP_FIRST_NAME + " " + leaverr.First().EMP_MIDDLE_NAME + " " + leaverr.First().EMP_LAST_NAME + "</td>"
                               + "<td align='center' style='border:1px dotted blue;'>" + unitfullname + "</td>"
                               + "<td align='center' style='border:1px dotted blue;'>" + leavetypename + "</td>"
                               + "<td align='center' style='border:1px dotted blue;'>" + item.FROM_DATE.Value.ToString("dd/MMM/yy") + "</td>"
                               + "<td align='center' style='border:1px dotted blue;'>" + item.TO_DATE.Value.ToString("dd/MMM/yy") + "</td>"
                               + "<td align='center' style='border:1px dotted blue;'>" + item.DAY_COUNT + "</td>"
                               + "<td align='center' style='border:1px dotted blue;'>" + item.ISSUE_DATE.Value.ToString("dd/MMM/yy") + "</td>"
                               + "<td align='center' style='border:1px dotted blue;'>" + item.REASON + "</td></tr>";





            }

            mail.Body = Body + "</table> <br><font size='2' color='blue'>RBC SMS</font><b> This is an automatically generated email, please do not reply.</b>";

            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtprelay.global.wfp.org";
            smtp.Port = 25;
            smtp.UseDefaultCredentials = true;
            smtp.Send(mail);

            //}//end if

        }


        // weekly report leave end

        ////////////////////////////// mission daily report


        public void MissionDailyReport(string groupName)
        {



            var WeeklyMission = db.MissionAuthorizations.Include(s => s.staff).Include(s => s.MissionType);
            WeeklyMission = WeeklyMission.Where(s => s.SignatureDate.Value.Day == DateTime.Now.Day && s.SignatureDate.Value.Month == DateTime.Now.Month && s.SignatureDate.Value.Year == DateTime.Now.Year).OrderBy(s => s.staff.unit.unit_description_english);



            //DateTime MDate = WeeklyMission.First().SignatureDate.Value;
            //DateTime NDate = DateTime.Now;
            //var weeks = (NDate.Subtract(MDate)).TotalDays / 7;
            //if (weeks == 0)
            //{



            MailMessage mail = new MailMessage();
            mail.To.Add("rbc.management@wfp.org"); //rbc.management@wfp.org
            mail.CC.Add("youssef.yassin@wfp.org"); //rbc.management@wfp.org
            //mail.CC.Add(cc);
            mail.Bcc.Add("ahmed.badr@wfp.org");
            mail.From = new MailAddress("RBC-no-reply@wfp.org");
            mail.Subject = "Mission Authorization Daily Report";

            string Body = "<table cellpadding='10'><tr><td align='center' style='border:1px solid red;'><font color='blue'>Index No.</font></td>"
                       + "<td align='center' style='border:1px solid red;'><font color='blue'>Staff/On Behalf</font></td>"
                       + "<td align='center' style='border:1px solid red;'><font color='blue'>Unit</font></td>"
                       + "<td align='center' style='border:1px solid red;'><font color='blue'>From</font></td>"
                       + "<td align='center' style='border:1px solid red;'><font color='blue'>To</font></td>"
                       + "<td align='center' style='border:1px solid red;'><font color='blue'>Number of Dayes</font></td>"
                       + "<td align='center' style='border:1px solid red;'><font color='blue'>Issued Date</font></td>"
                       + "<td align='center' style='border:1px solid red;'><font color='blue'>Funding</font></td>"
                       + "<td align='center' style='border:1px solid red;'><font color='blue'>Objective</font></td>"
                       + "<td align='center' style='border:1px solid red;'><font color='blue'>Approved</font></td>"
                       + "<td align='center' style='border:1px solid red;'><font color='blue'>Itinerary</font></td></tr>";



            foreach (var item in WeeklyMission)
            {




                var staffser = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                staffser = staffser.Where(s => s.staffid == item.staffid);
                var staffemailforIndex = staffser.First().staff_email;


                var leave = db.EMPLOYEES.Include(l => l.staff);
                leave = leave.Where(l => l.EMAIL_ADDRESS == staffemailforIndex);
                var empindex = "";
                if (leave.Any())
                {
                    empindex = leave.First().EMP_ID;
                }
                else
                {
                    var staffserindex = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                    staffserindex = staffserindex.Where(s => s.staff_email == staffemailforIndex);
                    if (staffserindex.Any())
                    {
                        empindex = staffserindex.First().staff_id;
                    }
                    else
                    {
                        empindex = " ";
                    }

                }

                var onbehalfFullName = "";
                if (item.staffonbehalf != null)
                {

                    var staffseronbehalf = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                    staffseronbehalf = staffseronbehalf.Where(s => s.staffid == item.staffonbehalf);
                    onbehalfFullName = staffseronbehalf.First().staff_first_name + " " + staffseronbehalf.First().staff_last_name;

                }

                var canbedoneee = "";
                if (item.canbedone == true)
                {
                    canbedoneee = "Yes";
                }
                else
                {
                    canbedoneee = "No";
                }

                var CleredBySupe2 = "";
                if (item.ClearedBySupervisor == true)
                {
                    CleredBySupe2 = "Yes";
                }
                else
                {
                    CleredBySupe2 = "No";
                }

                var FirstDate = item.FromDate;
                var SecondDate = item.ToDate;
                var dayesNumber = (SecondDate.Value - FirstDate.Value).TotalDays;


                Body = Body + "<tr><td align='center' style='border:1px solid blue;'>" + empindex + "</td>"
                        + "<td align='center' style='border:1px solid blue;'>" + staffser.First().staff_first_name + " " + staffser.First().staff_last_name + " / " + onbehalfFullName + "</td>"
                        + "<td align='center' style='border:1px solid blue;'>" + item.staff.unit.unit_description_english + "</td>"
                        + "<td align='center' style='border:1px solid blue;'>" + item.FromDate.Value.ToString("dd/MMM/yy") + "</td>"
                        + "<td align='center' style='border:1px solid blue;'>" + item.ToDate.Value.ToString("dd/MMM/yy") + "</td>"
                        + "<td align='center' style='border:1px solid blue;'>" + dayesNumber + "</td>"
                        + "<td align='center' style='border:1px solid blue;'>" + item.SignatureDate.Value.ToString("dd/MMM/yy") + "</td>"
                        + "<td align='center' style='border:1px solid blue;'>" + item.funding + "</td>"
                        + "<td align='center' style='border:1px solid blue;'>" + item.MissionObjective + "</td>"
                        + "<td align='center' style='border:1px solid blue;'>" + item.RDORDRDSignature + "</td>"
                        + "<td align='center' style='border:1px solid blue;'>" + item.MissionItinerary + "</td></tr>";




            }

            mail.Body = Body + "</table> <br><font size='2' color='blue'>RBC SMS</font><b> This is an automatically generated email, please do not reply.</b>";

            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtprelay.global.wfp.org";
            smtp.Port = 25;
            smtp.UseDefaultCredentials = true;
            smtp.Send(mail);

            //}//end if

        }

        ////////////////////////////// mission daily report end



        ////////////////////////////// leaves daily report


        public void LeaveDailyReport(string groupName)
        {



            MailMessage mail = new MailMessage();
            mail.To.Add("youssef.yassin@wfp.org");//youssef.yassin@wfp.org
            //mail.CC.Add(cc);
            mail.Bcc.Add("ahmed.badr@wfp.org");
            mail.From = new MailAddress("RBC-no-reply@wfp.org");
            mail.Subject = "Leave Daily Report";

            string Body = "<table cellpadding='10'><tr><td align='center' style='border:1px solid red;'><font color='blue'>Index No.</font></td>"
                       + "<td align='center' style='border:1px solid red;'><font color='blue'>Staff Name</font></td>"
                       + "<td align='center' style='border:1px solid red;'><font color='blue'>Unit</font></td>"
                       + "<td align='center' style='border:1px solid red;'><font color='blue'>Leave Type </font></td>"
                       + "<td align='center' style='border:1px solid red;'><font color='blue'>From</font></td>"
                       + "<td align='center' style='border:1px solid red;'><font color='blue'>To</font></td>"
                       + "<td align='center' style='border:1px solid red;'><font color='blue'>Number of Days</font></td>"
                       + "<td align='center' style='border:1px solid red;'><font color='blue'>Issue Date</font></td>"
                       + "<td align='center' style='border:1px solid red;'><font color='blue'>Description</font></td></tr>";




            //var empid = "";


            var leaves = db.VAC_TRANS.Include(ll => ll.staff);
            leaves = leaves.Where(ll => ll.ISSUE_DATE.Value.Month == DateTime.Now.Month && ll.ISSUE_DATE.Value.Day == DateTime.Now.Day && ll.ISSUE_DATE.Value.Year == DateTime.Now.Year || ll.FROM_DATE.Value.Day == DateTime.Now.Day && ll.FROM_DATE.Value.Month == DateTime.Now.Month && ll.FROM_DATE.Value.Year == DateTime.Now.Year);

            foreach (var itemL in leaves)
            {

                var leaverr = db.EMPLOYEES.Include(l => l.staff);
                leaverr = leaverr.Where(l => l.EMP_ID == itemL.EMP_ID);

                var leaveid = itemL.VACATION_TYPE_ID;
                var empidformail = itemL.EMP_ID;

                var empidforemail = db.EMPLOYEES.Include(l => l.staff);
                empidforemail = empidforemail.Where(l => l.EMP_ID == empidformail);
                var emailforunit = empidforemail.First().EMAIL_ADDRESS;
                var deptidforemp2 = empidforemail.First().DEPID;

                var unitIDfiremp = db.staffs.Include(l => l.unit);
                unitIDfiremp = unitIDfiremp.Where(l => l.staff_email == emailforunit);
                int? unitIDforUnitname = 1;
                if (unitIDfiremp.Any())
                {
                    unitIDforUnitname = (int?)unitIDfiremp.First().unit_id;
                }
                else
                {
                    unitIDforUnitname = 1;
                }

                var unitnameforemp = db.units.Include(l => l.staffs);
                unitnameforemp = unitnameforemp.Where(l => l.unitid == unitIDforUnitname);
                var unitfullname = "";

                if (unitnameforemp.Any())
                {
                    unitfullname = unitnameforemp.First().unit_description_english;
                }
                else
                {

                    var DepartForEmp = db.DEPARTMENTS.Include(s => s.EMPLOYEES);
                    DepartForEmp = DepartForEmp.Where(s => s.DEP_ID == deptidforemp2);
                    if (DepartForEmp.Any())
                    {
                        unitfullname = DepartForEmp.First().DEP_NAME;
                    }
                    else
                    {
                        unitfullname = " ";
                    }
                }

                var leavetype = db.VACATION_TYPE_LIST.Include(l => l.VAC_TRANS);
                leavetype = leavetype.Where(l => l.VACATION_TYPE_ID == leaveid);

                var leavetypename = leavetype.First().VACATION_TYPE_NAME;

                Body = Body + "<tr><td align='center' style='border:1px dotted blue;'>" + itemL.EMP_ID + "</td>"
                            + "<td align='center' style='border:1px dotted blue;'>" + leaverr.First().EMP_FIRST_NAME + " " + leaverr.First().EMP_MIDDLE_NAME + " " + leaverr.First().EMP_LAST_NAME + "</td>"
                            + "<td align='center' style='border:1px dotted blue;'>" + unitfullname + "</td>"
                            + "<td align='center' style='border:1px dotted blue;'>" + leavetypename + "</td>"
                            + "<td align='center' style='border:1px dotted blue;'>" + itemL.FROM_DATE.Value.ToString("dd/MMM/yy") + "</td>"
                            + "<td align='center' style='border:1px dotted blue;'>" + itemL.TO_DATE.Value.ToString("dd/MMM/yy") + "</td>"
                            + "<td align='center' style='border:1px dotted blue;'>" + itemL.DAY_COUNT + "</td>"
                            + "<td align='center' style='border:1px dotted blue;'>" + itemL.ISSUE_DATE.Value.ToString("dd/MMM/yy") + "</td>"
                            + "<td align='center' style='border:1px dotted blue;'>" + itemL.REASON + "</td></tr>";




            }

            //Body = Body + "<tr><td colspan='9' align='center' style='border:1px solid red;'>leave</td></tr>";


            mail.Body = Body + "</table> <br><font size='2' color='blue'>RBC SMS</font><b> This is an automatically generated email, please do not reply.</b>";

            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtprelay.global.wfp.org";
            smtp.Port = 25;
            smtp.UseDefaultCredentials = true;
            smtp.Send(mail);

            //}//end if

        }



        ////////////////////////////// Leaves daily report end

        //[HttpPost]
        //public ActionResult AddProduct(string id)
        //{
        //    return PartialView("ProductPartial", new  { PreviousFieldId = id });
        //}

        ////////////////////////////// leaves daily report2


        public void LeaveDailyReport2(string groupName)
        {



            MailMessage mail = new MailMessage();
            mail.To.Add("youssef.yassin@wfp.org");
            //mail.CC.Add(cc);
            mail.Bcc.Add("ahmed.badr@wfp.org");
            mail.From = new MailAddress("RBC-no-reply@wfp.org");
            mail.Subject = "Leave Daily Report";

            string Body = "<table cellpadding='10'><tr><td align='center' style='border:1px solid red;'><font color='blue'>Index No.</font></td>"
                       + "<td align='center' style='border:1px solid red;'><font color='blue'>Staff Name</font></td>"
                       + "<td align='center' style='border:1px solid red;'><font color='blue'>Unit</font></td>"
                       + "<td align='center' style='border:1px solid red;'><font color='blue'>Leave Type </font></td>"
                       + "<td align='center' style='border:1px solid red;'><font color='blue'>From</font></td>"
                       + "<td align='center' style='border:1px solid red;'><font color='blue'>To</font></td>"
                       + "<td align='center' style='border:1px solid red;'><font color='blue'>Number of Days</font></td>"
                       + "<td align='center' style='border:1px solid red;'><font color='blue'>Issue Date</font></td>"
                       + "<td align='center' style='border:1px solid red;'><font color='blue'>Description</font></td></tr>";




            var empid = "";


            var leaves = db.VAC_TRANS.Include(ll => ll.staff);
            leaves = leaves.Where(ll => ll.ISSUE_DATE.Value.Month == DateTime.Now.Month && ll.ISSUE_DATE.Value.Day == DateTime.Now.Day && ll.ISSUE_DATE.Value.Year == DateTime.Now.Year || ll.FROM_DATE.Value.Day == DateTime.Now.Day && ll.FROM_DATE.Value.Month == DateTime.Now.Month && ll.FROM_DATE.Value.Year == DateTime.Now.Year);

            foreach (var itemL in leaves)
            {

                var leaverr = db.EMPLOYEES.Include(l => l.staff);
                leaverr = leaverr.Where(l => l.EMP_ID == itemL.EMP_ID);

                var leaveid = itemL.VACATION_TYPE_ID;
                var empidformail = itemL.EMP_ID;

                var empidforemail = db.EMPLOYEES.Include(l => l.staff);
                empidforemail = empidforemail.Where(l => l.EMP_ID == empidformail);
                var emailforunit = empidforemail.First().EMAIL_ADDRESS;
                var deptidforemp2 = empidforemail.First().DEPID;

                var unitIDfiremp = db.staffs.Include(l => l.unit);
                unitIDfiremp = unitIDfiremp.Where(l => l.staff_email == emailforunit);
                int? unitIDforUnitname = 1;
                if (unitIDfiremp.Any())
                {
                    unitIDforUnitname = (int?)unitIDfiremp.First().unit_id;
                }
                else
                {
                    unitIDforUnitname = 1;
                }

                var unitnameforemp = db.units.Include(l => l.staffs);
                unitnameforemp = unitnameforemp.Where(l => l.unitid == unitIDforUnitname);
                var unitfullname = "";

                if (unitnameforemp.Any())
                {
                    unitfullname = unitnameforemp.First().unit_description_english;
                }
                else
                {

                    var DepartForEmp = db.DEPARTMENTS.Include(s => s.EMPLOYEES);
                    DepartForEmp = DepartForEmp.Where(s => s.DEP_ID == deptidforemp2);
                    if (DepartForEmp.Any())
                    {
                        unitfullname = DepartForEmp.First().DEP_NAME;
                    }
                    else
                    {
                        unitfullname = " ";
                    }
                }

                var leavetype = db.VACATION_TYPE_LIST.Include(l => l.VAC_TRANS);
                leavetype = leavetype.Where(l => l.VACATION_TYPE_ID == leaveid);

                var leavetypename = leavetype.First().VACATION_TYPE_NAME;

                Body = Body + "<tr><td align='center' style='border:1px dotted blue;'>" + itemL.EMP_ID + "</td>"
                            + "<td align='center' style='border:1px dotted blue;'>" + leaverr.First().EMP_FIRST_NAME + " " + leaverr.First().EMP_MIDDLE_NAME + " " + leaverr.First().EMP_LAST_NAME + "</td>"
                            + "<td align='center' style='border:1px dotted blue;'>" + unitfullname + "</td>"
                            + "<td align='center' style='border:1px dotted blue;'>" + leavetypename + "</td>"
                            + "<td align='center' style='border:1px dotted blue;'>" + itemL.FROM_DATE.Value.ToString("dd/MMM/yy") + "</td>"
                            + "<td align='center' style='border:1px dotted blue;'>" + itemL.TO_DATE.Value.ToString("dd/MMM/yy") + "</td>"
                            + "<td align='center' style='border:1px dotted blue;'>" + itemL.DAY_COUNT + "</td>"
                            + "<td align='center' style='border:1px dotted blue;'>" + itemL.ISSUE_DATE.Value.ToString("dd/MMM/yy") + "</td>"
                            + "<td align='center' style='border:1px dotted blue;'>" + itemL.REASON + "</td></tr>";




            }

            //Body = Body + "<tr><td colspan='9' align='center' style='border:1px solid red;'>leave</td></tr>";


            mail.Body = Body + "</table> <br><font size='2' color='blue'>RBC SMS</font><b> This is an automatically generated email, please do not reply.</b>";

            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtprelay.global.wfp.org";
            smtp.Port = 25;
            smtp.UseDefaultCredentials = true;
            smtp.Send(mail);

            //}//end if

        }



        ////////////////////////////// Leaves daily report2 end

        ////////////////////////////month report/////////////////////////
        //////////////////////////////////

        public void GetADGroupUsersM(string groupName)
        {
            RecurringJob.AddOrUpdate(() => GetADGroupUsers("ss"), Cron.Weekly(DayOfWeek.Thursday, 14));


            var WeeklyMission = db.MissionAuthorizations.Include(s => s.staff);
            WeeklyMission = WeeklyMission.Where(s => s.SignatureDate.Value.Month == DateTime.Now.Month && s.SignatureDate.Value.Year == DateTime.Now.Year || s.FromDate.Value.Month == DateTime.Now.Month && s.SignatureDate.Value.Year == DateTime.Now.Year).OrderBy(s => s.staff.unit.unit_description_english);



            //DateTime MDate = WeeklyMission.First().SignatureDate.Value;
            //DateTime NDate = DateTime.Now;
            //var weeks = (NDate.Subtract(MDate)).TotalDays / 7;
            //if (weeks == 0)
            //{


            MailMessage mail = new MailMessage();
            mail.To.Add("ahmed.badr@wfp.org");
            //mail.CC.Add(cc);
            mail.Bcc.Add("ahmed.badr@wfp.org");
            mail.From = new MailAddress("RBC-no-reply@wfp.org");
            mail.Subject = "Mission Authorization Monthly Report";

            string Body = "<table cellpadding='10'><tr><td align='center' style='border:1px solid red;'><font color='blue'>Index No.</font></td>"
                       + "<td align='center' style='border:1px solid red;'><font color='blue'>Staff</font></td>"
                       + "<td align='center' style='border:1px solid red;'><font color='blue'>Unit</font></td>"
                       + "<td align='center' style='border:1px solid red;'><font color='blue'>From</font></td>"
                       + "<td align='center' style='border:1px solid red;'><font color='blue'>To</font></td>"
                       + "<td align='center' style='border:1px solid red;'><font color='blue'>Number of Dayes</font></td>"
                       + "<td align='center' style='border:1px solid red;'><font color='blue'>Issued Date</font></td>"
                       + "<td align='center' style='border:1px solid red;'><font color='blue'>Funding</font></td>"
                       + "<td align='center' style='border:1px solid red;'><font color='blue'>Objective</font></td>"
                       + "<td align='center' style='border:1px solid red;'><font color='blue'>Approved</font></td>"
                       + "<td align='center' style='border:1px solid red;'><font color='blue'>Itinerary</font></td></tr>";



            foreach (var item in WeeklyMission)
            {




                var staffser = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                staffser = staffser.Where(s => s.staffid == item.staffid);
                var staffemailforIndex = staffser.First().staff_email;


                var leave = db.EMPLOYEES.Include(l => l.staff);
                leave = leave.Where(l => l.EMAIL_ADDRESS == staffemailforIndex);
                var empindex = "";
                if (leave.Any())
                {
                    empindex = leave.First().EMP_ID;
                }
                else
                {
                    var staffserindex = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                    staffserindex = staffserindex.Where(s => s.staff_id != null);
                    if (staffserindex.Any())
                    {
                        empindex = staffserindex.First().staff_id;
                    }
                    else
                    {
                        empindex = " ";
                    }

                }


                var canbedoneee = "";
                if (item.canbedone == true)
                {
                    canbedoneee = "Yes";
                }
                else
                {
                    canbedoneee = "No";
                }

                var CleredBySupe2 = "";
                if (item.ClearedBySupervisor == true)
                {
                    CleredBySupe2 = "Yes";
                }
                else
                {
                    CleredBySupe2 = "No";
                }

                var FirstDate = item.FromDate;
                var SecondDate = item.ToDate;
                var dayesNumber = (SecondDate.Value - FirstDate.Value).TotalDays;


                Body = Body + "<tr><td align='center' style='border:1px solid blue;'>" + empindex + "</td>"
                        + "<td align='center' style='border:1px solid blue;'>" + staffser.First().staff_first_name + " " + staffser.First().staff_last_name + "</td>"
                        + "<td align='center' style='border:1px solid blue;'>" + item.staff.unit.unit_description_english + "</td>"
                        + "<td align='center' style='border:1px solid blue;'>" + item.FromDate.Value.ToString("dd/MMM/yy") + "</td>"
                        + "<td align='center' style='border:1px solid blue;'>" + item.ToDate.Value.ToString("dd/MMM/yy") + "</td>"
                        + "<td align='center' style='border:1px solid blue;'>" + dayesNumber + "</td>"
                        + "<td align='center' style='border:1px solid blue;'>" + item.SignatureDate.Value.ToString("dd/MMM/yy") + "</td>"
                        + "<td align='center' style='border:1px solid blue;'>" + item.funding + "</td>"
                        + "<td align='center' style='border:1px solid blue;'>" + item.MissionObjective + "</td>"
                        + "<td align='center' style='border:1px solid blue;'>" + item.RDORDRDSignature + "</td>"
                        + "<td align='center' style='border:1px solid blue;'>" + item.MissionItinerary + "</td></tr>";




            }

            mail.Body = Body + "</table> <br><font size='2' color='blue'>RBC SMS</font><b> This is an automatically generated email, please do not reply.</b>";

            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtprelay.global.wfp.org";
            smtp.Port = 25;
            smtp.UseDefaultCredentials = true;
            smtp.Send(mail);

            //}//end if

        }

        ////////////////// report mission leave monthly

        public void missionleaveMonthly(string groupName)
        {
            RecurringJob.AddOrUpdate(() => GetADGroupUsers("ss"), Cron.Weekly(DayOfWeek.Thursday, 14));



            var WeeklyMission = db.MissionAuthorizations.Include(s => s.staff);
            WeeklyMission = WeeklyMission.Where(s => s.SignatureDate.Value.Month == DateTime.Now.Month || s.FromDate.Value.Month == DateTime.Now.Month).OrderBy(s => s.staff.unit.unit_description_english);



            //DateTime MDate = WeeklyMission.First().SignatureDate.Value;
            //DateTime NDate = DateTime.Now;
            //var weeks = (NDate.Subtract(MDate)).TotalDays / 7;
            //if (weeks == 0)
            //{


            MailMessage mail = new MailMessage();
            mail.To.Add("omar.nagy@wfp.org");
            mail.CC.Add("ahmed.badr@wfp.org");
            mail.Bcc.Add("ahmed.badr@wfp.org");
            mail.From = new MailAddress("RBC-no-reply@wfp.org");
            mail.Subject = "Mission Authorization and Leave Monthly Report";

            string Body = "<table cellpadding='10'><tr><td align='center' style='border:1px solid red;'><font color='blue'>ID</font></td>"
                       + "<td align='center' style='border:1px solid red;'><font color='blue'>Staff</font></td>"
                       + "<td align='center' style='border:1px solid red;'><font color='blue'>Unit</font></td>"
                       + "<td align='center' style='border:1px solid red;'><font color='blue'>From</font></td>"
                       + "<td align='center' style='border:1px solid red;'><font color='blue'>To</font></td>"
                       + "<td align='center' style='border:1px solid red;'><font color='blue'>Funding</font></td>"
                       + "<td align='center' style='border:1px solid red;'><font color='blue'>Objective</font></td>"
                       + "<td align='center' style='border:1px solid red;'><font color='blue'>Approved</font></td>"
                       + "<td align='center' style='border:1px solid red;'><font color='blue'>Itinerary</font></td>"
                       + "<td align='center' style='border:1px solid red;'><font color='blue'>Mission/Leave</font></td></tr>";


            var empid = "";

            foreach (var item in WeeklyMission)
            {

                var staffser = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                staffser = staffser.Where(s => s.staffid == item.staffid);
                var email = staffser.First().staff_email;

                var leave = db.EMPLOYEES.Include(l => l.staff);
                leave = leave.Where(l => l.EMAIL_ADDRESS == email);
                if (leave.Any())
                {
                    empid = leave.First().EMP_ID;
                }
                else
                {
                    empid = " ";
                }




                var canbedoneee = "";
                if (item.canbedone == true)
                {
                    canbedoneee = "Yes";
                }
                else
                {
                    canbedoneee = "No";
                }

                var CleredBySupe2 = "";
                if (item.ClearedBySupervisor == true)
                {
                    CleredBySupe2 = "Yes";
                }
                else
                {
                    CleredBySupe2 = "No";
                }

                Body = Body + "<tr><td align='center' style='border:1px solid blue;'>" + staffser.First().staff_id + "</td>"
                        + "<td align='center' style='border:1px solid blue;'>" + staffser.First().staff_first_name + " " + staffser.First().staff_last_name + "</td>"
                        + "<td align='center' style='border:1px solid blue;'>" + item.staff.unit.unit_description_english + "</td>"
                        + "<td align='center' style='border:1px solid blue;'>" + item.FromDate.Value.ToString("dd/MMM/yy") + "</td>"
                        + "<td align='center' style='border:1px solid blue;'>" + item.ToDate.Value.ToString("dd/MMM/yy") + "</td>"
                        + "<td align='center' style='border:1px solid blue;'>" + item.funding + "</td>"
                        + "<td align='center' style='border:1px solid blue;'>" + item.MissionObjective + "</td>"
                        + "<td align='center' style='border:1px solid blue;'>" + item.RDORDRDSignature + "</td>"
                        + "<td align='center' style='border:1px solid blue;'>" + item.MissionItinerary + "</td>"
                        + "<td align='center' style='border:1px solid blue;'>Mission</td></tr>";


                var leaves = db.VAC_TRANS.Include(ll => ll.staff);
                leaves = leaves.Where(ll => ll.EMP_ID == empid && ll.ISSUE_DATE.Value.Month == DateTime.Now.Month || ll.FROM_DATE.Value.Month == DateTime.Now.Month && ll.EMP_ID == empid);


                foreach (var itemL in leaves)
                {

                    var leaverr = db.EMPLOYEES.Include(l => l.staff);
                    leaverr = leaverr.Where(l => l.EMP_ID == itemL.EMP_ID);

                    var leaveid = itemL.VACATION_TYPE_ID;

                    var leavetype = db.VACATION_TYPE_LIST.Include(l => l.VAC_TRANS);
                    leavetype = leavetype.Where(l => l.VACATION_TYPE_ID == leaveid);

                    var leavetypename = leavetype.First().VACATION_TYPE_NAME;

                    Body = Body + "<tr><td align='center' style='border:1px dotted red;'>" + itemL.EMP_ID + "</td>"
                                + "<td align='center' style='border:1px dotted red;'>" + leaverr.First().EMP_FIRST_NAME + " " + leaverr.First().EMP_MIDDLE_NAME + " " + leaverr.First().EMP_LAST_NAME + "</td>"
                                + "<td align='center' style='border:1px dotted red;'>" + leavetypename + "</td>"
                                + "<td align='center' style='border:1px dotted red;'>" + itemL.FROM_DATE.Value.ToString("dd/MMM/yy") + "</td>"
                                + "<td align='center' style='border:1px dotted red;'>" + itemL.TO_DATE.Value.ToString("dd/MMM/yy") + "</td>"
                                + "<td align='center' style='border:1px dotted red;'>" + itemL.REASON + "</td></tr>";

                }




            }





            mail.Body = Body + "</table> <br><font size='2' color='blue'>RBC SMS</font><b> This is an automatically generated email, please do not reply.</b>";

            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtprelay.global.wfp.org";
            smtp.Port = 25;
            smtp.UseDefaultCredentials = true;
            smtp.Send(mail);

            //}//end if

        }


        ////////////////// report mission leave monthly end


        ////////////////////// leave report

        public void leaveMonthly(string groupName)
        {
            //RecurringJob.AddOrUpdate(() => GetADGroupUsers("ss"), Cron.Weekly(DayOfWeek.Thursday, 16));



            var WeeklyMission = db.MissionAuthorizations.Include(s => s.staff);
            WeeklyMission = WeeklyMission.Where(s => s.SignatureDate.Value.Month == DateTime.Now.Month || s.FromDate.Value.Month == DateTime.Now.Month).OrderBy(s => s.staff.unit.unit_description_english);



            //DateTime MDate = WeeklyMission.First().SignatureDate.Value;
            //DateTime NDate = DateTime.Now;
            //var weeks = (NDate.Subtract(MDate)).TotalDays / 7;
            //if (weeks == 0)
            //{


            MailMessage mail = new MailMessage();
            mail.To.Add("ahmed.badr@wfp.org");
            //mail.CC.Add(cc);
            mail.Bcc.Add("ahmed.badr@wfp.org");
            mail.From = new MailAddress("RBC-no-reply@wfp.org");
            mail.Subject = "Leave Monthly Report";

            string Body = "<table cellpadding='10'><tr><td align='center' style='border:1px solid red;'><font color='blue'>Index No.</font></td>"
                       + "<td align='center' style='border:1px solid red;'><font color='blue'>Staff Name</font></td>"
                       + "<td align='center' style='border:1px solid red;'><font color='blue'>Unit</font></td>"
                       + "<td align='center' style='border:1px solid red;'><font color='blue'>Leave Type </font></td>"
                       + "<td align='center' style='border:1px solid red;'><font color='blue'>From</font></td>"
                       + "<td align='center' style='border:1px solid red;'><font color='blue'>To</font></td>"
                       + "<td align='center' style='border:1px solid red;'><font color='blue'>Number of Days</font></td>"
                       + "<td align='center' style='border:1px solid red;'><font color='blue'>Issue Date</font></td>"
                       + "<td align='center' style='border:1px solid red;'><font color='blue'>Description</font></td></tr>";




            var empid = "";


            var leaves = db.VAC_TRANS.Include(ll => ll.staff);
            leaves = leaves.Where(ll => ll.ISSUE_DATE.Value.Month == DateTime.Now.Month && ll.ISSUE_DATE.Value.Year == DateTime.Now.Year || ll.FROM_DATE.Value.Month == DateTime.Now.Month && ll.ISSUE_DATE.Value.Year == DateTime.Now.Year);

            foreach (var itemL in leaves)
            {

                var leaverr = db.EMPLOYEES.Include(l => l.staff);
                leaverr = leaverr.Where(l => l.EMP_ID == itemL.EMP_ID);

                var leaveid = itemL.VACATION_TYPE_ID;
                var empidformail = itemL.EMP_ID;

                var empidforemail = db.EMPLOYEES.Include(l => l.staff);
                empidforemail = empidforemail.Where(l => l.EMP_ID == empidformail);
                var emailforunit = empidforemail.First().EMAIL_ADDRESS;
                var deptidforemp2 = empidforemail.First().DEPID;

                var unitIDfiremp = db.staffs.Include(l => l.unit);
                unitIDfiremp = unitIDfiremp.Where(l => l.staff_email == emailforunit);
                int? unitIDforUnitname = 1;
                if (unitIDfiremp.Any())
                {
                    unitIDforUnitname = (int?)unitIDfiremp.First().unit_id;
                }
                else
                {
                    unitIDforUnitname = 1;
                }

                var unitnameforemp = db.units.Include(l => l.staffs);
                unitnameforemp = unitnameforemp.Where(l => l.unitid == unitIDforUnitname);
                var unitfullname = "";

                if (unitnameforemp.Any())
                {
                    unitfullname = unitnameforemp.First().unit_description_english;
                }
                else
                {

                    var DepartForEmp = db.DEPARTMENTS.Include(s => s.EMPLOYEES);
                    DepartForEmp = DepartForEmp.Where(s => s.DEP_ID == deptidforemp2);
                    if (DepartForEmp.Any())
                    {
                        unitfullname = DepartForEmp.First().DEP_NAME;
                    }
                    else
                    {
                        unitfullname = " ";
                    }
                }

                var leavetype = db.VACATION_TYPE_LIST.Include(l => l.VAC_TRANS);
                leavetype = leavetype.Where(l => l.VACATION_TYPE_ID == leaveid);

                var leavetypename = leavetype.First().VACATION_TYPE_NAME;

                Body = Body + "<tr><td align='center' style='border:1px dotted blue;'>" + itemL.EMP_ID + "</td>"
                            + "<td align='center' style='border:1px dotted blue;'>" + leaverr.First().EMP_FIRST_NAME + " " + leaverr.First().EMP_MIDDLE_NAME + " " + leaverr.First().EMP_LAST_NAME + "</td>"
                            + "<td align='center' style='border:1px dotted blue;'>" + unitfullname + "</td>"
                            + "<td align='center' style='border:1px dotted blue;'>" + leavetypename + "</td>"
                            + "<td align='center' style='border:1px dotted blue;'>" + itemL.FROM_DATE.Value.ToString("dd/MMM/yy") + "</td>"
                            + "<td align='center' style='border:1px dotted blue;'>" + itemL.TO_DATE.Value.ToString("dd/MMM/yy") + "</td>"
                            + "<td align='center' style='border:1px dotted blue;'>" + itemL.DAY_COUNT + "</td>"
                            + "<td align='center' style='border:1px dotted blue;'>" + itemL.ISSUE_DATE.Value.ToString("dd/MMM/yy") + "</td>"
                            + "<td align='center' style='border:1px dotted blue;'>" + itemL.REASON + "</td></tr>";




            }

            //Body = Body + "<tr><td colspan='9' align='center' style='border:1px solid red;'>leave</td></tr>";


            mail.Body = Body + "</table> <br><font size='2' color='blue'>RBC SMS</font><b> This is an automatically generated email, please do not reply.</b>";

            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtprelay.global.wfp.org";
            smtp.Port = 25;
            smtp.UseDefaultCredentials = true;
            smtp.Send(mail);

            //}//end if

        }


        ///////////////////// end leave report




        /// <summary>mission report for staff 
        /// 
        public void MissionMonthyReportForStaff(string groupName)
        {



            var staffs = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
            staffs = staffs.Where(s => s.staff_email != null && s.Country_office.office_description_english.Contains("RB") && s.staff_id != null && s.unit_id == 3 && s.staff_email != "muhannad.hadi@wfp.org" && s.staff_email != "carlo.scaramella@wfp.org" && s.staff_email != "youssef.yassin@wfp.org");
            foreach (var itemstaff in staffs)
            {


                var WeeklyMission = db.MissionAuthorizations.Include(s => s.staff);
                WeeklyMission = WeeklyMission.Where(s => s.SignatureDate.Value.Month == DateTime.Now.Month && s.SignatureDate.Value.Year == DateTime.Now.Year && s.staffid == itemstaff.staffid || s.FromDate.Value.Month == DateTime.Now.Month && s.SignatureDate.Value.Year == DateTime.Now.Year && s.staffid == itemstaff.staffid).OrderBy(s => s.staff.unit.unit_description_english);



                //DateTime MDate = WeeklyMission.First().SignatureDate.Value;
                //DateTime NDate = DateTime.Now;
                //var weeks = (NDate.Subtract(MDate)).TotalDays / 7;
                //if (weeks == 0)
                //{
                var staffserx = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                staffserx = staffserx.Where(s => s.staffid == itemstaff.staffid);
                var staffemailforIndexx = staffserx.First().staff_email;



                var empindexx = staffserx.First().staff_id;
                if (empindexx == null)
                {

                    var leavee = db.EMPLOYEES.Include(l => l.staff);
                    leavee = leavee.Where(l => l.EMAIL_ADDRESS == staffemailforIndexx);
                    if (leavee.Any())
                    {
                        empindexx = leavee.First().EMP_ID;
                    }
                    else
                    {
                        empindexx = " ";
                    }

                }



                MailMessage mail = new MailMessage();
                mail.To.Add(itemstaff.staff_email);
                //mail.CC.Add(cc);
                mail.Bcc.Add("ahmed.badr@wfp.org");
                mail.From = new MailAddress("RBC-no-reply@wfp.org");
                mail.Subject = "Mission Authorization Monthly Report " + empindexx + " " + itemstaff.staff_first_name + " " + itemstaff.staff_last_name;

                string Body = "<table cellpadding='10'><tr><td align='center' style='border:1px solid red;'><font color='blue'>Index No.</font></td>"
                       + "<td align='center' style='border:1px solid red;'><font color='blue'>Staff/On Behalf</font></td>"
                       + "<td align='center' style='border:1px solid red;'><font color='blue'>Unit</font></td>"
                       + "<td align='center' style='border:1px solid red;'><font color='blue'>From</font></td>"
                       + "<td align='center' style='border:1px solid red;'><font color='blue'>To</font></td>"
                       + "<td align='center' style='border:1px solid red;'><font color='blue'>Number of Dayes</font></td>"
                       + "<td align='center' style='border:1px solid red;'><font color='blue'>Issued Date</font></td>"
                       + "<td align='center' style='border:1px solid red;'><font color='blue'>Funding</font></td>"
                    //+ "<td align='center' style='border:1px solid red;'><font color='blue'>Objective</font></td>"
                       + "<td align='center' style='border:1px solid red;'><font color='blue'>Approved</font></td>"
                       + "<td align='center' style='border:1px solid red;'><font color='blue'>Itinerary</font></td></tr>";


                if (WeeklyMission.Any())
                {

                    foreach (var item in WeeklyMission)
                    {


                        var staffser = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                        staffser = staffser.Where(s => s.staffid == item.staffid);
                        var staffemailforIndex = staffser.First().staff_email;
                        /////////////////////////////////////////////on behalf
                        var onbehalfFullName = "";
                        if (item.staffonbehalf != null)
                        {

                            var staffseronbehalf = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                            staffseronbehalf = staffseronbehalf.Where(s => s.staffid == item.staffonbehalf);
                            onbehalfFullName = staffseronbehalf.First().staff_first_name + " " + staffseronbehalf.First().staff_last_name;

                        }

                        /////////////////////////////////////////////on behalf
                        var leave = db.EMPLOYEES.Include(l => l.staff);
                        leave = leave.Where(l => l.EMAIL_ADDRESS == staffemailforIndex);
                        var empindex = "";
                        if (leave.Any())
                        {
                            empindex = leave.First().EMP_ID;
                        }
                        else
                        {
                            var staffserindex = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                            staffserindex = staffserindex.Where(s => s.staff_id != null);
                            if (staffserindex.Any())
                            {
                                empindex = staffserindex.First().staff_id;
                            }
                            else
                            {
                                empindex = " ";
                            }

                        }

                        var canbedoneee = "";
                        if (item.canbedone == true)
                        {
                            canbedoneee = "Yes";
                        }
                        else
                        {
                            canbedoneee = "No";
                        }

                        var CleredBySupe2 = "";
                        if (item.ClearedBySupervisor == true)
                        {
                            CleredBySupe2 = "Yes";
                        }
                        else
                        {
                            CleredBySupe2 = "No";
                        }

                        var FirstDate = item.FromDate;
                        var SecondDate = item.ToDate;
                        var dayesNumber = (SecondDate.Value - FirstDate.Value).TotalDays;


                        Body = Body + "<tr><td align='center' style='border:1px solid blue;'>" + empindexx + "</td>"
                            + "<td align='center' style='border:1px solid blue;'>" + staffser.First().staff_first_name + " " + staffser.First().staff_last_name + " / " + onbehalfFullName + "</td>"
                            + "<td align='center' style='border:1px solid blue;'>" + item.staff.unit.unit_description_english + "</td>"
                            + "<td align='center' style='border:1px solid blue;'>" + item.FromDate.Value.ToString("dd/MMM/yy") + "</td>"
                            + "<td align='center' style='border:1px solid blue;'>" + item.ToDate.Value.ToString("dd/MMM/yy") + "</td>"
                            + "<td align='center' style='border:1px solid blue;'>" + dayesNumber + "</td>"
                            + "<td align='center' style='border:1px solid blue;'>" + item.SignatureDate.Value.ToString("dd/MMM/yy") + "</td>"
                            + "<td align='center' style='border:1px solid blue;'>" + item.funding + "</td>"
                            //+ "<td align='center' style='border:1px solid blue;'>" + item.MissionObjective + "</td>"
                            + "<td align='center' style='border:1px solid blue;'>" + item.RDORDRDSignature + "</td>"
                            + "<td align='center' style='border:1px solid blue;'>" + item.MissionItinerary + "</td></tr>";


                    }
                }
                else
                {
                    Body = Body + "<tr><td align='center' style='border:1px solid blue;' colspan='10'>No Missions</td></tr>";
                }


                mail.Body = Body + "</table> <br><font size='2' color='blue'>RBC SMS</font><b> This is an automatically generated email, please do not reply.</b>";

                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtprelay.global.wfp.org";
                smtp.Port = 25;
                smtp.UseDefaultCredentials = true;
                smtp.Send(mail);

                //}//end if
            }
        }
        /// <summary>
        /// 
        /// 
        /// mission report for staff end
        /// 


        /// <summary>Leave report for staff 
        /// 
        public void LeaveMonthyReportForStaff(string groupName)
        {



            var staffs = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
            staffs = staffs.Where(s => s.staff_email != null && s.Country_office.office_description_english.Contains("RB") && s.staff_id != null && s.unit_id == 3 && s.staff_email != "muhannad.hadi@wfp.org" && s.staff_email != "carlo.scaramella@wfp.org" && s.staff_email != "waheed.habib@wfp.org" && s.staff_email != "youssef.yassin@wfp.org" && s.staff_email != "dalia.mansour@wfp.org");


            foreach (var itemstaff in staffs)
            {
                //var staff_Index = itemstaff.staff_id;
                //var WeeklyLeaveindex = db.VAC_TRANS.Include(s => s.staff);
                //WeeklyLeaveindex = WeeklyLeaveindex.Where(s => s.EMP_ID  == staff_Index) ;
                //var EmpID = WeeklyLeaveindex.First().EMP_ID;

                var WeeklyLeave = db.VAC_TRANS.Include(s => s.staff);
                WeeklyLeave = WeeklyLeave.Where(s => s.ISSUE_DATE.Value.Month == DateTime.Now.Month && s.ISSUE_DATE.Value.Year == DateTime.Now.Year && s.EMP_ID == itemstaff.staff_id || s.FROM_DATE.Value.Month == DateTime.Now.Month && s.ISSUE_DATE.Value.Year == DateTime.Now.Year && s.EMP_ID == itemstaff.staff_id).OrderBy(s => s.staff.unit.unit_description_english);



                //DateTime MDate = WeeklyMission.First().SignatureDate.Value;
                //DateTime NDate = DateTime.Now;
                //var weeks = (NDate.Subtract(MDate)).TotalDays / 7;
                //if (weeks == 0)
                //{
                var staffserx = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                staffserx = staffserx.Where(s => s.staffid == itemstaff.staffid);
                var staffemailforIndexx = staffserx.First().staff_email;



                var empindexx = staffserx.First().staff_id;
                if (empindexx == null)
                {

                    var leavee = db.EMPLOYEES.Include(l => l.staff);
                    leavee = leavee.Where(l => l.EMAIL_ADDRESS == staffemailforIndexx);
                    if (leavee.Any())
                    {
                        empindexx = leavee.First().EMP_ID;
                    }
                    else
                    {
                        empindexx = " ";
                    }

                }

                MailMessage mail = new MailMessage();
                mail.To.Add(itemstaff.staff_email);
                //mail.CC.Add(cc);
                mail.Bcc.Add("ahmed.badr@wfp.org");
                mail.From = new MailAddress("RBC-no-reply@wfp.org");
                mail.Subject = "Leave Monthly Report " + empindexx + " " + itemstaff.staff_first_name + " " + itemstaff.staff_last_name;

                string Body = "<table cellpadding='10'><tr><td align='center' style='border:1px solid red;'><font color='blue'>Index No.</font></td>"
                        + "<td align='center' style='border:1px solid red;'><font color='blue'>Staff Name</font></td>"
                        + "<td align='center' style='border:1px solid red;'><font color='blue'>Unit</font></td>"
                        + "<td align='center' style='border:1px solid red;'><font color='blue'>Leave Type </font></td>"
                        + "<td align='center' style='border:1px solid red;'><font color='blue'>From</font></td>"
                        + "<td align='center' style='border:1px solid red;'><font color='blue'>To</font></td>"
                        + "<td align='center' style='border:1px solid red;'><font color='blue'>Number of Days</font></td>"
                        + "<td align='center' style='border:1px solid red;'><font color='blue'>Issue Date</font></td>"
                        + "<td align='center' style='border:1px solid red;'><font color='blue'>Description</font></td></tr>";

                foreach (var item in WeeklyLeave)
                {

                    var staffser = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                    staffser = staffser.Where(s => s.staff_id == item.EMP_ID);
                    var staffemailforIndex = staffser.First().staff_email;

                    var leave = db.EMPLOYEES.Include(l => l.staff);
                    leave = leave.Where(l => l.EMAIL_ADDRESS == staffemailforIndex);
                    var empindex = "";
                    if (leave.Any())
                    {
                        empindex = leave.First().EMP_ID;
                    }
                    else
                    {
                        var staffserindex = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                        staffserindex = staffserindex.Where(s => s.staff_id != null);
                        if (staffserindex.Any())
                        {
                            empindex = staffserindex.First().staff_id;
                        }
                        else
                        {
                            empindex = " ";
                        }

                    }

                    var leaverr = db.EMPLOYEES.Include(l => l.staff);
                    leaverr = leaverr.Where(l => l.EMP_ID == item.EMP_ID);

                    var leaveid = item.VACATION_TYPE_ID;
                    var empidformail = item.EMP_ID;

                    var empidforemail = db.EMPLOYEES.Include(l => l.staff);
                    empidforemail = empidforemail.Where(l => l.EMP_ID == empidformail);
                    var emailforunit = empidforemail.First().EMAIL_ADDRESS;
                    var deptidforemp2 = empidforemail.First().DEPID;

                    var unitIDfiremp = db.staffs.Include(l => l.unit);
                    unitIDfiremp = unitIDfiremp.Where(l => l.staff_email == emailforunit);
                    int? unitIDforUnitname = 1;
                    if (unitIDfiremp.Any())
                    {
                        unitIDforUnitname = (int?)unitIDfiremp.First().unit_id;
                    }
                    else
                    {
                        unitIDforUnitname = 1;
                    }

                    var unitnameforemp = db.units.Include(l => l.staffs);
                    unitnameforemp = unitnameforemp.Where(l => l.unitid == unitIDforUnitname);
                    var unitfullname = "";

                    if (unitnameforemp.Any())
                    {
                        unitfullname = unitnameforemp.First().unit_description_english;
                    }
                    else
                    {

                        var DepartForEmp = db.DEPARTMENTS.Include(s => s.EMPLOYEES);
                        DepartForEmp = DepartForEmp.Where(s => s.DEP_ID == deptidforemp2);
                        if (DepartForEmp.Any())
                        {
                            unitfullname = DepartForEmp.First().DEP_NAME;
                        }
                        else
                        {
                            unitfullname = " ";
                        }
                    }

                    var leavetype = db.VACATION_TYPE_LIST.Include(l => l.VAC_TRANS);
                    leavetype = leavetype.Where(l => l.VACATION_TYPE_ID == leaveid);

                    var leavetypename = leavetype.First().VACATION_TYPE_NAME;


                    var FirstDate = item.FROM_DATE;
                    var SecondDate = item.TO_DATE;
                    var dayesNumber = (SecondDate.Value - FirstDate.Value).TotalDays;


                    Body = Body + "<tr><td align='center' style='border:1px dotted blue;'>" + item.EMP_ID + "</td>"
                                + "<td align='center' style='border:1px dotted blue;'>" + leaverr.First().EMP_FIRST_NAME + " " + leaverr.First().EMP_MIDDLE_NAME + " " + leaverr.First().EMP_LAST_NAME + "</td>"
                                + "<td align='center' style='border:1px dotted blue;'>" + unitfullname + "</td>"
                                + "<td align='center' style='border:1px dotted blue;'>" + leavetypename + "</td>"
                                + "<td align='center' style='border:1px dotted blue;'>" + item.FROM_DATE.Value.ToString("dd/MMM/yy") + "</td>"
                                + "<td align='center' style='border:1px dotted blue;'>" + item.TO_DATE.Value.ToString("dd/MMM/yy") + "</td>"
                                + "<td align='center' style='border:1px dotted blue;'>" + item.DAY_COUNT + "</td>"
                                + "<td align='center' style='border:1px dotted blue;'>" + item.ISSUE_DATE.Value.ToString("dd/MMM/yy") + "</td>"
                                + "<td align='center' style='border:1px dotted blue;'>" + item.REASON + "</td></tr>";

                }
                mail.Body = Body + "</table> <br><font size='2' color='blue'>RBC SMS</font><b> This is an automatically generated email, please do not reply.</b>";

                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtprelay.global.wfp.org";
                smtp.Port = 25;
                smtp.UseDefaultCredentials = true;
                smtp.Send(mail);

                //}//end if
            }
        }
        /// <summary>
        /// 
        /// 
        /// Leave report for staff end

        ///////// addmissing information end start
        public void AddMissingInformation(string groupName)
        {
            var staffs = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
            staffs = staffs.Where(s => s.staff_email != null);
            foreach (var itemstaff in staffs)
            {
                var stafemal11 = itemstaff.staff_email;

                if (itemstaff.staff_supervisorid == null || itemstaff.staff_first_name == null || itemstaff.unit_id == null || itemstaff.country_office_id == null || itemstaff.functional_title_id == null)
                {

                    ///////////////////////

                    var selectmanagerindex = db.EMPLOYEES.Include(emp => emp.staff);
                    selectmanagerindex = selectmanagerindex.Where(emp => emp.EMAIL_ADDRESS == stafemal11);
                    //var empEmail = selectmanagerindex.First().EMAIL_ADDRESS;

                    var managerindexB = "";
                    var Depid = "";
                    var JobTitleid = "";
                    if (selectmanagerindex.Any())
                    {

                        managerindexB = selectmanagerindex.First().MANAGER;

                        //title job
                        JobTitleid = selectmanagerindex.First().JOB_ID;
                        //title job end


                        Depid = selectmanagerindex.First().DEPID;
                        var firstnameB = selectmanagerindex.First().EMP_FIRST_NAME;
                        var lastnameB = selectmanagerindex.First().EMP_LAST_NAME;
                        var IndexNumberB = selectmanagerindex.First().EMP_ID;


                        //title job
                        var jobtitleforstaff = db.Job_DESCRIPTION.Include(s => s.EMPLOYEES);
                        jobtitleforstaff = jobtitleforstaff.Where(s => s.JOB_ID == JobTitleid);

                        var jobtitlename = "";
                        if (jobtitleforstaff.Any())
                        {

                            jobtitlename = jobtitleforstaff.First().JOB_TITLE;

                        }
                        else
                        {

                            jobtitlename = "";

                        }

                        var staffstitlejob = db.functional_title.Include(s => s.staffs);
                        staffstitlejob = staffstitlejob.Where(s => s.functional_title_description_english == jobtitlename);

                        if (staffstitlejob.Any())
                        {
                            int titlejobid = staffstitlejob.First().functionaltitleid;
                            var staffupdatetitlejob = db.staffs.Single(u => u.staff_email == stafemal11);
                            staffupdatetitlejob.functional_title_id = titlejobid;
                            db.SaveChanges();

                        }
                        else
                        {

                            var addunitB = db.functional_title.Add(new functional_title { functional_title_description_english = jobtitlename }).functional_title_description_english;
                            db.SaveChanges();

                            var selectnewtitleid = db.functional_title.Include(s => s.staffs);
                            selectnewtitleid = selectnewtitleid.Where(s => s.functional_title_description_english == jobtitlename);
                            var newtitlejobid = selectnewtitleid.First().functionaltitleid;

                            var staffudatetitlejobid = db.staffs.Single(u => u.staff_email == stafemal11);
                            staffudatetitlejobid.functional_title_id = newtitlejobid;
                            db.SaveChanges();

                        }

                        //title job end

                        //country_office update
                        var branchidB = selectmanagerindex.First().BRANCH_ID;

                        var brachForEmpB = db.BRANCHES.Include(s => s.EMPLOYEES);
                        brachForEmpB = brachForEmpB.Where(s => s.BRANCH_ID == branchidB);

                        var Branch_nameB = "";
                        var Branch_managerB = "";
                        if (brachForEmpB.Any())
                        {

                            Branch_nameB = brachForEmpB.First().BRANCH1;
                            Branch_managerB = brachForEmpB.First().BRANCH_MANAGER;
                        }
                        else
                        {

                            Branch_nameB = "egyco";
                            Branch_managerB = null;

                        }

                        var emailstaffforhouBB = db.EMPLOYEES.Include(s => s.staff);
                        emailstaffforhouBB = emailstaffforhouBB.Where(s => s.EMP_ID == Branch_managerB);
                        var mailhouempB = "";
                        if (emailstaffforhouBB.Any())
                        {

                            mailhouempB = emailstaffforhouBB.First().EMAIL_ADDRESS;
                        }
                        else
                        {

                            mailhouempB = null;

                        }

                        var staffhouidBB = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                        staffhouidBB = staffhouidBB.Where(s => s.staff_email == mailhouempB);
                        int houidEmBB = 1212;
                        if (staffhouidBB.Any())
                        {

                            houidEmBB = staffhouidBB.First().staffid;
                        }
                        else
                        {

                            houidEmBB = 1212;

                        }

                        var staffscompmidfornameunitB = db.Country_office.Include(s => s.staffs);
                        staffscompmidfornameunitB = staffscompmidfornameunitB.Where(s => s.office_description_english == Branch_nameB);

                        if (staffscompmidfornameunitB.Any())
                        {
                            int unitidB = staffscompmidfornameunitB.First().countryofficeid;
                            var staffmanagerupdateB = db.staffs.Single(u => u.staff_email == stafemal11);
                            staffmanagerupdateB.country_office_id = unitidB;
                            db.SaveChanges();

                        }
                        else
                        {

                            var addunitB = db.Country_office.Add(new Country_office { office_description_english = Branch_nameB }).office_description_english;
                            db.SaveChanges();

                            var selectnewunitB = db.Country_office.Include(s => s.staffs);
                            selectnewunitB = selectnewunitB.Where(s => s.office_description_english == Branch_nameB);
                            var newunitidB = selectnewunitB.First().countryofficeid;

                            var houfornewunitB = db.Country_office.Single(u => u.countryofficeid == newunitidB);
                            houfornewunitB.Manager = houidEmBB;
                            db.SaveChanges();

                            var staffudateunitidB = db.staffs.Single(u => u.staff_email == stafemal11);
                            staffudateunitidB.country_office_id = newunitidB;
                            db.SaveChanges();

                        }

                        //// country office id for staff
                        //var staffmanagerupdateBCO = db.staffs.Single(u => u.staff_email == stafemal11);
                        //staffmanagerupdateBCO.country_office_id = 28;
                        //db.SaveChanges();
                        //// country office id for staff end


                        //country_office update END

                        var updatefirslastname = db.staffs.Single(u => u.staff_email == stafemal11);
                        updatefirslastname.staff_first_name = firstnameB;
                        updatefirslastname.staff_id = IndexNumberB;
                        updatefirslastname.staff_index_number = IndexNumberB;

                        if (lastnameB == null)
                        {
                            updatefirslastname.staff_last_name = lastnameB;
                        }

                        db.SaveChanges();

                        var SuperForEmpM = db.EMPLOYEES.Include(s => s.staff);
                        SuperForEmpM = SuperForEmpM.Where(s => s.EMP_ID == managerindexB);
                        var mangerEmail = "";
                        if (SuperForEmpM.Any())
                        {
                            mangerEmail = SuperForEmpM.First().EMAIL_ADDRESS;

                        }
                        else
                        {

                            mangerEmail = null;

                        }

                        var staffscompmid = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                        staffscompmid = staffscompmid.Where(s => s.staff_email == mangerEmail);
                        int managerid = 1212;
                        if (staffscompmid.Any())
                        {
                            managerid = staffscompmid.First().staffid;

                        }
                        else
                        {

                            managerid = 1212;

                        }

                        var staffmanagerupdate1 = db.staffs.Single(u => u.staff_email == stafemal11);
                        staffmanagerupdate1.staff_supervisorid = managerid;
                        db.SaveChanges();

                        var UNITForEmpM = db.DEPARTMENTS.Include(s => s.EMPLOYEES);
                        UNITForEmpM = UNITForEmpM.Where(s => s.DEP_ID == Depid);
                        var unitnameforemp = "";
                        var dephouindex = "";
                        if (UNITForEmpM.Any())
                        {
                            unitnameforemp = UNITForEmpM.First().DEP_NAME;
                            dephouindex = UNITForEmpM.First().DEP_MANAGER;

                        }
                        else
                        {

                            unitnameforemp = "";
                            dephouindex = null;

                        }

                        var emailstaffforhou = db.EMPLOYEES.Include(s => s.staff);
                        emailstaffforhou = emailstaffforhou.Where(s => s.EMP_ID == dephouindex);
                        var mailhouemp = "";
                        if (emailstaffforhou.Any())
                        {
                            mailhouemp = emailstaffforhou.First().EMAIL_ADDRESS;

                        }
                        else
                        {

                            mailhouemp = "";

                        }

                        var staffhouid = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                        staffhouid = staffhouid.Where(s => s.staff_email == mailhouemp);
                        int houidEmp = 1;
                        if (staffscompmid.Any())
                        {
                            houidEmp = staffscompmid.First().staffid;

                        }
                        else
                        {

                            houidEmp = 1;

                        }

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


                    }

                }
            }

        }



        ///////// add missing information end



        /// <summary>
        /// //// report
        /// <returns></returns>
        public ActionResult Simple()
        {

            List<SiteMenu> all = new List<SiteMenu>();
            using (WFPEntities1 dc = new WFPEntities1())
            {
                all = dc.SiteMenus.OrderBy(a => a.ParentMenuID).ToList();
            }
            return View(all);
        }



        //public PartialViewResult Germany()
        //{
        //    var result = from r in db.countries
        //                 where r.country_name == "Egypt"
        //                 select r;
        //    return PartialView("relatedlinks", result);
        //}


        //public PartialViewResult Mexico()
        //{
        //    var result = from r in db.governorates
        //                 where r.governorates_name == "Assuit"
        //                 select r;
        //    return PartialView("relatedlinks", result);
        //}

        ////////////////////////////Links/////////////////////////

        // GET: MissionAuthorizations
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, string node)
        {

            /////sc
            int monht = DateTime.Now.Month;
            int year = DateTime.Now.Year;
            int day = DateTime.DaysInMonth(year, monht);
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
            RecurringJob.AddOrUpdate(() => GetADGroupUsers("ss"), Cron.Weekly(DayOfWeek.Thursday, 10));
            RecurringJob.AddOrUpdate(() => GetADGroupUsersM("ss"), Cron.Monthly(day, 14));
            //RecurringJob.AddOrUpdate(() => missionleaveMonthly("ss"), Cron.Monthly(day, 16));
            RecurringJob.AddOrUpdate(() => leaveMonthly("ss"), Cron.Monthly(day, 14));
            RecurringJob.AddOrUpdate(() => LeaveWeeklyReport("ss"), Cron.Weekly(DayOfWeek.Thursday, 10));
            RecurringJob.AddOrUpdate(() => MissionMonthyReportForStaff("ss"), Cron.Monthly(day, 14));
            RecurringJob.AddOrUpdate(() => AddMissingInformation("ss"), Cron.Monthly(day, 14));
            RecurringJob.AddOrUpdate(() => LeaveMonthyReportForStaff("ss"), Cron.Monthly(day, 14));
            RecurringJob.AddOrUpdate(() => GetADGroupUsersADDCall("ss"), Cron.Monthly(day, 14));
            RecurringJob.AddOrUpdate(() => MissionDailyReport("ss"), Cron.Daily(14));
            RecurringJob.AddOrUpdate(() => LeaveDailyReport("ss"), Cron.Daily(14));
            RecurringJob.AddOrUpdate(() => LeaveDailyReport2("ss"), Cron.Daily(21, 59));

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////    

            //////sc



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

            if (staffs.First().staff_first_name == null)
            {
                staffs.First().staff_first_name = "no Name";
            }

            if (staffs.First().staff_last_name == null)
            {
                staffs.First().staff_last_name = "no Name";
            }

            if (staffs.First().unit_id == null)
            {
                staffs.First().unit_id = 15;
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
                         on p.staffid equals a.staff_supervisorid


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
            //(staffs.First().functional_title.functional_title_description_english == "REGIONAL DIRECTOR" || staffs.First().functional_title.functional_title_description_english == "Deputy Regional Director")


            var staffser = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
            staffser = staffser.Where(s => s.staff_email.Contains(User.Identity.Name + "@wfp.org"));
            var userUnit = staffser.First().unit_id;

            var unithou = db.units.Include(d => d.staffs);
            unithou = unithou.Where(u => u.unitid == userUnit);
            var houid = unithou.First().HOU_ID;

            if (staffser.First().staffid == houid)
            {
                var missionAuthorizations = db.MissionAuthorizations.Include(m => m.staff);
                missionAuthorizations = missionAuthorizations.Where(s => s.staff.staff_supervisorid == (superid) || s.staff.staffid == (superid) && s.non5 != 11 || s.staff.unit_id == userUnit && s.non5 != 11).OrderByDescending(s => s.MissionID);

                

                ViewBag.loginid = staffs.First().staffid;


                if (!String.IsNullOrEmpty(searchString))
                {
                    missionAuthorizations = missionAuthorizations.Where(s => s.MissionID.Equals(searchString));

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
            missionAuthorizationss = missionAuthorizationss.Where(s => s.staff.staff_supervisorid == (superid) || s.staff.staffid == (superid) && s.non5 != 11 || s.staffonbehalf == s.staff.staffid).OrderByDescending(s => s.MissionID);
          
            var missionAuthorizationssOnbehalf = db.MissionAuthorizations.Include(m => m.staff);
            missionAuthorizationssOnbehalf = missionAuthorizationssOnbehalf.Where(s => s.staffonbehalf == s.staff.staffid);

           
            

            var onbehafname = "";

            if (missionAuthorizationssOnbehalf.Any())
            {
                onbehafname = missionAuthorizationssOnbehalf.First().staff.staff_first_name + " " + missionAuthorizationssOnbehalf.First().staff.staff_last_name;

            }


            onbehafname = "";

            ViewBag.onbehalfFullName = onbehafname;

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
                    missionAuthorizationss = missionAuthorizationss.OrderByDescending(s => s.FromDate);
                    break;
                case "name_desc":
                    missionAuthorizationss = missionAuthorizationss.OrderByDescending(s => s.staff.staff_email);
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
            //(staffs.First().functional_title.functional_title_description_english == "REGIONAL DIRECTOR" || staffs.First().functional_title.functional_title_description_english == "Deputy Regional Director")



            var missionAuthorizations = db.MissionAuthorizations.Include(m => m.staff);
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





        // GET: MissionAuthorizations/v




        // GET: MissionAuthorizations/Create

        public ActionResult Create(MissionAuthorization newrecord)
        {
           
               
            
           

            if (!Request.IsAuthenticated)
            {
                Response.Redirect("/RBC-SMS/account/login");
            }
           

           

                ViewBag.Mission_Type = new SelectList(db.MissionTypes, "ID", "Name");
                var staffs = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                staffs = staffs.Where(s => s.staff_email == User.Identity.Name + "@wfp.org");
                var stafemal11 = staffs.First().staff_email;
                var staffforhouforsuper = staffs.First().staffid;




////////////////////////////////////////////////////////////////////

                var latestmission2 = db.MissionAuthorizations.Include(s => s.MissionItineraries).Include(s => s.staff);
                latestmission2 = latestmission2.Where(s => s.staffid == staffforhouforsuper && s.non5 == 11);

                if (latestmission2.Any() == false)
                {
                    newrecord.staffid = staffforhouforsuper;
                    newrecord.non5 = 11;
                    db.MissionAuthorizations.Add(newrecord);
                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.SaveChanges();
                    int newrow = newrecord.MissionID;

                    ViewBag.MissionID22 = newrow;

                }
               
////////////////////////////////////////////////////////////////////

               var missionup = latestmission2.First ().MissionID;
               ViewBag.MissionID22 = missionup;
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
                ViewBag.StaffTitle = staffs.First().functional_title.functional_title_description_english;
                ViewBag.contractType = staffs.First().contract_type.contract_type_name;





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
                if (supervisoremailif == "muhannad.hadi@wfp.org")
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

                if (houmailUPDATE == "muhannad.hadi@wfp.org")
                {
                    houmailUPDATE = "rbc.management@wfp.org"; //rbc.management@wfp.org

                }


                ViewBag.houmail = houmailUPDATE;
            



            

            ViewBag.FromID = new SelectList(db.countries, "countryid", "country_name");
            ViewBag.ToID = new SelectList(db.countries, "countryid", "country_name");
            ViewBag.MissionID = new SelectList(db.MissionAuthorizations, "MissionID", "MissionID");
            ViewBag.destinationCountry = new SelectList(db.countries, "countryid", "country_name");
            return View();
        }

        //POST: MissionAuthorizations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(HttpPostedFileBase files, [Bind(Include = "MissionID,staffid,FromDate,ToDate,MissionItinerary,funding,ClearedBySupervisor,MissionObjective,ExpectedOutputs,FollowUp,SignatureDate,canbedone,StaffSignature,FromTime,ToTime,Mission_Type,destinationCountry,destinationTwon,UberCareem,non18,non17")] MissionAuthorization missionAuthorization)
        {
            if (!ModelState.IsValid)
            {
              
                return RedirectToAction("Create");
            }
            
            if (ModelState.IsValid)
                    {

                        db.Entry(missionAuthorization).State = EntityState.Modified;
                        db.SaveChanges();

                //db.MissionAuthorizations.Add(missionAuthorization);
                           
                //        db.SaveChanges();
                        if (files != null)
                        {

                        string path = Path.Combine(Server.MapPath("~/UploadedFiles"), files.FileName);
                        files.SaveAs(path);
                        var addmissionAuthorizationJPG = db.MissionAuthorizations.Single(u => u.MissionID == missionAuthorization.MissionID);
                        addmissionAuthorizationJPG.files = files.FileName;                    
                        db.SaveChanges();

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


                var missionitinarry = db.MissionItineraries.Include(i => i.MissionAuthorization);
                missionitinarry = missionitinarry.Where(i => i.MissionID == missionAuthorization.MissionID);


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
                    if (missionAuthorization.ClearedBySupervisor == true)
                    {
                        CleredBySupervisor = "Yes";
                    }
                    else
                    {
                        CleredBySupervisor = "No";
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

                    var Days142 = "";
                    if (missionAuthorization.non18 == true)
                    {
                        Days142 = "Yes";
                    }
                    else
                    {
                        Days142 = "No";
                    }

                    //////////////////////////////


                    var supersforhou = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                    supersforhou = supersforhou.Where(d => d.staffid == staffsupervisor);
                    var superforhounitsname = supersforhou.First().staff_first_name + " " + staffshou.First().staff_last_name;
                    var houmailUPDATE = supersforhou.First().staff_email;

                    if (houmailUPDATE == "muhannad.hadi@wfp.org")
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

                    string Bodyhou = "<table><tr><td colspan='2'>" + "<hr/>New Mission<hr/></td></tr><tr><td><font color='blue'>Request : </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorization.MissionID + "</td></tr>"
                    + "<tr><td>" + "<font color='blue'>From :</font></td><td> " + missionAuthorization.staff.staff_first_name + " " + missionAuthorization.staff.staff_last_name + " </td></tr>"
                        //+ "<tr><td>" + "<font color='blue'>Onbehalf: :</font></td><td> " + missionAuthorization.staff1.staff_first_name + " " + missionAuthorization.staff1.staff_last_name + " </td></tr>"
                    + "<tr><td>" + "<font color='blue'>Unit :</font></td><td> " + missionAuthorization.staff.unit.unit_abreviation_english + "</td></tr>"
                    + "<tr><td>" + "<font color='blue'>Title :</font></td><td> " + missionAuthorization.staff.functional_title.functional_title_description_english + " </td></tr>"
                    + "<tr><td>" + "<font color='blue'>Contract Type :</font></td><td> " + missionAuthorization.staff.contract_type.contract_type_name + " </td></tr>"

                    //+ "<tr><td><font color='blue'>Official Start Date :</font></td><td>" + missionAuthorization.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                        //+ "<tr><td><font color='blue'>Official End Date :</font></td><td>" + missionAuthorization.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                        //+ "<tr><td><font color='blue'>Reporting Time :</font></td><td>" + missionAuthorization.FromTime.Value.ToString("HH:mm") + "</td></tr>"
                        // + "<tr><td><font color='blue'>End Time :</font></td><td>" + missionAuthorization.ToTime.Value.ToString("HH:mm") + "</td></tr>"
                   + "<tr><td colspan='2'>" + "<hr/>Mission Itinerary <hr/>" + "</td></tr>";

                    foreach (var item in missionitinarry)
                    {

                        Bodyhou = Bodyhou

                           + "<tr><td><font color='red'>Departure:</font></td><td></td></tr>"
                           + "<tr><td><font color='blue'>Country:</font></td><td>" + item.country.country_name + " / " + item.Twon1 + "</td></tr>"

                           + "<tr><td><font color='blue'>Date/Time :</font></td><td>" + item.FromDate.Value.ToString("dd/MMM/yy") + "---" + item.Fromtime.Value.ToString("HH:mm") + "</td></tr>"
                           + "<tr><td><font color='red'>Arrival:</font></td><td></td></tr>"
                           + "<tr><td><font color='blue'>Country/City:</font></td><td>" + item.country1.country_name + " / " + item.Twon2 + "</td></tr>"

                           + "<tr><td><font color='blue'>Date/Time :</font></td><td>" + item.ToDate.Value.ToString("dd/MMM/yy") + "---" + item.Totime.Value.ToString("HH:mm") + "</td></tr>"
                           + "<tr><hr/>";
                    }
                    Bodyhou = Bodyhou

       + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
             //+ "<tr><td><font color='blue'>Mission Type :</font></td><td>" + missionAuthorization.MissionType.Name + "</td></tr>"
             + "<tr><td><font color='blue'>Attached Administrative Note :</font></td><td><a href='http://10.11.135.254:8080/SMS/UploadedFiles/" + missionAuthorization.files + "'>" + missionAuthorization.files + "</a></td></tr>"
             + "<tr><td><font color='blue'>Funding :</font></td><td>" + missionAuthorization.funding + "</td></tr>"
             + "<tr><td><font color='blue'>Objective :</font></td><td>" + missionAuthorization.MissionObjective + "</td></tr>"
             + "<tr><td><font color='blue'>Expected Outputs :</font></td><td>" + missionAuthorization.ExpectedOutputs + "</td></tr>"
             + "<tr><td><font color='blue'>Was The Ticket Requested 14 Days In Advance? :</font></td><td>" + Days142 + "</td></tr>"
             + "<tr><td><font color='blue'>If Not, Please Explain :</font></td><td>" + missionAuthorization.non17 + "</td></tr>"
                        //+ "<tr><td><font color='blue'>Mission Destination :</font></td><td>" + missionAuthorization.country.country_name + " " + missionAuthorization.destinationTwon + "</td></tr>"
             + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video ? :</font></td><td>" + canbedone + "</td></tr>"
             + "<tr><td><font color='blue'>Cleared By Supervisor :</font></td><td>" + CleredBySupervisor + "</td></tr>"
             + "<tr><td><font color='blue'>International Roaming :</font></td><td>" + Roming + "</td></tr>"
             + "<tr><td><font color='blue'>Comment :</font></td><td>" + missionAuthorization.FollowUp + "</td></tr>";

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

                    ViewBag.destinationCountry = new SelectList(db.countries, "countryid", "country_name", missionAuthorization.destinationCountry);
                   
                    return RedirectToAction("Index");

                }
               


                if (to == "muhannad.hadi@wfp.org")
                {
                    to = "rbc.management@wfp.org"; //rbc.management@wfp.org

                }


                var canbedoneee = "";
                if (missionAuthorization.canbedone == true)
                {
                    canbedoneee = "Yes";
                }
                else
                {
                    canbedoneee = "No";
                }

                var CleredBySuper = "";
                if (missionAuthorization.ClearedBySupervisor == true)
                {
                    CleredBySuper = "Yes";
                }
                else
                {
                    CleredBySuper = "No";
                }

                MailMessage mail = new MailMessage();
                mail.To.Add("ahmed.badr@wfp.org"); // var to
                //mail.CC.Add(cc);
                mail.Bcc.Add("ahmed.badr@wfp.org");
                mail.From = new MailAddress(from);
                mail.Subject = "Mission Authorization FormCREATE";

                string Body = "<table><tr><td colspan='2'>" + "<hr/>New Mission<hr/></td></tr><tr><td><font color='blue'>Request : </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorization.MissionID + "</td></tr>"
                        + "<tr><td>" + "<font color='blue'>From:</font></td><td>" + staffs.First().staff_first_name + " " + staffs.First().staff_last_name + "</td></tr>"
                        + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                        //+ "<tr><td><font color='blue'>From :</font></td><td>" + missionAuthorization.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                        //+ "<tr><td><font color='blue'>To :</font></td><td>" + missionAuthorization.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                        + "<tr><td><font color='blue'>Funding:</font></td><td>" + missionAuthorization.funding + "</td></tr>"
                        + "<tr><td><font color='blue'>Objective:</font></td><td>" + missionAuthorization.MissionObjective + "</td></tr>"
                        //+ "<tr><td><font color='blue'>Mission Itinerary:</font></td><td>" + missionAuthorization.MissionItinerary + "</td></tr>"
                        + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video?:</font></td><td>" + canbedoneee + "</td></tr>"
                        + "<tr><td><font color='blue'>Cleared By Supervisor:</font></td><td>" + CleredBySuper + "</td></tr>"
                        + "<tr><td><a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid="
                        + missionAuthorization.MissionID
                        + "&RDORDRDSignature=Yes&FromDate=" + missionAuthorization.FromDate
                        + " &ToDate=" + missionAuthorization.ToDate
                        + " &to=" + to + "&staffmail=" + staffs.First().staff_email
                        + "'>Accept</a></td></tr><tr><td><a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization.MissionID
                        + "&RDORDRDSignature=NO & FromDate=" + missionAuthorization.FromDate
                        + " &ToDate=" + missionAuthorization.ToDate
                        + "&to=" + to
                        + "&staffmail=" + staffs.First().staff_email
                        + "'>Reject</a>" + "</td></tr></table>";
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
                smtp.Send(mail);



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

                //}
            
                    }


            ///////////////////////////////////////////           ///////////////////////////////////////////////////


              var staffss = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
              staffss = staffss.Where(s => s.staff_email == User.Identity.Name + "@wfp.org");
              var stafemal11 = staffss.First().staff_email;
              var staffforhouforsuper = staffss.First().staffid;


              if (staffss.First().staff_supervisorid == null || staffss.First().staff_first_name == null || staffss.First().unit_id == null || staffss.First().unit_id == null)
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

                  return View(missionAuthorization);


              }

              var superid = staffss.First().staffid;


              if (staffss.First().staff_supervisorid == null)
              {
                  staffss.First().staff_supervisorid = 1212;
                  ViewBag.error = 1;
              }

              if (staffss.First().staff_first_name == null)
              {
                  staffss.First().staff_first_name = "You must have First name !!!";
                  ViewBag.error = 1;
              }

              if (staffss.First().staff_last_name == null)
              {
                  staffss.First().staff_last_name = "You must have Last name !!!";
                  ViewBag.error = 1;
              }


              //ViewBag.staffid = new SelectList(db.staffs, "staffs.First().staffid", staffs.First().staff_email);
              ViewBag.staffid = staffss.First().staffid;
              ViewBag.staffid2 = staffss.First().staff_first_name;
              ViewBag.FirstName = staffss.First().staff_first_name;
              ViewBag.LastName = staffss.First().staff_last_name;
              ViewBag.StaffTitle = staffss.First().functional_title.functional_title_description_english;
              ViewBag.contractType = staffss.First().contract_type.contract_type_name;

              if (staffss.First().unit_id == null)
              {
                  ViewBag.Unit = "You must have a Unit !!!!";
                  staffss.First().unit_id = 15;
                  ViewBag.error = 1;

              }
              else
              {

                  ViewBag.Unit = staffss.First().unit.unit_description_english;
              }
              var staffsupervisors = staffss.First().staff_supervisorid;

              var userUnits = staffss.First().unit_id;

              var superss = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
              superss = superss.Where(d => d.staffid == staffsupervisors);





              ViewBag.supervisor = (superss.First().staff_first_name + " " + superss.First().staff_last_name);


              var supervisoremailif = superss.First().staff_email;
              if (supervisoremailif == "muhannad.hadi@wfp.org")
              {
                  supervisoremailif = "rbc.management@wfp.org"; // rbc.management@wfp.org

              }

              ViewBag.supervisorEmail = supervisoremailif;
              ViewBag.SignatureDate = DateTime.Now;



              var unithous = db.units.Include(d => d.staffs);
              unithous = unithous.Where(u => u.unitid == userUnits);
              var houids= unithous.First().HOU_ID;

              var staffshous = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
              staffshous = staffshous.Where(h => h.staffid == houids);

              //////////////////////////////
              var superforhounitsnames = "";
              var houmailUPDATEs = "";
              var houidforSuper = staffshous.First().staffid;
              if (houidforSuper == staffforhouforsuper)
              {
                  var supersforhou = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                  supersforhou = supersforhou.Where(d => d.staffid == staffsupervisors);
                  superforhounitsnames = supersforhou.First().staff_first_name + " " + staffshous.First().staff_last_name;
                  houmailUPDATEs = supersforhou.First().staff_email;

              }
              ///////////////////////////////

              var tos = staffshous.First().staff_email;

              superforhounitsnames = staffshous.First().staff_first_name + " " + staffshous.First().staff_last_name;
              ViewBag.houname = superforhounitsnames;

              houmailUPDATEs = staffshous.First().staff_email;

              if (houmailUPDATEs == "muhannad.hadi@wfp.org")
              {
                  houmailUPDATEs = "rbc.management@wfp.org"; //rbc.management@wfp.org

              }


              ViewBag.houmail = houmailUPDATEs;
            

            //////////////////////////////////////////            /////////////////////////////////////////////////////

              ViewBag.FromID = new SelectList(db.countries, "countryid", "country_name");
              ViewBag.ToID = new SelectList(db.countries, "countryid", "country_name");
              ViewBag.MissionID = new SelectList(db.MissionAuthorizations, "MissionID", "MissionID");
              ViewBag.destinationCountry = new SelectList(db.countries, "countryid", "country_name");
              ViewBag.Mission_Type = new SelectList(db.MissionTypes, "ID", "Name", missionAuthorization.Mission_Type);

            //ViewBag.staffid = User.Identity.GetUserName() + "@wfp.org";
            return View(missionAuthorization);
        }

        // GET: MissionAuthorizations/Edit/5
        public ActionResult Edit(int? id, int? id2, int? drd, int? loginid, int? behalfOf)
        {
            ////////////////////////////////  b edit 

            var staffse = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
            staffse = staffse.Where(s => s.staff_email.Contains(User.Identity.Name + "@wfp.org"));


            //ViewBag.staffid = new SelectList(db.staffs, "staffs.First().staffid", staffs.First().staff_email);
            ViewBag.staffid22 = staffse.First().staffid;

            if (id == null || id2 != ViewBag.staffid22 )
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                string passingURL = "/RBC-SMS/MissionAuthorizations/Index/";
                return Redirect(passingURL);
            }


            //////////////////////////////////////////
            var staffs = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
            staffs = staffs.Where(s => s.staff_email.Contains(User.Identity.Name + "@wfp.org"));
            var staffforhouforsuper = staffs.First().staffid;
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

                return RedirectToAction("SuperEdit", "MissionAuthorizations", new { id = id, id2 = id2, drd = drd, loginid = loginid });
            }




            MissionAuthorization missionAuthorization = db.MissionAuthorizations.Find(id);
            if (missionAuthorization == null)
            {
                return HttpNotFound();
            }
/////////////////////////////////////////////////////




/////////////////////////////////////////////


            ViewBag.staffid = staffs.First().staffid;
            ViewBag.destinationCountry = new SelectList(db.countries, "countryid", "country_name", missionAuthorization.destinationCountry);
            ViewBag.Mission_Type = new SelectList(db.MissionTypes, "ID", "Name", missionAuthorization.Mission_Type);



///////////////////////////////////////////            ////////////



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
            if (supervisoremailif == "muhannad.hadi@wfp.org")
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

            if (houmailUPDATE == "muhannad.hadi@wfp.org")
            {
                houmailUPDATE = "rbc.management@wfp.org"; //rbc.management@wfp.org

            }


            ViewBag.houmail = houmailUPDATE;


//////////////////////////////////////////////////////////////////////

            ViewBag.FromDate = missionAuthorization.FromDate.ToString();
            ViewBag.ToDate = missionAuthorization.ToDate.ToString();
            ViewBag.FromTime = missionAuthorization.FromTime.ToString();
            ViewBag.ToTime = missionAuthorization.ToTime.ToString();
            ViewBag.staffid = staffs.First().staffid;
            ViewBag.staffid2 = staffs.First().staff_first_name;
            ViewBag.FirstName = staffs.First().staff_first_name;
            ViewBag.LastName = staffs.First().staff_last_name;
            ViewBag.StaffTitle = staffs.First().functional_title.functional_title_description_english;
            ViewBag.contractType = staffs.First().contract_type.contract_type_name; 

            return View(missionAuthorization);
        }

        // POST: MissionAuthorizations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(HttpPostedFileBase files, [Bind(Include = "MissionID,staffid,MissionItinerary,funding,ClearedBySupervisor,MissionObjective,ExpectedOutputs,FollowUp,SignatureDate,FromTime,ToTime,Mission_Type,destinationCountry,destinationTwon,UberCareem,non18,non17,canbedone,StaffSignature,UberCareem")] MissionAuthorization missionAuthorization)
        {


            if (ModelState.IsValid)
            {
                

                db.Entry(missionAuthorization).State = EntityState.Modified;
                db.SaveChanges();
                if (files != null)
                {

                    string path = Path.Combine(Server.MapPath("~/UploadedFiles"), files.FileName);
                    files.SaveAs(path);
                    var addmissionAuthorizationJPG = db.MissionAuthorizations.Single(u => u.MissionID == missionAuthorization.MissionID);
                    addmissionAuthorizationJPG.files = files.FileName;
                    db.SaveChanges();

                }
                   
                

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

                var missionitinarry = db.MissionItineraries.Include(i => i.MissionAuthorization);
                missionitinarry = missionitinarry.Where(i => i.MissionID == missionAuthorization.MissionID);


                var cc = supers.First().staff_email;
                var supername = supers.First().staff_first_name + " " + supers.First().staff_last_name;


                if (staffsedit.First().staffid == houid)
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
                    if (missionAuthorization.ClearedBySupervisor == true)
                    {
                        CleredBySupervisor = "Yes";
                    }
                    else
                    {
                        CleredBySupervisor = "No";
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

                    var Days142 = "";
                    if (missionAuthorization.non18 == true)
                    {
                        Days142 = "Yes";
                    }
                    else
                    {
                        Days142 = "No";
                    }

                    var supersforhou = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                    supersforhou = supersforhou.Where(d => d.staffid == staffsupervisor);
                    var superforhounitsname = supersforhou.First().staff_first_name + " " + staffshou.First().staff_last_name;
                    var houmailUPDATE = supersforhou.First().staff_email;

                    if (houmailUPDATE == "muhannad.hadi@wfp.org")
                    {
                        houmailUPDATE = "rbc.management@wfp.org";//rbc.management@wfp.org

                    }


                    MailMessage mailhou = new MailMessage();
                    mailhou.To.Add("ahmed.badr@wfp.org"); // RD/DRD OFFICE/rbc.management@wfp.org
                    mailhou.Bcc.Add("ahmed.badr@wfp.org");
                    mailhou.From = new MailAddress(from);
                    mailhou.Subject = "Mission Updated";

                    string Bodyhou = "<table><tr><td colspan='2'>" + "<hr/>New Mission<hr/></td></tr><tr><td><font color='blue'>Request : </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorization.MissionID + "</td></tr>"
                   + "<tr><td>" + "<font color='blue'>From :</font></td><td> " + missionAuthorization.staff.staff_first_name + " " + missionAuthorization.staff.staff_last_name + " </td></tr>"
                        //+ "<tr><td>" + "<font color='blue'>Onbehalf: :</font></td><td> " + missionAuthorization.staff1.staff_first_name + " " + missionAuthorization.staff1.staff_last_name + " </td></tr>"
                   + "<tr><td>" + "<font color='blue'>Unit :</font></td><td> " + missionAuthorization.staff.unit.unit_abreviation_english + "</td></tr>"
                   + "<tr><td>" + "<font color='blue'>Title :</font></td><td> " + missionAuthorization.staff.functional_title.functional_title_description_english + " </td></tr>"
                   + "<tr><td>" + "<font color='blue'>Contract Type :</font></td><td> " + missionAuthorization.staff.contract_type.contract_type_name + " </td></tr>"

                   //+ "<tr><td><font color='blue'>Official Start Date :</font></td><td>" + missionAuthorization.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                        //+ "<tr><td><font color='blue'>Official End Date :</font></td><td>" + missionAuthorization.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                        //+ "<tr><td><font color='blue'>Reporting Time :</font></td><td>" + missionAuthorization.FromTime.Value.ToString("HH:mm") + "</td></tr>"
                        // + "<tr><td><font color='blue'>End Time :</font></td><td>" + missionAuthorization.ToTime.Value.ToString("HH:mm") + "</td></tr>"
                  + "<tr><td colspan='2'>" + "<hr/>Mission Itinerary <hr/>" + "</td></tr>";

                    foreach (var item in missionitinarry)
                    {

                        Bodyhou = Bodyhou

                           + "<tr><td><font color='red'>Departure:</font></td><td></td></tr>"
                           + "<tr><td><font color='blue'>Country:</font></td><td>" + item.country.country_name + " / " + item.Twon1 + "</td></tr>"

                           + "<tr><td><font color='blue'>Date/Time :</font></td><td>" + item.FromDate.Value.ToString("dd/MMM/yy") + "---" + item.Fromtime.Value.ToString("HH:mm") + "</td></tr>"
                           + "<tr><td><font color='red'>Arrival:</font></td><td></td></tr>"
                           + "<tr><td><font color='blue'>Country/City:</font></td><td>" + item.country1.country_name + " / " + item.Twon2 + "</td></tr>"

                           + "<tr><td><font color='blue'>Date/Time :</font></td><td>" + item.ToDate.Value.ToString("dd/MMM/yy") + "---" + item.Totime.Value.ToString("HH:mm") + "</td></tr>"
                           + "<tr><hr/>";
                    }
                    Bodyhou = Bodyhou

       + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
             //+ "<tr><td><font color='blue'>Mission Type :</font></td><td>" + missionAuthorization.MissionType.Name + "</td></tr>"
             + "<tr><td><font color='blue'>Attached Administrative Note :</font></td><td><a href='http://10.11.135.254:8080/SMS/UploadedFiles/" + missionAuthorization.files + "'>" + missionAuthorization.files + "</a></td></tr>"
             + "<tr><td><font color='blue'>Funding :</font></td><td>" + missionAuthorization.funding + "</td></tr>"
             + "<tr><td><font color='blue'>Objective :</font></td><td>" + missionAuthorization.MissionObjective + "</td></tr>"
             + "<tr><td><font color='blue'>Expected Outputs :</font></td><td>" + missionAuthorization.ExpectedOutputs + "</td></tr>"
             + "<tr><td><font color='blue'>Was The Ticket Requested 14 Days In Advance? :</font></td><td>" + Days142 + "</td></tr>"
             + "<tr><td><font color='blue'>If Not, Please Explain :</font></td><td>" + missionAuthorization.non17 + "</td></tr>"
                        //+ "<tr><td><font color='blue'>Mission Destination :</font></td><td>" + missionAuthorization.country.country_name + " " + missionAuthorization.destinationTwon + "</td></tr>"
             + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video ? :</font></td><td>" + canbedone + "</td></tr>"
             + "<tr><td><font color='blue'>Cleared By Supervisor :</font></td><td>" + CleredBySupervisor + "</td></tr>"
             + "<tr><td><font color='blue'>International Roaming :</font></td><td>" + Roming + "</td></tr>"
             + "<tr><td><font color='blue'>Comment :</font></td><td>" + missionAuthorization.FollowUp + "</td></tr>";

                    mailhou.Body = Bodyhou;
                    mailhou.IsBodyHtml = true;
                    SmtpClient smtphou = new SmtpClient();
                    smtphou.Host = "smtprelay.global.wfp.org";
                    smtphou.Port = 25;
                    smtphou.UseDefaultCredentials = true;
                    smtphou.Send(mailhou);//-

                    ViewBag.destinationCountry = new SelectList(db.countries, "countryid", "country_name", missionAuthorization.destinationCountry);
                    return RedirectToAction("Index");

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
                    if (missionAuthorization.ClearedBySupervisor == true)
                    {
                        CleredBySupervisor1 = "Yes";
                    }
                    else
                    {
                        CleredBySupervisor1 = "No";
                    }

                    var supersforhou11 = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                    supersforhou11 = supersforhou11.Where(d => d.staffid == staffsupervisor);
                    var superforhounitsname11 = supersforhou11.First().staff_first_name + " " + staffshou.First().staff_last_name;
                    var houmailUPDATE11 = supersforhou11.First().staff_email;

                    if (houmailUPDATE11 == "muhannad.hadi@wfp.org")
                    {
                        houmailUPDATE11 = "rbc.management@wfp.org";//rbc.management@wfp.org

                    }


                    MailMessage mailhou1 = new MailMessage();
                    mailhou1.To.Add("ahmed.badr@wfp.org"); // RD/DRD OFFICE/rbc.management@wfp.org
                    mailhou1.Bcc.Add("ahmed.badr@wfp.org");
                    mailhou1.From = new MailAddress(from);
                    mailhou1.Subject = "Mission Updated";

                    string Bodyhou1 = "<table><tr><td colspan='2'>" + "<hr/>Updated Mission<hr/></td></tr><tr><td><font color='blue'>Request: </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorization.MissionID + "</td></tr>"
                        + "<tr><td>" + "<font color='blue'>From:</font></td><td>" + staffsedit.First().staff_first_name + " " + staffsedit.First().staff_last_name + "</td></tr>"
                        + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                        //+ "<tr><td><font color='blue'>From:</font></td><td>" + missionAuthorization.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                        //+ "<tr><td><font color='blue'>To:</font></td><td>" + missionAuthorization.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                        + "<tr><td><font color='blue'>Funding:</font></td><td>" + missionAuthorization.funding + "</td></tr>"
                        + "<tr><td><font color='blue'>Objective:</font></td><td>" + missionAuthorization.MissionObjective + "</td></tr>"
                        + "<tr><td><font color='blue'>Mission Itinerary:</font></td><td>" + missionAuthorization.MissionItinerary + "</td></tr>"
                        + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video?:</font></td><td>" + canbedone1 + "</td></tr>"
                        + "<tr><td><font color='blue'>Cleared By Supervisor:</font></td><td>" + CleredBySupervisor1 + "</td></tr>"
                        + "<tr><td><a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid="
                        + missionAuthorization.MissionID
                        + "&RDORDRDSignature=Yes&FromDate=" + missionAuthorization.FromDate
                        + " &ToDate=" + missionAuthorization.ToDate
                        + " &to=" + to + "&staffmail=" + staffsedit.First().staff_email
                        + "'>Accept</a></td></tr><tr><td><a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization.MissionID
                        + "&RDORDRDSignature=NO & FromDate=" + missionAuthorization.FromDate
                        + " &ToDate=" + missionAuthorization.ToDate
                        + "&to=" + to
                        + "&staffmail=" + staffsedit.First().staff_email
                        + "'>Reject</a>" + "</td></tr></table>";

                    mailhou1.Body = Bodyhou1;
                    mailhou1.IsBodyHtml = true;
                    SmtpClient smtphou1 = new SmtpClient();
                    smtphou1.Host = "smtprelay.global.wfp.org";
                    smtphou1.Port = 25;
                    smtphou1.UseDefaultCredentials = true;
                    smtphou1.Send(mailhou1);//-
                    return RedirectToAction("Index");
               
            }

            var staffs = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
            staffs = staffs.Where(s => s.staff_email.Contains(User.Identity.Name + "@wfp.org"));
            var staffforhouforsuper = staffs.First().staffid;

            ViewBag.FromDate = missionAuthorization.FromDate.ToString();
            ViewBag.ToDate = missionAuthorization.ToDate.ToString();
            ViewBag.FromTime = missionAuthorization.FromTime.ToString();
            ViewBag.ToTime = missionAuthorization.ToTime.ToString();
            ViewBag.staffid = staffs.First().staffid;
            ViewBag.staffid2 = staffs.First().staff_first_name;
            ViewBag.FirstName = staffs.First().staff_first_name;
            ViewBag.LastName = staffs.First().staff_last_name;
            ViewBag.StaffTitle = staffs.First().functional_title.functional_title_description_english;
            ViewBag.contractType = staffs.First().contract_type.contract_type_name; 


            ViewBag.staffid = staffs.First().staffid;
            ViewBag.FirstNme = staffs.First().staff_first_name;
            ViewBag.LastName = staffs.First().staff_last_name;
            ViewBag.destinationCountry = new SelectList(db.countries, "countryid", "country_name", missionAuthorization.destinationCountry);
            ViewBag.Mission_Type = new SelectList(db.MissionTypes, "ID", "Name", missionAuthorization.Mission_Type);


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
            var staffsupervisor1 = staffs.First().staff_supervisorid;

            var userUnit1 = staffs.First().unit_id;

            var supers1 = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
            supers1 = supers1.Where(d => d.staffid == staffsupervisor1);





            ViewBag.supervisor = (supers1.First().staff_first_name + " " + supers1.First().staff_last_name);


            var supervisoremailif = supers1.First().staff_email;
            if (supervisoremailif == "muhannad.hadi@wfp.org")
            {
                supervisoremailif = "rbc.management@wfp.org"; // rbc.management@wfp.org

            }

            ViewBag.supervisorEmail = supervisoremailif;
            ViewBag.SignatureDate = DateTime.Now;



            var unithou1 = db.units.Include(d => d.staffs);
            unithou1 = unithou1.Where(u => u.unitid == userUnit1);
            var houid1 = unithou1.First().HOU_ID;

            var staffshou1 = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
            staffshou1 = staffshou1.Where(h => h.staffid == houid1);

            //////////////////////////////
            var superforhounitsname1 = "";
            var houmailUPDATE1 = "";
            var houidforSuper1 = staffshou1.First().staffid;
            if (houidforSuper1 == staffforhouforsuper)
            {
                var supersforhou = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                supersforhou = supersforhou.Where(d => d.staffid == staffsupervisor1);
                superforhounitsname1 = supersforhou.First().staff_first_name + " " + staffshou1.First().staff_last_name;
                houmailUPDATE1 = supersforhou.First().staff_email;

            }
            ///////////////////////////////

            var to1 = staffshou1.First().staff_email;

            superforhounitsname1 = staffshou1.First().staff_first_name + " " + staffshou1.First().staff_last_name;
            ViewBag.houname = superforhounitsname1;

            houmailUPDATE1 = staffshou1.First().staff_email;

            if (houmailUPDATE1 == "muhannad.hadi@wfp.org")
            {
                houmailUPDATE1 = "rbc.management@wfp.org"; //rbc.management@wfp.org

            }


            ViewBag.houmail = houmailUPDATE1;


            //    }

            //}



            return View(missionAuthorization);
        }


        ////////////////////////////////// create on behalf

        // GET: MissionAuthorizations/Create

        public ActionResult CreateOnbehalf(MissionAuthorization newrecord)
        {
            ViewBag.staffonbehalf = new SelectList(db.staffs.OrderBy(staffs => staffs.staff_email), "staffid", "staff_email");
            ViewBag.nnon1 = new SelectList(db.staffs.OrderBy(staffs => staffs.staff_email), "staffid", "staff_email");
            ViewBag.non3 = new SelectList(db.staffs.OrderBy(staffs => staffs.staff_email), "staffid", "staff_email");

            //ViewBag.staffid = new SelectList(User.Identity.GetUserName() + "wfp.org");
            //var username = User.Identity.GetUserName();


            var staffss = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
            staffss = staffss.Where(s => s.staff_email == User.Identity.Name + "@wfp.org");
            var stafemal11 = staffss.First().staff_email;
            var staffforhouforsuper = staffss.First().staffid;
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            var latestmission2 = db.MissionAuthorizations.Include(s => s.MissionItineraries).Include(s => s.staff);
            latestmission2 = latestmission2.Where(s => s.staffid == staffforhouforsuper && s.non5 == 11);

            if (latestmission2.Any() == false)
            {
                newrecord.staffid = staffforhouforsuper;
                newrecord.non5 = 11;
                db.MissionAuthorizations.Add(newrecord);
                db.Configuration.ValidateOnSaveEnabled = false;
                db.SaveChanges();

                int newrow = newrecord.MissionID;

                ViewBag.MissionID22 = newrow;

            }


            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            var missionup = latestmission2.First().MissionID;
            ViewBag.MissionID22 = missionup;

            if (staffss.First().staff_supervisorid == null || staffss.First().staff_first_name == null || staffss.First().unit_id == null || staffss.First().unit_id == null)
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

                //db.SaveChanges();

                var SuperForEmpM = db.EMPLOYEES.Include(s => s.staff);
                SuperForEmpM = SuperForEmpM.Where(s => s.EMP_ID == managerindex);
                var mangerEmail = SuperForEmpM.First().EMAIL_ADDRESS;



                var staffscompmid = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                staffscompmid = staffscompmid.Where(s => s.staff_email == mangerEmail);
                var managerid = staffscompmid.First().staffid;


                var staffmanagerupdate1 = db.staffs.Single(u => u.staff_email == stafemal11);
                staffmanagerupdate1.staff_supervisorid = managerid;
                //db.SaveChanges();

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
                    //db.SaveChanges();

                }
                else
                {

                    var addunit = db.units.Add(new unit { unit_description_english = unitnameforemp }).unit_description_english;
                    //db.SaveChanges();



                    var selectnewunit = db.units.Include(s => s.Country_office);
                    selectnewunit = selectnewunit.Where(s => s.unit_description_english == unitnameforemp);
                    var newunitid = selectnewunit.First().unitid;

                    var houfornewunit = db.units.Single(u => u.unitid == newunitid);
                    houfornewunit.HOU_ID = houidEmp;
                    //db.SaveChanges();

                    var staffudateunitid = db.staffs.Single(u => u.staff_email == stafemal11);
                    staffudateunitid.unit_id = newunitid;
                    //db.SaveChanges();

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

            var superid = staffss.First().staffid;


            if (staffss.First().staff_supervisorid == null)
            {
                staffss.First().staff_supervisorid = 1212;
                ViewBag.error = 1;
            }

            if (staffss.First().staff_first_name == null)
            {
                staffss.First().staff_first_name = "You must have First name !!!";
                ViewBag.error = 1;
            }

            if (staffss.First().staff_last_name == null)
            {
                staffss.First().staff_last_name = "You must have Last name !!!";
                ViewBag.error = 1;
            }


            //ViewBag.staffid = new SelectList(db.staffs, "staffs.First().staffid", staffs.First().staff_email);
            ViewBag.staffid = staffss.First().staffid;
            ViewBag.staffid2 = staffss.First().staff_first_name;
            ViewBag.FirstName = staffss.First().staff_first_name;
            ViewBag.LastName = staffss.First().staff_last_name;
           
            ViewBag.StaffTitle = staffss.First().functional_title.functional_title_description_english;
            ViewBag.contractType = staffss.First().contract_type.contract_type_name;



            ViewBag.StaffTitle = staffss.First().functional_title.functional_title_description_english;
            ViewBag.contractType = staffss.First().contract_type.contract_type_name;

            if (staffss.First().unit_id == null)
            {
                ViewBag.Unit = "You must have a Unit !!!!";
                staffss.First().unit_id = 15;
                ViewBag.error = 1;

            }
            else
            {

                ViewBag.Unit = staffss.First().unit.unit_description_english;
            }
            var staffsupervisor = staffss.First().staff_supervisorid;

            var userUnit = staffss.First().unit_id;

            var supers = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
            supers = supers.Where(d => d.staffid == staffsupervisor);





            ViewBag.supervisor = (supers.First().staff_first_name + " " + supers.First().staff_last_name);


            var supervisoremailif = supers.First().staff_email;
            if (supervisoremailif == "muhannad.hadi@wfp.org")
            {
                supervisoremailif = "rbc.management@wfp.org";//rbc.management@wfp.org
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

            if (houmailUPDATE == "muhannad.hadi@wfp.org")
            {
                houmailUPDATE = "rbc.management@wfp.org";//rbc.management@wfp.org

            }


            ViewBag.FromID = new SelectList(db.countries, "countryid", "country_name");
            ViewBag.ToID = new SelectList(db.countries, "countryid", "country_name");
            ViewBag.MissionID = new SelectList(db.MissionAuthorizations, "MissionID", "MissionID");
            ViewBag.destinationCountry = new SelectList(db.countries, "countryid", "country_name");
            ViewBag.Mission_Type = new SelectList(db.MissionTypes, "ID", "Name");



            return View();
        }

        // POST: MissionAuthorizations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateOnbehalf(HttpPostedFileBase files, [Bind(Include = "MissionID,staffid,funding,ClearedBySupervisor,MissionObjective,ExpectedOutputs,FollowUp,SignatureDate,canbedone,StaffSignature,staffonbehalf,nnon1,non3,Mission_Type,destinationCountry,destinationTwon,UberCareem,non18,non17,fils")] MissionAuthorization missionAuthorization)
        {

            if (missionAuthorization.staffonbehalf == null || missionAuthorization.nnon1 == null || missionAuthorization.non3 == null)
            {

                ModelState.AddModelError("staffid2", "Name is required");
                ViewBag.staffid2 = "Name is required";

            }


            if (ModelState.IsValid)
            {
                db.Entry(missionAuthorization).State = EntityState.Modified;
                db.SaveChanges();

                //db.MissionAuthorizations.Add(missionAuthorization);

                //        db.SaveChanges();
                if (files != null)
                {

                    string path = Path.Combine(Server.MapPath("~/UploadedFiles"), files.FileName);
                    files.SaveAs(path);
                    var addmissionAuthorizationJPG = db.MissionAuthorizations.Single(u => u.MissionID == missionAuthorization.MissionID);
                    addmissionAuthorizationJPG.files = files.FileName;
                    db.SaveChanges();

                }
                   
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



                ViewBag.staffid = staffs.First().staffid;
                ViewBag.staffid2 = staffs.First().staff_first_name;
                ViewBag.FirstName = staffs.First().staff_first_name;
                ViewBag.LastName = staffs.First().staff_last_name;
                ViewBag.StaffTitle = staffs.First().functional_title.functional_title_description_english;
                ViewBag.contractType = staffs.First().contract_type.contract_type_name;

                var missionitinarry = db.MissionItineraries.Include(i => i.MissionAuthorization);
                missionitinarry = missionitinarry.Where(i => i.MissionID == missionAuthorization.MissionID);




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
                    if (missionAuthorization.ClearedBySupervisor == true)
                    {
                        CleredBySupervisor = "Yes";
                    }
                    else
                    {
                        CleredBySupervisor = "No";
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

                    var Days142 = "";
                    if (missionAuthorization.non18 == true)
                    {
                        Days142 = "Yes";
                    }
                    else
                    {
                        Days142 = "No";
                    }


                    //////////////////////////////


                    var supersforhou = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                    supersforhou = supersforhou.Where(d => d.staffid == staffsupervisor);
                    var superforhounitsname = supersforhou.First().staff_first_name + " " + staffshou.First().staff_last_name;
                    var houmailUPDATE = supersforhou.First().staff_email;

                    if (houmailUPDATE == "muhannad.hadi@wfp.org")
                    {
                        houmailUPDATE = "rbc.management@wfp.org";//rbc.management@wfp.org

                    }
                    /////////////////////////

                    MailMessage mailhou = new MailMessage();
                    mailhou.To.Add(houmailUPDATE); // RD/DRD OFFICE/rbc.management@wfp.org
                    //mail.CC.Add(cc);
                    mailhou.Bcc.Add("ahmed.badr@wfp.org");
                    mailhou.From = new MailAddress(from);
                    mailhou.Subject = "Mission Authorization Form";

                    string Bodyhou = "<table><tr><td colspan='2'>" + "<hr/>New Mission<hr/></td></tr><tr><td><font color='blue'>Request : </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorization.MissionID + "</td></tr>"
                    + "<tr><td>" + "<font color='blue'>From :</font></td><td> " + missionAuthorization.staff.staff_first_name + " " + missionAuthorization.staff.staff_last_name + " </td></tr>"
                        //+ "<tr><td>" + "<font color='blue'>Onbehalf: :</font></td><td> " + missionAuthorization.staff1.staff_first_name + " " + missionAuthorization.staff1.staff_last_name + " </td></tr>"
                    + "<tr><td>" + "<font color='blue'>Unit :</font></td><td> " + missionAuthorization.staff.unit.unit_abreviation_english + "</td></tr>"
                    + "<tr><td>" + "<font color='blue'>Title :</font></td><td> " + missionAuthorization.staff.functional_title.functional_title_description_english + " </td></tr>"
                    + "<tr><td>" + "<font color='blue'>Contract Type :</font></td><td> " + missionAuthorization.staff.contract_type.contract_type_name + " </td></tr>"

                    //+ "<tr><td><font color='blue'>Official Start Date :</font></td><td>" + missionAuthorization.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                        //+ "<tr><td><font color='blue'>Official End Date :</font></td><td>" + missionAuthorization.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                        //+ "<tr><td><font color='blue'>Reporting Time :</font></td><td>" + missionAuthorization.FromTime.Value.ToString("HH:mm") + "</td></tr>"
                        // + "<tr><td><font color='blue'>End Time :</font></td><td>" + missionAuthorization.ToTime.Value.ToString("HH:mm") + "</td></tr>"
                   + "<tr><td colspan='2'>" + "<hr/>Mission Itinerary <hr/>" + "</td></tr>";

                    foreach (var item in missionitinarry)
                    {

                        Bodyhou = Bodyhou

                           + "<tr><td><font color='red'>Departure:</font></td><td></td></tr>"
                           + "<tr><td><font color='blue'>Country:</font></td><td>" + item.country.country_name + " / " + item.Twon1 + "</td></tr>"

                           + "<tr><td><font color='blue'>Date/Time :</font></td><td>" + item.FromDate.Value.ToString("dd/MMM/yy") + "---" + item.Fromtime.Value.ToString("HH:mm") + "</td></tr>"
                           + "<tr><td><font color='red'>Arrival:</font></td><td></td></tr>"
                           + "<tr><td><font color='blue'>Country/City:</font></td><td>" + item.country1.country_name + " / " + item.Twon2 + "</td></tr>"

                           + "<tr><td><font color='blue'>Date/Time :</font></td><td>" + item.ToDate.Value.ToString("dd/MMM/yy") + "---" + item.Totime.Value.ToString("HH:mm") + "</td></tr>"
                           + "<tr><hr/>";
                    }
                    Bodyhou = Bodyhou

       + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
             //+ "<tr><td><font color='blue'>Mission Type :</font></td><td>" + missionAuthorization.MissionType.Name + "</td></tr>"
             + "<tr><td><font color='blue'>Attached Administrative Note :</font></td><td><a href='http://10.11.135.254:8080/SMS/UploadedFiles/" + missionAuthorization.files + "'>" + missionAuthorization.files + "</a></td></tr>"
             + "<tr><td><font color='blue'>Funding :</font></td><td>" + missionAuthorization.funding + "</td></tr>"
             + "<tr><td><font color='blue'>Objective :</font></td><td>" + missionAuthorization.MissionObjective + "</td></tr>"
             + "<tr><td><font color='blue'>Expected Outputs :</font></td><td>" + missionAuthorization.ExpectedOutputs + "</td></tr>"
             + "<tr><td><font color='blue'>Was The Ticket Requested 14 Days In Advance? :</font></td><td>" + Days142 + "</td></tr>"
             + "<tr><td><font color='blue'>If Not, Please Explain :</font></td><td>" + missionAuthorization.non17 + "</td></tr>"
                        //+ "<tr><td><font color='blue'>Mission Destination :</font></td><td>" + missionAuthorization.country.country_name + " " + missionAuthorization.destinationTwon + "</td></tr>"
             + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video ? :</font></td><td>" + canbedone + "</td></tr>"
             + "<tr><td><font color='blue'>Cleared By Supervisor :</font></td><td>" + CleredBySupervisor + "</td></tr>"
             + "<tr><td><font color='blue'>International Roaming :</font></td><td>" + Roming + "</td></tr>"
             + "<tr><td><font color='blue'>Comment :</font></td><td>" + missionAuthorization.FollowUp + "</td></tr>";

                    mailhou.Body = Bodyhou;
                    mailhou.IsBodyHtml = true;
                    SmtpClient smtphou = new SmtpClient();
                    smtphou.Host = "smtprelay.global.wfp.org";
                    smtphou.Port = 25;
                    smtphou.UseDefaultCredentials = true;
                    //smtp.Credentials = new System.Net.NetworkCredential
                    //("ahmed.badr", "Survivor2323");// Enter seders User name and password  
                    //smtp.EnableSsl = true;
                    //smtphou.Send(mailhou);


                    return RedirectToAction("Index");


                }



                if (to == "muhannad.hadi@wfp.org")
                {
                    to = "rbc.management@wfp.org";//rbc.management@wfp.org

                }


                var canbedoneee = "";
                if (missionAuthorization.canbedone == true)
                {
                    canbedoneee = "Yes";
                }
                else
                {
                    canbedoneee = "No";
                }

                var CleredBySuper = "";
                if (missionAuthorization.ClearedBySupervisor == true)
                {
                    CleredBySuper = "Yes";
                }
                else
                {
                    CleredBySuper = "No";
                }

                MailMessage mail = new MailMessage();
                mail.To.Add(to); // var to
                //mail.CC.Add(cc);
                mail.Bcc.Add("ahmed.badr@wfp.org");
                mail.From = new MailAddress(from);
                mail.Subject = "Mission Authorization Form";

                string Body = "<table><tr><td colspan='2'>" + "<hr/>New Mission<hr/></td></tr><tr><td><font color='blue'>Request: </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorization.MissionID + "</td></tr>"
                        + "<tr><td>" + "<font color='blue'>From:</font></td><td>" + staffs.First().staff_first_name + " " + staffs.First().staff_last_name + "</td></tr>"
                        + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                        //+ "<tr><td><font color='blue'>From:</font></td><td>" + missionAuthorization.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                        //+ "<tr><td><font color='blue'>To:</font></td><td>" + missionAuthorization.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                        + "<tr><td><font color='blue'>Funding:</font></td><td>" + missionAuthorization.funding + "</td></tr>"
                        + "<tr><td><font color='blue'>Objective:</font></td><td>" + missionAuthorization.MissionObjective + "</td></tr>"
                        + "<tr><td><font color='blue'>Mission Itinerary:</font></td><td>" + missionAuthorization.MissionItinerary + "</td></tr>"
                        + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video?:</font></td><td>" + canbedoneee + "</td></tr>"
                        + "<tr><td><font color='blue'>Cleared By Supervisor:</font></td><td>" + CleredBySuper + "</td></tr>"
                        + "<tr><td><a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid="
                        + missionAuthorization.MissionID
                        + "&RDORDRDSignature=Yes&FromDate=" + missionAuthorization.FromDate
                        + " &ToDate=" + missionAuthorization.ToDate
                        + " &to=" + to + "&staffmail=" + staffs.First().staff_email
                        + "'>Accept</a></td></tr><tr><td><a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization.MissionID
                        + "&RDORDRDSignature=NO & FromDate=" + missionAuthorization.FromDate
                        + " &ToDate=" + missionAuthorization.ToDate
                        + "&to=" + to
                        + "&staffmail=" + staffs.First().staff_email
                        + "'>Reject</a>" + "</td></tr></table>";
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


                return RedirectToAction("Index");


                //    }

                //}

            }

            //ViewBag.staffid = User.Identity.GetUserName() + "@wfp.org";
            return RedirectToAction("CreateOnbehalf");
        }


        //////////////////////////////// end create on behalf

        ///////////////////////////////  start of edit on behalf

        public ActionResult EditBehalfof(int? id, int? id2, int? drd, int? loginid, int? behalfOf)
        {

           

            var staffs1 = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
            staffs1 = staffs1.Where(s => s.staff_email.Contains(User.Identity.Name + "@wfp.org"));
            var staffforhouforsuper = staffs1.First().staffid;
            //ViewBag.staffid = new SelectList(db.staffs, "staffs.First().staffid", staffs.First().staff_email);
            ViewBag.staffid22 = staffs1.First().staffid;

          




            MissionAuthorization missionAuthorization = db.MissionAuthorizations.Find(id);
            if (missionAuthorization == null)
            {
                return HttpNotFound();
            }

            ViewBag.staffid = staffs1.First().staffid;
            ViewBag.destinationCountry = new SelectList(db.countries, "countryid", "country_name", missionAuthorization.destinationCountry);
            ViewBag.Mission_Type = new SelectList(db.MissionTypes, "ID", "Name", missionAuthorization.Mission_Type);



            ///////////////////////////////////////////            ////////////



            if (staffs1.First().unit_id == null)
            {
                ViewBag.Unit = "You must have a Unit !!!!";
                staffs1.First().unit_id = 15;
                ViewBag.error = 1;

            }
            else
            {

                ViewBag.Unit = staffs1.First().unit.unit_description_english;
            }
            var staffsupervisor = staffs1.First().staff_supervisorid;

            var userUnit = staffs1.First().unit_id;

            var supers = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
            supers = supers.Where(d => d.staffid == staffsupervisor);


            ViewBag.staffonbehalf = new SelectList(db.staffs, "staffid", "staff_email", missionAuthorization.staffonbehalf);
            ViewBag.nnon1 = new SelectList(db.staffs, "staffid", "staff_email", missionAuthorization.nnon1);
            ViewBag.non3 = new SelectList(db.staffs, "staffid", "staff_email", missionAuthorization.non3);
            ViewBag.destinationCountry = new SelectList(db.countries, "countryid", "country_name", missionAuthorization.destinationCountry);
            ViewBag.Mission_Type = new SelectList(db.MissionTypes, "ID", "Name", missionAuthorization.Mission_Type);


         

       

            ViewBag.supervisor = (supers.First().staff_first_name + " " + supers.First().staff_last_name);


            var supervisoremailif = supers.First().staff_email;
            if (supervisoremailif == "muhannad.hadi@wfp.org")
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

            if (houmailUPDATE == "muhannad.hadi@wfp.org")
            {
                houmailUPDATE = "rbc.management@wfp.org"; //rbc.management@wfp.org

            }


            ViewBag.houmail = houmailUPDATE;


            //////////////////////////////////////////////////////////////////////

            ViewBag.FromDate = missionAuthorization.FromDate.ToString();
            ViewBag.ToDate = missionAuthorization.ToDate.ToString();
            ViewBag.FromTime = missionAuthorization.FromTime.ToString();
            ViewBag.ToTime = missionAuthorization.ToTime.ToString();
            ViewBag.staffid = staffs1.First().staffid;
            ViewBag.staffid2 = staffs1.First().staff_first_name;
            ViewBag.FirstName = staffs1.First().staff_first_name;
            ViewBag.LastName = staffs1.First().staff_last_name;
            ViewBag.StaffTitle = staffs1.First().functional_title.functional_title_description_english;
            ViewBag.contractType = staffs1.First().contract_type.contract_type_name; 

            return View(missionAuthorization);
        }

        // POST: MissionAuthorizations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditBehalfof(HttpPostedFileBase files, [Bind(Include = "MissionID,staffid,FromDate,ToDate,MissionItinerary,funding,ClearedBySupervisor,MissionObjective,ExpectedOutputs,Achievements,FollowUp,SignatureDate,canbedone,StaffSignature,FromTime,ToTime,Mission_Type,destinationCountry,destinationTwon,UberCareem,non18,non17,fils,staffonbehalf,nnon1,non3,files")] MissionAuthorization missionAuthorization)
        {



            if (ModelState.IsValid)
            {

                db.Entry(missionAuthorization).State = EntityState.Modified;
                db.SaveChanges();

                //db.MissionAuthorizations.Add(missionAuthorization);

                //        db.SaveChanges();
                if (files != null)
                {

                    string path = Path.Combine(Server.MapPath("~/UploadedFiles"), files.FileName);
                    files.SaveAs(path);
                    var addmissionAuthorizationJPG = db.MissionAuthorizations.Single(u => u.MissionID == missionAuthorization.MissionID);
                    addmissionAuthorizationJPG.files = files.FileName;
                    db.SaveChanges();

                }
                   
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

                var missionitinarry = db.MissionItineraries.Include(i => i.MissionAuthorization);
                missionitinarry = missionitinarry.Where(i => i.MissionID == missionAuthorization.MissionID);



                var cc = supers.First().staff_email;
                var supername = supers.First().staff_first_name + " " + supers.First().staff_last_name;


                if (staffsedit.First().staffid == houid)
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
                    if (missionAuthorization.ClearedBySupervisor == true)
                    {
                        CleredBySupervisor = "Yes";
                    }
                    else
                    {
                        CleredBySupervisor = "No";
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

                    var Days142 = "";
                    if (missionAuthorization.non18 == true)
                    {
                        Days142 = "Yes";
                    }
                    else
                    {
                        Days142 = "No";
                    }

                    var supersforhou = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                    supersforhou = supersforhou.Where(d => d.staffid == staffsupervisor);
                    var superforhounitsname = supersforhou.First().staff_first_name + " " + staffshou.First().staff_last_name;
                    var houmailUPDATE = supersforhou.First().staff_email;

                    if (houmailUPDATE == "muhannad.hadi@wfp.org")
                    {
                        houmailUPDATE = "rbc.management@wfp.org";//rbc.management@wfp.org

                    }


                    MailMessage mailhou = new MailMessage();
                    mailhou.To.Add("ahmed.badr@wfp.org"); // RD/DRD OFFICE/rbc.management@wfp.org
                    mailhou.Bcc.Add("ahmed.badr@wfp.org");
                    mailhou.From = new MailAddress(from);
                    mailhou.Subject = "Mission Updated";

                    string Bodyhou = "<table><tr><td colspan='2'>" + "<hr/>New Mission<hr/></td></tr><tr><td><font color='blue'>Request : </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorization.MissionID + "</td></tr>"
                   + "<tr><td>" + "<font color='blue'>From :</font></td><td> " + missionAuthorization.staff.staff_first_name + " " + missionAuthorization.staff.staff_last_name + " </td></tr>"
                        //+ "<tr><td>" + "<font color='blue'>Onbehalf: :</font></td><td> " + missionAuthorization.staff1.staff_first_name + " " + missionAuthorization.staff1.staff_last_name + " </td></tr>"
                   + "<tr><td>" + "<font color='blue'>Unit :</font></td><td> " + missionAuthorization.staff.unit.unit_abreviation_english + "</td></tr>"
                   + "<tr><td>" + "<font color='blue'>Title :</font></td><td> " + missionAuthorization.staff.functional_title.functional_title_description_english + " </td></tr>"
                   + "<tr><td>" + "<font color='blue'>Contract Type :</font></td><td> " + missionAuthorization.staff.contract_type.contract_type_name + " </td></tr>"

                   //+ "<tr><td><font color='blue'>Official Start Date :</font></td><td>" + missionAuthorization.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                        //+ "<tr><td><font color='blue'>Official End Date :</font></td><td>" + missionAuthorization.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                        //+ "<tr><td><font color='blue'>Reporting Time :</font></td><td>" + missionAuthorization.FromTime.Value.ToString("HH:mm") + "</td></tr>"
                        // + "<tr><td><font color='blue'>End Time :</font></td><td>" + missionAuthorization.ToTime.Value.ToString("HH:mm") + "</td></tr>"
                  + "<tr><td colspan='2'>" + "<hr/>Mission Itinerary <hr/>" + "</td></tr>";

                    foreach (var item in missionitinarry)
                    {

                        Bodyhou = Bodyhou

                           + "<tr><td><font color='red'>Departure:</font></td><td></td></tr>"
                           + "<tr><td><font color='blue'>Country:</font></td><td>" + item.country.country_name + " / " + item.Twon1 + "</td></tr>"

                           + "<tr><td><font color='blue'>Date/Time :</font></td><td>" + item.FromDate.Value.ToString("dd/MMM/yy") + "---" + item.Fromtime.Value.ToString("HH:mm") + "</td></tr>"
                           + "<tr><td><font color='red'>Arrival:</font></td><td></td></tr>"
                           + "<tr><td><font color='blue'>Country/City:</font></td><td>" + item.country1.country_name + " / " + item.Twon2 + "</td></tr>"

                           + "<tr><td><font color='blue'>Date/Time :</font></td><td>" + item.ToDate.Value.ToString("dd/MMM/yy") + "---" + item.Totime.Value.ToString("HH:mm") + "</td></tr>"
                           + "<tr><hr/>";
                    }
                    Bodyhou = Bodyhou

       + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
             //+ "<tr><td><font color='blue'>Mission Type :</font></td><td>" + missionAuthorization.MissionType.Name + "</td></tr>"
             + "<tr><td><font color='blue'>Attached Administrative Note :</font></td><td><a href='http://10.11.135.254:8080/SMS/UploadedFiles/" + missionAuthorization.files + "'>" + missionAuthorization.files + "</a></td></tr>"
             + "<tr><td><font color='blue'>Funding :</font></td><td>" + missionAuthorization.funding + "</td></tr>"
             + "<tr><td><font color='blue'>Objective :</font></td><td>" + missionAuthorization.MissionObjective + "</td></tr>"
             + "<tr><td><font color='blue'>Expected Outputs :</font></td><td>" + missionAuthorization.ExpectedOutputs + "</td></tr>"
             + "<tr><td><font color='blue'>Was The Ticket Requested 14 Days In Advance? :</font></td><td>" + Days142 + "</td></tr>"
             + "<tr><td><font color='blue'>If Not, Please Explain :</font></td><td>" + missionAuthorization.non17 + "</td></tr>"
                        //+ "<tr><td><font color='blue'>Mission Destination :</font></td><td>" + missionAuthorization.country.country_name + " " + missionAuthorization.destinationTwon + "</td></tr>"
             + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video ? :</font></td><td>" + canbedone + "</td></tr>"
             + "<tr><td><font color='blue'>Cleared By Supervisor :</font></td><td>" + CleredBySupervisor + "</td></tr>"
             + "<tr><td><font color='blue'>International Roaming :</font></td><td>" + Roming + "</td></tr>"
             + "<tr><td><font color='blue'>Comment :</font></td><td>" + missionAuthorization.FollowUp + "</td></tr>";

                    mailhou.Body = Bodyhou;
                    mailhou.IsBodyHtml = true;
                    SmtpClient smtphou = new SmtpClient();
                    smtphou.Host = "smtprelay.global.wfp.org";
                    smtphou.Port = 25;
                    smtphou.UseDefaultCredentials = true;
                    smtphou.Send(mailhou);//-

                    return RedirectToAction("Index");


                }

                var canbedoneee = "";
                if (missionAuthorization.canbedone == true)
                {
                    canbedoneee = "Yes";
                }
                else
                {
                    canbedoneee = "No";
                }

                var CleredBySupe2 = "";
                if (missionAuthorization.ClearedBySupervisor == true)
                {
                    CleredBySupe2 = "Yes";
                }
                else
                {
                    CleredBySupe2 = "No";
                }

                MailMessage mailhou22 = new MailMessage();
                mailhou22.To.Add("ahmed.badr@wfp.org"); // RD/DRD OFFICE/rbc.management@wfp.org
                mailhou22.Bcc.Add("ahmed.badr@wfp.org");
                mailhou22.From = new MailAddress(from);
                mailhou22.Subject = "Mission Updated";

                string Bodyhou22 = "<table><tr><td colspan='2'>" + "<hr/>Updated Mission<hr/></td></tr><tr><td><font color='blue'>Request: </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorization.MissionID + "</td></tr>"
                    + "<tr><td>" + "<font color='blue'>From:</font></td><td>" + staffsedit.First().staff_first_name + " " + staffsedit.First().staff_last_name + "</td></tr>"
                    + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                    //+ "<tr><td><font color='blue'>From:</font></td><td>" + missionAuthorization.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                    //+ "<tr><td><font color='blue'>To:</font></td><td>" + missionAuthorization.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                    + "<tr><td><font color='blue'>Funding:</font></td><td>" + missionAuthorization.funding + "</td></tr>"
                    + "<tr><td><font color='blue'>Objective:</font></td><td>" + missionAuthorization.MissionObjective + "</td></tr>"
                    + "<tr><td><font color='blue'>Mission Itinerary:</font></td><td>" + missionAuthorization.MissionItinerary + "</td></tr>"
                    + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video?:</font></td><td>" + canbedoneee + "</td></tr>"
                    + "<tr><td><font color='blue'>Cleared By Supervisor:</font></td><td>" + CleredBySupe2 + "</td></tr>"
                    + "<tr><td><a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid="
                    + missionAuthorization.MissionID
                    + "&RDORDRDSignature=Yes&FromDate=" + missionAuthorization.FromDate
                    + " &ToDate=" + missionAuthorization.ToDate
                    + " &to=" + to + "&staffmail=" + staffsedit.First().staff_email
                    + "'>Accept</a></td></tr><tr><td><a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization.MissionID
                    + "&RDORDRDSignature=NO & FromDate=" + missionAuthorization.FromDate
                    + " &ToDate=" + missionAuthorization.ToDate
                    + "&to=" + to
                    + "&staffmail=" + staffsedit.First().staff_email
                    + "'>Reject</a>" + "</td></tr></table>";

                mailhou22.Body = Bodyhou22;
                mailhou22.IsBodyHtml = true;
                SmtpClient smtphou22 = new SmtpClient();
                smtphou22.Host = "smtprelay.global.wfp.org";
                smtphou22.Port = 25;
                smtphou22.UseDefaultCredentials = true;
                smtphou22.Send(mailhou22);//-

                return RedirectToAction("Index");

            }

            var staffs = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
            staffs = staffs.Where(s => s.staff_email.Contains(User.Identity.Name + "@wfp.org"));

            ViewBag.staffid = staffs.First().staffid;
            ViewBag.FirstNme = staffs.First().staff_first_name;
            ViewBag.LastName = staffs.First().staff_last_name;






            //    }

            //}



            return View(missionAuthorization);
        }



        public ActionResult IndexItineraryOnbehalf(string sortOrder, string currentFilter, string searchString, int? page, string node)
        {



            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = String.IsNullOrEmpty(sortOrder) ? "First_Name" : "";
            var students = from s in db.MissionItineraries
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

            var latestmission2 = db.MissionAuthorizations.Include(s => s.MissionItineraries).Include(s => s.staff);
            latestmission2 = latestmission2.Where(s => s.staffid == staffid && s.non5 == 11);

            if (latestmission2.Any() == false)
            {
                var missionItineraries22 = db.MissionItineraries.Include(m => m.country).Include(m => m.country1).Include(m => m.MissionAuthorization);
                missionItineraries22 = missionItineraries22.Where(u => u.non3 == staffid);

            }

            var missionItineraries = db.MissionItineraries.Include(m => m.country).Include(m => m.country1).Include(m => m.MissionAuthorization);
            missionItineraries = missionItineraries.Where(u => u.non3 == staffid && u.MissionID == latestmission2.FirstOrDefault().MissionID);
            ViewBag.id = latestmission2.FirstOrDefault().MissionID;


            if (searchString != null)
            {

                missionItineraries = missionItineraries.Where(s => s.Description == (searchString)
                                       || s.Description == (searchString));

            }

            switch (sortOrder)
            {
                case "First_Name":
                    missionItineraries = missionItineraries.OrderByDescending(s => s.Description);
                    break;
                case "name_desc":
                    missionItineraries = missionItineraries.OrderByDescending(s => s.MissionID);
                    break;

                default:  // Name ascending 
                    missionItineraries = missionItineraries.OrderBy(s => s.MissionID);
                    break;
            }


            ViewBag.rdDrd = 1;
            ViewBag.FromID = new SelectList(db.countries, "countryid", "country_name");
            ViewBag.ToID = new SelectList(db.countries, "countryid", "country_name");
            ViewBag.MissionID = new SelectList(db.MissionAuthorizations, "MissionID", "MissionID");


            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(missionItineraries.ToPagedList(pageNumber, pageSize));

        }

        public ActionResult IndexItineraryeditbehalfof(string sortOrder, string currentFilter, string searchString, int? page, string node,int? id)
        {



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

            var students = from s in db.MissionItineraries
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

            var missionItineraries = db.MissionItineraries.Include(m => m.country).Include(m => m.country1).Include(m => m.MissionAuthorization);
            missionItineraries = missionItineraries.Where(s => s.MissionID == (id));

            if (!String.IsNullOrEmpty(searchString))
            {
                missionItineraries = missionItineraries.Where(s => s.Equals(searchString)
                                       || s.country.Equals(searchString));

            }

            switch (sortOrder)
            {
                case "First_Name":
                    missionItineraries = missionItineraries.OrderByDescending(s => s.FromDate);
                    break;
                case "name_desc":
                    missionItineraries = missionItineraries.OrderByDescending(s => s.FromDate);
                    break;

                default:  // Name ascending 
                    missionItineraries = missionItineraries.OrderBy(s => s.FromDate);
                    break;
            }


            ViewBag.rdDrd = 1;
            ViewBag.FromID = new SelectList(db.countries, "countryid", "country_name");
            ViewBag.ToID = new SelectList(db.countries, "countryid", "country_name");
            ViewBag.MissionID = new SelectList(db.MissionAuthorizations, "MissionID", "MissionID");
            ViewBag.id22 = id;

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(missionItineraries.ToPagedList(pageNumber, pageSize));
            //return View();

        }

        public ActionResult CreateItineraryOnbehalf(MissionAuthorization newrecord, int? MissionID)
        {


            ViewBag.FromID = new SelectList(db.countries, "countryid", "country_name");
            ViewBag.ToID = new SelectList(db.countries, "countryid", "country_name");

            var staffser = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
            staffser = staffser.Where(s => s.staff_email.Contains(User.Identity.Name + "@wfp.org"));
            var staffid = staffser.First().staffid;

            //newrecord.staffid = staffid;
            //db.MissionAuthorizations.Add(newrecord);
            //db.SaveChanges();
            //int newrow = newrecord.MissionID;


            var latestmission2 = db.MissionAuthorizations.Include(s => s.MissionItineraries).Include(s => s.staff);
            latestmission2 = latestmission2.Where(s => s.staffid == staffid && s.non5 == 11);
            //ViewBag.MissionID = latestmission2.First().MissionID;
            //var staffmissions = db.MissionAuthorizations.Include(d => d.staff);
            //staffmissions = staffmissions.Where(u => u.staffid == staffid);



            //ViewBag.MissionID = new SelectList(staffmissions, "MissionID", "MissionID");
            if (latestmission2.Any())
            {
                ViewBag.MissionID = latestmission2.First().MissionID;
            }
            ViewBag.non3 = staffser.First().staffid;
            return View();
        }

        [HttpPost, ActionName("CreateItineraryOnbehalf")]

        public ActionResult CreateItineraryOnbehalf([Bind(Include = "ID,MissionID,Description,FromID,FromDate,ToID,ToDate,non1,non2,non3,non4,Fromtime,Totime,Twon1,Twon2")] MissionItinerary missionItinerary)
        {
            if (ModelState.IsValid)
            {


                db.MissionItineraries.Add(missionItinerary);
                db.SaveChanges();
                return RedirectToAction("CreateOnbehalf");
            }

            ViewBag.FromID = new SelectList(db.countries, "countryid", "country_name", missionItinerary.FromID);
            ViewBag.ToID = new SelectList(db.countries, "countryid", "country_name", missionItinerary.ToID);
            ViewBag.MissionID = new SelectList(db.MissionAuthorizations, "MissionID", "MissionID", missionItinerary.MissionID);
            //return View(missionItinerary);
            return RedirectToAction("CreateOnbehalf");
            //return PartialView("_createiti");
        }


        // GET: MissionItineraries/Delete/5
        public ActionResult DeleteItineraryOnbehalf(int? id)
        {

            var staffser = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
            staffser = staffser.Where(s => s.staff_email.Contains(User.Identity.Name + "@wfp.org"));
            var staffid = staffser.First().staffid;

            var itineryID = db.MissionItineraries.Include(d => d.MissionAuthorization);
            itineryID = itineryID.Where(u => u.non3 == staffid && u.MissionID == id);


            //ViewBag.ID2 = new SelectList(itineryID, "ID", "ID");
            ViewBag.ID = new SelectList(itineryID, "ID", "ID");
            //ViewBag.ID = new SelectList(db.MissionItineraries, "ID", "ID");

            return View();
        }

        // POST: MissionItineraries/Delete/5
        [HttpPost, ActionName("DeleteItineraryOnbehalf")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed33(int id)
        {
            MissionItinerary missionItinerary = db.MissionItineraries.Find(id);
            db.MissionItineraries.Remove(missionItinerary);
            db.SaveChanges();
            return RedirectToAction("CreateOnbehalf");
        }

        //////////////////////////////// end of edit on behalf



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
            if (drd.ToString() == "1")
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

                if (missionAuthorization.RDORDRDSignature == "Yes")
                {
                    var staffsemail22 = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                    staffsemail22 = staffsemail22.Where(s => s.staffid == missionAuthorization.staffid);

                    var staffemail2 = staffsemail22.First().staff_email;
                    var staffsupervisor = staffsemail22.First().staff_supervisorid;



                    var supers = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                    supers = supers.Where(d => d.staffid == staffsupervisor);

                    var cc = supers.First().staff_email;
                    var supername = supers.First().staff_first_name + " " + supers.First().staff_last_name;
                    var superemail = supers.First().staff_email;


                    var staffs = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                    staffs = staffs.Where(s => s.staff_email.Contains(staffemail2));




                    var userUnit = staffs.First().unit_id;
                    var unithou = db.units.Include(d => d.staffs);
                    unithou = unithou.Where(u => u.unitid == userUnit);
                    var houid = unithou.First().HOU_ID;

                    var staffshou = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                    staffshou = staffshou.Where(h => h.staffid == houid);
                    var hou = staffshou.First().staff_email;
                    //var houname = staffshou.First().staff_first_name + " " + staffshou.First().staff_last_name;

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
                    if (missionAuthorization.ClearedBySupervisor == true)
                    {
                        CleredBySupervisor = "Yes";
                    }
                    else
                    {
                        CleredBySupervisor = "No";
                    }

                    MailMessage mail = new MailMessage();
                    mail.To.Add(staffemail2);
                    mail.Bcc.Add("ahmed.badr@wfp.org");
                    mail.CC.Add(superemail);
                    mail.CC.Add("rbc.travel@wfp.org"); //travel unit
                    mail.CC.Add("mai.kenawi@wfp.org"); //travel unit
                    mail.CC.Add("amal.mohamed@wfp.org"); //travel unit
                    mail.CC.Add("ayten.el-sheikh@wfp.org"); //travel unit
                    mail.CC.Add("essraa.soliman@wfp.org"); //travel unit
                    mail.CC.Add("rbc.management@wfp.org"); //rd/drd office
                    mail.From = new MailAddress(hou);
                    mail.Subject = "New Mission Approved";
                    string Body = "<table><tr><td colspan='2'>" + "<hr/> Mission<hr/></td></tr><tr><td><font color='blue'>Request: </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorization.MissionID + "</td></tr>"
                            + "<tr><td>" + "<font color='blue'>From:</font></td><td>" + staffs.First().staff_first_name + " " + staffs.First().staff_last_name + "</td></tr>"
                            + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                            + "<tr><td><font color='blue'>From:</font></td><td>" + missionAuthorization.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                            + "<tr><td><font color='blue'>To:</font></td><td>" + missionAuthorization.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                            + "<tr><td><font color='blue'>Funding:</font></td><td>" + missionAuthorization.funding + "</td></tr>"
                            + "<tr><td><font color='blue'>Objective:</font></td><td>" + missionAuthorization.MissionObjective + "</td></tr>"
                            + "<tr><td><font color='blue'>Mission Itinerary:</font></td><td>" + missionAuthorization.MissionItinerary + "</td></tr>"
                            + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video?:</font></td><td>" + canbedone + "</td></tr>"
                            + "<tr><td><font color='blue'>Cleared By Supervisor:</font></td><td>" + CleredBySupervisor + "</td></tr></table>";

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

                var staffsemail22n = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                staffsemail22n = staffsemail22n.Where(s => s.staffid == missionAuthorization.staffid);

                var staffemail2n = staffsemail22n.First().staff_email;
                var staffsupervisorn = staffsemail22n.First().staff_supervisorid;



                var supersn = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                supersn = supersn.Where(d => d.staffid == staffsupervisorn);

                var ccn = supersn.First().staff_email;
                var supernamen = supersn.First().staff_first_name + " " + supersn.First().staff_last_name;
                var superemailn = supersn.First().staff_email;


                var staffsn = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                staffsn = staffsn.Where(s => s.staff_email.Contains(staffemail2n));




                var userUnitn = staffsn.First().unit_id;
                var unithoun = db.units.Include(d => d.staffs);
                unithoun = unithoun.Where(u => u.unitid == userUnitn);
                var houidn = unithoun.First().HOU_ID;

                var staffshoun = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                staffshoun = staffshoun.Where(h => h.staffid == houidn);
                var houn = staffshoun.First().staff_email;
                //var houname = staffshou.First().staff_first_name + " " + staffshou.First().staff_last_name;

                var canbedonee = "";
                if (missionAuthorization.canbedone == true)
                {
                    canbedonee = "Yes";
                }
                else
                {
                    canbedonee = "No";
                }

                var CleredBySupervisoror = "";
                if (missionAuthorization.ClearedBySupervisor == true)
                {
                    CleredBySupervisoror = "Yes";
                }
                else
                {
                    CleredBySupervisoror = "No";
                }

                MailMessage mailn = new MailMessage();
                mailn.To.Add(staffemail2n);
                mailn.Bcc.Add("ahmed.badr@wfp.org");
                mailn.CC.Add(superemailn);
                mailn.From = new MailAddress(houn);
                mailn.Subject = "New Mission Rejected";
                string Bodyn = "<table><tr><td colspan='2'>" + "<hr/> Mission<hr/></td></tr><tr><td><font color='blue'>Request: </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorization.MissionID + "</td></tr>"
                         + "<tr><td>" + "<font color='blue'>From:</font></td><td>" + staffsn.First().staff_first_name + " " + staffsn.First().staff_last_name + "</td></tr>"
                         + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                         + "<tr><td><font color='blue'>From:</font></td><td>" + missionAuthorization.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                         + "<tr><td><font color='blue'>To:</font></td><td>" + missionAuthorization.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                         + "<tr><td><font color='blue'>Funding:</font></td><td>" + missionAuthorization.funding + "</td></tr>"
                         + "<tr><td><font color='blue'>Objective:</font></td><td>" + missionAuthorization.MissionObjective + "</td></tr>"
                         + "<tr><td><font color='blue'>Mission Itinerary:</font></td><td>" + missionAuthorization.MissionItinerary + "</td></tr>"
                         + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video?:</font></td><td>" + canbedonee + "</td></tr>"
                         + "<tr><td><font color='blue'>Cleared By Supervisor:</font></td><td>" + CleredBySupervisoror + "</td></tr>"
                         + "<tr><td><font color='blue'>Approve/reject Comment:</font>" + missionAuthorization.StaffSignature + "</td></tr></table>";

                mailn.Body = Bodyn;
                mailn.IsBodyHtml = true;
                SmtpClient smtpn = new SmtpClient();
                smtpn.Host = "smtprelay.global.wfp.org";
                smtpn.Port = 25;
                smtpn.UseDefaultCredentials = true;
                //smtp.Credentials = new System.Net.NetworkCredential
                //("ahmed.badr", "Survivor2323");// Enter seders User name and password  
                //smtp.EnableSsl = true;
                smtpn.Send(mailn);
                return RedirectToAction("Index");
            }



            var staffsere = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
            staffsere = staffsere.Where(s => s.staff_email.Contains(User.Identity.Name + "@wfp.org"));

            ViewBag.staffid = staffsere.First().staffid;
            ViewBag.FirstNme = staffsere.First().staff_first_name;
            ViewBag.LastName = staffsere.First().staff_last_name;

            return RedirectToAction("SuperEdit");

            //return View(missionAuthorization);
        }







        // GET: MissionAuthorizations/Approval

        public ActionResult Approval(int? Mid, bool? ClearedBySupervisor, DateTime? FromDate, DateTime? ToDate, string staffmail, string supervisorname, int? onbehalfID, int? onbehalfSuperID, int? onbehalfHouID)
        {



            /////////////////////////////// start on behalf
            if (onbehalfID != null && onbehalfSuperID != null && onbehalfHouID != null )
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

                    var missionAuthorization = db.MissionAuthorizations.Single(u => u.MissionID == Mid);
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

                    var missionitinarry = db.MissionItineraries.Include(i => i.MissionAuthorization);
                        missionitinarry = missionitinarry.Where(i => i.MissionID  == Mid);


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
                        if (missionAuthorization.ClearedBySupervisor == true)
                        {
                            CleredBySupervisor = "Yes";
                        }
                        else
                        {
                            CleredBySupervisor = "No";
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
                        var Days14 = "";
                        if (missionAuthorization.non18 == true)
                        {
                            Days14 = "Yes";
                        }
                        else
                        {
                            Days14 = "No";
                        }
                        //////////////////////////////


                        var supersforhou = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                        supersforhou = supersforhou.Where(d => d.staffid == onbehalfSuperID);
                        var superforhounitsname = supersforhou.First().staff_first_name + " " + staffshou.First().staff_last_name;
                        var houmailUPDATE = supersforhou.First().staff_email;

                        if (houmailUPDATE == "muhannad.hadi@wfp.org")
                        {
                            houmailUPDATE = "rbc.management@wfp.org";//rbc.management@wfp.org

                        }
                        /////////////////////////
                        var onbehalfof = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                        onbehalfof = onbehalfof.Where(d => d.staffid == onbehalfID);
                        var onbehalfofName = onbehalfof.First().staff_first_name + " " + onbehalfof.First().staff_last_name;
                        var onbehalfofEmail = onbehalfof.First().staff_email;

                        ///////////////////////
                        if (from == "ahmed.moussa@wfp.org")
                        {
                            Response.Redirect("/RBC-SMS/account/login");
                        }
                        //////////////////////


                        MailMessage mailhou = new MailMessage();
                        mailhou.To.Add(houmailUPDATE); // RD/DRD OFFICE/rbc.management@wfp.org
                        //mailhou.CC.Add(cc);
                        mailhou.Bcc.Add("ahmed.badr@wfp.org");
                        mailhou.From = new MailAddress(from);
                        mailhou.Subject = "Mission approval requested" + " " + "on behalf of" + " " + onbehalfofName;

                        string Bodyhou = "<table><tr><td colspan='2'>" + "<hr/>New Mission<hr/></td></tr><tr><td><font color='blue'>Request: </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorization.MissionID + "</td></tr>"
                        + "<tr><td>" + "<font color='blue'>From:</font></td><td> " + staffs.First().staff_first_name + " " + staffs.First().staff_last_name + " </td></tr>"
                        + "<tr><td>" + "<font color='blue'>Unit:</font></td><td> " + staffs.First().unit.unit_abreviation_english + "</td></tr>"
                        + "<tr><td>" + "<font color='blue'>Title:</font></td><td> " + staffs.First().functional_title.functional_title_description_english + " </td></tr>"
                        + "<tr><td>" + "<font color='blue'>Contract Type:</font></td><td> " + staffs.First().contract_type.contract_type_name + " </td></tr>"


                            //+ "<tr><td><font color='blue'>Official Start Date :</font></td><td>" + missionAuthorization.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                            //+ "<tr><td><font color='blue'>Official End Date :</font></td><td>" + missionAuthorization.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                          + "<tr><td colspan='2'>" + "<hr/>Mission Itinerarys <hr/>" + "</td></tr>";

                        foreach (var item in missionitinarry)
                        {

                            Bodyhou = Bodyhou

                               + "<tr><td><font color='red'>Departure:</font></td><td></td></tr>"
                               + "<tr><td><font color='blue'>Country:</font></td><td>" + item.country.country_name + " / " + item.Twon1 + "</td></tr>"

                               + "<tr><td><font color='blue'>Date/Time :</font></td><td>" + item.FromDate.Value.ToString("dd/MMM/yy") + "---" + item.Fromtime.Value.ToString("HH:mm") + "</td></tr>"
                               + "<tr><td><font color='red'>Arrival:</font></td><td></td></tr>"
                               + "<tr><td><font color='blue'>Country/City:</font></td><td>" + item.country1.country_name + " / " + item.Twon2 + "</td></tr>"

                               + "<tr><td><font color='blue'>Date/Time:</font></td><td>" + item.ToDate.Value.ToString("dd/MMM/yy") + "---" + item.Totime.Value.ToString("HH:mm") + "</td></tr>"
                               + "<tr><hr/>";
                        }
                        Bodyhou = Bodyhou

                        + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                        + "<tr><td><font color='blue'>Mission Type:</font></td><td>" + missionAuthorization.MissionType.Name  + "</td></tr>"
                        + "<tr><td><font color='blue'>Attach Administrative Note :</font></td><td><a href='http://10.11.135.254:8080/SMS/UploadedFiles/" + missionAuthorization.files + "'>" + missionAuthorization.files + "</a></td></tr>"
                        + "<tr><td><font color='blue'>Funding:</font></td><td>" + missionAuthorization.funding + "</td></tr>"
                        + "<tr><td><font color='blue'>Objective:</font></td><td>" + missionAuthorization.MissionObjective + "</td></tr>"
                        + "<tr><td><font color='blue'>Expected Outputs:</font></td><td>" + missionAuthorization.ExpectedOutputs + "</td></tr>"
                        + "<tr><td><font color='blue'>Was The Ticket Requested 14 Days In Advance?:</font></td><td>" + Days14 + "</td></tr>"
                        + "<tr><td><font color='blue'>If Not, Please Explain:</font></td><td>" + missionAuthorization.non17 + "</td></tr>"
                        //+ "<tr><td><font color='blue'>Mission Destination:</font></td><td>" + missionAuthorization.country.country_name + " " + missionAuthorization.destinationTwon + "</td></tr>"
                        + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video?:</font></td><td>" + canbedone + "</td></tr>"
                        + "<tr><td><font color='blue'>Cleared By Supervisor:</font></td><td>" + CleredBySupervisor + "</td></tr>"
                        + "<tr><td><font color='blue'>International Roaming:</font></td><td>" + Roming + "</td></tr>"
                        + "<tr><td><font color='blue'>Comment:</font></td><td>" + missionAuthorization.FollowUp + "</td></tr>"
                        + "<tr><td><a href='http://10.11.135.254:8080/SMS/MissionAuthorizations/Approval2?Mid="
                        + missionAuthorization.MissionID
                        + "&RDORDRDSignature=Yes&FromDate=" + missionAuthorization.FromDate
                        + " &ToDate=" + missionAuthorization.ToDate
                        + " &to=" + to + "&staffmail=" + staffs.First().staff_email
                         + "&onbehalfID=" + onbehalfID
                            + "&onbehalfSuperID=" + onbehalfSuperID
                            + "&onbehalfHouID=" + onbehalfHouID
                        + "'>Accept</a></td><td><a href='http://10.11.135.254:8080/SMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization.MissionID
                        + "&RDORDRDSignature=NO & FromDate=" + missionAuthorization.FromDate
                        + " &ToDate=" + missionAuthorization.ToDate
                        + "&to=" + to
                        + "&staffmail=" + staffs.First().staff_email
                         + "&onbehalfID=" + onbehalfID
                            + "&onbehalfSuperID=" + onbehalfSuperID
                            + "&onbehalfHouID=" + onbehalfHouID
                        + "'>Reject</a>" + "</td></tr></table>";
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

                    if (to == "muhannad.hadi@wfp.org")
                    {
                        to = "rbc.management@wfp.org";//rbc.management@wfp.org

                    }


                    var canbedoneee = "";
                    if (missionAuthorization.canbedone == true)
                    {
                        canbedoneee = "Yes";
                    }
                    else
                    {
                        canbedoneee = "No";
                    }

                    var CleredBySuper = "";
                    if (missionAuthorization.ClearedBySupervisor == true)
                    {
                        CleredBySuper = "Yes";
                    }
                    else
                    {
                        CleredBySuper = "No";
                    }
                    var Roming2 = "";
                    if (missionAuthorization.StaffSignature == "True")
                    {
                        Roming2 = "Yes";
                    }
                    else
                    {
                        Roming2 = "No";
                    }
                    var Days142 = "";
                    if (missionAuthorization.non18 == true)
                    {
                        Days142 = "Yes";
                    }
                    else
                    {
                        Days142 = "No";
                    }
                    /////////////////////////////////////////////////////
                    if (from == "ahmed.moussa@wfp.org")
                    {
                        Response.Redirect("/RBC-SMS/account/login");
                    }
                    ///////////////////////////////////////////////////////
                    MailMessage mail = new MailMessage();
                    mail.To.Add(to); // var to
                  //  mail.CC.Add(cc);
                    mail.Bcc.Add("ahmed.badr@wfp.org");
                    mail.From = new MailAddress(from);
                    mail.Subject = "Mission approval requested on behalf of" + " " + onbehalfofName2;

                    string Body = "<table><tr><td colspan='2'>" + "<hr/>New Mission<hr/></td></tr><tr><td><font color='blue'>Request: </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorization.MissionID + "</td></tr>"
                        + "<tr><td>" + "<font color='blue'>From:</font></td><td> " + staffs.First().staff_first_name + " " + staffs.First().staff_last_name + " </td></tr>"
                        + "<tr><td>" + "<font color='blue'>Unit:</font></td><td> " + staffs.First().unit.unit_abreviation_english + "</td></tr>"
                        + "<tr><td>" + "<font color='blue'>Title:</font></td><td> " + staffs.First().functional_title.functional_title_description_english + " </td></tr>"
                        + "<tr><td>" + "<font color='blue'>Contract Type:</font></td><td> " + staffs.First().contract_type.contract_type_name + " </td></tr>"


                            //+ "<tr><td><font color='blue'>Official Start Date :</font></td><td>" + missionAuthorization.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                        //+ "<tr><td><font color='blue'>Official End Date :</font></td><td>" + missionAuthorization.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                          + "<tr><td colspan='2'>" + "<hr/>Mission Itinerarys <hr/>" + "</td></tr>";

                    foreach (var item in missionitinarry)
                    {

                        Body = Body

                           + "<tr><td><font color='red'>Departure:</font></td><td></td></tr>"
                           + "<tr><td><font color='blue'>Country:</font></td><td>" + item.country.country_name + " / " + item.Twon1 + "</td></tr>"

                           + "<tr><td><font color='blue'>Date/Time :</font></td><td>" + item.FromDate.Value.ToString("dd/MMM/yy") + "---" + item.Fromtime.Value.ToString("HH:mm") + "</td></tr>"
                           + "<tr><td><font color='red'>Arrival:</font></td><td></td></tr>"
                           + "<tr><td><font color='blue'>Country/City:</font></td><td>" + item.country1.country_name + " / " + item.Twon2 + "</td></tr>"

                           + "<tr><td><font color='blue'>Date/Time:</font></td><td>" + item.ToDate.Value.ToString("dd/MMM/yy") + "---" + item.Totime.Value.ToString("HH:mm") + "</td></tr>"
                           + "<tr><hr/>";
                    }
                    Body = Body

                     + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                        + "<tr><td><font color='blue'>Mission Type:</font></td><td>" + missionAuthorization.MissionType.Name + "</td></tr>"
                        + "<tr><td><font color='blue'>Attach Administrative Note:</font></td><td><a href='http://10.11.135.254:8080/SMS/UploadedFiles/" + missionAuthorization.files + "'>" + missionAuthorization.files + "</a></td></tr>"
                        + "<tr><td><font color='blue'>Funding:</font></td><td>" + missionAuthorization.funding + "</td></tr>"
                        + "<tr><td><font color='blue'>Objective:</font></td><td>" + missionAuthorization.MissionObjective + "</td></tr>"
                        + "<tr><td><font color='blue'>Expected Outputs:</font></td><td>" + missionAuthorization.ExpectedOutputs + "</td></tr>"
                        + "<tr><td><font color='blue'>Was The Ticket Requested 14 Days In Advance?:</font></td><td>" + Days142 + "</td></tr>"
                        + "<tr><td><font color='blue'>If Not, Please Explain:</font></td><td>" + missionAuthorization.non17 + "</td></tr>"
                        //+ "<tr><td><font color='blue'>Mission Destination :</font></td><td>" + missionAuthorization.country.country_name + " " + missionAuthorization.destinationTwon + "</td></tr>"
                        + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video?:</font></td><td>" + canbedoneee + "</td></tr>"
                        + "<tr><td><font color='blue'>Cleared By Supervisor:</font></td><td>" + CleredBySuper + "</td></tr>"
                        + "<tr><td><font color='blue'>International Roaming:</font></td><td>" + Roming2 + "</td></tr>"
                        + "<tr><td><font color='blue'>Comment:</font></td><td>" + missionAuthorization.FollowUp + "</td></tr>"
                        + "<tr><td><a href='http://10.11.135.254:8080/SMS/MissionAuthorizations/Approval2?Mid="
                        + missionAuthorization.MissionID
                        + "&RDORDRDSignature=Yes&FromDate=" + missionAuthorization.FromDate
                        + " &ToDate=" + missionAuthorization.ToDate
                        + " &to=" + to + "&staffmail=" + staffs.First().staff_email
                         + "&onbehalfID=" + onbehalfID
                            + "&onbehalfSuperID=" + onbehalfSuperID
                            + "&onbehalfHouID=" + onbehalfHouID
                        + "'>Accept</a></td><td><a href='http://10.11.135.254:8080/SMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization.MissionID
                        + "&RDORDRDSignature=NO & FromDate=" + missionAuthorization.FromDate
                        + " &ToDate=" + missionAuthorization.ToDate
                        + "&to=" + to
                        + "&staffmail=" + staffs.First().staff_email
                         + "&onbehalfID=" + onbehalfID
                            + "&onbehalfSuperID=" + onbehalfSuperID
                            + "&onbehalfHouID=" + onbehalfHouID
                        + "'>Reject</a>" + "</td></tr></table>";
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
                    smtp.Send(mail);




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
                    var staffs = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate).Include(s => s.country);
                    staffs = staffs.Where(s => s.staff_email.Contains(User.Identity.Name + "@wfp.org"));

                    var missionAuthorization = db.MissionAuthorizations.Single(u => u.MissionID == Mid);
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

                      var missionitinarry = db.MissionItineraries.Include(i => i.MissionAuthorization);
                        missionitinarry = missionitinarry.Where(i => i.MissionID == Mid);


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
                        if (missionAuthorization.ClearedBySupervisor == true)
                        {
                            CleredBySupervisor = "Yes";
                        }
                        else
                        {
                            CleredBySupervisor = "No";
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
                        var Days14 = "";
                        if (missionAuthorization.non18 == true)
                        {
                            Days14 = "Yes";
                        }
                        else
                        {
                            Days14 = "No";
                        }
                        //////////////////////////////


                        var supersforhou = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
                        supersforhou = supersforhou.Where(d => d.staffid == staffsupervisor);
                        var superforhounitsname = supersforhou.First().staff_first_name + " " + staffshou.First().staff_last_name;
                        var houmailUPDATE = supersforhou.First().staff_email;

                        if (houmailUPDATE == "muhannad.hadi@wfp.org")
                        {
                            houmailUPDATE = "rbc.management@wfp.org";//rbc.management@wfp.org

                        }
                        /////////////////////////

                        if (from == "ahmed.moussa@wfp.org")
                        {
                            Response.Redirect("/RBC-SMS/account/login");
                        }
                        /////////////////////////////////////////////////
                        MailMessage mailhou = new MailMessage();
                        mailhou.To.Add(houmailUPDATE); // RD/DRD OFFICE/rbc.management@wfp.org
                       // mailhou.CC.Add(cc);
                        mailhou.Bcc.Add("ahmed.badr@wfp.org");
                        mailhou.From = new MailAddress(from);
                        mailhou.Subject = "Mission Approval Requested By " + staffs.First().staff_first_name + " " + staffs.First().staff_last_name;

                        string Bodyhou = "<table><tr><td colspan='2'>" + "<hr/>New Mission<hr/></td></tr><tr><td><font color='blue'>Request: </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorization.MissionID + "</td></tr>"
                            + "<tr><td>" + "<font color='blue'>From:</font></td><td> "  + staffs.First().staff_first_name + " " + staffs.First().staff_last_name + " </td></tr>"
                            + "<tr><td>" + "<font color='blue'>Unit:</font></td><td> " + staffs.First().unit.unit_abreviation_english + "</td></tr>"
                            + "<tr><td>" + "<font color='blue'>Title:</font></td><td> " + staffs.First().functional_title .functional_title_description_english  + " </td></tr>"
                            + "<tr><td>" + "<font color='blue'>Contract Type:</font></td><td> " + staffs.First().contract_type .contract_type_name  + " </td></tr>"
                          
                            //+ "<tr><td><font color='blue'>Official Start Date :</font></td><td>" + missionAuthorization.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                            //+ "<tr><td><font color='blue'>Official End Date :</font></td><td>" + missionAuthorization.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                          +"<tr><td colspan='2'>" + "<hr/>Mission Itinerarys <hr/>" + "</td></tr>";

                              foreach (var item in missionitinarry)
                            {

                                Bodyhou = Bodyhou

                                   + "<tr><td><font color='red'>Departure:</font></td><td></td></tr>"
                                   + "<tr><td><font color='blue'>Country:</font></td><td>" + item.country.country_name + " / " + item.Twon1 + "</td></tr>"

                                   + "<tr><td><font color='blue'>Date/Time :</font></td><td>" + item.FromDate.Value.ToString("dd/MMM/yy") + "---" + item.Fromtime.Value.ToString("HH:mm") + "</td></tr>"
                                   + "<tr><td><font color='red'>Arrival:</font></td><td></td></tr>"
                                   + "<tr><td><font color='blue'>Country/City:</font></td><td>" + item.country1.country_name + " / " + item.Twon2 + "</td></tr>"

                                   + "<tr><td><font color='blue'>Date/Time:</font></td><td>" + item.ToDate.Value.ToString("dd/MMM/yy") + "---" + item.Totime.Value.ToString("HH:mm") + "</td></tr>"
                                   + "<tr><hr/>";
                            }
                            Bodyhou = Bodyhou

                             + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                            + "<tr><td><font color='blue'>Mission Type:</font></td><td>" + missionAuthorization.MissionType.Name + "</td></tr>"
                            + "<tr><td><font color='blue'>Attach Administrative Note:</font></td><td><a href='http://10.11.135.254:8080/SMS/UploadedFiles/"+ missionAuthorization.files+"'>" + missionAuthorization.files + "</a></td></tr>"
                            + "<tr><td><font color='blue'>Funding:</font></td><td>" + missionAuthorization.funding + "</td></tr>"
                            + "<tr><td><font color='blue'>Objective:</font></td><td>" + missionAuthorization.MissionObjective + "</td></tr>"
                            + "<tr><td><font color='blue'>Expected Outputs:</font></td><td>" + missionAuthorization.ExpectedOutputs + "</td></tr>"
                            + "<tr><td><font color='blue'>Was The Ticket Requested 14 Days In Advance?:</font></td><td>" + Days14 + "</td></tr>"
                            + "<tr><td><font color='blue'>If Not, Please Explain:</font></td><td>" + missionAuthorization.non17 + "</td></tr>"
                            //+ "<tr><td><font color='blue'>Mission Destination :</font></td><td>" + missionAuthorization.country.country_name + " " + missionAuthorization.destinationTwon + "</td></tr>"
                            + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video?:</font></td><td>" + canbedone + "</td></tr>"
                            + "<tr><td><font color='blue'>Cleared By Supervisor:</font></td><td>" + CleredBySupervisor + "</td></tr>"
                            + "<tr><td><font color='blue'>International Roaming:</font></td><td>" + Roming + "</td></tr>"
                            + "<tr><td><font color='blue'>Comment:</font></td><td>" + missionAuthorization.FollowUp + "</td></tr>"
                            + "<tr><td><a href='http://10.11.135.254:8080/SMS/MissionAuthorizations/Approval2?Mid="
                            + missionAuthorization.MissionID
                            + "&RDORDRDSignature=Yes&FromDate=" + missionAuthorization.FromDate
                            + " &ToDate=" + missionAuthorization.ToDate
                            + " &to=" + to + "&staffmail=" + staffs.First().staff_email
                             + "&onbehalfID=" + onbehalfID
                            + "&onbehalfSuperID=" + onbehalfSuperID
                            + "&onbehalfHouID=" + onbehalfHouID
                            + "'>Accept</a></td><td><a href='http://10.11.135.254:8080/SMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization.MissionID
                            + "&RDORDRDSignature=NO & FromDate=" + missionAuthorization.FromDate
                            + " &ToDate=" + missionAuthorization.ToDate
                            + "&to=" + to
                            + "&staffmail=" + staffs.First().staff_email
                             + "&onbehalfID=" + onbehalfID
                            + "&onbehalfSuperID=" + onbehalfSuperID
                            + "&onbehalfHouID=" + onbehalfHouID
                            + "'>Reject</a>" + "</td></tr></table>";

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



                    if (to == "muhannad.hadi@wfp.org")
                    {
                        to = "rbc.management@wfp.org";//rbc.management@wfp.org

                    }


                    var canbedoneee = "";
                    if (missionAuthorization.canbedone == true)
                    {
                        canbedoneee = "Yes";
                    }
                    else
                    {
                        canbedoneee = "No";
                    }

                    var CleredBySuper = "";
                    if (missionAuthorization.ClearedBySupervisor == true)
                    {
                        CleredBySuper = "Yes";
                    }
                    else
                    {
                        CleredBySuper = "No";
                    }
                    var Roming2 = "";
                    if (missionAuthorization.StaffSignature == "True")
                    {
                        Roming2 = "Yes";
                    }
                    else
                    {
                        Roming2 = "No";
                    }
                    var odcitcc = "";
                    if (Roming2 == "Yes")
                    {
                        odcitcc = "cairo.itservicedesk@wfp.org";
                    }
                    else
                    {
                        odcitcc = "ahmed.badr@wfp.org";
                    }
                    var Days142 = "";
                    if (missionAuthorization.non18 == true)
                    {
                        Days142 = "Yes";
                    }
                    else
                    {
                        Days142 = "No";
                    }
                    //////////////////////////////////////////////////////
                    if (from == "ahmed.moussa@wfp.org")
                    {
                        Response.Redirect("/RBC-SMS/account/login");
                    }
                    /////////////////////////////////////////////////

                    MailMessage mail = new MailMessage();
                    mail.To.Add(to); // var to
                   // mail.CC.Add(cc);
                    mail.Bcc.Add("ahmed.badr@wfp.org");
                    mail.From = new MailAddress(from);
                    mail.Subject = "Mission Approval Requested By " + staffs.First().staff_first_name + " " + staffs.First().staff_last_name;

                    string Body = "<table><tr><td colspan='2'>" + "<hr/>New Mission<hr/></td></tr><tr><td><font color='blue'>Request: </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID:</font></td><td>" + missionAuthorization.MissionID + "</td></tr>"
                           + "<tr><td>" + "<font color='blue'>From:</font></td><td> " + staffs.First().staff_first_name + " " + staffs.First().staff_last_name + " </td></tr>"
                           + "<tr><td>" + "<font color='blue'>Unit:</font></td><td> " + staffs.First().unit.unit_abreviation_english + "</td></tr>"
                           + "<tr><td>" + "<font color='blue'>Title:</font></td><td> " + staffs.First().functional_title.functional_title_description_english + " </td></tr>"
                           + "<tr><td>" + "<font color='blue'>Contract Type:</font></td><td> " + staffs.First().contract_type.contract_type_name + " </td></tr>"
                         
                           //+ "<tr><td><font color='blue'>Official Start Date :</font></td><td>" + missionAuthorization.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                           //+ "<tr><td><font color='blue'>Official End Date :</font></td><td>" + missionAuthorization.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                           +"<tr><td colspan='2'>" + "<hr/>Mission Itinerarys <hr/>" + "</td></tr>";

                            foreach (var item in missionitinarry)
                            {

                                Body = Body

                                   + "<tr><td><font color='red'>Departure:</font></td><td></td></tr>"
                                   + "<tr><td><font color='blue'>Country:</font></td><td>" + item.country.country_name + " / " + item.Twon1 + "</td></tr>"

                                   + "<tr><td><font color='blue'>Date/Time :</font></td><td>" + item.FromDate.Value.ToString("dd/MMM/yy") + "---" + item.Fromtime.Value.ToString("HH:mm") + "</td></tr>"
                                   + "<tr><td><font color='red'>Arrival:</font></td><td></td></tr>"
                                   + "<tr><td><font color='blue'>Country/City:</font></td><td>" + item.country1.country_name + " / " + item.Twon2 + "</td></tr>"

                                   + "<tr><td><font color='blue'>Date/Time :</font></td><td>" + item.ToDate.Value.ToString("dd/MMM/yy") + "---" + item.Totime.Value.ToString("HH:mm") + "</td></tr>"
                                   + "<tr><hr/>";
                            }
                            Body = Body

                            + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                           + "<tr><td><font color='blue'>Mission Type:</font></td><td>" + missionAuthorization.MissionType.Name + "</td></tr>"
                           + "<tr><td><font color='blue'>Attached Administrative Note:</font></td><td><a href='http://10.11.135.254:8080/SMS/UploadedFiles/" + missionAuthorization.files + "'>" + missionAuthorization.files + "</a></td></tr>"
                           + "<tr><td><font color='blue'>Funding:</font></td><td>" + missionAuthorization.funding + "</td></tr>"
                           + "<tr><td><font color='blue'>Objective:</font></td><td>" + missionAuthorization.MissionObjective + "</td></tr>"
                           + "<tr><td><font color='blue'>Expected Outputs:</font></td><td>" + missionAuthorization.ExpectedOutputs + "</td></tr>"
                           + "<tr><td><font color='blue'>Was The Ticket Requested 14 Days In Advance?:</font></td><td>" + Days142 + "</td></tr>"
                           + "<tr><td><font color='blue'>If Not, Please Explain:</font></td><td>" + missionAuthorization.non17 + "</td></tr>"
                           //+ "<tr><td><font color='blue'>Mission Destination :</font></td><td>" + missionAuthorization.country.country_name + " " + missionAuthorization.destinationTwon + "</td></tr>"
                           + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video?:</font></td><td>" + canbedoneee + "</td></tr>"
                           + "<tr><td><font color='blue'>Cleared By Supervisor:</font></td><td>" + CleredBySuper + "</td></tr>"
                           + "<tr><td><font color='blue'>International Roaming:</font></td><td>" + Roming2 + "</td></tr>"
                           + "<tr><td><font color='blue'>Comment:</font></td><td>" + missionAuthorization.FollowUp + "</td></tr>"
                           + "<tr><td><a href='http://10.11.135.254:8080/SMS/MissionAuthorizations/Approval2?Mid="
                           + missionAuthorization.MissionID
                           + "&RDORDRDSignature=Yes&FromDate=" + missionAuthorization.FromDate
                           + " &ToDate=" + missionAuthorization.ToDate
                           + " &to=" + to + "&staffmail=" + staffs.First().staff_email
                            + "&onbehalfID=" + onbehalfID
                            + "&onbehalfSuperID=" + onbehalfSuperID
                            + "&onbehalfHouID=" + onbehalfHouID
                           + "'>Accept</a></td><td><a href='http://10.11.135.254:8080/SMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization.MissionID
                           + "&RDORDRDSignature=NO & FromDate=" + missionAuthorization.FromDate
                           + " &ToDate=" + missionAuthorization.ToDate
                           + "&to=" + to
                           + "&staffmail=" + staffs.First().staff_email
                            + "&onbehalfID=" + onbehalfID
                            + "&onbehalfSuperID=" + onbehalfSuperID
                            + "&onbehalfHouID=" + onbehalfHouID
                           + "'>Reject</a>" + "</td></tr></table>";
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











        // GET: MissionAuthorizations/Approval2

        public ActionResult Approval2(int? Mid, string RDORDRDSignature, DateTime? FromDate, DateTime? ToDate, string to, string staffmail, string supervisorname, int? onbehalfID, int? onbehalfSuperID, int? onbehalfHouID)
        {


            if (!Request.IsAuthenticated)
            {
                Response.Redirect("/RBC-SMS/account/login");
            }

            ///////////////////////// start on behalf 

            if (onbehalfID != null && onbehalfSuperID != null && onbehalfHouID != null)
            {
                ViewBag.mid = Request.QueryString["Mid"];
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

                        var missionAuthorization = db.MissionAuthorizations.Single(u => u.MissionID == Mid);
                        missionAuthorization.RDORDRDSignature = "Yes";
                        missionAuthorization.RDORDRDDate = DateTime.Now;
                        db.SaveChanges();

                        var missionitinarry = db.MissionItineraries.Include(i => i.MissionAuthorization);
                        missionitinarry = missionitinarry.Where(i => i.MissionID == Mid);

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
                            if (missionAuthorization.canbedone == true)
                            {
                                canbedone = "Yes";
                            }
                            else
                            {
                                canbedone = "No";
                            }

                            var CleredBySupervisor = "";
                            if (missionAuthorization.ClearedBySupervisor == true)
                            {
                                CleredBySupervisor = "Yes";
                            }
                            else
                            {
                                CleredBySupervisor = "No";
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

                             var Days142 = "";
                    if (missionAuthorization.non18 == true)
                    {
                        Days142 = "Yes";
                    }
                    else
                    {
                        Days142 = "No";
                    }
                            MailMessage mail = new MailMessage();
                            mail.To.Add(onbehalfemail);
                            mail.Bcc.Add("ahmed.badr@wfp.org");
                            //mail.CC.Add("rbc.travel@wfp.org"); //travel unit
                            //mail.CC.Add("mai.kenawi@wfp.org"); //travel unit
                            //mail.CC.Add("amal.mohamed@wfp.org"); //travel unit
                            //mail.CC.Add("ayten.el-sheikh@wfp.org"); //travel unit
                            //mail.CC.Add("essraa.soliman@wfp.org"); //travel unit
                            //mail.CC.Add("hossam.metwally@wfp.org"); //travel unit
                            //mail.CC.Add("mina.samy@wfp.org"); //travel unit
                            //mail.CC.Add("rbc.management@wfp.org"); //RD/DRD Office
                            mail.Bcc.Add(odcitcc); //travel unit
                            mail.From = new MailAddress("rbc.management@wfp.org");
                            mail.Subject = "New Mission Approved";
                            string Body = "<table><tr><td colspan='2'>" + "<hr/>New Mission<hr/></td></tr><tr><td><font color='blue'>Request : </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorization.MissionID + "</td></tr>"
                        + "<tr><td>" + "<font color='blue'>From :</font></td><td> " + missionAuthorization.staff.staff_first_name + " " + missionAuthorization.staff.staff_last_name + " </td></tr>"
                        + "<tr><td>" + "<font color='blue'>On behalf: :</font></td><td> " + missionAuthorization.staff1.staff_first_name + " " + missionAuthorization.staff1.staff_last_name + " </td></tr>"
                        + "<tr><td>" + "<font color='blue'>Unit :</font></td><td> " + missionAuthorization.staff.unit.unit_abreviation_english + "</td></tr>"
                        + "<tr><td>" + "<font color='blue'>Title :</font></td><td> " + missionAuthorization.staff.functional_title.functional_title_description_english + " </td></tr>"
                        + "<tr><td>" + "<font color='blue'>Contract Type :</font></td><td> " + missionAuthorization.staff.contract_type.contract_type_name + " </td></tr>"
                      
                        //+ "<tr><td><font color='blue'>Official Start Date :</font></td><td>" + missionAuthorization.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                        //+ "<tr><td><font color='blue'>Official End Date :</font></td><td>" + missionAuthorization.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                        //+ "<tr><td><font color='blue'>Reporting Time :</font></td><td>" + missionAuthorization.FromTime.Value.ToString("HH:mm") + "</td></tr>"
                        //+ "<tr><td><font color='blue'>End Time :</font></td><td>" + missionAuthorization.ToTime.Value.ToString("HH:mm") + "</td></tr>"
                        + "<tr><td colspan='2'>" + "<hr/>Mission Itinerarys <hr/>" + "</td></tr>";

                            foreach (var item in missionitinarry)
                            {

                                Body = Body

                                   + "<tr><td><font color='red'>Departure:</font></td><td></td></tr>"
                                   + "<tr><td><font color='blue'>Country:</font></td><td>" + item.country.country_name + " / " + item.Twon1 + "</td></tr>"

                                   + "<tr><td><font color='blue'>Date/Time :</font></td><td>" + item.FromDate.Value.ToString("dd/MMM/yy") + "---" + item.Fromtime.Value.ToString("HH:mm") + "</td></tr>"
                                   + "<tr><td><font color='red'>Arrival:</font></td><td></td></tr>"
                                   + "<tr><td><font color='blue'>Country/City:</font></td><td>" + item.country1.country_name + " / " + item.Twon2 + "</td></tr>"

                                   + "<tr><td><font color='blue'>Date/Time :</font></td><td>" + item.ToDate.Value.ToString("dd/MMM/yy") + "---" + item.Totime.Value.ToString("HH:mm") + "</td></tr>"
                                   + "<tr><hr/>";
                            }
                            Body = Body

                    + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                    + "<tr><td><font color='blue'>Mission Type :</font></td><td>" + missionAuthorization.MissionType.Name + "</td></tr>"
                        + "<tr><td><font color='blue'>End Time :</font></td><td>" + missionAuthorization.ToTime + "</td></tr>"
                        + "<tr><td><font color='blue'>Mission Type :</font></td><td>" + missionAuthorization.MissionType.Name + "</td></tr>"
                        + "<tr><td><font color='blue'>Attached Administrative Note :</font></td><td><a href='http://10.11.135.254:8080/SMS/UploadedFiles/" + missionAuthorization.files + "'>" + missionAuthorization.files + "</a></td></tr>"
                        + "<tr><td><font color='blue'>Funding :</font></td><td>" + missionAuthorization.funding + "</td></tr>"
                        + "<tr><td><font color='blue'>Objective :</font></td><td>" + missionAuthorization.MissionObjective + "</td></tr>"
                        + "<tr><td><font color='blue'>Expected Outputs :</font></td><td>" + missionAuthorization.ExpectedOutputs + "</td></tr>"
                        + "<tr><td><font color='blue'>Was The Ticket Requested 14 Days In Advance? :</font></td><td>" + Days142 + "</td></tr>"
                        + "<tr><td><font color='blue'>If Not, Please Explain :</font></td><td>" + missionAuthorization.non17 + "</td></tr>"
                        //+ "<tr><td><font color='blue'>Mission Destination :</font></td><td>" + missionAuthorization.country.country_name + " " + missionAuthorization.destinationTwon + "</td></tr>"
                        + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video ? :</font></td><td>" + canbedone + "</td></tr>"
                        + "<tr><td><font color='blue'>Cleared By Supervisor :</font></td><td>" + CleredBySupervisor + "</td></tr>"
                        + "<tr><td><font color='blue'>International Roaming :</font></td><td>" + Roming + "</td></tr>"
                        + "<tr><td><font color='blue'>Comment :</font></td><td>" + missionAuthorization.FollowUp + "</td></tr>";
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
                        }

                        else
                        {
                            var canbedonee = "";
                            if (missionAuthorization.canbedone == true)
                            {
                                canbedonee = "Yes";
                            }
                            else
                            {
                                canbedonee = "No";
                            }

                            var ClearedBySupervisor = "";
                            if (missionAuthorization.ClearedBySupervisor == true)
                            {
                                ClearedBySupervisor = "Yes";
                            }
                            else
                            {
                                ClearedBySupervisor = "No";
                            }


                            if (staffmail == "muhannad.hadi@wfp.org")
                            {
                                staffmail = "rbc.management@wfp.org";//rbc.management@wfp.org

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
                            var Days142 = "";
                            if (missionAuthorization.non18 == true)
                            {
                                Days142 = "Yes";
                            }
                            else
                            {
                                Days142 = "No";
                            }
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
                            //mailhou.CC.Add("hossam.metwally@wfp.org"); //travel unit
                            //mailhou.CC.Add("mina.samy@wfp.org"); //travel unit
                            //mailhou.CC.Add("rbc.management@wfp.org"); //RD/DRD Office
                            mailhou.From = new MailAddress(hou);
                            mailhou.Subject = "New Mission Approved";
                            string Bodyhou = "<table><tr><td colspan='2'>" + "<hr/>New Mission<hr/></td></tr><tr><td><font color='blue'>Request : </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorization.MissionID + "</td></tr>"
                    + "<tr><td>" + "<font color='blue'>From :</font></td><td> " + missionAuthorization.staff.staff_first_name + " " + missionAuthorization.staff.staff_last_name + " </td></tr>"
                    + "<tr><td>" + "<font color='blue'>On behalf: :</font></td><td> " + missionAuthorization.staff1.staff_first_name + " " + missionAuthorization.staff1.staff_last_name + " </td></tr>"
                    + "<tr><td>" + "<font color='blue'>Unit :</font></td><td> " + missionAuthorization.staff.unit.unit_abreviation_english + "</td></tr>"
                    + "<tr><td>" + "<font color='blue'>Title :</font></td><td> " + missionAuthorization.staff.functional_title.functional_title_description_english + " </td></tr>"
                    + "<tr><td>" + "<font color='blue'>Contract Type :</font></td><td> " + missionAuthorization.staff.contract_type.contract_type_name + " </td></tr>"
                   
                    //+ "<tr><td><font color='blue'>Official Start Date :</font></td><td>" + missionAuthorization.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                    //+ "<tr><td><font color='blue'>Official End Date :</font></td><td>" + missionAuthorization.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                    //+ "<tr><td><font color='blue'>Reporting Time :</font></td><td>" + missionAuthorization.FromTime.Value.ToString("HH:mm") + "</td></tr>"
                    //    + "<tr><td><font color='blue'>End Time :</font></td><td>" + missionAuthorization.ToTime.Value.ToString("HH:mm") + "</td></tr>"
                    + "<tr><td colspan='2'>" + "<hr/>Mission Itinerarys <hr/>" + "</td></tr>";

                            foreach (var item in missionitinarry)
                            {

                                Bodyhou = Bodyhou

                                   + "<tr><td><font color='red'>Departure:</font></td><td></td></tr>"
                                   + "<tr><td><font color='blue'>Country:</font></td><td>" + item.country.country_name + " / " + item.Twon1 + "</td></tr>"

                                   + "<tr><td><font color='blue'>Date/Time :</font></td><td>" + item.FromDate.Value.ToString("dd/MMM/yy") + "---" + item.Fromtime.Value.ToString("HH:mm") + "</td></tr>"
                                   + "<tr><td><font color='red'>Arrival:</font></td><td></td></tr>"
                                   + "<tr><td><font color='blue'>Country/City:</font></td><td>" + item.country1.country_name + " / " + item.Twon2 + "</td></tr>"

                                   + "<tr><td><font color='blue'>Date/Time :</font></td><td>" + item.ToDate.Value.ToString("dd/MMM/yy") + "---" + item.Totime.Value.ToString("HH:mm") + "</td></tr>"
                                   + "<tr><hr/>";
                            }
                            Bodyhou = Bodyhou

                    + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                    + "<tr><td><font color='blue'>Mission Type :</font></td><td>" + missionAuthorization.MissionType.Name + "</td></tr>"
                    + "<tr><td><font color='blue'>Attached Administrative Note :</font></td><td><a href='http://10.11.135.254:8080/SMS/UploadedFiles/" + missionAuthorization.files + "'>" + missionAuthorization.files + "</a></td></tr>"
                    + "<tr><td><font color='blue'>Funding :</font></td><td>" + missionAuthorization.funding + "</td></tr>"
                    + "<tr><td><font color='blue'>Objective :</font></td><td>" + missionAuthorization.MissionObjective + "</td></tr>"
                    + "<tr><td><font color='blue'>Expected Outputs :</font></td><td>" + missionAuthorization.ExpectedOutputs + "</td></tr>"
                    + "<tr><td><font color='blue'>Was The Ticket Requested 14 Days In Advance? :</font></td><td>" + Days142 + "</td></tr>"
                    + "<tr><td><font color='blue'>If Not, Please Explain :</font></td><td>" + missionAuthorization.non17 + "</td></tr>"
                    //+ "<tr><td><font color='blue'>Mission Destination :</font></td><td>" + missionAuthorization.country.country_name + " " + missionAuthorization.destinationTwon + "</td></tr>"
                    + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video ? :</font></td><td>" + canbedonee + "</td></tr>"
                    + "<tr><td><font color='blue'>Cleared By Supervisor :</font></td><td>" + ClearedBySupervisor + "</td></tr>"
                    + "<tr><td><font color='blue'>International Roaming :</font></td><td>" + Roming + "</td></tr>"
                    + "<tr><td><font color='blue'>Comment :</font></td><td>" + missionAuthorization.FollowUp + "</td></tr>";

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
                    var missionAuthorizationn = db.MissionAuthorizations.Single(u => u.MissionID == Mid);
                    missionAuthorizationn.RDORDRDSignature = "No";
                    missionAuthorizationn.RDORDRDDate = DateTime.Now;
                    db.SaveChanges();

                    var missionitinarry = db.MissionItineraries.Include(i => i.MissionAuthorization);
                    missionitinarry = missionitinarry.Where(i => i.MissionID == Mid);

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
                        if (missionAuthorizationn.canbedone == true)
                        {
                            canbedone = "Yes";
                        }
                        else
                        {
                            canbedone = "No";
                        }

                        var CleredBySupervisor = "";
                        if (missionAuthorizationn.ClearedBySupervisor == true)
                        {
                            CleredBySupervisor = "Yes";
                        }
                        else
                        {
                            CleredBySupervisor = "No";
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
                        var odcitcc = "";
                        if (Roming == "Yes")
                        {
                            odcitcc = "cairo.itservicedesk@wfp.org";
                        }
                        else
                        {
                            odcitcc = "ahmed.badr@wfp.org";
                        }
                        var Days142 = "";
                        if (missionAuthorizationn.non18 == true)
                        {
                            Days142 = "Yes";
                        }
                        else
                        {
                            Days142 = "No";
                        }
                        MailMessage mail2 = new MailMessage();
                        mail2.To.Add(onbehalfemail);
                        mail2.Bcc.Add("ahmed.badr@wfp.org");
                        mail2.CC.Add(cc);
                        mail2.From = new MailAddress("rbc.management@wfp.org");
                        mail2.Subject = "New Mission Rejected";
                        string Body2 = "<table><tr><td colspan='2'>" + "<hr/>New Mission<hr/></td></tr><tr><td><font color='blue'>Request : </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorizationn.MissionID + "</td></tr>"
                    + "<tr><td>" + "<font color='blue'>From :</font></td><td> " + missionAuthorizationn.staff.staff_first_name + " " + missionAuthorizationn.staff.staff_last_name + " </td></tr>"
                      + "<tr><td>" + "<font color='blue'>On behalf: :</font></td><td> " + missionAuthorizationn.staff1.staff_first_name + " " + missionAuthorizationn.staff1.staff_last_name + " </td></tr>"
                    + "<tr><td>" + "<font color='blue'>Unit :</font></td><td> " + missionAuthorizationn.staff.unit.unit_abreviation_english + "</td></tr>"
                    + "<tr><td>" + "<font color='blue'>Title :</font></td><td> " + missionAuthorizationn.staff.functional_title.functional_title_description_english + " </td></tr>"
                    + "<tr><td>" + "<font color='blue'>Contract Type :</font></td><td> " + missionAuthorizationn.staff.contract_type.contract_type_name + " </td></tr>"
                   
                    //+ "<tr><td><font color='blue'>Official Start Date :</font></td><td>" + missionAuthorizationn.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                    //+ "<tr><td><font color='blue'>Official End Date :</font></td><td>" + missionAuthorizationn.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                    //+ "<tr><td><font color='blue'>Reporting Time :</font></td><td>" + missionAuthorizationn.FromTime.Value.ToString("HH:mm") + "</td></tr>"
                    //    + "<tr><td><font color='blue'>End Time :</font></td><td>" + missionAuthorizationn.ToTime.Value.ToString("HH:mm") + "</td></tr>"
                    + "<tr><td colspan='2'>" + "<hr/>Mission Itinerarys <hr/>" + "</td></tr>";

                        foreach (var item in missionitinarry)
                        {

                            Body2 = Body2

                               + "<tr><td><font color='red'>Departure:</font></td><td></td></tr>"
                                   + "<tr><td><font color='blue'>Country:</font></td><td>" + item.country.country_name + " / " + item.Twon1 + "</td></tr>"

                                   + "<tr><td><font color='blue'>Date/Time :</font></td><td>" + item.FromDate.Value.ToString("dd/MMM/yy") + "---" + item.Fromtime.Value.ToString("HH:mm") + "</td></tr>"
                                   + "<tr><td><font color='red'>Arrival:</font></td><td></td></tr>"
                                   + "<tr><td><font color='blue'>Country/City:</font></td><td>" + item.country1.country_name + " / " + item.Twon2 + "</td></tr>"

                                   + "<tr><td><font color='blue'>Date/Time :</font></td><td>" + item.ToDate.Value.ToString("dd/MMM/yy") + "---" + item.Totime.Value.ToString("HH:mm") + "</td></tr>"
                                   + "<tr><hr/>";
                        }
                        Body2 = Body2

                 + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                + "<tr><td><font color='blue'>Mission Type :</font></td><td>" + missionAuthorizationn.MissionType.Name + "</td></tr>"
                    + "<tr><td><font color='blue'>Attached Administrative Note :</font></td><td><a href='http://10.11.135.254:8080/SMS/UploadedFiles/" + missionAuthorizationn.files + "'>" + missionAuthorizationn.files + "</a></td></tr>"
                    + "<tr><td><font color='blue'>Funding :</font></td><td>" + missionAuthorizationn.funding + "</td></tr>"
                    + "<tr><td><font color='blue'>Objective :</font></td><td>" + missionAuthorizationn.MissionObjective + "</td></tr>"
                    + "<tr><td><font color='blue'>Expected Outputs :</font></td><td>" + missionAuthorizationn.ExpectedOutputs + "</td></tr>"
                    + "<tr><td><font color='blue'>Was The Ticket Requested 14 Days In Advance? :</font></td><td>" + Days142 + "</td></tr>"
                    + "<tr><td><font color='blue'>If Not, Please Explain :</font></td><td>" + missionAuthorizationn.non17 + "</td></tr>"
                    //+ "<tr><td><font color='blue'>Mission Destination :</font></td><td>" + missionAuthorizationn.country.country_name + " " + missionAuthorizationn.destinationTwon + "</td></tr>"
                    + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video ? :</font></td><td>" + canbedone + "</td></tr>"
                    + "<tr><td><font color='blue'>Cleared By Supervisor :</font></td><td>" + CleredBySupervisor + "</td></tr>"
                    + "<tr><td><font color='blue'>International Roaming :</font></td><td>" + Roming + "</td></tr>"
                    + "<tr><td><font color='blue'>Comment :</font></td><td>" + missionAuthorizationn.FollowUp + "</td></tr>";

                        mail2.Body = Body2;
                        mail2.IsBodyHtml = true;
                        SmtpClient smtp2 = new SmtpClient();
                        smtp2.Host = "smtprelay.global.wfp.org";
                        smtp2.Port = 25;
                        smtp2.UseDefaultCredentials = true;
                        //smtp.Credentials = new System.Net.NetworkCredential
                        //("ahmed.badr", "Survivor2323");// Enter seders User name and password  
                        //smtp.EnableSsl = true;
                        smtp2.Send(mail2);
                    }

                    else
                    {
                        var canbedoner = "";
                        if (missionAuthorizationn.canbedone == true)
                        {
                            canbedoner = "Yes";
                        }
                        else
                        {
                            canbedoner = "No";
                        }

                        var ClearedBySupervisor = "";
                        if (missionAuthorizationn.ClearedBySupervisor == true)
                        {
                            ClearedBySupervisor = "Yes";
                        }
                        else
                        {
                            ClearedBySupervisor = "No";
                        }

                        if (cc == "muhannad.hadi@wfp.org")
                        {
                            cc = "rbc.management@wfp.org";//rbc.management@wfp.org

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
                        var odcitcc = "";
                        if (Roming == "Yes")
                        {
                            odcitcc = "cairo.itservicedesk@wfp.org";
                        }
                        else
                        {
                            odcitcc = "ahmed.badr@wfp.org";
                        }
                        var Days142 = "";
                        if (missionAuthorizationn.non18 == true)
                        {
                            Days142 = "Yes";
                        }
                        else
                        {
                            Days142 = "No";
                        }

                        MailMessage mail = new MailMessage();
                        mail.To.Add(onbehalfemail);
                        mail.Bcc.Add("ahmed.badr@wfp.org");
                        mail.CC.Add(onbehafsuperemail);
                        mail.From = new MailAddress(hou);
                        mail.Subject = "New Mission Rejected";
                        string Body = "<table><tr><td colspan='2'>" + "<hr/>New Mission<hr/></td></tr><tr><td><font color='blue'>Request : </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorizationn.MissionID + "</td></tr>"
                    + "<tr><td>" + "<font color='blue'>From :</font></td><td> " + missionAuthorizationn.staff.staff_first_name + " " + missionAuthorizationn.staff.staff_last_name + " </td></tr>"
                      + "<tr><td>" + "<font color='blue'>On behalf: :</font></td><td> " + missionAuthorizationn.staff1.staff_first_name + " " + missionAuthorizationn.staff1.staff_last_name + " </td></tr>"
                    + "<tr><td>" + "<font color='blue'>Unit :</font></td><td> " + missionAuthorizationn.staff.unit.unit_abreviation_english + "</td></tr>"
                    + "<tr><td>" + "<font color='blue'>Title :</font></td><td> " + missionAuthorizationn.staff.functional_title.functional_title_description_english + " </td></tr>"
                    + "<tr><td>" + "<font color='blue'>Contract Type :</font></td><td> " + missionAuthorizationn.staff.contract_type.contract_type_name + " </td></tr>"
                   
                    //+ "<tr><td><font color='blue'>Official Start Date :</font></td><td>" + missionAuthorizationn.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                    //+ "<tr><td><font color='blue'>Official End Date :</font></td><td>" + missionAuthorizationn.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                    //+ "<tr><td><font color='blue'>Reporting Time :</font></td><td>" + missionAuthorizationn.FromTime.Value.ToString("HH:mm") + "</td></tr>"
                    //    + "<tr><td><font color='blue'>End Time :</font></td><td>" + missionAuthorizationn.ToTime.Value.ToString("HH:mm") + "</td></tr>"
                    + "<tr><td colspan='2'>" + "<hr/>Mission Itinerarys <hr/>" + "</td></tr>";

                        foreach (var item in missionitinarry)
                        {

                            Body = Body

                              + "<tr><td><font color='red'>Departure:</font></td><td></td></tr>"
                                   + "<tr><td><font color='blue'>Country:</font></td><td>" + item.country.country_name + " / " + item.Twon1 + "</td></tr>"

                                   + "<tr><td><font color='blue'>Date/Time :</font></td><td>" + item.FromDate.Value.ToString("dd/MMM/yy") + "---" + item.Fromtime.Value.ToString("HH:mm") + "</td></tr>"
                                   + "<tr><td><font color='red'>Arrival:</font></td><td></td></tr>"
                                   + "<tr><td><font color='blue'>Country/City:</font></td><td>" + item.country1.country_name + " / " + item.Twon2 + "</td></tr>"

                                   + "<tr><td><font color='blue'>Date/Time :</font></td><td>" + item.ToDate.Value.ToString("dd/MMM/yy") + "---" + item.Totime.Value.ToString("HH:mm") + "</td></tr>"
                                   + "<tr><hr/>";
                        }
                        Body = Body

                + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                + "<tr><td><font color='blue'>Mission Type :</font></td><td>" + missionAuthorizationn.MissionType.Name + "</td></tr>"
                    + "<tr><td><font color='blue'>Attached Administrative Note :</font></td><td><a href='http://10.11.135.254:8080/SMS/UploadedFiles/" + missionAuthorizationn.files + "'>" + missionAuthorizationn.files + "</a></td></tr>"
                    + "<tr><td><font color='blue'>Funding :</font></td><td>" + missionAuthorizationn.funding + "</td></tr>"
                    + "<tr><td><font color='blue'>Objective :</font></td><td>" + missionAuthorizationn.MissionObjective + "</td></tr>"
                    + "<tr><td><font color='blue'>Expected Outputs :</font></td><td>" + missionAuthorizationn.ExpectedOutputs + "</td></tr>"
                    + "<tr><td><font color='blue'>Was The Ticket Requested 14 Days In Advance? :</font></td><td>" + Days142 + "</td></tr>"
                    + "<tr><td><font color='blue'>If Not, Please Explain :</font></td><td>" + missionAuthorizationn.non17 + "</td></tr>"
                    //+ "<tr><td><font color='blue'>Mission Destination :</font></td><td>" + missionAuthorizationn.country.country_name + " " + missionAuthorizationn.destinationTwon + "</td></tr>"
                    + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video ? :</font></td><td>" + canbedoner + "</td></tr>"
                    + "<tr><td><font color='blue'>Cleared By Supervisor :</font></td><td>" + ClearedBySupervisor + "</td></tr>"
                    + "<tr><td><font color='blue'>International Roaming :</font></td><td>" + Roming + "</td></tr>"
                    + "<tr><td><font color='blue'>Comment :</font></td><td>" + missionAuthorizationn.FollowUp + "</td></tr>";

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


                    }
                }



            }
            ///////////////////////// end on behalf 
            else
            {
                ViewBag.mid = Request.QueryString["Mid"];
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

                        var missionAuthorization = db.MissionAuthorizations.Single(u => u.MissionID == Mid);
                        missionAuthorization.RDORDRDSignature = "Yes";
                        missionAuthorization.RDORDRDDate = DateTime.Now;
                        db.SaveChanges();

                        var missionitinarry = db.MissionItineraries.Include(i => i.MissionAuthorization);
                        missionitinarry = missionitinarry.Where(i => i.MissionID == Mid);


                        var staffs = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                        staffs = staffs.Where(s => s.staff_email.Contains(staffmail));
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
                            if (missionAuthorization.ClearedBySupervisor == true)
                            {
                                CleredBySupervisor = "Yes";
                            }
                            else
                            {
                                CleredBySupervisor = "No";
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
                            var Days142 = "";
                            if (missionAuthorization.non18 == true)
                            {
                                Days142 = "Yes";
                            }
                            else
                            {
                                Days142 = "No";
                            }

                            MailMessage mail = new MailMessage();
                            mail.To.Add(staffmail);
                            mail.Bcc.Add("ahmed.badr@wfp.org");
                            mail.Bcc.Add(odcitcc); //travel unit
                            //mail.CC.Add("rbc.travel@wfp.org"); //travel unit
                            //mail.CC.Add("mai.kenawi@wfp.org"); //travel unit
                            //mail.CC.Add("amal.mohamed@wfp.org"); //travel unit
                            //mail.CC.Add("ayten.el-sheikh@wfp.org"); //travel unit
                            //mail.CC.Add("essraa.soliman@wfp.org"); //travel unit
                            //mail.CC.Add("hossam.metwally@wfp.org"); //travel unit
                            //mail.CC.Add("mina.samy@wfp.org"); //travel unit
                            //mail.CC.Add("rbc.management@wfp.org"); //RD/DRD Office
                            mail.From = new MailAddress(from);
                            mail.Subject = "New Mission Approved";
                            string Body = "<table><tr><td colspan='2'>" + "<hr/>New Mission<hr/></td></tr><tr><td><font color='blue'>Request : </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorization.MissionID + "</td></tr>"
                    + "<tr><td>" + "<font color='blue'>From :</font></td><td> " + missionAuthorization.staff.staff_first_name + " " + missionAuthorization.staff.staff_last_name + " </td></tr>"
                    //+ "<tr><td>" + "<font color='blue'>Onb ehalf: :</font></td><td> " + missionAuthorization.staff1.staff_first_name + " " + missionAuthorization.staff1.staff_last_name + " </td></tr>"
                    + "<tr><td>" + "<font color='blue'>Unit :</font></td><td> " + missionAuthorization.staff.unit.unit_abreviation_english + "</td></tr>"
                    + "<tr><td>" + "<font color='blue'>Title :</font></td><td> " + missionAuthorization.staff.functional_title.functional_title_description_english + " </td></tr>"
                    + "<tr><td>" + "<font color='blue'>Contract Type :</font></td><td> " + missionAuthorization.staff.contract_type.contract_type_name + " </td></tr>"
                   
                    //+ "<tr><td><font color='blue'>Official Start Date :</font></td><td>" + missionAuthorization.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                    //+ "<tr><td><font color='blue'>Official End Date :</font></td><td>" + missionAuthorization.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                    //+ "<tr><td><font color='blue'>Reporting Time :</font></td><td>" + missionAuthorization.FromTime.Value.ToString("HH:mm") + "</td></tr>"
                    //+ "<tr><td><font color='blue'>End Time :</font></td><td>" + missionAuthorization.ToTime.Value.ToString("HH:mm") + "</td></tr>"
                    + "<tr><td colspan='2'>" + "<hr/>Mission Itinerary <hr/>" + "</td></tr>";

                        foreach (var item in missionitinarry)
                          {

                              Body = Body

                                  + "<tr><td><font color='red'>Departure:</font></td><td></td></tr>"
                                   + "<tr><td><font color='blue'>Country:</font></td><td>" + item.country.country_name + " / " + item.Twon1 + "</td></tr>"

                                   + "<tr><td><font color='blue'>Date/Time :</font></td><td>" + item.FromDate.Value.ToString("dd/MMM/yy") + "---" + item.Fromtime.Value.ToString("HH:mm") + "</td></tr>"
                                   + "<tr><td><font color='red'>Arrival:</font></td><td></td></tr>"
                                   + "<tr><td><font color='blue'>Country/City:</font></td><td>" + item.country1.country_name + " / " + item.Twon2 + "</td></tr>"

                                   + "<tr><td><font color='blue'>Date/Time :</font></td><td>" + item.ToDate.Value.ToString("dd/MMM/yy") + "---" + item.Totime.Value.ToString("HH:mm") + "</td></tr>"
                                   + "<tr><hr/>";
                          }
                            Body = Body

                    + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                    + "<tr><td><font color='blue'>Mission Type :</font></td><td>" + missionAuthorization.MissionType.Name + "</td></tr>"
                    + "<tr><td><font color='blue'>Attached Administrative Note :</font></td><td><a href='http://10.11.135.254:8080/SMS/UploadedFiles/" + missionAuthorization.files + "'>" + missionAuthorization.files + "</a></td></tr>"
                    + "<tr><td><font color='blue'>Funding :</font></td><td>" + missionAuthorization.funding + "</td></tr>"
                    + "<tr><td><font color='blue'>Objective :</font></td><td>" + missionAuthorization.MissionObjective + "</td></tr>"
                    + "<tr><td><font color='blue'>Expected Outputs :</font></td><td>" + missionAuthorization.ExpectedOutputs + "</td></tr>"
                    + "<tr><td><font color='blue'>Was The Ticket Requested 14 Days In Advance? :</font></td><td>" + Days142 + "</td></tr>"
                    + "<tr><td><font color='blue'>If Not, Please Explain :</font></td><td>" + missionAuthorization.non17 + "</td></tr>"
                    //+ "<tr><td><font color='blue'>Mission Destination :</font></td><td>" + missionAuthorization.country.country_name + " " + missionAuthorization.destinationTwon + "</td></tr>"
                    + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video ? :</font></td><td>" + canbedone + "</td></tr>"
                    + "<tr><td><font color='blue'>Cleared By Supervisor :</font></td><td>" + CleredBySupervisor + "</td></tr>"
                    + "<tr><td><font color='blue'>International Roaming :</font></td><td>" + Roming + "</td></tr>"
                    + "<tr><td><font color='blue'>Comment :</font></td><td>" + missionAuthorization.FollowUp + "</td></tr>";

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
                        }

                        else
                        {
                            var canbedonee = "";
                            if (missionAuthorization.canbedone == true)
                            {
                                canbedonee = "Yes";
                            }
                            else
                            {
                                canbedonee = "No";
                            }

                            var ClearedBySupervisor = "";
                            if (missionAuthorization.ClearedBySupervisor == true)
                            {
                                ClearedBySupervisor = "Yes";
                            }
                            else
                            {
                                ClearedBySupervisor = "No";
                            }


                            if (staffmail == "muhannad.hadi@wfp.org")
                            {
                                staffmail = "rbc.management@wfp.org";//rbc.management@wfp.org

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
                            var Days142 = "";
                            if (missionAuthorization.non18 == true)
                            {
                                Days142 = "Yes";
                            }
                            else
                            {
                                Days142 = "No";
                            }
                            MailMessage mailhou = new MailMessage();
                            mailhou.To.Add(staffmail); //staffmail
                            mailhou.Bcc.Add("ahmed.badr@wfp.org");
                            mailhou.Bcc.Add(odcitcc); //travel unit
                            mailhou.CC.Add(superemail);
                            //mailhou.CC.Add("rbc.travel@wfp.org"); //travel unit
                            //mailhou.CC.Add("mai.kenawi@wfp.org"); //travel unit
                            //mailhou.CC.Add("amal.mohamed@wfp.org"); //travel unit
                            //mailhou.CC.Add("ayten.el-sheikh@wfp.org"); //travel unit
                            //mailhou.CC.Add("essraa.soliman@wfp.org"); //travel unit
                            //mailhou.CC.Add("hossam.metwally@wfp.org"); //travel unit
                            //mailhou.CC.Add("mina.samy@wfp.org"); //travel unit
                            //mailhou.CC.Add("rbc.management@wfp.org"); //RD/DRD Office
                            mailhou.From = new MailAddress(hou);
                            mailhou.Subject = "New Mission Approved";
                            string Bodyhou = "<table><tr><td colspan='2'>" + "<hr/>New Mission<hr/></td></tr><tr><td><font color='blue'>Request : </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorization.MissionID + "</td></tr>"
                     + "<tr><td>" + "<font color='blue'>From :</font></td><td> " + missionAuthorization.staff.staff_first_name + " " + missionAuthorization.staff.staff_last_name + " </td></tr>"
                      
                  //+ "<tr><td>" + "<font color='blue'>On behalf: :</font></td><td> " + fonname + " " + lonname + " </td></tr>"
                     + "<tr><td>" + "<font color='blue'>Unit :</font></td><td> " + missionAuthorization.staff.unit.unit_abreviation_english + "</td></tr>"
                     + "<tr><td>" + "<font color='blue'>Title :</font></td><td> " + missionAuthorization.staff.functional_title.functional_title_description_english + " </td></tr>"
                     + "<tr><td>" + "<font color='blue'>Contract Type :</font></td><td> " + missionAuthorization.staff.contract_type.contract_type_name + " </td></tr>"
                    
                     //+ "<tr><td><font color='blue'>Official Start Date :</font></td><td>" + missionAuthorization.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                     //+ "<tr><td><font color='blue'>Official End Date :</font></td><td>" + missionAuthorization.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                     //+ "<tr><td><font color='blue'>Reporting Time :</font></td><td>" + missionAuthorization.FromTime.Value.ToString("HH:mm") + "</td></tr>"
                     // + "<tr><td><font color='blue'>End Time :</font></td><td>" + missionAuthorization.ToTime.Value.ToString("HH:mm") + "</td></tr>"
                    + "<tr><td colspan='2'>" + "<hr/>Mission Itinerary <hr/>" + "</td></tr>";

                            foreach (var item in missionitinarry)
                            {

                                Bodyhou = Bodyhou

                                   + "<tr><td><font color='red'>Departure:</font></td><td></td></tr>"
                                   + "<tr><td><font color='blue'>Country:</font></td><td>" + item.country.country_name + " / " + item.Twon1 + "</td></tr>"

                                   + "<tr><td><font color='blue'>Date/Time :</font></td><td>" + item.FromDate.Value.ToString("dd/MMM/yy") + "---" + item.Fromtime.Value.ToString("HH:mm") + "</td></tr>"
                                   + "<tr><td><font color='red'>Arrival:</font></td><td></td></tr>"
                                   + "<tr><td><font color='blue'>Country/City:</font></td><td>" + item.country1.country_name + " / " + item.Twon2 + "</td></tr>"

                                   + "<tr><td><font color='blue'>Date/Time :</font></td><td>" + item.ToDate.Value.ToString("dd/MMM/yy") + "---" + item.Totime.Value.ToString("HH:mm") + "</td></tr>"
                                   + "<tr><hr/>";
                            }
                            Bodyhou = Bodyhou

               + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                     + "<tr><td><font color='blue'>Mission Type :</font></td><td>" + missionAuthorization.MissionType.Name + "</td></tr>"
                     + "<tr><td><font color='blue'>Attached Administrative Note :</font></td><td><a href='http://10.11.135.254:8080/SMS/UploadedFiles/" + missionAuthorization.files + "'>" + missionAuthorization.files + "</a></td></tr>"
                     + "<tr><td><font color='blue'>Funding :</font></td><td>" + missionAuthorization.funding + "</td></tr>"
                     + "<tr><td><font color='blue'>Objective :</font></td><td>" + missionAuthorization.MissionObjective + "</td></tr>"
                     + "<tr><td><font color='blue'>Expected Outputs :</font></td><td>" + missionAuthorization.ExpectedOutputs + "</td></tr>"
                     + "<tr><td><font color='blue'>Was The Ticket Requested 14 Days In Advance? :</font></td><td>" + Days142 + "</td></tr>"
                     + "<tr><td><font color='blue'>If Not, Please Explain :</font></td><td>" + missionAuthorization.non17 + "</td></tr>"
                     //+ "<tr><td><font color='blue'>Mission Destination :</font></td><td>" + missionAuthorization.country.country_name + " " + missionAuthorization.destinationTwon + "</td></tr>"
                     + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video ? :</font></td><td>" + canbedonee + "</td></tr>"
                     + "<tr><td><font color='blue'>Cleared By Supervisor :</font></td><td>" + ClearedBySupervisor + "</td></tr>"
                     + "<tr><td><font color='blue'>International Roaming :</font></td><td>" + Roming + "</td></tr>"
                     + "<tr><td><font color='blue'>Comment :</font></td><td>" + missionAuthorization.FollowUp + "</td></tr>";

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
                    var missionAuthorizationn = db.MissionAuthorizations.Single(u => u.MissionID == Mid);
                    missionAuthorizationn.RDORDRDSignature = "No";
                    missionAuthorizationn.RDORDRDDate = DateTime.Now;
                    db.SaveChanges();


                    var missionitinarry = db.MissionItineraries.Include(i => i.MissionAuthorization);
                    missionitinarry = missionitinarry.Where(i => i.MissionID == Mid);



                    var staffss = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
                    staffss = staffss.Where(s => s.staff_email.Contains(staffmail));
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

                        var CleredBySupervisor = "";
                        if (missionAuthorizationn.ClearedBySupervisor == true)
                        {
                            CleredBySupervisor = "Yes";
                        }
                        else
                        {
                            CleredBySupervisor = "No";
                        }
                        var Roming = "";
                        if (missionAuthorizationn.StaffSignature == "true")
                        {
                            Roming = "Yes";
                        }
                        else
                        {
                            Roming = "No";
                        }
                        var Days142 = "";
                        if (missionAuthorizationn.non18 == true)
                        {
                            Days142 = "Yes";
                        }
                        else
                        {
                            Days142 = "No";
                        }
                        MailMessage mail2 = new MailMessage();
                        mail2.To.Add(staffmail);
                        mail2.Bcc.Add("ahmed.badr@wfp.org");
                        mail2.CC.Add(cc);
                        mail2.From = new MailAddress(from);
                        mail2.Subject = "New Mission Rejected";
                        string Body2 = "<table><tr><td colspan='2'>" + "<hr/>New Mission<hr/></td></tr><tr><td><font color='blue'>Request : </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorizationn.MissionID + "</td></tr>"
                   + "<tr><td>" + "<font color='blue'>From :</font></td><td> " + missionAuthorizationn.staff.staff_first_name + " " + missionAuthorizationn.staff.staff_last_name + " </td></tr>"
                   + "<tr><td>" + "<font color='blue'>Unit :</font></td><td> " + missionAuthorizationn.staff.unit.unit_abreviation_english + "</td></tr>"
                   + "<tr><td>" + "<font color='blue'>Title :</font></td><td> " + missionAuthorizationn.staff.functional_title.functional_title_description_english + " </td></tr>"
                   + "<tr><td>" + "<font color='blue'>Contract Type :</font></td><td> " + missionAuthorizationn.staff.contract_type.contract_type_name + " </td></tr>"
                  
                   //+ "<tr><td><font color='blue'>Official Start Date :</font></td><td>" + missionAuthorizationn.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                   //+ "<tr><td><font color='blue'>Official End Date :</font></td><td>" + missionAuthorizationn.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                   //+ "<tr><td><font color='blue'>Reporting Time :</font></td><td>" + missionAuthorizationn.FromTime.Value.ToString("HH:mm") + "</td></tr>"
                   //     + "<tr><td><font color='blue'>End Time :</font></td><td>" + missionAuthorizationn.ToTime.Value.ToString("HH:mm") + "</td></tr>"
                    + "<tr><td colspan='2'>" + "<hr/>Mission Itinerary <hr/>" + "</td></tr>";

                        foreach (var item in missionitinarry)
                        {

                            Body2 = Body2

                               + "<tr><td><font color='red'>Departure:</font></td><td></td></tr>"
                                   + "<tr><td><font color='blue'>Country:</font></td><td>" + item.country.country_name + " / " + item.Twon1 + "</td></tr>"

                                   + "<tr><td><font color='blue'>Date/Time :</font></td><td>" + item.FromDate.Value.ToString("dd/MMM/yy") + "---" + item.Fromtime.Value.ToString("HH:mm") + "</td></tr>"
                                   + "<tr><td><font color='red'>Arrival:</font></td><td></td></tr>"
                                   + "<tr><td><font color='blue'>Country/City:</font></td><td>" + item.country1.country_name + " / " + item.Twon2 + "</td></tr>"

                                   + "<tr><td><font color='blue'>Date/Time :</font></td><td>" + item.ToDate.Value.ToString("dd/MMM/yy") + "---" + item.Totime.Value.ToString("HH:mm") + "</td></tr>"
                                   + "<tr><hr/>";
                        }
                        Body2 = Body2

                + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                + "<tr><td><font color='blue'>Mission Type :</font></td><td>" + missionAuthorizationn.MissionType.Name + "</td></tr>"
                   + "<tr><td><font color='blue'>Attached Administrative Note :</font></td><td><a href='http://10.11.135.254:8080/SMS/UploadedFiles/" + missionAuthorizationn.files + "'>" + missionAuthorizationn.files + "</a></td></tr>"
                   + "<tr><td><font color='blue'>Funding :</font></td><td>" + missionAuthorizationn.funding + "</td></tr>"
                   + "<tr><td><font color='blue'>Objective :</font></td><td>" + missionAuthorizationn.MissionObjective + "</td></tr>"
                   + "<tr><td><font color='blue'>Expected Outputs :</font></td><td>" + missionAuthorizationn.ExpectedOutputs + "</td></tr>"
                   + "<tr><td><font color='blue'>Was The Ticket Requested 14 Days In Advance? :</font></td><td>" + Days142 + "</td></tr>"
                   + "<tr><td><font color='blue'>If Not, Please Explain :</font></td><td>" + missionAuthorizationn.non17 + "</td></tr>"
                   //+ "<tr><td><font color='blue'>Mission Destination :</font></td><td>" + missionAuthorizationn.country.country_name + " " + missionAuthorizationn.destinationTwon + "</td></tr>"
                   + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video ? :</font></td><td>" + canbedone + "</td></tr>"
                   + "<tr><td><font color='blue'>Cleared By Supervisor :</font></td><td>" + CleredBySupervisor + "</td></tr>"
                   + "<tr><td><font color='blue'>International Roaming :</font></td><td>" + Roming + "</td></tr>"
                   + "<tr><td><font color='blue'>Comment :</font></td><td>" + missionAuthorizationn.FollowUp + "</td></tr>";

                        mail2.Body = Body2;
                        mail2.IsBodyHtml = true;
                        SmtpClient smtp2 = new SmtpClient();
                        smtp2.Host = "smtprelay.global.wfp.org";
                        smtp2.Port = 25;
                        smtp2.UseDefaultCredentials = true;
                        //smtp.Credentials = new System.Net.NetworkCredential
                        //("ahmed.badr", "Survivor2323");// Enter seders User name and password  
                        //smtp.EnableSsl = true;
                        smtp2.Send(mail2);
                    }

                    else
                    {
                        var canbedoner = "";
                        if (missionAuthorizationn.canbedone == true)
                        {
                            canbedoner = "Yes";
                        }
                        else
                        {
                            canbedoner = "No";
                        }

                        var ClearedBySupervisor = "";
                        if (missionAuthorizationn.ClearedBySupervisor == true)
                        {
                            ClearedBySupervisor = "Yes";
                        }
                        else
                        {
                            ClearedBySupervisor = "No";
                        }

                        if (cc == "muhannad.hadi@wfp.org")
                        {
                            cc = "rbc.management@wfp.org";//rbc.management@wfp.org

                        }
                        var Roming = "";
                        if (missionAuthorizationn.StaffSignature == "true")
                        {
                            Roming = "Yes";
                        }
                        else
                        {
                            Roming = "No";
                        }
                        var Days142 = "";
                        if (missionAuthorizationn.non18 == true)
                        {
                            Days142 = "Yes";
                        }
                        else
                        {
                            Days142 = "No";
                        }
                        MailMessage mail = new MailMessage();
                        mail.To.Add(staffmail);
                        mail.Bcc.Add("ahmed.badr@wfp.org");
                        mail.CC.Add(cc);
                        mail.From = new MailAddress(hou);
                        mail.Subject = "New Mission Rejected";
                        string Body = "<table><tr><td colspan='2'>" + "<hr/>New Mission<hr/></td></tr><tr><td><font color='blue'>Request : </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorizationn.MissionID + "</td></tr>"
                    + "<tr><td>" + "<font color='blue'>From:</font></td><td> " + missionAuthorizationn.staff.staff_first_name + " " + missionAuthorizationn.staff.staff_last_name + " </td></tr>"
                    + "<tr><td>" + "<font color='blue'>Unit:</font></td><td> " + missionAuthorizationn.staff.unit.unit_abreviation_english + "</td></tr>"
                    + "<tr><td>" + "<font color='blue'>Title:</font></td><td> " + missionAuthorizationn.staff.functional_title.functional_title_description_english + " </td></tr>"
                    + "<tr><td>" + "<font color='blue'>Contract Type:</font></td><td> " + missionAuthorizationn.staff.contract_type.contract_type_name + " </td></tr>"
                   
                    //+ "<tr><td><font color='blue'>Official Start Date :</font></td><td>" + missionAuthorizationn.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                    //+ "<tr><td><font color='blue'>Official End Date :</font></td><td>" + missionAuthorizationn.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                    //+ "<tr><td><font color='blue'>Reporting Time :</font></td><td>" + missionAuthorizationn.FromTime.Value.ToString("HH:mm") + "</td></tr>"
                    //+ "<tr><td><font color='blue'>End Time :</font></td><td>" + missionAuthorizationn.ToTime.Value.ToString("HH:mm") + "</td></tr>"
                    + "<tr><td colspan='2'>" + "<hr/>Mission Itinerary <hr/>" + "</td></tr>";

                        foreach (var item in missionitinarry)
                        {

                            Body = Body

                                + "<tr><td><font color='red'>Departure:</font></td><td></td></tr>"
                                   + "<tr><td><font color='blue'>Country:</font></td><td>" + item.country.country_name + " / " + item.Twon1 + "</td></tr>"

                                   + "<tr><td><font color='blue'>Date/Time :</font></td><td>" + item.FromDate.Value.ToString("dd/MMM/yy") + "---" + item.Fromtime.Value.ToString("HH:mm") + "</td></tr>"
                                   + "<tr><td><font color='red'>Arrival:</font></td><td></td></tr>"
                                   + "<tr><td><font color='blue'>Country/City:</font></td><td>" + item.country1.country_name + " / " + item.Twon2 + "</td></tr>"

                                   + "<tr><td><font color='blue'>Date/Time:</font></td><td>" + item.ToDate.Value.ToString("dd/MMM/yy") + "---" + item.Totime.Value.ToString("HH:mm") + "</td></tr>"
                                   + "<tr><hr/>";
                        }
                        Body = Body

                 + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                + "<tr><td><font color='blue'>Mission Type:</font></td><td>" + missionAuthorizationn.MissionType.Name + "</td></tr>"
                    + "<tr><td><font color='blue'>Attached Administrative Note:</font></td><td><a href='http://10.11.135.254:8080/SMS/UploadedFiles/" + missionAuthorizationn.files + "'>" + missionAuthorizationn.files + "</a></td></tr>"
                    + "<tr><td><font color='blue'>Funding:</font></td><td>" + missionAuthorizationn.funding + "</td></tr>"
                    + "<tr><td><font color='blue'>Objective:</font></td><td>" + missionAuthorizationn.MissionObjective + "</td></tr>"
                    + "<tr><td><font color='blue'>Expected Outputs:</font></td><td>" + missionAuthorizationn.ExpectedOutputs + "</td></tr>"
                    + "<tr><td><font color='blue'>Was The Ticket Requested 14 Days In Advance? :</font></td><td>" + Days142 + "</td></tr>"
                    + "<tr><td><font color='blue'>If Not, Please Explain:</font></td><td>" + missionAuthorizationn.non17 + "</td></tr>"
                    //+ "<tr><td><font color='blue'>Mission Destination :</font></td><td>" + missionAuthorizationn.country.country_name + " " + missionAuthorizationn.destinationTwon + "</td></tr>"
                    + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video ? :</font></td><td>" + canbedoner + "</td></tr>"
                    + "<tr><td><font color='blue'>Cleared By Supervisor:</font></td><td>" + ClearedBySupervisor + "</td></tr>"
                    + "<tr><td><font color='blue'>International Roaming:</font></td><td>" + Roming + "</td></tr>"
                    + "<tr><td><font color='blue'>Comment:</font></td><td>" + missionAuthorizationn.FollowUp + "</td></tr>";

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

                    }
                }
            }
            return View();

        }




        // GET: MissionAuthorizations/Delete/5
        public ActionResult Delete(int? id, int? id2, string submitted)
        {

            var staffs = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
            staffs = staffs.Where(s => s.staff_email.Contains(User.Identity.Name + "@wfp.org"));


            //ViewBag.staffid = new SelectList(db.staffs, "staffs.First().staffid", staffs.First().staff_email);
            ViewBag.staffid22 = staffs.First().staffid;

            if (id == null || id2 != ViewBag.staffid22 || submitted == "Yes")
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                string passingURL = "/RBC-SMS/MissionAuthorizations/Index/";
                return Redirect(passingURL);
            }
            MissionAuthorization missionAuthorization = db.MissionAuthorizations.Find(id);
            if (missionAuthorization == null)
            {
                string passingURL = "/RBC-SMS/MissionAuthorizations/Index/";
                return Redirect(passingURL);
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

            //////////////////////////////////////////////////////////////////////////////////////////////////////

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
            if (missionAuthorization.canbedone == true)
            {
                canbedone = "Yes";
            }
            else
            {
                canbedone = "No";
            }

            var CleredBySupervisor = "";
            if (missionAuthorization.ClearedBySupervisor == true)
            {
                CleredBySupervisor = "Yes";
            }
            else
            {
                CleredBySupervisor = "No";
            }

            //////////////////////////////


            var supersforhou = db.staffs.Include(d => d.contract_details).Include(d => d.contract_type).Include(d => d.Country_office).Include(d => d.functional_title).Include(d => d.staff2).Include(d => d.sub_office).Include(d => d.unit).Include(d => d.country).Include(d => d.governorate);
            supersforhou = supersforhou.Where(d => d.staffid == staffsupervisor);
            var superforhounitsname = supersforhou.First().staff_first_name + " " + staffshou.First().staff_last_name;
            var houmailUPDATE = supersforhou.First().staff_email;

            if (houmailUPDATE == "muhannad.hadi@wfp.org")
            {
                houmailUPDATE = "rbc.management@wfp.org"; //rbc.management@wfp.org

            }

            /////////////////////////

            MailMessage mailhou = new MailMessage();
            mailhou.To.Add("ahmed.badr@wfp.org"); // RD/DRD OFFICE/rbc.management@wfp.org
            //mail.CC.Add(cc);
            mailhou.Bcc.Add("ahmed.badr@wfp.org");
            mailhou.From = new MailAddress(from);
            mailhou.Subject = "Mission Authorization Deleted";

            string Bodyhou = "<table><tr><td colspan='2'>" + "<hr/>New Mission<hr/></td></tr><tr><td><font color='blue'>Request : </font></td><td>Mission</td></tr><tr><td><font color='blue'>Mission ID :</font></td><td>" + missionAuthorization.MissionID + "</td></tr>"
                + "<tr><td>" + "<font color='blue'>From:</font></td><td>" + staffs.First().staff_first_name + " " + staffs.First().staff_last_name + "</td></tr>"
                + "<tr><td colspan='2'>" + "<hr/>Mission Information<hr/>" + "</td></tr>"
                //+ "<tr><td><font color='blue'>From:</font></td><td>" + missionAuthorization.FromDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                //+ "<tr><td><font color='blue'>To:</font></td><td>" + missionAuthorization.ToDate.Value.ToString("dd/MMM/yy") + "</td></tr>"
                + "<tr><td><font color='blue'>Funding:</font></td><td>" + missionAuthorization.funding + "</td></tr>"
                + "<tr><td><font color='blue'>Objective:</font></td><td>" + missionAuthorization.MissionObjective + "</td></tr>"
                + "<tr><td><font color='blue'>Mission Itinerary:</font></td><td>" + missionAuthorization.MissionItinerary + "</td></tr>"
                + "<tr><td><font color='blue'>Can This Meeting be <br> Done Over Voice/Video?:</font></td><td>" + canbedone + "</td></tr>"
                + "<tr><td><font color='blue'>Cleared By Supervisor:</font></td><td>" + CleredBySupervisor + "</td></tr>"
                + "<tr><td><a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid="
                + missionAuthorization.MissionID
                + "&RDORDRDSignature=Yes&FromDate=" + missionAuthorization.FromDate
                + " &ToDate=" + missionAuthorization.ToDate
                + " &to=" + to + "&staffmail=" + staffs.First().staff_email
                + "'>Accept</a></td><td><a href='http://10.11.135.254:8080/RBC-SMS/MissionAuthorizations/Approval2?Mid=" + missionAuthorization.MissionID
                + "&RDORDRDSignature=NO & FromDate=" + missionAuthorization.FromDate
                + " &ToDate=" + missionAuthorization.ToDate
                + "&to=" + to
                + "&staffmail=" + staffs.First().staff_email
                + "'>Reject</a>" + "</td></tr></table>";

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
            //smtphou.Send(mailhou);
            /////////////////////////////////////////////////////////////////////////////////////////////////////
            return RedirectToAction("Index");
        }

        public ActionResult CreateItinerary(MissionAuthorization newrecord, int? MissionID)
        {
            
            
            ViewBag.FromID = new SelectList(db.countries, "countryid", "country_name");
            ViewBag.ToID = new SelectList(db.countries, "countryid", "country_name");

            var staffser = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
            staffser = staffser.Where(s => s.staff_email.Contains(User.Identity.Name + "@wfp.org"));
            var staffid = staffser.First().staffid;

            //newrecord.staffid = staffid;
            //db.MissionAuthorizations.Add(newrecord);
            //db.SaveChanges();
            //int newrow = newrecord.MissionID;

           
            var latestmission2 = db.MissionAuthorizations.Include(s => s.MissionItineraries).Include(s => s.staff);
            latestmission2 = latestmission2.Where(s => s.staffid == staffid && s.non5 == 11);
            //ViewBag.MissionID = latestmission2.First().MissionID;
            //var staffmissions = db.MissionAuthorizations.Include(d => d.staff);
            //staffmissions = staffmissions.Where(u => u.staffid == staffid);



            //ViewBag.MissionID = new SelectList(staffmissions, "MissionID", "MissionID");
            if (latestmission2.Any())
            {
            ViewBag.MissionID = latestmission2.First().MissionID;
                }
            ViewBag.non3 = staffser.First().staffid;
            return View();
        }




        /// <summary>
        /// ////////////////////////////////////////////////////
        public ActionResult staffonbehalfperson(int? id)
        {

            ViewBag.ss = id;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            staff staff = db.staffs.Find(id);
            if (staff == null)
            {
                return HttpNotFound();
            }
            var staffser = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.staff1).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
            staffser = staffser.Where(s => s.staffid == id);
            if (staffser.Any())
            {
                  ViewBag.stafffullname = staffser.First().staff_first_name + " " + staffser.First().staff_last_name;
            ViewBag.stafnationality = staffser.First().staff_nationality;
            return View();
            }
               
            else {
                ViewBag.stafffullname = "";
                ViewBag.stafnationality = "";
                return View();
            }
        }
        /// ///////////////////////////////////////////////////
        /// </summary>
        /// <param name="missionItinerary"></param>
        /// <returns></returns>
      
        [HttpPost, ActionName("CreateItinerary")]

        public ActionResult CreateItinerary([Bind(Include = "ID,MissionID,Description,FromID,FromDate,ToID,ToDate,non1,non2,non3,non4,Fromtime,Totime,Twon1,Twon2")] MissionItinerary missionItinerary)
        {


            if (ModelState.IsValid)
            {


                db.MissionItineraries.Add(missionItinerary);
                db.SaveChanges();
                return RedirectToAction("Create", "MissionAuthorizations");
            }

            ViewBag.FromID = new SelectList(db.countries, "countryid", "country_name", missionItinerary.FromID);
            ViewBag.ToID = new SelectList(db.countries, "countryid", "country_name", missionItinerary.ToID);
            ViewBag.MissionID = new SelectList(db.MissionAuthorizations, "MissionID", "MissionID", missionItinerary.MissionID);
            //return View(missionItinerary);
            return RedirectToAction("Create", "MissionAuthorizations");
            //return PartialView("_createiti");
        }



        //////////////////////////////////////////////////////////edit create itinerary
        public ActionResult CreateItineraryforedit(MissionAuthorization newrecord, int? id)
        {
            ViewBag.FromID = new SelectList(db.countries, "countryid", "country_name");
            ViewBag.ToID = new SelectList(db.countries, "countryid", "country_name");

            var staffser = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
            staffser = staffser.Where(s => s.staff_email.Contains(User.Identity.Name + "@wfp.org"));
            var staffid = staffser.First().staffid;

            //newrecord.staffid = staffid;
            //db.MissionAuthorizations.Add(newrecord);
            //db.SaveChanges();
            //int newrow = newrecord.MissionID;


            var latestmission2 = db.MissionAuthorizations.Include(s => s.MissionItineraries).Include(s => s.staff);
            latestmission2 = latestmission2.Where(s => s.staffid == staffid && s.MissionID == id);
            //ViewBag.MissionID = latestmission2.First().MissionID;
            //var staffmissions = db.MissionAuthorizations.Include(d => d.staff);
            //staffmissions = staffmissions.Where(u => u.staffid == staffid);



            //ViewBag.MissionID = new SelectList(staffmissions, "MissionID", "MissionID");
            if (latestmission2.Any())
            {
                ViewBag.MissionID = latestmission2.First().MissionID;
            }
            ViewBag.non3 = staffser.First().staffid;
            return View();
        }

        [HttpPost, ActionName("CreateItineraryforedit")]
        public ActionResult CreateItineraryforedit([Bind(Include = "ID,MissionID,Description,FromID,FromDate,ToID,ToDate,non1,non2,non3,non4,Fromtime,Totime,Twon1,Twon2")] MissionItinerary missionItinerary)
        {

            var staffser = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
            staffser = staffser.Where(s => s.staff_email.Contains(User.Identity.Name + "@wfp.org"));
            var staffid = staffser.First().staffid;
            var staffmail = staffser.First().staff_email ;

            //var itineryID = db.MissionItineraries.Include(d => d.MissionAuthorization);
            //itineryID = itineryID.Where(u => u.ID  == id);
            //var moID = itineryID.First().MissionID;

            if (ModelState.IsValid)
            {


                db.MissionItineraries.Add(missionItinerary);
                db.SaveChanges();
                return RedirectToAction("Edit", new { id = missionItinerary.MissionID, id2 = staffid, staffmail });
            }

            ViewBag.FromID = new SelectList(db.countries, "countryid", "country_name", missionItinerary.FromID);
            ViewBag.ToID = new SelectList(db.countries, "countryid", "country_name", missionItinerary.ToID);
            ViewBag.MissionID = new SelectList(db.MissionAuthorizations, "MissionID", "MissionID", missionItinerary.MissionID);
            //return View(missionItinerary);
           return RedirectToAction("Edit", new { id = missionItinerary.MissionID, id2 = staffid, staffmail });
            //return PartialView("_createiti");
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public ActionResult CreateItineraryforeditonbehalfof(MissionAuthorization newrecord, int? id)
        {
            ViewBag.FromID = new SelectList(db.countries, "countryid", "country_name");
            ViewBag.ToID = new SelectList(db.countries, "countryid", "country_name");

            var staffser = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
            staffser = staffser.Where(s => s.staff_email.Contains(User.Identity.Name + "@wfp.org"));
            var staffid = staffser.First().staffid;

            //newrecord.staffid = staffid;
            //db.MissionAuthorizations.Add(newrecord);
            //db.SaveChanges();
            //int newrow = newrecord.MissionID;


            var latestmission2 = db.MissionAuthorizations.Include(s => s.MissionItineraries).Include(s => s.staff);
            latestmission2 = latestmission2.Where(s => s.staffid == staffid && s.MissionID == id);
            //ViewBag.MissionID = latestmission2.First().MissionID;
            //var staffmissions = db.MissionAuthorizations.Include(d => d.staff);
            //staffmissions = staffmissions.Where(u => u.staffid == staffid);



            //ViewBag.MissionID = new SelectList(staffmissions, "MissionID", "MissionID");
            if (latestmission2.Any())
            {
                ViewBag.MissionID = latestmission2.First().MissionID;
            }
            ViewBag.non3 = staffser.First().staffid;
            return View();
        }

        [HttpPost, ActionName("CreateItineraryforeditonbehalfof")]

        public ActionResult CreateItineraryforeditonbehalfof([Bind(Include = "ID,MissionID,Description,FromID,FromDate,ToID,ToDate,non1,non2,non3,non4,Fromtime,Totime,Twon1,Twon2")] MissionItinerary missionItinerary)
        {

            var staffser = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
            staffser = staffser.Where(s => s.staff_email.Contains(User.Identity.Name + "@wfp.org"));
            var staffid = staffser.First().staffid;
            var staffmail = staffser.First().staff_email;

            //var itineryID = db.MissionItineraries.Include(d => d.MissionAuthorization);
            //itineryID = itineryID.Where(u => u.ID  == id);
            //var moID = itineryID.First().MissionID;

            if (ModelState.IsValid)
            {


                db.MissionItineraries.Add(missionItinerary);
                db.SaveChanges();
                return RedirectToAction("EditBehalfof", new { id = missionItinerary.MissionID, id2 = staffid, staffmail });
            }

            ViewBag.FromID = new SelectList(db.countries, "countryid", "country_name", missionItinerary.FromID);
            ViewBag.ToID = new SelectList(db.countries, "countryid", "country_name", missionItinerary.ToID);
            ViewBag.MissionID = new SelectList(db.MissionAuthorizations, "MissionID", "MissionID", missionItinerary.MissionID);
            //return View(missionItinerary);
            return RedirectToAction("EditBehalfof", new { id = missionItinerary.MissionID, id2 = staffid, staffmail });
            //return PartialView("_createiti");
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        // GET: MissionItineraries/Edit/5

        public ActionResult IndexItinerary(string sortOrder, string currentFilter, string searchString, int? page, string node)
        {



            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = String.IsNullOrEmpty(sortOrder) ? "First_Name" : "";
            var students = from s in db.MissionItineraries 
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

            var latestmission2 = db.MissionAuthorizations.Include(s => s.MissionItineraries).Include(s => s.staff);
            latestmission2 = latestmission2.Where(s => s.staffid == staffid && s.non5 == 11);

            if (latestmission2.Any() == false)
            {
                var missionItineraries22 = db.MissionItineraries.Include(m => m.country).Include(m => m.country1).Include(m => m.MissionAuthorization);
                missionItineraries22 = missionItineraries22.Where(u => u.non3 == staffid );

            }
           
            var missionItineraries = db.MissionItineraries.Include(m => m.country).Include(m => m.country1).Include(m => m.MissionAuthorization);
            missionItineraries = missionItineraries.Where(u => u.non3 == staffid && u.MissionID == latestmission2.FirstOrDefault().MissionID);
            ViewBag.id = latestmission2.FirstOrDefault().MissionID;
          

            if (searchString != null)
            {
                
                missionItineraries = missionItineraries.Where(s => s.Description  == (searchString)
                                       || s.Description  == (searchString));

            }

            switch (sortOrder)
            {
                case "First_Name":
                    missionItineraries = missionItineraries.OrderByDescending(s => s.Description );
                    break;
                case "name_desc":
                    missionItineraries = missionItineraries.OrderByDescending(s => s.MissionID);
                    break;

                default:  // Name ascending 
                    missionItineraries = missionItineraries.OrderBy(s => s.MissionID);
                    break;
            }


            ViewBag.rdDrd = 1;
            ViewBag.FromID = new SelectList(db.countries, "countryid", "country_name");
            ViewBag.ToID = new SelectList(db.countries, "countryid", "country_name");
            ViewBag.MissionID = new SelectList(db.MissionAuthorizations, "MissionID", "MissionID");


            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(missionItineraries.ToPagedList(pageNumber, pageSize));

        }

        
        public ActionResult indexitin(string sortOrder, string currentFilter, string searchString, int? page, string node, int? id)
        {
           
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

            var students = from s in db.MissionItineraries
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

            var missionItineraries = db.MissionItineraries.Include(m => m.country).Include(m => m.country1).Include(m => m.MissionAuthorization);
            missionItineraries = missionItineraries.Where(s => s.MissionID == (id));

            if (!String.IsNullOrEmpty(searchString))
            {
                missionItineraries = missionItineraries.Where(s => s.Equals(searchString)
                                       || s.country.Equals(searchString));

            }

            switch (sortOrder)
            {
                case "First_Name":
                    missionItineraries = missionItineraries.OrderByDescending(s => s.FromDate);
                    break;
                case "name_desc":
                    missionItineraries = missionItineraries.OrderByDescending(s => s.FromDate);
                    break;

                default:  // Name ascending 
                    missionItineraries = missionItineraries.OrderBy(s => s.FromDate);
                    break;
            }


            ViewBag.rdDrd = 1;
            ViewBag.FromID = new SelectList(db.countries, "countryid", "country_name");
            ViewBag.ToID = new SelectList(db.countries, "countryid", "country_name");
            ViewBag.MissionID = new SelectList(db.MissionAuthorizations, "MissionID", "MissionID");
            ViewBag.id22 = id;

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(missionItineraries.ToPagedList(pageNumber, pageSize));
            //return View();
        }


        // GET: MissionItineraries/Delete/5
        public ActionResult DeleteItinerary(int? id)
        {

            var staffser = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
            staffser = staffser.Where(s => s.staff_email.Contains(User.Identity.Name + "@wfp.org"));
            var staffid = staffser.First().staffid;

            var itineryID = db.MissionItineraries.Include(d => d.MissionAuthorization);
            itineryID = itineryID.Where(u => u.non3 == staffid && u.MissionID == id);


            //ViewBag.ID2 = new SelectList(itineryID, "ID", "ID");
            ViewBag.ID= new SelectList(itineryID, "ID", "ID");
            //ViewBag.ID = new SelectList(db.MissionItineraries, "ID", "ID");

            return View();
        }

        // POST: MissionItineraries/Delete/5
        [HttpPost, ActionName("DeleteItinerary")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed2(int id)
        {
            MissionItinerary missionItinerary = db.MissionItineraries.Find(id);
            db.MissionItineraries.Remove(missionItinerary);
            db.SaveChanges();
            return RedirectToAction("Create");
        }

        public ActionResult DeleteItineraryforedit(int? id)
        {

            var staffser = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
            staffser = staffser.Where(s => s.staff_email.Contains(User.Identity.Name + "@wfp.org"));
            var staffid = staffser.First().staffid;

            var itineryID = db.MissionItineraries.Include(d => d.MissionAuthorization);
            itineryID = itineryID.Where(u => u.non3 == staffid && u.MissionID == id);


            //ViewBag.ID2 = new SelectList(itineryID, "ID", "ID");
            ViewBag.ID = new SelectList(itineryID, "ID", "ID");
            //ViewBag.ID = new SelectList(db.MissionItineraries, "ID", "ID");

            return View();
        }

        // POST: MissionItineraries/Delete/5
        [HttpPost, ActionName("DeleteItineraryforedit")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed22(int id)
        {
            //MissionItinerary missionItinerary = db.MissionItineraries.Find(id);
            //db.MissionItineraries.Remove(missionItinerary);
            //db.SaveChanges();

            var staffser = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
            staffser = staffser.Where(s => s.staff_email.Contains(User.Identity.Name + "@wfp.org"));
            var staffid = staffser.First().staffid;
            var staffmail = staffser.First().staff_email ;

            var itineryID = db.MissionItineraries.Include(d => d.MissionAuthorization);
            itineryID = itineryID.Where(u => u.ID  == id);
            var moID = itineryID.First().MissionID;

            MissionItinerary missionItinerary = db.MissionItineraries.Find(id);
            db.MissionItineraries.Remove(missionItinerary);
            db.SaveChanges();

            return RedirectToAction("Edit", new { id = moID, id2 = staffid, staffmail });
        }

        public ActionResult DeleteItineraryforeditonbehalfof(int? id)
        {

            var staffser = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
            staffser = staffser.Where(s => s.staff_email.Contains(User.Identity.Name + "@wfp.org"));
            var staffid = staffser.First().staffid;

            var itineryID = db.MissionItineraries.Include(d => d.MissionAuthorization);
            itineryID = itineryID.Where(u => u.non3 == staffid && u.MissionID == id);


            //ViewBag.ID2 = new SelectList(itineryID, "ID", "ID");
            ViewBag.ID = new SelectList(itineryID, "ID", "ID");
            //ViewBag.ID = new SelectList(db.MissionItineraries, "ID", "ID");

            return View();
        }

        // POST: MissionItineraries/Delete/5
        [HttpPost, ActionName("DeleteItineraryforeditonbehalfof")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed22onbehalfof(int id)
        {
            //MissionItinerary missionItinerary = db.MissionItineraries.Find(id);
            //db.MissionItineraries.Remove(missionItinerary);
            //db.SaveChanges();

            var staffser = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
            staffser = staffser.Where(s => s.staff_email.Contains(User.Identity.Name + "@wfp.org"));
            var staffid = staffser.First().staffid;
            var staffmail = staffser.First().staff_email;

            var itineryID = db.MissionItineraries.Include(d => d.MissionAuthorization);
            itineryID = itineryID.Where(u => u.ID == id);
            var moID = itineryID.First().MissionID;

            MissionItinerary missionItinerary = db.MissionItineraries.Find(id);
            db.MissionItineraries.Remove(missionItinerary);
            db.SaveChanges();

            return RedirectToAction("EditBehalfof", new { id = moID, id2 = staffid, staffmail });
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

