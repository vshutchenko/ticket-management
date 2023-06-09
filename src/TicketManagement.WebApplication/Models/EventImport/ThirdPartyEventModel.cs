﻿using System;

namespace TicketManagement.WebApplication.Models.EventImport
{
    public class ThirdPartyEventModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string PosterImage { get; set; } = string.Empty;
    }
}