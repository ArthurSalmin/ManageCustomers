using ManageCustomersApi.Models;
using System.Threading.Tasks;

namespace ManageCustomersApi.Interfaces
{
    public interface ICustomerRepository : IBaseRepository<CustomerModel>
    {
        Task<SetStatusModel> SetStatus(int idCustomer, string status, int? idLockedCustomer);
        Task<string> GetStatus(int idCustomer);
    }
}
