using Microsoft.AspNetCore;
using Price.Api;

var config = CreateConfiguration().Build();

var builder = WebHost
    .CreateDefaultBuilder(args)
    .ConfigureServices((ctx, services) =>
    {
        // This can go in program.cs
        // var cosmosDbSettings = new CosmosDbSettings();
        // _configuration.Bind("CosmosDb", cosmosDbSettings);
        
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    })
    .ConfigureAppConfiguration(app =>
    {
        app.AddConfiguration(config);
    })
    .UseStartup<Startup>();

// builder.Services.AddControllers();
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();
//
// var app = builder.Build();
//
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }
//
// app.UseHttpsRedirection();
// app.UseAuthorization();
// app.MapControllers();

await builder.Build().RunAsync();

static IConfigurationBuilder CreateConfiguration()
{
    return new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .AddEnvironmentVariables();
}