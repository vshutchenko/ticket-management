using TicketManagement.VenueApi.Models;

namespace TicketManagement.VenueApi.Services.Interfaces
{
    public interface ILayoutService
    {
        IEnumerable<LayoutModel> GetAll();

        Task<IEnumerable<LayoutModel>> GetByVenueIdAsync(int venueId);

        Task<LayoutModel> GetByIdAsync(int id);

        Task<int> CreateAsync(LayoutModel layoutModel);

        Task UpdateAsync(LayoutModel layoutModel);

        Task DeleteAsync(int id);
    }
}
