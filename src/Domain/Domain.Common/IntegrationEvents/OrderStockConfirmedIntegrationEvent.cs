using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

namespace Microsoft.eShopOnContainers.Domain.Common.IntegrationEvents;

public record OrderStockConfirmedIntegrationEvent : IntegrationEvent
{
    public int OrderId { get; }

    public OrderStockConfirmedIntegrationEvent(int orderId)
    {
        OrderId = orderId;
    }
}
