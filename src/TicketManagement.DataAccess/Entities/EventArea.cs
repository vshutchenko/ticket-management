﻿namespace TicketManagement.DataAccess.Entities
{
    public class EventArea
    {
        public int Id { get; set; }

        public int EventId { get; set; }

        public string Descpription { get; set; }

        public int CoordX { get; set; }

        public int CoordY { get; set; }

        public decimal Price { get; set; }
    }
}
