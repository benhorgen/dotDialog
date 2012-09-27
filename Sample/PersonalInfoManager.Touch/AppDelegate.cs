using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoCross.Touch;
using System.IO;
using MonoCross.Navigation;

namespace dotDialog.Sample.PersonalInfoManger.Touch
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the 
	// User Interface of the application, as well as listening (and optionally responding) to 
	// application events from iOS.
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		UIWindow _window;
		
		public static TabBar AppTabBar;
		
		// This method is invoked when the application has loaded its UI and its ready to run
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			_window = new UIWindow(UIScreen.MainScreen.Bounds);
			var mxApp = new App();
			MXTouchContainer.Initialize(mxApp, this, _window);
			
			// Setup the applications structure (i.e. tab bar or splitviw controller)
			AppTabBar = new TabBar((MXTouchContainer)MXTouchContainer.Instance);
			((MXTouchContainer)MXTouchContainer.Instance).SetTabBarControllerAsRoot(AppTabBar);
			
			// Add Contact related views
			MXTouchContainer.AddView<ContactListModel>(new ContactListView());
			string noDataMsg = "Couldn't retreive contact list";
			MXTouchContainer.AddView<ContactListModel>(new NoConnectionContactListView(ContactListView.ViewTitle, noDataMsg, 0.85f), "No Data");
			
			MXTouchContainer.AddView<Contact>(new ContactView());
			MXTouchContainer.AddView<Contact>(new ContactEditView(ContactEditView.CreateButtonText), ViewPerspective.Create);
			MXTouchContainer.AddView<Contact>(new ContactEditView(ContactEditView.SaveButtonText), ViewPerspective.Update);

			// Add Calendar related views
			MXTouchContainer.AddView<CalendarListModel>(new CalendarView());			
			MXTouchContainer.AddView<CalEvent>(new CalendarEventView());
			MXTouchContainer.AddView<CalEvent>(new CalendarEventUpdateView(CalendarEventUpdateView.CreateButtonText), ViewPerspective.Create);			
			MXTouchContainer.AddView<CalEvent>(new CalendarEventUpdateView(CalendarEventUpdateView.SaveButtonText), ViewPerspective.Update);

			// Add Task related views
			MXTouchContainer.AddView<List<Task>>(new TaskListView());
			MXTouchContainer.AddView<Task>(new TaskView());
			MXTouchContainer.AddView<Task>(new TaskEditView(TaskEditView.CreateButtonText), ViewPerspective.Create);
			MXTouchContainer.AddView<Task>(new TaskEditView(TaskEditView.SaveButtonText), ViewPerspective.Update);

			MXTouchContainer.AddView<object>(new TabBar((MXTouchContainer)MXTouchContainer.Instance));

			// Kick of the application with a naviation call.
			MXTouchContainer.Navigate(MXContainer.Instance.App.NavigateOnLoad);
			
			return true;
		}
	}
}

