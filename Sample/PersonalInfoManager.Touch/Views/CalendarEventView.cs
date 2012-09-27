using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

using MonoCross.Touch;
using MonoTouch.Dialog;
using MonoCross.Navigation;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;
using MonoTouch.Dialog.AddOn;

namespace dotDialog.Sample.PersonalInfoManger.Touch
{
	public partial class CalendarEventView : MXTouchDialogView<CalEvent>
	{
		public CalendarEventView() : base(UITableViewStyle.Grouped, null, true) 
		{ 
			// build action sheet
			var button = new UIBarButtonItem(UIBarButtonSystemItem.Edit );
			button.Clicked += delegate(object sender, EventArgs e) { 
				MXTouchContainer.Navigate(CalendarEventController.Uri(Model.Id, ViewPerspective.Update)); 
			};
			NavigationItem.SetRightBarButtonItem(button, true);		
		}

		public override void Render ()
		{
			// verify model is not null
			if (Model == null) { throw new Exception("Model cannot be null when rendering: " + GetType()); } 
			
			Root = new RootElement("Event");
			var sections = CalendarEventDialogSections.CreateEventDetailsSection(Model);
			Root.Add(sections);

			string updateUri = CalendarEventController.Uri(Model.Id, ViewPerspective.Update);
			var editButton = GlassButtonExtension.CreateGlassButton("Edit Event");
			editButton.TouchUpInside += (sender, e) => { MXTouchContainer.Navigate(updateUri); };
			Section buttonSection = new Section() { editButton };
			Root.Add(buttonSection);
		}

		class TableViewStringDataSource : UITableViewDataSource
		{
			static NSString skey = new NSString ("TableViewStringDataSourceCell");
			public UITextAlignment Alignment = UITextAlignment.Left;

			private List<string> entries;
			
			public TableViewStringDataSource(List<string> rows) { entries = rows; }

			public override int RowsInSection(UITableView tableview, int section) 
			{
				return entries.Count;
			}

			public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
			{
				var cell = tableView.DequeueReusableCell(skey);
				if (cell == null){
					cell = new UITableViewCell(UITableViewCellStyle.Default, skey); //UITableViewCellStyle.Value1, skey);
					cell.SelectionStyle = UITableViewCellSelectionStyle.None;
				}
				cell.Accessory = UITableViewCellAccessory.None;
				if (cell.AccessoryView != null) { cell.AccessoryView = null; }
				cell.TextLabel.Text = entries[indexPath.Row];
				cell.TextLabel.Font = UIFont.SystemFontOfSize(15);
				cell.TextLabel.TextAlignment = Alignment;
				
				// The check is needed because the cell might have been recycled.
				if (cell.DetailTextLabel != null)
				{
					//cell.DetailTextLabel.Text = Value == null ? "" : Value;
					cell.TextLabel.Font = cell.DetailTextLabel.Font;
				}

				return cell;
			}

			public override int NumberOfSections(UITableView tableView) { return 1; }
			public override string TitleForHeader(UITableView tableView, int section) { return string.Empty; }		
		}
	}
}

