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
    
    public partial class TB_EVENT_LOG_2
    {
        public int nEventLogIdn { get; set; }
        public int nDateTime { get; set; }
        public int nReaderIdn { get; set; }
        public int nEventIdn { get; set; }
        public int nUserID { get; set; }
        public short nIsLog { get; set; }
        public short nTNAEvent { get; set; }
        public short nIsUseTA { get; set; }
        public short nType { get; set; }
        public Nullable<System.DateTime> date { get; set; }
    }
}
