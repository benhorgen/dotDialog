
using System;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

using MonoCross.Touch;
using MonoCross.Navigation;

using MonoTouch.Dialog;
using MonoTouch.Dialog.AddOn;



namespace dotDialog.Sample.PersonalInfoManger.Touch
{
	public partial class TaskEditView : MXTouchDialogView<Task>
	{
		public TaskEditView(string barButtonText) : base (UITableViewStyle.Grouped, null, true) 
		{ 
			button = new UIBarButtonItem(barButtonText, UIBarButtonItemStyle.Done, null);
			button.Clicked += delegate(object sender, EventArgs e) { BarButton_Click(button); };
			NavigationItem.SetRightBarButtonItem(button, true);
		}

		public override void Render ()
		{
			// add shared dialog sections
			_sections = TaskEdiDialogSections.BuildDialogSections(Model);
			Root = new RootElement(Model.Date.ToShortDateString()) { _sections };

			// add a delete button
			if (button.Title == SaveButtonText)
			{
				var editButton = GlassButtonExtension.CreateGlassButton("Delete", UIColor.Red);
				string deleteUri = TaskController.Uri(Model.Id, ViewPerspective.Delete);
				editButton.TouchUpInside += (sender, e) => { MXTouchContainer.Navigate(deleteUri); };
				Section buttonSection = new Section() { editButton };
				Root.Add(buttonSection);
			}
		}
		Section[] _sections;

		private void BarButton_Click(UIBarButtonItem button)
		{
			// save dialog values back to model using shared dotDialog APIs
			TaskEdiDialogSections.SaveDialogElementsToModel(Model, _sections);

			// save the task, create new if determined appropriate
			bool createNew = (button.Title == CreateButtonText);
			bool success = TaskListController.SaveTaskToDataSource(Model, createNew, true);

			if (success)
				MXTouchContainer.Navigate(TaskListController.Uri);
			else
				new UIAlertView("Error", "Failed to save", null, "Ok", null).Show();
		}
		UIBarButtonItem button;

		public static string SaveButtonText = "Save";
		public static string CreateButtonText = "Create";
	}
}


