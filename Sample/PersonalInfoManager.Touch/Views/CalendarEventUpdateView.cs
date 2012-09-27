using System;
using System.Collections.Generic;
using MonoTouch.UIKit;
using MonoCross.Touch;
using MonoTouch.Dialog;
using MonoTouch.Dialog.AddOn;

using System.Drawing;
using MonoCross.Navigation;


namespace dotDialog.Sample.PersonalInfoManger.Touch
{
	public class CalendarEventUpdateView : MXTouchDialogView<CalEvent>
	{
		public CalendarEventUpdateView() : base(UITableViewStyle.Grouped, null, true) { }
		public CalendarEventUpdateView(string buttonText) : base(UITableViewStyle.Grouped, null, true) 
		{ 
			button = new UIBarButtonItem(buttonText, UIBarButtonItemStyle.Done, null);
			button.Clicked += delegate(object sender, EventArgs e) { BarButton_Click(button); };
			NavigationItem.SetRightBarButtonItem(button, true);		
		}

		public override void Render () 
		{ 
			sections = CalendarEventUpdateDialogSections.BuildDialogSections(Model);

			Root = new RootElement("Edit Event") { sections };

			// add a delete button if editing an existing calendar event
			if (button.Title == SaveButtonText)
			{
				var deleteButton = GlassButtonExtension.CreateGlassButton("Delete", UIColor.Red);
				deleteButton.TouchUpInside += DeleteButton_Click;
				var buttonElement = GlassButtonExtension.CreateGlassButtonElement(deleteButton);
				Section buttonSection = new Section() { buttonElement };
				Root.Add(buttonSection);
			}
		}
		List<Section>  sections;

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			NavigationController.SetNavigationBarHidden(false, false);
		}

		private void BarButton_Click(UIBarButtonItem button)
		{
			CalendarEventUpdateDialogSections.SaveDialogElementsToModel(Model, sections);

			bool createNew = (button.Title == CreateButtonText);
			bool success = CalendarListController.SaveEventToCalendar(Model, createNew, true);

			if (success)
				MXTouchContainer.Navigate(CalendarListController.Uri);
			else
				new UIAlertView("Error", "Failed to save event", null, "Ok", null).Show();
		}
		UIBarButtonItem button;

		private void DeleteButton_Click(object o, EventArgs e)
		{
			new UIAlertView("Calendar event would be deleted", string.Empty, null, "Ok", null).Show();

			var deleteUri = CalendarEventController.Uri(Model.Id, ViewPerspective.Delete);
			new System.Threading.Thread (() => 
			{
				using (new MonoTouch.Foundation.NSAutoreleasePool()) 
				{
					MXTouchContainer.Navigate(deleteUri);
				}
			}).Start ();
		}

		public static string SaveButtonText = "Save";
		public static string CreateButtonText = "Create";
	}
}

