using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using OrderAPI.Data;
using OrderAPI.DTOs.Common;
using OrderAPI.DTOs.User;
using OrderAPI.Entities;
using OrderAPI.Exceptions;
using OrderAPI.Services.Interfaces;

namespace OrderAPI.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        public UserService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<PagedList<UserResponse>> GetAllUser(string? keyword, string? sortDir, string? sortBy, int page, int pageSize)
        {
            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(query=>query.Name.Contains(keyword));
            }
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                bool isDesc = string.Equals(sortDir,"desc",StringComparison.OrdinalIgnoreCase);
                if(string.Equals(sortBy,"name",StringComparison.OrdinalIgnoreCase))
                {
                    query = isDesc?query.OrderByDescending(query=>query.Name):query.OrderBy(query=>query.Name);
                }
                else
                {
                    query = query.OrderByDescending(query => query.CreatedDate);
                }
            }
            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalItems/(double)pageSize);
            int skip = (page-1) * pageSize;
            var data = await query.Skip(skip).Take(pageSize).Select(u => new UserResponse
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                RoleName = u.Role.RoleName,
                IsActive = u.IsActive,
                CreatedDate = u.CreatedDate
            }).ToListAsync();
            return new PagedList<UserResponse>
            {
                Items = data,
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
            };
        }
        public async Task<UserResponse> GetUserById(int id)
        {
            var users = await _context.Users
                .Include(u=>u.Role)
                .FirstOrDefaultAsync(u=>u.Id== id);
            if (users==null)
            {
                throw new NotFoundException("UserId not found");
            }
            return new UserResponse
            {
                Id = id,
                Name = users.Name,
                Email = users.Email,
                RoleName = users.Role.RoleName,
                IsActive = users.IsActive,
                CreatedDate = users.CreatedDate,
            };
        }

        public async Task<UserResponse> CreateUser(CreateUserRequest request)
        {
            var checkEmail = await _context.Users
                .FirstOrDefaultAsync(x=>x.Email == request.Email);
            var checkRole = await _context.Roles.AnyAsync(r=>r.Id == request.RoleId);
            if (checkEmail != null)
            {
                throw new ConflictDataException("Email is exists");
            }
            else
            {
                if (checkRole)
                {
                    var createUser = new User
                    {
                        Name = request.Name,
                        Email = request.Email,
                        RoleId = request.RoleId,
                        IsActive = request.IsActive,
                        CreatedDate = request.CreatedDate,
                    };
                    await _context.Users.AddAsync(createUser);
                    await _context.SaveChangesAsync();
                    var userWithRole = await _context.Users
                        .Include(u => u.Role)
                        .FirstOrDefaultAsync(u => u.Id == createUser.Id);
                    return new UserResponse
                    {
                        Id = userWithRole.Id,
                        Name = userWithRole.Name,
                        Email = userWithRole.Email,
                        RoleName = userWithRole.Role.RoleName,
                        IsActive = userWithRole.IsActive,
                        CreatedDate = userWithRole.CreatedDate,
                    };
                }
                else
                {
                    throw new NotFoundException("Role is not exists");
                }
            }
        }

        public async Task<UserResponse> UpdateUser(int id, UpdateUserRequest request)
        {
            var checkUser = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == id);
            if (checkUser == null)
            {
                throw new NotFoundException("UserId not found");
            }
            else
            {
                var checkRole = await _context.Roles.AnyAsync(r => r.Id == request.RoleId);
                if (checkRole)
                {
                    checkUser.Name = request.Name;
                    checkUser.RoleId = request.RoleId;
                    checkUser.IsActive = request.IsActive;
                    _context.Users.Update(checkUser);
                    await _context.SaveChangesAsync();
                    var userWithRole = await _context.Users
                        .Include(u => u.Role)
                        .FirstOrDefaultAsync(u => u.Id == id);
                    return new UserResponse
                    {
                        Id = userWithRole.Id,
                        Name = userWithRole.Name,
                        Email = userWithRole.Email,
                        RoleName = userWithRole.Role.RoleName,
                        IsActive = userWithRole.IsActive,
                        CreatedDate = userWithRole.CreatedDate,
                    };

                }
                else
                {
                    throw new NotFoundException("Role is not exists");
                }
            }
        }
        public async Task<bool> DeleteUser(int id)
        {
            var checkUser = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == id);
            if (checkUser == null)
            {
                throw new NotFoundException("UserId not found");
            }
            else
            {
                 _context.Users.Remove(checkUser);
                await _context.SaveChangesAsync();
                return true;
            }
        }
    }
}
