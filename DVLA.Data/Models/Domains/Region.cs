using DVLA.Data.Models.BaseFolder;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DVLA.DATA.Domains
{

    public partial class Region
    {
        public Region()
        {
            Districts = new HashSet<District>();
        }
        public int Id { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        public string PrefixName { get; set; }
        public virtual ICollection<District> Districts { get; set; }

}
}
