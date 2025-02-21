using DVLA.Data.Models.Enumerables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data.Models.DataObjects.PaystackDtos
{
    public class InitiatePaymentRequest
    {
        public string email { get; set; }
        public string amount { get; set; }
        public int accessType { get; set; }
        public int OptometristFirmId { get; set; }
        public string currency { get; set; } = "GHS";
        public string reference { get; set; }

        public string UserId { get; set; }
    }
}
