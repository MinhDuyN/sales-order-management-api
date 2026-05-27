using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using OrderAPI.Data;
using OrderAPI.DTOs.Common;
using OrderAPI.DTOs.Order;
using OrderAPI.Entities;
using OrderAPI.Exceptions;
using OrderAPI.Services.Interfaces;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace OrderAPI.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;
        public OrderService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<PagedList<OrderResponse>> GetAllOrder(string? keyword, int? userId, DateTime? startDate, DateTime? endDate, int page, int pageSize)
        {
            var query = _context.Orders
                //.Include(x=>x.User)
                //.Include(x=>x.OrderItems)
                //.ThenInclude(x=>x.Product)
                .AsQueryable();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(query => query.Status.Contains(keyword));
            }
            if (userId.HasValue)
            {
                query = query.Where(query => query.UserId == userId);
            }
            if(startDate.HasValue)
            {
                query = query.Where(query=>query.CreatedDate>= startDate);
            }
            if (endDate.HasValue)
            {
                query = query.Where(query => query.CreatedDate <= endDate);
            }
            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            int skip = (page - 1) * pageSize;
            var data = await query.Skip(skip)
                .Take(pageSize)
                .Select(x=>new OrderResponse { 
                    Id = x.Id,
                    UserId = x.UserId,
                    UserName = x.User!.Name,
                    Status = x.Status,
                    TotalAmount = x.TotalAmount,
                    CreatedDate = x.CreatedDate,
                    OrderItems = x.OrderItems.Select(x1=>new OrderItemResponse
                    {
                        Id = x1.Id,
                        OrderId = x1.OrderId,
                        ProductId = x1.ProductId,
                        ProductName = x1.Product.ProductName,
                        Quantity = x1.Quantity,
                        UnitPrice = x1.UnitPrice,
                        LineTotal = x1.LineTotal,
                    }).ToList()
                }).ToListAsync();
            return new PagedList<OrderResponse>
            {
                Items = data,
                TotalItems = totalItems,
                TotalPages = totalPages,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<OrderResponse> GetOrderById(int id)
        {
            //var order = await _context.Orders
            //    .Include(x=>x.User)
            //    .Include(x=>x.OrderItems)
            //    .ThenInclude(oi=>oi.Product)
            //    .FirstOrDefaultAsync(x => x.Id == id);
            //if (order == null)
            //{
            //    throw new NotFoundException("Order not found");
            //}
            //return new OrderResponse()
            //{
            //    Id = id,
            //    UserId = order.UserId,
            //    UserName = order.User!.Name,
            //    Status = order.Status,
            //    TotalAmount = order.TotalAmount,
            //    CreatedDate = order.CreatedDate,
            //    OrderItems = order.OrderItems.Select(oi => new OrderItemResponse
            //    {
            //        Id = oi.Id,
            //        OrderId = oi.OrderId,
            //        ProductId = oi.ProductId,
            //        ProductName = oi.Product.ProductName,
            //        Quantity = oi.Quantity,
            //        UnitPrice = oi.UnitPrice,
            //        LineTotal = oi.LineTotal,
            //    }).ToList()
            //};
            var data = await _context.Orders.Select(x => new OrderResponse
            {
                Id = x.Id,
                UserId = x.UserId,
                UserName = x.User!.Name,
                Status = x.Status,
                TotalAmount = x.TotalAmount,
                CreatedDate = x.CreatedDate,
                OrderItems = x.OrderItems.Select(x1 => new OrderItemResponse
                {
                    Id = x1.Id,
                    OrderId = x1.OrderId,
                    ProductId = x1.ProductId,
                    ProductName = x1.Product.ProductName,
                    Quantity = x1.Quantity,
                    UnitPrice = x1.UnitPrice,
                    LineTotal = x1.LineTotal,
                }).ToList()
            }).FirstOrDefaultAsync(x => x.Id == id);
            if (data == null)
            {
                throw new NotFoundException("Order not found");
            }
            return data;
        }

        public async Task<OrderResponse> CreateOrder(int userId, CreateOrderRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var checkUserId = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
                if (checkUserId==null)
                {
                    throw new NotFoundException($"UserId {userId} is not found");
                }
                var createOrder = new Order
                {
                    UserId = userId,
                    Status = "Pending",
                    TotalAmount = 0,
                    CreatedDate = DateTime.UtcNow,
                    OrderItems = new List<OrderItem>()
                };
                foreach (var item in request.createOrderItemRequests)
                {
                    var checkProductId = await _context.Products.FirstOrDefaultAsync(x => x.Id == item.ProductId);
                    if (checkProductId == null)
                    {
                        throw new NotFoundException($"ProductId {item.ProductId} is not found");
                    }
                    if (checkProductId.StockQuantity < item.Quantity)
                    {
                        throw new NotFoundException($"Quantity want to buy: {item.Quantity} is greater than StockQuantity: {checkProductId.StockQuantity} ");
                    }
                    var createOrderItem = new OrderItem
                    {
                        OrderId = createOrder.Id,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = checkProductId.Price,
                        LineTotal = checkProductId.Price * item.Quantity,
                    };
                    createOrder.OrderItems.Add(createOrderItem);
                    createOrder.TotalAmount += createOrderItem.LineTotal;
                    checkProductId.StockQuantity -= item.Quantity;
                }
                await _context.Orders.AddAsync(createOrder);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return new OrderResponse
                {
                    Id = createOrder.Id,
                    UserId = createOrder.UserId,
                    UserName = checkUserId.Name,
                    Status = createOrder.Status,
                    TotalAmount = createOrder.TotalAmount,
                    CreatedDate = createOrder.CreatedDate,
                    OrderItems = createOrder.OrderItems.Select(oi => new OrderItemResponse
                    {
                        Id = oi.Id,
                        OrderId = oi.OrderId,
                        ProductId = oi.ProductId,
                        ProductName = oi.Product.ProductName,
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice,
                        LineTotal = oi.LineTotal,
                    }).ToList()
                };
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<OrderResponse> UpdateOrder(int id, UpdateOrderRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var order = await _context.Orders
                    .Include(o=>o.OrderItems)
                    .ThenInclude(oi=>oi.Product)
                    .Include(o=>o.User)
                    .FirstOrDefaultAsync(o => o.Id == id);
                if (order == null)
                {
                    throw new NotFoundException("Order is not found");
                }
                var newStatus = request.Status.Trim();
                var checkStatus = new Dictionary<string, List<string>>
                {
                    {"Pending",new List<string> {"Confirmed","Cancelled"} },
                    {"Confirmed",new List<string> {"Cancelled","Shipped"} },
                    {"Cancelled",new List<string>{} },
                    {"Shipped",new List<string>{ } }
                };
                if (!checkStatus.ContainsKey(newStatus))
                {
                    throw new BadRequestException("New Status is not validate");
                }
                if (order.Status== "Cancelled" && newStatus == "Cancelled")
                {
                    throw new BadRequestException("This order is Cancelled");
                }
                if(newStatus=="Cancelled")
                {
                    foreach (var item in order.OrderItems)
                    {
                        item.Product.StockQuantity += item.Quantity;
                    }
                }
                order.Status = newStatus;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return new OrderResponse
                {
                    Id = order.Id,
                    UserId = order.UserId,
                    UserName = order.User!.Name,
                    Status = order.Status,
                    TotalAmount = order.TotalAmount,
                    CreatedDate = order.CreatedDate,
                    OrderItems = order.OrderItems.Select(oi => new OrderItemResponse
                    {
                        Id = oi.Id,
                        OrderId = oi.OrderId,
                        ProductId = oi.ProductId,
                        ProductName = oi.Product.ProductName,
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice,
                        LineTotal = oi.LineTotal,
                    }).ToList()
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> DeleteOrder(int id)
        {
            var order = await _context.Orders
                .Include(o=>o.OrderItems)
                .ThenInclude(oi=>oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id);
            if(order == null)
            {
                throw new NotFoundException("Order not found");
            }
            if(order.Status == "Pending")
            {
                foreach(var item in order.OrderItems)
                {
                    item.Product.StockQuantity += item.Quantity;
                }
                _context.Orders.Remove(order);
                _context.SaveChanges();
                return true;
            }
            else
            {
                throw new BadRequestException("Just only delete order when it's Pending status");
            }
        }
    }
}
