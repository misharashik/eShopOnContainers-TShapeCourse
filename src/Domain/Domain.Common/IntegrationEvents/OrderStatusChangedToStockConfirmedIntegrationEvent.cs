using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

namespace Microsoft.eShopOnContainers.Domain.Common.IntegrationEvents;

public record OrderStatusChangedToStockConfirmedIntegrationEvent : IntegrationEvent
{
    public OrderStatusChangedToStockConfirmedIntegrationEvent(
        int orderId,
        string orderStatus,
        int buyerid,
        string buyerName,
        decimal orderTotal,
        decimal orderLoyaltyPoints)
    {
        OrderId = orderId;
        OrderStatus = orderStatus;
        Buyerid = buyerid;
        BuyerName = buyerName;
        OrderTotal = orderTotal;
        OrderLoyaltyPoints = orderLoyaltyPoints;
    }

    public int OrderId { get; }
    public string OrderStatus { get; }
    public int Buyerid { get; }
    public string BuyerName { get; }

    public decimal OrderTotal { get; }
    public decimal OrderLoyaltyPoints { get; }
}
