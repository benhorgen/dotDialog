using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;

using MonoCross.Navigation;


namespace dotDialog.Sample.PersonalInfoManger
{
	public class TaskController : MXController<Task>, IMXController
	{
		public override string Load (System.Collections.Generic.Dictionary<string, string> parameters)
		{
			string crudOperation = "Read";
			parameters.TryGetValue(TaskController.crudKey, out crudOperation);
			
			string id = string.Empty;
			if (!parameters.TryGetValue(TaskController.idKey, out id))
			{
				string msg = "NO " + TaskController.idKey + " for " + GetType().ToString() + ".Load()";
				Console.WriteLine(msg);
			}
			

			List<Task> taskList = TaskListController.LoadModel(true);
			if (taskList != null) 
			{
				Model = (from individual in taskList 
				         where individual.Id == id 
				         select individual).FirstOrDefault();
			}
			else { Console.WriteLine("Failed to deserlize Tasks when looking up an individual one"); }
			
			switch (crudOperation)
			{
			case ViewPerspective.Default:
				break; //nothing to do
			case ViewPerspective.Create:
				Model = new Task();
				break;
			case ViewPerspective.Update:
				if (Model == null) { Console.WriteLine("WARNING: Controller can't find contact for update"); }
				break;
			case ViewPerspective.Delete:
				//TODO:  Implement Delete CRUD operation
				Console.WriteLine("DELETE is not implemented for contact yet");
				MXContainer.Instance.Redirect(TaskListController.Uri);
				break;
			default:
				Console.WriteLine("Unexpected crud operation string value of: " + crudOperation);
				crudOperation = ViewPerspective.Default; //set to default if unknown
				break;
			}
			if (Model == null && crudOperation != ViewPerspective.Create)  
			{
				Console.WriteLine(string.Format("Null Model, overriding crud from \"{0}\" to \"{1}\"", 
				                                crudOperation, TaskController.NoData));
				crudOperation = TaskController.NoData;
			}
			
			return crudOperation;
		}

		public static string Uri(string id) { return Uri(id, ViewPerspective.Default); }
		public static string Uri(string id, string crud) 
		{ 
			return string.Format("{0}{1}/{2}", baseUri, id, crud);
		}
		public static string UriForNew() { return Uri("0", ViewPerspective.Create); }
		
		public static void RegisterUris(MXApplication theApp)
		{
			var ctrl = new TaskController();
			
			string uri = TaskController.Uri("{" + TaskController.idKey + "}");
			theApp.NavigationMap.Add(uri, ctrl);

			string updateUri = TaskController.Uri("{" + TaskController.idKey + "}", "{" + TaskController.crudKey + "}");
			theApp.NavigationMap.Add(updateUri, ctrl);
			
		}
		
		private static string baseUri = "Task/";
		private static string taskCreateUri = "TaskAdd";
		public static string idKey = "id";
		public static string crudKey = "crud";
		public const string NoData = "No Data";

		public TaskController () { }
	}
}

