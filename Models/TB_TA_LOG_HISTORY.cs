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
    
    public partial class TB_TA_LOG_HISTORY
    {
        public int nHistroryIdn { get; set; }
        public int nUpdateTime { get; set; }
        public string sUserID { get; set; }
        public string sIP { get; set; }
        public int nDateTime { get; set; }
        public int nUserIdn { get; set; }
        public int nReaderIdn { get; set; }
        public short nTNAEvent { get; set; }
        public int nFirstInTime { get; set; }
        public int nLastOutTime { get; set; }
        public string sFirstInEvent { get; set; }
        public string sLastOutEvent { get; set; }
        public int nTAResult { get; set; }
        public int nWorkTime { get; set; }
        public int nTimeCategoryIdn { get; set; }
        public short nFlag { get; set; }
    }
}
