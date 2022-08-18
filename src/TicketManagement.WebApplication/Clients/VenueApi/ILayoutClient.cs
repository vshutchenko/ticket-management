using RestEase;
using TicketManagement.WebApplication.Clients.VenueApi.Models;

namespace TicketManagement.WebApplication.Clients.VenueApi
{
    [Header("Content-Type", "application/json")]
    public interface ILayoutClient
    {
        [Post("layouts")]
        public Task CreateAsync([Body] LayoutModel venueModel,
            [Header("Authorization")] string jwtToken,
            CancellationToken cancellationToken = default);

        [Put("layouts")]
        public Task UpdateAsync([Body] LayoutModel venueModel,
            [Header("Authorization")] string jwtToken,
            CancellationToken cancellationToken = default);

        [Delete("layouts/{id}")]
        public Task DeleteAsync([Path] int id,
            [Header("Authorization")] string jwtToken,
            CancellationToken cancellationToken = default);

        [Get("layouts/{id}")]
        public Task<LayoutModel> GetByIdAsync([Path] int id,
            [Header("Authorization")] string jwtToken,
            CancellationToken cancellationToken = default);

        [Get("layouts/{venueId}")]
        public Task<List<LayoutModel>> GetByVenueIdAsync([Path] int venueId,
            [Header("Authorization")] string jwtToken,
            CancellationToken cancellationToken = default);

        [Get("layouts")]
        public Task<List<LayoutModel>> GetAllAsync(
            [Header("Authorization")] string jwtToken,
            CancellationToken cancellationToken = default);
    }
}
