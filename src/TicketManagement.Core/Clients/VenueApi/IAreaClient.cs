using RestEase;
using TicketManagement.Core.Clients.VenueApi.Models;

namespace TicketManagement.Core.Clients.VenueApi
{
    [Header("Content-Type", "application/json")]
    public interface IAreaClient
    {
        [Post("areas")]
        public Task CreateAsync([Body] AreaModel areaModel,
            [Header("Authorization")] string jwtToken,
            CancellationToken cancellationToken = default);

        [Put("areas")]
        public Task UpdateAsync([Body] AreaModel areaModel,
            [Header("Authorization")] string jwtToken,
            CancellationToken cancellationToken = default);

        [Delete("areas/{id}")]
        public Task DeleteAsync([Path] int id,
            [Header("Authorization")] string jwtToken,
            CancellationToken cancellationToken = default);

        [Get("areas/{id}")]
        public Task<AreaModel> GetByIdAsync([Path] int id,
            [Header("Authorization")] string jwtToken,
            CancellationToken cancellationToken = default);

        [Get("areas/layouts/{layoutId}")]
        public Task<List<AreaModel>> GetByLayoutIdAsync([Path] int layoutId,
            [Header("Authorization")] string jwtToken,
            CancellationToken cancellationToken = default);

        [Get("areas")]
        public Task<List<AreaModel>> GetAllAsync(
            [Header("Authorization")] string jwtToken,
            CancellationToken cancellationToken = default);
    }
}
