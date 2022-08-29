namespace TicketManagement.PurchaseApi.Models
{
    public class PurchasedSeatModel
    {
        public int Id { get; set; }

        public int PurchaseId { get; set; }

        public int EventSeatId { get; set; }

        public decimal Price { get; set; }
    }
}
