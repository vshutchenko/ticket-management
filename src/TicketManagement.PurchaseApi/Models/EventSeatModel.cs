﻿using TicketManagement.Core.Models;

namespace TicketManagement.PurchaseApi.Models
{
    public class EventSeatModel
    {
        public int Id { get; set; }

        public int EventAreaId { get; set; }

        public int Row { get; set; }

        public int Number { get; set; }

        public EventSeatState State { get; set; }
    }
}
