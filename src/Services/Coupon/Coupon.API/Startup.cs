using Coupon.API.Extensions;
using Coupon.API.Filters;
using Coupon.API.IntegrationEvents.EventHandlers;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using Microsoft.eShopOnContainers.Domain.Common.IntegrationEvents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Coupon.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(options => options.Filters.Add<ValidateModelAttribute>());

            services.AddCustomSettings(Configuration)
                .AddCouponRegister(Configuration)
                .AddCustomPolicies()
                .AddAppInsights(Configuration)
                .AddEventBus(Configuration)
                .AddCustomAuthentication(Configuration)
                .AddCustomAuthorization()
                .AddSwagger(Configuration)
                .AddCustomHealthCheck(Configuration);

            services.AddTransient<IIntegrationEventHandler<OrderStatusChangedToAwaitingCouponValidationIntegrationEvent>, OrderStatusChangedToAwaitingCouponValidationIntegrationEventHandler>();
            services.AddTransient<IIntegrationEventHandler<OrderStatusChangedToCancelledIntegrationEvent>, OrderStatusChangedToCancelledIntegrationEventHandler>();
            services.AddTransient<IIntegrationEventHandler<OrderStatusChangedToStockConfirmedIntegrationEvent>, OrderStatusChangedToStockConfirmedIntegrationEventHandler>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            string pathBase = Configuration["PATH_BASE"];

            if (!string.IsNullOrEmpty(pathBase))
            {
                app.UsePathBase(pathBase);
            }

            app.UseSwagger()
                .UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint($"{(!string.IsNullOrEmpty(pathBase) ? pathBase : string.Empty)}/swagger/v1/swagger.json", "Coupon.API V1");
                    options.OAuthClientId("couponswaggerui");
                    options.OAuthAppName("eShop-Learn.Coupon.API Swagger UI");
                })
                .UseCors("CorsPolicy")
                .UseRouting()
                .UseAuthentication()
                .UseAuthorization()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();

                    // Add the endpoints.MapHealthChecks code
                    endpoints.MapHealthChecks("/hc", new HealthCheckOptions()
                    {
                        Predicate = _ => true,
                        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                    });
                    endpoints.MapHealthChecks("/liveness", new HealthCheckOptions
                    {
                        Predicate = r => r.Name.Contains("self")
                    });
                });

            ConfigureEventBus(app);
        }

        private void ConfigureEventBus(IApplicationBuilder app)
        {
            IEventBus eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();

            eventBus.Subscribe<OrderStatusChangedToAwaitingCouponValidationIntegrationEvent, IIntegrationEventHandler<OrderStatusChangedToAwaitingCouponValidationIntegrationEvent>>();
            eventBus.Subscribe<OrderStatusChangedToCancelledIntegrationEvent, IIntegrationEventHandler<OrderStatusChangedToCancelledIntegrationEvent>>();
            eventBus.Subscribe<OrderStatusChangedToStockConfirmedIntegrationEvent, IIntegrationEventHandler<OrderStatusChangedToStockConfirmedIntegrationEvent>>();
        }
    }
}
