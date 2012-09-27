using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoCross.Touch;
using MonoTouch.CoreGraphics;
using System.Collections.Generic;

namespace dotDialog.Sample.PersonalInfoManger.Touch
{
	public partial class TaskListView : MXTouchViewController<List<Task>>
	{
		public TaskListView() : base() 
		{ 
			Title = "Tasks";

			scrollView = new UIScrollView();
			const int defaultTabBarHeight = 49;
			float containingViewHeight = View.SizeThatFits(new SizeF(320, 480)).Height;
			float height = containingViewHeight - defaultTabBarHeight;
			scrollView.Frame = new RectangleF(new PointF(0, 0), new SizeF(320, height));
			scrollView.BackgroundColor = UIColor.Clear;
			Add(scrollView);

			var rect = new RectangleF(0,0,0,0);
			Model = new List<Task>();
			scrollView.Add(BuildTaskList(rect, width));		

			var button = new UIBarButtonItem(UIBarButtonSystemItem.Add);
			button.Clicked += delegate(object sender, EventArgs e) { MXTouchContainer.Navigate(TaskController.UriForNew()); };
			NavigationItem.SetRightBarButtonItem(button, true);
		}
		
		public override void Render () 
		{
			// calc correct location
			int row = 0;
			if (Model != null) row = Model.Count;
			_table.Frame = CalcTableFrame(row, width, headerHeight);

			_table.Source = new TaskTableViewSource(Model);
			_table.Delegate = new TableViewDelegate(Model);
			_table.ReloadData();
		}
		UITableView _table;
		UIScrollView scrollView;
		const float width = 300;
		const float headerHeight = 25;
		UIColor _defaultLabelTextColor = UIColor.FromRGB(0, 63, 107);

		private static RectangleF CalcTableFrame(int rows, float width, float headerHeight)
		{
			if (rows <= 0) rows = 1;
			float height = (rows * TableViewCell.CellHeight) + headerHeight;
			var viewSize = new SizeF(width, height - 1); //to hide line

			float xLoc = 10;
			float yLoc = 10;
			var frame = new RectangleF(new PointF(xLoc, yLoc), viewSize);

			return frame;
		}

		private UIView BuildTaskList(RectangleF rect, float width)
		{
			_table = new UITableView(rect, UITableViewStyle.Plain);
			_table.Source = new TaskTableViewSource(Model);
			_table.ScrollEnabled = false;
			_table.BackgroundColor = UIColor.Clear;
			_table.SeparatorColor = UIColor.FromRGB(0, 63, 107);
			_table.SeparatorStyle = UITableViewCellSeparatorStyle.SingleLine;
			_table.Layer.CornerRadius = 8f;
			_table.BackgroundColor = UIColor.White;
			_table.Layer.BorderWidth = 1.0f;
			_table.Layer.BorderColor = new CGColor(0.5f, 1.0f);
			
			UIView cvHeader = new UILabel(new RectangleF(0,0,width, headerHeight));
			cvHeader.BackgroundColor = UIColor.Clear;
			
			float cornersHeight = cvHeader.Frame.Height / 2;
			UIColor backgroundColor = UIColor.LightGray;
			UIView straightCorners = new UILabel(new RectangleF(0,cornersHeight,cvHeader.Frame.Width, cornersHeight));
			straightCorners.BackgroundColor = backgroundColor;
			cvHeader.Add(straightCorners);			
			
			var headerLabel = new UILabel(new RectangleF(0,0,width, headerHeight));
			headerLabel.Text = "  Current";
			headerLabel.TextColor = _defaultLabelTextColor;
			headerLabel.TextAlignment = UITextAlignment.Left;
			headerLabel.BackgroundColor = backgroundColor;
			headerLabel.Layer.CornerRadius = 8f;
			cvHeader.Add(headerLabel);
			
			_table.TableHeaderView = cvHeader;
						
			return _table;
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			// set context size for scroll view
			if (_table != null) 
			{
				_table.ReloadData();
				var adjustedSize = _table.Frame.Size;
				adjustedSize.Height = adjustedSize.Height + 120;
				scrollView.ContentSize = adjustedSize;
			}
			else
			{
				string msg = "Unexpected State of TaskListView... _table is null";
				Console.Write(msg);
				throw new ApplicationException(msg);
			}
		}

		private class TaskTableViewSource : UITableViewSource
		{
			List<Task> _rows;
			
			public TaskTableViewSource(List<Task> rows) { _rows = rows; }
			
			public override int RowsInSection(UITableView tableview, int section) 
			{
				if (_rows.Count <= 0) return 1; //for "no tasks" cell
				return _rows.Count; 
			}

			public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
			{
				Task row = null;
				string subtext = null;
				string text = null;

				UITableViewCell cell = null;
				if (_rows == null || _rows.Count < 1)
				{
					cell = tableView.DequeueReusableCell(skey_NoTask);
					if (null == cell)
					{
						cell = new TableViewCell("No Tasks exist", null, UITableViewCellStyle.Subtitle, skey_NoTask);
						cell.TextLabel.TextAlignment = UITextAlignment.Center;
						cell.TextLabel.Font = UIFont.FromName("Georgia-Italic", 16.0f);
						cell.TextLabel.AdjustsFontSizeToFitWidth = true;
						cell.AccessoryView  = null;
						cell.Accessory = UITableViewCellAccessory.None;
					}
				}
				else if (_rows.Count > indexPath.Row && _rows[indexPath.Row] != null)
				{
					row = _rows[indexPath.Row];
					text = row.Description;
					subtext = row.Date.ToString("d");

					cell = tableView.DequeueReusableCell(skey);
					if (cell != null)
					{
						cell.TextLabel.Text = text;
						cell.DetailTextLabel.Text = subtext;
						cell.AccessoryView  = null;
						cell.Accessory = UITableViewCellAccessory.DisclosureIndicator; 
					}
				}

				// build a new default cell if none exists
				if (cell == null)
				{
					cell = new TableViewCell(text ?? "Error in the data", subtext, UITableViewCellStyle.Subtitle, skey);
					cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
				}
				return cell;
			}

			static NSString skey = new NSString ("TaskListTableCell");
			static NSString skey_NoTask = new NSString ("NoTaskTableCell");
		}

		class TableViewDelegate : UITableViewDelegate
	    {
			public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
	        {
				if (_rows.Count > 0)
				{
		            UIActivityIndicatorView spinner = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.Gray);
		            spinner.StartAnimating();
		            tableView.CellAt(indexPath).AccessoryView = spinner;				

					// TODO:  Replace under contruction with the correct ones
					string id = _rows[indexPath.Row].Id.ToString();
					string uri = TaskController.Uri(id);

					new System.Threading.Thread (() => 
					{
						using (new MonoTouch.Foundation.NSAutoreleasePool()) 
						{
							MXTouchContainer.Navigate(uri);
						}
					}).Start ();				
				}
			}
	        
	        public TableViewDelegate(List<Task> rows) { _rows = rows; }	        
			private List<Task> _rows;
	    }
	}
}

