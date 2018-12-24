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
using GalaSoft.MvvmLight.CommandWpf;

namespace CustomerTracker.Views
{
    /// <summary>
    /// Interaction logic for LockedCustomerDialog.xaml
    /// </summary>
    public partial class LockedCustomerDialog : Window
    {
        private RelayCommand _readonlyCommand;
        private RelayCommand _forceEditCommand;
        private RelayCommand _cancelCommand;

        public LockedCustomerDialog()
        {
            InitializeComponent();
        }

        public RelayCommand ReadonlyCommand
        {
            get {
                return _readonlyCommand = new RelayCommand(() => {
                    DialogResult = true;
                });
            }
        }

        public RelayCommand ForceEditCommand
        {
            get
            {
                return _forceEditCommand = new RelayCommand(() => {
                    DialogResult = false;
                });
            }
        }

        public RelayCommand CancelCommand
        {
            get
            {
                return _cancelCommand = new RelayCommand(() => {
                    this.Close();
                });
            }
        }
    }
}
