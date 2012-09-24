/// <summary>
/// Multiline entry element: a gist from martinbowling (Github)
/// Public Clone URL:	git://gist.github.com/315408.git
/// </summary>
using System;
using System.Drawing;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using MonoTouch.Foundation;


namespace MonoTouch.Dialog.AddOn
{
	public class MultilineEntryElement : Element, IElementSizing  
	{
		public string Value;
		static NSString ekey = new NSString ("EntryElement");
		bool isPassword;
		UITextView entry;
		string placeholder;
		static UIFont font = UIFont.BoldSystemFontOfSize (17);
		
		/// <summary>
		/// Constructs an EntryElement with the given caption, placeholder and initial value.
		/// </summary>
		/// <param name="caption">
		/// The caption to use
		/// </param>
		/// <param name="placeholder">
		/// Placeholder to display.
		/// </param>
		/// <param name="value">
		/// Initial value.
		/// </param>
		public MultilineEntryElement (string caption, string placeholder, string value) : base (caption)
		{
			Value = value;
			this.placeholder = placeholder;
		}
		
		/// <summary>
		/// Constructs  an EntryElement for password entry with the given caption, placeholder and initial value.
		/// </summary>
		/// <param name="caption">
		/// The caption to use
		/// </param>
		/// <param name="placeholder">
		/// Placeholder to display.
		/// </param>
		/// <param name="value">
		/// Initial value.
		/// </param>
		/// <param name="isPassword">
		/// True if this should be used to enter a password.
		/// </param>

		public override string Summary ()
		{
			return Value;
		}

		// 
		// Computes the X position for the entry by aligning all the entries in the Section
		//
		SizeF ComputeEntryPosition (UITableView tv, UITableViewCell cell)
		{
			Section s = Parent as Section;
			if (s.EntryAlignment.Width != 0)
				return s.EntryAlignment;
			
			SizeF max = new SizeF (-1, -1);
			foreach (var e in s.Elements){
				var ee = e as MultilineEntryElement;
				if (ee == null)
					continue;
				
				SizeF size = new SizeF(0,0);
				if (!string.IsNullOrEmpty(ee.Caption)) 
				{ 
					size = tv.StringSize (ee.Caption, font); 
				}
				else
				{ 
					var useHeight = tv.StringSize (" ", font); 
					size = new SizeF(0, useHeight.Height);
				}
				if (size.Width > max.Width)
					max = size;				
			}

			if (max.Width > 0)
			{
				s.EntryAlignment = new SizeF (25 + Math.Min (max.Width, 160), max.Height);
			}
			else { s.EntryAlignment = new SizeF(2, max.Height); }

			return s.EntryAlignment;
		}
		
		public override UITableViewCell GetCell (UITableView tv)
		{
			var cell = tv.DequeueReusableCell (ekey);
			if (cell == null){
				cell = new UITableViewCell (UITableViewCellStyle.Default, ekey);
				cell.SelectionStyle = UITableViewCellSelectionStyle.None;
			} else 
				RemoveTag (cell, 1);
			
			
			if (entry == null){
				SizeF size = ComputeEntryPosition (tv, cell);
				/*entry = new UITextField (new RectangleF (size.Width, (cell.ContentView.Bounds.Height-size.Height)/2-1, 320-size.Width, size.Height)){
					Tag = 1,
					Placeholder = placeholder,
					SecureTextEntry = isPassword
				};*/
				entry = new UITextView(new RectangleF(size.Width,(cell.ContentView.Bounds.Height-size.Height)/2-1, 320-size.Width, 96));
				entry.Text = Value ?? "";
				entry.AutoresizingMask = UIViewAutoresizing.FlexibleWidth |
					UIViewAutoresizing.FlexibleLeftMargin;
				
				entry.Ended += delegate {
					Value = entry.Text;
				};
				entry.ReturnKeyType = UIReturnKeyType.Done;
				entry.Changed += delegate(object sender, EventArgs e) {
					int i = entry.Text.IndexOf("\n", entry.Text.Length -1);
					if (i > -1)
					{
						entry.Text = entry.Text.Substring(0,entry.Text.Length -1); 
						entry.ResignFirstResponder();	
					}
				};
			}

			
			cell.TextLabel.Text = Caption;
			cell.ContentView.AddSubview (entry);
			return cell;
		}
		
