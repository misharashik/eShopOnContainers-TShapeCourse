using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

namespace Microsoft.eShopOnContainers.Domain.Common.IntegrationEvents;

public record OrderPaymentFailedIntegrationEvent : IntegrationEvent
{
    public int OrderId { get; }

    public OrderPaymentFailedIntegrationEvent(int orderId)
    {
        OrderId = orderId;
    }
}
