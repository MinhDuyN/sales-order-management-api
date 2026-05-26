using OrderAPI.DTOs.Payment;

namespace OrderAPI.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentResponse> GetPaymentById(int id);
        Task<PaymentResponse> CreatePayment(CreatePaymentRequest request);
        Task<PaymentResponse> UpdatePayment(int id, UpdatePaymentRequest request);
    }
}
