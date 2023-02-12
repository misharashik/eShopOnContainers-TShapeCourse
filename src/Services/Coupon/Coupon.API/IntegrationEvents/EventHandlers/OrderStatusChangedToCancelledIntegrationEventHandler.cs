using Coupon.API.Infrastructure.Repositories;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using Microsoft.eShopOnContainers.Domain.Common.IntegrationEvents;
using System.Threading.Tasks;

namespace Coupon.API.IntegrationEvents.EventHandlers
{
    public class OrderStatusChangedToCancelledIntegrationEventHandler : IIntegrationEventHandler<OrderStatusChangedToCancelledIntegrationEvent>
    {
        private readonly ICouponRepository _couponRepository;

        public OrderStatusChangedToCancelledIntegrationEventHandler(ICouponRepository couponRepository)
        {
            _couponRepository = couponRepository;
        }

        public async Task Handle(OrderStatusChangedToCancelledIntegrationEvent @event)
        {
            await _couponRepository.UpdateCouponReleasedByOrderIdAsync(@event.OrderId);
        }
    }
}
