using System.Collections.Generic;
using Android.Views;
using Android.Widget;
using MonoCross.Droid;
using Android.Dialog;
using MonoCross.Navigation;


namespace dotDialog.Sample.PersonalInfoManger.Droid
{
    public class CalendarEventUpdateView : MXDialogFragmentView<CalEvent>
    {
        private string _parameter;

        public CalendarEventUpdateView(string buttonText)
        {
            _parameter = buttonText;
        }

        public override void Render()
        {
            sections = CalendarEventUpdateDialogSections.BuildDialogSections(Model);
            Root = new RootElement("Edit Event") { sections };
        }
        List<Section> sections;

        #region Menu
        public override void OnCreate(Android.OS.Bundle bundle)
        {
            base.OnCreate(bundle);
            SetHasOptionsMenu(true);
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            inflater.Inflate(_parameter == SaveButtonText ? Resource.Menu.EditMenu : Resource.Menu.CreateMenu, menu);
            base.OnCreateOptionsMenu(menu, inflater);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.menu_save:
                    CalendarEventUpdateDialogSections.SaveDialogElementsToModel(Model, sections);

                    bool createNew = _parameter == CreateButtonText;
                    bool success = CalendarListController.SaveEventToCalendar(Model, createNew, true);

                    if (success)
                        MXContainer.Navigate(CalendarListController.Uri);
                    else Toast.MakeText(Activity, "Failed to save event", ToastLength.Short).Show();
                    return true;
                case Resource.Id.menu_delete:
                    var deleteUri = CalendarEventController.Uri(Model.Id, ViewPerspective.Delete);
                    new System.Threading.Thread(() => MXContainer.Navigate(deleteUri)).Start();
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }
        #endregion

        public static string SaveButtonText = "Save";
        public static string CreateButtonText = "Create";
    }
}

