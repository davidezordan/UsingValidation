# UsingValidation
Xamarin.Forms validation sample using INotifyDataErrorInfo

<p style="text-align: justify;"><em>This post is also available in the <strong><a href="https://blogs.msdn.microsoft.com/premier_developer/2017/04/03/validate-input-in-xamarin-forms-using-inotifydataerrorinfo-custom-behaviors-effects-and-prism/" target="_blank">Premier Developer blog</a></strong>.</em></p>
<p style="text-align: justify;">I have recently been investigating the support available in Xamarin.Forms for validation and, in particular, researched the possibility of using <strong>INotifyDataErrorInfo</strong> to complement the traditional approach of using <strong>Behaviors</strong>.</p>
<p style="text-align: justify;">In simple scenarios, it's possible to perform validation by simply attaching a Behavior to the required view, as shown by the following sample code:</p>

<pre title="Sample validation Behavior" class="lang:default decode:true ">public class SampleValidationBehavior:Behavior&lt;Entry&gt;
{
    protected override void OnAttachedTo(Entry entry)
    {
        entry.TextChanged += OnEntryTextChanged;
        base.OnAttachedTo(entry);
    }

    protected override void OnDetachingFrom(Entry entry)
    {
        entry.TextChanged -= OnEntryTextChanged;
        base.OnDetachingFrom(entry);
    }

    void OnEntryTextChanged(object sender, TextChangedEventArgs args)
    {
	 var textValue = args.NewTextValue;
        bool isValid = !string.IsNullOrEmpty(textValue) &amp;&amp; textValue.Length &gt;= 5;
        ((Entry)sender).TextColor = isValid ? Color.Default : Color.Red;
    }
}
</pre>
In this case, we are validating the input and modifying the UI if the number of characters entered is less than 5.

What about more articulated scenarios when multiple business rules are required to be checked for the input values?

In these cases, we could take advantage of other types available to make our code more structured and extensible:
<ul>
 	<li><strong>INotifyDataErrorInfo</strong>: available in Xamarin.Forms. When implemented it permits specifying custom validation supporting multiple errors per property, cross-property errors and entity-level errors;</li>
 	<li><strong>DataAnnotations</strong> decorate the data models using attributes which specify validation conditions to be applied to the specific field;</li>
 	<li><strong>Forms Behaviors</strong> specify the specific UI to be applied to the specific validation scenarios, integrating with INotifyDataErrorInfo and DataAnnotations.</li>
</ul>
To start exploring this approach, I created a new Xamarin.Forms Prism solution using the <a href="https://marketplace.visualstudio.com/items?itemName=BrianLagunas.PrismTemplatePack" target="_blank">Prism Template Pack</a> which generated the following project structure:

<img class="size-medium wp-image-8095 aligncenter" src="https://davide.dev/wp-content/uploads/2017/03/Capture-1-300x150.png" alt="" width="300" height="150" />

Then, I added the following new model to be validated using <strong>DataAnnotations</strong> and <strong>INotifyDataErrorInfo</strong>:
<pre title="DataAnntotations" class="lang:default decode:true ">using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using UsingValidation.Validation;

namespace UsingValidation.Models
{
    public class Item : ValidationBase
    {
        public Item()
        {
            Name = string.Empty;
            Description = string.Empty;
        }

        private string _name;
        private string _description;

        [Required(ErrorMessage = "Name cannot be empty!")]
        public string Name
        {
            get { return _name; }
            set
            {
                ValidateProperty(value);
                SetProperty(ref _name, value);
            }
        }

        [Required(ErrorMessage = "Description cannot be empty!")]
        [RegularExpression(@"\w{5,}", ErrorMessage = "Description: more than 4 letters/numbers required")]
        public string Description
        {
            get { return _description; }
            set
            {
                ValidateProperty(value);
                SetProperty(ref _description, value);
            }
        }

        protected override void ValidateProperty(object value, [CallerMemberName] string propertyName = null)
        {
            base.ValidateProperty(value, propertyName);

            OnPropertyChanged("IsSubmitEnabled");
        }

        public bool IsSubmitEnabled
        {
            get
            {
                return !HasErrors;
            }
        }
    }
}</pre>
The model uses attributes declared in the <strong>SystemComponentModel.DataAnnotations</strong> namespace which can be referenced in the solution modifying the Portable Class Library profile of the <strong>UsingValidation</strong> common project:

<img class="size-medium wp-image-8096 aligncenter" src="https://davidezordan.github.io/wp-content/uploads/2017/03/Capture-2-300x279.png" alt="" width="300" height="279" />

Quick tip: to be able to change the PCL profile I had to remove all the NuGet packages used by the common project, remove the Windows Phone 8 profile and then add back all the removed NuGet packages to the <strong>UsingValidation</strong> PCL.

To use the capability offered by <strong>INotifyDataErrorInfo</strong>, the model needs to implements 3 members defined in the interface:
<ul>
 	<li><strong>GetErrors()</strong> returns an IEnumerable sequence of strings containing the error messages triggered by validation;</li>
 	<li>the<strong> HasErrors </strong>property returns a boolean value indicating if there are validation errors;</li>
 	<li><strong>ErrorsChanged </strong>event can be triggered to Notify if the validation errors have been updated.</li>
