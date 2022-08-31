using RestEase;
using TicketManagement.Core.Clients.PurchaseApi.Models;

namespace TicketManagement.Core.Clients.PurchaseApi
{
    [Header("Content-Type", "application/json")]
    public interface IPurchaseClient
    {
        [Get("purchases/user/{userId}")]
        public Task<List<PurchaseModel>> GetByUserIdAsync([Path] string userId,
            [Header("Authorization")] string jwtToken,
            CancellationToken cancellationToken = default);

        [Post("purchases")]
        public Task PurchaseAsync([Body] PurchaseModel purchaseModel,
            [Header("Authorization")] string jwtToken,
            CancellationToken cancellationToken = default);
    }
}
