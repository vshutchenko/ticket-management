using AutoMapper;
using TicketManagement.DataAccess.Entities;
using TicketManagement.VenueApi.Models;

namespace TicketManagement.VenueApi.MappingConfig
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Venue, VenueModel>().ReverseMap();
            CreateMap<Layout, LayoutModel>().ReverseMap();
            CreateMap<Area, AreaModel>().ReverseMap();
            CreateMap<Seat, SeatModel>().ReverseMap();
        }
    }
}
