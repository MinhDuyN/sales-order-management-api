using Microsoft.EntityFrameworkCore;
using OrderAPI.Data;
using OrderAPI.DTOs.Common;
using OrderAPI.DTOs.Product;
using OrderAPI.Entities;
using OrderAPI.Exceptions;
using OrderAPI.Services.Interfaces;

namespace OrderAPI.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;
        public ProductService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<PagedList<ProductResponse>> GetAllProduct(string? keyword, int? categoryId, string? sortDir, string? sortBy, int page, int pageSize)
        {
            var query = _context.Products
                .Include(x=>x.Category)
                .AsQueryable();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(query=>query.ProductName.Contains(keyword));
            }
            if (categoryId.HasValue)
            {
                query = query.Where(query => query.CategoryId == categoryId);
            }
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                bool isDesc = string.Equals(sortDir,"desc",StringComparison.OrdinalIgnoreCase);
                if (string.Equals(sortBy, "name", StringComparison.OrdinalIgnoreCase))
                {
                    query = isDesc ? query.OrderByDescending(query => query.ProductName) : query.OrderBy(query => query.ProductName);
                }
                else
                {
                    query = query.OrderByDescending(query => query.CreatedDate);
                }
            }
            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            int skip = (page - 1) * pageSize;
            var data = await query.Skip(skip)
                .Take(pageSize)
                .Select(query => new ProductResponse
                {
                    Id = query.Id,
                    ProductName = query.ProductName,
                    Price = query.Price,
                    StockQuantity = query.StockQuantity,
                    CategoryName = query.Category.CategoryName,
                    IsActive = query.IsActive,
                    CreatedDate = query.CreatedDate
                }).ToListAsync();
            return new PagedList<ProductResponse>
            {
                Items = data,
                TotalItems = totalItems,
                TotalPages = totalPages,
                Page = page,
                PageSize = pageSize,
            };
        }
        public async Task<ProductResponse> CreateProduct(CreateProductRequest request)
        {
            bool checkCategory = await _context.Categories.AnyAsync(c=>c.Id == request.CategoryId);
            if (checkCategory)
            {
                var product = new Product
                {
                    ProductName = request.ProductName,
                    Price = request.Price,
                    StockQuantity = request.StockQuantity,
                    CategoryId = request.CategoryId,
                    IsActive = request.IsActive,
                    CreatedDate = DateTime.UtcNow
                };
                await _context.Products.AddAsync(product);
                await _context.SaveChangesAsync();
                var productData = await _context.Products
                    .Include(x=>x.Category)
                    .FirstOrDefaultAsync(p=>p.Id == product.Id);
                return new ProductResponse
                {
                    Id = productData.Id,
                    ProductName = productData.ProductName,
                    Price = productData.Price,
                    CategoryName = productData.Category.CategoryName.ToString(),
                    StockQuantity = productData.StockQuantity,
                    IsActive = productData.IsActive,
                    CreatedDate = productData.CreatedDate
                };
            }
            else
            {
                throw new NotFoundException($"CategoryId {request.CategoryId} is not exists");
            }
        }
        public async Task<ProductResponse> UpdateProduct(int id, UpdateProductRequest request)
        {
            bool checkCategory = await _context.Categories.AnyAsync(c => c.Id == request.CategoryId);
            if (checkCategory)
            {
                var checkProduct = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
                if ((checkProduct == null))
                {
                    throw new NotFoundException("Product is not found");
                }
                else if (checkProduct.ProductName == request.ProductName)
                {
                    throw new NotFoundException($"ProductName {request.ProductName} is exists");
                }
                checkProduct.ProductName = request.ProductName;
                checkProduct.Price = request.Price;
                checkProduct.StockQuantity = request.StockQuantity;
                checkProduct.CategoryId = request.CategoryId;
                checkProduct.IsActive = request.IsActive;
                _context.Products.Update(checkProduct);
                await _context.SaveChangesAsync();
                var productData = await _context.Products
                    .Include(x => x.Category)
                    .FirstOrDefaultAsync(p => p.Id == checkProduct.Id);
                return new ProductResponse
                {
                    Id = productData.Id,
                    ProductName = productData.ProductName,
                    Price = productData.Price,
                    CategoryName = productData.Category.CategoryName.ToString(),
                    StockQuantity = productData.StockQuantity,
                    IsActive = productData.IsActive,
                    CreatedDate = productData.CreatedDate
                };
            }
            else
            {
                throw new NotFoundException($"CategoryId {request.CategoryId} is not exists");
            }
        }
        public async Task<bool> DeleteProduct(int id)
        {
            var checkProduct = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (checkProduct == null)
            {
                throw new NotFoundException("ProductId is not found");
            }
            _context.Products.Remove(checkProduct);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
