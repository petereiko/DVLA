using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data.Models.DataObjects.PaystackDtos
{
    public class VerificationBadRequestResponse
    {
        public bool status { get; set; }
        public string message { get; set; }
        public VerificationBadRequestMeta meta { get; set; }
        public string type { get; set; }
        public string code { get; set; }
    }

    public class VerificationBadRequestMeta
    {
        public string nextStep { get; set; }
    }
}
