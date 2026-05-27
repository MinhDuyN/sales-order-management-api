using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderAPI.Data;
using OrderAPI.DTOs.Payment;
using OrderAPI.Entities;
using OrderAPI.Exceptions;
using OrderAPI.Services.Interfaces;

namespace OrderAPI.Services
{
    public class PaymentServcie : IPaymentService
    {
        private readonly AppDbContext _context;
        public PaymentServcie(AppDbContext context)
        {
            _context = context;
        }
        public async Task<PaymentResponse> GetPaymentById(int id)
        {
            var payment = await _context.Payments.FirstOrDefaultAsync(x => x.Id == id);
            if (payment == null)
            {
                throw new NotFoundException("PaymentId is not found");
            }
            return new PaymentResponse
            {
                Id = id,
                OrderId = payment.OrderId,
                Amount = payment.Amount,
                Status = payment.Status,
                Method = payment.Method,
                CreatedDate = payment.CreatedDate,
            };
        }

        public async Task<PaymentResponse> CreatePayment(CreatePaymentRequest request)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(x => x.Id == request.OrderId);
            if (order!=null)
            {
                if (order.Status == "Confirmed")
                {
                    var existingPayment = await _context.Payments
                    .AnyAsync(x => x.OrderId == request.OrderId && x.Status!= "Failed");

                    if (existingPayment)
                    {
                        throw new ConflictDataException($"OrderId {request.OrderId} already has a payment");
                    }
                        

                    var validMethod = new List<string>(new string[] {"Cash","Momo", "BankTransfer" });
                    if (validMethod.Contains(request.Method))
                    {
                        var payment = new Payment
                        {
                            OrderId = request.OrderId,
                            Amount = order.TotalAmount,
                            Status = "Pending",
                            Method = request.Method,
                            CreatedDate = DateTime.UtcNow
                        };
                        await _context.Payments.AddAsync(payment);
                        await _context.SaveChangesAsync();
                        return new PaymentResponse
                        {
                            Id = payment.Id,
                            OrderId = payment.OrderId,
                            Amount = payment.Amount,
                            Status = payment.Status,
                            Method = payment.Method,
                            CreatedDate = payment.CreatedDate
                        };
                    }
                    else
                    {
                        throw new BadRequestException($"Method is not validate");
                    }
                    
                }
                else
                {
                    throw new BadRequestException($"Order Status must be 'Confirmed' to create payment");
                }
            }
            else
            {
                throw new NotFoundException($"OrderId {request.OrderId} is not exists");
            }
        }

        public async Task<PaymentResponse> UpdatePayment(int id, UpdatePaymentRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var payment = await _context.Payments
                    .Include(x=>x.Order)
                    .FirstOrDefaultAsync(x => x.Id == id);
                if (payment == null)
                {
                    throw new NotFoundException("Payment is not found");
                }
                var validStatus = new Dictionary<string, List<string>>
                {
                    {"Pending",new List<string>{"Paid", "Failed" } },
                    {"Paid",new List<string>{} },
                    {"Failed",new List<string>{} }
                };
                var newStatus = request.Status.Trim();
                if (!validStatus.ContainsKey(newStatus))
                {
                    throw new BadRequestException("NewStatus is not validate");
                }
                payment.Status = newStatus;

                if (newStatus == "Paid")
                {
                    payment.Order.Status = "Shipped";
                }
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return new PaymentResponse
                {
                    Id = payment.Id,
                    OrderId = payment.OrderId,
                    Amount = payment.Amount,
                    Status = payment.Status,
                    Method = payment.Method,
                    CreatedDate = payment.CreatedDate
                };
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
