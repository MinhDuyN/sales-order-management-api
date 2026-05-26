using OrderAPI.DTOs.Common;
using OrderAPI.DTOs.Product;

namespace OrderAPI.Services.Interfaces
{
    public interface IProductService
    {
        Task<PagedList<ProductResponse>> GetAllProduct(string? keyword, int? categoryId, string? sortDir, string? sortBy, int page, int pageSize);
        Task<ProductResponse> CreateProduct(CreateProductRequest request);
        Task<ProductResponse> UpdateProduct(int id, UpdateProductRequest request);
        Task<bool> DeleteProduct(int id);
    }
}
