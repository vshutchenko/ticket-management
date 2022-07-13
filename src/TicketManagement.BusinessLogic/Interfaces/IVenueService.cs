using System.Collections.Generic;
using System.Threading.Tasks;
using TicketManagement.BusinessLogic.Models;

namespace TicketManagement.BusinessLogic.Interfaces
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
