using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.VerificationPortal.Domain.Entities
{
    public class ApiClient
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? ApiKey { get; set; }
        public bool IsActive { get; set; }
        public string? IP { get; set; }
    }
}
