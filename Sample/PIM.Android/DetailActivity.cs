using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using MonoCross.Droid;
using System;
using MonoCross.Navigation;
using Fragment = Android.Support.V4.App.Fragment;

namespace dotDialog.Sample.PersonalInfoManger.Droid
{
    [Activity(Label = "@string/app_name",
    Icon = "@drawable/icon",
    Theme = "@style/ApplicationTheme",
    ConfigurationChanges = ConfigChanges.KeyboardHidden | ConfigChanges.Orientation | ConfigChanges.ScreenSize,
    WindowSoftInputMode = SoftInput.AdjustPan)]
    public class DetailActivity : FragmentActivity
    {
        private Action<Type> _oldHandler;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.detail);
            _oldHandler = MXDroidContainer.NavigationHandler;
            MXDroidContainer.NavigationHandler = NavigationHandler;
            NavigationHandler(Type.GetType(Intent.Extras.GetString("type")));
        }

        private void NavigationHandler(Type viewType)
        {
            var transaction = SupportFragmentManager.BeginTransaction();
            var perspective = MXContainer.Instance.Views.GetViewPerspectiveForViewType(viewType);
            var mxView = MXContainer.Instance.Views.GetView(perspective);
            if (mxView != null && !(mxView is Fragment)) return;
            var view = FindViewById<FrameLayout>(Resource.Id.detail_fragment);
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

        protected override void OnDestroy()
        {
            base.OnDestroy();
            MXDroidContainer.NavigationHandler = _oldHandler;
            _oldHandler = null;
        }
    }
}