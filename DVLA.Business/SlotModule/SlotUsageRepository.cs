using DVLA.Business.UserModule;
using DVLA.Data;
using DVLA.Data.Models.Auth;
using DVLA.Data.Models.DataObjects.DTOs;
using DVLA.Data.Models.DataObjects.ViewModels;
using DVLA.Data.Models.Enumerables;
using DVLA.DATA.Domains;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Business.SlotModule
{
    public class SlotUsageRepository : ISlotUsageRepository
    {
        private readonly DVLADbContext _context;
        private readonly ILogger<SlotUsageRepository> _logger;
        private readonly string _connectionString;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserService _userService;
        public SlotUsageRepository(DVLADbContext context, ILogger<SlotUsageRepository> logger, IConfiguration configuration, UserManager<ApplicationUser> userManager, IUserService userService)
        {
            _context = context;
            _logger = logger;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _userManager = userManager;
            _userService = userService;
        }

        public async Task<long[]> GetTotalSlots()
        {
            long[] result = new long[2];
            UserViewModel userModel = _userService.GetUserData();
            ApplicationUser applicationUser = await _userManager.FindByIdAsync(userModel.Id);
            bool isSysAdmin = await _userManager.IsInRoleAsync(applicationUser, AppRoles.SYSTEMADMIN);

            SlotUsageBarModel bar = isSysAdmin? FetchSlotUsageBar(null): FetchSlotUsageBar(userModel.OptometristFirmId);
            
            result = new long[] { bar.LearnUnusedSlot, bar.OtherUnusedSlot };
            return result;
        }

        //Todo
        public List<TestAnalysisModel> FetchTestAnalysis(long? optometristId, DateTime StartDate, DateTime EndDate)
        {
            List<TestAnalysisModel> result = new();
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("SlotUsageByDay", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@OptometristID", optometristId ?? System.Data.SqlTypes.SqlInt64.Null);
                        cmd.Parameters.AddWithValue("@StartDate", StartDate);
                        cmd.Parameters.AddWithValue("@EndDate", EndDate);

                        conn.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result.Add(new TestAnalysisModel
                                {
                                    BusinessName = !reader.IsDBNull(reader.GetOrdinal("BusinessName")) ? reader.GetString("BusinessName") : null,
                                    Metric = !reader.IsDBNull(reader.GetOrdinal("Metric")) ? reader.GetString("Metric") : null,
                                    Quantity = reader.GetInt32("Quantity"),
                                    Region = !reader.IsDBNull(reader.GetOrdinal("Region")) ? reader.GetString("Region") : null,
                                    TestDate = !reader.IsDBNull(reader.GetOrdinal("TestDate")) ? reader.GetDateTime("TestDate") : (DateTime?)null
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

        public List<TestAnalysisModel> FetchMonthlySlots(long? optometristId)
        {
            List<TestAnalysisModel> result = new();
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("SlotUsageByMonth", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@OptometristID", optometristId ?? System.Data.SqlTypes.SqlInt64.Null);

                        conn.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result.Add(new TestAnalysisModel
                                {
                                    BusinessName = !reader.IsDBNull(reader.GetOrdinal("BusinessName")) ? reader.GetString("BusinessName") : null,
                                    Metric = !reader.IsDBNull(reader.GetOrdinal("Metric")) ? reader.GetString("Metric") : null,
                                    Quantity = reader.GetInt32("Quantity"),
                                    Region = !reader.IsDBNull(reader.GetOrdinal("Region")) ? reader.GetString("Region") : null,
                                    TestDate = !reader.IsDBNull(reader.GetOrdinal("TestDate")) ? reader.GetDateTime("TestDate") : (DateTime?)null
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

        public List<SlotUsageModel> FetchSlotUsage(DateTime? StartDate, DateTime? EndDate, AccessType? accessType)
        {
            StartDate = StartDate.HasValue ? StartDate.Value : DateTime.Now.Date;
            EndDate = EndDate.HasValue ? EndDate : DateTime.Now.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

            List<SlotUsageModel> result = new();
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("FetchSlotUsage", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@StartDate", StartDate ?? System.Data.SqlTypes.SqlDateTime.Null);
                        cmd.Parameters.AddWithValue("@EndDate", EndDate ?? System.Data.SqlTypes.SqlDateTime.Null);
                        cmd.Parameters.AddWithValue("@AccessType", accessType ?? 0);

                        conn.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result.Add(new SlotUsageModel
                                {
                                    BusinessName = !reader.IsDBNull(reader.GetOrdinal("BusinessName")) ? reader.GetString("BusinessName") : null,
                                    AccessType = !reader.IsDBNull(reader.GetOrdinal("AccessType")) ? reader.GetString("AccessType") : null,
                                    Balance = reader.GetInt32("Balance"),
                                    TotalSlotPurchased = reader.GetInt32("TotalSlotPurchased"),
                                    TotalSlotUsed = reader.GetInt32("TotalSlotUsed")
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

        public List<SlotUsageModel> FetchOptometristSlotUsage(DateTime? StartDate, DateTime? EndDate, AccessType? accessType, int? optometristFirmId = null)
        {
            int? accType = null;
            if (accessType.HasValue) accType = (int)accessType.Value;
            if (accessType == 0) accType = null;

            StartDate = StartDate == null ? StartDate : Utility.StartOfDay(StartDate.Value);
            EndDate = EndDate == null ? EndDate : Utility.EndOfDay(EndDate.Value);

            List<SlotUsageModel> result = new();
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("FetchOptometristFirmSlotUsage", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@StartDate", StartDate ?? System.Data.SqlTypes.SqlDateTime.Null);
                        cmd.Parameters.AddWithValue("@EndDate", EndDate ?? System.Data.SqlTypes.SqlDateTime.Null);
                        cmd.Parameters.AddWithValue("@AccessType", accType ?? System.Data.SqlTypes.SqlInt32.Null);
                        cmd.Parameters.AddWithValue("@OptometristFirmId", optometristFirmId ?? System.Data.SqlTypes.SqlInt32.Null);

                        conn.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result.Add(new SlotUsageModel
                                {
                                    BusinessName = !reader.IsDBNull(reader.GetOrdinal("BusinessName")) ? reader.GetString("BusinessName") : null,
                                    AccessType = !reader.IsDBNull(reader.GetOrdinal("AccessType")) ? reader.GetString("AccessType") : null,
                                    Balance = reader.GetInt32("Balance"),
                                    TotalSlotPurchased = reader.GetInt32("TotalSlotPurchased"),
                                    TotalSlotUsed = reader.GetInt32("TotalSlotUsed")
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

        public SlotUsageBarModel FetchSlotUsageBar(int? optometristFirmId = null)
        {

            SlotUsageBarModel result = new();
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("FetchSlotUsageBar", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@OptometristFirmId", optometristFirmId ?? System.Data.SqlTypes.SqlInt32.Null);

                        conn.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result = new()
                                {
                                    LearnerUsedSlot = reader.GetInt32("LearnerUsedSlot"),
                                    LearnUnusedSlot = reader.GetInt32("LearnUnusedSlot"),
                                    OtherUnusedSlot = reader.GetInt32("OtherUnusedSlot"),
                                    OtherUsedSlot = reader.GetInt32("OtherUsedSlot"),
                                    TotalSlot = reader.GetInt32("TotalSlot")
                                };
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


        public List<TestAnalysisModel> FetchWeeklySlots(long? optometristId)
        {
            List<TestAnalysisModel> result = new();
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("SlotUsageByWeek", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@OptometristID", optometristId ?? System.Data.SqlTypes.SqlInt64.Null);

                        conn.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result.Add(new TestAnalysisModel
                                {
                                    BusinessName = !reader.IsDBNull(reader.GetOrdinal("BusinessName")) ? reader.GetString("BusinessName") : null,
                                    Metric = !reader.IsDBNull(reader.GetOrdinal("Metric")) ? reader.GetString("Metric") : null,
                                    Quantity = reader.GetInt32("Quantity"),
                                    Region = reader.GetString("Region"),
                                    TestDate = !reader.IsDBNull(reader.GetOrdinal("TestDate")) ? reader.GetDateTime("TestDate") : (DateTime?)null
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

        public List<TestAnalysisModel> FetchYearlySlots(long? optometristId)
        {
            List<TestAnalysisModel> result = new();
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("SlotUsageByYear", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@OptometristID", optometristId ?? System.Data.SqlTypes.SqlInt64.Null);

                        conn.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result.Add(new TestAnalysisModel
                                {
                                    BusinessName = !reader.IsDBNull(reader.GetOrdinal("BusinessName")) ? reader.GetString("BusinessName") : null,
                                    Metric = !reader.IsDBNull(reader.GetOrdinal("Metric")) ? reader.GetString("Metric") : null,
                                    Quantity = reader.GetInt32("Quantity"),
                                    Region = reader.GetString("Region"),
                                    TestDate = !reader.IsDBNull(reader.GetOrdinal("TestDate")) ? reader.GetDateTime("TestDate") : (DateTime?)null
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

        public IEnumerable<ChartModel> SlotUsageByTestCenterByDay()
        {
            return Enumerable.Empty<ChartModel>(); //_context.Database.SqlQuery<ChartModel>("SlotUsageByTestCenterByDay").AsEnumerable();
        }

        public IEnumerable<ChartModel> SlotUsageByTestCenterByDay(long userId)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@UserId",userId)
            };
            return Enumerable.Empty<ChartModel>(); //_context.Database.SqlQuery<ChartModel>("SlotUsageByOptometristByDay @UserId", parameters).AsEnumerable();
        }

        public IEnumerable<ChartModel> SlotUsageByTestCenterByMonth()
        {
            return Enumerable.Empty<ChartModel>(); //_context.Database.SqlQuery<ChartModel>("SlotUsageByTestCenterByMonth").AsEnumerable();
        }

        public IEnumerable<ChartModel> SlotUsageByTestCenterByMonth(long userId)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@UserId",userId)
            };
            return Enumerable.Empty<ChartModel>(); //_context.Database.SqlQuery<ChartModel>("SlotUsageByOptometristByMonth @UserId", parameters).AsEnumerable();
        }

        public IEnumerable<ChartModel> SlotUsageByTestCenterByWeek()
        {
            return Enumerable.Empty<ChartModel>(); //_context.Database.SqlQuery<ChartModel>("SlotUsageByTestCenterByWeek").AsEnumerable();
        }

        public IEnumerable<ChartModel> SlotUsageByTestCenterByWeek(long userId)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@UserId",userId)
            };
            return Enumerable.Empty<ChartModel>(); //_context.Database.SqlQuery<ChartModel>("SlotUsageByOptometristByWeek @UserId", parameters).AsEnumerable();
        }

        public IEnumerable<ChartModel> SlotUsageByTestCenterByYear()
        {
            return Enumerable.Empty<ChartModel>(); //_context.Database.SqlQuery<ChartModel>("SlotUsageByTestCenterByYear").AsEnumerable();
        }

        public IEnumerable<ChartModel> SlotUsageByTestCenterByYear(long userId)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@UserId",userId)
            };
            return Enumerable.Empty<ChartModel>();//_context.Database.SqlQuery<ChartModel>("SlotUsageByOptometristByYear @UserId", parameters).AsEnumerable();
        }
    }
}
