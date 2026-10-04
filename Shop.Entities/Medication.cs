namespace Shop.Entities
{
    public class Medication : Base
    {
        public Guid PracticeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? GenericName { get; set; }
        public string? Strength { get; set; }
        public string? Form { get; set; }
    }
}
