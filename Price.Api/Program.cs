using Microsoft.AspNetCore;
using Price.Api;

var config = CreateConfiguration().Build();

var builder = WebHost
    .CreateDefaultBuilder(args)
    .ConfigureServices((ctx, services) =>
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    })
    .ConfigureAppConfiguration(app =>
    {
        app.AddConfiguration(config);
    })
    .UseStartup<Startup>();

await builder.Build().RunAsync();
return;

static IConfigurationBuilder CreateConfiguration()
{
    return new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .AddEnvironmentVariables();
}