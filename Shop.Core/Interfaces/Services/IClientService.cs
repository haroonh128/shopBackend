using Shop.Core.DTOs;

namespace Shop.Core.Interfaces.Services
{
    public interface IClientService
    {
        Task<BaseResponse<ClientResponse>> GetByIdAsync(Guid userId, Guid id);
        Task<BaseResponse<IEnumerable<ClientResponse>>> GetAllAsync(Guid userId);
        Task<BaseResponse<IEnumerable<ClientResponse>>> GetByModuleIdAsync(Guid userId, Guid moduleId);
        Task<BaseResponse<ClientResponse>> CreateAsync(Guid userId, ClientRequest request);
        Task<BaseResponse<ClientResponse>> UpdateAsync(Guid userId, Guid id, ClientRequest request);
        Task<BaseResponse<bool>> DeleteAsync(Guid userId, Guid id);
    }
}
