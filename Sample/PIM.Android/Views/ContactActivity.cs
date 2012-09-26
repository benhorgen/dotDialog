
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using MonoCross.Droid;
using Android.Dialog;

using dotDialog.Sample.PersonalInfoManger;


namespace dotDialog.Sample.PersonalInfoManger.Android
{
	[Activity (Label = "Contact Details")]			
	public class ContactActivity : MXDialogActivityView<Contact>
	{
		public override void Render ()
		{
			var dialogSection = ContactDialogSections.CreateContactDetailSections(Model);

			var sections = ContactDialogSections.CreateContactDetailSections(Model);
			if (sections != null && sections.Length > 2)
			{
				// add a click handler to the Phone numbers
				foreach(Element e in sections[0])
				{
					StringElement se = e as StringElement;
					//se.Tapped += () => { DisplayPhoneCallDialog(se); };
				}
				
				for(int i = 0; i < sections[1].Count; i++)
				{
					StringElement se = sections[1][i] as StringElement;
					se.Click += (object sender, EventArgs e) => { StartEmailIntent(se.Value); };
				}
			}

			this.Root = new RootElement(null) { dialogSection };	
		}

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Create your application here
		}

		private void StartEmailIntent(string email)
		{
			/* Create the Intent */
			//Intent emailIntent = new Intent(Android.Content.Intent.ActionSend);
			
			/* Fill it with Data */
			//emailIntent.SetType("plain/text");
			//emailIntent.PutExtra(Android.Content.Intent.ExtraEmail, new String[]{email});
			//emailIntent.putExtra(android.content.Intent.EXTRA_SUBJECT, "Default subject");
			//emailIntent.putExtra(android.content.Intent.EXTRA_TEXT, "Default text");
			
			/* Send it off to the Activity-Chooser */
			//StartActivity(Intent.CreateChooser(emailIntent, "Send mail..."));
		}
	}
}

