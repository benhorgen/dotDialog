using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using MonoCross.Droid;
using MonoCross.Navigation;

using dotDialog.Sample.PersonalInfoManger;

namespace dotDialog.Sample.PersonalInfoManger.Android
{
	[Activity (Label = "Info Mgr", Theme = "@android:style/Theme.Black.NoTitleBar", MainLauncher = true, Icon = "@drawable/icon", NoHistory = true)]
	public class SplashActivity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate(bundle);
			
			// assign a layout with an image
			SetContentView(Resource.Layout.Splash);
			
			// initialize app
			MXDroidContainer.Initialize(new dotDialog.Sample.PersonalInfoManger.App(), this.ApplicationContext);
			
			// initialize views
			MXDroidContainer.AddView<ContactListModel>(typeof(ContactListActivity));
			MXDroidContainer.AddView<Contact>(typeof(ContactActivity));
			
			// navigate to first view
			MXDroidContainer.Navigate(null, MXContainer.Instance.App.NavigateOnLoad);
		}
		
		protected override void OnResume()
		{
			base.OnResume();
		}
	}
}


