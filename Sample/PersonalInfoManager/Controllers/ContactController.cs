using System;
using System.Linq;
using System.Collections.Generic;

using MonoCross.Navigation;

namespace dotDialog.Sample.PersonalInfoManger
{
	public class ContactController : MXController<Contact>, IMXController
	{
		public override string  Load(Dictionary<string,string> parameters) 
		{ 
			string crudOperation = "Read";
			parameters.TryGetValue(ContactController.crudKey, out crudOperation);
			
			string id = string.Empty;
			if (!parameters.TryGetValue(ContactController.idKey, out id))
			{
				string msg = "NO " + ContactController.idKey + " for " + GetType().ToString() + ".Load()";
				Console.WriteLine(msg);
			}
			
			
			var contactViewModel = ContactListController.LoadModel(true);
			if (contactViewModel != null) 
			{
				Model = (from individual in contactViewModel.Contacts 
				         where individual.Id == id 
				         select individual).FirstOrDefault();
			}
			else { Console.WriteLine("Failed to deserlize Contacts when looking up an individual one"); }
			
			string vp = ViewPerspective.Default;
			switch (crudOperation)
			{
			case "Create":
				Model = new Contact();
				vp = ViewPerspective.Create;
				break;
			case "Update":
				vp = ViewPerspective.Update;
				if (Model == null) { Console.WriteLine("WARNING: Controller can't find contact for update"); }
				break;
			}
			if (Model == null) { vp = ContactController.NoData; }
			
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
			var contactDetailController = new ContactController();
			
			string uri = ContactController.Uri("{" + ContactController.idKey + "}");
			theApp.NavigationMap.Add(uri, contactDetailController);
			
			string updateUri = ContactController.Uri("{" + ContactController.idKey + "}", "{" + ContactController.crudKey + "}");
			theApp.NavigationMap.Add(updateUri, contactDetailController);
			
		}
		
		private static string baseUri = "Contact/";
		public static string idKey = "id";
		public static string crudKey = "crud";
		public const string NoData = "No Data";

		public ContactController () { }
	}
}

