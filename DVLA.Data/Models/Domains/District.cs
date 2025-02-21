

using DVLA.Data.Models.BaseFolder;
using System.ComponentModel.DataAnnotations;

namespace DVLA.DATA.Domains
{
    public class District
    {
        public int Id { get; set; }

        [StringLength(50)]
        public string Name { get; set; }
        public int RegionId { get; set; }
        public virtual Region Region { get; set; }
    }
}
