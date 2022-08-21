using System.Linq;
using System.Threading.Tasks;

namespace TicketManagement.DataAccess.Interfaces
{
    public interface IRepository<T>
    {
        IQueryable<T> GetAll();

        Task<T> GetByIdAsync(int id);

        Task<int> CreateAsync(T item);

        Task UpdateAsync(T item);

        Task DeleteAsync(int id);
    }
}
