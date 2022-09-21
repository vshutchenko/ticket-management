using RestEase;
using TicketManagement.Core.Clients.PurchaseApi.Models;

namespace TicketManagement.Core.Clients.PurchaseApi
{
    [Header("Content-Type", "application/json")]
    public interface IPurchaseClient
    {
        [Get("purchases/user/{id}")]
        public Task<List<PurchaseModel>> GetByUserIdAsync([Path] string id,
            [Header("Authorization")] string jwtToken,
            CancellationToken cancellationToken = default);

        [Post("purchases")]
        public Task PurchaseAsync([Body] PurchaseModel purchaseModel,
            [Header("Authorization")] string jwtToken,
            CancellationToken cancellationToken = default);
    }
}
