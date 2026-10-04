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
        public DbSet<Measurements> Measurements { get; set; }
        public DbSet<Orders> Orders { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Cost> Costs { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<Modules> Modules { get; set; }
        public DbSet<PackageModule> PackageModules { get; set; }
        public DbSet<LicenseSubscription> LicenseSubscriptions { get; set; }
        public DbSet<License> Licenses { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<StoreItem> ShopInventory { get; set; }

        public DbSet<Practice> Practices { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Consultation> Consultations { get; set; }
        public DbSet<ConsultationVitals> ConsultationVitals { get; set; }
        public DbSet<Diagnosis> Diagnoses { get; set; }
        public DbSet<ConsultationDiagnosis> ConsultationDiagnoses { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<ConsultationTest> ConsultationTests { get; set; }
        public DbSet<Medication> Medications { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<PrescriptionItem> PrescriptionItems { get; set; }

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

            if (!Database.IsSqlServer())
            {
                modelBuilder.Entity<Client>()
                    .Property(e => e.Id)
                    .HasConversion<string>();

                modelBuilder.Entity<User>()
                    .Property(e => e.Id)
                    .HasConversion<string>();
            }

            if (Database.IsSqlServer())
            {
                modelBuilder.Entity<Client>()
                    .Property(e => e.Id)
                    .HasColumnType("uniqueidentifier");

                modelBuilder.Entity<User>()
                    .Property(e => e.Id)
                    .HasColumnType("uniqueidentifier");
            }

            ConfigureClinicModel(modelBuilder);
        }

        private static void ConfigureClinicModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Practice>(e =>
            {
                e.HasIndex(x => x.UserId);
                e.Property(x => x.Name).HasMaxLength(200);
                e.Property(x => x.LogoUrl).HasMaxLength(500);
            });

            modelBuilder.Entity<Doctor>(e =>
            {
                e.HasIndex(x => x.PracticeId);
                e.Property(x => x.FullName).HasMaxLength(200);
                e.Property(x => x.DefaultConsultationFee).HasPrecision(18, 2);
                e.HasOne<Practice>().WithMany().HasForeignKey(x => x.PracticeId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Patient>(e =>
            {
                e.HasIndex(x => x.PracticeId);
                e.HasIndex(x => new { x.PracticeId, x.MRNumber }).IsUnique();
                e.HasIndex(x => x.ContactNumber);
                e.HasIndex(x => x.FullName);
                e.Property(x => x.MRNumber).HasMaxLength(50);
                e.Property(x => x.FullName).HasMaxLength(200);
                e.Property(x => x.ContactNumber).HasMaxLength(50);
                e.Property(x => x.Allergies).HasMaxLength(1000);
                e.Property(x => x.Notes).HasMaxLength(2000);
                e.Property(x => x.Height).HasPrecision(18, 2);
                e.Property(x => x.Weight).HasPrecision(18, 2);
                e.Property(x => x.BMI).HasPrecision(18, 2);
                e.HasOne<Practice>().WithMany().HasForeignKey(x => x.PracticeId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Consultation>(e =>
            {
                e.HasIndex(x => x.PracticeId);
                e.HasIndex(x => x.PatientId);
                e.HasIndex(x => x.DoctorId);
                e.HasIndex(x => x.VisitDate);
                e.HasIndex(x => new { x.PracticeId, x.VisitNumber }).IsUnique();
                e.Property(x => x.VisitNumber).HasMaxLength(50);
                e.Property(x => x.ChiefComplaint).HasMaxLength(2000);
                e.Property(x => x.History).HasMaxLength(4000);
                e.Property(x => x.Examination).HasMaxLength(4000);
                e.Property(x => x.Observations).HasMaxLength(4000);
                e.Property(x => x.DiagnosisNotes).HasMaxLength(2000);
                e.Property(x => x.Advice).HasMaxLength(4000);
                e.Property(x => x.FollowUpNotes).HasMaxLength(2000);
                e.Property(x => x.ConsultationFee).HasPrecision(18, 2);
                e.HasOne<Practice>().WithMany().HasForeignKey(x => x.PracticeId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne<Patient>().WithMany().HasForeignKey(x => x.PatientId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne<Doctor>().WithMany().HasForeignKey(x => x.DoctorId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ConsultationVitals>(e =>
            {
                e.HasIndex(x => x.ConsultationId).IsUnique();
                e.Property(x => x.Temperature).HasPrecision(18, 2);
                e.Property(x => x.OxygenSaturation).HasPrecision(18, 2);
                e.Property(x => x.Height).HasPrecision(18, 2);
                e.Property(x => x.Weight).HasPrecision(18, 2);
                e.Property(x => x.BMI).HasPrecision(18, 2);
                e.HasOne<Consultation>().WithMany().HasForeignKey(x => x.ConsultationId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Diagnosis>(e =>
            {
                e.HasIndex(x => x.PracticeId);
                e.Property(x => x.Name).HasMaxLength(200);
                e.Property(x => x.Code).HasMaxLength(50);
                e.HasOne<Practice>().WithMany().HasForeignKey(x => x.PracticeId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ConsultationDiagnosis>(e =>
            {
                e.HasIndex(x => x.ConsultationId);
                e.Property(x => x.DiagnosisName).HasMaxLength(200);
                e.Property(x => x.Notes).HasMaxLength(1000);
                e.HasOne<Consultation>().WithMany().HasForeignKey(x => x.ConsultationId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne<Diagnosis>().WithMany().HasForeignKey(x => x.DiagnosisId).OnDelete(DeleteBehavior.Restrict).IsRequired(false);
            });

            modelBuilder.Entity<Test>(e =>
            {
                e.HasIndex(x => x.PracticeId);
                e.Property(x => x.Name).HasMaxLength(200);
                e.Property(x => x.Category).HasMaxLength(100);
                e.HasOne<Practice>().WithMany().HasForeignKey(x => x.PracticeId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ConsultationTest>(e =>
            {
                e.HasIndex(x => x.ConsultationId);
                e.Property(x => x.TestName).HasMaxLength(200);
                e.Property(x => x.Notes).HasMaxLength(1000);
                e.HasOne<Consultation>().WithMany().HasForeignKey(x => x.ConsultationId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne<Test>().WithMany().HasForeignKey(x => x.TestId).OnDelete(DeleteBehavior.Restrict).IsRequired(false);
            });

            modelBuilder.Entity<Medication>(e =>
            {
                e.HasIndex(x => x.PracticeId);
                e.Property(x => x.Name).HasMaxLength(200);
                e.Property(x => x.GenericName).HasMaxLength(200);
                e.Property(x => x.Strength).HasMaxLength(100);
                e.Property(x => x.Form).HasMaxLength(100);
                e.HasOne<Practice>().WithMany().HasForeignKey(x => x.PracticeId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Prescription>(e =>
            {
                e.HasIndex(x => x.ConsultationId).IsUnique();
                e.Property(x => x.Notes).HasMaxLength(2000);
                e.HasOne<Consultation>().WithMany().HasForeignKey(x => x.ConsultationId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<PrescriptionItem>(e =>
            {
                e.HasIndex(x => x.PrescriptionId);
                e.Property(x => x.MedicationName).HasMaxLength(200);
                e.Property(x => x.Strength).HasMaxLength(100);
                e.Property(x => x.Dosage).HasMaxLength(100);
                e.Property(x => x.Frequency).HasMaxLength(100);
                e.Property(x => x.Route).HasMaxLength(100);
                e.Property(x => x.Duration).HasMaxLength(100);
                e.Property(x => x.Quantity).HasMaxLength(100);
                e.Property(x => x.Instructions).HasMaxLength(1000);
                e.HasOne<Prescription>().WithMany().HasForeignKey(x => x.PrescriptionId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne<Medication>().WithMany().HasForeignKey(x => x.MedicationId).OnDelete(DeleteBehavior.Restrict).IsRequired(false);
            });
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder builder)
        {
            builder.Properties<DateTime>()
                .HaveConversion<DateTime>();

            builder.Properties<string>().HaveMaxLength(255);

            builder.Properties<bool>();

            builder.Properties<decimal>()
               .HavePrecision(18, 2);
        }
    }
}
