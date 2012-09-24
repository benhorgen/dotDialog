using System;
using System.Globalization;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.IO;
using System.Xml;

namespace dotDialog.Sample.PersonalInfoManger
{
    [DataContract]
    public class ContactListModel
	{
		public ContactListModel() { Contacts = new List<Contact>(); }

		[DataMember(Name = "Contacts")]
		public List<Contact> Contacts;

		[DataMember]
		public long StaleDate { get; set; }

		[DataMember]
		public  long ExpirationDate { get; set; }

		#region Supporting Methods and constructors
		public static ContactListModel FromBytes(byte[] bytes)
		{
			ContactListModel result = null;
			try 
			{
				MemoryStream fs = new MemoryStream(bytes, 0, bytes.Length);
				XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());
				
				// Create the DataContractSerializer instance.
				DataContractSerializer ser = new DataContractSerializer(new ContactListModel().GetType());
				
				// Deserialize the data and read it from the instance.
				result = (ContactListModel)ser.ReadObject(reader);
				fs.Close();
			}
			catch (Exception e) { Console.WriteLine("The following exception occurred deserializing:\r\n" + e); }
			
			return result; 
		}
		
		public static bool Add(List<Contact> contacts, Contact c) 
		{ 
			//TODO: Replace with a post to a server
			bool added = false;
			if (contacts != null) 
			{ 
				contacts.Add(c); 
				added = true;
			}
			else { throw new ArgumentNullException("tasks", "task cannot be added to a null list"); }
			return added;
		}
		
		public static bool Update(IEnumerable<Contact> contacts, Contact contact) 
		{
			bool updated = false;
			if (contacts != null)
			{
				foreach(Contact c in contacts)
				{
					if (c.Id == contact.Id)
					{
						c.Address = contact.Address;
						c.Email = contact.Email;
						c.FirstName = contact.FirstName;
						c.LastName = contact.LastName;
						c.Phone = contact.Phone;

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

	[DataContract] 
    public class Contact
    {
        public Contact() { Id = Guid.NewGuid().ToString(); }

		[DataMember]
        public string Id { get; set; }

		[DataMember]
        public string FirstName { get; set; }

		[DataMember]
        public string LastName { get; set; }

		[DataMember]
        public string Email { get; set; }

		[DataMember]
        public string Phone { get; set; }

		[DataMember]
        public string Address { get; set; }
    }	
}