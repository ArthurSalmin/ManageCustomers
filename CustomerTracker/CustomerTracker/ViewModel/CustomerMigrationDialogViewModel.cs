using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerTracker.ViewModel
{
    public class CustomerMigrationDialogViewModel : ViewModelBase, IDataErrorInfo
    {
        private CustomerViewModel _customerModel;
        private string _street;
        private CityViewModel _selectedCity;
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

        public string Street
        {
            get { return _street; }
            set
            {
                _street = value;
                CanSave = true;
                RaisePropertyChanged(nameof(Street));
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

        public CityViewModel SelectedCity
        {
            get { return _selectedCity; }
            set
            {
                _selectedCity = value;
                RaisePropertyChanged(nameof(SelectedCity));
            }
        }


        public RelayCommand ResetCommand
        {
            get
            {
                return _resetCommand = new RelayCommand(() =>
                {
                    Street = _customerModel.Street;
                    SelectedCity = _customerModel.City;
                    RaisePropertyChanged(nameof(SelectedCity));
                });
            }
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
                    
                    case nameof(Street):
                        if (string.IsNullOrEmpty(Street))
                        {
                            error = "Input field can't contain symbols or numbers";
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

        public CustomerMigrationDialogViewModel(CustomerViewModel customerViewModel)
        {
            _customerModel = customerViewModel;
            _street = customerViewModel.Street;
            SelectedCity = customerViewModel.City;
        }

        /// <summary>
        /// Save model for sending to the server
        /// </summary>
        public void SaveCustomer()
        {
            _customerModel.Street = Street;
            _customerModel.City = SelectedCity;
            _customerModel.ReSaveModel();
        }
    }
}
