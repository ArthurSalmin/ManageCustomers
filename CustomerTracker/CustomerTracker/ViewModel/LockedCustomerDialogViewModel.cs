using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerTracker.ViewModel
{
    class LockedCustomerDialogViewModel
    {
        private RelayCommand _cancelCommand;

        public RelayCommand CancelCommand
        {
            get{ return _cancelCommand = new RelayCommand(()=> 
            {


            }); }
        }
    }
}
