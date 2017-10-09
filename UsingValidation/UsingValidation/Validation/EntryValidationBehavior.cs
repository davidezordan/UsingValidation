using System.Linq;
using UsingValidation.Effects;
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
                    var borderEffect = _associatedObject.Effects.FirstOrDefault(eff => eff is BorderEffect);
                    if (borderEffect == null)
                    {
                        _associatedObject.Effects.Add(new BorderEffect());
                    }
                }
                else
				{
                    var borderEffect = _associatedObject.Effects.FirstOrDefault(eff => eff is BorderEffect);
                    if (borderEffect != null)
                    {
                        _associatedObject.Effects.Remove(borderEffect);
                    }
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
