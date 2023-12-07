using Comments_database_manager;
using Comments_database_manager.Models;
using CommentsService.Database;
using Microsoft.EntityFrameworkCore;
using System;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        IConfiguration configuration = hostContext.Configuration;

        AppSettings.ConnectionString = configuration.GetConnectionString("Postgres_Db");

        var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
        optionsBuilder.UseNpgsql(AppSettings.ConnectionString);

        services.AddScoped<DataContext>(db => new DataContext(optionsBuilder.Options));
        services.AddHostedService<Worker>();
    })
    .Build();

using (var scope = host.Services.CreateScope())
{
    var service = scope.ServiceProvider;

    try
    {
        var context = service.GetRequiredService<DataContext>();
        context.Database.EnsureCreated();
    }
    catch (Exception)
    {

        throw;
    }
}
await host.RunAsync();
