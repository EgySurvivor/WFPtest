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
    
    public partial class dependent
    {
        public int dependentsid { get; set; }
        public string dependents_code { get; set; }
        public Nullable<int> staffid { get; set; }
        public string dependents_first_name { get; set; }
        public string dependents_last_name { get; set; }
        public string dependents_gender { get; set; }
        public string dependents_phone_num { get; set; }
        public string dependents_blood_group { get; set; }
        public string dependents_passport_num { get; set; }
        public Nullable<System.DateTime> dependents_passport_expiry_date { get; set; }
        public string dependents_notes { get; set; }
        public string dependents_medical_condition { get; set; }
    
        public virtual staff staff { get; set; }
    }
}
