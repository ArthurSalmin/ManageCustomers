using CustomerTracker.Model;
using GalaSoft.MvvmLight;

namespace CustomerTracker.ViewModel
{
    public class CityViewModel : ViewModelBase
    {
        private CityModel _cityModel;
        private string _name;

        public int Id { get; private set; }

        public string Name
        {
            get { return _name; }
            private set
            {
                _name = value;
                RaisePropertyChanged(nameof(Name));
            }
        }

        public CityViewModel(CityModel cityModel)
        {
            _cityModel = cityModel;
            _name = cityModel.Name;
            Id = cityModel.Id;
        }
    }
}
