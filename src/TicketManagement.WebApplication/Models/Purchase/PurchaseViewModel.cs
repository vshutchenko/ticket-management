namespace TicketManagement.WebApplication.Models.Purchase
{
    public class PurchaseViewModel
    {
        public int Id { get; set; }

        public List<PurchasedSeatViewModel> PurchasedSeats { get; set; } = new List<PurchasedSeatViewModel>();

        public decimal Price { get; set; }
    }
}
