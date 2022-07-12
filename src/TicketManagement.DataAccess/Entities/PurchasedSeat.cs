using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

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
