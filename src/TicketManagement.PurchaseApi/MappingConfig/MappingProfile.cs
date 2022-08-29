using AutoMapper;
using TicketManagement.DataAccess.Entities;
using TicketManagement.PurchaseApi.Models;

namespace TicketManagement.PurchaseApi.MappingConfig
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<EventSeat, EventSeatModel>();
            CreateMap<Purchase, PurchaseModel>();
            CreateMap<PurchaseModel, Purchase>()
                .ForSourceMember(x => x.SeatIds, opt => opt.DoNotValidate());
            CreateMap<PurchasedSeat, PurchasedSeatModel>();
        }
    }
}
