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
    
    public partial class CourseSession
    {
        public CourseSession()
        {
            this.CourseRegs = new HashSet<CourseReg>();
        }
    
        public int Sessionid { get; set; }
        public Nullable<int> CourseID { get; set; }
        public string CourseDuration { get; set; }
        public string CourseStartTime { get; set; }
        public string CourseEndTime { get; set; }
        public Nullable<System.DateTime> CourseStartDate { get; set; }
        public Nullable<System.DateTime> CourseEndDate { get; set; }
        public string CourseLocation { get; set; }
        public string SessionName { get; set; }
        public string CoursePrerequisite { get; set; }
    
        public virtual ICollection<CourseReg> CourseRegs { get; set; }
        public virtual Cours Cours { get; set; }
    }
}
