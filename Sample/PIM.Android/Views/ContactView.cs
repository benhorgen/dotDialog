using System;
using Android.Content;
using Android.Dialog;
using Android.Telephony;
using Android.Views;
using MonoCross.Droid;
using MonoCross.Navigation;

namespace dotDialog.Sample.PersonalInfoManger.Droid
{
    public partial class ContactView : MXDialogFragmentView<Contact>
    {
        public override void Render()
        {
            // verify model is not null
            if (Model == null) { throw new Exception("Model cannot be null when rendering: " + GetType()); }

            Root = new RootElement(Model.FirstName + " " + Model.LastName);

            // phone numbers

            var sections = ContactDialogSections.CreateContactDetailSections(Model);
            if (sections != null && sections.Length > 2)
            {
                // add a click handler to the Phone numbers
                foreach (Element e in sections[0])
                {
                    StringElement se = e as StringElement;
                    se.Tapped += (ob, ev) => DisplayPhoneCallDialog(se);
                }

                for (int i = 0; i < sections[1].Count; i++)
                {
                    StringElement se = sections[1][i] as StringElement;
                    se.Tapped += (ob, ev) => InitiateNewEmail(se.Value);
                }
            }
            Root.Add(sections);
        }

        void InitiateNewEmail(string email)
        {
            /* Create the Intent */
            Intent emailIntent = new Intent(Intent.ActionSend);

            /* Fill it with Data */
            emailIntent.SetType("plain/text");
            emailIntent.PutExtra(Intent.ExtraEmail, new[] { email });
            emailIntent.PutExtra(Intent.ExtraSubject, "Default subject");
            emailIntent.PutExtra(Intent.ExtraText, "Default text");

            /* Send it off to the Activity-Chooser */
            StartActivity(Intent.CreateChooser(emailIntent, "Send mail"));
        }

        void DisplayPhoneCallDialog(StringElement element)
        {
            string phoneNumber = element.Value;
            if (string.IsNullOrEmpty(phoneNumber)) return;
            phoneNumber = PhoneNumberUtils.FormatNumber(phoneNumber);
            var newIntent = new Intent(Intent.ActionDial, Android.Net.Uri.Parse("tel:" + phoneNumber));
            StartActivity(newIntent);
        }

        #region Menu
        public override void OnCreate(Android.OS.Bundle bundle)
        {
            base.OnCreate(bundle);
            SetHasOptionsMenu(true);
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            inflater.Inflate(Resource.Menu.ContactMenu, menu);
            base.OnCreateOptionsMenu(menu, inflater);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.menu_edit:
                    MXContainer.Navigate(ContactController.Uri(Model.Id, ViewPerspective.Update));
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }
        #endregion
    }
}