namespace Shop.Entities
{
    public class Product : Base
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string RegistrationNumber { get; set; } = string.Empty;
        public string TaxId { get; set; } = string.Empty;
        public string Industry { get; set; } = string.Empty;
        public string Website { get; set; } = string.Empty;
    }
}
