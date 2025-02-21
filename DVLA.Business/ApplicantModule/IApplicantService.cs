using DVLA.Data.Models.DataObjects.DTOs;
using DVLA.Data.Models.DataObjects.UtilityObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Business.ApplicantModule
{
    public interface IApplicantService
    {
        ApplicantModel Get(long id);
        MessageResponse Update(ApplicantModel model, string Id);
    }
}
