namespace Shop.Entities
{
    public class ConsultationVitals : Base
    {
        public Guid ConsultationId { get; set; }
        public decimal? Temperature { get; set; }
        public int? SystolicBP { get; set; }
        public int? DiastolicBP { get; set; }
        public int? Pulse { get; set; }
        public int? RespiratoryRate { get; set; }
        public decimal? OxygenSaturation { get; set; }
        public decimal? Height { get; set; }
        public decimal? Weight { get; set; }
        public decimal? BMI { get; set; }
    }
}
