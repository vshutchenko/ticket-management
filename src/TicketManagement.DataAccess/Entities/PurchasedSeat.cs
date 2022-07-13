using System.ComponentModel.DataAnnotations.Schema;

namespace TicketManagement.DataAccess.Entities
{
    [Table("PurchasedSeat")]
    public class PurchasedSeat
    {
        public int Id { get; set; }

        public int PurchaseId { get; set; }

        public int EventSeatId { get; set; }
    }
}
