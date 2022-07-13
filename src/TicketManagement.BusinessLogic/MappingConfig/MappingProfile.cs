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
            CreateMap<Venue, VenueModel>().ReverseMap();
            CreateMap<Layout, LayoutModel>().ReverseMap();
            CreateMap<Area, AreaModel>().ReverseMap();
            CreateMap<Seat, SeatModel>().ReverseMap();
            CreateMap<EventArea, EventAreaModel>();
            CreateMap<EventSeat, EventSeatModel>();
            CreateMap<Language, LanguageModel>().ReverseMap();
            CreateMap<EventSeatState, EventSeatStateModel>().ReverseMap();
            CreateMap<Purchase, PurchaseModel>();
            CreateMap<PurchaseModel, Purchase>()
                .ForSourceMember(x => x.SeatIds, opt => opt.DoNotValidate());
            CreateMap<PurchasedSeat, PurchasedSeatModel>();
            CreateMap<User, UserModel>().ReverseMap();
        }
    }
}
