using TicketManagement.PurchaseApi.Models;

namespace TicketManagement.PurchaseApi.Services.Interfaces
{
    public interface IPurchaseService
    {
        Task PurchaseSeatAsync(PurchaseModel model);

        IEnumerable<PurchaseModel> GetByUserId(string userId);

        IEnumerable<EventSeatModel> GetByPurchaseId(int purchaseId);
    }
}
