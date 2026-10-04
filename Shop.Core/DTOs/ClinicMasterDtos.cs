namespace Shop.Core.DTOs
{
    public class DiagnosisResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Code { get; set; }
        public bool Active { get; set; }
    }

    public class DiagnosisRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Code { get; set; }
        public bool Active { get; set; } = true;
    }

    public class TestResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Category { get; set; }
        public bool Active { get; set; }
    }

    public class TestRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Category { get; set; }
        public bool Active { get; set; } = true;
    }

    public class MedicationResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? GenericName { get; set; }
        public string? Strength { get; set; }
        public string? Form { get; set; }
        public bool Active { get; set; }
    }

    public class MedicationRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? GenericName { get; set; }
        public string? Strength { get; set; }
        public string? Form { get; set; }
        public bool Active { get; set; } = true;
    }
}
