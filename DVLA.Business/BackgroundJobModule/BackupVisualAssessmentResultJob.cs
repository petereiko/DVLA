using DVLA.Data;
using DVLA.Data.Models.Domains;
using DVLA.DATA.Domains;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Business.BackgroundJobModule
{
    [DisallowConcurrentExecution]
    public class BackupVisualAssessmentResultJob : IJob
    {
        private readonly ILogger<BackupVisualAssessmentResultJob> _logger;
        private readonly DVLADbContext _context;

        public BackupVisualAssessmentResultJob(ILogger<BackupVisualAssessmentResultJob> logger, DVLADbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                _logger.LogInformation($"Back up Visual Assessment Result Started");

                IQueryable<VisualAssessmentResult> visualAssessmentResults = _context.VisualAssessmentResults.Where(x => x.TestDate <= DateTime.UtcNow.AddMonths(-3) && x.IsTransmitted); //_reportRepository.FetchAllPendingTransmissions();
                foreach (VisualAssessmentResult item in visualAssessmentResults)
                {
                    try
                    {
                        VisualAssessmentResultBackup backup = new VisualAssessmentResultBackup
                        {
                            OptometristFirmId = item.OptometristFirmId,
                            AccessType = item.AccessType,
                            ReferenceNumber = item.ReferenceNumber,
                            ResultServiceType = item.ResultServiceType,
                            TestType = item.TestType,
                            PassOrFail = item.PassOrFail,
                            Surname = item.Surname,
                            FirstName = item.FirstName,
                            OtherName = item.OtherName,
                            DOB = item.DOB,
                            BCV_OD = item.BCV_OD,
                            BCV_OS = item.BCV_OS,
                            BCV_OU = item.BCV_OU,
                            HX_BCV_OD = item.HX_BCV_OD,
                            HX_BCV_OS = item.HX_BCV_OS,
                            HX_BCV_OU = item.HX_BCV_OU,
                            SingleImage_BCV_OU = item.SingleImage_BCV_OU,
                            Unaided_OD = item.Unaided_OD,
                            Unaided_OS = item.Unaided_OS,
                            Unaided_OU = item.Unaided_OU,
                            PostalAddress = item.PostalAddress,
                            ContactNumber = item.ContactNumber,
                            ColourVision_BCV_OU = item.ColourVision_BCV_OU,
                            ContrastSensitivity_BCV = item.ContrastSensitivity_BCV,
                            GlareTest_BCV_OD = item.GlareTest_BCV_OD,
                            GlareTest_BCV_OS = item.GlareTest_BCV_OS,
                            GlareTest_BCV_OU = item.GlareTest_BCV_OU,
                            CreatedBy = item.CreatedBy,
                            CreatedDate = item.CreatedDate,
                            Email = item.Email,
                            Gender = item.Gender,
                            HasTransmissionError = item.HasTransmissionError,
                            IsActive = item.IsActive,
                            IsDeleted = item.IsDeleted,
                            IsTransmitted = item.IsTransmitted,
                            ModifiedBy = item.ModifiedBy,
                            ModifiedDate = item.ModifiedDate,
                            IsRegistration = item.IsRegistration,
                            IsSynchronized = item.IsSynchronized,
                            Nationality = item.Nationality,
                            TestDate = item.TestDate,
                            VisualAssessmentResultId = item.Id,
                            OptometristNameIsUpdate = item.OptometristNameIsUpdate,
                            ResultConclusion = item.ResultConclusion,
                            PassportImageUrl = item.PassportImageUrl,
                            PathologicalRemarks = item.PathologicalRemarks,
                            PassResult = item.PassResult,
                            Status = item.Status,
                            TransmissionError = item.TransmissionError,
                            TransmittedDate = item.TransmittedDate,
                            NationalID = item.NationalID,
                            PassportNumber = item.PassportNumber,
                            DvlaLicenseNumber = item.DvlaLicenseNumber
                        };
                        await _context.VisualAssessmentResultBackups.AddAsync(backup);
                        await _context.SaveChangesAsync();

                        VisualAssessmentResult result = await _context.VisualAssessmentResults.FirstOrDefaultAsync(x => x.Id == item.Id);
                        if (result != null)
                        {
                            result.IsBackedUp = true;
                            result.BackupDate = DateTime.UtcNow;
                            await _context.SaveChangesAsync();
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.Message, ex);
                        continue;
                    }

                }

                _logger.LogInformation($"Back up Visual Assessment Result Ended");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }
    }
}
