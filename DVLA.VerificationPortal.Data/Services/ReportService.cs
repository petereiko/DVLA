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
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DVLA.VerificationPortal.Application.Services
{
    public class ReportService : IReportService
    {
        private readonly string _connectionString;
        private readonly ILogger<ReportService> _logger;
        private readonly IGenericRepository<VisualAssessmentResult> _visualAssessmentResultRepository;
        private readonly IUserRepository _userRepository;

        public ReportService(IConfiguration configuration, ILogger<ReportService> logger, IGenericRepository<VisualAssessmentResult> visualAssessmentResultRepository, IUserRepository userRepository)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _logger = logger;
            _visualAssessmentResultRepository = visualAssessmentResultRepository;
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<TestResultCountDto>> GetResults(DateTime StartDate, DateTime EndDate, PassOrFail? passOrFail)
        {
            StartDate = Utility.StartOfDay(StartDate);
            EndDate = Utility.EndOfDay(EndDate);
            var result = new List<TestResultCountDto>();
            
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("GetResults", conn))
                    {
                        int? passOrFailInt = null;

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@StartDate", StartDate);
                        cmd.Parameters.AddWithValue("@EndDate", EndDate);
                        cmd.Parameters.AddWithValue("@PassOrFail", passOrFail.HasValue ? (int)passOrFail : passOrFailInt ?? System.Data.SqlTypes.SqlInt32.Null);

                        await conn.OpenAsync();
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                result.Add(new TestResultCountDto
                                {
                                    Count = reader.GetInt32("Count"),
                                    PassOrFail = (PassOrFail)reader.GetInt32("PassOrFail"),
                                    ResultServiceType = (ResultServiceType)reader.GetInt32("ResultServiceType"),
                                    OptometristFirmName = reader.IsDBNull(reader.GetOrdinal("OptometristFirmName"))
            ? null
            : reader.GetString(reader.GetOrdinal("OptometristFirmName"))
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

        

        public async Task<IEnumerable<VerifiedItemDto>> GetVerifiedResults(DateTime StartDate, DateTime EndDate)
        {
            StartDate = Utility.StartOfDay(StartDate);
            EndDate = Utility.EndOfDay(EndDate);
            var result = new List<VerifiedItemDto>();

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("GetVerifiedResults", conn))
                    {

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@StartDate", StartDate);
                        cmd.Parameters.AddWithValue("@EndDate", EndDate);

                        await conn.OpenAsync();
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                result.Add(new VerifiedItemDto
                                {
                                    Count = reader.GetInt32("Count"),
                                    VerifierEmail = reader.GetString("VerifierEmail"),
                                     UserId=reader.GetString("UserId")
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



        public async Task<IEnumerable<TestResultDto>> VerifiedResultsByUser(string userId)
        {
            IEnumerable<VisualAssessmentResult> query = await _visualAssessmentResultRepository.FilterAsync(x => x.VerifiedBy == userId, false);

            var allUser = await _userRepository.GetAllUsers();

            return query.Select(x => new TestResultDto
            {
                FullName = x.FirstName + " " + x.Surname,
                PassConclusion = x.ResultConclusion,
                TestDate = x.TestDate,
                Passport = x.PassportImageUrl,
                TestType = EnumHelper.GetEnumDescription(x.ResultServiceType),
                Verified = x.IsVerified
            });
        }

        public async Task<int> GetUsedSlot(int? optometristFirmId)
        {
            DateTime endDate = DateTime.UtcNow.AddMonths(-3);
            int usedSlots = 0;
            try
            {
                IEnumerable<VisualAssessmentResult> results = await _visualAssessmentResultRepository.FilterAsync(x => x.OptometristFirmId == optometristFirmId && x.TestDate <= endDate, false);
                usedSlots = results.Count();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
            return usedSlots;
        }
    }
}
