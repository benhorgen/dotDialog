using System;

#if MONOTOUCH
using MonoTouch.Dialog;
using MonoTouch.Dialog.AddOn;
#elif ANDROID
using Android.Dialog;
#endif

namespace dotDialog.Sample.PersonalInfoManger
{
	public static class TaskEdiDialogSections
	{
		public static Section[] BuildDialogSections(Task t)
		{
			// init to default values
			DateTime date = DateTime.Now;
			string description = null;
			
			// update values if a contact was passed in
			if (t != null)
			{
				date = t.Date;
				if (!string.IsNullOrEmpty(t.Description)) description = t.Description;
			}
			
			// build dialog section
			var name = new Section();
			name.Add(new DateTimeElement("Date", date));
			name.Add(new MultiLineEntrySubTextItem("Description", description ?? string.Empty, true) { Rows = 4 });
		
			return new Section[] { name };
		}
		
		public static bool SaveDialogElementsToModel(Task t, Section[] s)
		{
			if (s != null)
			{
				foreach (Element el in s[0])
				{
					if (el.Caption == "Date")
					{
						string dateValue = ((DateTimeElement)el).Value;
						t.Date = Convert.ToDateTime(dateValue);
					}
					else if (el.Caption == "Description")
					{
						t.Description = ((MultiLineEntrySubTextItem)el).Value;
					}
				}
			}
			
			//TODO: Refactor method to void if it can't fail
			return true;
		}
	}
}