namespace Shop.Entities
{
    public class License:Base
    {
        public string Key { get; set; } = string.Empty;
        public long ProductId { get; set; }
        public long UserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
