using System;
using Android.Dialog;
using MonoCross.Droid;
using MonoCross.Navigation;

namespace dotDialog.Sample.PersonalInfoManger.Droid
{
    public partial class TaskView : MXDialogFragmentView<Task>
    {
        public override void Render()
        {
            // verify model is not null
            if (Model == null) { throw new Exception("Model cannot be null when rendering: " + GetType()); }

            Root = new RootElement(Model.Date.ToString("d"));
            var sections = TaskDialogSections.CreateTaskDetailSections(Model);
            sections[sections.Length - 1].FooterView = new ButtonElement("Edit Task", (sender, e) => MXContainer.Navigate(TaskController.Uri(Model.Id, ViewPerspective.Update)));
            Root.Add(sections);
        }
    }
}