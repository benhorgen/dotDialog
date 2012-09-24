using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Runtime.Serialization;
using System.Collections.Generic;

using MonoCross.Navigation;

namespace dotDialog.Sample.PersonalInfoManger
{
	public class ContactListController : MXController<ContactListModel>, IMXController
	{
		public override string  Load(Dictionary<string,string> parameters)
		{
			Model = LoadModel(true); //reset model
			return ViewPerspective.Default;
		}

		//TODO: Harvest this as a pattern for Model from resting point
		public static ContactListModel LoadModel(bool createNewModelIfEmpty)
		{
			int allowedFailures = 2;
			ContactListModel model = null;
			while (model == null && allowedFailures > 0) 
			{
				var dataBytes = App.LoadBytesFromDataSource(DataUri, false);
				if (dataBytes != null)
				{
					model = ContactListModel.FromBytes(dataBytes);
					if (model == null) { Console.WriteLine("Failed to deserlize ContactViewModel"); }
				}
				if (model == null) { allowedFailures--; }
			}
			
			// check if retrieval was successful
			if (model == null && createNewModelIfEmpty)
			{
				model = new ContactListModel();
				Console.WriteLine("An empty ContactViewModel was created but not serialized");
			}

			return model;
		}

		public static bool SaveContactToDataSource(Contact Model, bool addIfNew, bool createListIfEmpty)
		{
			bool saved = false;

			//TODO: Replace with a call to the server, currently just caches back to disk record
			#region Replace with server call to post, and maybe receive back new record to cache to disk
			ContactListModel contactViewModel = LoadModel(createListIfEmpty);

			// add new contact to hydrated model and save back to disk
			if (contactViewModel != null)
			{
				saved = ContactListModel.Update(contactViewModel.Contacts, Model);
				if (!saved && addIfNew) { saved = ContactListModel.Add(contactViewModel.Contacts, Model); }
				if (saved) { saved = ContactListController.SaveModelToDisk(contactViewModel); }
			}
			#endregion

			return saved;
		}

		//TODO: Harvest this as a pattern for saving Models to Disk
		static bool SaveModelToDisk(ContactListModel model)
		{
			bool saved = true;

			DataContractSerializer serializer = new DataContractSerializer(model.GetType());
			string uri = ContactListController.DataUri;
			string xml = "[null]";
			try 
			{ xml = App.CacheToDisk(serializer, model, uri); }
			catch (Exception e) 
			{
				string msg = string.Format("A {0} occurred writing to disk:\r\nUri: {1}\r\nXML:\r\n{2}",
				                           e.Message, uri, xml);
				Console.WriteLine(msg);
				saved = false;
			}

			return saved;
		}


		public static string Uri = "ContactList";
		public static string NoData = "No Data";
		
		public const string DataUri = "https://m.qa.advisorcompass.com/cmmobile/contactlist.xml";

		public ContactListController () { }
	}

	public class TabbarController : MXController<object>, IMXController
	{
		public override string Load (Dictionary<string, string> parameters)
		{
			return ViewPerspective.Default;
		}

		public static string Uri = "TabBar";
	}
}

