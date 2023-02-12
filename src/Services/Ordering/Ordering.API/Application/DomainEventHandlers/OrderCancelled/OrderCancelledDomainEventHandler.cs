using Microsoft.eShopOnContainers.Domain.Common.IntegrationEvents;

namespace Microsoft.eShopOnContainers.Services.Ordering.API.Application.DomainEventHandlers.OrderCancelled;

public class OrderCancelledDomainEventHandler
                : INotificationHandler<OrderCancelledDomainEvent>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IBuyerRepository _buyerRepository;
    private readonly ILoggerFactory _logger;
    private readonly IOrderingIntegrationEventService _orderingIntegrationEventService;

    public OrderCancelledDomainEventHandler(
        IOrderRepository orderRepository,
        ILoggerFactory logger,
        IBuyerRepository buyerRepository,
        IOrderingIntegrationEventService orderingIntegrationEventService)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _buyerRepository = buyerRepository ?? throw new ArgumentNullException(nameof(buyerRepository));
        _orderingIntegrationEventService = orderingIntegrationEventService;
    }

    public async Task Handle(OrderCancelledDomainEvent orderCancelledDomainEvent, CancellationToken cancellationToken)
    {
        _logger.CreateLogger<OrderCancelledDomainEvent>()
            .LogTrace("Order with Id: {OrderId} has been successfully updated to status {Status} ({Id})",
                orderCancelledDomainEvent.Order.Id, nameof(OrderStatus.Cancelled), OrderStatus.Cancelled.Id);

        Domain.AggregatesModel.OrderAggregate.Order order = await _orderRepository.GetAsync(orderCancelledDomainEvent.Order.Id);
        Buyer buyer = await _buyerRepository.FindByIdAsync(order.GetBuyerId.Value.ToString());

        OrderStatusChangedToCancelledIntegrationEvent orderStatusChangedToCancelledIntegrationEvent =
            new OrderStatusChangedToCancelledIntegrationEvent(
                order.Id, order.OrderStatus.Name, buyer.IdentityGuid, buyer.Name, "NO_DISCOUNT", 0, order.GetTotal());
        //TODO: Add discount code and loyalty point debit

        await _orderingIntegrationEventService.AddAndSaveEventAsync(orderStatusChangedToCancelledIntegrationEvent);
    }
}
