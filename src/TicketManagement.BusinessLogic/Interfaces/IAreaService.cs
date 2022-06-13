using System.Collections.Generic;
using System.Threading.Tasks;
using TicketManagement.DataAccess.Entities;

namespace TicketManagement.BusinessLogic.Interfaces
{
    public interface IAreaService
    {
        IEnumerable<Area> GetAll();
        Task<Area> GetByIdAsync(int id);
        Task<int> CreateAsync(Area area);
        Task UpdateAsync(Area area);
        Task DeleteAsync(int id);
    }
}
