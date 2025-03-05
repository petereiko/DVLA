

using DVLA.Data.Models.BaseFolder;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DVLA.DATA.Domains
{
    public class District
    {
        public District()
        {
            OptometristFirms = new HashSet<OptometristFirm>();
        }
        public int Id { get; set; }

        [StringLength(50)]
        public string Name { get; set; }
        public int RegionId { get; set; }
        public virtual Region Region { get; set; }
        public virtual ICollection<OptometristFirm> OptometristFirms { get; set; }
    }
}
