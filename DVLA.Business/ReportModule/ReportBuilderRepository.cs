using DVLA.Data.Models.DataObjects.DTOs;
using DVLA.Data;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DVLA.Business.ReportModule
{
    public class ReportBuilderRepository : IReportBuilderRepository, IDisposable
    {
        private readonly DVLADbContext _context;
        private readonly string _connectionString;
        private readonly ILogger<ReportBuilderRepository> _logger;

        public ReportBuilderRepository(DVLADbContext context, IConfiguration configuration, ILogger<ReportBuilderRepository> logger)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _logger = logger;
        }
        public List<QueryBuilderModel> FetchQueryBuilder(int reportType, long id)
        {
            List<QueryBuilderModel> result = new();
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("FetchQueryBuilder", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@reportType", reportType);
                        cmd.Parameters.AddWithValue("@Id", id);

                        conn.OpenAsync();
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result.Add(new QueryBuilderModel
                                {
                                    CreatedBy = reader.GetString("CreatedBy"),
                                    DateGenerated = reader.GetString("DateGenerated"),
                                    DateLastModified = reader.GetString("DateLastModified"),
                                    Description = reader.GetString("Description"),
                                    ID = reader.GetInt64("ID"),
                                    IsActive = reader.GetBoolean("IsActive"),
                                    IsDeleted = reader.GetBoolean("IsDeleted"),
                                    QueryName = reader.GetString("QueryName"),
                                    ReportType = reader.GetString("ReportType"),
                                    TransactionSelectFields = reader.GetString("TransactionSelectFields"),
                                    TransactionWhereClause = reader.GetString("TransactionWhereClause"),
                                    UpdatedBy = reader.GetString("ModifiedBy")
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
        public void Dispose()
        {
            _context.Dispose();
        }


    }
}
