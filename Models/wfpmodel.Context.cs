﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class WFPEntities1 : DbContext
    {
        public WFPEntities1()
            : base("name=WFPEntities1")
        {
        }
    
        
    
        public virtual DbSet<contract_details> contract_details { get; set; }
        public virtual DbSet<contract_type> contract_type { get; set; }
        public virtual DbSet<country> countries { get; set; }
        public virtual DbSet<Country_office> Country_office { get; set; }
        public virtual DbSet<dependent> dependents { get; set; }
        public virtual DbSet<Evaluation> Evaluations { get; set; }
        public virtual DbSet<functional_title> functional_title { get; set; }
        public virtual DbSet<Fund> Funds { get; set; }
        public virtual DbSet<staff> staffs { get; set; }
        public virtual DbSet<sub_office> sub_office { get; set; }
        public virtual DbSet<unit> units { get; set; }
        public virtual DbSet<governorate> governorates { get; set; }
        public virtual DbSet<district> districts { get; set; }
        public virtual DbSet<category> categories { get; set; }
        public virtual DbSet<WardenGroup> WardenGroups { get; set; }
        public virtual DbSet<CourseReg> CourseRegs { get; set; }
        public virtual DbSet<Cours> Courses { get; set; }
        public virtual DbSet<CourseSession> CourseSessions { get; set; }
        public virtual DbSet<MissionAuthorization> MissionAuthorizations { get; set; }
        public virtual DbSet<request> requests { get; set; }
        public virtual DbSet<SiteMenu> SiteMenus { get; set; }
        public virtual DbSet<CourseDetail> CourseDetails { get; set; }
        public virtual DbSet<CourseTypt> CourseTypts { get; set; }
        public virtual DbSet<VACATION_TYPE_LIST> VACATION_TYPE_LIST { get; set; }
        public virtual DbSet<DEPARTMENT> DEPARTMENTS { get; set; }
        public virtual DbSet<error_login> error_login { get; set; }
        public virtual DbSet<securitytracking> securitytrackings { get; set; }
        public virtual DbSet<sir_access> sir_access { get; set; }
        public virtual DbSet<sir_category> sir_category { get; set; }
        public virtual DbSet<sir_category_access> sir_category_access { get; set; }
        public virtual DbSet<sir_item> sir_item { get; set; }
        public virtual DbSet<sir_item_category_access> sir_item_category_access { get; set; }
        public virtual DbSet<sir_staff_item> sir_staff_item { get; set; }
        public virtual DbSet<staff_login_history> staff_login_history { get; set; }
        public virtual DbSet<staff_pwd> staff_pwd { get; set; }
        public virtual DbSet<stafftest> stafftests { get; set; }
        public virtual DbSet<stafftesttest> stafftesttests { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<VACATION_GROUP_DETAILS> VACATION_GROUP_DETAILS { get; set; }
        public virtual DbSet<VACATION_GROUPS> VACATION_GROUPS { get; set; }
        public virtual DbSet<VACATION_PERIODS> VACATION_PERIODS { get; set; }
        public virtual DbSet<AggregatedCounter> AggregatedCounters { get; set; }
        public virtual DbSet<Counter> Counters { get; set; }
        public virtual DbSet<Hash> Hashes { get; set; }
        public virtual DbSet<Job> Jobs { get; set; }
        public virtual DbSet<JobParameter> JobParameters { get; set; }
        public virtual DbSet<JobQueue> JobQueues { get; set; }
        public virtual DbSet<List> Lists { get; set; }
        public virtual DbSet<Schema> Schemata { get; set; }
        public virtual DbSet<Server> Servers { get; set; }
        public virtual DbSet<Set> Sets { get; set; }
        public virtual DbSet<State> States { get; set; }
        public virtual DbSet<MissionAuthorizationH> MissionAuthorizationHs { get; set; }
        public virtual DbSet<EMPLOYEE> EMPLOYEES { get; set; }
        public virtual DbSet<VAC_TRANS> VAC_TRANS { get; set; }
        public virtual DbSet<BRANCH> BRANCHES { get; set; }
        public virtual DbSet<Job_DESCRIPTION> Job_DESCRIPTION { get; set; }
        public virtual DbSet<TRAINEE> TRAINEES { get; set; }
        public virtual DbSet<TRAINING_CERTIFICATION> TRAINING_CERTIFICATION { get; set; }
        public virtual DbSet<TRAINING_COMPANIES> TRAINING_COMPANIES { get; set; }
        public virtual DbSet<TRAINING_PLAN> TRAINING_PLAN { get; set; }
        public virtual DbSet<TRAINING_PLAN_TRAINERS> TRAINING_PLAN_TRAINERS { get; set; }
        public virtual DbSet<TRAINING_PLAN_TRAINING_CLASSES> TRAINING_PLAN_TRAINING_CLASSES { get; set; }
        public virtual DbSet<TRAINING_PLAN_TRAINING_COMPANIES> TRAINING_PLAN_TRAINING_COMPANIES { get; set; }
        public virtual DbSet<TRAINING_PLAN_TRAININGS> TRAINING_PLAN_TRAININGS { get; set; }
        public virtual DbSet<TRAINING_SC> TRAINING_SC { get; set; }
        public virtual DbSet<TRAINING_TRAINING_COMPANIES> TRAINING_TRAINING_COMPANIES { get; set; }
        public virtual DbSet<TRAINING> TRAININGS { get; set; }
        public virtual DbSet<TRAINING_TYPE> TRAINING_TYPE { get; set; }
        public virtual DbSet<MissionAuthorization1> MissionAuthorization1 { get; set; }
        public virtual DbSet<Attend> Attends { get; set; }
        public virtual DbSet<Staff_Event> Staff_Event { get; set; }
        public virtual DbSet<requests1> requests1 { get; set; }
        public virtual DbSet<evalution> evalutions { get; set; }
        public virtual DbSet<Event_requests> Event_requests { get; set; }
        public virtual DbSet<MissionItinerary> MissionItineraries { get; set; }
        public virtual DbSet<MissionType> MissionTypes { get; set; }
    }
}
