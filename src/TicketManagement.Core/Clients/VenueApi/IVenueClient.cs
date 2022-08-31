using RestEase;
using TicketManagement.Core.Clients.VenueApi.Models;

namespace TicketManagement.Core.Clients.VenueApi
{
    [Header("Content-Type", "application/json")]
    public interface IVenueClient
    {
        [Post("venues")]
        public Task CreateAsync([Body] VenueModel venueModel,
            [Header("Authorization")] string jwtToken,
            CancellationToken cancellationToken = default);

        [Put("venues")]
        public Task UpdateAsync([Body] VenueModel venueModel,
            [Header("Authorization")] string jwtToken,
            CancellationToken cancellationToken = default);

        [Delete("venues/{id}")]
        public Task DeleteAsync([Path] int id,
            [Header("Authorization")] string jwtToken,
            CancellationToken cancellationToken = default);

        [Get("venues/{id}")]
        public Task<VenueModel> GetByIdAsync([Path] int id,
            [Header("Authorization")] string jwtToken,
            CancellationToken cancellationToken = default);

        [Get("venues")]
        public Task<List<VenueModel>> GetAllAsync(
            [Header("Authorization")] string jwtToken,
            CancellationToken cancellationToken = default);
    }
}
