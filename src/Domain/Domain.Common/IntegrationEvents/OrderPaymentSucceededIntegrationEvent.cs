using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

namespace Microsoft.eShopOnContainers.Domain.Common.IntegrationEvents;

public record OrderPaymentSucceededIntegrationEvent : IntegrationEvent
{
    public int OrderId { get; }

    public OrderPaymentSucceededIntegrationEvent(int orderId)
    {
        OrderId = orderId;
    }
}
