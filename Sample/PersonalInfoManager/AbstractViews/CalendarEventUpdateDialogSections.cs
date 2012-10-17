using System;
using System.Collections.Generic;
using System.Linq;

#if MONOTOUCH
using MonoTouch.Dialog;
using MonoTouch.Dialog.AddOn;
#elif ANDROID
using Android.Dialog;
#endif

namespace dotDialog.Sample.PersonalInfoManger
{
    public static class CalendarEventUpdateDialogSections
    {
        public static List<Section> BuildDialogSections(CalEvent Model)
        {
            List<Section> sections = new List<Section>();
            var section = new Section();
            sections.Add(section);
            int typeOption;
            for (typeOption = 0; typeOption < TypeOptions.Length; typeOption++)
            {
                if (TypeOptions[typeOption] == Model.Type) { break; }
            }
            if (typeOption >= TypeOptions.Length) { typeOption = 0; } //set to default
            var details = new Section("Details");
            sections.Add(details);
            details.Add(new EntryElement("Subject", "no subject entered", Model.Subject));

            details.Add(new DateTimeElement("Start Time", Model.StartTime));
            DateTime endTime = Model.EndTime;
            if (endTime.Ticks == 0) { endTime = Model.StartTime.AddHours(1); }
            details.Add(new DateTimeElement("End Time", endTime));
            details.Add(new EntryElement("Location", "no location entered", Model.Location));

            var additionalDetails = new Section("Additional Details");
            sections.Add(additionalDetails);

            int priorityOption;
            for (priorityOption = 0; priorityOption < PriorityOptions.Length; priorityOption++)
            {
                if (PriorityOptions[priorityOption] == Model.Priority) { break; }
            }
            if (priorityOption >= PriorityOptions.Length) { priorityOption = 0; } //set to default
            var options = new List<Element>(PriorityOptions.Length);
            foreach (string o in PriorityOptions) { options.Add(new RadioBounceBackElement(o)); }
            var optionsSection = new Section();
            optionsSection.Elements.AddRange(options);
            var priorityRoot = new RootElement("Priority", new RadioGroup(priorityOption)) { optionsSection };
            additionalDetails.Add(priorityRoot);
            return sections;
        }

        public static bool SaveDialogElementsToModel(CalEvent t, List<Section> sections)
        {
            if (sections != null)
            {
                foreach (var e in sections.SelectMany(section => section))
                {
                    if (e.Caption == "Type") { t.Type = TypeOptions[((RootElement)e).RadioSelected]; }
                    else if (e.Caption == "Start Time")
                    {
                        var start = ((DateTimeElement)e).Value;
                        t.StartTimeAsLong = Convert.ToDateTime(start).Ticks;
                    }
                    else if (e.Caption == "End Time")
                    {
                        var end = ((DateTimeElement)e).Value;
                        t.EndTimeAsLong = Convert.ToDateTime(end).Ticks;
                    }
                    else if (e.Caption == "Subject") { t.Subject = ((EntryElement)e).Value; }
                    else if (e.Caption == "Location") { t.Location = ((EntryElement)e).Value; }
                    else if (e.Caption == "Priority") { t.Priority = PriorityOptions[((RootElement)e).RadioSelected]; }
                }
            }

            return true;
        }

        public static string[] TypeOptions = new string[] { "Call", "In person", "Skype" };
        public static string[] PriorityOptions = new string[] { "High", "Medium", "Low" };
    }
}