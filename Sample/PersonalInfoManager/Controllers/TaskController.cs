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

			string vp = ViewPerspective.Default;
			switch (crudOperation)
			{
			case "Create":
				Model = new Task();
				vp = ViewPerspective.Create;
				break;
			case "Update":
				vp = ViewPerspective.Update;
				if (Model == null) { Console.WriteLine("WARNING: Controller can't find task for update"); }
				break;
			}
			if (Model == null) { vp = TaskController.NoData; }
			
			return vp;
		}

		public static string Uri(string id) { return baseUri + id; }
		public static string Uri(string id, string crud) 
		{ 
			return string.Format("{0}{1}/{2}", baseUri, id, crud);
		}
		public static string UriForNew() { return Uri("0", "Create"); }
		
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

