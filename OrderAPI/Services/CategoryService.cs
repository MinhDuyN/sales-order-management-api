using Microsoft.EntityFrameworkCore;
using OrderAPI.Data;
using OrderAPI.DTOs.Category;
using OrderAPI.DTOs.Common;
using OrderAPI.Entities;
using OrderAPI.Exceptions;
using OrderAPI.Services.Interfaces;

namespace OrderAPI.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CategoryService> _logger;
        public CategoryService(AppDbContext context, ILogger<CategoryService> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<PagedList<CategoryResponse>> GetAllCategory(string? keyword, int page, int pageSize)
        {
            var query = _context.Categories.AsQueryable();
            if(!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(query=>query.CategoryName.Equals(keyword));
            }
            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalItems/(double)pageSize);
            int skip = (page - 1)*pageSize;
            var data = await query
                .Skip(skip)
                .Take(pageSize)
                .Select(query => new CategoryResponse
            {
                Id = query.Id,
                CategoryName = query.CategoryName,
                CreatedDate = query.CreatedDate,
            }).ToListAsync();
            return new PagedList<CategoryResponse>
            {
                Items = data,
                TotalItems = totalItems,
                TotalPages = totalPages,
                Page = page,
                PageSize = pageSize
            };
        }
        public async Task<CategoryResponse> GetCategoryById(int id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (category == null)
            {
                throw new NotFoundException("Category not found");
            }
            return new CategoryResponse
            {
                Id = id,
                CategoryName = category.CategoryName,
                CreatedDate = category.CreatedDate,
            };
         }
        public async Task<CategoryResponse> CreateCategory(CreateCategoryRequest request)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c=>c.CategoryName == request.CategoryName);
            if (category == null)
            {
                var insertCategory = new Category
                {
                    CategoryName = request.CategoryName,
                    CreatedDate = DateTime.UtcNow,
                };
                await _context.Categories.AddAsync(insertCategory);
                await _context.SaveChangesAsync();
                return new CategoryResponse
                {
                    Id = insertCategory.Id,
                    CategoryName = insertCategory.CategoryName,
                    CreatedDate = insertCategory.CreatedDate,
                };
            }
            else
            {
                throw new BadRequestException("Category is exists");
            }
        }

        public async Task<CategoryResponse> UpdateCategory(int id, UpdateCategoryRequest request)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c=>c.Id == id);
            if(category== null)
            {
                throw new NotFoundException("Category not found");
            }
            var checkNameCategory = await _context.Categories.AnyAsync(c=>c.CategoryName==request.CategoryName && c.Id != id);
            if (!checkNameCategory)
            {
                category.CategoryName = request.CategoryName;
                _context.Categories.Update(category);
                await _context.SaveChangesAsync();
                return new CategoryResponse
                {
                    Id = id,
                    CategoryName = category.CategoryName,
                    CreatedDate = category.CreatedDate
                };
            }
            else
            {
                throw new BadRequestException("This category name is exists");
            }
            
        }

        public async Task<bool> DeleteCategory(int id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (category == null)
            {
                throw new NotFoundException("Category not found");
            }
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
