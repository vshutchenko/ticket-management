using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TicketManagement.DataAccess.Entities
{
    [Table("Purchase")]
    public class Purchase
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int EventId { get; set; }
        public decimal Price { get; set; }
    }
}
