using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data.Models.DataObjects.DTOs
{
    public class ClientModel
    {
        public string FullName { get; set; }
        public string ContactNumber { get; set; }
        public string ReferenceNumber { get; set; }
        public string PostalAddress { get; set; }
        public string OptometristCenter { get; set; }
        public string Email { get; set; }
        public DateTime? TestDate { get; set; }
        public string DriversLicence { get; set; }
        public string DVLAReferenceNo { get; set; }
        public string Region { get; set; }
    }
}
