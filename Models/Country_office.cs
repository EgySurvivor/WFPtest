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
    
    public partial class Country_office
    {
        public Country_office()
        {
            this.staffs = new HashSet<staff>();
            this.sub_office = new HashSet<sub_office>();
            this.units = new HashSet<unit>();
        }
    
        public int countryofficeid { get; set; }
        public string office_id { get; set; }
        public string office_description_english { get; set; }
        public string office_abreviation_english { get; set; }
        public string office_description_french { get; set; }
        public string office_abreviation_french { get; set; }
        public string office_status { get; set; }
        public string office_created_by { get; set; }
        public Nullable<System.DateTime> office_created_datetime { get; set; }
        public string office_last_modified_by { get; set; }
        public Nullable<System.DateTime> office_last_modified_datetime { get; set; }
        public string office_deleted_by { get; set; }
        public Nullable<System.DateTime> office_deleted_datetime { get; set; }
        public Nullable<int> country_code { get; set; }
        public Nullable<int> Manager { get; set; }
        public Nullable<int> RB_ID { get; set; }
    
        public virtual country country { get; set; }
        public virtual ICollection<staff> staffs { get; set; }
        public virtual ICollection<sub_office> sub_office { get; set; }
        public virtual ICollection<unit> units { get; set; }
    }
}
