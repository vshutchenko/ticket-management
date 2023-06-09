﻿using AutoMapper;
using TicketManagement.DataAccess.Entities;
using TicketManagement.EventApi.Models;

namespace TicketManagement.EventApi.MappingConfig
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Event, EventModel>().ReverseMap();
            CreateMap<EventArea, EventAreaModel>();
            CreateMap<EventSeat, EventSeatModel>();
        }
    }
}
