using AutoMapper;
using Domain.Dtos;
using Domain.Models;

namespace Domain.Profiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<RegisterDto, User>()
            .ForMember(dest =>
                dest.Username,
                opt => opt.MapFrom(src => src.Username))
            .ForMember(dest =>
                dest.Role,
                opt => opt.MapFrom(src => src.Role))
            .ForMember(dest =>
                dest.PasswordHash,
                opt => opt.Ignore())
            .ForMember(dest =>
                dest.PasswordSalt,
                opt => opt.Ignore())
            .ForMember(dest =>
                dest.Id,
                opt => opt.Ignore());

        CreateMap<User, UpdateRoleDto>();

        CreateMap<UpdateRoleDto, User>()
            .ForMember(dest =>
                    dest.Id,
                opt => opt.Ignore())
            .ForMember(dest =>
                    dest.Username,
                opt => opt.Ignore())
            .ForMember(dest =>
                    dest.Role,
                opt => opt.MapFrom(src => src.Role))
            .ForMember(dest =>
                    dest.PasswordHash,
                opt => opt.Ignore())
            .ForMember(dest =>
                    dest.PasswordSalt,
                opt => opt.Ignore());
    }
}