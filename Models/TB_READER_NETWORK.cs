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
    
    public partial class TB_READER_NETWORK
    {
        public int nReaderIdn { get; set; }
        public short nType { get; set; }
        public int nPort { get; set; }
        public int nMaxConnection { get; set; }
        public short nSSL { get; set; }
        public short nDHCP { get; set; }
        public string sIPAddress { get; set; }
        public string sGateway { get; set; }
        public string sSubnetMask { get; set; }
        public string sServerIP { get; set; }
        public short nTimeSync { get; set; }
        public string sComPort { get; set; }
        public int nRS485Mode { get; set; }
        public int nRS485Baudrate { get; set; }
        public int nRS232Baudrate { get; set; }
        public int nSecureIO { get; set; }
        public short nIsHistory { get; set; }
        public int nUpdateDate { get; set; }
    }
}
