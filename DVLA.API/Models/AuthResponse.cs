using DVLA.Data.Models.DataObjects.ViewModels;
using System;

namespace DVLA.API.Models
{
    public class AuthResponse
    {
        public string AccessToken { get; set; }
        public DateTime ExpiresAtUtc { get; set; }
        public UserViewModel User { get; set; }
    }
}
