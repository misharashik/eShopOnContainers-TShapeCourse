using Microsoft.eShopOnContainers.Domain.Common.IntegrationEvents;

namespace Microsoft.eShopOnContainers.Payment.API;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public IServiceProvider ConfigureServices(IServiceCollection services)
    {
        services.AddCustomHealthCheck(Configuration);
        services.Configure<PaymentSettings>(Configuration);

        RegisterAppInsights(services);

        if (Configuration.GetValue<bool>("AzureServiceBusEnabled"))
        {
            services.AddSingleton<IServiceBusPersisterConnection>(sp =>
            {
                string serviceBusConnectionString = Configuration["EventBusConnection"];
                string subscriptionClientName = Configuration["SubscriptionClientName"];

                return new DefaultServiceBusPersisterConnection(serviceBusConnectionString);
            });
        }
        else
        {
            services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
            {
                ILogger<DefaultRabbitMQPersistentConnection> logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();
                ConnectionFactory factory = new ConnectionFactory()
                {
                    HostName = Configuration["EventBusConnection"],
                    DispatchConsumersAsync = true
                };

                if (!string.IsNullOrEmpty(Configuration["EventBusUserName"]))
                {
                    factory.UserName = Configuration["EventBusUserName"];
                }

                if (!string.IsNullOrEmpty(Configuration["EventBusPassword"]))
                {
                    factory.Password = Configuration["EventBusPassword"];
                }

                int retryCount = 5;
                if (!string.IsNullOrEmpty(Configuration["EventBusRetryCount"]))
                {
                    retryCount = int.Parse(Configuration["EventBusRetryCount"]);
                }

                return new DefaultRabbitMQPersistentConnection(factory, logger, retryCount);
            });
        }

        RegisterEventBus(services);

        ContainerBuilder container = new ContainerBuilder();
        container.Populate(services);
        return new AutofacServiceProvider(container.Build());
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
    {
        //loggerFactory.AddAzureWebAppDiagnostics();
        //loggerFactory.AddApplicationInsights(app.ApplicationServices, LogLevel.Trace);

        string pathBase = Configuration["PATH_BASE"];
        if (!string.IsNullOrEmpty(pathBase))
        {
            app.UsePathBase(pathBase);
        }

        ConfigureEventBus(app);

        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
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
    }

    private void RegisterAppInsights(IServiceCollection services)
    {
        services.AddApplicationInsightsTelemetry(Configuration);
        services.AddApplicationInsightsKubernetesEnricher();
    }

    private void RegisterEventBus(IServiceCollection services)
    {
        if (Configuration.GetValue<bool>("AzureServiceBusEnabled"))
        {
            services.AddSingleton<IEventBus, EventBusServiceBus>(sp =>
            {
                IServiceBusPersisterConnection serviceBusPersisterConnection = sp.GetRequiredService<IServiceBusPersisterConnection>();
                ILifetimeScope iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
                ILogger<EventBusServiceBus> logger = sp.GetRequiredService<ILogger<EventBusServiceBus>>();
                IEventBusSubscriptionsManager eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();
                string subscriptionName = Configuration["SubscriptionClientName"];

                return new EventBusServiceBus(serviceBusPersisterConnection, logger,
                    eventBusSubcriptionsManager, iLifetimeScope, subscriptionName);
            });
        }
        else
        {
            services.AddSingleton<IEventBus, EventBusRabbitMQ>(sp =>
            {
                string subscriptionClientName = Configuration["SubscriptionClientName"];
                IRabbitMQPersistentConnection rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
                ILifetimeScope iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
                ILogger<EventBusRabbitMQ> logger = sp.GetRequiredService<ILogger<EventBusRabbitMQ>>();
                IEventBusSubscriptionsManager eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

                int retryCount = 5;
                if (!string.IsNullOrEmpty(Configuration["EventBusRetryCount"]))
                {
                    retryCount = int.Parse(Configuration["EventBusRetryCount"]);
                }

                return new EventBusRabbitMQ(rabbitMQPersistentConnection, logger, iLifetimeScope, eventBusSubcriptionsManager, subscriptionClientName, retryCount);
            });
        }

        services.AddTransient<OrderStatusChangedToLoyaltyPointsProcessedIntegrationEventHandler>();
        services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
    }

    private void ConfigureEventBus(IApplicationBuilder app)
    {
        IEventBus eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
        eventBus.Subscribe<OrderStatusChangedToLoyaltyPointsProcessedIntegrationEvent, OrderStatusChangedToLoyaltyPointsProcessedIntegrationEventHandler>();
    }
}

public static class CustomExtensionMethods
{
    public static IServiceCollection AddCustomHealthCheck(this IServiceCollection services, IConfiguration configuration)
    {
        IHealthChecksBuilder hcBuilder = services.AddHealthChecks();

        hcBuilder.AddCheck("self", () => HealthCheckResult.Healthy());

        if (configuration.GetValue<bool>("AzureServiceBusEnabled"))
        {
            hcBuilder
                .AddAzureServiceBusTopic(
                    configuration["EventBusConnection"],
                    topicName: "eshop_event_bus",
                    name: "payment-servicebus-check",
                    tags: new string[] { "servicebus" });
        }
        else
        {
            hcBuilder
                .AddRabbitMQ(
                    $"amqp://{configuration["EventBusConnection"]}",
                    name: "payment-rabbitmqbus-check",
                    tags: new string[] { "rabbitmqbus" });
        }

        return services;
    }
}
