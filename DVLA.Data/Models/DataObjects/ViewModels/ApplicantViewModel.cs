using DVLA.Data.Models.Enumerables;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data.Models.DataObjects.ViewModels
{
    public class ApplicantViewModel
    {
        public ApplicantViewModel()
        {
            Status = Enumerables.Status.InProgress;
        }
        public long Id { get; set; }
        public long OptometristFirmId { get; set; }
        public ResultServiceType? ResultServiceType { get; set; }
        public LearnerDriversLicenceType? LearnerDriversLicence { get; set; }
        public string Surname { get; set; }
        public string FirstName { get; set; }
        public string OtherName { get; set; }
        [DataType(DataType.Date)]
        public DateTime? DOB { get; set; }
        public string PostalAddress { get; set; }
        public string ContactNumber { get; set; }
        public string TaxIdentificationNumber { get; set; }
        public string Email { get; set; }
        public Status? Status { get; set; }
        public string ActionType { get; set; }
        public string DateOfBirth { get; set; }
    }
}
