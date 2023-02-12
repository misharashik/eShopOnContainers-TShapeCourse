using Microsoft.eShopOnContainers.Domain.Common.IntegrationEvents;

namespace Microsoft.eShopOnContainers.Services.Ordering.API.Application.DomainEventHandlers.OrderPaid;

public class OrderStatusChangedToPaidDomainEventHandler
                : INotificationHandler<OrderStatusChangedToPaidDomainEvent>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ILoggerFactory _logger;
    private readonly IBuyerRepository _buyerRepository;
    private readonly IOrderingIntegrationEventService _orderingIntegrationEventService;


    public OrderStatusChangedToPaidDomainEventHandler(
        IOrderRepository orderRepository, ILoggerFactory logger,
        IBuyerRepository buyerRepository,
        IOrderingIntegrationEventService orderingIntegrationEventService
        )
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _buyerRepository = buyerRepository ?? throw new ArgumentNullException(nameof(buyerRepository));
        _orderingIntegrationEventService = orderingIntegrationEventService ?? throw new ArgumentNullException(nameof(orderingIntegrationEventService));
    }

    public async Task Handle(OrderStatusChangedToPaidDomainEvent orderStatusChangedToPaidDomainEvent, CancellationToken cancellationToken)
    {
        _logger.CreateLogger<OrderStatusChangedToPaidDomainEventHandler>()
            .LogTrace("Order with Id: {OrderId} has been successfully updated to status {Status} ({Id})",
                orderStatusChangedToPaidDomainEvent.OrderId, nameof(OrderStatus.Paid), OrderStatus.Paid.Id);

        Domain.AggregatesModel.OrderAggregate.Order order = await _orderRepository.GetAsync(orderStatusChangedToPaidDomainEvent.OrderId);
        Buyer buyer = await _buyerRepository.FindByIdAsync(order.GetBuyerId.Value.ToString());

        IEnumerable<OrderStockItem> orderStockList = orderStatusChangedToPaidDomainEvent.OrderItems
            .Select(orderItem => new OrderStockItem(orderItem.ProductId, orderItem.GetUnits()));

        OrderStatusChangedToPaidIntegrationEvent orderStatusChangedToPaidIntegrationEvent = new OrderStatusChangedToPaidIntegrationEvent(
            orderStatusChangedToPaidDomainEvent.OrderId,
            order.OrderStatus.Name,
            buyer.Name,
            orderStockList);

        await _orderingIntegrationEventService.AddAndSaveEventAsync(orderStatusChangedToPaidIntegrationEvent);
    }
}
