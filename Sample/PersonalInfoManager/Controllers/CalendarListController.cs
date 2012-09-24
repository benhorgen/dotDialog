using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using MonoCross.Navigation;

using System.Xml.Serialization;
using System.Diagnostics;
using System.Xml.Linq;
using System.Text;
using System.Runtime.Serialization;


namespace dotDialog.Sample.PersonalInfoManger
{
	public class CalendarListController : MXController<CalendarListModel>, IMXController
	{
		public CalendarListController() { }

		public override string Load (Dictionary<string, string> parameters)
		{
			Model = LoadModel(true);
			return ViewPerspective.Default;
		}

		//TODO: Harvest this as a pattern for Model from resting point
		public static CalendarListModel LoadModel(bool createNewModelIfEmpty)
		{
			int allowedFailures = 2;
			CalendarListModel calendar = null;
			while (calendar == null && allowedFailures > 0) 
			{
				byte[] dataBytes = App.LoadBytesFromDataSource(DataUri, false);
				if (dataBytes != null)
				{
					calendar = CalendarListModel.FromBytes(dataBytes);
					if (calendar == null) { Console.WriteLine("Failed to deserlize CalendarViewModel"); }
				}
				if (calendar == null) { allowedFailures--; }
			}
			
			// check if retrieval was successful
			if (calendar == null && createNewModelIfEmpty)
			{
				calendar = new CalendarListModel();
				Console.WriteLine("An empty CalendarViewModel was created but not serialized");
			}

			return calendar;
		}

		public static bool SaveEventToCalendar(CalEvent item, bool addIfNew, bool createNewModelIfEmpty)
		{
			bool saved = false;

			var model = LoadModel(createNewModelIfEmpty);
			if (model != null)
			{
				saved = CalendarListModel.Update(model.Events, item);	
				if (!saved && addIfNew) { saved = CalendarListModel.Add(model.Events, item); }
				if (saved) { SaveModelToDisk(model); }
			}

			return saved;
		}

		//TODO: Harvest this as a pattern for saving Models to Disk
		public static bool SaveModelToDisk(CalendarListModel model)
		{
			bool saved = true;

			// Serialize the object to xml
			DataContractSerializer serializer = new DataContractSerializer(model.GetType());
			
			// Cache serialized bytes to disk
			string uri = DataUri;
			string xml = "[null]";

			try { xml = App.CacheToDisk(serializer, model, uri); }
			catch (Exception e) 
			{
				string msg = string.Format("A {0} occurred writing to disk:\r\nUri: {1}\r\nXML:\r\n{2}",
				                           e.Message, uri, xml);
				Console.WriteLine(msg);

				saved = false;
			}

			
			return saved;
		}

		public static string Uri = "Calendar";
		public static string DataUri = "http://localhost0.1/calendar.xml";
	}
}

