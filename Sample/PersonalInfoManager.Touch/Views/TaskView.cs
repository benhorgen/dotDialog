using System;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Collections.Generic;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

using MonoTouch.Dialog;

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

			Section buttonSection = new Section();
			buttonSection.Add(CreateEditButton("Edit Task", TaskController.Uri(Model.Id, ViewPerspective.Update)));
			Root.Add(buttonSection);
		}

		public static Element CreateEditButton(string buttonLabelText, string uri)
		{
			float labelHeight = 44;
			float buttonWidth = 300;
			GlassButton button = new GlassButton(new RectangleF(0, 0, buttonWidth, labelHeight));
			button.NormalColor = UIColor.FromRGB(0, 63, 107);
			button.HighlightedColor = UIColor.LightGray;
			button.SetTitleColor(UIColor.FromRGBA(255, 255, 255, 255), UIControlState.Normal);
			button.SetTitleColor(UIColor.FromRGBA(0, 0, 0, 255), UIControlState.Highlighted);
			button.HorizontalAlignment = UIControlContentHorizontalAlignment.Center;
			button.SetTitle(buttonLabelText, UIControlState.Normal);
			button.TouchUpInside += (sender, e) => 
			{ 
				// navigate to next screen
				string buttonTitle = button.Title(UIControlState.Normal);
				buttonTitle = buttonTitle.Substring(buttonTitle.IndexOf(' ')+1);
				MXTouchContainer.Navigate(uri);

			};
			UIViewElement imageElement = new UIViewElement(null, button, true);
			imageElement.Flags = UIViewElement.CellFlags.DisableSelection | UIViewElement.CellFlags.Transparent;				
			return imageElement;
		}
	}
}

