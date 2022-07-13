using System.ComponentModel.DataAnnotations.Schema;

namespace TicketManagement.DataAccess.Entities
{
    [Table("EventSeat")]
    public class EventSeat
    {
        public int Id { get; set; }

        public int EventAreaId { get; set; }

        public int Row { get; set; }

        public int Number { get; set; }

        public EventSeatState State { get; set; }
    }
}
