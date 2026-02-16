namespace Shop.Entities
{
    
        public class PackageModule : Base
        {
            public Guid PackageId { get; set; }
            public Package Package { get; set; } = null!;

            public Guid ModuleId { get; set; }
        }
}
