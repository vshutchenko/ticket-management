using RestEase;
using TicketManagement.Core.Clients.VenueApi.Models;

namespace TicketManagement.Core.Clients.VenueApi
{
    [Header("Content-Type", "application/json")]
    public interface ISeatClient
    {
        [Post("seats")]
        public Task CreateAsync([Body] SeatModel seatModel,
            [Header("Authorization")] string jwtToken,
            CancellationToken cancellationToken = default);

        [Put("seats")]
        public Task UpdateAsync([Body] SeatModel seatModel,
            [Header("Authorization")] string jwtToken,
            CancellationToken cancellationToken = default);

        [Delete("seats/{id}")]
        public Task DeleteAsync([Path] int id,
            [Header("Authorization")] string jwtToken,
            CancellationToken cancellationToken = default);

        [Get("seats/{id}")]
        public Task<SeatModel> GetByIdAsync([Path] int id,
            [Header("Authorization")] string jwtToken,
            CancellationToken cancellationToken = default);

        [Get("seats")]
        public Task<List<SeatModel>> GetAllAsync(
            [Header("Authorization")] string jwtToken,
            CancellationToken cancellationToken = default);
    }
}
