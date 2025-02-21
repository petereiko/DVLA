using DVLA.Data.Models.DataObjects.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data.Models.DataObjects.DTOs
{
    public class EmailTemplateDto:BaseViewModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string EmailName { get; set; }
        [Required]


        public string EmailBody { get; set; }
        [Required]
        public string EmailSubject { get; set; }
    }
}
