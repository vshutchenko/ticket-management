namespace TicketManagement.WebApplication.Models.Purchase
{
    public class CreatePurchaseViewModel
    {
        public int Id { get; set; }

        public int EventId { get; set; }

        public string? UserId { get; set; }

        public List<int> SeatIds { get; set; } = new List<int>();
    }
}
