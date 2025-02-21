using DVLA.Data.Models.DataObjects.DTOs;
using DVLA.Data.Models.Enumerables;
using DVLA.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Data;
using Microsoft.Extensions.Configuration;
using DVLA.Data.Models.DataObjects.ViewModels;

namespace DVLA.Business.DashboardModule
{
    public class AnalyticRepository : IAnalyticRepository, IDisposable
    {
        private readonly DVLADbContext _context;
        private readonly ILogger<AnalyticRepository> _logger;
        private readonly string _connectionString;
        public AnalyticRepository(DVLADbContext context, ILogger<AnalyticRepository> logger, IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        private int GetMonthsDifference(DateTime startDate, DateTime endDate)
        {
            // Calculate the difference in years and convert it to months
            int months = (endDate.Year - startDate.Year) * 12 + (endDate.Month - startDate.Month);

            // Adjust if the end date's day is earlier in the month than the start date's day
            if (endDate.Day < startDate.Day)
            {
                months--;
            }

            return months;
        }


        private int GetYearsDifference(DateTime startDate, DateTime endDate)
        {
            // Calculate the initial year difference
            int years = endDate.Year - startDate.Year;

            // Adjust if the end date hasn't reached the same month and day as the start date yet
            if (endDate < startDate.AddYears(years))
            {
                years--;
            }

            return years;
        }


        public ChartSummaryCount GetApprovedRequestCount(long? optometristFirmId = null)
        {
            int? LearnersResult = 0;
            int? OthersResult = 0;
            int? MonthlyLearnersResult = 0;
            if (optometristFirmId != null)
            {
                LearnersResult = _context.SlotRequests.Where(x => x.OptometristFirmId == optometristFirmId && x.Status == SlotRequestStatus.Approved)?.Count();
                //OthersResult = _context.SlotRequests.Where(x => x.OptometristFirmId == optometristFirmId && x.AccessType == AccessType.OtherLicenceCategory && x.Status == SlotRequestStatus.Approved)?.Count();
                MonthlyLearnersResult = _context.SlotRequests.Where(x => x.OptometristFirmId == optometristFirmId && x.Status == SlotRequestStatus.Approved && GetYearsDifference(x.DateApproved.GetValueOrDefault() ,DateTime.Now) == 0 && GetMonthsDifference(x.DateApproved.Value, DateTime.Now) == 0)?.Count();

            }
            else
            {
                LearnersResult = _context.SlotRequests.Where(x => x.Status == SlotRequestStatus.Approved)?.Count();
                //OthersResult = _context.SlotRequests.Where(x => x.AccessType == AccessType.OtherLicenceCategory && x.Status == SlotRequestStatus.Approved)?.Count();
                MonthlyLearnersResult = _context.SlotRequests.Where(x => x.Status == SlotRequestStatus.Approved && GetYearsDifference(x.DateApproved.Value, DateTime.Now) == 0 && GetMonthsDifference(x.DateApproved.Value, DateTime.Now) == 0)?.Count();

            }
            return new ChartSummaryCount
            {
                LearnerValue = LearnersResult ?? 0,
                OthersValue = OthersResult ?? 0,
                MonthlyLearnerValue = MonthlyLearnersResult ?? 0
            };
        }


        public ChartSummaryCount GetDeclinedRequestCount(long? optometristFirmId = null)
        {
            int? LearnersResult = 0;
            int? OthersResult = 0;
            int? MonthlyLearnersResult = 0;
            if (optometristFirmId != null)
            {
                LearnersResult = _context.SlotRequests.Where(x => x.OptometristFirmId == optometristFirmId && x.Status == SlotRequestStatus.Reject)?.Count();
                //OthersResult = _context.SlotRequests.Where(x => x.OptometristFirmId == optometristFirmId && x.AccessType == AccessType.OtherLicenceCategory && x.Status == SlotRequestStatus.Reject)?.Count();
                MonthlyLearnersResult = _context.SlotRequests.Where(x => x.OptometristFirmId == optometristFirmId && x.Status == SlotRequestStatus.Reject && GetYearsDifference(x.DateApproved.Value, DateTime.Now) == 0 && GetMonthsDifference(x.DateApproved.Value, DateTime.Now) == 0)?.Count();

            }
            else
            {
                LearnersResult = _context.SlotRequests.Where(x => x.Status == SlotRequestStatus.Reject)?.Count();
                //OthersResult = _context.SlotRequests.Where(x => x.AccessType == AccessType.OtherLicenceCategory && x.Status == SlotRequestStatus.Reject)?.Count();
                MonthlyLearnersResult = _context.SlotRequests.Where(x => x.Status == SlotRequestStatus.Reject && GetYearsDifference(x.DateApproved.Value, DateTime.Now) == 0 && GetMonthsDifference(x.DateApproved.Value, DateTime.Now) == 0)?.Count();
            }
            return new ChartSummaryCount
            {
                LearnerValue = LearnersResult ?? 0,
                OthersValue = OthersResult ?? 0,
                MonthlyLearnerValue = MonthlyLearnersResult ?? 0
            };
        }

        public List<ChartCount> GetRequestChartCount(long status, long? optometristFirmId = null)
        {
            var result = new List<ChartCount>();
            try
            {
                var id = optometristFirmId ?? System.Data.SqlTypes.SqlInt64.Null;
               
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("GetRequestCount", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Status", status);
                        cmd.Parameters.AddWithValue("@OptomestristFirmId", id);

                        conn.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result.Add(new ChartCount
                                {
                                    Color="",
                                     Name=reader.GetString("Name"),
                                      Value=reader.GetInt32("Value")
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


        public List<ChartCount> GetSychronizationChartCount(long? optometristFirmId = null)
        {
            var result = new List<ChartCount>();
            try
            {
                var id = optometristFirmId ?? System.Data.SqlTypes.SqlInt64.Null;
                

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("GetSynchronizationCount", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@OptomestristFirmId", id);

                        conn.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result.Add(new ChartCount
                                {
                                    Color = "",
                                    Name = reader.GetString("Name"),
                                    Value = reader.GetInt32("Value")
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

        public DashboardViewModel GetDashboardData(int? optometristFirmId = null)
        {
            DashboardViewModel result = new ();
            try
            {
                var id = optometristFirmId ?? System.Data.SqlTypes.SqlInt32.Null;


                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("DashboardData", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@OptometristFirmId", id);

                        conn.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result = new()
                                {
                                    LearnerGrantedSlotCount = Convert.ToInt32(reader["LearnerGrantedSlotCount"]),
                                    LearnerUtilizedSlotCount = Convert.ToInt32(reader["LearnerUtilizedSlotCount"]),
                                    OtherGrantedSlotCount = Convert.ToInt32(reader["OtherGrantedSlotCount"]),
                                    OtherUtilizedSlotCount = Convert.ToInt32(reader["OtherUtilizedSlotCount"])
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

        public ChartSummaryCount GetAvailableSlots(long? optometristFirmId = null)
        {
            int? LearnersResult = 0;
            int? OthersResult = 0;


            if (optometristFirmId != null)
            {
                LearnersResult = _context.Slots.FirstOrDefault(x => x.AccessType == AccessType.LearnerDriversLicence && x.OptometristFirmId == optometristFirmId)?.Quantity;
                OthersResult = _context.Slots.FirstOrDefault(x => x.AccessType == AccessType.OtherLicenceCategory && x.OptometristFirmId == optometristFirmId)?.Quantity;

            }
            else
            {
                var slots = _context.Slots.ToList();
                LearnersResult = slots == null ? 0 : slots.Where(x => x.AccessType == AccessType.LearnerDriversLicence).Sum(x => x.Quantity);
                OthersResult = slots == null ? 0 : slots.Where(x => x.AccessType == AccessType.OtherLicenceCategory).Sum(x => x.Quantity);

            }
            return new ChartSummaryCount
            {
                LearnerValue = LearnersResult ?? 0,
                OthersValue = OthersResult ?? 0,

            };
        }


        public List<ChartCount> GetUsedSlotChartCount(long? optometristFirmId = null)
        {
            var result = new List<ChartCount>();
            try
            {
                var id = optometristFirmId ?? System.Data.SqlTypes.SqlInt64.Null;
                

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("GetUsedSlotCount", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@OptomestristFirmId", id);

                        conn.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result.Add(new ChartCount
                                {
                                    Color = "",
                                    Name = reader.GetString("Name"),
                                    Value = reader.GetInt32("Value")
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

        public ChartSummaryCount GetUsedSlots(long? optometristFirmId = null)
        {
            int? LearnersResult = 0;
            int? OthersResult = 0;
            int? MonthlyLearnersResult = 0;
            int? MonthlyOthersResult = 0;
            if (optometristFirmId != null)
            {
                LearnersResult = _context.VisualAssessmentResults.Where(x => x.OptometristFirmId == optometristFirmId && x.AccessType == AccessType.LearnerDriversLicence && x.ReferenceNumber != null)?.Count();
                OthersResult = _context.VisualAssessmentResults.Where(x => x.OptometristFirmId == optometristFirmId && x.AccessType == AccessType.OtherLicenceCategory && x.ReferenceNumber != null)?.Count();
                MonthlyLearnersResult = _context.VisualAssessmentResults.Where(x => x.OptometristFirmId == optometristFirmId && x.AccessType == AccessType.LearnerDriversLicence && x.ReferenceNumber != null && GetYearsDifference(x.TestDate.Value, DateTime.Now) == 0 && GetMonthsDifference(x.TestDate.Value, DateTime.Now) == 0)?.Count();
                MonthlyOthersResult = _context.VisualAssessmentResults.Where(x => x.OptometristFirmId == optometristFirmId && x.AccessType == AccessType.OtherLicenceCategory && x.ReferenceNumber != null && GetYearsDifference(x.TestDate.Value, DateTime.Now) == 0 && GetMonthsDifference(x.TestDate.Value, DateTime.Now) == 0)?.Count();
            }
            else
            {
                LearnersResult = _context.VisualAssessmentResults?.Where(x => x.AccessType == AccessType.LearnerDriversLicence && x.ReferenceNumber != null).Count();
                OthersResult = _context.VisualAssessmentResults?.Where(x => x.AccessType == AccessType.OtherLicenceCategory && x.ReferenceNumber != null).Count();
                MonthlyLearnersResult = _context.VisualAssessmentResults?.Where(x => x.AccessType == AccessType.LearnerDriversLicence && x.ReferenceNumber != null && GetYearsDifference(x.TestDate.Value, DateTime.Now) == 0 && GetMonthsDifference(x.TestDate.Value, DateTime.Now) == 0).Count();
                MonthlyOthersResult = _context.VisualAssessmentResults?.Where(x => x.AccessType == AccessType.OtherLicenceCategory && x.ReferenceNumber != null && GetYearsDifference(x.TestDate.Value, DateTime.Now) == 0 && GetMonthsDifference(x.TestDate.Value, DateTime.Now) == 0).Count();

            }
            return new ChartSummaryCount
            {
                LearnerValue = LearnersResult ?? 0,
                OthersValue = OthersResult ?? 0,
                MonthlyLearnerValue = MonthlyLearnersResult ?? 0,
                MonthlyOthersValue = MonthlyOthersResult ?? 0,
            };
        }

        public List<ChartCount> GetIncomeChartCount()
        {
            var result = new List<ChartCount>();
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("GetIncomeCount", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        conn.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result.Add(new ChartCount
                                {
                                    Color = "",
                                    Name = reader.GetString("Name"),
                                    Value = reader.GetInt32("Value")
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

        public List<ChartCount> GetOptometristFirmChartCount()
        {
            var result = new List<ChartCount>();
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("GetOptometristFirmCount", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        conn.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result.Add(new ChartCount
                                {
                                    Color = "",
                                    Name = reader.GetString("Name"),
                                    Value = reader.GetInt32("Value")
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
