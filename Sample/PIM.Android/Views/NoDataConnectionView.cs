using System;
using System.Drawing;
using Android.Content;
using Android.Widget;
using MonoCross.Droid;
using System.Collections.Generic;
using MonoCross.Navigation;

namespace dotDialog.Sample.PersonalInfoManger.Droid
{
    public abstract partial class NoDataConnectionView<T> : IMXView
    {
        private string _title;
        private string _message;

        public NoDataConnectionView(string title, string message)
        {
            _title = title;
            _message = message;
        }

        public NoDataConnectionView() : this("No Data", "No Data Connection") { }
        public NoDataConnectionView(string message) : this("No Data", message) { }

        public T Model { get; set; }

        public Type ModelType { get { return typeof(T); } }
        public void SetModel(object model)
        {
            Model = (T)model;
        }

        public abstract Context Context { get; set; }

        public virtual void Render()
        {
            Toast.MakeText(Context, _title + " - " + _message, ToastLength.Long).Show();
        }
    }
}

