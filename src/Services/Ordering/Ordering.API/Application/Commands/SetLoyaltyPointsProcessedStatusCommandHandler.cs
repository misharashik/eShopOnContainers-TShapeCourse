namespace Microsoft.eShopOnContainers.Services.Ordering.API.Application.Commands;

// Regular CommandHandler
public class SetLoyaltyPointsProcessedStatusCommandHandler : IRequestHandler<SetLoyaltyPointsProcessedStatusCommand, bool>
{
    private readonly IOrderRepository _orderRepository;

    public SetLoyaltyPointsProcessedStatusCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    /// <summary>
    /// Handler which processes the command when
    /// Stock service confirms the request
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public async Task<bool> Handle(SetLoyaltyPointsProcessedStatusCommand command, CancellationToken cancellationToken)
    {
        // Simulate a work time for confirming the stock
        await Task.Delay(10000, cancellationToken);

        Domain.AggregatesModel.OrderAggregate.Order orderToUpdate = await _orderRepository.GetAsync(command.OrderNumber);
        if (orderToUpdate == null)
        {
            return false;
        }

        orderToUpdate.SetLoyaltyPointsProcessedStatus();
        return await _orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}


// Use for Idempotency in Command process
public class SetLoyaltyPointsProcessedStatusvOrderStatusIdenfifiedCommandHandler : IdentifiedCommandHandler<SetLoyaltyPointsProcessedStatusCommand, bool>
{
    public SetLoyaltyPointsProcessedStatusvOrderStatusIdenfifiedCommandHandler(
        IMediator mediator,
        IRequestManager requestManager,
        ILogger<IdentifiedCommandHandler<SetLoyaltyPointsProcessedStatusCommand, bool>> logger)
        : base(mediator, requestManager, logger)
    {
    }

    protected override bool CreateResultForDuplicateRequest()
    {
        return true;                // Ignore duplicate requests for processing order.
    }
}
