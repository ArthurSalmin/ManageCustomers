using System.Windows;
using CustomerTracker.ViewModel;
using GalaSoft.MvvmLight.CommandWpf;

namespace CustomerTracker.Views
{
    /// <summary>
    /// Interaction logic for CustomerDialog.xaml
    /// </summary>
    public partial class CustomerDialog : Window
    {
        private RelayCommand _saveCommand;
        
        public CustomerDialog()
        {
            InitializeComponent();
        }
        
        public RelayCommand SaveCommand
        {
            get { return _saveCommand = new RelayCommand(() =>
            {
                (DataContext as CustomerDialogViewModel)?.SaveCustomer();
                DialogResult = true;
            }); }
        }

    }
}
