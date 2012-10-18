using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using Android.Content;
using Android.Dialog;
using Android.Views;
using Android.Widget;
using MonoCross.Droid;
using MonoCross.Navigation;

namespace dotDialog.Sample.PersonalInfoManger.Droid
{
    public partial class ContactListView : MXDialogFragmentView<ContactListModel>
    {
        public ContactListView()
        {
            /* setup search bar
            _searchBar = new UISearchBar();
            _searchBar.AutocorrectionType = UITextAutocorrectionType.No;
            _searchBar.SizeToFit();
            _searchBar.CancelButtonClicked += (sender, e) =>
            {
                _contactSections.Clear();
                _contactSections.AddRange(_sectionedContactList);
                _tableView.ReloadData();
            };

            _sdc = new UISearchDisplayController(_searchBar, this);

            var button = new UIBarButtonItem(UIBarButtonSystemItem.Add);
            button.Click += delegate(object sender, EventArgs e)
            {
                MXContainer.Navigate(ContactController.UriForNew());
            };
            NavigationItem.SetRightBarButtonItem(button, true);*/
        }

        public override void Render()
        {
            var root = new RootElement(string.Empty);
            var contacts = new List<Contact>();
            if (Model != null) contacts.AddRange(Model.Contacts);

            if (!contacts.Any())
            {
                root.Add(new Section { new StringElement(string.Empty, "No Contacts") });
            }
            else foreach (char chr in _chars)
                {
                    var alphas = contacts.Where(c => c.LastName.ToUpper()[0] == chr).ToList();
                    contacts.RemoveAll(c => c.LastName.ToUpper()[0] == chr);
                    if (!alphas.Any()) continue;
                    var section = new Section(chr.ToString(CultureInfo.InvariantCulture)) { alphas.Select(RenderContact).Cast<Element>() };
                    root.Add(section);
                }

            if (contacts.Count > 0)
                root.Insert(0, new Section("#") { contacts.Select(RenderContact).Cast<Element>() });
            Root = root;
        }

        #region Menu
        public override void OnCreate(Android.OS.Bundle bundle)
        {
            base.OnCreate(bundle);
            SetHasOptionsMenu(true);
            MXContainer.AddView<ContactListModel>(this);
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            inflater.Inflate(Resource.Menu.ContactListMenu, menu);
            base.OnCreateOptionsMenu(menu, inflater);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.menu_add:
                    MXContainer.Navigate("Contact/" + ViewPerspective.Create + "/0");
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }
        #endregion

        protected override void OnViewModelChanged(object model)
        {
            Render();
        }

        private static StringElement RenderContact(Contact c)
        {
            string name;
            if (!string.IsNullOrEmpty(c.FirstName)) name = c.LastName + ", " + c.FirstName;
            else { name = c.LastName; }

            string phone = string.IsNullOrEmpty(c.Phone) ? "No Phone" : c.Phone;
            var item = new StringElement(name, phone) { Tag = c.Id };
            return item;
        }

        public override void OnListItemClick(ListView p0, View p1, int position, long p3)
        {
            if (Model == null || !Model.Contacts.Any()) return;
            var id = ((DialogAdapter)p0.Adapter).ElementAtIndex(position).Tag.ToString();
            string uri = ContactController.Uri(id);
            MXContainer.Navigate(uri);
        }

        internal const string ViewTitle = "Contacts";
        readonly char[] _chars = new[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 
			'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' 
		};

        //class SearchBarDelegate : UISearchBarDelegate
        //{
        //    List<Contact>[] _tableDataSourceOriginalList;
        //    List<List<Contact>> _tableDataSource;

        //    public SearchBarDelegate(ContactListView view, List<List<Contact>> model)
        //        : base()
        //    {
        //        _tableDataSource = model;
        //        _tableDataSourceOriginalList = _tableDataSource.ToArray();
        //    }

        //    public override void TextChanged(UISearchBar searchBar, string searchText)
        //    {
        //        List<List<Contact>> searchResults = new List<List<Contact>>();

        //        if (searchText.Length > 0)
        //        {
        //            List<Contact>[] _model = _tableDataSourceOriginalList;
        //            for (int i = 0; i < _model.Length; i++)
        //            {
        //                if (_model[i] != null)
        //                {
        //                    var results = (from c in _model[i]
        //                                   where c.FirstName.ToLower().Contains(searchText.ToLower()) ||
        //                                         c.LastName.ToLower().Contains(searchText.ToLower())
        //                                   select c).ToList();
        //                    searchResults.Add(results);
        //                }
        //                else { searchResults.Add(new List<Contact>()); /*insert empty section */ }
        //            }
        //        }
        //        else
        //        {
        //            // reset to original list
        //            searchResults.Clear();
        //            searchResults.AddRange(_tableDataSourceOriginalList);
        //        }

        //        _tableDataSource.Clear();
        //        _tableDataSource.AddRange(searchResults);
        //    }
        //}
    }

    internal class NoConnectionContactListView : NoDataConnectionView<ContactListModel>
    {
        public NoConnectionContactListView(Context context, string title, string msg)
            : base(title, msg)
        {
            Context = context;
        }

        public override Context Context { get; set; }

        public override void Render()
        {
            new Thread(() =>
            {
                Thread.Sleep(10000); //sleep a few seconds & try again
                MXContainer.Navigate(ContactListController.Uri);
            }).Start();
        }
    }
}