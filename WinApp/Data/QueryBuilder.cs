namespace WinApp.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class QueryBuilder
    {
        public long Id { get; set; }

        public int ReportType { get; set; }

        public string DateGenerated { get; set; }

        public string DateLastModified { get; set; }

        public string QueryName { get; set; }

        public string Description { get; set; }

        public string TransactionSelectFields { get; set; }

        public string TransactionWhereClause { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime CreatedDate { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? ModifiedDate { get; set; }

        public string CreatedBy { get; set; }

        public string ModifiedBy { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        [Column(TypeName = "timestamp")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [MaxLength(8)]
        public byte[] RowVersion { get; set; }
    }
}
