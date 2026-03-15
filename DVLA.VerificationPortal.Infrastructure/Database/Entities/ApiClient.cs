using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.VerificationPortal.Infrastructure.Database.Entities
{
    public class ApiClient
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? ApiKey { get; set; }
    }
}
