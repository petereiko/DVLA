using DVLA.Data.Models.Enumerables;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data.Models.DataObjects.DTOs
{
    public class VisualAssessmentExportDto
    {
        public long Id { get; set; }
        public string OptometristFirm { get; set; }

        [StringLength(50)]
        public string ReferenceNumber { get; set; }
        public string ResultServiceType { get; set; }
        //public string TestType { get; set; }

        public string PassOrFail { get; set; }

        [StringLength(50)]
        public string Surname { get; set; }

        [StringLength(50)]
        public string FirstName { get; set; }

        [StringLength(50)]
        public string OtherName { get; set; }

        public DateTime? DOB { get; set; }

        [StringLength(500)]
        public string PostalAddress { get; set; }

        [StringLength(50)]
        public string ContactNumber { get; set; }

        [StringLength(50)]
        public string Nationality { get; set; }

        [StringLength(50)]
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

        //public bool? IsSynchronized { get; set; }

        public DateTime? TestDate { get; set; }

        public string PassportImageUrl { get; set; }
        public string Status { get; set; }
        //public bool? IsRegistration { get; set; }
        public string AccessType { get; set; }
        public string PassResult { get; set; }
        //public bool IsTransmitted { get; set; }
        //public DateTime TransmittedDate { get; set; }
        public string Gender { get; set; }
        //public string TransmissionError { get; set; }
        //public bool HasTransmissionError { get; set; }
    }
}
