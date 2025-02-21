namespace WinApp.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class FormNumber
    {
        public long Id { get; set; }

        public long LastCount { get; set; }

        public int SerialType { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime DateCreated { get; set; }
    }
}
