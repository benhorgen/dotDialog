using System;
using System.Collections.Generic;

#if MONOTOUCH
using MonoTouch.Dialog;
using MonoTouch.Dialog.AddOn;
#elif ANDROID
using Android.Dialog;
#endif

namespace dotDialog.Sample.PersonalInfoManger
{
    public static class ContactDialogSections
    {
        public static Section[] CreateContactDetailSections(Contact c)
        {
            List<Section> sections = new List<Section>();

            //TODO: awating a refactor of Phones/Emails/Address to being lists in Contact
            var Phones = new List<string>(new string[] { c.Phone });
            var emails = new List<string>(new string[] { c.Email });
            var addresses = new List<string>(new string[] { c.Address });

            var phoneSec = new Section() { Caption = "Phone Numbers" };
            sections.Add(phoneSec);
            if (Phones != null && Phones.Count > 0)
            {
                var cell = new StringElement("Cell", Phones[0]);
                phoneSec.Add(cell);
            }
            var work = new StringElement("Office", "651-555-1212");
            phoneSec.Add(work);
            var home = new StringElement("Home", "507-555-1212");
            phoneSec.Add(home);

            // email
            var emailSec = new Section() { Caption = "Email Addresses" };
            sections.Add(emailSec);
            if (emails != null && emails.Count > 0)
            {
                string emailAddress = emails[0];
                var email = new StringElement("Work", emailAddress);
                emailSec.Add(email);
            }
            var homeEmail = new StringElement("Home", "me@home.com");
            emailSec.Add(homeEmail);

            // addresses
            var addressSec = new Section() { Caption = "Addresses" };
            sections.Add(addressSec);
            if (addresses != null && addresses.Count > 0)
                addressSec.Add(new MultilineElement("Home", addresses[0]));
            return sections.ToArray();
        }

    }
}