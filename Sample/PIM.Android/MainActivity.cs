using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using Java.Lang;
using MonoCross.Droid;
using MonoCross.Navigation;
using Android.Support.V4.App;
using Fragment = Android.Support.V4.App.Fragment;
using FragmentManager = Android.Support.V4.App.FragmentManager;

namespace dotDialog.Sample.PersonalInfoManger.Droid
{
    [Activity(Label = "@string/app_name",
    MainLauncher = true,
    Icon = "@drawable/icon",
    Theme = "@style/ApplicationTheme",
    ConfigurationChanges = ConfigChanges.KeyboardHidden | ConfigChanges.Orientation | ConfigChanges.ScreenSize,
    WindowSoftInputMode = SoftInput.AdjustPan)]
    public class MainActivity : FragmentActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // assign a layout with an image
            SetContentView(Resource.Layout.Main);

            // initialize app
            MXDroidContainer.Initialize(new App(), this);
            MXDroidContainer.NavigationHandler = NavigationHandler;

            // initialize views
            // Add Contact related views
            MXContainer.AddView<ContactListModel>(typeof(ContactListView));
            string noDataMsg = "Couldn't retreive contact list";
            MXContainer.AddView<ContactListModel>(new NoConnectionContactListView(this, ContactListView.ViewTitle, noDataMsg), "No Data");

            MXContainer.AddView<Contact>(typeof(ContactView));
            MXContainer.AddView<Contact>(new ContactEditView(ContactEditView.CreateButtonText), ViewPerspective.Create);
            MXContainer.AddView<Contact>(new ContactEditView(ContactEditView.SaveButtonText), ViewPerspective.Update);

            // Add Calendar related views
            MXContainer.AddView<CalendarListModel>(typeof(CalendarView));
            MXContainer.AddView<CalEvent>(typeof(CalendarEventView));
            MXContainer.AddView<CalEvent>(new CalendarEventUpdateView(CalendarEventUpdateView.CreateButtonText), ViewPerspective.Create);
            MXContainer.AddView<CalEvent>(new CalendarEventUpdateView(CalendarEventUpdateView.SaveButtonText), ViewPerspective.Update);

            // Add Task related views
            MXContainer.AddView<List<Task>>(typeof(TaskListView));
            MXContainer.AddView<Task>(typeof(TaskView));
            MXContainer.AddView<Task>(new TaskEditView(TaskEditView.CreateButtonText), ViewPerspective.Create);
            MXContainer.AddView<Task>(new TaskEditView(TaskEditView.SaveButtonText), ViewPerspective.Update);

            // navigate to first view
            MXContainer.Navigate(null, MXContainer.Instance.App.NavigateOnLoad);

            _adapter = new ViewPagerAdapter(SupportFragmentManager);
            var pager = FindViewById<ViewPager>(Resource.Id.pager);
            pager.Adapter = _adapter;
        }

        private ViewPagerAdapter _adapter;

        private void NavigationHandler(Type viewType)
        {
            var isDetailView = viewType != typeof(ContactListView) && viewType != typeof(CalendarView) && viewType != typeof(TaskListView);
            if (FindViewById(Resource.Id.detail_fragment) == null && isDetailView)
            {
                var detailIntent = new Intent(this, typeof(DetailActivity));
                var icicle = new Bundle();
                icicle.PutString("type", viewType.AssemblyQualifiedName);
                detailIntent.PutExtras(icicle);
                StartActivity(detailIntent);
                return;
            }

            var perspective = MXContainer.Instance.Views.GetViewPerspectiveForViewType(viewType);
            var mxView = MXContainer.Instance.Views.GetView(perspective);
            if (mxView != null && !(mxView is Fragment)) return;
            if (isDetailView)
            {
                var view = FindViewById<FrameLayout>(Resource.Id.detail_fragment);
                var transaction = SupportFragmentManager.BeginTransaction();
                if (view != null && view.ChildCount > 0)
                {
                    transaction.SetTransition(Android.Support.V4.App.FragmentTransaction.TransitFragmentOpen);
                    transaction.AddToBackStack(null);

                }
                RunOnUiThread(() =>
                {
                    var fragment = (Fragment)(mxView ?? Activator.CreateInstance(viewType));
                    if (fragment.IsAdded)
                    {
                        Finish();
                        return;
                    }
                    transaction.Replace(Resource.Id.detail_fragment, fragment);
                    transaction.Commit();
                });
            }
            else
            {
                RunOnUiThread(() => _adapter.SetFragment((Fragment)(mxView ?? Activator.CreateInstance(viewType))));
            }
        }
    }

    public class ViewPagerAdapter : FragmentPagerAdapter
    {
        public ViewPagerAdapter(FragmentManager fm) : base(fm) { }
        private readonly Fragment[] _tabs = new Fragment[3];
        private readonly List<string> _titles = new List<string> { "Contacts", "Calendar", "Tasks" };

        public void SetFragment(Fragment fragment)
        {
            int i = fragment is ContactListView ? 0 : (fragment is CalendarView ? 1 : 2);
            _tabs[i] = fragment;
            NotifyDataSetChanged();
        }

        public override Fragment GetItem(int i)
        {
            var fragment = _tabs[i];
            if (fragment == null)
            {
                MXContainer.Navigate(i == 0 ? ContactListController.Uri : (i == 1 ? CalendarListController.Uri : TaskListController.Uri));
                switch (i)
                {
                    case 0: fragment = new ContactListView(); break;
                    case 1: fragment = new CalendarView(); break;
                    case 2: fragment = new TaskListView(); break;
                };
                _tabs[i] = fragment;
            }

            return fragment;
        }

        public override int Count
        {
            get { return _tabs.Length; }
        }

        public override ICharSequence GetPageTitleFormatted(int position)
        {
            return new Java.Lang.String(_titles[position]);
        }
    }
}