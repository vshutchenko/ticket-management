using TicketManagement.EventApi.Models;

namespace TicketManagement.EventApi.Services.Interfaces
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
