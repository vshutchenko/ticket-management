using System.Collections.Generic;

namespace TicketManagement.BusinessLogic.Models
{
    public class PurchaseModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int EventId { get; set; }
        public decimal Price { get; set; }
        public List<int> SeatIds { get; set; } = new List<int>();
    }
}
