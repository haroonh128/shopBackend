using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.Core.DTOs;
using Shop.Core.Interfaces.Services;

namespace ShopBackend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PackageController : ControllerBase
    {
        private readonly IPackageService _packageService;

        public PackageController(IPackageService packageService)
        {
            _packageService = packageService;
        }

        // ----- Packages -----
        [HttpGet("packages")]
        [ProducesResponseType(typeof(BaseResponse<IEnumerable<PackageResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllPackages()
        {
            var result = await _packageService.GetAllPackagesAsync();
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("packages/{id:guid}")]
        [ProducesResponseType(typeof(BaseResponse<PackageResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPackageById(Guid id)
        {
            var result = await _packageService.GetPackageByIdAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpPost("packages")]
        [ProducesResponseType(typeof(BaseResponse<PackageResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreatePackage([FromBody] CreatePackageRequest request)
        {
            var result = await _packageService.CreatePackageAsync(request);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpPut("packages/{id:guid}")]
        [ProducesResponseType(typeof(BaseResponse<PackageResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdatePackage(Guid id, [FromBody] UpdatePackageRequest request)
        {
            var result = await _packageService.UpdatePackageAsync(id, request);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpDelete("packages/{id:guid}")]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletePackage(Guid id)
        {
            var result = await _packageService.DeletePackageAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        // ----- Modules -----
        [HttpGet("modules")]
        [ProducesResponseType(typeof(BaseResponse<IEnumerable<ModuleResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllModules()
        {
            var result = await _packageService.GetAllModulesAsync();
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("modules/{id:guid}")]
        [ProducesResponseType(typeof(BaseResponse<ModuleResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetModuleById(Guid id)
        {
            var result = await _packageService.GetModuleByIdAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpPost("modules")]
        [ProducesResponseType(typeof(BaseResponse<ModuleResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateModule([FromBody] CreateModuleRequest request)
        {
            var result = await _packageService.CreateModuleAsync(request);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpPut("modules/{id:guid}")]
        [ProducesResponseType(typeof(BaseResponse<ModuleResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateModule(Guid id, [FromBody] UpdateModuleRequest request)
        {
            var result = await _packageService.UpdateModuleAsync(id, request);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpDelete("modules/{id:guid}")]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteModule(Guid id)
        {
            var result = await _packageService.DeleteModuleAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        // ----- PackageModules -----
        [HttpGet("packages/{packageId:guid}/modules")]
        [ProducesResponseType(typeof(BaseResponse<IEnumerable<PackageModuleResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPackageModules(Guid packageId)
        {
            var result = await _packageService.GetPackageModulesByPackageIdAsync(packageId);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("package-modules/{id:guid}")]
        [ProducesResponseType(typeof(BaseResponse<PackageModuleResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPackageModuleById(Guid id)
        {
            var result = await _packageService.GetPackageModuleByIdAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpPost("package-modules")]
        [ProducesResponseType(typeof(BaseResponse<PackageModuleResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreatePackageModule([FromBody] CreatePackageModuleRequest request)
        {
            var result = await _packageService.CreatePackageModuleAsync(request);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("package-modules/{id:guid}")]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletePackageModule(Guid id)
        {
            var result = await _packageService.DeletePackageModuleAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }
    }
}
