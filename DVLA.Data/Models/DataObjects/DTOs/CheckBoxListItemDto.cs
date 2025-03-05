using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data.Models.DataObjects.DTOs
{
    public class CheckBoxListItemDto
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public bool IsChecked { get; set; }
    }
}
