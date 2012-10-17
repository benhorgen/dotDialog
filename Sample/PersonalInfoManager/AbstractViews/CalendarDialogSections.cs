using Android.Dialog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace dotDialog.Sample.PersonalInfoManger
{
    public static class CalendarDialogSections
    {
        public static IEnumerable<Section> CreateCalendarSection(CalendarListModel model)
        {
            IEnumerable<CalEvent> eventsList = new List<CalEvent>();
            if (model.Events != null)
            {
                eventsList = from e in model.Events
                             orderby e.StartTime ascending
                             select e;
            }
            List<CalEvent> events = new List<CalEvent>(eventsList);
            DateTime prevDate = new DateTime(0);
            var _root = new List<Section>();
            Section date = null;
            foreach (CalEvent e in events)
            {
                if (e.StartTime.Date != prevDate.Date)
                {
                    date = new Section(e.StartTime.Date.ToShortDateString());
                    _root.Add(date);
                }
                string startTime = e.StartTime.ToString("t");
                var se = new StringElement(e.Subject, startTime) { Tag = e.Id, };
                date.Add(se);
                prevDate = e.StartTime;
            }
            if (_root.Count == 0)
                _root.Add(new Section { new StringElement(string.Empty, "No Events") });
            return _root;
        }
    }
}