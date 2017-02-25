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
        private object _lock = new Object();
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
                if (_errors.ContainsKey(propertyName) && (_errors[propertyName] != null) &&
                    _errors[propertyName].Count > 0)
                    return _errors[propertyName].ToList();
                else
                    return null;
            }
            else
                return _errors.SelectMany(err => err.Value.ToList());
        }

        public bool HasErrors
        {
            get
            {
                return _errors.Any(propErrors => propErrors.Value != null && propErrors.Value.Count > 0);
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

            if (_errors.ContainsKey(propertyName))
            {
                _errors.Remove(propertyName);
            }

            RaiseErrorsChanged(propertyName);
            HandleValidationResults(validationResults);
        }

        private void HandleValidationResults(List<ValidationResult> validationResults)
        {
            var resultsByPropNames = from res in validationResults
                                     from mname in res.MemberNames
                                     group res by mname into g
                                     select g;

            foreach (var prop in resultsByPropNames)
            {
                var messages = prop.Select(r => r.ErrorMessage).ToList();
                _errors.Add(prop.Key, messages);
                RaiseErrorsChanged(prop.Key);
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
                if (_errors != null && _errors.Count > 0)
                {
                    return _errors.Select(kp => kp.Value).First();
                }
                return null;
            }
        }
    }
}
