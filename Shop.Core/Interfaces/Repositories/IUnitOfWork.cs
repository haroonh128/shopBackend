namespace Shop.Core.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IOtpRepository OtpCodes { get; }
        IRefreshTokenRepository RefreshTokens { get; }
        IAuditLogRepository AuditLogs { get; }
        ILicenseRepository Licenses { get; }
        ILicenseSubscriptionRepository LicenseSubscriptions { get; }
        IPackageRepository Packages { get; }
        IModuleRepository Modules { get; }
        IPackageModuleRepository PackageModules { get; }
        IProductRepository Products { get; }
        IAddressRepository Addresses { get; }
        IStoreItemRepository StoreItems { get; }
        IMeasurementRepository Measurements { get; }
        IOrderRepository Orders { get; }
        IClientRepository Clients { get; }
        ICostRepository Costs { get; }

        IPracticeRepository Practices { get; }
        IDoctorRepository Doctors { get; }
        IPatientRepository Patients { get; }
        IConsultationRepository Consultations { get; }
        IConsultationVitalsRepository ConsultationVitals { get; }
        IConsultationDiagnosisRepository ConsultationDiagnoses { get; }
        IConsultationTestRepository ConsultationTests { get; }
        IDiagnosisRepository Diagnoses { get; }
        ITestRepository Tests { get; }
        IMedicationRepository Medications { get; }
        IPrescriptionRepository Prescriptions { get; }
        IPrescriptionItemRepository PrescriptionItems { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