</ul>
This interface is quite flexible and is designed to be customised depending on the different scenarios needed: I took as a starting point <a href="https://github.com/MaulikParmar/XamarinForms/tree/master/ValidationDemo" target="_blank">this implementation</a> available on GitHub and modified it accordingly: I decided to separate the implementation of <strong>INotifyDataErrorInfo</strong> in a different base class called <strong>ValidationBase</strong> which contains the following code using a <strong>Dictionary&lt;string, List&lt;string&gt;&gt;</strong> needed for storing the generated validation errors:
<pre title="ValidationBase class" class="lang:default decode:true">public class ValidationBase : BindableBase, INotifyDataErrorInfo
{
    private Dictionary&lt;string, List&lt;string&gt;&gt; _errors = new Dictionary&lt;string, List&lt;string&gt;&gt;();

    public ValidationBase()
    {
        ErrorsChanged += ValidationBase_ErrorsChanged;
    }

    private void ValidationBase_ErrorsChanged(object sender, DataErrorsChangedEventArgs e)
    {
        OnPropertyChanged("HasErrors");
        OnPropertyChanged("ErrorsList");
    }

    #region INotifyDataErrorInfo Members

    public event EventHandler&lt;DataErrorsChangedEventArgs&gt; ErrorsChanged;

    public IEnumerable GetErrors(string propertyName)
    {
		if (!string.IsNullOrEmpty(propertyName))
		{
			if (_errors.ContainsKey(propertyName) &amp;&amp; (_errors[propertyName].Any()))
			{
			   return _errors[propertyName].ToList();
			}
			else
			{
			   return new List&lt;string&gt;();
			}
		}
		else
		{
		   return _errors.SelectMany(err =&gt; err.Value.ToList()).ToList();
		}
    }

    public bool HasErrors
    {
        get
        {
	    return _errors.Any(propErrors =&gt; propErrors.Value.Any());
        }
    }

    #endregion
</pre>
The validation is performed by this method which evaluates the DataAnnotations decorating the model using the <strong>Validator</strong> available in the <strong>System.ComponentModel.DataAnnotations</strong> namespace and then stores the error messages in the dictionary:
<pre title="Performing validation" class="lang:default decode:true ">protected virtual void ValidateProperty(object value, [CallerMemberName] string propertyName = null)
{
    var validationContext = new ValidationContext(this, null)
    {
        MemberName = propertyName
    };

    var validationResults = new List&lt;ValidationResult&gt;();
    Validator.TryValidateProperty(value, validationContext, validationResults);
    RemoveErrorsByPropertyName(propertyName);

    HandleValidationResults(validationResults);
}</pre>
At this stage, I needed a solution for linking the UI to the model, and modifying the visuals depending on the presence or not of validation errors.

The ViewModel for the sample page, contains only a property storing an instance of the item defined in the model:
<pre title="MainPage ViewModel definition" class="lang:default decode:true ">public class MainPageViewModel : BindableBase, INavigationAware
{
    public MainPageViewModel()
    {
        DemoItem = new Item();
    }

    private Item _item;
    public Item DemoItem
    {
        get { return _item; }
        set { SetProperty(ref _item, value); }
    }

    public void OnNavigatedFrom(NavigationParameters parameters)
    {
    }

    public void OnNavigatedTo(NavigationParameters parameters)
    {
        DemoItem.Name = string.Empty;
        DemoItem.Description = string.Empty;
    }
}</pre>
Then, the corresponding XAML contains two Entry views used for input and a ListView used for showing the validation errors:
<pre title="MainPage XAML definition" class="lang:default decode:true ">&lt;?xml version="1.0" encoding="utf-8" ?&gt;
&lt;ContentPage xmlns="http://xamarin.com/schemas/2014/forms"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:prism="clr-namespace:Prism.Mvvm;assembly=Prism.Forms"
             xmlns:validation="clr-namespace:UsingValidation.Validation"
             xmlns:effects="clr-namespace:UsingValidation.Effects"             
             prism:ViewModelLocator.AutowireViewModel="True"
             x:Class="UsingValidation.Views.MainPage"
             Title="MainPage"&gt;  
  &lt;Grid VerticalOptions="Center"&gt;
……   
    &lt;Label Text ="Name: " VerticalTextAlignment="Center" Grid.Column="1" /&gt;   
    &lt;Entry Text="{Binding Name, Mode=TwoWay}" BindingContext="{Binding DemoItem}"
	 	   Grid.Column="2" HorizontalOptions="FillAndExpand"&gt;
		&lt;Entry.Behaviors&gt;
			&lt;validation:EntryValidationBehavior PropertyName="Name" /&gt;	
		&lt;/Entry.Behaviors&gt;
    &lt;/Entry&gt;

    &lt;Label Text ="Description: " VerticalTextAlignment="Center" Grid.Column="1" Grid.Row="2" /&gt;
    &lt;Entry Text="{Binding Description, Mode=TwoWay}" BindingContext="{Binding DemoItem}"
	 	   Grid.Column="2" HorizontalOptions="FillAndExpand" Grid.Row="2"&gt;
	&lt;Entry.Behaviors&gt;
		&lt;validation:EntryValidationBehavior PropertyName="Description" /&gt;
	&lt;/Entry.Behaviors&gt;
    &lt;/Entry&gt;

    &lt;ListView ItemsSource="{Binding DemoItem.ErrorsList}" 
              Grid.Row="4" Grid.Column="1" Grid.ColumnSpan="2" /&gt;

    &lt;Button Text="Submit" IsEnabled="{Binding DemoItem.IsSubmitEnabled}" 
            Grid.Row="6" Grid.Column="1" Grid.ColumnSpan="2" /&gt;
  &lt;/Grid&gt;
&lt;/ContentPage&gt;</pre>
The sample page uses a Behavior called <strong>EntryValidationBehavior</strong> which take care of changing the colour of the <strong>Entry</strong> background views in the case validation errors are present:
<pre title="EntryValidationBehavior" class="lang:default decode:true ">using System.Linq;
using UsingValidation.Effects;
using Xamarin.Forms;

namespace UsingValidation.Validation
{
    public class EntryValidationBehavior : Behavior&lt;Entry&gt;
    {
	private Entry _associatedObject;

        protected override void OnAttachedTo(Entry bindable)
        {
            base.OnAttachedTo(bindable);

            _associatedObject = bindable;

	    _associatedObject.TextChanged += _associatedObject_TextChanged;
        }

	void _associatedObject_TextChanged(object sender, TextChangedEventArgs e)
	{
	     var source = _associatedObject.BindingContext as ValidationBase;
	     if (source != null &amp;&amp; !string.IsNullOrEmpty(PropertyName))
	     {
		var errors = source.GetErrors(PropertyName).Cast&lt;string&gt;();
		if (errors != null &amp;&amp; errors.Any())
		{
                   var borderEffect = _associatedObject.Effects.FirstOrDefault(eff =&gt; eff is BorderEffect);
                    if (borderEffect == null)
                    {
                        _associatedObject.Effects.Add(new BorderEffect());
                    }

                    if (Device.OS != TargetPlatform.Windows)
                    {
                        _associatedObject.BackgroundColor = Color.Red;
                    }
                }
                else
		{
                    var borderEffect = _associatedObject.Effects.FirstOrDefault(eff =&gt; eff is BorderEffect);
                    if (borderEffect != null)
                    {
                        _associatedObject.Effects.Remove(borderEffect);
                    }

                    if (Device.OS != TargetPlatform.Windows)
                    {
                        _associatedObject.BackgroundColor = Color.Default;
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
}</pre>
The UI is also fine-tuned using a <a href="https://developer.xamarin.com/guides/xamarin-forms/effects/creating/" target="_blank">Xamarin.Forms effect</a> applied only to the UWP platform, in order to change the colour of the Entry border when validation errors occur:
<pre title="BorderEffect specific to UWP" class="lang:default decode:true ">using UsingValidation.UWP.Effects;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;

[assembly: ResolutionGroupName("UsingValidationSample")]
[assembly: ExportEffect(typeof(BorderEffect), "BorderEffect")]
namespace UsingValidation.UWP.Effects
{
    public class BorderEffect : PlatformEffect
    {
        Brush _previousBrush;
        Brush _previousBorderBrush;
        Brush _previousFocusBrush;
        FormsTextBox _control;

        protected override void OnAttached()
        {
            _control = Control as FormsTextBox;           
            if (_control != null)
            {
                _previousBrush = _control.Background;
                _previousFocusBrush = _control.BackgroundFocusBrush;
                _previousBorderBrush = _control.BorderBrush;
                _control.Background = new SolidColorBrush(Colors.Red);
                _control.BackgroundFocusBrush = new SolidColorBrush(Colors.Red);
                _control.BorderBrush = new SolidColorBrush(Colors.Red);                 
            }
        }

        protected override void OnDetached()
        {
            if (_control != null)
            {
                _control.Background = _previousBrush;
                _control.BackgroundFocusBrush = _previousFocusBrush;
                _control.BorderBrush = _previousBorderBrush;
            }
        }
    }
}</pre>
And this is the result when the application is executed on Android and UWP:

<img class="size-medium wp-image-8097 aligncenter" src="https://davide.dev/wp-content/uploads/2017/03/Capture-3-169x300.png" alt="" width="169" height="300" />

<img class="size-medium wp-image-8098 aligncenter" src="https://davide.dev/wp-content/uploads/2017/03/Capture-5-300x233.png" alt="" width="300" height="233" />

<strong>Conclusions</strong>

Xamarin.Forms provides a rich set of features for implementing validation: the usage of <strong>INotifyDataErrorInfo</strong>, <strong>Data Annotations</strong>, <strong>Behaviors</strong> and <strong>Effects</strong> permit the handling of complex scenarios including multiple conditions per field, cross-property validation, entity-level validation and custom UI depending on the platform.

Happy coding!
