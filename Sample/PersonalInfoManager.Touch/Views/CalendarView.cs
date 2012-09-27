using System;
using System.Drawing;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;

using MonoCross.Touch;
using MonoCross.Navigation;

namespace dotDialog.Sample.PersonalInfoManger.Touch
{
	public partial class CalendarView : MXTouchViewController<CalendarListModel>
	{
		public override void Render() 
		{ 
			_events = Model.Events;

			BuildAgendaTableView();
		}

		public CalendarView() : base() 
		{ 
			Title = "Calendar"; 
			View.BackgroundColor = UIColor.FromRGB(0,63,107);

			float containingViewWidth = View.Frame.Width;
			float defaultHeight = View.Frame.Height;

			_tableView = new UITableView();
			_tableView.Frame = new RectangleF(0, 0, containingViewWidth, defaultHeight);
			Add(_tableView);

			var button = new UIBarButtonItem(UIBarButtonSystemItem.Add);
			button.Clicked += delegate(object sender, EventArgs e) {
					MXTouchContainer.Navigate(CalendarEventController.UriForNew());
			};
			NavigationItem.SetRightBarButtonItem(button, true);
		}

		private bool BuildAgendaTableView()
		{
			IEnumerable<CalEvent> eventsList = new List<CalEvent>();
			if (_events != null)
			{
				eventsList = from e in _events
					            orderby e.StartTime ascending
					            select e;
			}
			List<CalEvent> events = new List<CalEvent>(eventsList);
			DateTime prevDate = new DateTime(0);
			List<List<CalEvent>> sectionedEventLists = new List<List<CalEvent>>();
			foreach(CalEvent e in events)
			{
				if (e.StartTime.Date != prevDate.Date)
				{
					var newList = new List<CalEvent>();
					newList.Add(e);
					sectionedEventLists.Add(newList);
				}
				else 
				{
					int loc = 0;
					if (sectionedEventLists.Count > 0) { loc = sectionedEventLists.Count-1; }
					else
					{
						var newList = new List<CalEvent>();
						sectionedEventLists.Add(newList);
					}
					sectionedEventLists[loc].Add(e);
				}
				prevDate = e.StartTime;
			}
			_tableView.DataSource = new TableViewDataSource(sectionedEventLists);
			_tableView.Delegate = new TableViewDelegate(sectionedEventLists);
			return true;
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			_tableView.ReloadData();
		}

		UITableView _tableView;
		List<CalEvent> _events;

		class TableViewDataSource : UITableViewDataSource
		{
			public List<List<CalEvent>> Sections;
			private List<string> _sectionIndexs;
			
			public TableViewDataSource(List<List<CalEvent>> sections) 
			{ 
				Sections = sections; 
				_sectionIndexs = new List<string>();

				foreach(List<CalEvent> c in sections)
				{
					if (c.Count > 0)
					{
						_sectionIndexs.Add(c[0].StartTime.ToString("d"));
					}
					else
					{
						string msg = "A List<CalEvent> in the collection had no items";
						Console.WriteLine(msg);
						throw new ArgumentOutOfRangeException("sections", "A List<CalEvent> in the collection had no items");
					}
				}
			}
			
			public override int NumberOfSections(UITableView tableView)
			{
				return Sections.Count;
			}
			
			public override int RowsInSection(UITableView tableview, int section) 
			{
				int totalCount = 0;
				if (Sections != null && Sections[section] != null)
				{
					totalCount = Sections[section].Count;
				}
				return totalCount;
			}

			public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
			{
				var c = Sections[indexPath.Section][indexPath.Row];
				string subj;
				subj = c.Subject;
				
				string startTime = c.StartTime.ToString("t");

				var cell = tableView.DequeueReusableCell(skey);
				if (cell == null)
				{
					cell = new TableViewCell(subj, startTime, UITableViewCellStyle.Subtitle, skey);
				}
				else
				{
					cell.TextLabel.Text = subj;
					cell.DetailTextLabel.Text = startTime;
					if (cell.AccessoryView != null) { cell.AccessoryView = null; }
				}
				if (string.IsNullOrEmpty(subj))
				{
					//TODO: Create a better "can't build data view
					cell.TextLabel.Text = "Error loading item in list";
					cell.TextLabel.TextColor = UIColor.Red;
					cell.TextLabel.Font = UIFont.FromName("CourierNewPS-ItalicMT", 12.0f);
				}
				return cell;
			}
			
			public override string TitleForHeader(UITableView tableView, int section)
			{
				return _sectionIndexs[section];
			}

			static NSString skey = new NSString ("CalendarListCell");
		}
		
		class TableViewDelegate : UITableViewDelegate
	    {
			public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
	        {
				string uri = CalendarEventController.Uri(_list[indexPath.Section][indexPath.Row].Id);

				UIActivityIndicatorView spinner = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.Gray);
				spinner.StartAnimating();
				tableView.CellAt(indexPath).AccessoryView = spinner;				
				
				new System.Threading.Thread (() => {
					using (new MonoTouch.Foundation.NSAutoreleasePool()) 
					{
						MXTouchContainer.Navigate(uri);
					}
				}).Start ();
	        }

			public TableViewDelegate(List<List<CalEvent>>  list) {  _list = list; }
			private List<List<CalEvent>>  _list;
	    }
	}
}

