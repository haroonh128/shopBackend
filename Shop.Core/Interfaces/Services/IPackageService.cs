using Shop.Core.DTOs;

namespace Shop.Core.Interfaces.Services
{
    public interface IPackageService
    {
        Task<BaseResponse<PackageResponse>> GetPackageByIdAsync(Guid id);
        Task<BaseResponse<IEnumerable<PackageResponse>>> GetAllPackagesAsync();
        Task<BaseResponse<PackageResponse>> CreatePackageAsync(CreatePackageRequest request);
        Task<BaseResponse<PackageResponse>> UpdatePackageAsync(Guid id, UpdatePackageRequest request);
        Task<BaseResponse<bool>> DeletePackageAsync(Guid id);

        Task<BaseResponse<ModuleResponse>> GetModuleByIdAsync(Guid id);
        Task<BaseResponse<IEnumerable<ModuleResponse>>> GetAllModulesAsync();
        Task<BaseResponse<ModuleResponse>> CreateModuleAsync(CreateModuleRequest request);
        Task<BaseResponse<ModuleResponse>> UpdateModuleAsync(Guid id, UpdateModuleRequest request);
        Task<BaseResponse<bool>> DeleteModuleAsync(Guid id);

        Task<BaseResponse<PackageModuleResponse>> GetPackageModuleByIdAsync(Guid id);
        Task<BaseResponse<IEnumerable<PackageModuleResponse>>> GetPackageModulesByPackageIdAsync(Guid packageId);
        Task<BaseResponse<PackageModuleResponse>> CreatePackageModuleAsync(CreatePackageModuleRequest request);
        Task<BaseResponse<bool>> DeletePackageModuleAsync(Guid id);
    }
}
