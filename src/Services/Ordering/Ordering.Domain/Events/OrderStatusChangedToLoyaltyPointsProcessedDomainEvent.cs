namespace Microsoft.eShopOnContainers.Services.Ordering.Domain.Events;

/// <summary>
/// Event used when the order's loyalty points are processed
/// </summary>
public class OrderStatusChangedToLoyaltyPointsProcessedDomainEvent
    : INotification
{
    public int OrderId { get; }

    public OrderStatusChangedToLoyaltyPointsProcessedDomainEvent(int orderId)
    {
        OrderId = orderId;
    }
}
