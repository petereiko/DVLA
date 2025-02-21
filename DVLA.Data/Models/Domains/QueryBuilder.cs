using DVLA.Data.Models.BaseFolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.DATA.Domains
{
    public class QueryBuilder : BaseObjectInt64
	{
		public int ReportType { get; set; }
		public string DateGenerated { get; set; }
		public string DateLastModified { get; set; }
		public string QueryName { get; set; }
		public string Description { get; set; }
		public string TransactionSelectFields { get; set; }
		public string TransactionWhereClause { get; set; }
	}
}
