using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace MonoTouch.Dialog.AddOn
{
	public class RadioBounceBackElement : RadioElement
	{
		public override void Selected (DialogViewController dvc, UITableView tableView, NSIndexPath indexPath)
		{
			base.Selected (dvc, tableView, indexPath);

			dvc.NavigationController.PopViewControllerAnimated(true);
		}

		public RadioBounceBackElement(string caption) : base(caption) { }
	}
}
