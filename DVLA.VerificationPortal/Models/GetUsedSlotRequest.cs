namespace DVLA.VerificationPortal.Models
{
    public class GetUsedSlotRequest
    {
        public DateTime StartDate { get; set; } = DateTime.UtcNow.AddMonths(-1);
        public DateTime EndDate { get; set; } = DateTime.UtcNow;
        public int? OptometristFirmId { get; set; }
    }
}
