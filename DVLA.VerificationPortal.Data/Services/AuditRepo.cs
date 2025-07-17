using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DVLA.VerificationPortal.Application.Interfaces;
using DVLA.VerificationPortal.Domain.Entities;
using DVLA.VerificationPortal.Domain.Interfaces;
using DVLA.VerificationPortal.Shared;
using DVLA.VerificationPortal.Shared.DTOs;
using DVLA.VerificationPortal.Shared.Enums;
using DVLA.VerificationPortal.Shared.Responses;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DVLA.VerificationPortal.Application.Services
{
    public class AuditRepo : IAuditRepo
    {
        private readonly ILogger<AuditRepo> _logger;
        private readonly IUserService _userService;
        private readonly IAuthUser _authUser;
        private readonly IGenericRepository<AuditLog> _auditLogRepository;

        private static object initLock = new object();

        public AuditRepo(ILogger<AuditRepo> logger, IUserService userService, IAuthUser authUser, IGenericRepository<AuditLog> auditLogRepository)
        {
            _logger = logger;
            _userService = userService;
            _authUser = authUser;
            _auditLogRepository = auditLogRepository;
        }

        public async Task AddAuditAsync(string action, string description)
        {
            var user = await _userService.GetUserByIdAsync(_authUser.UserId);

            var auditLog = new AuditLog
            {
                 Action=action,
                 UserId=_authUser.UserId,
                Description = description,
                CreatedDate = DateTime.Now
            };
            await _auditLogRepository.AddAsync(auditLog);
        }


        public async Task<List<ActivityModel>> GetAuditAsync(AuditFilterModel model)
        {
            var result = new List<ActivityModel>();
            try
            {
                DateTime? startDate = model.StartDate == null ? null : Utility.StartOfDay(model.StartDate.Value);
                DateTime? endDate = model.EndDate == null ? null : Utility.EndOfDay(model.EndDate.Value);


                var query = await _auditLogRepository.FilterAsync(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate);

                PaginatedResponse<ApplicationUserDto> userQuery = await _userService.GetAllAsync(1, 1000);
                IEnumerable<ApplicationUserDto> users = userQuery.Items.Where(x => query.Select(y => y.UserId).Contains(x.Id));

                result = query.Select(x => new ActivityModel
                {
                    Action = x.Action,
                    CreatedDate = x.CreatedDate,
                    Description = x.Description,
                    UserId = x.UserId,
                     UserName=users.FirstOrDefault(y=>y.Id==x.UserId)?.UserName
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
            return result;
        }
    }
}
