using Shop.Core.DTOs;

namespace Shop.Core.Interfaces.Services
{
    public interface IAddressService
    {
        Task<BaseResponse<AddressResponse>> GetByIdAsync(Guid id);
        Task<BaseResponse<IEnumerable<AddressResponse>>> GetAllAsync();
        Task<BaseResponse<IEnumerable<AddressResponse>>> GetByAddresseeIdAsync(string addresseeId);
        Task<BaseResponse<AddressResponse>> CreateAsync(CreateAddressRequest request);
        Task<BaseResponse<AddressResponse>> UpdateAsync(Guid id, UpdateAddressRequest request);
        Task<BaseResponse<bool>> DeleteAsync(Guid id);
    }
}
