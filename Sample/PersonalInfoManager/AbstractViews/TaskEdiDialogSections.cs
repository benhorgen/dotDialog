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
			DateTime date;
			string description = string.Empty;
			
			// update values if a contact was passed in
			if (t != null)
			{
				date = t.Date;
				if (!string.IsNullOrEmpty(t.Description)) description = t.Description;
			}
			
			// build dialog section
			var name = new Section();
			name.Add(new DateTimeElement("Date", date));
#if MONOTOUCH
			name.Add(new MultiLineEntrySubTextItem("Description", description ?? string.Empty, true) { Rows = 4 });
#endif
#if ANDROID
			name.Add(new MultilineEntryElement("Description", description ?? string.Empty));
#endif
		
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
#if MONOTOUCH
						t.Description = ((MultiLineEntrySubTextItem)el).Value;
#endif
#if ANDROID
						t.Description = ((MultilineEntryElement)el).Value;
#endif
					}
				}
			}
			
			//TODO: Refactor method to void if it can't fail
			return true;
		}
	}
}

