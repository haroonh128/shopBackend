using Microsoft.Extensions.Logging;
using Shop.Core.DTOs;
using Shop.Core.Interfaces.Repositories;
using Shop.Core.Interfaces.Services;
using Shop.Entities;

namespace Shop.Infrastructure.Services
{
    public class ClientService : IClientService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ClientService> _logger;

        public ClientService(IUnitOfWork unitOfWork, ILogger<ClientService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<BaseResponse<ClientResponse>> GetByIdAsync(Guid id)
        {
            try
            {
                var client = await _unitOfWork.Clients.GetByIdAsync(id);
                if (client == null || client.IsDeleted)
                    return BaseResponse<ClientResponse>.ErrorResponse("Client not found");
                return BaseResponse<ClientResponse>.SuccessResponse(MapToResponse(client));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting client {Id}", id);
                return BaseResponse<ClientResponse>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<IEnumerable<ClientResponse>>> GetAllAsync()
        {
            try
            {
                var list = (await _unitOfWork.Clients.GetAllAsync())
                    .Where(c => !c.IsDeleted)
                    .Select(MapToResponse)
                    .ToList();
                return BaseResponse<IEnumerable<ClientResponse>>.SuccessResponse(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting clients");
                return BaseResponse<IEnumerable<ClientResponse>>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<IEnumerable<ClientResponse>>> GetByModuleIdAsync(Guid moduleId)
        {
            try
            {
                var list = (await _unitOfWork.Clients.GetByModuleIdAsync(moduleId))
                    .Select(MapToResponse)
                    .ToList();
                return BaseResponse<IEnumerable<ClientResponse>>.SuccessResponse(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting clients for module {ModuleId}", moduleId);
                return BaseResponse<IEnumerable<ClientResponse>>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<ClientResponse>> CreateAsync(ClientRequest request)
        {
            try
            {
                if (request.ModuleId == Guid.Empty)
                    return BaseResponse<ClientResponse>.ErrorResponse("ModuleId is required");

                //var module = await _unitOfWork.Modules.GetByIdAsync(request.ModuleId);
                //if (module == null || module.IsDeleted)
                //    return BaseResponse<ClientResponse>.ErrorResponse("Module not found");

                var client = new Client
                {
                    Name = request.Name,
                    Description = request.Description,
                    Phone = request.Phone,
                    Address = request.Address,
                    Gender = request.Gender,
                    ModuleId = request.ModuleId,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false,
                    Active = true
                };
                var added = await _unitOfWork.Clients.AddAsync(client);
                await _unitOfWork.SaveChangesAsync();
                return BaseResponse<ClientResponse>.SuccessResponse(MapToResponse(added), "Client created");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating client");
                return BaseResponse<ClientResponse>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<ClientResponse>> UpdateAsync(Guid id, ClientRequest request)
        {
            try
            {
                var client = await _unitOfWork.Clients.GetByIdAsync(id);
                if (client == null || client.IsDeleted)
                    return BaseResponse<ClientResponse>.ErrorResponse("Client not found");

                if (request.ModuleId == Guid.Empty)
                    return BaseResponse<ClientResponse>.ErrorResponse("ModuleId is required");

                //var module = await _unitOfWork.Modules.GetByIdAsync(request.ModuleId);
                //if (module == null || module.IsDeleted)
                //    return BaseResponse<ClientResponse>.ErrorResponse($"Module not found: {request.ModuleId}");

                client.Name = request.Name;
                client.Description = request.Description;
                client.Phone = request.Phone;
                client.Address = request.Address;
                client.Gender = request.Gender;
                client.ModuleId = request.ModuleId;
                client.UpdatedAt = DateTime.UtcNow;
                client.Active = request.Active;
                client.IsDeleted = request.IsDeleted;
                _unitOfWork.Clients.Update(client);
                await _unitOfWork.SaveChangesAsync();
                return BaseResponse<ClientResponse>.SuccessResponse(MapToResponse(client), "Client updated");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating client {Id}", id);
                return BaseResponse<ClientResponse>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<bool>> DeleteAsync(Guid id)
        {
            try
            {
                var client = await _unitOfWork.Clients.GetByIdAsync(id);
                if (client == null || client.IsDeleted)
                    return BaseResponse<bool>.ErrorResponse("Client not found");

                client.IsDeleted = true;
                client.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Clients.Update(client);
                await _unitOfWork.SaveChangesAsync();
                return BaseResponse<bool>.SuccessResponse(true, "Client deleted");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting client {Id}", id);
                return BaseResponse<bool>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        private static ClientResponse MapToResponse(Client c) => new()
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description,
            Phone = c.Phone,
            Address = c.Address,
            Gender = c.Gender,
            ModuleId = c.ModuleId,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt
        };
    }
}
