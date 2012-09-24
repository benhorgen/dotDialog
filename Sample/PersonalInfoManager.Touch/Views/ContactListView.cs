using System;
using System.Linq;
using System.Drawing;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;

using MonoCross.Touch;
using MonoCross.Navigation;

namespace dotDialog.Sample.PersonalInfoManger.Touch
{
	public partial class ContactListView : MXTouchViewController<ContactListModel>
	{
		public ContactListView() : base() 
		{ 
			Title = ViewTitle;

			// determin table size based on screen space remaining
			var frame = this.View.Frame;				
			float heightAvailable = frame.Height;
			
			// setup table and it's view properties
			var tableSize = new SizeF(frame.Width, heightAvailable);
			var tableFrame = new RectangleF(new PointF(0,0), tableSize);
			_tableView = new UITableView(tableFrame, UITableViewStyle.Plain);
			_tableView.ScrollEnabled = true;
			_tableView.BackgroundColor = UIColor.White;
			_tableView.SeparatorColor = UIColor.FromRGB(0, 63, 107);
			this.Add(_tableView);
			
			// setup search bar
			_searchBar = new UISearchBar();
			_searchBar.AutocorrectionType = UITextAutocorrectionType.No;
			_searchBar.SizeToFit();
			_searchBar.CancelButtonClicked += (sender, e) => {
				_contactSections.Clear();
				_contactSections.AddRange(_sectionedContactList);
				_tableView.ReloadData();
			};
			
			_sdc = new UISearchDisplayController(_searchBar, this);

			var button = new UIBarButtonItem(UIBarButtonSystemItem.Add);
			button.Clicked += delegate(object sender, EventArgs e) {
					MXTouchContainer.Navigate(ContactController.UriForNew());
			};
			NavigationItem.SetRightBarButtonItem(button, true);		
		}
		
		public override void Render() 
		{

			if (_contactSections == null) { _contactSections = new List<List<Contact>>(_chars.Length); }
			else { _contactSections.Clear(); }
			
			for(int i = 0; i < _chars.Length; i++)
			{
				char chr = _chars[i];
				var contacts = (from c in Model.Contacts where c.LastName.ToUpper()[0] == chr select c).ToList();
				_contactSections.Add(new List<Contact>(contacts));
			}
			_sectionedContactList = _contactSections.ToArray();
			
			
			_tableView.DataSource = new TableViewDataSource(_contactSections, _chars);
			_tableView.Delegate = new TableViewDelegate(_contactSections);
			_searchBar.Delegate = new SearchBarDelegate(this, _contactSections);
		}
				
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear(animated);
			
			if (_tableView != null) 
			{  
				_sdc.SearchResultsDataSource = _tableView.DataSource;
				_tableView.TableHeaderView = _searchBar;
				_tableView.ReloadData(); 
			}
			else
			{
				string msg = "Unexpected State of ContactListView... _table is null";
				throw new ApplicationException(msg);
			}
		}

