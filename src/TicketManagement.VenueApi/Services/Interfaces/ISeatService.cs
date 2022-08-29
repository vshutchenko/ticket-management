using TicketManagement.VenueApi.Models;

namespace TicketManagement.VenueApi.Services.Interfaces
{
    public interface ISeatService
    {
        IEnumerable<SeatModel> GetAll();

        Task<IEnumerable<SeatModel>> GetByAreaIdAsync(int areaId);

        Task<SeatModel> GetByIdAsync(int id);

        Task<int> CreateAsync(SeatModel seatModel);

        Task UpdateAsync(SeatModel seatModel);

        Task DeleteAsync(int id);
    }
}
