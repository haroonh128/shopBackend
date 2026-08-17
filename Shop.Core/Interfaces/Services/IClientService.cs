using Shop.Core.DTOs;

namespace Shop.Core.Interfaces.Services
{
    public interface IClientService
    {
        Task<BaseResponse<ClientResponse>> GetByIdAsync(Guid id);
        Task<BaseResponse<IEnumerable<ClientResponse>>> GetAllAsync();
        Task<BaseResponse<IEnumerable<ClientResponse>>> GetByModuleIdAsync(Guid moduleId);
        Task<BaseResponse<ClientResponse>> CreateAsync(ClientRequest request);
        Task<BaseResponse<ClientResponse>> UpdateAsync(Guid id, ClientRequest request);
        Task<BaseResponse<bool>> DeleteAsync(Guid id);
    }
}