		UITableView _tableView = null;
		List<List<Contact>> _contactSections;
		List<Contact>[] _sectionedContactList;
		UISearchBar _searchBar = null;
		UISearchDisplayController _sdc = null;
		internal const string ViewTitle = "Contacts";
		char[] _chars = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 
			'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T',
			'U', 'V', 'W', 'X', 'Y', 'Z' 
		};

		#region Supporting Table and Search View Delegates
		class TableViewDataSource : UITableViewDataSource
		{
			public List<List<Contact>> Sections;
			private List<string> _sectionIndexs;
			
			public TableViewDataSource(List<List<Contact>> sections, char[] sectionIndexs) 
			{ 
				Sections = sections; 

				_sectionIndexs = new List<string>();
				foreach(char c in sectionIndexs)
				{
					_sectionIndexs.Add(c.ToString());
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
				string name;
				if (!string.IsNullOrEmpty(c.FirstName)) name = c.LastName + ", " + c.FirstName;
				else { name = c.LastName; }
				
				string phone = string.IsNullOrEmpty(c.Phone) ? "No Phone" : c.Phone;

				var cell = tableView.DequeueReusableCell(skey);
				if (cell == null)
				{
					cell = new TableViewCell(name, phone, UITableViewCellStyle.Subtitle, skey);
				}
				else
				{
					cell.TextLabel.Text = name;
					cell.DetailTextLabel.Text = phone;
					if (cell.AccessoryView != null) { cell.AccessoryView = null; }
				}

				if (string.IsNullOrEmpty(name))
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
		
			public override string[] SectionIndexTitles(UITableView tableView) 
			{ 
				return _sectionIndexs.ToArray(); 
			}

			static NSString skey = new NSString ("ContactListCell");
		}
		
		class TableViewDelegate : UITableViewDelegate
	    {
	        private List<List<Contact>> _list;
	        public TableViewDelegate(List<List<Contact>> list) {  _list = list; }
	        public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
	        {
				string uri = ContactController.Uri(_list[indexPath.Section][indexPath.Row].Id);

				UIActivityIndicatorView spinner = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.Gray);
				spinner.StartAnimating();
				tableView.CellAt(indexPath).AccessoryView = spinner;				
				
				new System.Threading.Thread (() => 
				                             {
					using (new MonoTouch.Foundation.NSAutoreleasePool()) 
					{
						MXTouchContainer.Navigate(uri);
					}
				}).Start ();
	        }
	    }		
		
		class SearchBarDelegate : UISearchBarDelegate
		{
			List<Contact>[] _tableDataSourceOriginalList;
			List<List<Contact>> _tableDataSource;
			
			public SearchBarDelegate(ContactListView view, List<List<Contact>> model) : base() 
			{ 
				_tableDataSource = model;
				_tableDataSourceOriginalList = _tableDataSource.ToArray();
			}

			public override void TextChanged (UISearchBar searchBar, string searchText)
			{
				List<List<Contact>> searchResults = new List<List<Contact>>();
				
				if (searchText.Length > 0)
				{
					List<Contact>[] _model = _tableDataSourceOriginalList;
					for(int i = 0; i < _model.Length; i++)
					{
						if (_model[i] != null)
						{
							var results = (from c in _model[i] 
							               where c.FirstName.ToLower().Contains(searchText.ToLower()) ||
									             c.LastName.ToLower().Contains(searchText.ToLower())
							               select c).ToList();
							searchResults.Add(results);	
						}
						else { searchResults.Add(new List<Contact>()); /*insert empty section */ }
					}					
				}
				else 
				{
					// reset to original list
					searchResults.Clear();
					searchResults.AddRange(_tableDataSourceOriginalList);
				}
				
				_tableDataSource.Clear();
				_tableDataSource.AddRange(searchResults);
			}			
		}			
		#endregion
	}
	
	internal class NoConnectionContactListView : NoDataConnectionView<ContactListModel> 
	{
		public NoConnectionContactListView(string title, string msg, float transparency) : base(title, msg, transparency) 
		{ 
			var button = new UIBarButtonItem(UIBarButtonSystemItem.Add);
			button.Clicked += delegate(object sender, EventArgs e) {
				MXTouchContainer.Navigate("Contact/"+"Create/"+"0");
			};
			NavigationItem.SetRightBarButtonItem(button, true);		
		}

		public override void Render ()
		{
			new System.Threading.Thread (() => {
				Thread.Sleep(10000); //sleep a few seconds & try again
				MXTouchContainer.Navigate(ContactListController.Uri);
			}).Start();
		}

		void HideViewDelegate()
		{
			Debug.WriteLine("HideView delegate called for " + GetType());

			InvokeOnMainThread( () => 
            {
				if (NavigationController != null)
				{
					List<UIViewController> allVCs = new List<UIViewController>(NavigationController.ViewControllers);
					allVCs.Remove(this);
					NavigationController.SetViewControllers(allVCs.ToArray(), false);
				}
			});

			new System.Threading.Thread (() => {
				using (new MonoTouch.Foundation.NSAutoreleasePool()) 
				{
					Thread.Sleep(10);
					MXTouchContainer.Navigate(ContactListController.Uri);
				}
			}).Start();
		}
	}	
}

