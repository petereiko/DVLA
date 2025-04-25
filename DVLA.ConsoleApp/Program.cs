using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DVLA.ConsoleApp.DBContext;
using Microsoft.EntityFrameworkCore;
using DVLA.ConsoleApp.Services;


namespace DVLA.ConsoleApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            

            var host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                config.SetBasePath(Directory.GetCurrentDirectory());
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                var connStr = context.Configuration.GetConnectionString("Destination");

                services.AddDbContext<DestinationDbContext>(options =>
                    options.UseSqlServer(connStr));

                services.AddTransient<DestinationService>(); // your main application logic
            })
            .Build();

            Console.WriteLine("Program started");
            string cs = "Server=195.250.23.229;Database=DVLAVerificationDB;User Id=admin_verify;password=267tp8Va@;Encrypt=false;TrustServerCertificate=true;MultipleActiveResultSets=true;";
            var records = GetData(cs);

            Console.WriteLine($"{records.Count} records found");

            var app = host.Services.GetRequiredService<DestinationService>();
            app.InsertRecords(records);


            

           

            //cs = "Server=ingtechoptodriv\\SQLEXPRESS;Database=DVLAVerificationDB;User Id=dvla;password=Securityr&d2;Trusted_Connection=true;Encrypt=false;TrustServerCertificate=true;MultipleActiveResultSets=true;";
            cs = "Server=PETER-EIKORE\\SQLEXPRESS;Database=DVLAVerificationDB;User Id=sa;password=password;Encrypt=false;TrustServerCertificate=true;MultipleActiveResultSets=true;";

            Console.WriteLine("Inserting records to new Table");
            //BulkInsertDataTable(records, cs, "VisualAssessmentResults");
            InsertData(records, cs);

            Console.WriteLine("Done Inserting records");
        }

        static bool ReferenceExist(string reference, string cs)
        {
            using SqlConnection conn = new SqlConnection(cs);
            if (conn.State == ConnectionState.Closed)
                conn.Open();
            string cmdText = $"select COUNT(Id) from VisualAssessmentResults where LTRIM(RTRIM(ReferenceNumber))='{reference}'";
            using SqlCommand cmd=new SqlCommand(cmdText, conn);
            int count = (int)cmd.ExecuteScalar();
            return count > 0;
        }

        static void InsertData(List<VisualAssessmentResult> records, string cs)
        {
            foreach (VisualAssessmentResult rec in records) 
            {
                bool referenceExists = ReferenceExist(rec.ReferenceNumber, cs);
                if (referenceExists) continue;

                using SqlConnection conn = new SqlConnection(cs);
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                string cmdText = $"Insert into VisualAssessmentResults values ({rec.VisualAssessmentResultId},{rec.OptometristFirmId},'{rec.ReferenceNumber}',{rec.ResultServiceType},{rec.TestType},{rec.PassOrFail},'{rec.Surname}','{rec.FirstName}','{rec.OtherName}','{rec.DOB}','{rec.PostalAddress}','{rec.ContactNumber}','{rec.Email}','{rec.Unaided_OD}','{rec.Unaided_OS}','{rec.Unaided_OU}','{rec.BCV_OD}','{rec.BCV_OS}','{rec.BCV_OU}','{rec.HX_BCV_OD}','{rec.HX_BCV_OS}','{rec.HX_BCV_OU}','{rec.SingleImage_BCV_OU}','{rec.GlareTest_BCV_OD}','{rec.GlareTest_BCV_OS}','{rec.GlareTest_BCV_OU}','{rec.ColourVision_BCV_OU}','{rec.ContrastSensitivity_BCV}','{rec.PathologicalRemarks}','{rec.ResultConclusion}','{rec.TestDate}', '{rec.PassportImageUrl}',{rec.Status},{rec.IsRegistration},{rec.AccessType},{rec.PassResult},{rec.TransmittedDate},{rec.CreatedDate},'{rec.CreatedBy}',{rec.IsVerified},'{rec.VerifiedDate}',{rec.Gender},'{rec.Nationality}','{rec.OptometristFirmName}','{rec.OptometristName}')";

                using SqlCommand cmd = new SqlCommand(cmdText, conn);
                cmd.ExecuteNonQuery();

            }
        }

        static List<VisualAssessmentResult> GetData(string cs)
        {
            List<VisualAssessmentResult> records = new List<VisualAssessmentResult>();
            try
            {
                using SqlConnection conn = new SqlConnection(cs);
                conn.Open();
                using SqlCommand cmd = new SqlCommand("select top 1 * from VisualAssessmentResults where CreatedDate>'2025-03-11 10:16:03.8952335'", conn);

                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    records.Add(new VisualAssessmentResult
                    {
                        AccessType = rdr["AccessType"] == DBNull.Value ? null : Convert.ToInt32(rdr["AccessType"]),
                        Gender = rdr["Gender"] == DBNull.Value ? null : Convert.ToInt32(rdr["Gender"]),
                        Id = rdr["Id"] == DBNull.Value ? 0 : Convert.ToInt64(rdr["Id"]),
                        OptometristFirmId = Convert.ToInt32(rdr["OptometristFirmId"]),
                        PassOrFail = rdr["PassOrFail"] == DBNull.Value ? null : Convert.ToInt32(rdr["PassOrFail"]),

                        BCV_OD = rdr["BCV_OD"] == DBNull.Value ? null : rdr["BCV_OD"].ToString(),
                        BCV_OS = rdr["BCV_OS"] == DBNull.Value ? null : rdr["BCV_OS"].ToString(),
                        BCV_OU = rdr["BCV_OU"] == DBNull.Value ? null : rdr["BCV_OU"].ToString(),

                        CreatedBy = rdr["CreatedBy"] == DBNull.Value ? null : rdr["CreatedBy"].ToString(),
                        DOB = rdr["DOB"] == DBNull.Value ? null : Convert.ToDateTime(rdr["DOB"]),
                        Email = rdr["Email"] == DBNull.Value ? null : rdr["Email"].ToString(),

                        FirstName = rdr["FirstName"] == DBNull.Value ? null : rdr["FirstName"].ToString(),
                        GlareTest_BCV_OD = rdr["GlareTest_BCV_OD"] == DBNull.Value ? null : rdr["GlareTest_BCV_OD"].ToString(),
                        GlareTest_BCV_OS = rdr["GlareTest_BCV_OS"] == DBNull.Value ? null : rdr["GlareTest_BCV_OS"].ToString(),

                        GlareTest_BCV_OU = rdr["GlareTest_BCV_OU"] == DBNull.Value ? null : rdr["GlareTest_BCV_OU"].ToString(),
                        HX_BCV_OD = rdr["HX_BCV_OD"] == DBNull.Value ? null : rdr["HX_BCV_OD"].ToString(),
                        HX_BCV_OS = rdr["HX_BCV_OS"] == DBNull.Value ? null : rdr["HX_BCV_OS"].ToString(),

                        HX_BCV_OU = rdr["HX_BCV_OU"] == DBNull.Value ? null : rdr["HX_BCV_OU"].ToString(),
                        Nationality = rdr["Nationality"] == DBNull.Value ? null : rdr["Nationality"].ToString(),
                        OptometristFirmName = rdr["OptometristFirmName"] == DBNull.Value ? null : rdr["OptometristFirmName"].ToString(),

                        OptometristName = rdr["OptometristName"] == DBNull.Value ? null : rdr["OptometristName"].ToString(),
                        OtherName = rdr["OtherName"] == DBNull.Value ? null : rdr["OtherName"].ToString(),
                        PathologicalRemarks = rdr["PathologicalRemarks"] == DBNull.Value ? null : rdr["PathologicalRemarks"].ToString(),

                        PassportImageUrl = rdr["PassportImageUrl"] == DBNull.Value ? null : rdr["PassportImageUrl"].ToString(),
                        PostalAddress = rdr["PostalAddress"] == DBNull.Value ? null : rdr["PostalAddress"].ToString(),
                        ReferenceNumber = rdr["ReferenceNumber"] == DBNull.Value ? null : rdr["ReferenceNumber"].ToString(),

                        ResultConclusion = rdr["ResultConclusion"] == DBNull.Value ? null : rdr["ResultConclusion"].ToString(),
                        SingleImage_BCV_OU = rdr["SingleImage_BCV_OU"] == DBNull.Value ? null : rdr["SingleImage_BCV_OU"].ToString(),
                        Surname = rdr["Surname"] == DBNull.Value ? null : rdr["Surname"].ToString(),

                        Unaided_OD = rdr["Unaided_OD"] == DBNull.Value ? null : rdr["Unaided_OD"].ToString(),
                        Unaided_OS = rdr["Unaided_OS"] == DBNull.Value ? null : rdr["Unaided_OS"].ToString(),
                        Unaided_OU = rdr["Unaided_OU"] == DBNull.Value ? null : rdr["Unaided_OU"].ToString(),

                        VisualAssessmentResultId = Convert.ToInt64(rdr["Id"]),
                        IsRegistration = rdr["IsRegistration"] == DBNull.Value ? null : Convert.ToBoolean(rdr["IsRegistration"]),
                        CreatedDate = Convert.ToDateTime(rdr["CreatedDate"]),

                        IsVerified = Convert.ToBoolean(rdr["IsVerified"]),
                        PassResult = rdr["PassResult"] == DBNull.Value ? null : Convert.ToInt32(rdr["PassResult"]),
                        ResultServiceType = rdr["ResultServiceType"] == DBNull.Value ? null : Convert.ToInt32(rdr["ResultServiceType"]),

                        Status = rdr["Status"] == DBNull.Value ? null : Convert.ToInt32(rdr["Status"]),
                        TestDate = rdr["TestDate"] == DBNull.Value ? null : Convert.ToDateTime(rdr["TestDate"]),
                        TestType = Convert.ToInt32(rdr["TestType"]),

                        TransmittedDate = rdr["TransmittedDate"] == DBNull.Value ? null : Convert.ToDateTime(rdr["TransmittedDate"]),
                        VerifiedDate = rdr["VerifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(rdr["VerifiedDate"]),


                        ColourVision_BCV_OU = rdr["ColourVision_BCV_OU"] == DBNull.Value ? null : rdr["ColourVision_BCV_OU"].ToString(),
                        ContactNumber = rdr["ContactNumber"] == DBNull.Value ? null : rdr["ContactNumber"].ToString(),
                        ContrastSensitivity_BCV = rdr["ContrastSensitivity_BCV"] == DBNull.Value ? null : rdr["ContrastSensitivity_BCV"].ToString(),

                    });
                }
            }
            catch (Exception ex)
            {
            }
            
            return records;
        }

        static DataTable GetDataTable(string cs)
        {
            DataTable records = new DataTable();    
            try
            {
                using SqlConnection conn = new SqlConnection(cs);
                conn.Open();
                using SqlDataAdapter sda = new SqlDataAdapter("select * from VisualAssessmentResults where CreatedDate>'2025-03-11 10:16:03.8952335'", conn);

                sda.Fill(records);
                
            }
            catch (Exception ex)
            {
            }

            return records;
        }


        public static void BulkInsertDataTable(DataTable dataTable, string connectionString, string destinationTableName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                {
                    bulkCopy.DestinationTableName = destinationTableName;

                    // Optional: map columns if column names in DataTable and DB table differ
                    // bulkCopy.ColumnMappings.Add("SourceColumn", "DestinationColumn");

                    bulkCopy.WriteToServer(dataTable);
                }
            }
        }


    }
}
