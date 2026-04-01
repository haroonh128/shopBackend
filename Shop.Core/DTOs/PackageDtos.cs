namespace Shop.Core.DTOs
{
    public class PackageResponse
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class ModuleResponse
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class PackageModuleResponse
    {
        public Guid Id { get; set; }
        public Guid PackageId { get; set; }
        public string? PackageCode { get; set; }
        public Guid ModuleId { get; set; }
        public string? ModuleCode { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreatePackageRequest
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

    public class UpdatePackageRequest
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

    public class CreateModuleRequest
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

    public class UpdateModuleRequest
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

    public class CreatePackageModuleRequest
    {
        public Guid PackageId { get; set; }
        public Guid ModuleId { get; set; }
    }
}
