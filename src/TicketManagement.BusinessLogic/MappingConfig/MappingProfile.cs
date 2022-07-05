using AutoMapper;
using TicketManagement.BusinessLogic.Models;
using TicketManagement.DataAccess.Entities;

namespace TicketManagement.BusinessLogic.MappingConfig
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Event, EventModel>().ReverseMap();
            CreateMap<Venue, VenueModel>();
            CreateMap<Layout, LayoutModel>();
            CreateMap<Area, AreaModel>();
            CreateMap<Seat, SeatModel>();
            CreateMap<EventArea, EventAreaModel>();
            CreateMap<EventSeat, EventSeatModel>();
            CreateMap<Language, LanguageModel>();
            CreateMap<Purchase, PurchaseModel>();
            CreateMap<PurchaseModel, Purchase>()
                .ForSourceMember(x => x.SeatIds, opt => opt.DoNotValidate());
            CreateMap<PurchasedSeat, PurchasedSeatModel>();
            CreateMap<User, UserModel>().ReverseMap();
        }
    }
}
