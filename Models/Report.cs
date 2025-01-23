namespace Bazaarly.Models
{
    public class Report
    {
        public int ReportId { get; set; }
        public int ProductId { get; set; }
        public string Reason { get; set; }
        public string Details { get; set; }
        public DateTime DateReported { get; set; }
    }
}

