using System.ComponentModel.DataAnnotations.Schema;

namespace TicketManagement.DataAccess.Entities
{
    [Table("Venue")]
    public class Venue
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public string Address { get; set; }

        public string Phone { get; set; }
    }
}
