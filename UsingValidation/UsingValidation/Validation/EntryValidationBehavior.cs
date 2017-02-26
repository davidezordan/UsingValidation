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
