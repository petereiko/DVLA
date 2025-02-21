using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data.Models.DataObjects.DTOs
{
    public class EmailTokenDto
    {
        public string TokenName { get; set; }

        public string Token { get; set; }

        public string TokenValue { get; set; }
    }
}
