using System.Collections.Generic;
using System.Threading.Tasks;
using TicketManagement.DataAccess.Entities;

namespace TicketManagement.BusinessLogic.Interfaces
{
    internal interface IVenueService
    {
        IEnumerable<Venue> GetAll();
        Task<Venue> GetByIdAsync(int id);
        Task<int> CreateAsync(Venue venue);
        Task UpdateAsync(Venue venue);
        Task DeleteAsync(int id);
    }
}
