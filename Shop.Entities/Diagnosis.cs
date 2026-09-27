namespace Shop.Entities
{
    public class Diagnosis : Base
    {
        public Guid PracticeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Code { get; set; }
    }
}
