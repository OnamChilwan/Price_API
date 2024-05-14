// See https://aka.ms/new-console-template for more information

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Price.GRPC.Api;

var config = CreateConfiguration().Build();

var builder = WebHost
    .CreateDefaultBuilder(args)
    .ConfigureKestrel(options =>
    {
        options.ListenLocalhost(
            7001, 
            o => o.Protocols = HttpProtocols.Http2);
    })
    .ConfigureAppConfiguration(app =>
    {
        app.AddConfiguration(config);
    })
    .UseStartup<Startup>();

await builder
    .Build()
    .RunAsync();

static IConfigurationBuilder CreateConfiguration()
{
    return new ConfigurationBuilder()
        //.AddJsonFile("appsettings.json")
        .AddEnvironmentVariables();
}