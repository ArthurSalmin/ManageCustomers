using System;
using CustomerTracker.Model;
using GalaSoft.MvvmLight;

namespace CustomerTracker.ViewModel
{
    public class CustomerViewModel : ViewModelBase
    {
        private CustomerModel _customerModel;
        private string _firstName;
        private string _name;
        private string _street;
        private DateTime _dateOfBirth;
        private CityViewModel _city;
        private int _id;

        public int Id
        {
            get { return _id; }
            set
            {
                _id = value;
                RaisePropertyChanged(nameof(Id));
            }
        }

        public string FirstName
        {
            get { return _firstName; }
            set
            {
                _firstName = value;
                RaisePropertyChanged(nameof(FirstName));
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChanged(nameof(Name));
            }
        }

        public string Street
        {
            get
            {
                if (_street != null)
                    return _street;
                else
                    return string.Empty;
            }
            set
            {
                _street = value;
                RaisePropertyChanged(nameof(Street));
            }
        }

        public DateTime DateOfBirth
        {
            get { return _dateOfBirth; }
            set
            {
                _dateOfBirth = value;
                RaisePropertyChanged(nameof(DateOfBirth));
            }
        }

        public CityViewModel City
        {
            get { return _city; }
            set
            {
                _city = value;
                RaisePropertyChanged(nameof(City));
            }
        }

        public string CityName
        {
            get
            {
                if (_city != null)
                {
                    if (_city.Name != "WithoutMigrations")
                        return _city.Name;
                    else
                        return "WithoutMigrations";
                }
                else
                {
                    return "WithoutMigrations";
                }
            }
        }

        public CustomerViewModel(CustomerModel customerModel, CityViewModel city)
        {
            _id = customerModel.Id;
            _customerModel = customerModel;
            _firstName = customerModel.FirstName;
            _name = customerModel.Name;
            _street = customerModel.Street;
            _dateOfBirth = customerModel.DateOfBirth;
            _city = city;
        }

        public void ReSaveModel()
        {
            _customerModel.FirstName = FirstName;
            _customerModel.Name = Name;
            _customerModel.DateOfBirth = DateOfBirth;
            _customerModel.Street = Street;
            _customerModel.CityId = 1;
        }

    }
}