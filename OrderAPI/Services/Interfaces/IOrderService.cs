using OrderAPI.DTOs.Common;
using OrderAPI.DTOs.Order;

namespace OrderAPI.Services.Interfaces
{
    public interface IOrderService
    {
        Task<PagedList<OrderResponse>> GetAllOrder(string? keyword, int?userId, DateTime? startDate, DateTime? endDate, int page, int pageSize);
        Task<OrderResponse> GetOrderById(int id);

        Task<OrderResponse> CreateOrder(int userId, CreateOrderRequest request);
        Task<OrderResponse> UpdateOrder(int id, UpdateOrderRequest request);
        Task<bool> DeleteOrder(int id);
    }
}
