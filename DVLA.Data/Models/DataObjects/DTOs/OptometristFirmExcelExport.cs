using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data.Models.DataObjects.DTOs
{
    public class OptometristFirmExcelExport
    {
        //BusinessName	BusinessRegistrationNumber	DigitalAddress	BusinessTelephoneNumber	BusinessAddress	Town	DVLAAccreditationNumber	Region	District	ContactPersonLastName	ContactPersonOtherName	ContactPersonPhoneNumber	ContactPersonEmailAddress	CentreCode	ReorderLevel

        public string CentreCode { get; set; }
        public int? ReorderLevel { get; set; }
        public string BusinessAddress { get; set; }
        public string TelephoneNumber { get; set; }
        public string MobileNumber { get; set; }
        public string BusinessName { get; set; }
        public string AccreditationNumber { get; set; }
        public string RegistrationNumber { get; set; }
        public string DigitalAddress { get; set; }
        public string ContactFirstName { get; set; }
        public string ContactLastName { get; set; }
        public string ContactPhoneNumber { get; set; }
        public string ContactEmailAddress { get; set; }
        public string RegionName { get; set; }
        public string Town { get; set; }
        public string DistrictName { get; set; }
    }
}
