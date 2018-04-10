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
using System.Collections;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;


namespace WFPtest.Controllers
{


    public class staffController : Controller
    {
        /// <summary>
        /// AD Users method
        /// </summary>
/////////////////////////////////////////////////////////// AD Sync//////////////////////////////////////////////////////////////
        //ArrayList GetADGroupUsers(string groupName)
        //{
        //    ArrayList myItems = new ArrayList();
        //    PrincipalContext ctx = new PrincipalContext(ContextType.Domain, "global");
        //    GroupPrincipal group = GroupPrincipal.FindByIdentity(ctx, "GG-EGRBCAI Users");
        //    PrincipalSearchResult<Principal> members = group.GetMembers();

        //    foreach (Principal user in members)
        //    {
        //        myItems.Add(user.UserPrincipalName);
        //        //myItems.Add(user.SamAccountName );
        //        //myItems.Add(user.UserPrincipalName); //E-mail
        //        //myItems.Add(user.Name); //Full Name (Second//First)
        //        //myItems.Add(user.DisplayName);  //Full Name (Second//First)
        //        //myItems.Add(user.Description); //empty
        //        //myItems.Add(user.ContextType ); //Domain

        //    }
        //    return myItems;

        //    //SearchResult result;
        //    //DirectorySearcher search = new DirectorySearcher();
        //    //search.Filter = String.Format("(cn={0})", groupName);
        //    //search.PropertiesToLoad.Add("member");
        //    //search.PropertiesToLoad.Add("mail");
        //    //result = search.FindOne();
        //    //ArrayList usersinfo = new ArrayList();
        //    //usersinfo  = members;
        //    //return members;
        //}
        ////////////////////////////////////////// AD Sync //////////////////////////////////////////////////////////////

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

        /// <summary>
        /// AD Users end
        /// </summary>



        private WFPEntities1 db = new WFPEntities1();

        // GET: /staff/
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)

        {
            ////////////////////////////////////////////////// AD Sync /////////////////////////////////////////////////////////////////////////////
            //ArrayList newuser = GetADGroupUsers("GG-EGRB ICT");
            ////ViewBag.ADusers = GetADGroupUsers("GG-EGRB ICT").IndexOf ("user") ;
            //foreach (object el in newuser)
            //{
            //    string ff = newuser.Capacity.ToString();
            //    string username = el.ToString();
            //    //string start = "=";
            //    //string end = ",";
            //    //int startIndex = username.IndexOf("=");
            //    //int endIndex = username.LastIndexOf(",");
            //    //ViewBag.ADusers = username.Substring(username .IndexOf ("="), username .IndexOf (","));

            //    //ViewBag.ADusers = (el);

            //    char[] commaSeparator = new char[] { ',' };
            //    string[] authors = username.Split(commaSeparator, StringSplitOptions.None);
            //    foreach (string author in authors)
            //    {
            //        var staffemailcom = author;
            //        var staffscomp = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
            //        staffscomp = staffscomp.Where(s => s.staff_email == staffemailcom);
            //        //var newstaff = staffscomp.First().staff_email.DefaultIfEmpty();
            //        if (staffscomp.Count() == 0)
            //        {
            //            //staff staffemailnew = null ;
            //            //staffemailnew.staff_email = author;
            //            //db.staffs.Add(staffemailnew) ;
            //            var staffemailnew = db.staffs.Add(
            //             new staff { staff_email = author }).staff_email;
            //            db.SaveChanges();

            //        }

            //        //ViewBag.ADusers += author + "      ";

            //    }

            //}
            //////////////////////////////////////////////////////////// AD Sync ////////////////////////////////////////////////////////////////////////         


            //    staff NewuUser = new staff();
            //    NewuUser.staff_first_name = ADStaff.;
            //    NewuUser.Stone = Model.Stone;
            //    NewuUser.Pound = Model.Pound;
            //    NewuUser.Date = System.DateTime.Now;

            //    db.Weights.Add(Model);
            //    db.SaveChanges();
           


            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = String.IsNullOrEmpty(sortOrder) ? "First_Name" : "";
            var students = from s in db.staffs
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
            var staffs = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit).Include(s => s.country).Include(s => s.governorate);
            staffs = staffs.Where(s => s.staff_first_name != "THERE IS NO NAME !!!");