		protected override void Dispose (bool disposing)
		{
			if (disposing){
				entry.Dispose ();
				entry = null;
			}
		}
		
		public float GetHeight (UITableView tableView, NSIndexPath indexPath)
		{
			return 112;
		}
	}


	public class MultiLineEntrySubTextItem : Element, IElementSizing  {
		public string Value;
		public int Rows;
		static NSString ekey = new NSString ("MultiLineEntryElement");
		UITextView entry;
		static UIFont font = UIFont.BoldSystemFontOfSize (17);
		private bool _bottomItem;

		/// <summary>
		/// Constructs an EntryElement with the given caption, placeholder and initial value.
		/// </summary>
		/// <param name="caption">
		/// The caption to use
		/// </param>
		/// <param name="placeholder">
		/// Placeholder to display.
		/// </param>
		/// <param name="value">
		/// Initial value.
		/// </param>
		public MultiLineEntrySubTextItem(string caption, string value, bool bottomItem)
			: base(caption)
		{
			Value = value;
			_bottomItem = bottomItem;
		}

		public override string Summary () { return Value; }
		
		// 
		// Computes the X position for the entry by aligning all the entries in the Section
		//
		SizeF ComputeEntryPosition (UITableView tv, UITableViewCell cell)
		{
			Section s = Parent as Section;
			if (s.EntryAlignment.Width != 0)
				return s.EntryAlignment;
			
			SizeF max = new SizeF (-1, -1);
			foreach (var e in s.Elements){
				var ee = e as MultiLineEntrySubTextItem;
				if (ee == null)
					continue;
				
				var size = tv.StringSize (ee.Caption, font);
				if (size.Width > max.Width)
					max = size;				
			}
			s.EntryAlignment = new SizeF (25 + Math.Min (max.Width, 160), max.Height);
			return s.EntryAlignment;
		}
		
		public override UITableViewCell GetCell (UITableView tv)
		{
			var cell = tv.DequeueReusableCell (ekey);
			if (cell == null){
				cell = new TableViewCell(UITableViewCellStyle.Default, ekey);
				cell.SelectionStyle = UITableViewCellSelectionStyle.None;
				cell.BackgroundColor = UIColor.White;
			} else 
				RemoveTag (cell, 1);
			
			
			if (entry == null)
			{
				var s = new RectangleF(0, string.IsNullOrEmpty(Caption) ? 10.5f : 32,
				                       cell.ContentView.Bounds.Width, 12 + (20 * (Rows + 1)));
				entry = new UITextView(s);
				
				entry.Font = UIFont.SystemFontOfSize(17);
				entry.Text = Value ?? string.Empty;
				
				entry.AutoresizingMask = UIViewAutoresizing.FlexibleWidth |UIViewAutoresizing.FlexibleLeftMargin;

				//TODO: Find a way to just curver bottom
				//if (_topItem) { entry.Layer.CornerRadius = 15.0f; }
				if (_bottomItem) { entry.Layer.CornerRadius = 15.0f; }

				entry.Ended += delegate {
					if (entry != null)
					{
						Value = entry.Text;
						
						entry.ResignFirstResponder();
						if (entry.ReturnKeyType == UIReturnKeyType.Go)
						{
							//TODO: Do we submit the form here
							throw new ApplicationException("We need to submit form here");
						}
					}
				};
				entry.Changed += delegate(object sender, EventArgs e) {
					if(!string.IsNullOrEmpty(entry.Text))
					{
						int i = entry.Text.IndexOf("\n", entry.Text.Length -1);
						if (i > -1)
						{
							entry.Text = entry.Text.Substring(0,entry.Text.Length -1); 
							entry.ResignFirstResponder();	
						}
					}
					Value = entry.Text;
				};
				
				entry.ReturnKeyType = UIReturnKeyType.Done;
			}

			cell.TextLabel.Text = Caption; 
			cell.ContentView.AddSubview(entry);
			return cell;
		}
		
