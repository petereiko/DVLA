using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data.Models.DataObjects.DTOs
{
    public class ClientSearchModel
    {
        public string OptometristPracticeName { get; set; }
        public string LocationAddress { get; set; }
        public string ReferenceNumber { get; set; }
        public string TelephoneNumber { get; set; }
        public string MobileNumber { get; set; }
        public string CompanyName { get; set; }
        public string OwnerName { get; set; }
        public string OwnerContactNumber { get; set; }
        public string Email { get; set; }
        public string Region { get; set; }
        public string Grade { get; set; }
        public string FullName { get; set; }
        public string DVLAReferenceNo { get; set; }
        public string DriversLicence { get; set; }
        public string CreatedOn { get; set; }
    }
}
