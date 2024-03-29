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
    
    public partial class RequestingForm
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public Nullable<int> VendorId { get; set; }
        public string Description { get; set; }
        public string Amount { get; set; }
        public Nullable<int> RequestingUnitID { get; set; }
        public Nullable<int> AssignedTo { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public Nullable<System.DateTime> Duedate { get; set; }
        public string non1 { get; set; }
        public Nullable<int> non2 { get; set; }
        public string non3 { get; set; }
        public string type { get; set; }
        public string CodeNon { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedON { get; set; }
        public string Status { get; set; }
        public Nullable<int> DeletedBy { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public string HOUApproval { get; set; }
        public string Project { get; set; }
        public string DeliveryPoint { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<decimal> UnitPrice { get; set; }
        public Nullable<decimal> ExtendedPrice { get; set; }
        public string Currency { get; set; }
        public string PONumber { get; set; }
        public string MPOnumber { get; set; }
        public string ReferenceNumber { get; set; }
        public string GeneralReferenceNumber { get; set; }
        public Nullable<int> CertifiedByProjectOfficer { get; set; }
        public string CertifiedByProjectOfficerApproval { get; set; }
        public Nullable<System.DateTime> CertifiedByProjectOfficerApprovalOn { get; set; }
        public Nullable<int> CertifiedByFinance { get; set; }
        public string CertifiedByFinanceApproval { get; set; }
        public Nullable<System.DateTime> CertifiedByFinanceApprovalOn { get; set; }
        public Nullable<int> CertifiedByCD { get; set; }
        public string CertifiedByCDApproval { get; set; }
        public Nullable<System.DateTime> CertifiedByCDApprovalOn { get; set; }
        public Nullable<int> ReceivedBy { get; set; }
        public string ReceivedByApproval { get; set; }
        public Nullable<System.DateTime> ReceivedByApprovalOn { get; set; }
    }
}
