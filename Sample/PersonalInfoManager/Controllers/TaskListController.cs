using System;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using System.Runtime.Serialization;
using System.Xml;

using MonoCross.Navigation;

namespace dotDialog.Sample.PersonalInfoManger
{
	public class TaskListController : MXController<List<Task>>, IMXController
	{
		public TaskListController() { }

		public override string Load (System.Collections.Generic.Dictionary<string, string> parameters)
		{
			Model = LoadModel(true);
			return ViewPerspective.Default;
		}

		//TODO: Harvest this as a pattern for Model from resting point
		public static List<Task> LoadModel(bool createNewModelIfEmpty)
		{
			int allowedFailures = 2;
			List<Task> tasks = null;
			while (tasks == null && allowedFailures > 0) 
			{
				var dataBytes = App.LoadBytesFromDataSource(DataUri, false);
				if (dataBytes != null)
				{
					tasks = Task.BytesToTaskList(dataBytes);
					if (tasks == null) { Console.WriteLine("Failed to deserlize Tasks"); }
				}
				if (tasks == null) { allowedFailures--; }
			}
			// check if retrieval was successful
			if (tasks == null && createNewModelIfEmpty)
			{
				tasks = new List<Task>();
				Console.WriteLine("An empty list of tasks was created but not serialized");
			}

			return tasks;
		}

		public static bool SaveTaskToDataSource(Task t, bool addIfNew, bool createNewModelIfEmpty)
		{
			//TODO: Replace with a call to the server, currently just caches back to disk record
			List<Task> taskList = LoadModel(createNewModelIfEmpty);

			bool saved = Task.UpdateTask(taskList, t);
			if (!saved && addIfNew) { saved = Task.AddTask(taskList, t); }
			if (saved) { saved = TaskListController.SaveModelToDisk(taskList); }

			return saved;
		}
		static bool SaveModelToDisk(List<Task> taskList)
		{
			bool saved = true;
			
			// Serialize the object to xml
			DataContractSerializer serializer = new DataContractSerializer(taskList.GetType());
			
			// Cache serialized bytes to disk
			string uri = TaskListController.DataUri;
			string xml = "[null]";
			try { xml = App.CacheToDisk(serializer, taskList, uri); }
			catch (Exception e) 
			{
				string msg = string.Format("A {0} occurred writing to disk:\r\nUri: {1}\r\nXML:\r\n{2}",
				                           e.Message, uri, xml);
				Console.WriteLine(msg);
				saved = false;
			}
			return saved;
		}

		public static string Uri { get {return "TaskList"; } }
		public static string Temp_VP = "sp_temp_vp";

		public static string DataUri = "http://localhost0.1/tasklist.xml";
	}
}

