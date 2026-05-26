using OrderAPI.DTOs.Category;
using OrderAPI.DTOs.Common;

namespace OrderAPI.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<PagedList<CategoryResponse>> GetAllCategory(string?keyword,int page, int pageSize);
        Task<CategoryResponse> GetCategoryById(int id); 
        Task<CategoryResponse> CreateCategory(CreateCategoryRequest request);
        Task<CategoryResponse> UpdateCategory(int id, UpdateCategoryRequest request);
        Task<bool> DeleteCategory(int id);
    }
}
