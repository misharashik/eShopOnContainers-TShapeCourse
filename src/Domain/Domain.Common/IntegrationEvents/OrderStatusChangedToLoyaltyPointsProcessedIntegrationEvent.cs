using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

namespace Microsoft.eShopOnContainers.Domain.Common.IntegrationEvents;

public record OrderStatusChangedToLoyaltyPointsProcessedIntegrationEvent : IntegrationEvent
{
    public int OrderId { get; }
    public string OrderStatus { get; }
    public string BuyerName { get; }

    public OrderStatusChangedToLoyaltyPointsProcessedIntegrationEvent(int orderId, string orderStatus, string buyerName)
    {
        OrderId = orderId;
        OrderStatus = orderStatus;
        BuyerName = buyerName;
    }
}
