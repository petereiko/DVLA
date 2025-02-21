using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data.Models.DataObjects.UtilityObjects
{
    public class PaginationResponseModel<T> where T : new()
    {
        public PaginationResponseModel() { }
        public PaginationResponseModel(int totalCount, int pageSize, int filteredRecords)
        {

            ListResult = new T();
            TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            offset = (pageSize - 1) * pageSize;
            StartIndex = offset + 1;
            EndIndex= Math.Min(offset + pageSize, filteredRecords);
        }
        public T ListResult { get; set; }
        public int TotalCount { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public int TotalPages { get; set; }

        private int offset;

        public int FilteredRecords { get; set; }  // Filtered records that match the current query
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
    }
}
