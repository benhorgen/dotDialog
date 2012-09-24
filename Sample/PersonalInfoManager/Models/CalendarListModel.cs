using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Linq;
using System.IO;
using System.Xml;

namespace dotDialog.Sample.PersonalInfoManger
{
	[DataContract]
	public class CalendarListModel
	{
		[DataMember]
		public List<CalEvent> Events;

		[DataMember]
		public long StaleDate { get; set; }

		[DataMember]
		public long ExpirationDate { get; set; }

		public void Add(CalEvent e) { Events.Add(e); }

		public CalendarListModel() { Events = new List<CalEvent>(); }

		#region Supporting Methods and constructors
		public static CalendarListModel FromBytes(byte[] bytes)
		{
			CalendarListModel result = null;
			try 
			{
				MemoryStream fs = new MemoryStream(bytes, 0, bytes.Length);
				XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());
				
				// Create the DataContractSerializer instance.
				DataContractSerializer ser = new DataContractSerializer(new CalendarListModel().GetType());
				
				// Deserialize the data and read it from the instance.
				result = (CalendarListModel)ser.ReadObject(reader);
				fs.Close();
			}
			catch (Exception e) { Console.WriteLine("The following exception occurred deserializing:\r\n" + e); }
			
			return result; 
		}
		
		public static bool Add(List<CalEvent> events, CalEvent c) 
		{ 
			//TODO: Replace with a post to a server
			bool added = false;
			if (events != null) 
			{ 
				events.Add(c); 
				added = true;
			}
			else { throw new ArgumentNullException("events", "CalEvent cannot be added to a null list"); }
			return added;
		}
		
		public static bool Update(IEnumerable<CalEvent> events, CalEvent calEvent) 
		{
			bool updated = false;
			if (events != null)
			{
				foreach(CalEvent c in events)
				{
					if (c.Id == calEvent.Id)
					{
						c.Description = calEvent.Description;
						c.EndTimeAsLong = calEvent.EndTimeAsLong;
						c.Location = calEvent.Location;
						c.Priority = calEvent.Priority;
						c.StartTimeAsLong = calEvent.StartTimeAsLong;
						c.Subject = calEvent.Subject;
						
						updated = true;
						break;
					}
				}
			}
			else { throw new ArgumentNullException("tasks", "task cannot be updated in a null list"); }
			return updated;
		}
		#endregion
	}

	public class CalEvent
	{
		public CalEvent() 
		{ 
			Id = Guid.NewGuid().ToString(); 

			//Set start time to next hour
			var	dateValue = DateTime.Now.Date;
			if (DateTime.Now.Hour != 23) 
			{
				dateValue = dateValue.AddHours(DateTime.Now.Hour + 1);
			}
			else { dateValue = dateValue.AddDays(1); }
			StartTimeAsLong = dateValue.Ticks;
		}

        public string Id { get; set; }
		public string Type { get; set; }
        public string Subject { get; set; }
		public long StartTimeAsLong { get; set; }
		public long EndTimeAsLong { get; set; }
		public string Location { get; set; }
		public string Description { get; set; }
		public string Priority { get; set; }

		public DateTime StartTime 
		{ 
			get { return new DateTime(StartTimeAsLong); } 
			set { StartTimeAsLong = value.Ticks; }
		}

		public DateTime EndTime 
		{
			get { return new DateTime(EndTimeAsLong); } 
			set { EndTimeAsLong = value.Ticks; }
		}
	}
}

