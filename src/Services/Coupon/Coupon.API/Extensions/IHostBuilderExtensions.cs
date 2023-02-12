using Coupon.API.IntegrationEvents.EventHandlers;
using Microsoft.Data.SqlClient;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using Microsoft.eShopOnContainers.Domain.Common.IntegrationEvents;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using System;

namespace Coupon.API.Extensions
{
    public static class IHostBuilderExtensions
    {
        public static IHost SeedDatabaseStrategy<TContext>(this IHost host, Action<TContext> seeder)
        {
            using (IServiceScope scope = host.Services.CreateScope())
            {
                TContext context = scope.ServiceProvider.GetService<TContext>();

                Polly.Retry.RetryPolicy policy = Policy.Handle<SqlException>()
                    .WaitAndRetry(new TimeSpan[]
                    {
                        TimeSpan.FromSeconds(3),
                        TimeSpan.FromSeconds(5),
                        TimeSpan.FromSeconds(8),
                    });

                policy.Execute(() =>
                {
                    seeder.Invoke(context);
                });
            }

            return host;
        }

        public static IHost SubscribersIntegrationEvents(this IHost host)
        {
            using (IServiceScope scope = host.Services.CreateScope())
            {
                IEventBus eventBus = scope.ServiceProvider.GetRequiredService<IEventBus>();

                eventBus.Subscribe<OrderStatusChangedToAwaitingCouponValidationIntegrationEvent, OrderStatusChangedToAwaitingCouponValidationIntegrationEventHandler>();
            }

            return host;
        }
    }
}
