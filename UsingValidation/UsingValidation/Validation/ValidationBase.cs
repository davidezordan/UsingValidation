using Prism.Mvvm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;

namespace UsingValidation.Validation
{
    public class ValidationBase : BindableBase, INotifyDataErrorInfo
    {
        private Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();

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

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public IEnumerable GetErrors(string propertyName)
        {
			if (!string.IsNullOrEmpty(propertyName))
			{
				if (_errors.ContainsKey(propertyName) && (_errors[propertyName].Any()))
				{
					return _errors[propertyName];
				}
				else
				{
					return null;
				}
			}
			else
			{
				return _errors.SelectMany(err => err.Value.ToList());
			}
        }

        public bool HasErrors
        {
            get
            {
				return _errors.Any(propErrors => propErrors.Value.Any());
            }
        }

        #endregion

        protected virtual void ValidateProperty(object value, [CallerMemberName] string propertyName = null)
        {
            var validationContext = new ValidationContext(this, null)
            {
                MemberName = propertyName
            };

            var validationResults = new List<ValidationResult>();
            Validator.TryValidateProperty(value, validationContext, validationResults);

			RemoveErrorsByPropertyName(propertyName);

            HandleValidationResults(validationResults);
        }

		private void RemoveErrorsByPropertyName(string propertyName)
		{
			if (_errors.ContainsKey(propertyName))
			{
				_errors.Remove(propertyName);
			}

			RaiseErrorsChanged(propertyName);
		}

        private void HandleValidationResults(List<ValidationResult> validationResults)
        {
            var resultsByPropertyName = from results in validationResults
                                     	from memberNames in results.MemberNames
                                     	group results by memberNames into groups
                                     	select groups;
				
            foreach (var property in resultsByPropertyName)
            {
                _errors.Add(property.Key, property.Select(r => r.ErrorMessage).ToList());
                RaiseErrorsChanged(property.Key);
            }
        }

        private void RaiseErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        public IList<string> ErrorsList
        {
            get
            {
				return GetErrors(string.Empty).Cast<string>().ToList();
            }
        }
    }
}
