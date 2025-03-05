using AutoMapper;
using DVLA.VerificationPortal.Domain.Entities;
using DVLA.VerificationPortal.Infrastructure.Database.Entities;
using DVLA.VerificationPortal.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.VerificationPortal.Infrastructure.MappingProfiles
{
    public class InfrastructureMappingProfile:Profile
    {
        public InfrastructureMappingProfile()
        {
            CreateMap<ApplicationUserDto, ApplicationUser>().ReverseMap();
            CreateMap<RoleDto, ApplicationRole>().ReverseMap();
            CreateMap<VisualAssessmentResultDto, VisualAssessmentResult>().ReverseMap();
        }
    }
}
