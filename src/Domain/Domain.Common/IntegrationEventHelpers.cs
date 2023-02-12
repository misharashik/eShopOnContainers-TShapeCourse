using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;
using Microsoft.eShopOnContainers.Domain.Common.IntegrationEvents;
using System.Reflection;

namespace Domain.Common
{
    internal class IntegrationEventHelpers
    {
        public void GetEventTypes()
        {
            List<Type> eventTypes = Assembly.Load(
                Assembly.GetAssembly(typeof(OrderStartedIntegrationEvent)).FullName)
                .GetTypes()
                .Where(t => t.Name.EndsWith(nameof(IntegrationEvent)))
                .ToList();
        }
    }
}
