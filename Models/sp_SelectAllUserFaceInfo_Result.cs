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
    
    public partial class sp_SelectAllUserFaceInfo_Result
    {
        public int nUserIdn { get; set; }
        public short nIndex { get; set; }
        public byte[] bTemplate { get; set; }
        public int nTemplatecs { get; set; }
        public int nImageLen { get; set; }
        public byte[] bImage { get; set; }
        public short nEncryption { get; set; }
        public int nSecurityLevel { get; set; }
        public int nAuthLimitCount { get; set; }
    }
}
