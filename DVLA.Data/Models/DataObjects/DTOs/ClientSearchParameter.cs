using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data.Models.DataObjects.DTOs
{
    public class ClientSearchParameter
    {
        public string DVLANumber { get; set; }
        public string DriversLicenceNumber { get; set; }
        public string TestCenter { get; set; }
        public string ApplicantName { get; set; }
        public string ReferenceNumber { get; set; }
    }
}
