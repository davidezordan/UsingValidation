using System.ComponentModel.DataAnnotations;
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
}
