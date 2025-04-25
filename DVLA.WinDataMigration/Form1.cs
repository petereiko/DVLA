using System.Collections.Generic;
using DVLA.WinDataMigration.Databases;
using DVLA.WinDataMigration.Entities;
using Microsoft.EntityFrameworkCore;

namespace DVLA.WinDataMigration
{
    public partial class Form1 : Form
    {
        private List<VisualAssessmentResult> assessments;
        
        public Form1()
        {
            InitializeComponent();
            txtSourceIP.Text = "195.250.23.229";
            txtSourceUserID.Text = "admin_verify";
            txtSourcePassword.Text = "267tp8Va@";
            txtSqlQuery.Text = "select * from VisualAssessmentResults where CreatedDate>'2025-03-11 10:16:03.8952335'";

            txtDestinationIP.Text = "PETER-EIKORE\\SQLEXPRESS";
            txtDestinationUserID.Text = "sa";
            txtDestinationPassword.Text = "password";


        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void BtnDownloadData_Click(object sender, EventArgs e)
        {
            try
            {
                BtnDownloadData.Text = "Downloading...";
                lblMessage.Text = "Please wait...";
                string userInputConnectionString = $"Server={txtSourceIP.Text.Trim()};Database=DVLAVerificationDB;User Id={txtSourceUserID.Text.Trim()};password={txtSourcePassword.Text.Trim()};Encrypt=false;TrustServerCertificate=true;MultipleActiveResultSets=true;";

                //DbContextOptions<SourceDbContext> options = new DbContextOptionsBuilder<SourceDbContext>()
                //    .UseSqlServer(userInputConnectionString)
                //    .Options;

                DbContextOptions<SourceDbContext> options = new DbContextOptionsBuilder<SourceDbContext>()
    .UseSqlServer(userInputConnectionString, sqlOptions =>
    {
        sqlOptions.CommandTimeout(18000); // timeout in seconds (e.g., 3 minutes)
    })
    .Options;

                using (var context = new SourceDbContext(options))
                {
                    if (context.Database.CanConnect())
                    {
                        // Proceed to main form, pass the DbContext or options
                        IQueryable<VisualAssessmentResult> query = context.Database.SqlQueryRaw<VisualAssessmentResult>(txtSqlQuery.Text.Trim());
                        assessments = query.ToList();
                        lblMessage.Text = "Data downloaded";
                    }
                    else
                    {
                        MessageBox.Show("Unable to connect. Please check your internet connection or your connection string.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void BtnPush_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtSourceIP.Text == txtDestinationIP.Text)
                {
                    lblMessage.Text = "Invalid ips";
                    return;
                }
                BtnPush.Text = "Pushing...";
                lblMessage.Text = "Please wait...";
                string userInputConnectionString = $"Server={txtDestinationIP.Text.Trim()};Database=DVLAVerificationDB;User Id={txtDestinationUserID.Text.Trim()};password={txtDestinationPassword.Text.Trim()};Encrypt=false;TrustServerCertificate=true;MultipleActiveResultSets=true;";

                //DbContextOptions<SourceDbContext> options = new DbContextOptionsBuilder<SourceDbContext>()
                //    .UseSqlServer(userInputConnectionString)
                //    .Options;

                DbContextOptions<DestinationDbContext> options = new DbContextOptionsBuilder<DestinationDbContext>()
    .UseSqlServer(userInputConnectionString, sqlOptions =>
    {
        sqlOptions.CommandTimeout(18000); // timeout in seconds (e.g., 3 minutes)
    })
    .Options;

                using (var context = new DestinationDbContext(options))
                {
                    if (context.Database.CanConnect())
                    {
                        // Proceed to main form, pass the DbContext or options

                        IEnumerable<VisualAssessmentResult> records = assessments.Select(x => new VisualAssessmentResult
                        {
                            AccessType = x.AccessType,
                            BCV_OD = x.BCV_OD,
                            BCV_OS = x.BCV_OS,
                            BCV_OU = x.BCV_OU,
                            ColourVision_BCV_OU = x.ColourVision_BCV_OU,
                            ContactNumber = x.ContactNumber,
                            ContrastSensitivity_BCV = x.ContrastSensitivity_BCV,
                            CreatedBy = x.CreatedBy,
                            CreatedDate = x.CreatedDate,
                            DOB = x.DOB,
                            Email = x.Email,
                            FirstName = x.FirstName,
                            Gender = x.Gender,
                            GlareTest_BCV_OD = x.GlareTest_BCV_OD,
                            GlareTest_BCV_OS = x.GlareTest_BCV_OS,
                            GlareTest_BCV_OU = x.GlareTest_BCV_OU,
                            HX_BCV_OD = x.HX_BCV_OD,
                            HX_BCV_OS = x.HX_BCV_OD,
                            HX_BCV_OU = x.HX_BCV_OD,
                            IsRegistration = x.IsRegistration,
                            IsVerified = x.IsVerified,
                            Nationality = x.Nationality,
                            OptometristFirmId = x.OptometristFirmId,
                            OptometristFirmName = x.OptometristFirmName,
                            OptometristName = x.OptometristName,
                            OtherName = x.OtherName,
                            PassOrFail = x.PassOrFail,
                            PassportImageUrl = x.PassportImageUrl,
                            PassResult = x.PassResult,
                            PathologicalRemarks = x.PathologicalRemarks,
                            PostalAddress = x.PostalAddress,
                            ReferenceNumber = x.ReferenceNumber,
                            ResultConclusion = x.ResultConclusion,
                            ResultServiceType = x.ResultServiceType,
                            SingleImage_BCV_OU = x.SingleImage_BCV_OU,
                            Status = x.Status,
                            Surname = x.Surname,
                            TestDate = x.TestDate,
                            TestType = x.TestType,
                            TransmittedDate = x.TransmittedDate,
                            Unaided_OD = x.Unaided_OD,
                            Unaided_OS = x.Unaided_OS,
                            Unaided_OU = x.Unaided_OU,
                            VerifiedDate = x.VerifiedDate,
                            VisualAssessmentResultId = x.VisualAssessmentResultId
                        });
                        int i = 0;
                        foreach (var item in records)
                        {
                            i++;
                           bool exist = context.VisualAssessmentResults.Any(x => x.ReferenceNumber == item.ReferenceNumber);
                            if (exist) continue;

                            context.VisualAssessmentResults.Add(item);
                            context.SaveChanges();
                            lblMessage.Text = $"{i} records inserted successfully";
                        }
                    }
                    else
                    {
                        MessageBox.Show("Unable to connect. Please check your connection string.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }
    }
}
