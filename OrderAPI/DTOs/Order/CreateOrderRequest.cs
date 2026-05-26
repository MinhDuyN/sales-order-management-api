using System.ComponentModel.DataAnnotations;

namespace OrderAPI.DTOs.Order
{
    public class CreateOrderRequest
    {
        [Required(ErrorMessage = "OrderItems is required")]
        public List<CreateOrderItemRequest> createOrderItemRequests { get; set; }
    }
}
