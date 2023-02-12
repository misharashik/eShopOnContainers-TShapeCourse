using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

namespace Microsoft.eShopOnContainers.Domain.Common.IntegrationEvents;

public record OrderLoyaltyPointsProcessisngFailedIntegrationEvent : IntegrationEvent
{
    public int OrderId { get; }

    public OrderLoyaltyPointsProcessisngFailedIntegrationEvent(int orderId)
    {
        OrderId = orderId;
    }
}
