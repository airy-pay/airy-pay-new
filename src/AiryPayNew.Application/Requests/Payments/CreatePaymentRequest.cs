﻿using AiryPayNew.Application.Payments;
using AiryPayNew.Domain.Common;
using AiryPayNew.Domain.Entities.Bills;
using AiryPayNew.Domain.Entities.Products;
using AiryPayNew.Domain.Entities.Shops;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AiryPayNew.Application.Requests.Payments;

public record CreatePaymentRequest(
    ProductId ProductId, string PaymentServiceName, string PaymentMethodId, ulong BuyerId, ulong ShopId)
    : IRequest<OperationResult<string>>;

public class CreatePaymentRequestHandler(
    IBillRepository billRepository,
    IProductRepository productRepository,
    IShopRepository shopRepository,
    IEnumerable<IPaymentService> paymentServices,
    ILogger<CreatePaymentRequestHandler> logger) : IRequestHandler<CreatePaymentRequest, OperationResult<string>>
{
    public async Task<OperationResult<string>> Handle(CreatePaymentRequest request, CancellationToken cancellationToken)
    {
        var shop = await shopRepository.GetByIdNoTrackingAsync(new ShopId(request.ShopId), cancellationToken);
        if (shop is null)
            return Error("Shop not found.");
        if (shop.Blocked)
            return Error("Shop is blocked.");
        
        var paymentService = paymentServices.FirstOrDefault(x =>
            x.GetServiceName() == request.PaymentServiceName);
        if (paymentService is null)
            return Error("Payment service not found.");
        
        var product = await productRepository.GetByIdNoTrackingAsync(request.ProductId, cancellationToken);
        if (product is null)
            return Error("Product not found.");
        if (product.ShopId != shop.Id)
            return Error("Access denied.");
        
        var newBill = new Bill
        {
            BillStatus = BillStatus.Unpaid,
            BuyerDiscordId = request.BuyerId,
            ProductId = product.Id,
            ShopId = shop.Id
        };
        
        newBill.Id = await billRepository.CreateAsync(newBill, cancellationToken);

        var bill = await billRepository.GetByIdNoTrackingAsync(newBill.Id, cancellationToken);
        if (bill is null)
            return Error("Failed to create payment.");
        
        var paymentUrl = await paymentService.CreateAsync(bill, request.PaymentMethodId);

        logger.LogInformation(string.Format(
            "Successfully created a new payment for bill #{0}",
            newBill.Id));
        
        return OperationResult<string>.Success(paymentUrl.Entity);
    }

    private static OperationResult<string> Error(string message)
    {
        return OperationResult<string>.Error(string.Empty, message);
    }
}