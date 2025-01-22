using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        private Dictionary<string, string> _validationErrors = new Dictionary<string, string>();

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value)) return false;
            storage = value;
            OnPropertyChanged(propertyName);
            Validate(propertyName);
            return true;
        }

        protected bool IsValid(string propertyName)
        {
            return !_validationErrors.ContainsKey(propertyName);
        }

        protected void AddValidationError(string propertyName, string errorMessage)
        {
            if (!_validationErrors.ContainsKey(propertyName))
                _validationErrors.Add(propertyName, errorMessage);
        }

        protected void RemoveValidationError(string propertyName)
        {
            if (_validationErrors.ContainsKey(propertyName))
                _validationErrors.Remove(propertyName);
        }

        protected virtual void Validate(string propertyName)
        {
            // Validation logic based on property
        }

        public bool HasValidationErrors => _validationErrors.Count > 0;



    }
}
