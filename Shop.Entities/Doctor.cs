namespace Shop.Entities
{
    public class Doctor : Base
    {
        public Guid PracticeId { get; set; }
        public Guid? UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Qualifications { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public string RegistrationNumber { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public decimal DefaultConsultationFee { get; set; }
    }
}
