using CustomerTracker.ViewModel;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CustomerTracker.Views
{
    /// <summary>
    /// Interaction logic for MigrationCustomerDialog.xaml
    /// </summary>
    public partial class CustomerMigrationDialog : Window
    {
        private RelayCommand _saveCommand;
        public CustomerMigrationDialog()
        {
            InitializeComponent();
        }
        public RelayCommand SaveCommand
        {
            get
            {
                return _saveCommand = new RelayCommand(() =>
                {
                    (DataContext as CustomerMigrationDialogViewModel)?.SaveCustomer();
                    DialogResult = true;
                });
            }
        }
    }
}
