
using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;

using MonoCross.Touch;
using MonoCross.Navigation;

using System.Collections.Generic;
using MonoTouch.Dialog.AddOn;


namespace dotDialog.Sample.PersonalInfoManger.Touch
{
	public partial class ContactEditView : MXTouchDialogView<Contact>
	{
		public ContactEditView (string barButtonText) : base (UITableViewStyle.Grouped, null, true) 
		{ 
			// build action sheet
			button = new UIBarButtonItem(barButtonText, UIBarButtonItemStyle.Done, null);
			button.Clicked += delegate(object sender, EventArgs e) { BarButton_Click(button); };
			NavigationItem.SetRightBarButtonItem(button, true);
		}

		public override void Render ()
		{
			sections = ContactEditDialogSections.BuildDialogSections(Model);
			Root = new RootElement("Add Contact") { sections };

			if (button.Title == SaveButtonText)
			{
				var deleteButton = GlassButtonExtension.CreateGlassButton("Delete", UIColor.Red);
				deleteButton.TouchUpInside += (object sender, EventArgs e) => {
					MXTouchContainer.Navigate(ContactController.Uri(Model.Id, ViewPerspective.Delete));
				};
				var gbs = GlassButtonExtension.CreateGlassButtonElement(deleteButton);
				Section buttonSection = new Section() { gbs };
				Root.Add(buttonSection);
			}
		}
		Section[] sections = null;

		private void BarButton_Click(UIBarButtonItem button)
		{
			ContactEditDialogSections.SaveDialogElementsToModel(Model, sections);

			bool createNew = (button.Title == CreateButtonText);

			bool success = ContactListController.SaveContactToDataSource(Model, createNew, true);
			if (success)
			{
				MXTouchContainer.Navigate(ContactListController.Uri);
			}
			else
				new UIAlertView("Error", "Failed to save", null, "Ok", null).Show();
		}
		UIBarButtonItem button;

		public static string SaveButtonText = "Save";
		public static string CreateButtonText = "Create";
	}
}