            if (!String.IsNullOrEmpty(searchString))
            {
                staffs = staffs.Where(s => s.staff_last_name.Contains(searchString)
                                       || s.staff_first_name.Contains(searchString)
                                       || s.staff_email.Contains(searchString)
                                       || s.unit .unit_description_english  .Contains(searchString));
                                       
            }

            switch (sortOrder)
            {
               case "First_Name":
                    staffs = staffs.OrderByDescending(s => s.staff_first_name);
                    break;
               case "name_desc":
                    staffs = staffs.OrderByDescending(s => s.staff_last_name);
                    break;
               
                default:  // Name ascending 
                    staffs = staffs.OrderBy(s => s.staff_email   );
                     break;
            }

        int pageSize = 10;
        int pageNumber = (page ?? 1);
        return View(staffs.ToPagedList(pageNumber, pageSize));

        //return View(staffs.ToList());
        }

        //public ViewResult Index(string sortOrder, string searchString)
        //{
        //    ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
        //    ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
        //    var students = from s in db.staffs
        //                   select s;
        //    if (!String.IsNullOrEmpty(searchString))
        //    {
        //        students = students.Where(s => s.staff_last_name.Contains(searchString)
        //                               || s.staff_first_name.Contains(searchString));
        //    }
        //    switch (sortOrder)
        //    {
        //        case "name_desc":
        //            students = students.OrderByDescending(s => s.staff_last_name);
        //            break;
        //        case "Date":
        //            students = students.OrderBy(s => s.staff_dob);
        //            break;
        //        case "date_desc":
        //            students = students.OrderByDescending(s => s.staff_eod);
        //            break;
        //        default:
        //            students = students.OrderBy(s => s.staff_last_name);
        //            break;
        //    }

        //    return View(students.ToList());
        //}


        //public ActionResult OrderByfirstName()
        //{
        //    var staffs = from s in db.staffs
        //                 orderby s.staff_first_name ascending
        //                 select s;
        //    return View (staffs);

        //}

        public ActionResult ConDetails(int? id)
        {
            if (id == null)
            {

                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //var staffs = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit);
            //return View(staffs.ToList());
            //var contract_details = db.contract_details.Include(s => s.Fund);
            contract_details contract_details = db.contract_details.Find(id);
            if (contract_details == null)
            {
                return HttpNotFound();
            }

            return View(contract_details);
        }






        // GET: /contractdetails/Edit/5
        public ActionResult EditCon(int? id)
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
        public ActionResult EditCon([Bind(Include = "contractdetailsid,contract_details_id,contract_details_description_english,contract_details_abreviation_english,contract_details_description_french,contract_details_abreviation_french,contract_details_status,contract_details_created_by,contract_details_created_datetime,contract_details_last_modified_by,contract_details_last_modified_datetime,contract_details_deleted_by,contract_details_deleted_datetime,contract_type_code,fundid,staffid")] contract_details contract_details)
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

              
                //db.SaveChanges();
                //return RedirectToAction("Index");

                
                return RedirectToAction("Index");
            }

            ViewBag.fundid = new SelectList(db.Funds, "fundid", "resource");
            ViewBag.staffid = new SelectList(db.staffs, "staffid", "staff_email");
            return View(contract_details);
        }








        //// GET: /contractdetails/staffracking/5
        //public ActionResult staffracking(int? id)
        //{
        //    if (id == null)
        //    {

        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    contract_details contract_details = db.contract_details.Find(id);
        //    if (contract_details == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.fundid = new SelectList(db.Funds, "fundid", "resource", contract_details.fundid);
        //    ViewBag.staffid = new SelectList(db.staffs, "staffid", "staff_email", contract_details.staffid);



        //    db.SaveChanges();

        //    return View(contract_details);



        //}

        //// POST: /staffracking/staffracking
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult staffracking([Bind(Include = "contractdetailsid,contract_details_id,contract_details_description_english,contract_details_abreviation_english,contract_details_description_french,contract_details_abreviation_french,contract_details_status,contract_details_created_by,contract_details_created_datetime,contract_details_last_modified_by,contract_details_last_modified_datetime,contract_details_deleted_by,contract_details_deleted_datetime,contract_type_code,fundid,staffid")] contract_details contract_details)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(contract_details).State = EntityState.Modified;
        //        db.SaveChanges();

        //        //var res = (from c in db.staffs
        //        //           where c.staffid == contract_details.staffid
        //        //           select c).SingleOrDefault();

        //        //res.stuff_contract_details = contract_details.contractdetailsid;

        //        //db.SaveChanges();

        //        //var res = (from c in db.staffs
        //        //           where c.staffid == contract_details.staffid
        //        //           select c).SingleOrDefault();

        //        //res.stuff_contract_details = null;


        //        //db.SaveChanges();
        //        //return RedirectToAction("Index");


        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.fundid = new SelectList(db.Funds, "fundid", "resource");
        //    ViewBag.staffid = new SelectList(db.staffs, "staffid", "staff_email");
        //    return View(contract_details);
        //}


        


        public ActionResult SuperDetails(int? id)
        {
            //var staffs = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit);
            //return View(staffs.ToList());
            //var contract_details = db.contract_details.Include(s => s.Fund);
            staff staff = db.staffs.Find(id);
            return View(staff);
        }

        public ActionResult wardenDetails(int? id)
        {
            //var staffs = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit);
            //return View(staffs.ToList());
            //var contract_details = db.contract_details.Include(s => s.Fund);
            staff staff = db.staffs.Find(id);
            return View(staff);
        }

        public ActionResult alternate_warden_1Details(int? id)
        {
            //var staffs = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit);
            //return View(staffs.ToList());
            //var contract_details = db.contract_details.Include(s => s.Fund);
            staff staff = db.staffs.Find(id);
            return View(staff);
        }

        public ActionResult alternate_warden_2Details(int? id)
        {
            //var staffs = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit);
            //return View(staffs.ToList());
            //var contract_details = db.contract_details.Include(s => s.Fund);
            staff staff = db.staffs.Find(id);
            return View(staff);
        }

        public ActionResult StaffDependents(int? id)
        {
            //var staffs = db.staffs.Include(s => s.contract_details).Include(s => s.contract_type).Include(s => s.Country_office).Include(s => s.functional_title).Include(s => s.staff2).Include(s => s.sub_office).Include(s => s.unit);
            //return View(staffs.ToList());
            //var contract_details = db.contract_details.Include(s => s.Fund);

            //dependent dependent = db.dependents.Find(id);
            //return View(dependent);

            //dependent dependent = db.dependents.FirstOrDefault (p => p.staffid == id);
            //return View(dependent);
            var result = db.dependents.Where(p => p.staffid == id);
            return View(result.ToList());

                                    
           }


        // GET: /staff/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            staff staff = db.staffs.Find(id);
            if (staff == null)
            {
                return HttpNotFound();
            }
            return View(staff);
        }

        // GET: /staff/Create
        public ActionResult Create()
        {
            ViewBag.stuff_contract_details = new SelectList(db.contract_details, "contractdetailsid", "contract_details_id");
            ViewBag.contract_type_id = new SelectList(db.contract_type, "contractTypeid", "contract_type_name");
            ViewBag.country_office_id = new SelectList(db.Country_office, "countryofficeid", "office_description_english");
            ViewBag.functional_title_id = new SelectList(db.functional_title, "functionaltitleid", "functional_title_description_english");
            ViewBag.staff_supervisorid = new SelectList(db.staffs, "staffid", "staff_email");
            ViewBag.staff_warden = new SelectList(db.staffs, "staffid", "staff_email");
            ViewBag.alternate_warden_1 = new SelectList(db.staffs, "staffid", "staff_email");
            ViewBag.alternate_warden_2 = new SelectList(db.staffs, "staffid", "staff_email");
            ViewBag.sub_office_id = new SelectList(db.sub_office, "subofficeid", "office_description_english");
            ViewBag.unit_id = new SelectList(db.units, "unitid", "unit_description_english");
            ViewBag.countryid = new SelectList(db.countries, "countryid", "country_name");
            ViewBag.governorates_code = new SelectList(db.governorates, "governorates_code", "governorates_name");
            ViewBag.areacode = new SelectList(db.districts, "areacode", "area_name");
            ViewBag.CatID = new SelectList(db.categories, "CatID", "CatName");
            ViewBag.WardenZone = new SelectList(db.districts, "areacode", "area_name");
            return View();
        }

        // POST: /staff/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "staffid,staff_id,country_office_id,sub_office_id,unit_id,contract_type_id,functional_title_id,staff_login,staff_first_name,staff_last_name,staff_created_by,staff_created_datetime,staff_status,staff_index_number,staff_vendor_number,staff_nationality,staff_address,staff_dob,staff_eod,staff_nte,staff_extension_number,staff_email,staff_access_main,staff_access_dts,staff_access_tb,staff_access_ct,staff_access_li,staff_access_lc,staff_access_sr,staff_access_admin,staff_last_updated_by,staff_last_updated_datetime,staff_deleted_by,staff_deleted_datetime,staff_supervisorid,stuff_contract_details,staff_warden,alternate_warden_1,alternate_warden_2,Call_Sign,Landline,Mobile,countryid,governorates_code,areacode,international,CatID,WardenZone")] staff staff)
        {
            if (ModelState.IsValid)
            {
                db.staffs.Add(staff);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.stuff_contract_details = new SelectList(db.contract_details, "contractdetailsid", "contract_details_id", staff.stuff_contract_details);
            ViewBag.contract_type_id = new SelectList(db.contract_type, "contractTypeid", "contract_type_name", staff.contract_type_id);
            ViewBag.country_office_id = new SelectList(db.Country_office, "countryofficeid", "office_description_english", staff.country_office_id);
            ViewBag.functional_title_id = new SelectList(db.functional_title, "functionaltitleid", "functional_title_description_english", staff.functional_title_id);
            ViewBag.staff_supervisorid = new SelectList(db.staffs, "staffid", "staff_email", staff.staff_supervisorid);
            ViewBag.staff_warden = new SelectList(db.staffs, "staffid", "staff_email", staff.staff_warden );
            ViewBag.alternate_warden_1 = new SelectList(db.staffs, "staffid", "staff_email", staff.alternate_warden_1 );
            ViewBag.alternate_warden_2 = new SelectList(db.staffs, "staffid", "staff_email", staff.alternate_warden_2 );
            ViewBag.sub_office_id = new SelectList(db.sub_office, "subofficeid", "office_description_english", staff.sub_office_id);
            ViewBag.unit_id = new SelectList(db.units, "unitid", "unit_description_english", staff.unit_id);
            ViewBag.countryid = new SelectList(db.countries, "countryid", "country_name", staff.countryid );
            ViewBag.governorates_code = new SelectList(db.governorates, "governorates_code", "governorates_name", staff.governorates_code );
            ViewBag.areacode = new SelectList(db.districts, "areacode", "area_name", staff.areacode);
            ViewBag.CatID = new SelectList(db.categories, "CatID", "CatName", staff.CatID);
            ViewBag.WardenZone = new SelectList(db.districts, "areacode", "area_name", staff.WardenZone);
            return View(staff);
        }



        // GET: /staff/UserCreateStaff
        public ActionResult UserCreateStaff()
        {
            ViewBag.stuff_contract_details = new SelectList(db.contract_details, "contractdetailsid", "contract_details_id");
            ViewBag.contract_type_id = new SelectList(db.contract_type, "contractTypeid", "contract_type_name");
            ViewBag.country_office_id = new SelectList(db.Country_office, "countryofficeid", "office_description_english");
            ViewBag.functional_title_id = new SelectList(db.functional_title.OrderBy(functional_title => functional_title.functional_title_description_english), "functionaltitleid", "functional_title_description_english");
            ViewBag.staff_supervisorid = new SelectList(db.staffs.OrderBy(staffs => staffs.staff_email), "staffid", "staff_email");
            ViewBag.staff_warden = new SelectList(db.staffs, "staffid", "staff_email");
            ViewBag.alternate_warden_1 = new SelectList(db.staffs, "staffid", "staff_email");
            ViewBag.alternate_warden_2 = new SelectList(db.staffs, "staffid", "staff_email");
            ViewBag.sub_office_id = new SelectList(db.sub_office, "subofficeid", "office_description_english");
            ViewBag.unit_id = new SelectList(db.units, "unitid", "unit_description_english");
            ViewBag.countryid = new SelectList(db.countries, "countryid", "country_name");
            ViewBag.governorates_code = new SelectList(db.governorates, "governorates_code", "governorates_name");
            ViewBag.areacode = new SelectList(db.districts, "areacode", "area_name");
            ViewBag.CatID = new SelectList(db.categories, "CatID", "CatName");
            ViewBag.WardenZone = new SelectList(db.districts, "areacode", "area_name");
            return View();
        }

        // POST: /staff/UserCreateStaff
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserCreateStaff([Bind(Include = "staffid,staff_id,country_office_id,sub_office_id,unit_id,contract_type_id,functional_title_id,staff_login,staff_first_name,staff_last_name,staff_created_by,staff_created_datetime,staff_status,staff_index_number,staff_vendor_number,staff_nationality,staff_address,staff_dob,staff_eod,staff_nte,staff_extension_number,staff_email,staff_access_main,staff_access_dts,staff_access_tb,staff_access_ct,staff_access_li,staff_access_lc,staff_access_sr,staff_access_admin,staff_last_updated_by,staff_last_updated_datetime,staff_deleted_by,staff_deleted_datetime,staff_supervisorid,stuff_contract_details,staff_warden,alternate_warden_1,alternate_warden_2,Call_Sign,Landline,Mobile,countryid,governorates_code,areacode,international,CatID,WardenZone")] staff staff)
        {
            if (ModelState.IsValid)
            {
                db.staffs.Add(staff);
                db.SaveChanges();
                return RedirectToAction("Create", "CourseRegs");
            }

            ViewBag.stuff_contract_details = new SelectList(db.contract_details, "contractdetailsid", "contract_details_id", staff.stuff_contract_details);
            ViewBag.contract_type_id = new SelectList(db.contract_type, "contractTypeid", "contract_type_name", staff.contract_type_id);
            ViewBag.country_office_id = new SelectList(db.Country_office, "countryofficeid", "office_description_english", staff.country_office_id);
            ViewBag.functional_title_id = new SelectList(db.functional_title.OrderBy(functional_title => functional_title.functional_title_description_english), "functionaltitleid", "functional_title_description_english", staff.functional_title_id);
            ViewBag.staff_supervisorid = new SelectList(db.staffs.OrderBy(staffs => staffs.staff_email), "staffid", "staff_email", staff.staff_supervisorid);
            ViewBag.staff_warden = new SelectList(db.staffs, "staffid", "staff_email", staff.staff_warden);
            ViewBag.alternate_warden_1 = new SelectList(db.staffs, "staffid", "staff_email", staff.alternate_warden_1);
            ViewBag.alternate_warden_2 = new SelectList(db.staffs, "staffid", "staff_email", staff.alternate_warden_2);
            ViewBag.sub_office_id = new SelectList(db.sub_office, "subofficeid", "office_description_english", staff.sub_office_id);
            ViewBag.unit_id = new SelectList(db.units, "unitid", "unit_description_english", staff.unit_id);
            ViewBag.countryid = new SelectList(db.countries, "countryid", "country_name", staff.countryid);
            ViewBag.governorates_code = new SelectList(db.governorates, "governorates_code", "governorates_name", staff.governorates_code);
            ViewBag.areacode = new SelectList(db.districts, "areacode", "area_name", staff.areacode);
            ViewBag.CatID = new SelectList(db.categories, "CatID", "CatName", staff.CatID);
            ViewBag.WardenZone = new SelectList(db.districts, "areacode", "area_name", staff.WardenZone);
            return View(staff);
        }

        // GET: /staff/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            staff staff = db.staffs.Find(id);
            if (staff == null)
            {
                return HttpNotFound();
            }
            ViewBag.stuff_contract_details = new SelectList(db.contract_details, "contractdetailsid", "contract_details_id", staff.stuff_contract_details);
            ViewBag.contract_type_id = new SelectList(db.contract_type, "contractTypeid", "contract_type_name", staff.contract_type_id);
            ViewBag.country_office_id = new SelectList(db.Country_office, "countryofficeid", "office_description_english", staff.country_office_id);
            ViewBag.functional_title_id = new SelectList(db.functional_title, "functionaltitleid", "functional_title_description_english", staff.functional_title_id);
            ViewBag.staff_supervisorid = new SelectList(db.staffs.OrderBy(staffs => staffs.staff_email), "staffid", "staff_email", staff.staff_supervisorid);
            ViewBag.staff_warden = new SelectList(db.staffs, "staffid", "staff_email", staff.staff_warden);
            ViewBag.alternate_warden_1 = new SelectList(db.staffs, "staffid", "staff_email", staff.alternate_warden_1);
            ViewBag.alternate_warden_2 = new SelectList(db.staffs, "staffid", "staff_email", staff.alternate_warden_2);
            ViewBag.sub_office_id = new SelectList(db.sub_office, "subofficeid", "office_description_english", staff.sub_office_id);
            ViewBag.unit_id = new SelectList(db.units, "unitid", "unit_description_english", staff.unit_id);
            ViewBag.countryid = new SelectList(db.countries, "countryid", "country_name", staff.countryid);
            ViewBag.governorates_code = new SelectList(db.governorates, "governorates_code", "governorates_name", staff.governorates_code);
            ViewBag.areacode = new SelectList(db.districts, "areacode", "area_name", staff.areacode);
            ViewBag.CatID = new SelectList(db.categories, "CatID", "CatName", staff.CatID);
            ViewBag.WardenZone = new SelectList(db.districts, "areacode", "area_name", staff.WardenZone);
            
            return View(staff);
        }

        // POST: /staff/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "staffid,staff_id,country_office_id,sub_office_id,unit_id,contract_type_id,functional_title_id,staff_login,staff_first_name,staff_last_name,staff_created_by,staff_created_datetime,staff_status,staff_index_number,staff_vendor_number,staff_nationality,staff_address,staff_dob,staff_eod,staff_nte,staff_extension_number,staff_email,staff_access_main,staff_access_dts,staff_access_tb,staff_access_ct,staff_access_li,staff_access_lc,staff_access_sr,staff_access_admin,staff_last_updated_by,staff_last_updated_datetime,staff_deleted_by,staff_deleted_datetime,staff_supervisorid,stuff_contract_details,,staff_warden,alternate_warden_1,alternate_warden_2,Call_Sign,Landline,Mobile,countryid,governorates_code,areacode,cate,CatID,WardenZone")] staff staff)
        {
            if (ModelState.IsValid)
            {
                db.Entry(staff).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.stuff_contract_details = new SelectList(db.contract_details, "contractdetailsid", "contract_details_id", staff.stuff_contract_details);
            ViewBag.contract_type_id = new SelectList(db.contract_type, "contractTypeid", "contract_Type_code", staff.contract_type_id);
            ViewBag.country_office_id = new SelectList(db.Country_office, "countryofficeid", "office_description_english", staff.country_office_id);
            ViewBag.functional_title_id = new SelectList(db.functional_title, "functionaltitleid", "functional_title_description_english", staff.functional_title_id );
            ViewBag.staff_supervisorid = new SelectList(db.staffs, "staffid", "staff_id", staff.staff_supervisorid);
            ViewBag.staff_warden = new SelectList(db.staffs, "staffid", "staff_id", staff.staff_warden);
            ViewBag.alternate_warden_1 = new SelectList(db.staffs, "staffid", "staff_id", staff.alternate_warden_1);
            ViewBag.alternate_warden_2 = new SelectList(db.staffs, "staffid", "staff_id", staff.alternate_warden_2);
            ViewBag.sub_office_id = new SelectList(db.sub_office, "subofficeid", "office_description_english", staff.sub_office_id);
            ViewBag.unit_id = new SelectList(db.units, "unitid", "unit_id", staff.unit_id);
            ViewBag.countryid = new SelectList(db.countries, "countryid", "countryid", staff.countryid );
            ViewBag.governorates_code = new SelectList(db.governorates, "governorates_code", "governorates_code", staff.governorates_code);
            ViewBag.areacode = new SelectList(db.districts, "areacode", "areacode", staff.areacode);
            ViewBag.CatID = new SelectList(db.categories, "CatID", "CatID", staff.CatID);
            ViewBag.WardenZone = new SelectList(db.districts, "areacode", "areacode", staff.WardenZone);
            
            return View(staff);
        }

        // GET: /staff/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            staff staff = db.staffs.Find(id);
            if (staff == null)
            {
                return HttpNotFound();
            }
            return View(staff);
        }

        // POST: /staff/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
            staff staff = db.staffs.Find(id);
            db.staffs.Remove(staff);
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
