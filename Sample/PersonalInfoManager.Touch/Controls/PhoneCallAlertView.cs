using System;
using System.Diagnostics;

using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace dotDialog.Sample.PersonalInfoManger.Touch
{
	class PhoneAlertDelegate : UIAlertViewDelegate
	{	
		public PhoneAlertDelegate(string phoneNumber)
		{
			if (string.IsNullOrEmpty(phoneNumber))
			{
				throw new ArgumentNullException("phoneNumber", "Phone number cannot be null");
			}
			_phone = phoneNumber;
		}
		public override void Clicked(UIAlertView alertView, int buttonIndex)
		{
			if (buttonIndex > 0) //if not the cancel button
			{
				Debug.WriteLine("Will dial the phone #: " + _phone);
				
				new System.Threading.Thread (() => 
				                             {
					using (new MonoTouch.Foundation.NSAutoreleasePool()) 
					{
						try { 
							UIApplication.SharedApplication.OpenUrl(new NSUrl("tel:" + _phone)); 
						}
						catch (Exception e)
						{
							Debug.WriteLine("Following exception occurred while trying to dial:\r\n" + e);
							new UIAlertView("Error Dialing", "please check phone #", null, "OK", null).Show();
						}
					}
				}).Start();
			}
		}
		
		string _phone;
	}	
}

