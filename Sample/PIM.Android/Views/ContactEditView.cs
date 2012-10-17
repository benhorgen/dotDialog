using Android.Dialog;
using Android.Views;
using Android.Widget;
using MonoCross.Droid;
using MonoCross.Navigation;

namespace dotDialog.Sample.PersonalInfoManger.Droid
{
    public partial class ContactEditView : MXDialogFragmentView<Contact>
    {
        private string _parameter;
        public ContactEditView(string barButtonText)
        {
            // build action sheet
            _parameter = barButtonText;
        }

        public override void Render()
        {
            sections = ContactEditDialogSections.BuildDialogSections(Model);
            Root = new RootElement("Add Contact") { sections };
        }
        Section[] sections;

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
                    ContactEditDialogSections.SaveDialogElementsToModel(Model, sections);

                    bool createNew = _parameter == CreateButtonText;

                    bool success = ContactListController.SaveContactToDataSource(Model, createNew, true);
                    if (success)
                    {
                        MXDroidContainer.Navigate(ContactListController.Uri);
                    }
                    else Toast.MakeText(Activity, "Failed to save", ToastLength.Short).Show();
                    return true;
                case Resource.Id.menu_delete:
                    string deleteUri = ContactController.Uri(Model.Id, ViewPerspective.Delete);
                    MXDroidContainer.Navigate(deleteUri);
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }
        #endregion

        public const string SaveButtonText = "Save";
        public const string CreateButtonText = "Create";
    }
}