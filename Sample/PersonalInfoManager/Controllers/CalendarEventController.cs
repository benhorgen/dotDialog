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
			string crudOperation = "Read";
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


			string vp = ViewPerspective.Default;
			switch (crudOperation)
			{
			case "Create":
				Model = new CalEvent();
				vp = ViewPerspective.Create;
				break;
			case "Update":
				vp = ViewPerspective.Update;
				if (Model == null) { Console.WriteLine("WARNING: Controller can't find calendar event to update"); }
				break;
			default:
				vp = ViewPerspective.Default;
				break;
			}
			if (Model == null) { vp = CalendarEventController.NoData; }
			
			return vp;
		}

		public static string Uri(string id) { return baseUri + id; }
		public static string Uri(string id, string crud) 
		{ 
			return string.Format("{0}{1}/{2}", baseUri, crud, id);
		}
		public static string UriForNew() { return Uri("0", "Create"); }
		
		
		public static void RegisterUris(MXApplication theApp)
		{
			var ctrl = new CalendarEventController();
			
			string uri = CalendarEventController.Uri("{" + CalendarEventController.idKey + "}");
			theApp.NavigationMap.Add(uri, ctrl);
			
			string updateUri = CalendarEventController.Uri("{" + CalendarEventController.idKey + "}", "{" + CalendarEventController.crudKey + "}");
			theApp.NavigationMap.Add(updateUri, ctrl);
			
		}

		private static string baseUri = "CalendarEvent/";
		public static string idKey = "id";
		public static string crudKey = "crud";
		public static string EventEdit = "Update";
		public static string NoData = "No Data";
	}
}

