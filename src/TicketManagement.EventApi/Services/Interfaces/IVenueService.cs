using TicketManagement.EventApi.Models;

namespace TicketManagement.EventApi.Services.Interfaces
{
    public interface IVenueService
    {
        IEnumerable<VenueModel> GetAll();

        Task<VenueModel> GetByIdAsync(int id);

        Task<int> CreateAsync(VenueModel venueModel);

        Task UpdateAsync(VenueModel venueModel);

        Task DeleteAsync(int id);
    }
}
