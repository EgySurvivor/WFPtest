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
    
    public partial class TRAINING_PLAN_TRAINING_COMPANIES
    {
        public string PLAN_ID { get; set; }
        public string TRAINING_COMPANY_ID { get; set; }
        public string TRAINING_ID { get; set; }
    
        public virtual TRAINING_COMPANIES TRAINING_COMPANIES { get; set; }
    }
}
