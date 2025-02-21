using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data.Models.DataObjects.DTOs
{
    public class QueryBuilderModel
    {
        public long ID { get; set; }
        public string ReportType { get; set; }
        public string DateGenerated { get; set; }
        public string DateLastModified { get; set; }
        public string QueryName { get; set; }
        public string Description { get; set; }
        public string TransactionSelectFields { get; set; }
        public string TransactionWhereClause { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }

    }
}
