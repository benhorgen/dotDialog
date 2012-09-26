using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MonoCross.Droid;

using dotDialog.Sample.PersonalInfoManger;

namespace dotDialog.Sample.PersonalInfoManger.Android
{

	[Activity(Label = "Contact List")]// Icon = "@drawable/icon_36")]
	public class ContactListActivity : MXListActivityView<ContactListModel>
	{
		public override void Render()
		{
			#region Create a fake contact
			//TODO: Remove as soon as Android has a Add button
			if (Model != null && Model.Contacts.Count < 1)
			{
				Model.Contacts.Add(new Contact() {
					FirstName = "John",
					LastName = "Dow",
					Email = "john@johndow.com",
					Phone = "(612)555-1212,",
					Address = "4200 1st Ave NW Minneapolis, MN 55402",
				});
			}
			var s = new DataContractSerializer(Model.GetType());
			App.CacheToDisk(s, Model, ContactListController.DataUri);
			#endregion

			ListView.Adapter = new ContactAdapter(this, 0, Model);
		}

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			
			// Create your application here
		}

		protected override void OnListItemClick(ListView l, View v, int position, long id)
		{
			base.OnListItemClick(l, v, position, id);
			
			MXDroidContainer.Navigate(string.Format(ContactController.Uri(Model.Contacts[position].Id)));
		}
		
		class ContactAdapter : ArrayAdapter<Contact>
		{
			List<Contact> items;
			
			public ContactAdapter(Context context, int textViewResourceId, ContactListModel items)
				: base(context, textViewResourceId, items.Contacts)
			{
				this.items = items.Contacts;
			}
			
			public override View GetView(int position, View convertView, ViewGroup parent)
			{
				View v = convertView;
				if (v == null) {
					LayoutInflater li = (LayoutInflater)this.Context.GetSystemService(Context.LayoutInflaterService);
					v = li.Inflate(Android.Resource.Layout.list_view, null);
				}
				
				Contact c = items[position];
				if (c != null) {
					TextView tt = (TextView)v.FindViewById(Android.Resource.Id.list_view_Text);
					if (tt != null)
						tt.Text = string.Format("{0}, {1}", c.LastName, c.FirstName);
				}
				return v;
			}
		}
	}
}

