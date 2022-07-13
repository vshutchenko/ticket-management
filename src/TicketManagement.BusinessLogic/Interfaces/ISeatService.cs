using System.Collections.Generic;
using System.Threading.Tasks;
using TicketManagement.BusinessLogic.Models;

namespace TicketManagement.BusinessLogic.Interfaces
{
    public interface ISeatService
    {
        IEnumerable<SeatModel> GetAll();

        Task<SeatModel> GetByIdAsync(int id);

        Task<int> CreateAsync(SeatModel seatModel);

        Task UpdateAsync(SeatModel seatModel);

        Task DeleteAsync(int id);
    }
}
