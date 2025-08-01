using DVLA.Data.Models.Enumerables;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data.Models.DataObjects.ViewModels
{
    public class VisualAssessmentResultViewModel:BaseViewModel
    {
        public VisualAssessmentResultViewModel()
        {
            //PassportImageUrl = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8Vw8AAoEBfymqrywAAAAASUVORK5CYII=";
            Status = Enumerables.Status.InProgress;
            IdentityTypes = Enum.GetValues(typeof(IdentityType))
            .Cast<IdentityType>()
            .Select(e => new SelectListItem
            {
                Value = ((int)e).ToString(),
                Text = EnumHelper.GetDescription(e)
            }).ToList();
        }

        [Required(ErrorMessage ="Gender is required")]
        public Gender? Gender { get; set; }
        public string PassportUploadType { get; set; }
        public long Id { get; set; }
        public long OptometristFirmId { get; set; }
        public string ReferenceNumber { get; set; }
        public ResultServiceType? ResultServiceType { get; set; }
        public string Surname { get; set; }
        public TestType? TestType { get; set; }
        public string FirstName { get; set; }
        public string OtherName { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DOB { get; set; }
        public PassOrFail? PassOrFail { get; set; }
        public int? PassOrFailInt { get; set; }
        public PassResult? PassResult { get; set; }
        public string PostalAddress { get; set; }
        public string ContactNumber { get; set; }
        public string Nationality { get; set; }
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
        public Status? Action { get; set; }
        public bool? IsRegistration { get; set; }
        public string Filename { get; set; }
        //public string NationalID { get; set; }
        //public string PassportNumber { get; set; }

        [Required(ErrorMessage = "Please select an Identity Type.")] 
        public IdentityType IdentityType { get; set; }

        [Required(ErrorMessage = "Please enter identity number.")] 
        public string IdentityNumber { get; set; }

        public List<SelectListItem> IdentityTypes { get; set; }
        public string DvlaLicenseNumber { get; set; }
    }
}
