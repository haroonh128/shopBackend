namespace Shop.Entities
{
    public class ConsultationTest : Base
    {
        public Guid ConsultationId { get; set; }
        public Guid? TestId { get; set; }
        public string TestName { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }
}
