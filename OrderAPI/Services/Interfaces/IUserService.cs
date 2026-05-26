using OrderAPI.DTOs.Common;
using OrderAPI.DTOs.User;

namespace OrderAPI.Services.Interfaces
{
    public interface IUserService
    {
        Task<PagedList<UserResponse>> GetAllUser(string?keyword,string?sortDir,string?sortBy, int page, int pageSize);
        Task<UserResponse> GetUserById (int id);
        Task<UserResponse> CreateUser(CreateUserRequest request);
        Task<UserResponse> UpdateUser(int id,UpdateUserRequest request);
        Task<bool> DeleteUser(int id);
    }
}
