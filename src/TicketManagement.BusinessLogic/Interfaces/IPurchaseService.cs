using System.Collections.Generic;
using System.Threading.Tasks;
using TicketManagement.BusinessLogic.Models;

namespace TicketManagement.BusinessLogic.Interfaces
{
    public interface IPurchaseService
    {
        Task PurchaseSeatAsync(PurchaseModel model);

        IEnumerable<PurchaseModel> GetByUserId(string userId);

        IEnumerable<EventSeatModel> GetByPurchaseId(int purchaseId);
    }
}
