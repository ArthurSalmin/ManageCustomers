using ManageCustomersApi.Models;
using System.Threading.Tasks;

namespace ManageCustomersApi.Interfaces
{
    public interface ICustomerRepository : IBaseRepository<CustomerModel>
    {
        Task<SetStatusModel> SetStatus(int idCustomer, string status, int? IdUserLocked);
        Task<SetStatusModel> GetStatus(int idCustomer);
    }
}
