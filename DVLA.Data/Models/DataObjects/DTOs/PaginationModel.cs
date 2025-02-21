using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data.Models.DataObjects.DTOs
{
    public class PaginationModel
    {
        public string Action {  get; set; }
        public string Controller { get; set; }
        public string Area { get; set; }
        public int PageIndex { get; set;}
        public int TotalPages { get; set;}

        public int FilteredCount { get; set; }
        public int TotalCount { get; set; }
        public int EndIndex { get; set; }
    }
}
