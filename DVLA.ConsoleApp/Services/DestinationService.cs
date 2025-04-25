using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DVLA.ConsoleApp.DBContext;

namespace DVLA.ConsoleApp.Services
{
    public class DestinationService
    {
        private readonly DestinationDbContext _context;

        public DestinationService(DestinationDbContext context)
        {
            _context = context;
        }

        public bool ReferenceExist(string reference)
        {
            return _context.VisualAssessmentResults.Any(x=>x.ReferenceNumber == reference);
        }

        public void InsertRecords(List<VisualAssessmentResult> records)
        {
            try
            {
                foreach (var record in records)
                {
                    bool exists = ReferenceExist(record.ReferenceNumber);
                    if (exists) continue;

                    VisualAssessmentResult result = new()
                    {
                        AccessType = record.AccessType,
                        BCV_OD = record.BCV_OD,
                        ReferenceNumber = record.ReferenceNumber,
                        BCV_OS = record.BCV_OS,
                        BCV_OU = record.BCV_OU,
                        ColourVision_BCV_OU = record.ColourVision_BCV_OU,
                        ContactNumber = record.ContactNumber,
                        ContrastSensitivity_BCV = record.ContrastSensitivity_BCV,
                        CreatedBy = record.CreatedBy,
                        CreatedDate = record.CreatedDate,
                        DOB = record.DOB,
                        Email = record.Email,
                        FirstName = record.FirstName,
                        Gender = record.Gender,
                        GlareTest_BCV_OD = record.GlareTest_BCV_OD,
                        GlareTest_BCV_OS = record.GlareTest_BCV_OS,
                        GlareTest_BCV_OU = record.GlareTest_BCV_OU,
                        HX_BCV_OD = record.HX_BCV_OD,
                        HX_BCV_OS = record.HX_BCV_OS,
                        HX_BCV_OU = record.HX_BCV_OD,
                        IsRegistration = record.IsRegistration,
                        IsVerified = record.IsVerified,
                        Nationality = record.Nationality,
                        OptometristFirmId = record.OptometristFirmId,
                        OptometristFirmName = record.OptometristFirmName,
                        OptometristName = record.OptometristName,
                        OtherName = record.OtherName,
                        PassOrFail = record.PassOrFail,
                        PassportImageUrl = record.PassportImageUrl,
                        PassResult = record.PassResult,
                        PathologicalRemarks = record.PathologicalRemarks,
                        PostalAddress = record.PostalAddress,
                        ResultConclusion = record.ResultConclusion,
                        ResultServiceType = record.ResultServiceType,
                        SingleImage_BCV_OU = record.SingleImage_BCV_OU,
                        Status = record.Status,
                        Surname = record.Surname,
                        TestDate = record.TestDate,
                        TestType = record.TestType,
                        TransmittedDate = record.TransmittedDate,
                        Unaided_OD = record.Unaided_OD,
                        Unaided_OS = record.Unaided_OS,
                        Unaided_OU = record.Unaided_OU,
                        VerifiedDate = record.VerifiedDate,
                        VisualAssessmentResultId = record.VisualAssessmentResultId,
                    };
                    _context.VisualAssessmentResults.Add(result);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            
        }
    }
}
