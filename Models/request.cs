//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WFPtest.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class request
    {
        public int request_no { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public Nullable<int> job_title { get; set; }
        public Nullable<int> unit { get; set; }
        public string budget_code { get; set; }
        public string index_number { get; set; }
        public string duty_station { get; set; }
        public Nullable<int> supervisor_email { get; set; }
        public string appointment_type { get; set; }
        public Nullable<System.DateTime> start_date { get; set; }
        public Nullable<System.DateTime> end_date { get; set; }
        public Nullable<bool> computerLaptop { get; set; }
        public Nullable<bool> computerDeskyop { get; set; }
        public Nullable<bool> email { get; set; }
        public Nullable<bool> access_P { get; set; }
        public Nullable<bool> telephone { get; set; }
        public Nullable<bool> pincode_ext { get; set; }
        public Nullable<bool> local_sim { get; set; }
        public Nullable<bool> international { get; set; }
        public Nullable<bool> roaming { get; set; }
        public Nullable<bool> SmartPhone { get; set; }
        public Nullable<bool> BasicPhone { get; set; }
        public Nullable<bool> usb_modem { get; set; }
        public Nullable<bool> color_printer { get; set; }
        public string BlackberryService { get; set; }
        public string IphoneService { get; set; }
        public string mobile_phone { get; set; }
        public string other { get; set; }
        public string location { get; set; }
        public Nullable<int> requested_by { get; set; }
    
        public virtual functional_title functional_title { get; set; }
        public virtual staff staff { get; set; }
        public virtual staff staff1 { get; set; }
        public virtual unit unit1 { get; set; }
    }
}
