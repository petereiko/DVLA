using DVLA.Data.Models.DataObjects.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data.Models.DataObjects.ViewModels
{
    public class ClientViewModel
    {
        public ClientSearchParameter SearchParameter { get; set; }
        public List<ClientModel> Clients { get; set; } = new();
    }
}
