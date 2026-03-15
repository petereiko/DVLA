using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.VerificationPortal.Shared.Responses
{
    public class GenesysResponse
    {
        public string? status { get; set; }
        public string? msg { get; set; }
        public string? code { get; set; }
        public object? data { get; set; }
    }
}
