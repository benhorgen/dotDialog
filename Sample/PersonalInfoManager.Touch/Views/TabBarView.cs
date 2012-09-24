using System;
using System.Linq;
using System.Drawing;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

using MonoTouch;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

using MonoCross.Touch;
using MonoTouch.Dialog;
using MonoCross.Navigation;

namespace dotDialog.Sample.PersonalInfoManger.Touch
{
	public class TabBar : UITabBarController, IMXView
	{
		#region IMXView implementation
		public void SetModel (object model)
		{
			Model = model;
		}

		public Type ModelType { get { return Model.GetType(); } }
		#endregion

		#region IMXView implementation

		public void Render ()
		{
			return;
		}
		Object Model;
		#endregion

		public TabBar(MXTouchContainer touchContainer) 
		{
			Model = new Object();

			// define view size
			View.Frame = new RectangleF (0, 20, 320, 460); 
			
			// setup view controllers for each tab
			var navBarTint = UIColor.FromRGB(0, 63, 107);
			var rootTabBarCtrls = new List<UIViewController>(3);
			
			var navCtrl = new UINavigationController();
			navCtrl.NavigationBar.TintColor = navBarTint;
			navCtrl.TabBarItem = new UITabBarItem("Contacts", UIImage.FromBundle("images/contacts.png"), 0);
			rootTabBarCtrls.Add(navCtrl);
			
			navCtrl = new UINavigationController();
			navCtrl.NavigationBar.TintColor = navBarTint;
			navCtrl.TabBarItem = new UITabBarItem("Calendar", UIImage.FromBundle("images/cal.png"), 0);
			rootTabBarCtrls.Add(navCtrl);
			
			navCtrl = new UINavigationController();
			navCtrl.NavigationBar.TintColor = navBarTint;
			navCtrl.TabBarItem = new UITabBarItem("Tasks", UIImage.FromBundle("images/filecab.png"), 0);
			rootTabBarCtrls.Add(navCtrl);
			
			SetViewControllers(rootTabBarCtrls.ToArray(), false);
			Delegate = new TabBarDelegate();
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			SelectedIndex = 0;
			MXTouchContainer.Navigate(ContactListController.Uri);
		}
	}

	public class TabBarDelegate : UITabBarControllerDelegate
	{		
		public override void ViewControllerSelected(UITabBarController tabBarController, UIViewController viewController)
		{
			var renderStart = DateTime.Now;

			string uri = null;
			switch (tabBarController.SelectedIndex) 
			{
			case 0:
				uri = ContactListController.Uri;
				break;
			case 1:
				uri = CalendarListController.Uri;
				break;
			case 2:
				uri = TaskListController.Uri;
				break;
			}
			
			var vc = (UINavigationController)tabBarController.ViewControllers[tabBarController.SelectedIndex];
			if (vc.TopViewController == null)
			{
				new System.Threading.Thread (() => 
				{
					using (new MonoTouch.Foundation.NSAutoreleasePool()) 
					{
							MXTouchContainer.Navigate(uri);							
					}
				}).Start ();
			}
			else { Debug.WriteLine("TopViewController is not null, not navigating to: " + uri); }

			Console.WriteLine("ViewControllerSelected cost: {0:0.0}ms", DateTime.Now.Subtract(renderStart).TotalMilliseconds);
		}
	}
}


