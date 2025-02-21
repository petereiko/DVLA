using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data.Models.DataObjects.ViewModels
{
    public class VisualAssessmentResultUploadViewModel:BaseViewModel
    {
        [Required(ErrorMessage = "Browse for file")]
        public IFormFile file { get; set; }

    }
}
