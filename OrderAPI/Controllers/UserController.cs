using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderAPI.DTOs.Common;
using OrderAPI.DTOs.Product;
using OrderAPI.DTOs.User;
using OrderAPI.Entities;
using OrderAPI.Services.Interfaces;
using System.Globalization;
using System.Security.Claims;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace OrderAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles ="Admin")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllUser(string? keyword=null, string? sortDir=null, string? sortBy=null, int page=1, int pageSize=5)
        {
            var users = await _userService.GetAllUser(keyword, sortDir, sortBy, page, pageSize);
            return Ok(ApiResponse<PagedList<UserResponse>>.Ok("Get users successfully", users));
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            
            var users = await _userService.GetUserById(id);
            return Ok(ApiResponse<UserResponse>.Ok("Get users successfully", users));
            
            
        }
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            if(!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList();
                return BadRequest(ApiResponse<object>.Fail("Validation failed", errors.ToList()));
            }
                var users = await _userService.CreateUser(request);
                return CreatedAtAction(nameof(GetUserById), new { users.Id }, ApiResponse<UserResponse>.Ok("Create user successfully", users));

            
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id,[FromBody] UpdateUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList();
                return BadRequest(ApiResponse<object>.Fail("Validation failed", errors.ToList()));
            }
            
            var users = await _userService.UpdateUser(id, request);
                
            return Ok(ApiResponse<UserResponse>.Ok("Update users successfully", users));
            
            
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        { 
            await _userService.DeleteUser(id);
            return NoContent();
        }
    }
}
