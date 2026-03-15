using DVLA.VerificationPortal.Infrastructure.Database.Entities;
using DVLA.VerificationPortal.Infrastructure.Models;
using DVLA.VerificationPortal.Shared.DTOs;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.VerificationPortal.Infrastructure.Repositories
{
    public class AuditRepo : IAuditRepo
    {
        private readonly IGenericRepository<AuditLog> _auditLogRepository;
        private readonly IGenericRepository<ApplicationUser> _userRepository;
        private readonly IHttpContextAccessor _accessor;

        public AuditRepo(IGenericRepository<AuditLog> auditLogRepository, IGenericRepository<ApplicationUser> userRepository, IHttpContextAccessor accessor)
        {
            _auditLogRepository = auditLogRepository;
            _userRepository = userRepository;
            _accessor = accessor;
        }

        public async Task AddAuditAsync(string actionName, string description)
        {
            AuditLog auditLog = new()
            {
                Action = actionName,
                Description = description,
                CreatedDate = DateTime.UtcNow,
                UserId = _accessor.HttpContext.User.Identity.GetUserId()
            };
            await _auditLogRepository.AddAsync(auditLog);
        }

        public async Task<List<ActivityModel>> GetAuditAsync(AuditFilterModel filter)
        {
            var entities = await _auditLogRepository.FilterAsync(x => x.CreatedDate >= filter.StartDate && x.CreatedDate <= filter.EndDate, false);

            List<string> userIds = entities.Select(x => x.UserId).ToList();

            IEnumerable<ApplicationUser> users = await _userRepository.FilterAsync(x => userIds.Contains(x.Id), false);

            List<ActivityModel> result = entities.Select(x => new ActivityModel
            {
                CreatedDate = x.CreatedDate,
                Action = x.Action,
                Description = x.Description,
                UserId = x.UserId,
                UserName = users.FirstOrDefault(u => u.Id == x.UserId)?.UserName
            }).ToList();

            return result;
        }
    }
}
