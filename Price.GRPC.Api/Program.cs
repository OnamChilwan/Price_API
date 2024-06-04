// See https://aka.ms/new-console-template for more information

using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Price.GRPC.Api;
using Price.GRPC.Api.Configuration;
using Serilog;

var configurationRoot = CreateConfiguration().Build();
var telemetryConfiguration = new TelemetryConfiguration();
telemetryConfiguration.ConnectionString = configurationRoot["ApplicationInsights:ConnectionString"];

var builder = WebHost
    .CreateDefaultBuilder(args)
    .ConfigureKestrel(options =>
    {
        options.ListenLocalhost(7001, o => o.Protocols = HttpProtocols.Http2);
    })
    .ConfigureAppConfiguration(app =>
    {
        app.AddConfiguration(configurationRoot);
    })
    .ConfigureServices((context, services) =>
    {
        var apiSettings = new ApiSettings();
        context.Configuration.Bind(nameof(ApiSettings), apiSettings);
        
        services.AddSingleton(apiSettings);
        services.AddApplicationInsightsTelemetry();
        services.AddLogging(b => b.AddApplicationInsights());
        services.AddLogging(builder => builder.AddSerilog());
    })
    .UseStartup<Startup>();

await builder
    .Build()
    .RunAsync();

static IConfigurationBuilder CreateConfiguration()
{
    return new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: false)
        .AddEnvironmentVariables();
}