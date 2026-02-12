using Microsoft.EntityFrameworkCore;
using Shop.Entities;

namespace Shop.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<OtpCode> OtpCodes { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Expenses> Expenses { get; set; }
        public DbSet<Khata> Khatas { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<StoreItems> StoreItems { get; set; }
        public DbSet<Measurments> Measurments { get; set; }
        public DbSet<Client> Clients { get; set; }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }

        private void UpdateTimestamps()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is Base &&
                           (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                var entity = (Base)entry.Entity;

                if (entry.State == EntityState.Added)
                {
                    entity.CreatedAt = DateTime.UtcNow;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entity.UpdatedAt = DateTime.UtcNow;
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // -----------------------------
            // PROVIDER-SPECIFIC FIXES
            // -----------------------------

            if (!Database.IsSqlServer())
            {
                // SQLite: store GUIDs as TEXT
                modelBuilder.Entity<Client>()
                    .Property(e => e.Id)
                    .HasConversion<string>();

                modelBuilder.Entity<User>()
                    .Property(e => e.Id)
                    .HasConversion<string>();

                // repeat ONLY for entities with Guid PKs
            }

            if (Database.IsSqlServer())
            {
                // SQL Server: native GUIDs
                modelBuilder.Entity<Client>()
                    .Property(e => e.Id)
                    .HasColumnType("uniqueidentifier");

                modelBuilder.Entity<User>()
                    .Property(e => e.Id)
                    .HasColumnType("uniqueidentifier");
            }
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder builder)
        {
            var db = Database.ProviderName;

            builder.Properties<DateTime>()
                .HaveConversion<DateTime>();

            builder.Properties<string>().HaveMaxLength(255);

            builder.Properties<bool>();

            builder.Properties<decimal>()
               .HavePrecision(18, 2);
        }
    }
}
