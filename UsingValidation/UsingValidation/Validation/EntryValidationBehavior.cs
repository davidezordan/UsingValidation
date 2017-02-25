using System.Linq;
using Xamarin.Forms;

namespace UsingValidation.Validation
{
    public class EntryValidationBehavior : Behavior<Entry>
    {
        private Entry AssociatedObject;

        #region AttachBehavior property

        public static readonly BindableProperty AttachBehaviorProperty =
            BindableProperty.CreateAttached("AttachBehavior", typeof(bool), typeof(EntryValidationBehavior), false, propertyChanged: OnAttachBehaviorChanged);

        public static bool GetAttachBehavior(BindableObject view)
        {
            return (bool)view.GetValue(AttachBehaviorProperty);
        }

        public static void SetAttachBehavior(BindableObject view, bool value)
        {
            view.SetValue(AttachBehaviorProperty, value);
        }

        static void OnAttachBehaviorChanged(BindableObject view, object oldValue, object newValue)
        {
            var entry = view as Entry;
            if (entry == null)
            {
                return;
            }

            bool attachBehavior = (bool)newValue;
            if (attachBehavior)
            {
                entry.Behaviors.Add(new EntryValidationBehavior());
            }
            else
            {
                var toRemove = entry.Behaviors.FirstOrDefault(b => b is EntryValidationBehavior);
                if (toRemove != null)
                {
                    entry.Behaviors.Remove(toRemove);
                }
            }
        }

        #endregion

        #region Source Bindable Property

        public static readonly BindableProperty SourceProperty =
                BindableProperty.CreateAttached("Source", typeof(ValidationBase), typeof(EntryValidationBehavior), null, propertyChanged: OnSourceChanged);

        public static ValidationBase GetSource(BindableObject view)
        {
            return (ValidationBase)view.GetValue(SourceProperty);
        }

        public static void SetSource(BindableObject view, ValidationBase value)
        {
            view.SetValue(SourceProperty, value);
        }

        static void OnSourceChanged(BindableObject view, object oldValue, object newValue)
        {
            var entry = view as Entry;
            if (entry == null)
            {
                return;
            }

            //var oldValidationObject = newValue as ValidationBase;
            //if (oldValidationObject != null)
            //{
            //    oldValidationObject.ErrorsChanged -= ValidationObject_ErrorsChanged;
            //}

            //var validationObject = newValue as ValidationBase;
            //if (validationObject != null)
            //{
            //    validationObject.ErrorsChanged += ValidationObject_ErrorsChanged;
            //}
        }

        private static void ValidationObject_ErrorsChanged(object sender, System.ComponentModel.DataErrorsChangedEventArgs e)
        {
            var prop = e.PropertyName;

            var validationObject = (sender as ValidationBase);
            if (validationObject != null)
            {
                if (validationObject.HasErrors)
                {
                    //AssociatedObject.BackgroundColor = Color.Red;
                    var errors = validationObject.GetErrors(prop);
                    var message = errors?.Cast<string>()?.FirstOrDefault();
                    if (message != null)
                    {
                        //AssociatedObject.Placeholder = message;
                    }
                }
                else
                {
                    //AssociatedObject.Placeholder = string.Empty;
                    //AssociatedObject.BackgroundColor = Color.Default;
                }
            }
        }

        #endregion

        protected override void OnAttachedTo(Entry bindable)
        {
            base.OnAttachedTo(bindable);
            // Perform setup       

            AssociatedObject = bindable;
        }

        protected override void OnDetachingFrom(Entry bindable)
        {
            base.OnDetachingFrom(bindable);
            // Perform clean up
            
            AssociatedObject = null;
        }
    }
}
