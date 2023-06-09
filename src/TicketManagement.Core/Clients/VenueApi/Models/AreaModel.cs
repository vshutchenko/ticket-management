﻿namespace TicketManagement.Core.Clients.VenueApi.Models
{
    public class AreaModel
    {
        public int Id { get; set; }

        public int LayoutId { get; set; }

        public string? Description { get; set; }

        public int CoordX { get; set; }

        public int CoordY { get; set; }
    }
}
