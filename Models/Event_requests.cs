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
    
    public partial class Event_requests
    {
        public int ID { get; set; }
        public Nullable<System.DateTime> RequestingDate { get; set; }
        public string RequestingUnit { get; set; }
        public Nullable<int> FocalPoint { get; set; }
        public string Purpose { get; set; }
        public Nullable<System.DateTime> FromDate { get; set; }
        public Nullable<System.DateTime> ToDate { get; set; }
        public string NumOFP_CO { get; set; }
        public string NumOFP_RB { get; set; }
        public string NumOFP_HQ { get; set; }
        public string NumOFP_Other { get; set; }
        public string Country { get; set; }
        public string GOV { get; set; }
        public string District { get; set; }
        public string OtherPlace { get; set; }
        public string Budget_TR { get; set; }
        public string Number_Roms { get; set; }
        public Nullable<System.DateTime> Check_inDate { get; set; }
        public Nullable<System.DateTime> Check_OutDate { get; set; }
        public string Rom_Type { get; set; }
        public Nullable<bool> Microphone { get; set; }
        public Nullable<bool> HiSpeedInternet { get; set; }
        public Nullable<bool> Conf_Call_Device { get; set; }
        public string MMR_Days { get; set; }
        public string BOR_Days { get; set; }
        public Nullable<bool> COffeBreak { get; set; }
        public string COffeBreak_UNM { get; set; }
        public Nullable<bool> Lunch { get; set; }
        public Nullable<bool> Dinner { get; set; }
        public Nullable<bool> Coctail_Reception { get; set; }
        public Nullable<bool> Dedicated_IT_SUPP { get; set; }
        public Nullable<bool> Transportstion_ForLocal_Staff { get; set; }
        public Nullable<bool> Airport_Picup_Services { get; set; }
        public string Other_Services { get; set; }
    }
}
