using System.Linq;
using Xamarin.Forms;

namespace UsingValidation.Validation
{
    public class EntryValidationBehavior : Behavior<Entry>
    {
		private Entry _associatedObject;

        protected override void OnAttachedTo(Entry bindable)
        {
            base.OnAttachedTo(bindable);
            // Perform setup       

            _associatedObject = bindable;

			_associatedObject.TextChanged += _associatedObject_TextChanged;
        }

		void _associatedObject_TextChanged(object sender, TextChangedEventArgs e)
		{
			var source = _associatedObject.BindingContext as ValidationBase;
			if (source != null && !string.IsNullOrEmpty(PropertyName))
			{
				var errors = source.GetErrors(PropertyName).Cast<string>();
				if (errors != null && errors.Any())
				{
					_associatedObject.BackgroundColor = Color.Red;
				}
				else
				{
					_associatedObject.BackgroundColor = Color.Default;
				}
			}
		}

		protected override void OnDetachingFrom(Entry bindable)
        {
            base.OnDetachingFrom(bindable);
            // Perform clean up

			_associatedObject.TextChanged += _associatedObject_TextChanged;

            _associatedObject = null;
        }

		public string PropertyName { get; set; }
    }
}
