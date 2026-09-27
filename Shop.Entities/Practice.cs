namespace Shop.Entities
{
    public class Practice : Base
    {
        public Guid UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Website { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }
    }
}
