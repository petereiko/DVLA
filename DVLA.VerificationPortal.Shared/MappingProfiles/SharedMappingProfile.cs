using AutoMapper;
using DVLA.VerificationPortal.Shared.DTOs;
using DVLA.VerificationPortal.Shared.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.VerificationPortal.Shared.MappingProfiles
{
    public class SharedMappingProfile:Profile
    {
        public SharedMappingProfile()
        {
            CreateMap<ApplicationUserDto, OnboardUserRequest>().ReverseMap();
            CreateMap<ApplicationUserDto, EditUserRequest>().ReverseMap();
            CreateMap<OnboardUserRequest, EditUserRequest>().ReverseMap();
        }
    }
}
