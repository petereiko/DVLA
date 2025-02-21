namespace WinApp.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class EmailLog
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public EmailLog()
        {
            EmailLogAttachments = new HashSet<EmailLogAttachment>();
        }

        public long Id { get; set; }

        public int RetryCount { get; set; }

        public string Sender { get; set; }

        public string Recepient { get; set; }

        public string Cc { get; set; }

        public string Bcc { get; set; }

        public string Subject { get; set; }

        public string Message { get; set; }

        public bool IsSent { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? DateSent { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime DateToSend { get; set; }

        public bool HasAttachment { get; set; }

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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmailLogAttachment> EmailLogAttachments { get; set; }
    }
}
