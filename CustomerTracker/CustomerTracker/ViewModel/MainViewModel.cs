using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using CustomerTracker.Model;
using CustomerTracker.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;

namespace CustomerTracker.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private ObservableCollection<CustomerViewModel> _customers = new ObservableCollection<CustomerViewModel>();
        private ObservableCollection<CityViewModel> _cities = new ObservableCollection<CityViewModel>();
        private RelayCommand _addCommand;
        private RelayCommand _editCommand;
        private RelayCommand _removeCommand;
        private CustomerViewModel _selectedCustomer;
        private ICollectionView _customerView;
        private string _filterString = string.Empty;
        private static HttpClient client = new HttpClient();

        public ObservableCollection<CustomerViewModel> Customers
        {
            get { return _customers; }
            set
            {
                _customers = value;
                RaisePropertyChanged(nameof(Customers));
            }
        }

        public ObservableCollection<CityViewModel> Cities
        {
            get { return _cities; }
            set
            {
                _cities = value;
                RaisePropertyChanged(nameof(Cities));
            }
        }

        public ICollectionView CustomersView
        {
            get { return _customerView; }
            set { }
        }

        public string FilterString
        {
            get { return _filterString; }
            set
            {
                _filterString = value;
                _customerView.Refresh();
                RaisePropertyChanged(nameof(FilterString));
            }
        }

        public CustomerViewModel SelectedCustomer
        {
            get { return _selectedCustomer; }
            set
            {
                _selectedCustomer = value;
                RaisePropertyChanged(nameof(SelectedCustomer));
            }
        }

        public RelayCommand AddCustomer
        {
            get
            {
                return _addCommand = new RelayCommand(async () =>
                {
                    var vm = new CustomerDialogViewModel(new CustomerViewModel(new CustomerModel() { DateOfBirth = DateTime.Today }, _cities.FirstOrDefault()));
                    var customerDialog = new CustomerDialog() { Owner = Application.Current.MainWindow, DataContext = vm };
                    if (customerDialog.ShowDialog() == true)
                    {
                        CustomerModel newCustomer = new CustomerModel
                        {
                            Id = Customers.Count + 1,
                            Name = vm.CustomerViewModel.Name,
                            FirstName = vm.CustomerViewModel.FirstName,
                            DateOfBirth = vm.CustomerViewModel.DateOfBirth,
                            Street = vm.CustomerViewModel.Street,
                            CityId = vm.SelectedCity.Id
                        };
                        var result = await CreateCustomerAsync(newCustomer);
                        if (result == HttpStatusCode.OK)
                        {
                            vm.CustomerViewModel.Id = newCustomer.Id;
                            App.Current.Dispatcher.Invoke((System.Action)delegate
                            {
                                _customers.Add(vm.CustomerViewModel);
                            });
                            SelectedCustomer = vm.CustomerViewModel;
                        }
                        else
                        {
                            MessageBox.Show($"Can't add new customer", "Error", MessageBoxButton.OK);
                        }
                    }

                });
            }
        }

        public RelayCommand EditCustomer
        {
            get
            {
                return _editCommand = new RelayCommand(async () =>
                {
                    var vm = new CustomerDialogViewModel(SelectedCustomer);
                    string customerStatus = await GetStatusCode(SelectedCustomer.Id);
                    if (customerStatus == "\"online\"")
                    {
                        await SetStatus(SelectedCustomer.Id, "busy");
                        var customerDialog = new CustomerDialog() { Owner = Application.Current.MainWindow, DataContext = vm };

                        if (customerDialog.ShowDialog() == true)
                        {
                            //string finalCustomerStatus = await GetStatusCode(SelectedCustomer.Id);
                            CustomerModel newCustomer = new CustomerModel
                            {
                                Id = vm.CustomerViewModel.Id,
                                Name = vm.CustomerViewModel.Name,
                                FirstName = vm.CustomerViewModel.FirstName,
                                DateOfBirth = vm.CustomerViewModel.DateOfBirth,
                                Street = vm.CustomerViewModel.Street,
                                CityId = vm.SelectedCity.Id
                            };

                            newCustomer = await UpdateCustomerAsync(newCustomer);
                            
                            if (newCustomer == null)
                            {
                                MessageBox.Show($"Can't edit customer", "Error", MessageBoxButton.OK);
                            }
                        }
                        await SetStatus(SelectedCustomer.Id, "online");
                        await GetCustomers();
                    }
                    else
                    {
                        MessageBoxResult result = MessageBox.Show("This customer is busy at the moment, try again later", "Error");
                        //var customerDialog = new LockedCustomerDialog() { Owner = Application.Current.MainWindow};
                        //if (customerDialog.ShowDialog() == true)
                        //{
                        //    string s = "todo create dialog with vm values readonly";
                        //}
                        //else if (customerDialog.ShowDialog() == false)
                        //{
                        //    string s = "todo force edit vm";
                        //}
                        
                    }

                }, () => SelectedCustomer != null);
            }
        }

        public RelayCommand RemoveCustomer
        {
            get
            {
                return _removeCommand = new RelayCommand(async () =>
                {
                    var response = MessageBox.Show($"Are you realy want to remove {SelectedCustomer.Name}?", "Remove Customer", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (response == MessageBoxResult.Yes)
                    {
                        var result = await DeleteCustomerAsync(SelectedCustomer.Id);
                        if (result == HttpStatusCode.OK)
                        {
                            _customers.Remove(SelectedCustomer);
                        }
                        else
                        {
                            MessageBox.Show($"Can't delete customer", "Error", MessageBoxButton.OK);
                        }
                    }
                }, () => SelectedCustomer != null);
            }
        }

        public MainViewModel()
        {
            RunsAsync().GetAwaiter().GetResult();
        }

        private void SetClient()
        {
            client.BaseAddress = new Uri("https://localhost:44327/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        private async Task RunsAsync()
        {
            SetClient();

            List<CityModel> cityModels = await GetCities();
            List<CustomerModel> customerModels = await GetCustomers();
            foreach (var item in cityModels)
            {
                _cities.Add(new CityViewModel(item));
            }

            foreach (var item in customerModels)
            {
                _customers.Add(new CustomerViewModel(item, _cities.FirstOrDefault(x => x.Id == item.CityId)));
            }

            _customerView = CollectionViewSource.GetDefaultView(_customers);
            _customerView.GroupDescriptions.Add(new PropertyGroupDescription(nameof(SelectedCustomer.CityName)));
            _customerView.Filter = o =>
            {
                var item = o as CustomerViewModel;

                var filter = FilterString.ToLower();

                if (item != null && (item.Name.ToLower().Contains(filter) || item.FirstName.ToLower().Contains(filter)))
                {
                    return true;
                }

                return false;
            };
        }

        private async Task<List<CustomerModel>> GetCustomers()
        {
            List<CustomerModel> allCustomers;
            string path = "api/v1/customers";
            HttpResponseMessage response = client.GetAsync(path).Result;
            if (response.IsSuccessStatusCode)
            {
                var customers = await response.Content.ReadAsAsync<IEnumerable<CustomerModel>>();
                allCustomers = new List<CustomerModel>(customers.ToList());
                return allCustomers;
            }
            else
                return null;
        }

        private async Task<List<CityModel>> GetCities()
        {
            List<CityModel> allCities;
            string path = "api/v1/cities";
            HttpResponseMessage response = client.GetAsync(path).Result;
            if (response.IsSuccessStatusCode)
            {
                var cities = await response.Content.ReadAsAsync<IEnumerable<CityModel>>();
                allCities = new List<CityModel>(cities.ToList());
                return allCities;
            }
            else
                return null;
        }

        private async Task<string> GetStatusCode(int idCustomer)
        {
            string path = $"api/v1/customers/getStatus/{idCustomer}";
            HttpResponseMessage response = client.GetAsync(path).Result;
            string status = null;
            if (response.IsSuccessStatusCode)
            {
                status = await response.Content.ReadAsStringAsync();
            }
            return status;
        }

        private async Task<SetStatusModel> SetStatus(int idSelectedCustomer, string status)
        {
            SetStatusModel statusModel;
            if (status == "busy")
            {
                Random rnd = new Random();
                statusModel = new SetStatusModel
                {
                    IdCustomer = SelectedCustomer.Id,
                    Status = status,
                    IdLockedCustomer = rnd.Next(200)
                };
            }
            else if (status == "online")
            {
                statusModel = new SetStatusModel
                {
                    IdCustomer = SelectedCustomer.Id,
                    Status = status,
                    IdLockedCustomer = null
                };
            }
            else
            {
                throw new Exception("Set correct status!!!");
            }

            HttpResponseMessage response = await client.PutAsJsonAsync(
                "api/v1/customers/setStatus", statusModel);
            response.EnsureSuccessStatusCode();

            statusModel = await response.Content.ReadAsAsync<SetStatusModel>();
            return statusModel;
        }

        private async Task<CustomerModel> UpdateCustomerAsync(CustomerModel customer)
        {
            HttpResponseMessage response = await client.PutAsJsonAsync(
                "api/v1/customers", customer);
            response.EnsureSuccessStatusCode();

            customer = await response.Content.ReadAsAsync<CustomerModel>();
            return customer;
        }

        private async Task<HttpStatusCode> DeleteCustomerAsync(int id)
        {
            HttpResponseMessage response = await client.DeleteAsync(
                $"api/v1/customers/{id}");
            response.EnsureSuccessStatusCode();
            return response.StatusCode;
        }

        private async Task<HttpStatusCode> CreateCustomerAsync(CustomerModel customer)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync(
                "api/v1/customers/create", customer);
            response.EnsureSuccessStatusCode();
            return response.StatusCode;
        }
    }
}