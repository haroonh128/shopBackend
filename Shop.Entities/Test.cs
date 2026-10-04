namespace Shop.Entities
{
    public class Test : Base
    {
        public Guid PracticeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Category { get; set; }
    }
}
