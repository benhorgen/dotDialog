using System;
using System.Drawing;
using MonoTouch.UIKit;

namespace MonoTouch.Dialog.AddOn
{
	public static class GlassButtonExtension
	{
		public static Element CreateGlassButtonElement(string buttonLabelText, string uri)
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
				//TODO:  Find a away to pass in an TouchUpInside delegate instead of a Uri
			};
			UIViewElement imageElement = new UIViewElement(null, button, true);
			imageElement.Flags = UIViewElement.CellFlags.DisableSelection | UIViewElement.CellFlags.Transparent;				
			return imageElement;
		}
	}
}

