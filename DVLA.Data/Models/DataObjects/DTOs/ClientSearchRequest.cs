using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data.Models.DataObjects.DTOs
{
    public class ClientSearchRequest
    {
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime EndDate { get; set; } = DateTime.Now;
        public int? OptometristFirmId {  get; set; }
        public string Search { get; set; }
        public int Entries {  get; set; }
    }
}
