using System;
using System.ComponentModel;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace CustomerTracker.ViewModel
{
    public class CustomerDialogViewModel : ViewModelBase, IDataErrorInfo
    {
        private CustomerViewModel _customerModel;
        private string _firstName;
        private string _name;
        private DateTime _dateOfBirth;
        private RelayCommand _resetCommand;
        private bool _canSave;
        private bool _readonly;

        public bool Readonly
        {
            get { return _readonly; }
            set
            {
                _readonly = value;
                RaisePropertyChanged(nameof(Readonly));
            }
        }

        public string FirstName
        {
            get { return _firstName; }
            set
            {
                _firstName = value;
                CanSave = true;
                RaisePropertyChanged(nameof(FirstName));
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                CanSave = true;
                RaisePropertyChanged(nameof(Name));
            }
        }
        
        public DateTime DateOfBirth
        {
            get { return _dateOfBirth; }
            set
            {
                _dateOfBirth = value;
                CanSave = true;
                RaisePropertyChanged(nameof(DateOfBirth));
            }
        }

        public DateTime StartDateTime
        {
            get { return new DateTime(1920, 1, 1); }
            set { throw new NotImplementedException(); }
        }

        public DateTime EndDateTime
        {
            get { return DateTime.Today; }
            set { throw new NotImplementedException(); }
        }
        
        public RelayCommand ResetCommand
        {
            get { return _resetCommand = new RelayCommand(() =>
            {
                FirstName = _customerModel.FirstName;
                Name = _customerModel.Name;
                DateOfBirth = _customerModel.DateOfBirth;
                RaisePropertyChanged(nameof(DateOfBirth));
            }); }
        }

        /// <summary>
        /// Validate input information
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public string this[string columnName]
        {
            get
            {
                string error = String.Empty;
                switch (columnName)
                {
                    case nameof(FirstName):
                        if (string.IsNullOrEmpty(FirstName) || !FirstName.All(Char.IsLetter))
                        {
                            error = "Input field can't contain symbols or numbers";
                            CanSave = false;
                        }
                        break;
                    case nameof(Name):
                        if (string.IsNullOrEmpty(Name) || !Name.All(Char.IsLetter))
                        {
                            error = "Input field can't contain symbols or numbers";
                            CanSave = false;
                        }
                        break;
                    
                    case nameof(DateOfBirth):
                        if (DateOfBirth.Date < StartDateTime.Date || DateOfBirth.Date > EndDateTime.Date)
                        {
                            error = "Choose the correct date";
                            CanSave = false;
                        }
                        break;
                }

                return error;
            }
        }

        public CustomerViewModel CustomerViewModel
        {
            get { return _customerModel; }
            set
            {
                _customerModel = value;
                RaisePropertyChanged(nameof(CustomerViewModel));
            }
        }


        public bool CanSave
        {
            get { return _canSave; }
            set
            {
                _canSave = value;
                RaisePropertyChanged(nameof(CanSave));
            }
        }

        public string Error { get; }

        public CustomerDialogViewModel(CustomerViewModel customerViewModel)
        {
            _customerModel = customerViewModel;
            _firstName = customerViewModel.FirstName;
            _name = customerViewModel.Name;
            _dateOfBirth = customerViewModel.DateOfBirth;
        }

        /// <summary>
        /// Save model for sending to the server
        /// </summary>
        public void SaveCustomer()
        {
            _customerModel.FirstName = FirstName;
            _customerModel.Name = Name;
            _customerModel.DateOfBirth = DateOfBirth;

            _customerModel.ReSaveModel();
        }
    }
}
