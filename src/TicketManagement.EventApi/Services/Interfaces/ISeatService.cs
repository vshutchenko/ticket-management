using TicketManagement.EventApi.Models;

namespace TicketManagement.EventApi.Services.Interfaces
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
