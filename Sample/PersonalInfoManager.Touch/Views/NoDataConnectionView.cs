using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoCross.Touch;
using System.Collections.Generic;
using MonoTouch.CoreGraphics;

namespace dotDialog.Sample.PersonalInfoManger.Touch
{
	public abstract partial class NoDataConnectionView<T> : MXTouchViewController<T>
	{
		public NoDataConnectionView(string title, string message, float transparency)
		{
			Title = title;
			
			var rect = new RectangleF(0,0, 320, 366);
			var v = new UIView(rect);
			v.BackgroundColor = UIColor.LightGray;
			
			float height = 60;
			float width = 200;
			float centeringHeightBuffer = 20;
			var labelSize = new SizeF(width, height);
			var labelLoc = new PointF((rect.Width - width)/2, (rect.Height - height)/2 - centeringHeightBuffer);
			var label = new UILabel(new RectangleF(labelLoc, labelSize));
			label.Text = message;
			label.Layer.CornerRadius = 8f;
			label.Layer.BorderWidth = 1.0f;
			label.Layer.BorderColor = new CGColor(0.5f, 1.0f);
			label.TextAlignment = UITextAlignment.Center;
			label.AdjustsFontSizeToFitWidth = true;
			label.BaselineAdjustment = UIBaselineAdjustment.AlignCenters;
			label.LineBreakMode = UILineBreakMode.WordWrap;
			label.Lines = 2;
			label.BackgroundColor = UIColor.FromRGBA(0xFF,0xFF, 0xFF, transparency);
			
			v.Add(label);
			
			this.Add(v);
		}
		public NoDataConnectionView() : this("No Data", "No Data Connection", 0.85f) { }
		public NoDataConnectionView(string message, float transparency) : this("No Data", message, transparency) { }

		public override void Render () { }
	}
}

