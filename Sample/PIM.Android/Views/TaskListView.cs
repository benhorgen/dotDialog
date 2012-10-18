using Android.Dialog;
using Android.Views;
using Android.Widget;
using MonoCross.Droid;
using System.Collections.Generic;
using MonoCross.Navigation;

namespace dotDialog.Sample.PersonalInfoManger.Droid
{
    public class TaskListView : MXDialogFragmentView<List<Task>>
    {
        public override void OnCreate(Android.OS.Bundle bundle)
        {
            base.OnCreate(bundle);
            SetHasOptionsMenu(true);
            MXContainer.AddView<List<Task>>(this);
        }

        protected override void OnViewModelChanged(object model)
        {
            Render();
        }

        public override void OnListItemClick(ListView p0, View p1, int position, long id)
        {
            base.OnListItemClick(p0, p1, position, id);
            if (Model == null || Model.Count == 0)
                return;
            string taskId = Model[position - 1].Id;
            string uri = TaskController.Uri(taskId);
            MXContainer.Navigate(uri);
        }

        #region Overrides of MXDialogFragmentView<List<Task>>

        /// <summary>
        /// Displays the view
        /// </summary>
        public override void Render()
        {
            Root = new RootElement(null) { TaskListDialogSections.CreateTaskListSection(Model) };
        }

        #endregion

        #region Menu
        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            inflater.Inflate(Resource.Menu.TaskListMenu, menu);
            base.OnCreateOptionsMenu(menu, inflater);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.menu_add:
                    MXContainer.Navigate(TaskController.UriForNew());
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }
        #endregion
    }
}