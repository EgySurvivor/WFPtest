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
    
    public partial class TRAINING
    {
        public TRAINING()
        {
            this.TRAINING_CERTIFICATION = new HashSet<TRAINING_CERTIFICATION>();
            this.TRAININGS1 = new HashSet<TRAINING>();
            this.TRAINEES = new HashSet<TRAINEE>();
        }
    
        public string TRAINING_ID { get; set; }
        public string TRAINING_NAME { get; set; }
        public string TRAINING_DESC { get; set; }
        public string TRAINING_PLACE { get; set; }
        public Nullable<double> TRAINING_PERIOD { get; set; }
        public string PARENT_TRAINING_ID { get; set; }
    
        public virtual ICollection<TRAINING_CERTIFICATION> TRAINING_CERTIFICATION { get; set; }
        public virtual ICollection<TRAINING> TRAININGS1 { get; set; }
        public virtual TRAINING TRAINING1 { get; set; }
        public virtual ICollection<TRAINEE> TRAINEES { get; set; }
    }
}
