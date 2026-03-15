using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.VerificationPortal.Shared.Responses
{
    public class DvlaResponse
    {
        public string? status { get; set; }
        public string? msg { get; set; }
        public string? code { get; set; }
        public object? data { get; set; }
        //      "status": "ok",
        //"msg": "Eye Test Result Saved Successfully",
        //"code": "00",
        //"data": []
    }
}
