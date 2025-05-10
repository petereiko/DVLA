using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DVLA.Data.Models.DataObjects.DTOs
{
    public class TempPasswordDto
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }

        [Required]
        public string Password { get; set; } = "optodrivedvla";

        public string UserId { get; set; }

        public List<string> Errors { get; set; } = new();

    }
}
