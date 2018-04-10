using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Text.RegularExpressions;

namespace WFPtest.Models
{
    public class ModelServices
    {
        private readonly WFPEntities1 entities = new WFPEntities1();

        public IEnumerable<MissionItinerary> GetMissionItinerary()
        {
            return entities.MissionItineraries.ToList();
        }

        public IEnumerable<MissionItinerary> GetEmployeePage(int pageNumber, int pageSize, string searchCriteria)
        {
            if (pageNumber < 1)
                pageNumber = 1;

            return entities.MissionItineraries 
                .OrderBy(m => m.ID)
              .Skip((pageNumber - 1) * pageSize)
              .Take(pageSize)
              .ToList();
        }
        public int CountAllEmployee()
        {
            return entities.MissionItineraries.Count();
        }

        public void Dispose()
        {
            entities.Dispose();
        }

    //    //For Edit 
    //    public MissionItinerary GetEmployeeDetail(int mCustID)
    //    {
    //        return entities.Employee.Where(m => m.ID == mCustID).FirstOrDefault();
    //    }

    //    public bool AddEmployee(MissionItinerary emp)
    //    {
    //        try
    //        {
    //            entities.MissionItinerary.Add(emp);
    //            entities.SaveChanges();
    //            return true;
    //        }
    //        catch (Exception ex)
    //        {
    //            return false;
    //        }
    //    }

    //    public bool UpdateEmployee(MissionItinerary emp)
    //    {
    //        try
    //        {
    //            MissionItinerary data = entities.MissionItinerary.Where(m => m.ID == emp.ID).FirstOrDefault();

    //            data.Name = emp.Name;
    //            data.Dept = emp.Dept;
    //            data.City = emp.City;
    //            data.State = emp.State;
    //            data.Country = emp.Country;
    //            data.Mobile = emp.Mobile;
    //            entities.SaveChanges();
    //            return true;
    //        }
    //        catch (Exception mex)
    //        {
    //            return false;
    //        }
    //    }

    //    public bool DeleteEmployee(int mCustID)
    //    {
    //        try
    //        {
    //            Employee data = entities.Employee.Where(m => m.ID == mCustID).FirstOrDefault();
    //            entities.Employee.Remove(data);
    //            entities.SaveChanges();
    //            return true;
    //        }
    //        catch (Exception mex)
    //        {
    //            return false;
    //        }
    //    }
    }

    public class PagedEmployeeModel
    {
        public int TotalRows { get; set; }
        public IEnumerable<MissionItinerary> MissionItinerary { get; set; }
        public int PageSize { get; set; }
    }

    public class MissionItineraryModel
    {

        [ScaffoldColumn(false)]
        public int ID { get; set; }

        [Required(ErrorMessage = "Please Enter Employee Code")]
        public string MissionID { get; set; }

        [Required(ErrorMessage = "Please Enter Name")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Please Enter Department")]
        public string FromID { get; set; }

        [Required(ErrorMessage = "Please Enter City")]
        public string FromDate { get; set; }

        [Required(ErrorMessage = "Please Enter State")]
        public string ToID { get; set; }

        [Required(ErrorMessage = "Please Enter Country")]
        public string ToDateToDate { get; set; }

        
    }
}