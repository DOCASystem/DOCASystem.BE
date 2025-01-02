using System.Transactions;
using AutoMapper;
using DOCA.API.Constants;
using DOCA.API.Enums;
using DOCA.API.Payload.Request.CheckOut;
using DOCA.API.Payload.Response.Cart;
using DOCA.API.Payload.Response.CheckOut;
using DOCA.API.Services.Interface;
using DOCA.API.Utils;
using DOCA.Domain.Models;
using DOCA.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Net.payOS;
using Net.payOS.Types;
using Newtonsoft.Json;

namespace DOCA.API.Services.Implement;

public class PaymentService : BaseService<PaymentService>, IPaymentService
{
    private IConfiguration _configuration;
    // private readonly IRedisService _redisService;
    public PaymentService(IUnitOfWork<DOCADbContext> unitOfWork, ILogger<PaymentService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor, IConfiguration configuration, IRedisService redisService) : base(unitOfWork, logger, mapper, httpContextAccessor, configuration)
    {
        _configuration = configuration;
        // _redisService = redisService;
    }

    public async Task<string> CheckOut(CheckOutRequest request)
    {
         PayOS payOs = new PayOS(_configuration["PAYOS:PAYOS_CLIENT_ID"]!,
            _configuration["PAYOS:PAYOS_API_KEY"]!,
            _configuration["PAYOS:PAYOS_CHECKSUM_KEY"]!);
        
        var userId = GetUserIdFromJwt();
        
        var member = await _unitOfWork.GetRepository<Member>().SingleOrDefaultAsync(
            predicate: m => m.UserId == userId,
            include: m => m.Include(m => m.User)
            );
        if (member == null) throw new UnauthorizedAccessException(MessageConstant.User.UserNotFound);
        if( member.Commune == null || member.Province == null || member.District == null || member.Address == null) 
            throw new BadHttpRequestException(MessageConstant.User.MemberAddressNotFound);
        
        var key = "Cart:" + userId;
        // var cartData = await _redisService.GetStringAsync(key);

        // if (string.IsNullOrEmpty(cartData)) throw new BadHttpRequestException(MessageConstant.Cart.CartNotFound);
        //
        // var cart = JsonConvert.DeserializeObject<List<CartModelResponse>>(cartData);
        
        int orderCode = int.Parse(DateTimeOffset.Now.ToString("ffffff"));
        var order = new Order()
        {
            Id = Guid.NewGuid(),
            Code = "ORDER-" + orderCode,
            CreatedAt = TimeUtil.GetCurrentSEATime(),
            ModifiedAt = TimeUtil.GetCurrentSEATime(),
            Status = OrderStatus.Pending,
            MemberId = member.Id
        };
        
        var orderItems = new List<OrderItem>();
        decimal orderTotal = 0;
        List<ItemData> items = new List<ItemData>();
        
        // foreach (var cartModel in cart)
        // {
        //
        //     var product = await _unitOfWork.GetRepository<Product>().SingleOrDefaultAsync(
        //         predicate: x => x.Id == cartModel.ProductId,
        //         include: p => p
        //             .Include(p => p.ProductImages)
        //             .Include(p => p.ProductCategories)
        //             .ThenInclude(pc => pc.Category)
        //     );
        //     var blog = await _unitOfWork.GetRepository<Blog>().SingleOrDefaultAsync(
        //         predicate: x => x.Id == cartModel.BlogId,
        //         include: b => b
        //             .Include(b => b.BlogCategoryRelationship)
        //             .ThenInclude(b => b.BlogCategory)
        //             .Include(b => b.BlogAnimal)
        //             .ThenInclude(b => b.Animal)
        //     );
        //     if (product.Quantity < cartModel.Quantity) throw new BadHttpRequestException(MessageConstant.Product.ProductOutOfStock);
        //     var orderItem = new OrderItem()
        //     {
        //         Id = Guid.NewGuid(),
        //         ProductId = product.Id,
        //         BlogID = blog.Id,
        //         Quantity = cartModel.Quantity,
        //         CreatedAt = TimeUtil.GetCurrentSEATime(),
        //         ModifiedAt =TimeUtil.GetCurrentSEATime(),
        //         Order = order,
        //         WarrantyCode = CodeUtil.GenerateWarrantyCode(product.Id),
        //         WarrantyExpired = null
        //     };
        //     orderItems.Add(orderItem);
        //     
        //     decimal itemTotal = product.Price * cartModel.Quantity;
        //     orderTotal += itemTotal;
        //     var item = new ItemData(product.Name, cartModel.Quantity, (int) itemTotal);
        //     items.Add(item);
        // }

        order.Total = orderTotal;
        order.Address = request.Address;
        var payment = new Payment()
        {
            Id = Guid.NewGuid(),
            OrderCode = orderCode,
            CreatedAt = TimeUtil.GetCurrentSEATime(),
            ModifiedAt = TimeUtil.GetCurrentSEATime(),
            Status = PaymentStatus.Processing,
            Amount = order.Total,
            Order = order
        };
        using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            try
            {
                // foreach (var cartModel in cart)
                // {
                //     var productCarts = await _unitOfWork.GetRepository<Product>().SingleOrDefaultAsync(
                //         predicate: x => x.Id == cartModel.ProductId,
                //         include: p => p
                //             .Include(p => p.ProductImages)
                //             .Include(p => p.ProductCategories)
                //             .ThenInclude(pc => pc.Category)
                //     );
                //     productCarts.Quantity -= cartModel.Quantity;
                //     _unitOfWork.GetRepository<Product>().UpdateAsync(productCarts);
                // }
                await _unitOfWork.GetRepository<Order>().InsertAsync(order);
                await _unitOfWork.GetRepository<Payment>().InsertAsync(payment);
                
                foreach (var orderItem in orderItems) 
                { 
                    orderItem.OrderId = order.Id; 
                } 
                await _unitOfWork.GetRepository<OrderItem>().InsertRangeAsync(orderItems);
                
                var isSuccess = await _unitOfWork.CommitAsync() > 0; 
                transaction.Complete(); 
                if (!isSuccess) throw new BadHttpRequestException(MessageConstant.Order.CreateOrderFail);
                PaymentData paymentData = new PaymentData(
                    orderCode, 
                    (int)order.Total, 
                    "Thanh toán đơn hàng", 
                    items, 
                    "https://stemlabs.store/cancel", 
                    "https://stemlabs.store/success",
                    buyerName: member.User.FullName, 
                    buyerPhone: member.User.PhoneNumber,
                    expiredAt: ((DateTimeOffset) TimeUtil.GetCurrentSEATime().AddMinutes(10)).ToUnixTimeSeconds()
                    );
                // Call the external payment service to create a payment link
                CreatePaymentResult createPayment = await payOs.createPaymentLink(paymentData);
                
                return createPayment.checkoutUrl;
            }
            catch (TransactionException ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return null;
            }
        }
    }

    public async Task<PaymentWithOrderResponse> HandlePayment( UpdatePaymentOrderStatusRequest request)
    {
         if(request.OrderCode == 0) throw new BadHttpRequestException(MessageConstant.Payment.OrderCodeNotNull);
        
        var payment = await _unitOfWork.GetRepository<Payment>().SingleOrDefaultAsync(
            predicate: p => p.OrderCode == request.OrderCode,
            include: p => p.Include(p => p.Order)
            );
        if (payment == null) throw new BadHttpRequestException(MessageConstant.Payment.PaymentNotFound);
        
        if (payment.Status == PaymentStatus.Paid)
            throw new BadHttpRequestException(MessageConstant.Payment.YourOrderIsPaid);
        if (payment.Status == PaymentStatus.Fail)
            throw new BadHttpRequestException(MessageConstant.Payment.YourOrderIsCancelled);
        
        PayOS _payOs = new PayOS(_configuration["PAYOS:PAYOS_CLIENT_ID"] ?? throw new Exception("Cannot find environment"),
            _configuration["PAYOS:PAYOS_API_KEY"] ?? throw new Exception("Cannot find environment"),
            _configuration["PAYOS:PAYOS_CHECKSUM_KEY"] ?? throw new Exception("Cannot find environment"));
        
        PaymentLinkInformation paymentLinkInformation = await _payOs.getPaymentLinkInformation(request.OrderCode);
        if(paymentLinkInformation == null) 
            throw new BadHttpRequestException(MessageConstant.Payment.CannotFindPaymentLinkInformation);
        if (paymentLinkInformation.status == PayOsStatus.PENDING.ToString())
            throw new BadHttpRequestException(MessageConstant.Payment.YourOrderIsNotPaid);
        
        using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            try
            {
                switch (EnumUtil.ParseEnum<PayOsStatus>(paymentLinkInformation.status))
                {
                    case PayOsStatus.PAID:
                        payment.Status = PaymentStatus.Paid;
                        payment.ModifiedAt = TimeUtil.GetCurrentSEATime();
                        payment.PaymentDateTime =
                            DateTime.Parse(paymentLinkInformation.transactions[0].transactionDateTime);
                        payment.Order.Status = OrderStatus.Prepare;
                        payment.Order.ModifiedAt = TimeUtil.GetCurrentSEATime();
                        _unitOfWork.GetRepository<Payment>().UpdateAsync(payment);

                        // var orderItems = await _orderItemRepository.GetOrderItemByOrderIdAsync(payment.Order.Id);
                        // foreach (var orderItem in orderItems)
                        // {
                        //     orderItem.Product.Quantity -= orderItem.Quantity;
                        // }
                        // _orderItemRepository.UpdateRangeAsync(orderItems);
                        break;
                    case PayOsStatus.EXPIRED:
                    case PayOsStatus.CANCELLED:
                        payment.Status = PaymentStatus.Fail;
                        payment.ModifiedAt = TimeUtil.GetCurrentSEATime();
                        payment.Order.Status = OrderStatus.Cancelled;
                        payment.Order.ModifiedAt = TimeUtil.GetCurrentSEATime();
                        // _unitOfWork.GetRepository<Payment>().UpdateAsync(payment);
                        _unitOfWork.GetRepository<Payment>().UpdateAsync(payment);
                        var orderItems = await _unitOfWork.GetRepository<OrderItem>().GetListAsync(
                            predicate: oi => oi.OrderId == payment.Order.Id,
                            include: oi => oi.Include(oi => oi.Product)
                                .ThenInclude(p => p.ProductImages)
                            );
                        foreach (var orderItem in orderItems)
                        {
                            orderItem.Product.Quantity += orderItem.Quantity;
                        }
                        _unitOfWork.GetRepository<OrderItem>().UpdateRange(orderItems);
                        break;
                    default:
                        throw new BadHttpRequestException(MessageConstant.Payment.PayOsStatusNotTrue);
                }

                bool isSuccess = await _unitOfWork.CommitAsync() > 0;
                transaction.Complete();
                PaymentWithOrderResponse response = null;
                if (isSuccess) response = _mapper.Map<PaymentWithOrderResponse>(payment);
                return response;
            }
            catch (TransactionException ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return null;
            }
        }
    }

    public async Task<bool> UpdateExpiredPayment()
    {
        var paymentExpires = await _unitOfWork.GetRepository<Payment>().GetListAsync(
            predicate: p => p.Status == PaymentStatus.Processing && p.CreatedAt.AddMinutes(10) < DateTime.Now,
            include: p => p.Include(p => p.Order)
            );
        var hasExpiredPayments = false;
        if (paymentExpires.Any())
        {
            foreach (var paymentExpire in paymentExpires)
            {
                
                paymentExpire.Status = PaymentStatus.Fail;
                paymentExpire.ModifiedAt = TimeUtil.GetCurrentSEATime();
                paymentExpire.Order.Status = OrderStatus.Cancelled;
                paymentExpire.Order.ModifiedAt = TimeUtil.GetCurrentSEATime();
                _unitOfWork.GetRepository<Payment>().UpdateAsync(paymentExpire);
                var orderItems = await _unitOfWork.GetRepository<OrderItem>().GetListAsync(
                    predicate: oi => oi.OrderId == paymentExpire.Order.Id,
                    include: oi => oi.Include(oi => oi.Product)
                        .ThenInclude(p => p.ProductImages)
                );
                foreach (var orderItem in orderItems)
                {
                    orderItem.Product.Quantity += orderItem.Quantity;
                }
                _unitOfWork.GetRepository<OrderItem>().UpdateRange(orderItems);
                hasExpiredPayments = true;
            }
        }
        if (hasExpiredPayments)
        {
            var isSuccess = await _unitOfWork.CommitAsync() > 0;
            return isSuccess;
        }
        return false;
    }
}