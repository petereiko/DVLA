using DVLA.DATA.Domains;
using DVLA.Data.Models.DataObjects.DTOs;
using DVLA.Data.Models.Domains;
using DVLA.Data;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace DVLA.Business.UserModule
{
    public class AuditRepo : IAuditRepo
    {
        private readonly DVLADbContext _context;
        private readonly ILogger<AuditRepo> _logger;
        private readonly string _connectionString;
        private readonly IUserService _userService;
        private readonly IAuthUser _authUser;

        private static object initLock = new object();

        public AuditRepo(DVLADbContext context, ILogger<AuditRepo> logger, IConfiguration configuration, IUserService userService, IAuthUser authUser)
        {
            _context = context;
            _logger = logger;
            _connectionString = configuration["DefaultConnection"];
            _userService = userService;
            _authUser = authUser;
        }

        public void AddAudit(long moduleActionId, string description)
        {
            var user = _context.ApplicationUsers.FirstOrDefault(x => x.Id == _authUser.UserId);

            var auditLog = new ActivityLog
            {
                NameOfUser = user?.LastName == null ? user?.LastName : user?.FirstName,
                ModuleActionId = moduleActionId,
                CreatedBy = _authUser.UserId,
                Description = description,
                CreatedDate = DateTime.Now
            };
            _context.ActivityLogs.Add(auditLog);
            _context.SaveChanges();
        }


        public async Task<List<ActivityModel>> GetAudit(AuditFilterModel model)
        {
            var result = new List<ActivityModel>();
            try
            {
                string startDate = model.StartDate == null ? "" : model.StartDate;
                string endDate = model.EndDate == null ? "" : model.EndDate;

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("GetAudits", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@UserId", model.UserId ?? System.Data.SqlTypes.SqlString.Null);
                        cmd.Parameters.AddWithValue("@OptometristFirmId", model.OptometristFirmId ?? System.Data.SqlTypes.SqlInt32.Null);
                        cmd.Parameters.AddWithValue("@ModuleId", model.ModuleId ?? System.Data.SqlTypes.SqlInt32.Null);
                        cmd.Parameters.AddWithValue("@StartDate", startDate);
                        cmd.Parameters.AddWithValue("@EndDate", endDate);

                        await conn.OpenAsync();
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                result.Add(new ActivityModel
                                {
                                    Description = reader.GetString("Description"),
                                    FullName = reader.GetString("FullName"),
                                    ModuleName = reader.GetString("ModuleName"),
                                    DateCreated = reader.GetDateTime("CreatedDate"),
                                    Id = reader.GetInt64("Id")
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);   
            }
            return result;
        }

    }
}
