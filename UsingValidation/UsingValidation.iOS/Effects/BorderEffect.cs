using System;
using UIKit;
using UsingValidation.iOS.Effects;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ResolutionGroupName("UsingValidationSample")]
[assembly: ExportEffect(typeof(BorderEffect), "BorderEffect")]
namespace UsingValidation.iOS.Effects
{
    public class BorderEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            try
            {
                Control.Layer.BorderColor = UIColor.Red.CGColor;
                Control.Layer.BorderWidth = 1;
            }
            catch (Exception)
            {
            }
        }

        protected override void OnDetached()
        {
            try
            {
                Control.Layer.BorderWidth = 0;
            }
            catch (Exception)
            {
            }
        }
    }
}