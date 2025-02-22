using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.VerificationPortal.Shared.DTOs
{
    public class EmailLogDto
    {
        public string Email { get; set; }
        public string Message { get; set; }
        public string Subject { get; set; }
        public string Url { get; set; }
    }
}
