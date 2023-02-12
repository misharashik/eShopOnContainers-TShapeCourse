using Coupon.API.Infrastructure.Repositories;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;
using Microsoft.eShopOnContainers.Domain.Common.IntegrationEvents;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Context;
using System.Threading.Tasks;

namespace Coupon.API.IntegrationEvents.EventHandlers;

public class OrderStatusChangedToStockConfirmedIntegrationEventHandler :
    IIntegrationEventHandler<OrderStatusChangedToStockConfirmedIntegrationEvent>
{
    private readonly ILoyaltyCardRepository _loyaltyCardRepository;
    private readonly IEventBus _eventBus;
    private readonly ILogger<OrderStatusChangedToStockConfirmedIntegrationEventHandler> _logger;

    public OrderStatusChangedToStockConfirmedIntegrationEventHandler(
        ILoyaltyCardRepository loyaltyCardRepository,
        IEventBus eventBus,
        ILogger<OrderStatusChangedToStockConfirmedIntegrationEventHandler> logger)
    {
        _loyaltyCardRepository = loyaltyCardRepository;
        _eventBus = eventBus;
        _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
    }

    public async Task Handle(OrderStatusChangedToStockConfirmedIntegrationEvent @event)
    {
        using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{"Program.AppName"}"))
        {
            _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", @event.Id, "Program.AppName", @event);

            IntegrationEvent orderPaymentIntegrationEvent = await ProcessIntegrationEventAsync(@event);

            _logger.LogInformation("----- Publishing integration event: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})", orderPaymentIntegrationEvent.Id, "Program.AppName", orderPaymentIntegrationEvent);

            _eventBus.Publish(orderPaymentIntegrationEvent);

            await Task.CompletedTask;
        }
    }

    private async Task<IntegrationEvent> ProcessIntegrationEventAsync(OrderStatusChangedToStockConfirmedIntegrationEvent integrationEvent)
    {
        Infrastructure.Models.LoyaltyCard loyaltyCard = await _loyaltyCardRepository.FindLoyaltyCardByBuyerAsync(integrationEvent.Buyerid);

        Log.Information("----- Loyalty card \"{LoyaltyCard}\": {@Coupon}", integrationEvent.Buyerid, loyaltyCard);

        decimal earnedPoints = integrationEvent.OrderTotal;
        decimal usedPoints = integrationEvent.OrderLoyaltyPoints;

        bool processResult = await _loyaltyCardRepository.TryProcessLoyaltyEntryAsync(
            integrationEvent.Buyerid,
            new Infrastructure.Models.LoyaltyCardEntry()
            {
                OrderId = integrationEvent.OrderId,
                EarnedPoints = earnedPoints,
                UsedPoints = usedPoints
            });

        if (processResult)
        {
            Log.Information("Loyalty points processed: Earned {EarnedPoints}, Used {UsedPoints}", earnedPoints, usedPoints);
            return new OrderLoyaltyPointsProcessisngSucceededIntegrationEvent(integrationEvent.OrderId);
        }

        Log.Information("Loyalty points processing failed: Earned {EarnedPoints}, Used {UsedPoints}", earnedPoints, usedPoints);
        return new OrderLoyaltyPointsProcessisngFailedIntegrationEvent(integrationEvent.OrderId);
    }
}
