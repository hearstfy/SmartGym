using System;
using AutoMapper;
using SmartGym.Data;
using SmartGym.Models;

namespace SmartGym.Profiles
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<UserProfile, ProfileDto>();
            CreateMap<ProfileDto, UserProfile>().ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => Convert.ToDateTime(src.DateOfBirth)));
        }

    }
}