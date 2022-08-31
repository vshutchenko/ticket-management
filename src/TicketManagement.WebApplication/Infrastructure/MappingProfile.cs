using AutoMapper;
using TicketManagement.Core.Clients.EventApi.Models;
using TicketManagement.Core.Clients.PurchaseApi.Models;
using TicketManagement.Core.Clients.UserApi.Models;
using TicketManagement.Core.Clients.VenueApi.Models;
using TicketManagement.WebApplication.Models.Account;
using TicketManagement.WebApplication.Models.Event;
using TicketManagement.WebApplication.Models.EventArea;
using TicketManagement.WebApplication.Models.EventSeat;
using TicketManagement.WebApplication.Models.Purchase;
using TicketManagement.WebApplication.Models.VenueManagement;

namespace TicketManagement.WebApplication.Infrastructure
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<EventModel, EventViewModel>();
            CreateMap<EventModel, PurchaseSeatsViewModel>();
            CreateMap<CreateEventViewModel, EventModel>()
                .ForMember(x => x.LayoutId, opt => opt.MapFrom(m => int.Parse(m.Layout)))
                .ForSourceMember(m => m.Layouts, opt => opt.DoNotValidate())
                .ForSourceMember(m => m.Venues, opt => opt.DoNotValidate());

            CreateMap<EventModel, CreateEventViewModel>();

            CreateMap<EventAreaModel, EventAreaViewModel>();

            CreateMap<EventSeatModel, EventSeatViewModel>();

            CreateMap<LayoutModel, Models.Event.LayoutViewModel>();

            CreateMap<VenueModel, Models.Event.VenueViewModel>();

            CreateMap<LayoutModel, Models.VenueManagement.LayoutViewModel>().ReverseMap();

            CreateMap<SeatModel, SeatViewModel>().ReverseMap();

            CreateMap<VenueModel, Models.VenueManagement.VenueViewModel>()
                .ForMember(vm => vm.Layouts, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<AreaModel, AreaViewModel>()
                .ForMember(vm => vm.Seats, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<CreatePurchaseViewModel, PurchaseModel>()
            .ForMember(p => p.Id, opt => opt.Ignore());

            CreateMap<EditUserViewModel, UserModel>()
                .ForSourceMember(m => m.Cultures, opt => opt.DoNotValidate())
                .ForSourceMember(m => m.TimeZones, opt => opt.DoNotValidate());

            CreateMap<RegisterViewModel, UserModel>()
                .ForSourceMember(m => m.Cultures, opt => opt.DoNotValidate())
                .ForSourceMember(m => m.TimeZones, opt => opt.DoNotValidate())
                .ForSourceMember(m => m.Password, opt => opt.DoNotValidate())
                .ForSourceMember(m => m.ConfirmPassword, opt => opt.DoNotValidate());

            CreateMap<LoginViewModel, LoginModel>().ReverseMap();
            CreateMap<RegisterViewModel, RegisterModel>().ReverseMap();

            CreateMap<RegisterViewModel, RegisterModel>()
                .ForSourceMember(vm => vm.Cultures, opt => opt.DoNotValidate())
                .ForSourceMember(vm => vm.TimeZones, opt => opt.DoNotValidate())
                .ForSourceMember(vm => vm.ConfirmPassword, opt => opt.DoNotValidate())
                .ReverseMap();
        }
    }
}
