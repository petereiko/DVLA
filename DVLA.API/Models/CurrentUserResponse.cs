using System.Collections.Generic;

namespace DVLA.API.Models
{
    public class CurrentUserResponse
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string OptometristFirmId { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}
