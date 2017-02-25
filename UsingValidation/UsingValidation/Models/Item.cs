using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using UsingValidation.Validation;

namespace UsingValidation.Models
{
    public class Item : ValidationBase, INotifyDataErrorInfo
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
        [RegularExpression(@"\w{9,}", ErrorMessage = "Description should contain more than 9 characters")]
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
