using System.Linq;
using Android.App;
using Android.Content;
using Android.Telephony;
using MonoCross.Droid;
using Android.Dialog;

namespace dotDialog.Sample.PersonalInfoManger.Droid
{
    [Activity(Label = "Contact Details")]
    public class ContactActivity : MXDialogFragmentView<Contact>
    {
        public override void Render()
        {
            var dialogSection = ContactDialogSections.CreateContactDetailSections(Model);

            var sections = ContactDialogSections.CreateContactDetailSections(Model);
            if (sections != null && sections.Length > 2)
            {
                // add a click handler to the Phone numbers
                foreach (var se in sections[0].Select(e => e as StringElement))
                {
                    var local = se;
                    se.Tapped += (o, ev) => DisplayPhoneCallDialog(local.Value);
                }

                // add a click handler to the Phone numbers
                foreach (var se in sections[1].Select(e => e as StringElement))
                {
                    var local = se;
                    se.Click += (sender, e) => StartEmailIntent(local.Value);
                }
            }

            this.Root = new RootElement(null) { dialogSection };
        }

        private void StartEmailIntent(string email)
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

        private void DisplayPhoneCallDialog(string phone)
        {
            string phoneNumber = PhoneNumberUtils.FormatNumber(phone);
            var newIntent = new Intent(Intent.ActionDial, Android.Net.Uri.Parse("tel:" + phoneNumber));
            StartActivity(newIntent);
        }
    }
}