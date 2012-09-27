using System;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Collections.Generic;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

using MonoTouch.Dialog;
using MonoTouch.Dialog.AddOn;

using MonoCross.Touch;
using MonoCross.Navigation;

namespace dotDialog.Sample.PersonalInfoManger.Touch
{
	public partial class TaskView : MXTouchDialogView<Task>
	{
		public TaskView() : base(UITableViewStyle.Grouped, null, true) { }

		public override void Render ()
		{
			// verify model is not null
			if (Model == null) { throw new Exception("Model cannot be null when rendering: " + GetType()); }

			Root = new RootElement(Model.Date.ToString("d"));
			var sections = TaskDialogSections.CreateTaskDetailSections(Model);
			Root.Add(sections);

			string updateUri = TaskController.Uri(Model.Id, ViewPerspective.Update);
			var editButton = GlassButtonExtension.CreateGlassButton("Edit Task");
			editButton.TouchUpInside += (sender, e) => { MXTouchContainer.Navigate(updateUri); };
			Section buttonSection = new Section() { editButton };
			Root.Add(buttonSection);
		}
	}
}

