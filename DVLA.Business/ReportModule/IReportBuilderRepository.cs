using DVLA.Data.Models.DataObjects.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Business.ReportModule
{
    public interface IReportBuilderRepository
    {
        List<QueryBuilderModel> FetchQueryBuilder(int reportType, long id);
    }
}
