using System.ComponentModel.DataAnnotations.Schema;

namespace TicketManagement.DataAccess.Entities
{
    [Table("Layout")]
    public class Layout
    {
        public int Id { get; set; }

        public int VenueId { get; set; }

        public string Description { get; set; }
    }
}
