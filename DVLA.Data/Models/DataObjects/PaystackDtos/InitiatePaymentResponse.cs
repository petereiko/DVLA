using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data.Models.DataObjects.PaystackDtos
{
    public class InitiatePaymentResponse
    {
        public bool status { get; set; }
        public string message { get; set; }
        public InitiatePaymentData data { get; set; }
    }
}
