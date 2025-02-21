using DVLA.Data.Models.DataObjects.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Business.UserModule
{
    public interface IAuditRepo
    {
        void AddAudit(long moduleActionId, string description);

        Task<List<ActivityModel>> GetAudit(AuditFilterModel model);
    }
}
