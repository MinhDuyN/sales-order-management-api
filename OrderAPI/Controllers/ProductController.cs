using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderAPI.DTOs.Category;
using OrderAPI.DTOs.Common;
using OrderAPI.DTOs.Payment;
using OrderAPI.DTOs.Product;
using OrderAPI.Services;
using OrderAPI.Services.Interfaces;

namespace OrderAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productServicecontext)
        {
            _productService = productServicecontext;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllProduct(string? keyword, int? categoryId, string? sortDir, string? sortBy, int page=1, int pageSize=10)
        {
            var products = await _productService.GetAllProduct(keyword, categoryId, sortDir, sortBy, page, pageSize);
            return Ok(ApiResponse<PagedList<ProductResponse>>.Ok("Get products successfully", products));
        }
        [HttpPost]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList();
                return BadRequest(ApiResponse<object>.Fail("Validation failed", errors.ToList()));
            }
            var createProduct = await _productService.CreateProduct(request);
            return Ok(ApiResponse<ProductResponse>.Ok("Create product successfully",createProduct));
                
        }
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList();
                return BadRequest(ApiResponse<object>.Fail("Validation failed", errors.ToList()));
            }
            
            var updateProduct = await _productService.UpdateProduct(id,request);
            return Ok(ApiResponse<ProductResponse>.Ok("Update product successfully", updateProduct));
            
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            await _productService.DeleteProduct(id);
            return NoContent();
        }
    }
}
