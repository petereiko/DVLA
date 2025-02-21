using DVLA.Data.Models.BaseFolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.DATA.Domains
{
    public class QueryBuilderItems : BaseObjectInt64
    {
		public long QueryBuilderID { get; set; }
		public string ParameterName { get; set; }
		public string ParameterValue { get; set; }
		public string ParameterSelected { get; set; }
		public string Operator { get; set; }
		public string OperatorID { get; set; }
		public int ParameterSource { get; set; }
		public int FieldType { get; set; }
		public string JoinType { get; set; }
		public string LeftBracket { get; set; }
		public string RightBracket { get; set; }
		public string Alias { get; set; }
	}
}
