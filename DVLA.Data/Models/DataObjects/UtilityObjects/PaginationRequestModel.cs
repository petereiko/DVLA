using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data.Models.DataObjects.UtilityObjects
{
    public class PaginationRequestModel
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }

    public class PaginationRequestModel<T> where T : class
    {
        public T InputModel { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }
}
