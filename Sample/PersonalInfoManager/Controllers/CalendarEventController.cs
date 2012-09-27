using System;
using System.Linq;
using System.Collections.Generic;

using MonoCross.Navigation;

using System.Xml.Linq;
using System.Runtime.Serialization;


namespace dotDialog.Sample.PersonalInfoManger
{
	public class CalendarEventController : MXController<CalEvent>, IMXController
	{
		public override string Load(Dictionary<string, string> parameters)
		{
			string crudOperation = ViewPerspective.Default;
			parameters.TryGetValue(ContactController.crudKey, out crudOperation);
			
			string id = string.Empty;
			if (!parameters.TryGetValue(ContactController.idKey, out id))
			{
				string err = string.Format("Failed getting {0} key from parameters",
				                           CalendarEventController.idKey);
				throw new Exception(err);
			}
			
			var events = CalendarListController.LoadModel(true);
			if (events != null) 
			{
				//TODO: Check Stale date first
				Model = (from e in events.Events where e.Id == id select e).FirstOrDefault();
			}
			else 
			{ 
				string msg = "Failed to load calendars when looking for specific event";
				Console.WriteLine(msg);
			}


			switch (crudOperation)
			{
			case ViewPerspective.Default:
				break; //nothing to do
			case ViewPerspective.Create:
				Model = new CalEvent();
				break;
			case ViewPerspective.Update:
				if (Model == null) { Console.WriteLine("WARNING: Controller can't find calendar event to update"); }
				break;
			case ViewPerspective.Delete:
				//TODO:  Implement Delete CRUD operation
				Console.WriteLine("DELETE is not implemented yet, sorry");
				MXContainer.Instance.Redirect(CalendarListController.Uri);
				break;
			default:
				Console.WriteLine("Unexpected crud operation string value of: " + crudOperation);
				crudOperation = ViewPerspective.Default; //set to default if unknown
				break;
			}
			if (Model == null && crudOperation != ViewPerspective.Create)  
			{
				Console.WriteLine(string.Format("Null Model, overriding crud from \"{0}\" to \"{1}\"", 
								                                crudOperation, CalendarEventController.NoData));
				crudOperation = CalendarEventController.NoData;
			}
			
			return crudOperation;
		}

		public static string Uri(string id) { return Uri(id, ViewPerspective.Default); }
		public static string Uri(string id, string crud) 
		{ 
			return string.Format("{0}/{1}/{2}", baseUri, crud, id);
		}
		public static string UriForNew() { return Uri("0", ViewPerspective.Create); }
		
		public static void RegisterUris(MXApplication theApp)
		{
			var ctrl = new CalendarEventController();
			
			string uri = CalendarEventController.Uri("{" + CalendarEventController.idKey + "}");
			theApp.NavigationMap.Add(uri, ctrl);
			
			string updateUri = CalendarEventController.Uri("{" + CalendarEventController.idKey + "}", "{" + CalendarEventController.crudKey + "}");
			theApp.NavigationMap.Add(updateUri, ctrl);
			
		}

		private static string baseUri = "CalendarEvent";
		public static string idKey = "id";
		public static string crudKey = "crud";
		public static string NoData = "No Data";
	}
}

