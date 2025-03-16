using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data.Models.DataObjects.ViewModels
{
    public class ForgotPasswordViewModel
    {
        public string Email { get; set;}
        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }
    }
}
