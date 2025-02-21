using DVLA.Data.Models.BaseFolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data.Models.Domains
{
    public class InitiatePaystackTransferResponse:BaseObjectInt64
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public string AuthorizationUrl {  get; set; }
        public string AccessCode {  get; set; }
        public string Reference { get; set; }
        public long? InitiatePaystackTransferRequestId { get; set; }
        public virtual InitiatePaystackTransferRequest InitiatePaystackTransferRequest { get; set; }
    }
}
