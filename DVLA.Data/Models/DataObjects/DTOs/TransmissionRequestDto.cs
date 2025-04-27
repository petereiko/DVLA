using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data.Models.DataObjects.DTOs
{
    public class TransmissionRequestDto
    {
        public string SourceConnectionString { get; set; }
        public string SqlQuery { get; set; }
        public string DestinationConnectionString { get; set; }

    }
}
