
using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;

using MonoCross.Touch;
using MonoCross.Navigation;

using System.Collections.Generic;


namespace dotDialog.Sample.PersonalInfoManger.Touch
{
	public partial class ContactEditView : MXTouchDialogView<Contact>
	{
		public ContactEditView () : base (UITableViewStyle.Grouped, null, true) 
		{ 
			// build action sheet
			var button = new UIBarButtonItem("Create", UIBarButtonItemStyle.Done, null);
			button.Clicked += delegate(object sender, EventArgs e) { BarButton_Click(button); };
			NavigationItem.SetRightBarButtonItem(button, true);
		}

		public override void Render ()
		{
			sections = ContactEditDialogSections.BuildDialogSections(Model);
			Root = new RootElement("Add Contact") { sections };
		}
		Section[] sections = null;

		private void BarButton_Click(UIBarButtonItem button)
		{
			ContactEditDialogSections.SaveDialogElementsToModel(Model, sections);

			bool createNew = (button.Title == "Create");

			bool success = ContactListController.SaveContactToDataSource(Model, createNew, true);
			if (success)
			{
				MXTouchContainer.Navigate(ContactListController.Uri);
			}
			else
				new UIAlertView("Error", "Failed to save", null, "Ok", null).Show();
		}
	}
}