using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data.Models
{
    public class RemindersModel
    {
        public long Id { get; set; }
        public string ReferenceNumber { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string ContactNumber { get; set; }
        public long PassResultId { get; set; }
        public DateTime TestDate { get; set; }
        public string DueDate { get; set; }
        public long ReminderId { get; set; }
    }
}
