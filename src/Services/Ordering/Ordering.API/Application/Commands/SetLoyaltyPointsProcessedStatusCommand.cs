namespace Microsoft.eShopOnContainers.Services.Ordering.API.Application.Commands;

public class SetLoyaltyPointsProcessedStatusCommand : IRequest<bool>
{

    [DataMember]
    public int OrderNumber { get; private set; }

    public SetLoyaltyPointsProcessedStatusCommand(int orderNumber)
    {
        OrderNumber = orderNumber;
    }
}
