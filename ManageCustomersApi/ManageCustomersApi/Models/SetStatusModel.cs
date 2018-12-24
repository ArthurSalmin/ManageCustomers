using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageCustomersApi.Models
{
    public class SetStatusModel
    {
        public int IdCustomer { get; set; }
        public string Status { get; set; }
        public int? IdLockedCustomer { get; set; }
    }
}
