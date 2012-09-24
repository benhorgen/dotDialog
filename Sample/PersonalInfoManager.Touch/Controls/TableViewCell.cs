using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Collections.Generic;

namespace dotDialog.Sample.PersonalInfoManger.Touch
{
	public class TableViewCell : UITableViewCell
	{
        public TableViewCell(string text, string detailText, UITableViewCellStyle style, string reuseId) : base(style, reuseId)
		{				
			TextLabel.Text = text;
			TextLabel.TextColor = UIColor.FromRGB(0, 63, 107);
			TextLabel.Font = UIFont.FromName("Georgia-Bold", 18.0f);
			TextLabel.AdjustsFontSizeToFitWidth = true;
			TextLabel.MinimumFontSize = 14.0f;
			TextLabel.LineBreakMode = UILineBreakMode.WordWrap;
			TextLabel.Lines = 1;
			TextLabel.SizeToFit();
				
			if (DetailTextLabel != null) 
			{ 
				DetailTextLabel.Text = detailText; 
				DetailTextLabel.TextColor = UIColor.DarkGray;
				DetailTextLabel.Font = UIFont.FromName("Georgia-Bold", 14.0f);
				DetailTextLabel.AdjustsFontSizeToFitWidth = true;
                DetailTextLabel.MinimumFontSize = 6.0f;
				DetailTextLabel.LineBreakMode = UILineBreakMode.WordWrap;
				DetailTextLabel.Lines = 0;
				DetailTextLabel.SizeToFit();
			}
			
            BackgroundColor = UIColor.White;
			Accessory = UITableViewCellAccessory.None;
			SelectionStyle = UITableViewCellSelectionStyle.None;
        }

		public static UITableViewCell CreateErrorCell(NSString key)
		{
			var cell = new TableViewCell(string.Empty, null, UITableViewCellStyle.Default, key);
			cell.ConvertToErrorCell();
			return cell;
		}

		public const float CellHeight = 44;
	}

	public static class TableViewCellExtensions
	{
		public static void ConvertToErrorCell(this UITableViewCell cell)
		{
			//TODO: Create a better "can't build data view
			cell.TextLabel.Text = "Error loading item in list";
			cell.TextLabel.TextColor = UIColor.Red;
			cell.TextLabel.Font = UIFont.FromName("CourierNewPS-ItalicMT", 12.0f);
		}
	}

	public class ChevronDataSource : UITableViewDataSource
	{
		public ChevronDataSource(List<string> rows, NSString cellKey) : base() 
		{
			_rows = rows; 
			_skey = cellKey;
		}

		public override int RowsInSection (UITableView tableView, int section) { return _rows.Count; }

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			string option = _rows[indexPath.Row];

			var cell = tableView.DequeueReusableCell(_skey);
			if (cell == null) {
				cell = new TableViewCell(option, null, UITableViewCellStyle.Default, _skey);
				cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
			}
			else
			{
				cell.TextLabel.Text = option;
				if (cell.AccessoryView != null) 
				{ 
					cell.AccessoryView = null; 
					cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
				}
			}

			if (string.IsNullOrEmpty(option)) { cell.ConvertToErrorCell(); }
			return cell;
		}

		private List<string> _rows;

		private NSString _skey;
	}

	public class SubTextDataSource<T> : UITableViewDataSource where T : IDataSourceRow
	{
		public SubTextDataSource(List<T> rows, NSString cellKey) : base() 
		{
			_rows = rows;
			_skey = cellKey;
		}
		
		public override int RowsInSection (UITableView tableView, int section) { return _rows.Count; }
		
		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var text = _rows[indexPath.Row].Text;
			var subtext = _rows[indexPath.Row].SubText;

			var cell = tableView.DequeueReusableCell(_skey);
			if (cell == null) 
			{
				cell = new TableViewCell(text, subtext, UITableViewCellStyle.Subtitle, _skey);
				cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
			}
			else
			{
				cell.TextLabel.Text = text;
				cell.DetailTextLabel.Text = subtext;
				if (cell.AccessoryView != null) 
				{ 
					cell.AccessoryView = null; 
					cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
				}
			}
			
			if (string.IsNullOrEmpty(text)) { cell.ConvertToErrorCell(); }
			return cell;
		}
		
		private List<T> _rows;
		private NSString _skey;
	}

	public interface IDataSourceRow
	{
		string Text { get; set; }
		string SubText { get; set; }
	}
}

