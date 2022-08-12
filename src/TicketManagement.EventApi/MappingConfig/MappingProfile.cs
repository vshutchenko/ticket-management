using AutoMapper;
using TicketManagement.DataAccess.Entities;
using TicketManagement.EventApi.Models;

namespace TicketManagement.EventApi.MappingConfig
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Event, EventModel>().ReverseMap();
            CreateMap<Venue, VenueModel>().ReverseMap();
            CreateMap<Layout, LayoutModel>().ReverseMap();
            CreateMap<Area, AreaModel>().ReverseMap();
            CreateMap<Seat, SeatModel>().ReverseMap();
            CreateMap<EventArea, EventAreaModel>();
            CreateMap<EventSeat, EventSeatModel>();
            CreateMap<EventSeatState, EventSeatStateModel>().ReverseMap();
        }
    }
}
