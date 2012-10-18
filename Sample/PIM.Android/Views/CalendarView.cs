using System;
using System.Linq;
using System.Collections.Generic;
using Android.Dialog;
using Android.OS;
using Android.Views;
using Android.Widget;
using MonoCross.Droid;
using MonoCross.Navigation;

namespace dotDialog.Sample.PersonalInfoManger.Droid
{
    public partial class CalendarView : MXDialogFragmentView<CalendarListModel>
    {
        public override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetHasOptionsMenu(true);
            MXContainer.AddView<CalendarListModel>(this);
        }

        public override void Render()
        {
            if (Model == null) return;
            Root = new RootElement(string.Empty) { CalendarDialogSections.CreateCalendarSection(Model) };
        }

        protected override void OnViewModelChanged(object model)
        {
            Render();
        }

        public override void OnListItemClick(ListView p0, View p1, int position, long p3)
        {
            if (Model == null || !Model.Events.Any()) return;
            var id = ((DialogAdapter)p0.Adapter).ElementAtIndex(position).Tag.ToString();
            string uri = CalendarEventController.Uri(id);
            MXContainer.Navigate(uri);
        }

        #region Menu
        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            inflater.Inflate(Resource.Menu.CalendarMenu, menu);
            base.OnCreateOptionsMenu(menu, inflater);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.menu_add:
                    MXContainer.Navigate(CalendarEventController.UriForNew());
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }
        #endregion
    }
}