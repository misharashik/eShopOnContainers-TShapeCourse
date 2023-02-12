using Microsoft.eShopOnContainers.Domain.Common.IntegrationEvents;

namespace Microsoft.eShopOnContainers.Services.Ordering.API.Application.DomainEventHandlers.OrderStockConfirmed;

public class OrderStatusChangedToLoyaltyPointsProcessedDomainEventHandler
                : INotificationHandler<OrderStatusChangedToLoyaltyPointsProcessedDomainEvent>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IBuyerRepository _buyerRepository;
    private readonly ILoggerFactory _logger;
    private readonly IOrderingIntegrationEventService _orderingIntegrationEventService;

    public OrderStatusChangedToLoyaltyPointsProcessedDomainEventHandler(
        IOrderRepository orderRepository,
        IBuyerRepository buyerRepository,
        ILoggerFactory logger,
        IOrderingIntegrationEventService orderingIntegrationEventService)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _buyerRepository = buyerRepository ?? throw new ArgumentNullException(nameof(buyerRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _orderingIntegrationEventService = orderingIntegrationEventService;
    }

    public async Task Handle(OrderStatusChangedToLoyaltyPointsProcessedDomainEvent orderStatusChangedToLoyaltyPointsProcessedDomainEvent, CancellationToken cancellationToken)
    {
        _logger.CreateLogger<OrderStatusChangedToLoyaltyPointsProcessedDomainEvent>()
            .LogTrace("Order with Id: {OrderId} has been successfully updated to status {Status} ({Id})",
                orderStatusChangedToLoyaltyPointsProcessedDomainEvent.OrderId, nameof(OrderStatus.LoyaltyPointsProcessed), OrderStatus.LoyaltyPointsProcessed.Id);

        Domain.AggregatesModel.OrderAggregate.Order order = await _orderRepository.GetAsync(orderStatusChangedToLoyaltyPointsProcessedDomainEvent.OrderId);
        Buyer buyer = await _buyerRepository.FindByIdAsync(order.GetBuyerId.Value.ToString());

        OrderStatusChangedToLoyaltyPointsProcessedIntegrationEvent integrationEvent =
            new OrderStatusChangedToLoyaltyPointsProcessedIntegrationEvent(order.Id, order.OrderStatus.Name, buyer.Name);
        await _orderingIntegrationEventService.AddAndSaveEventAsync(integrationEvent);
    }
}
