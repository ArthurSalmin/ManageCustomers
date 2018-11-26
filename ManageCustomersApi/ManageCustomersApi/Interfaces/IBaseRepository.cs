using System.Collections.Generic;
using System.Threading.Tasks;

namespace ManageCustomersApi.Interfaces
{
    public interface IBaseRepository<T> 
    {
        Task<T> GetAsync(int id);
        Task<List<T>> GetAllAsync();
        Task<T> PostAsync(T obj);
        Task<bool> DeleteAsync(int id);
        Task<T> PutAsync(T obj);
    }
}
