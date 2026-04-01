using Microsoft.Extensions.Logging;
using Shop.Core.DTOs;
using Shop.Core.Interfaces.Repositories;
using Shop.Core.Interfaces.Services;
using Shop.Entities;

namespace Shop.Infrastructure.Services
{
    public class PackageService : IPackageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PackageService> _logger;

        public PackageService(IUnitOfWork unitOfWork, ILogger<PackageService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<BaseResponse<PackageResponse>> GetPackageByIdAsync(Guid id)
        {
            try
            {
                var pkg = await _unitOfWork.Packages.GetByIdAsync(id);
                if (pkg == null)
                    return BaseResponse<PackageResponse>.ErrorResponse("Package not found");
                return BaseResponse<PackageResponse>.SuccessResponse(MapToPackageResponse(pkg));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting package {Id}", id);
                return BaseResponse<PackageResponse>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<IEnumerable<PackageResponse>>> GetAllPackagesAsync()
        {
            try
            {
                var list = (await _unitOfWork.Packages.GetAllAsync()).Select(MapToPackageResponse).ToList();
                return BaseResponse<IEnumerable<PackageResponse>>.SuccessResponse(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting packages");
                return BaseResponse<IEnumerable<PackageResponse>>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<PackageResponse>> CreatePackageAsync(CreatePackageRequest request)
        {
            try
            {
                if (await _unitOfWork.Packages.GetByCodeAsync(request.Code) != null)
                    return BaseResponse<PackageResponse>.ErrorResponse("Package code already exists");

                var pkg = new Package { Code = request.Code, Name = request.Name };
                var added = await _unitOfWork.Packages.AddAsync(pkg);
                await _unitOfWork.SaveChangesAsync();
                return BaseResponse<PackageResponse>.SuccessResponse(MapToPackageResponse(added), "Package created");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating package");
                return BaseResponse<PackageResponse>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<PackageResponse>> UpdatePackageAsync(Guid id, UpdatePackageRequest request)
        {
            try
            {
                var pkg = await _unitOfWork.Packages.GetByIdAsync(id);
                if (pkg == null)
                    return BaseResponse<PackageResponse>.ErrorResponse("Package not found");

                pkg.Code = request.Code;
                pkg.Name = request.Name;
                _unitOfWork.Packages.Update(pkg);
                await _unitOfWork.SaveChangesAsync();
                return BaseResponse<PackageResponse>.SuccessResponse(MapToPackageResponse(pkg), "Package updated");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating package {Id}", id);
                return BaseResponse<PackageResponse>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<bool>> DeletePackageAsync(Guid id)
        {
            try
            {
                var pkg = await _unitOfWork.Packages.GetByIdAsync(id);
                if (pkg == null)
                    return BaseResponse<bool>.ErrorResponse("Package not found");

                pkg.IsDeleted = true;
                pkg.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Packages.Update(pkg);
                await _unitOfWork.SaveChangesAsync();
                return BaseResponse<bool>.SuccessResponse(true, "Package deleted");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting package {Id}", id);
                return BaseResponse<bool>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<ModuleResponse>> GetModuleByIdAsync(Guid id)
        {
            try
            {
                var mod = await _unitOfWork.Modules.GetByIdAsync(id);
                if (mod == null)
                    return BaseResponse<ModuleResponse>.ErrorResponse("Module not found");
                return BaseResponse<ModuleResponse>.SuccessResponse(MapToModuleResponse(mod));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting module {Id}", id);
                return BaseResponse<ModuleResponse>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<IEnumerable<ModuleResponse>>> GetAllModulesAsync()
        {
            try
            {
                var list = (await _unitOfWork.Modules.GetAllAsync()).Select(MapToModuleResponse).ToList();
                return BaseResponse<IEnumerable<ModuleResponse>>.SuccessResponse(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting modules");
                return BaseResponse<IEnumerable<ModuleResponse>>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<ModuleResponse>> CreateModuleAsync(CreateModuleRequest request)
        {
            try
            {
                if (await _unitOfWork.Modules.GetByCodeAsync(request.Code) != null)
                    return BaseResponse<ModuleResponse>.ErrorResponse("Module code already exists");

                var mod = new Modules { Code = request.Code, Name = request.Name };
                var added = await _unitOfWork.Modules.AddAsync(mod);
                await _unitOfWork.SaveChangesAsync();
                return BaseResponse<ModuleResponse>.SuccessResponse(MapToModuleResponse(added), "Module created");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating module");
                return BaseResponse<ModuleResponse>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<ModuleResponse>> UpdateModuleAsync(Guid id, UpdateModuleRequest request)
        {
            try
            {
                var mod = await _unitOfWork.Modules.GetByIdAsync(id);
                if (mod == null)
                    return BaseResponse<ModuleResponse>.ErrorResponse("Module not found");

                mod.Code = request.Code;
                mod.Name = request.Name;
                _unitOfWork.Modules.Update(mod);
                await _unitOfWork.SaveChangesAsync();
                return BaseResponse<ModuleResponse>.SuccessResponse(MapToModuleResponse(mod), "Module updated");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating module {Id}", id);
                return BaseResponse<ModuleResponse>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<bool>> DeleteModuleAsync(Guid id)
        {
            try
            {
                var mod = await _unitOfWork.Modules.GetByIdAsync(id);
                if (mod == null)
                    return BaseResponse<bool>.ErrorResponse("Module not found");

                mod.IsDeleted = true;
                mod.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Modules.Update(mod);
                await _unitOfWork.SaveChangesAsync();
                return BaseResponse<bool>.SuccessResponse(true, "Module deleted");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting module {Id}", id);
                return BaseResponse<bool>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<PackageModuleResponse>> GetPackageModuleByIdAsync(Guid id)
        {
            try
            {
                var list = await _unitOfWork.PackageModules.FindAsync(pm => pm.Id == id);
                var pm = list.FirstOrDefault();
                if (pm == null)
                    return BaseResponse<PackageModuleResponse>.ErrorResponse("PackageModule not found");

                var pkg = await _unitOfWork.Packages.GetByIdAsync(pm.PackageId);
                var mod = await _unitOfWork.Modules.GetByIdAsync(pm.ModuleId);
                return BaseResponse<PackageModuleResponse>.SuccessResponse(new PackageModuleResponse
                {
                    Id = pm.Id,
                    PackageId = pm.PackageId,
                    PackageCode = pkg?.Code,
                    ModuleId = pm.ModuleId,
                    ModuleCode = mod?.Code,
                    CreatedAt = pm.CreatedAt
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting package module {Id}", id);
                return BaseResponse<PackageModuleResponse>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<IEnumerable<PackageModuleResponse>>> GetPackageModulesByPackageIdAsync(Guid packageId)
        {
            try
            {
                var list = await _unitOfWork.PackageModules.GetByPackageIdAsync(packageId);
                var response = list.Select(pm => new PackageModuleResponse
                {
                    Id = pm.Id,
                    PackageId = pm.PackageId,
                    PackageCode = pm.Package?.Code,
                    ModuleId = pm.ModuleId,
                    ModuleCode = pm.Module?.Code,
                    CreatedAt = pm.CreatedAt
                }).ToList();
                return BaseResponse<IEnumerable<PackageModuleResponse>>.SuccessResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting package modules for package {PackageId}", packageId);
                return BaseResponse<IEnumerable<PackageModuleResponse>>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<PackageModuleResponse>> CreatePackageModuleAsync(CreatePackageModuleRequest request)
        {
            try
            {
                var pkg = await _unitOfWork.Packages.GetByIdAsync(request.PackageId);
                if (pkg == null)
                    return BaseResponse<PackageModuleResponse>.ErrorResponse("Package not found");
                var mod = await _unitOfWork.Modules.GetByIdAsync(request.ModuleId);
                if (mod == null)
                    return BaseResponse<PackageModuleResponse>.ErrorResponse("Module not found");

                var exists = await _unitOfWork.PackageModules.ExistsAsync(pm =>
                    pm.PackageId == request.PackageId && pm.ModuleId == request.ModuleId && !pm.IsDeleted);
                if (exists)
                    return BaseResponse<PackageModuleResponse>.ErrorResponse("This package-module association already exists");

                var pm = new PackageModule { PackageId = request.PackageId, ModuleId = request.ModuleId };
                var added = await _unitOfWork.PackageModules.AddAsync(pm);
                await _unitOfWork.SaveChangesAsync();
                return BaseResponse<PackageModuleResponse>.SuccessResponse(new PackageModuleResponse
                {
                    Id = added.Id,
                    PackageId = added.PackageId,
                    PackageCode = pkg.Code,
                    ModuleId = added.ModuleId,
                    ModuleCode = mod.Code,
                    CreatedAt = added.CreatedAt
                }, "Package module created");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating package module");
                return BaseResponse<PackageModuleResponse>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<bool>> DeletePackageModuleAsync(Guid id)
        {
            try
            {
                var pm = await _unitOfWork.PackageModules.GetByIdAsync(id);
                if (pm == null)
                    return BaseResponse<bool>.ErrorResponse("PackageModule not found");

                pm.IsDeleted = true;
                pm.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.PackageModules.Update(pm);
                await _unitOfWork.SaveChangesAsync();
                return BaseResponse<bool>.SuccessResponse(true, "Package module deleted");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting package module {Id}", id);
                return BaseResponse<bool>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        private static PackageResponse MapToPackageResponse(Package p) => new()
        {
            Id = p.Id,
            Code = p.Code,
            Name = p.Name,
            CreatedAt = p.CreatedAt
        };

        private static ModuleResponse MapToModuleResponse(Modules m) => new()
        {
            Id = m.Id,
            Code = m.Code,
            Name = m.Name,
            CreatedAt = m.CreatedAt
        };
    }
}
