using Autofac.Extensions.DependencyInjection;
using Coupon.API.Extensions;
using Coupon.API.Infrastructure;
using Coupon.API.Infrastructure.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using System.IO;

namespace Coupon.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args)
                .Build()
                .SeedDatabaseStrategy<MongoDbContext>(context => new CouponSeed().SeedAsync(context).Wait())
                .SubscribersIntegrationEvents()
                .Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureAppConfiguration((host, builder) =>
                {
                    builder.SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{host.HostingEnvironment.EnvironmentName}.json", optional: false, reloadOnChange: true)
                        .AddEnvironmentVariables();

                    IConfigurationRoot config = builder.Build();

                    if (config.GetValue("UseVault", false))
                    {
                        builder.AddAzureKeyVault($"https://{config["Vault:Name"]}.vault.azure.net/", config["Vault:ClientId"], config["Vault:ClientSecret"]);
                    }
                })
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>())
                .UseSerilog((host, builder) =>
                {
                    builder.MinimumLevel.Verbose()
                        .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                        .Enrich.WithProperty("ApplicationContext", host.HostingEnvironment.ApplicationName)
                        .Enrich.FromLogContext()
                        .WriteTo.Console()
                        .WriteTo.Seq(string.IsNullOrWhiteSpace(host.Configuration["Serilog:SeqServerUrl"]) ? "http://seq" : host.Configuration["Serilog:SeqServerUrl"])
                        .WriteTo.Http(string.IsNullOrWhiteSpace(host.Configuration["Serilog:LogstashUrl"]) ? "http://logstash:8080" : host.Configuration["Serilog:LogstashUrl"])
                        .ReadFrom.Configuration(host.Configuration);
                });
        }
    }
}
