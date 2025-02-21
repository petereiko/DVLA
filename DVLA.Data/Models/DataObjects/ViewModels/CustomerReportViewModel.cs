using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data.Models.DataObjects.ViewModels
{
    public class CustomerReportViewModel
    {
        public string ReferenceNumber { get; set; }
        public string FullName { get; set; }
        public string DriversLicence { get; set; }
        public string TaxIdentificationNumber { get; set; }
        public string DVLAReferenceNo { get; set; }
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
        public string Grade { get; set; }
        public string CreatedOn { get; set; }

    }
}
