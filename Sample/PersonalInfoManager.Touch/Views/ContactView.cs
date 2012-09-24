using System;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Collections.Generic;

using MonoTouch.UIKit;
using MonoTouch.Dialog;
using MonoTouch.Foundation;

using MonoCross.Touch;
using MonoCross.Navigation;

namespace dotDialog.Sample.PersonalInfoManger.Touch
{
	public partial class ContactView : MXTouchDialogView<Contact>
	{
		public ContactView() : base(UITableViewStyle.Grouped, null, true) 
		{
			// build action sheet
			var button = new UIBarButtonItem(UIBarButtonSystemItem.Action );
			button.Clicked += delegate(object sender, EventArgs e) { DisplayActionSheet(); };
			NavigationItem.SetRightBarButtonItem(button, true);
		}

		public override void Render ()
		{
			// verify model is not null
			if (Model == null) { throw new Exception("Model cannot be null when rendering: " + GetType()); }

			Root = new RootElement(Model.FirstName + " " + Model.LastName);

			// phone numbers

			var sections = ContactDialogSections.CreateContactDetailSections(Model);
			if (sections != null && sections.Length > 2)
			{
				// add a click handler to the Phone numbers
				foreach(Element e in sections[0])
		        {
					StringElement se = e as StringElement;
					se.Tapped += () => { DisplayPhoneCallDialog(se); };
				}

				for(int i = 0; i < sections[1].Count; i++)
		        {
					StringElement se = sections[1][i] as StringElement;
					se.Tapped += () => { InitiateNewEmail(se.Value); };
				}
			}
			Root.Add(sections);
		}

		void InitiateNewEmail(string email)
		{
			new System.Threading.Thread (() => {
				using (new NSAutoreleasePool()) 
				{
					try {
						new NSObject().InvokeOnMainThread( () => 
						                                  //TODO: Replace the Model.Email with the unique email address
						                                  UIApplication.SharedApplication.OpenUrl(new NSUrl("mailto:" + email))); 
					}
					catch (Exception ex) { Console.WriteLine("Exception occurred trying to email your contact:\r\n" + ex); }
				}
			}).Start();
		}

		void DisplayPhoneCallDialog(StringElement element)
		{
			string errorMessage = null;
			bool showErrorDialog = false;

			string name = string.Format("{0} {1}?", Model.FirstName, Model.LastName);
			string phoneNumber = element.Value;
			if (!string.IsNullOrEmpty(phoneNumber))
			{
				string phone = null;
				try
				{
					// loop through and pull numbers only (remove formatting)
					string str = phoneNumber;
					StringBuilder sb = new StringBuilder();
					for (int i = 0; i < str.Length; i++)
					{
						if (str[i] >= '0' && str[i] <= '9') { sb.Append(str[i]); }
					}
					phone = sb.ToString();

					var buttons = new string[] {"OK"};
					phoneCallAlertView = new UIAlertView("Dial", "Call " + name, new PhoneAlertDelegate(phone), "Cancel", buttons);

					try
					{
						new System.Threading.Thread (() => 
						{
							using (new MonoTouch.Foundation.NSAutoreleasePool()) 
							{
								phoneCallAlertView.Show(); 
							}
						}).Start();
					}
					catch (Exception ex)
					{
						Debug.WriteLine("Following exception occurred while displaying dialing dialog:\r\n" + ex);
						showErrorDialog = true;
						errorMessage = "Could not initiate a phone call";
					}

				}
				catch (Exception e)
				{
					Debug.WriteLine("Following exception occurred while parsing # to dial:\r\n" + e);
					showErrorDialog = true;
					errorMessage = "please check phone # format";
				}
			}
			else
			{
				showErrorDialog = true;
				errorMessage = "No phone number for " + name;
			}

			if (showErrorDialog) {
				new UIAlertView("Error Dialing", errorMessage, null, "OK", null).Show();
			}
		}
		UIAlertView phoneCallAlertView = null;

		void DisplayActionSheet()
		{
			string[] ActionItems = menuItems;
			var actionSheet = new UIActionSheet(string.Empty, null, "Cancel", null, ActionItems){
				Style = UIActionSheetStyle.Default,
			};
			actionSheet.Clicked += delegate (object s, UIButtonEventArgs a){ ActionSheetClickDelegate(s,a); };
			actionSheet.ShowInView(View.Window);
		}
		void ActionSheetClickDelegate(object sender, UIButtonEventArgs args)
		{
			string menuItemText = "Cancel";
			if (args.ButtonIndex > menuItems.Length) 
			{ 
				string msg = "Unexpected value was " + args.ButtonIndex;
				throw new ArgumentOutOfRangeException("args.ButtonIndex",msg);
			}
			else if (args.ButtonIndex < menuItems.Length) 
			{ 
				switch(args.ButtonIndex)
				{
				case 0:
					MXTouchContainer.Navigate(CalendarEventController.UriForNew());
					break;
				case 1:
					MXTouchContainer.Navigate(TaskController.Uri(Model.Id));
					break;
				default:
					Console.WriteLine("Unexpected args.ButtonIndex value of: " + args.ButtonIndex);
					break;
				}
			}
			
			Debug.WriteLine("Clicked on item {0}, index {1}", menuItemText, args.ButtonIndex);
		}	
		string[] menuItems = new string[] { "New Appointment", "New Task" };
	}
}

