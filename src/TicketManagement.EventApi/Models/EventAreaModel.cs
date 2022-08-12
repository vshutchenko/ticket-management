﻿namespace TicketManagement.EventApi.Models
{
    public class EventAreaModel
    {
        public int Id { get; set; }

        public int EventId { get; set; }

        public string? Description { get; set; }

        public int CoordX { get; set; }

        public int CoordY { get; set; }

        public decimal Price { get; set; }
    }
}
