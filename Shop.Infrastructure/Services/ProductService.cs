using Microsoft.Extensions.Logging;
using Shop.Core.DTOs;
using Shop.Core.Interfaces.Repositories;
using Shop.Core.Interfaces.Services;
using Shop.Entities;

namespace Shop.Infrastructure.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IUnitOfWork unitOfWork, ILogger<ProductService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<BaseResponse<ProductResponse>> GetByIdAsync(Guid id)
        {
            try
            {
                var product = await _unitOfWork.Products.GetByIdAsync(id);
                if (product == null)
                    return BaseResponse<ProductResponse>.ErrorResponse("Product not found");
                return BaseResponse<ProductResponse>.SuccessResponse(MapToResponse(product));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product {Id}", id);
                return BaseResponse<ProductResponse>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<IEnumerable<ProductResponse>>> GetAllAsync()
        {
            try
            {
                var list = (await _unitOfWork.Products.GetAllAsync()).Select(MapToResponse).ToList();
                return BaseResponse<IEnumerable<ProductResponse>>.SuccessResponse(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting products");
                return BaseResponse<IEnumerable<ProductResponse>>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<ProductResponse>> CreateAsync(CreateProductRequest request)
        {
            try
            {
                if (await _unitOfWork.Products.GetByRegistrationNumberAsync(request.RegistrationNumber) != null)
                    return BaseResponse<ProductResponse>.ErrorResponse("Product with this registration number already exists");

                var product = new Product
                {
                    Name = request.Name,
                    Description = request.Description,
                    RegistrationNumber = request.RegistrationNumber,
                    TaxId = request.TaxId,
                    Industry = request.Industry,
                    Website = request.Website
                };
                var added = await _unitOfWork.Products.AddAsync(product);
                await _unitOfWork.SaveChangesAsync();
                return BaseResponse<ProductResponse>.SuccessResponse(MapToResponse(added), "Product created");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product");
                return BaseResponse<ProductResponse>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<ProductResponse>> UpdateAsync(Guid id, UpdateProductRequest request)
        {
            try
            {
                var product = await _unitOfWork.Products.GetByIdAsync(id);
                if (product == null)
                    return BaseResponse<ProductResponse>.ErrorResponse("Product not found");

                product.Name = request.Name;
                product.Description = request.Description;
                product.TaxId = request.TaxId;
                product.Industry = request.Industry;
                product.Website = request.Website;
                _unitOfWork.Products.Update(product);
                await _unitOfWork.SaveChangesAsync();
                return BaseResponse<ProductResponse>.SuccessResponse(MapToResponse(product), "Product updated");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product {Id}", id);
                return BaseResponse<ProductResponse>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<bool>> DeleteAsync(Guid id)
        {
            try
            {
                var product = await _unitOfWork.Products.GetByIdAsync(id);
                if (product == null)
                    return BaseResponse<bool>.ErrorResponse("Product not found");

                product.IsDeleted = true;
                product.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Products.Update(product);
                await _unitOfWork.SaveChangesAsync();
                return BaseResponse<bool>.SuccessResponse(true, "Product deleted");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product {Id}", id);
                return BaseResponse<bool>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        private static ProductResponse MapToResponse(Product p) => new()
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            RegistrationNumber = p.RegistrationNumber,
            TaxId = p.TaxId,
            Industry = p.Industry,
            Website = p.Website,
            CreatedAt = p.CreatedAt
        };
    }
}
