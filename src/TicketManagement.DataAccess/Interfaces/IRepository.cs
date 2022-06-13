using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("TicketManagement.IntegrationTests")]
[assembly: InternalsVisibleTo("TicketManagement.UnitTests")]

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