		protected override void Dispose (bool disposing)
		{
			if (disposing && entry != null)
			{
				entry.RemoveFromSuperview();
				entry.Dispose ();
				entry = null;
			}
		}
		
		public float GetHeight (UITableView tableView, NSIndexPath indexPath)
		{
			return 44 + (20 * (Rows + 1));
		}
	}
		
#if (ANDROID)
		[Android.Runtime.Preserve( AllMembers = true )]
#elif (TOUCH)
		[MonoTouch.Foundation.Preserve (AllMembers = true)]
#endif
	public class TableViewCell : UITableViewCell //, IUrlImageUpdated<string>
	{
		const float HorizontalPadding = 10;
		const float VerticalPadding = 5;
		const float Margin = 10;
		const float SmallMargin = 6;
		const float Spacing = 8;
		const float ControlPadding = 8;
		const float DefaultTextViewLines = 5;
		const float MoreButtonMargin = 40;
		const float DefaultRowHeight = 44;
		
		private static UIColor AppleBlue = UIColor.FromRGB(81, 102, 145);

		private UILabel BesideLabel = null;
		private UILabel MessageLabel = null;
		private UILabel TopLabel = null;

		public float Height { get; set; }
		public string BesideText { get; set; }
		public string MessageText { get; set; }
		public string TopText { get; set; }
		public string ShopStarImage { get; set; }
		
		public delegate void ImageLoadedDelegate(string id);
		public event ImageLoadedDelegate ImageLoaded;
		
		public TableViewCell() : base() { Height = 44; }
		
		public TableViewCell(IntPtr handle) : base(handle)
		{
//				Height = 44;
//				CellStyle = TableViewCellStyle.Default;
		}
		
		public TableViewCell(NSCoder coder) : base(coder) { Height = 44; }
		
		public TableViewCell(NSObjectFlag t) : base(t) { Height = 44; }
		
		public TableViewCell(UITableViewCellStyle style, string reuseIdentifier) : base(style, reuseIdentifier) { Height = 44; }
		
		public override void LayoutSubviews()
		{
			base.LayoutSubviews();

			float labelWidth = ContentView.Frame.Width - (HorizontalPadding * 2);
			
			TextLabel.Font = UIFont.BoldSystemFontOfSize(17);
			TextLabel.LineBreakMode = UILineBreakMode.TailTruncation;
			TextLabel.Lines = 1;
			TextLabel.SizeToFit();
			TextLabel.Frame = new RectangleF(HorizontalPadding, VerticalPadding, labelWidth, 21);

			//TODO: Find away to resize based on UITextView
			if (ContentView != null) 
				Frame = new RectangleF(Frame.X, Frame.Y, Frame.Width, ContentView.Frame.Bottom);

			SetNeedsDisplay();
			// This does not appear to be necessary unless the UITableView is in grouped mode (?)
			//			BackgroundColor = appStyle.LayerItemBackgroundColor.UIColor();
			
		}
		
