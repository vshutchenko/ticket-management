using AutoMapper;
using TicketManagement.DataAccess.Entities;
using TicketManagement.UserApi.Models;

namespace TicketManagement.UserApi.MappingConfig
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Language, LanguageModel>().ReverseMap();
            CreateMap<User, UserModel>().ReverseMap();
        }
    }
}
