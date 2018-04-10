using System;
using CodeEffects.Rule.Common;
using CodeEffects.Rule.Attributes;
using WFPtest.Models;

namespace CodeEffects.Rule.Demo.Bre.Mvc.Services
{
	/// <summary>
	/// The purpose of this class is to demonstrate the use of external static and instance in-rule methods and actions.
	/// The Patient class uses the ExternalMethodAttribute and ExternalActionAttribute to "reference" methods of this PatientService class.
	/// </summary>
	public class PatientService
	{
		// Static in-rule method
		[Method("Is Today", "Indicates if the param date is today")]
		public static bool IsToday([Parameter(ValueInputType.All, Description = "The date to test")] DateTime? date)
		{
			if(date == null) return false;
			DateTime now = DateTime.Now;
			return date.Value.Day == now.Day && date.Value.Month == now.Month && date.Value.Year == now.Year;
		}

		// Instance action (void) method
		[Action("Request More Info", "Requires additional info from the patient")]
        public void RequestInfo(request patient, [Parameter(ValueInputType.User, Description = "Output message")] string message)
		{
			patient.Output = message;
		}
	}
}