		public override void Draw (RectangleF rect)
		{
			base.Draw (rect);
			
			// for some reason, the ContentView frame returns an incorrect width value when the label
			// is created so the label's frame must be updated here where the ContentView frame is correct
			if (BesideLabel != null)
			{
				BesideLabel.Frame = new RectangleF(BesideLabel.Frame.X, BesideLabel.Frame.Y,
				                                   ContentView.Frame.Width - (2 * HorizontalPadding),
				                                   BesideLabel.Frame.Height);
			}
			
//				if (layer is FormLayer || CellStyle == TableViewCellStyle.Message)
//					return;
			
			long i;
			SizeF numberSize;
			UILabel label;
			if (BesideText != null && long.TryParse(BesideText, out i))
			{
				label = BesideLabel;
			}
//			else if (uiStyle == UITableViewCellStyle.Value1 && DetailTextLabel != null)
//			{
//				if (DetailTextLabel.Text != null && long.TryParse(DetailTextLabel.Text, out i))
//				{
//					DetailTextLabel.BackgroundColor = UIColor.Clear;
//					label = DetailTextLabel;
//				}
//				else
//					return;
//			}
			else
				return;
			
			label.TextColor = UIColor.White;
			numberSize = StringSize(label.Text, UIFont.SystemFontOfSize(17));
			
			float width = label.Frame.Right - numberSize.Width - (HorizontalPadding - ContentView.Frame.Left);
			RectangleF bounds = new RectangleF(width + 4, ContentView.Frame.Y, numberSize.Width - 10, 22);
			
			MonoTouch.CoreGraphics.CGContext context = UIGraphics.GetCurrentContext();
			float radius = bounds.Size.Height / 2.0f;
			
			context.SaveState();
			
			UIColor color;
			if (Highlighted || Selected)
				color = UIColor.White;
			else
				color = UIColor.FromRGBA(0.530f, 0.600f, 0.738f, 1.000f);
			
			context.SetFillColorWithColor(color.CGColor);
			context.BeginPath();
			context.AddArc(bounds.X + radius, radius + bounds.Height / 2, radius, (float)(Math.PI / 2), (float)(3 * Math.PI / 2), false);
			context.AddArc(radius + bounds.X + bounds.Width, radius + bounds.Height / 2, radius, (float)(3 * Math.PI / 2), (float)(Math.PI / 2), false);
			context.ClosePath();
			context.FillPath();
			context.RestoreState();
			
			context.SetBlendMode(MonoTouch.CoreGraphics.CGBlendMode.Clear);
		}
		
		public override void SetHighlighted (bool highlighted, bool animated)
		{
			base.SetHighlighted (highlighted, animated);
			
			long i;
			if (BesideLabel != null && !long.TryParse(BesideText, out i))
				BesideLabel.TextColor = highlighted || Selected ? UIColor.White : AppleBlue;
			
			if (MessageLabel != null)
				MessageLabel.TextColor = highlighted || Selected ? UIColor.White : UIColor.Orange;
			
			if (TopLabel != null)
				TopLabel.TextColor = highlighted || Selected ? UIColor.White : UIColor.Purple;
			
			SetNeedsDisplay();
		}
		
//			public void Reset(iLayer layer)
//			{
//				this.layer = layer;
//				
//				Height = 44;
//				CellStyle = TableViewCellStyle.Default;
//				Accessory = UITableViewCellAccessory.None;
//				ImageView.Image = null;
//				
//				if (TopLabel != null)
//				{
//					TopLabel.RemoveFromSuperview();
//				}
//				
//				if (MessageLabel != null)
//				{
//					MessageLabel.RemoveFromSuperview();
//				}
//				
//				if (BesideLabel != null)
//				{
//					BesideLabel.RemoveFromSuperview();
//				}
//				
//				TextLabel.Text = null;
//				if (DetailTextLabel != null)
//					DetailTextLabel.Text = null;
//			}
		
		public void UrlImageUpdated(string id)
		{
			if (ImageLoaded != null)
				ImageLoaded(id);
		}
		
		protected override void Dispose (bool disposing)
		{
			if (disposing)
			{
				if (TopLabel != null)
				{
					TopLabel.RemoveFromSuperview();
					TopLabel.Dispose();
					TopLabel = null;
				}
				
				if (MessageLabel != null)
				{
					MessageLabel.RemoveFromSuperview();
					MessageLabel.Dispose();
					MessageLabel = null;
				}
				
				if (BesideLabel != null)
				{
					BesideLabel.RemoveFromSuperview();
					BesideLabel.Dispose();
					BesideLabel = null;
				}
			}
			
			base.Dispose (disposing);
		}
	}
}