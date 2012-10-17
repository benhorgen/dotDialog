using Android.Dialog;
using System.Collections.Generic;
using System.Linq;

namespace dotDialog.Sample.PersonalInfoManger
{
    public static class TaskListDialogSections
    {
        public static List<Section> CreateTaskListSection(IEnumerable<Task> taskList)
        {
            return new List<Section> { new Section
            {
                taskList != null && taskList.Any() ? 
                taskList.Select<Task, Element>(task => new StringElement(task.Date.ToString("d"), task.Description)):
                new List<Element> { new StringElement(string.Empty, "No Tasks"), }
            } };
        }
    }
}