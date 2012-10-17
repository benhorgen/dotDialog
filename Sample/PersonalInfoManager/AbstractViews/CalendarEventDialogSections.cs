using System.Collections.Generic;

#if MONOTOUCH
using MonoTouch.Dialog;
using MonoTouch.Dialog.AddOn;
#elif ANDROID
using Android.Dialog;
#endif

namespace dotDialog.Sample.PersonalInfoManger
{
	public static class CalendarEventDialogSections
	{
		public static List<Section> CreateEventDetailsSection(CalEvent theEvent)
		{ 
			var sections = new List<Section>();

			var section = new Section();
			sections.Add(section);
			section.Add(new StringElement("Type", theEvent.Type));

			var details = new Section("Details");
			sections.Add(details);
			details.Add(new StringElement("Subject", theEvent.Subject));
			string time = theEvent.StartTime.Ticks > 0 ? theEvent.StartTime.ToString("d") : "unknown";
			details.Add(new StringElement("Start Time", time));
			time = theEvent.EndTime.Ticks > 0 ? theEvent.EndTime.ToString("d") : "unknown";
			details.Add(new StringElement("End Time", time));
			details.Add(new StringElement("Location", theEvent.Location));

			var additionalDetails = new Section("Additional Details");
			sections.Add(additionalDetails);
			additionalDetails.Add(new StringElement("Priority", theEvent.Priority));

			return sections;
		}
	}
}