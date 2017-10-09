using Android.Support.V4.Content;
using System;
using UsingValidation.Droid.Effects;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ResolutionGroupName("UsingValidationSample")]
[assembly: ExportEffect(typeof(BorderEffect), "BorderEffect")]
namespace UsingValidation.Droid.Effects
{
    public class BorderEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            try
            {
                var drawable = ContextCompat.GetDrawable(Control.Context, Resource.Drawable.border);
                Control.SetBackground(drawable);
            }
            catch (Exception)
            {
            }
        }

        protected override void OnDetached()
        {
            try
            {
                Control.SetBackground(null);
            }
            catch (Exception)
            {
            }
        }
    }
}