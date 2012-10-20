using System.Linq;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using MonoCross.Droid;
using System.Collections.Generic;
using MonoCross.Navigation;

namespace dotDialog.Sample.PersonalInfoManger.Droid
{
    public partial class TaskListView : MXListFragmentView<List<Task>>
    {
        private TaskListAdapter _adapter;
        public override void Render()
        {
            ListAdapter = _adapter = new TaskListAdapter(Activity, Model);
        }

        public override void OnViewModelChanged(object model)
        {
            _adapter.Items = Model;
        }

        public override void OnCreate(Android.OS.Bundle bundle)
        {
            base.OnCreate(bundle);
            SetHasOptionsMenu(true);
            MXContainer.AddView<List<Task>>(this);
        }

        private class TaskListAdapter : ArrayAdapter<Task>
        {
            private List<Task> _items;
            public List<Task> Items
            {
                get { return _items; }
                set
                {
                    _items = value;
                    ((Activity)Context).RunOnUiThread(NotifyDataSetChanged);
                }
            }

            public TaskListAdapter(Context context, List<Task> items)
                : base(context, Android.Resource.Layout.SimpleListItem2, items)
            {
                _items = items;
            }

            public override int Count
            {
                get { return Items == null || Items.Count == 0 ? 1 : Items.Count; }
            }

            public override View GetView(int position, View convertView, ViewGroup parent)
            {
                View v = convertView;
                if (v == null)
                {
                    var li = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);
                    v = li.Inflate(Android.Resource.Layout.SimpleListItem2, null);
                }

                var tt = v.FindViewById<TextView>(Android.Resource.Id.Text1);
                var bt = v.FindViewById<TextView>(Android.Resource.Id.Text2);

                if (Items == null || !Items.Any())
                {
                    tt.Text = string.Empty;
                    bt.Text = "No Tasks";
                }
                else
                {
                    var o = Items[position];
                    if (o != null)
                    {
                        if (tt != null)
                            tt.Text = o.Description;
                        if (bt != null)
                            bt.Text = o.Date.ToString("d");
                    }
                }

                return v;
            }
        }

        public override void OnListItemClick(ListView p0, View p1, int position, long id)
        {
            base.OnListItemClick(p0, p1, position, id);
            if (_adapter.Items == null || _adapter.Items.Count == 0)
                return;
            string taskId = _adapter.Items[position].Id;
            string uri = TaskController.Uri(taskId);
            MXContainer.Navigate(uri);
        }

        #region Menu
        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            inflater.Inflate(Resource.Menu.TaskListMenu, menu);
            base.OnCreateOptionsMenu(menu, inflater);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.menu_add:
                    MXContainer.Navigate(TaskController.UriForNew());
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }
        #endregion
    }
}