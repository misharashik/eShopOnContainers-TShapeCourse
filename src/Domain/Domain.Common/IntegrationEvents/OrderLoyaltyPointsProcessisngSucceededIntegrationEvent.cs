using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

namespace Microsoft.eShopOnContainers.Domain.Common.IntegrationEvents;

public record OrderLoyaltyPointsProcessisngSucceededIntegrationEvent : IntegrationEvent
{
    public int OrderId { get; }

    public OrderLoyaltyPointsProcessisngSucceededIntegrationEvent(int orderId)
    {
        OrderId = orderId;
    }
}
