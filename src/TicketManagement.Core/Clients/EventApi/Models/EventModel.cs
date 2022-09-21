using System;

namespace TicketManagement.Core.Clients.EventApi.Models
{
    public class EventModel
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public int LayoutId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string? ImageUrl { get; set; }

        public bool Published { get; set; }
    }
}
