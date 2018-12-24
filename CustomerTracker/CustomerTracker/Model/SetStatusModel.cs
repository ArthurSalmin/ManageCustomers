using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerTracker.Model
{
    class SetStatusModel
    {
        public int IdCustomer { get; set; }
        public string Status { get; set; }
        public int? IdLockedCustomer { get; set; }
    }
}
