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
    
    public partial class CourseReg
    {
        public int RegID { get; set; }
        public Nullable<int> staffid { get; set; }
        public Nullable<int> courseid { get; set; }
        public Nullable<int> sessionid { get; set; }
        public Nullable<bool> LMSPre { get; set; }
        public Nullable<bool> SupervisorApprove { get; set; }
    
        public virtual Cours Cours { get; set; }
        public virtual CourseSession CourseSession { get; set; }
        public virtual staff staff { get; set; }
    }
}
