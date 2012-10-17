using System;
using Android.Dialog;
using Android.OS;
using Android.Views;
using MonoCross.Droid;
using MonoCross.Navigation;

namespace dotDialog.Sample.PersonalInfoManger.Droid
{
    public partial class CalendarEventView : MXDialogFragmentView<CalEvent>
    {
        public override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetHasOptionsMenu(true);
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            inflater.Inflate(Resource.Menu.CalendarEventMenu, menu);
            base.OnCreateOptionsMenu(menu, inflater);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.menu_edit:
                    string updateUri = CalendarEventController.Uri(Model.Id, ViewPerspective.Update);
                    MXDroidContainer.Navigate(updateUri);
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }

        public override void Render()
        {
            // verify model is not null
            if (Model == null) { throw new Exception("Model cannot be null when rendering: " + GetType()); }

            Root = new RootElement("Event");
            var sections = CalendarEventDialogSections.CreateEventDetailsSection(Model);
            Root.Add(sections);
        }
    }
}