using Android.Views;
using Android.Widget;
using MonoCross.Droid;
using MonoCross.Navigation;

using Android.Dialog;

namespace dotDialog.Sample.PersonalInfoManger.Droid
{
    public partial class TaskEditView : MXDialogFragmentView<Task>
    {
        private string parameter = null;
        public TaskEditView(string barButtonText)
        {
            parameter = barButtonText;
        }

        public override void Render()
        {
            // add shared dialog sections
            _sections = TaskEdiDialogSections.BuildDialogSections(Model);
            Root = new RootElement(Model.Date.ToShortDateString()) { _sections };
        }
        Section[] _sections;

        #region Menu
        public override void OnCreate(Android.OS.Bundle bundle)
        {
            base.OnCreate(bundle);
            SetHasOptionsMenu(true);
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            inflater.Inflate(parameter == SaveButtonText ? Resource.Menu.EditMenu : Resource.Menu.CreateMenu, menu);
            base.OnCreateOptionsMenu(menu, inflater);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.menu_save:
                    // save dialog values back to model using shared dotDialog APIs
                    TaskEdiDialogSections.SaveDialogElementsToModel(Model, _sections);

                    // save the task, create new if determined appropriate
                    bool createNew = parameter == CreateButtonText;
                    bool success = TaskListController.SaveTaskToDataSource(Model, createNew, true);

                    if (success)
                        MXDroidContainer.Navigate(TaskListController.Uri);
                    else Toast.MakeText(Activity, "Failed to save", ToastLength.Short).Show();
                    return true;
                case Resource.Id.menu_delete:
                    string deleteUri = TaskController.Uri(Model.Id, ViewPerspective.Delete);
                    MXDroidContainer.Navigate(deleteUri);
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }
        #endregion

        public static string SaveButtonText = "Save";
        public static string CreateButtonText = "Create";
    }
}