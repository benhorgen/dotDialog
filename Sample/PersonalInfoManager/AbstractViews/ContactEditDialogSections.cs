using System.Collections.Generic;

#if MONOTOUCH
using MonoTouch.Dialog;
using MonoTouch.Dialog.AddOn;
#elif ANDROID
using Android.Dialog;
#endif

namespace dotDialog.Sample.PersonalInfoManger
{
    public static class ContactEditDialogSections
    {
        public static Section[] BuildDialogSections(Contact c)
        {
            // init to default values
            string fName = string.Empty;
            string lName = string.Empty;
            string phoneNumber = string.Empty;
            string emailAddress = string.Empty;
            string address = string.Empty;

            // update values if a contact was passed in
            if (c != null)
            {
                if (!string.IsNullOrEmpty(c.FirstName)) fName = c.FirstName;
                if (!string.IsNullOrEmpty(c.LastName)) lName = c.LastName;
                if (!string.IsNullOrEmpty(c.Phone)) phoneNumber = c.Phone;
                if (!string.IsNullOrEmpty(c.Email)) emailAddress = c.Email;
                if (!string.IsNullOrEmpty(c.Address)) address = c.Address;
            }

            // build dialog section
            List<Section> sections = new List<Section>();

            var name = new Section() { Caption = "Individual" };
            sections.Add(name);
            name.Add(new EntryElement("First Name", null, fName) { KeyboardType = UIKeyboardType.NamePhonePad });
            name.Add(new EntryElement("Last Name", null, lName) { KeyboardType = UIKeyboardType.NamePhonePad });

            var phoneSec = new Section() { Caption = "Phone Numbers" };
            sections.Add(phoneSec);
            //TODO: Add a button which adds a new row
            phoneSec.Add(new EntryElement("Work", null, phoneNumber) { KeyboardType = UIKeyboardType.PhonePad });

            // email
            var emailSec = new Section() { Caption = "Email Addresses" };
            sections.Add(emailSec);
            emailSec.Add(new EntryElement("Work", null, emailAddress) { KeyboardType = UIKeyboardType.EmailAddress });

            // addresses
            var addressSec = new Section() { Caption = "Addresses" };
            sections.Add(addressSec);
            addressSec.Add(new MultilineEntryElement("Home", address));
            return sections.ToArray();
        }

        public static bool SaveDialogElementsToModel(Contact c, Section[] section)
        {
            if (section != null)
            {
                foreach (Section s in section)
                {
                    foreach (Element e in s)
                    {
                        if (e.Caption == "First Name") { c.FirstName = ((EntryElement)e).Value; }
                        else if (e.Caption == "Last Name") { c.LastName = ((EntryElement)e).Value; }
                        else if (s.Caption == "Phone Numbers" && e.Caption == "Work") { c.Phone = ((EntryElement)e).Value; }
                        else if (s.Caption == "Email Addresses" && e.Caption == "Work") { c.Email = ((EntryElement)e).Value; }
                        else if (s.Caption == "Addresses" && e.Caption == "Home") { c.Address = ((MultilineEntryElement)e).Value; }
                    }
                }
            }

            //TODO: Refactor method to void if it can't fail
            return true;
        }
    }
}