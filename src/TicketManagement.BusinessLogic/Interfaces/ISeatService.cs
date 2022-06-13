using System.Collections.Generic;
using System.Threading.Tasks;
using TicketManagement.DataAccess.Entities;

namespace TicketManagement.BusinessLogic.Interfaces
{
    public interface ISeatService
    {
        IEnumerable<Seat> GetAll();
        Task<Seat> GetByIdAsync(int id);
        Task<int> CreateAsync(Seat seat);
        Task UpdateAsync(Seat seat);
        Task DeleteAsync(int id);
    }
}
