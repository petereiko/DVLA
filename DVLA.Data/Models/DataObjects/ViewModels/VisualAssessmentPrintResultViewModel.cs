using DVLA.Data.Models.Enumerables;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data.Models.DataObjects.ViewModels
{
    public class VisualAssessmentPrintResultViewModel
    {
        public VisualAssessmentPrintResultViewModel()
        {
            //PassportImageUrl = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8Vw8AAoEBfymqrywAAAAASUVORK5CYII=";
            Status = Enumerables.Status.InProgress;
        }

        public long? Id { get; set; }
        public long OptometristFirmId { get; set; }
        public string ReferenceNumber { get; set; }
        ///public string FormNumber { get; set; }
        public string ResultServiceType { get; set; }
        public string LearnerDriversLicenceType { get; set; }
        public string NameTitle { get; set; }
        public string Surname { get; set; }
        //public string DriversLicence { get; set; }
        //public string DVLAReferenceNo { get; set; }
        public string TestType { get; set; }
        //public string OldDVLAReferenceNo { get; set; }
        public string FirstName { get; set; }
        public string OtherName { get; set; }
        //[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime? DOB { get; set; }
        public string PassOrFail { get; set; }
        public long? PassResultId { get; set; }
        public string PassResult { get; set; }
        public string PostalAddress { get; set; }
        public string ContactNumber { get; set; }
        public string TaxIdentificationNumber { get; set; }
        public string Email { get; set; }
        public string Unaided_OD { get; set; }
        public string Unaided_OS { get; set; }
        public string Unaided_OU { get; set; }
        public string BCV_OD { get; set; }
        public string BCV_OS { get; set; }
        public string BCV_OU { get; set; }
        public string HX_BCV_OD { get; set; }
        public string HX_BCV_OS { get; set; }
        public string HX_BCV_OU { get; set; }
        public string SingleImage_BCV_OU { get; set; }
        public string GlareTest_BCV_OD { get; set; }
        public string GlareTest_BCV_OS { get; set; }
        public string GlareTest_BCV_OU { get; set; }
        public string ColourVision_BCV_OU { get; set; }
        public string ContrastSensitivity_BCV { get; set; }
        public string PathologicalRemarks { get; set; }
        public string ResultConclusion { get; set; }
        public DateTime? TestDate { get; set; }
        public string PassportImageUrl { get; set; }
        public Status? Status { get; set; }
        public string ActionType { get; set; }
        public IFormFile Image { get; set; }
        public string DateOfBirth { get; set; }
    }
}
