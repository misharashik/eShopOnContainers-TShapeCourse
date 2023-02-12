using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

namespace Microsoft.eShopOnContainers.Domain.Common.IntegrationEvents;

public record OrderStatusChangedToCancelledIntegrationEvent : IntegrationEvent
{
    public int OrderId { get; init; }
    public string OrderStatus { get; init; }
    public string BuyerId { get; init; }
    public string BuyerName { get; init; }
    public string DiscountCode { get; init; }
    public decimal LoyaltyPointsDebit { get; init; }
    public decimal LoyaltyPointsCredit { get; init; }

    public OrderStatusChangedToCancelledIntegrationEvent(
        int orderId,
        string orderStatus,
        string buyerId,
        string buyerName,
        string discountCode,
        decimal loyaltyPointsDebit,
        decimal loyaltyPointsCredit)
    {
        OrderId = orderId;
        OrderStatus = orderStatus;
        BuyerId = buyerId;
        BuyerName = buyerName;
        DiscountCode = discountCode;
        LoyaltyPointsDebit = loyaltyPointsDebit;
        LoyaltyPointsCredit = loyaltyPointsCredit;
    }
}
