using Microsoft.EntityFrameworkCore;
using OrderAPI.Data;
using OrderAPI.DTOs.Report;
using OrderAPI.Exceptions;
using OrderAPI.Services.Interfaces;

namespace OrderAPI.Services
{
    public class ReportService : IReportService
    {
        private readonly AppDbContext _context;
        public ReportService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<RevenueResponse> RevenueReport(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
            {
                throw new BadRequestException($"StartDate cannot be greater than EndDate");
            }
            var query = _context.Orders
                .Where(q => q.CreatedDate >= startDate && q.CreatedDate <= endDate && q.Status == "Shipped")
                .AsQueryable();

            var totalRevenue = await query.SumAsync(x => x.TotalAmount);
            var orderCount = await query.CountAsync();

            return new RevenueResponse
            {
                TotalRevenue = totalRevenue,
                StartDate = startDate,
                EndDate = endDate,
                OrderCount = orderCount
            };
        }

        public async Task<List<DailyRevenueResponse>> DailyRevenueReport(DateTime startDate, DateTime endDate)
        {
            var query = _context.Orders.AsQueryable();
            if (startDate > endDate)
            {
                throw new BadRequestException($"StartDate cannot be greater than EndDate");
            }
            query = query.Where(query => query.CreatedDate >= startDate && query.CreatedDate <= endDate && query.Status == "Shipped");
            var groupby = query.GroupBy(x => x.CreatedDate.Date);
            var data = await groupby.Select(g => new
            {
                Revenue = g.Sum(x => x.TotalAmount),
                CreatedDate = DateOnly.FromDateTime(g.Key.Date)
            }).OrderByDescending(x => x.CreatedDate)
                .ToListAsync();
            return data.Select(x => new DailyRevenueResponse
            {
                Revenue = x.Revenue,
                CreatedDate = x.CreatedDate,
            }).ToList();
        }

        public async Task<List<MonthRevenueResponse>> MonthRevenueReport(DateTime startDate, DateTime endDate)
        {
            var query = _context.Orders.AsQueryable();
            if (startDate > endDate)
            {
                throw new BadRequestException($"StartDate cannot be greater than EndDate");
            }
            query = query.Where(query => query.CreatedDate >= startDate && query.CreatedDate <= endDate && query.Status == "Shipped");
            var groupby = query.GroupBy(x => new { x.CreatedDate.Month, x.CreatedDate.Year });
            var data = await groupby.Select(g => new
            {
                Revenue = g.Sum(x => x.TotalAmount),
                Month = g.Key.Month,
                Year = g.Key.Year,
            }).OrderByDescending(x => x.Year)
              .ThenByDescending(x => x.Month)
              .ToListAsync();
            return data.Select(x => new MonthRevenueResponse
            {
                Revenue = x.Revenue,
                Month = x.Month,
                Year = x.Year
            }).ToList();
        }

        public async Task<List<TopProductResponse>> TopProductReport(DateTime startDate, DateTime endDate, int top)
        {
            var query = _context.OrderItems
                .Include(x => x.Order)
                .Include(x => x.Product)
                .AsQueryable();
            if (startDate > endDate)
            {
                throw new BadRequestException($"StartDate cannot be greater than EndDate");
            }
            query = query.Where(query => query.Order.CreatedDate >= startDate && query.Order.CreatedDate <= endDate && query.Order.Status == "Shipped");
            var groupby = query.GroupBy(x => new { x.Product.Id, x.Product.ProductName, x.Order.CreatedDate.Date });
            var data = await groupby.Select(g => new
            {
                ProductId = g.Key.Id,
                ProductName = g.Key.ProductName,
                Revenue = g.Sum(x => x.LineTotal),
                CreatedDate = DateOnly.FromDateTime(g.Key.Date)
            }).OrderByDescending(x => x.Revenue)
                .Take(top)
                .ToListAsync();
            return data.Select(x => new TopProductResponse
            {
                ProductId = x.ProductId,
                ProductName = x.ProductName,
                Revenue = x.Revenue,
                CreatedDate = x.CreatedDate
            }).ToList();
        }

        public async Task<List<TopUserResponse>> TopUserReport(DateTime startDate, DateTime endDate, int top)
        {
            var query = _context.Orders
                .Include(x => x.User)
                .AsQueryable();
            if (startDate > endDate)
            {
                throw new BadRequestException($"StartDate cannot be greater than EndDate");
            }
            query = query.Where(q => q.CreatedDate >= startDate && q.CreatedDate <= endDate && q.Status == "Shipped");
            var groupby = query.GroupBy(x => new { x.User.Id, x.User.Name, x.User.Email });
            var data = await groupby.Select(g => new
            {
                UserId = g.Key.Id,
                UserName = g.Key.Name,
                UserEmail = g.Key.Email,
                TotalSpent = g.Sum(x => x.TotalAmount),
                OrderCount = g.Count()
            }).OrderByDescending(x => x.TotalSpent)
                .Take(top)
                .ToListAsync();
            return data.Select(x => new TopUserResponse
            {
                UserId = x.UserId,
                UserName = x.UserName,
                UserEmail = x.UserEmail,
                TotalSpent = x.TotalSpent,
                OrderCount = x.OrderCount
            }).ToList();
        }

        public async Task<List<CategoryRevenueResponse>> CategoryRevenueReport(DateTime startDate, DateTime endDate)
        {
            var query = _context.OrderItems
                .Include(x => x.Order)
                .Include(x => x.Product)
                .ThenInclude(x => x.Category)
                .AsQueryable();
            if (startDate > endDate)
            {
                throw new BadRequestException($"StartDate cannot be greater than EndDate");
            }
            query = query.Where(q => q.Order.CreatedDate >= startDate && q.Order.CreatedDate <= endDate && q.Order.Status == "Shipped");
            var groupby = query.GroupBy(x => new { x.Product.Category.Id, x.Product.Category.CategoryName });
            var data = await groupby.Select(g => new
            {
                CategoryId = g.Key.Id,
                CategoryName = g.Key.CategoryName,
                Revenue = g.Sum(x => x.LineTotal),
                ProductCount = g.Select(x => x.Product.Id).Distinct().Count()
            }).OrderByDescending(x => x.Revenue)
                .ToListAsync();
            return data.Select(x => new CategoryRevenueResponse
            {
                CategoryId = x.CategoryId,
                CategoryName = x.CategoryName,
                Revenue = x.Revenue,
                ProductCount = x.ProductCount
            }).ToList();
        }

        public async Task<List<ActiveUserResponse>> ActiveUserReport(DateTime startDate, DateTime endDate)
        {
            var query = _context.Orders
                .Include(x => x.User)
                .AsQueryable();
            if (startDate > endDate)
            {
                throw new BadRequestException($"StartDate cannot be greater than EndDate");
            }
            query = query.Where(q => q.CreatedDate >= startDate && q.CreatedDate <= endDate);
            var groupby = query.GroupBy(x => new { x.User.Id, x.User.Name, x.User.Email });
            var data = await groupby.Select(g => new
            {
                UserId = g.Key.Id,
                UserName = g.Key.Name,
                UserEmail = g.Key.Email,
                OrderCount = g.Count(),
                LastOrderDate = g.Max(x => x.CreatedDate)
            }).OrderByDescending(x => x.LastOrderDate)
                .ToListAsync();
            return data.Select(x => new ActiveUserResponse
            {
                UserId = x.UserId,
                UserName = x.UserName,
                UserEmail = x.UserEmail,
                OrderCount = x.OrderCount,
                LastOrderDate = x.LastOrderDate
            }).ToList();
        }
    }
}
