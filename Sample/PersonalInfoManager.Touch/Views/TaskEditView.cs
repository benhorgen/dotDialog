
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
			var button = new UIBarButtonItem(barButtonText, UIBarButtonItemStyle.Done, null);
			button.Clicked += delegate(object sender, EventArgs e) { BarButton_Click(button); };
			NavigationItem.SetRightBarButtonItem(button, true);
		}

		public override void Render ()
		{
			_sections = TaskEdiDialogSections.BuildDialogSections(Model);
			Root = new RootElement(Model.Date.ToShortDateString()) { _sections };
		}
		Section[] _sections;

		private void BarButton_Click(UIBarButtonItem button)
		{
			TaskEdiDialogSections.SaveDialogElementsToModel(Model, _sections);

			bool createNew = (button.Title == "Create");
			bool success = TaskListController.SaveTaskToDataSource(Model, createNew, true);

			if (success)
				MXTouchContainer.Navigate(TaskListController.Uri);
			else
				new UIAlertView("Error", "Failed to save", null, "Ok", null).Show();
		}
	}
}


