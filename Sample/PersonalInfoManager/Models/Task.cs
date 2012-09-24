using System;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.IO;
using System.Xml;


namespace dotDialog.Sample.PersonalInfoManger
{
	[DataContract]
	public class Task
	{
		[DataMember]
		public string Id;

		[DataMember]
		public DateTime Date;

		[DataMember]
		public string Description;

		#region Supporting methods and constructors
		public Task () 
		{
			Id = Guid.NewGuid().ToString();
			//TODO: Replace with code that rounds up to neared hour
			Date = DateTime.Now;
		}

		public static List<Task> BytesToTaskList(byte[] bytes)
		{
			List<Task> results = null;
			try 
			{
				MemoryStream fs = new MemoryStream(bytes, 0, bytes.Length);
				XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());
				
				// Create the DataContractSerializer instance.
				DataContractSerializer ser = new DataContractSerializer(new List<Task>().GetType());
				
				// Deserialize the data and read it from the instance.
				results = (List<Task>)ser.ReadObject(reader);
				fs.Close();
			}
			catch (Exception e) { Console.WriteLine("The following exception occurred deserializing:\r\n" + e); }

			return results; 
		}

		public static bool AddTask(List<Task> tasks, Task t) 
		{ 
			//TODO: Replace with a post to a server
			bool added = false;
			if (tasks != null) 
			{ 
				tasks.Add(t); 
				added = true;
			}
			else { throw new ArgumentNullException("tasks", "task cannot be added to a null list"); }
			return added;
		}

		public static bool UpdateTask(IEnumerable<Task> tasks, Task task) 
		{
			bool updated = false;
			if (tasks != null)
			{
				foreach(Task t in tasks)
				{
					if (t.Id == task.Id)
					{
						t.Date = task.Date;
						t.Description = task.Description;

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
}

