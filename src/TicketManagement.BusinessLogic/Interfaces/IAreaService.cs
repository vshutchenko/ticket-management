using System.Collections.Generic;
using System.Threading.Tasks;
using TicketManagement.BusinessLogic.Models;

namespace TicketManagement.BusinessLogic.Interfaces
{
    public interface IAreaService
    {
        IEnumerable<AreaModel> GetAll();
        Task<AreaModel> GetByIdAsync(int id);
        Task<int> CreateAsync(AreaModel areaModel);
        Task UpdateAsync(AreaModel areaModel);
        Task DeleteAsync(int id);
    }
}
