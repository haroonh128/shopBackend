namespace Shop.Core.DTOs
{
    public class AddressResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string AddressLine1 { get; set; } = string.Empty;
        public string AddressLine2 { get; set; } = string.Empty;
        public string AddresseeId { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Zipcode { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class CreateAddressRequest
    {
        public string Name { get; set; } = string.Empty;
        public string AddressLine1 { get; set; } = string.Empty;
        public string AddressLine2 { get; set; } = string.Empty;
        public string AddresseeId { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Zipcode { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }

    public class UpdateAddressRequest
    {
        public string Name { get; set; } = string.Empty;
        public string AddressLine1 { get; set; } = string.Empty;
        public string AddressLine2 { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Zipcode { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }
}
