using System;
using System.Collections.Generic;
using MonoTouch.UIKit;
using MonoCross.Touch;
using MonoTouch.Dialog;
using MonoTouch.Dialog.AddOn;

using System.Drawing;


namespace dotDialog.Sample.PersonalInfoManger.Touch
{
	public class CalendarEventUpdateView : MXTouchDialogView<CalEvent>
	{
		public CalendarEventUpdateView() : base(UITableViewStyle.Grouped, null, true) { }
		public CalendarEventUpdateView(string buttonText) : base(UITableViewStyle.Grouped, null, true) 
		{ 
			_buttonSection = new Section();
			var glassButton = new GlassButton(new RectangleF(0, 0, 300, 44));
			glassButton.SetTitle("Delete", UIControlState.Normal);
			glassButton.TouchUpInside += DeleteButton_Click;
			UIViewElement buttonElement = new UIViewElement(string.Empty, glassButton, true);
			buttonElement.Flags = UIViewElement.CellFlags.DisableSelection | UIViewElement.CellFlags.Transparent;
			_buttonSection.Add(buttonElement);


			var button = new UIBarButtonItem(buttonText, UIBarButtonItemStyle.Done, null);
			button.Clicked += delegate(object sender, EventArgs e) { BarButton_Click(button); };
			NavigationItem.SetRightBarButtonItem(button, true);		
		}

		public override void Render () 
		{ 
			sections = CalendarEventUpdateDialogSections.BuildDialogSections(Model);

			sections.Add(_buttonSection);

			Root = new RootElement("Edit Event") { sections };
		}
		List<Section>  sections;
		Section _buttonSection;

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			NavigationController.SetNavigationBarHidden(false, false);
		}

		private void BarButton_Click(UIBarButtonItem button)
		{
			CalendarEventUpdateDialogSections.SaveDialogElementsToModel(Model, sections);

			bool createNew = (button.Title == "Create");
			bool success = CalendarListController.SaveEventToCalendar(Model, createNew, true);

			if (success)
				MXTouchContainer.Navigate(CalendarListController.Uri);
			else
				new UIAlertView("Error", "Failed to save event", null, "Ok", null).Show();
		}

		private void DeleteButton_Click(object o, EventArgs e)
		{
			//TODO:  Replace with actual delete command (probably in the controller through)
			new UIAlertView("Calendar event would be deleted", string.Empty, null, "Ok", null).Show();

			new System.Threading.Thread (() => 
			{
				using (new MonoTouch.Foundation.NSAutoreleasePool()) 
				{
					MXTouchContainer.Navigate(CalendarListController.Uri);
				}
			}).Start ();
		}
	}
}

