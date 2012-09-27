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
			string crudOperation = ViewPerspective.Default;
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
			
			switch (crudOperation)
			{
			case ViewPerspective.Default:
				break; //nothing to do
			case ViewPerspective.Create:
				Model = new Contact();
				break;
			case ViewPerspective.Update:
				if (Model == null) { Console.WriteLine("WARNING: Controller can't find contact for update"); }
				break;
			case ViewPerspective.Delete:
				//TODO:  Implement Delete CRUD operation
				Console.WriteLine("DELETE is not implemented for contact yet");
				MXContainer.Instance.Redirect(ContactListController.Uri);
				break;
			default:
				Console.WriteLine("Unexpected crud operation string value of: " + crudOperation);
				crudOperation = ViewPerspective.Default; //set to default if unknown
				break;
			}
			if (Model == null && crudOperation != ViewPerspective.Create) 
			{
				Console.WriteLine(string.Format("Null Model, overriding crud from \"{0}\" to \"{1}\"", 
				                                crudOperation, ContactController.NoData));
				crudOperation = ContactController.NoData;
			}

			return crudOperation;
		}

		public static string Uri(string id) { return Uri(id, ViewPerspective.Default); }
		public static string Uri(string id, string crud) 
		{ 
			return string.Format("{0}{1}/{2}", baseUri, crud, id);
		}
		public static string UriForNew() { return Uri("0", ViewPerspective.Create); }
		
		
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

