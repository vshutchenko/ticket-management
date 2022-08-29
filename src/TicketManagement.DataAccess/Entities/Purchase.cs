using System.ComponentModel.DataAnnotations.Schema;

namespace TicketManagement.DataAccess.Entities
{
    [Table("Purchase")]
    public class Purchase
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public int EventId { get; set; }

        [Column(TypeName = "decimal(16,2)")]
        public decimal Price { get; set; }
    }
}
