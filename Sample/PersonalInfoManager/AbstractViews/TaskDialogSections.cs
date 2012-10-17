using System;
using System.Collections.Generic;

#if MONOTOUCH
using MonoTouch.Dialog;
using MonoTouch.Dialog.AddOn;
#elif ANDROID
using Android.Dialog;
#endif

namespace dotDialog.Sample.PersonalInfoManger
{
	public static class TaskDialogSections
	{
		public static Section[] CreateTaskDetailSections(Task task)
		{
			List<Section> sections = new List<Section>();
			
			// additional info
			var detailSection = new Section();
			sections.Add(detailSection);
			
			string dateValue = task.Date.ToShortTimeString();
			detailSection.Add(new StringElement("Date", dateValue));
			detailSection.Add(new StyledMultilineElement("Description", task.Description));
			return sections.ToArray();
		}
	}
}

