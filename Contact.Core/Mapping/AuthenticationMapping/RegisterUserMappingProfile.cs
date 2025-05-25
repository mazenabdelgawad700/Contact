using AutoMapper;
using Contact.Core.Featuers.Authentication.Command.Model;
using Contact.Domain.Entities;

namespace Contact.Core.Mapping.AuthenticationMapping
{
    public class RegisterUserMappingProfile : Profile
    {
        public RegisterUserMappingProfile()
        {
            CreateMap<RegisterUserCommand, User>().ReverseMap()
                .ForMember(dest => dest.Password, opt => opt.Ignore());
        }
    }
}
