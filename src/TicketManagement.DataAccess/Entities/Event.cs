using System;

namespace TicketManagement.DataAccess.Entities
{
    public class Event
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Descpription { get; set; }

        public int LayoutId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
