using Microsoft.Extensions.Logging;
using Shop.Core.DTOs;
using Shop.Core.Interfaces.Repositories;
using Shop.Core.Interfaces.Services;
using Shop.Entities;

namespace Shop.Infrastructure.Services
{
    public class AddressService : IAddressService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AddressService> _logger;

        public AddressService(IUnitOfWork unitOfWork, ILogger<AddressService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<BaseResponse<AddressResponse>> GetByIdAsync(Guid id)
        {
            try
            {
                var address = await _unitOfWork.Addresses.GetByIdAsync(id);
                if (address == null)
                    return BaseResponse<AddressResponse>.ErrorResponse("Address not found");
                return BaseResponse<AddressResponse>.SuccessResponse(MapToResponse(address));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting address {Id}", id);
                return BaseResponse<AddressResponse>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<IEnumerable<AddressResponse>>> GetAllAsync()
        {
            try
            {
                var list = (await _unitOfWork.Addresses.GetAllAsync()).Select(MapToResponse).ToList();
                return BaseResponse<IEnumerable<AddressResponse>>.SuccessResponse(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting addresses");
                return BaseResponse<IEnumerable<AddressResponse>>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<IEnumerable<AddressResponse>>> GetByAddresseeIdAsync(string addresseeId)
        {
            try
            {
                var list = (await _unitOfWork.Addresses.GetByAddresseeIdAsync(addresseeId)).Select(MapToResponse).ToList();
                return BaseResponse<IEnumerable<AddressResponse>>.SuccessResponse(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting addresses for addressee {AddresseeId}", addresseeId);
                return BaseResponse<IEnumerable<AddressResponse>>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<AddressResponse>> CreateAsync(CreateAddressRequest request)
        {
            try
            {
                var address = new Address
                {
                    Name = request.Name,
                    AddressLine1 = request.AddressLine1,
                    AddressLine2 = request.AddressLine2,
                    AddresseeId = request.AddresseeId,
                    City = request.City,
                    Country = request.Country,
                    Zipcode = request.Zipcode,
                    Phone = request.Phone
                };
                var added = await _unitOfWork.Addresses.AddAsync(address);
                await _unitOfWork.SaveChangesAsync();
                return BaseResponse<AddressResponse>.SuccessResponse(MapToResponse(added), "Address created");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating address");
                return BaseResponse<AddressResponse>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<AddressResponse>> UpdateAsync(Guid id, UpdateAddressRequest request)
        {
            try
            {
                var address = await _unitOfWork.Addresses.GetByIdAsync(id);
                if (address == null)
                    return BaseResponse<AddressResponse>.ErrorResponse("Address not found");

                address.Name = request.Name;
                address.AddressLine1 = request.AddressLine1;
                address.AddressLine2 = request.AddressLine2;
                address.City = request.City;
                address.Country = request.Country;
                address.Zipcode = request.Zipcode;
                address.Phone = request.Phone;
                _unitOfWork.Addresses.Update(address);
                await _unitOfWork.SaveChangesAsync();
                return BaseResponse<AddressResponse>.SuccessResponse(MapToResponse(address), "Address updated");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating address {Id}", id);
                return BaseResponse<AddressResponse>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<bool>> DeleteAsync(Guid id)
        {
            try
            {
                var address = await _unitOfWork.Addresses.GetByIdAsync(id);
                if (address == null)
                    return BaseResponse<bool>.ErrorResponse("Address not found");

                address.IsDeleted = true;
                address.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Addresses.Update(address);
                await _unitOfWork.SaveChangesAsync();
                return BaseResponse<bool>.SuccessResponse(true, "Address deleted");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting address {Id}", id);
                return BaseResponse<bool>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        private static AddressResponse MapToResponse(Address a) => new()
        {
            Id = a.Id,
            Name = a.Name,
            AddressLine1 = a.AddressLine1,
            AddressLine2 = a.AddressLine2,
            AddresseeId = a.AddresseeId,
            City = a.City,
            Country = a.Country,
            Zipcode = a.Zipcode,
            Phone = a.Phone,
            CreatedAt = a.CreatedAt
        };
    }
}
