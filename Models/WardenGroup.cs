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
    
    public partial class WardenGroup
    {
        public WardenGroup()
        {
            this.staffs = new HashSet<staff>();
        }
    
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public Nullable<int> GroupZone { get; set; }
        public string GroupDescription { get; set; }
    
        public virtual district district { get; set; }
        public virtual ICollection<staff> staffs { get; set; }
    }
}
