using DVLA.Data.Models.BaseFolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data.Models.Domains
{
    public class InitiatePaystackTransferRequest:BaseObjectInt64
    {
        public int OptometristFirmId { get; set; }
        public decimal Amount { get; set; }
        public string Email { get; set; }
        public string Reference { get; set; }
    }
}
