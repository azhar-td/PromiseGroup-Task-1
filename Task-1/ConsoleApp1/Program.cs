using Application.Benchmarking;
using Application.Printing;
using Domain.Abstractions;
using Infrastructure.Common;
using Infrastructure.EntityFramework;
using Infrastructure.Sql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var connectionString = configuration.GetConnectionString("DefaultConnection")
         ?? throw new InvalidOperationException("Connection string not found.");


var services = new ServiceCollection();

services.AddDbContext<TaskOneDbContext>(opt => opt.UseSqlServer(connectionString));
services.AddScoped<ICategoryTreeBuilder, EfCategoryTreeBuilder>();
services.AddScoped<SpCategoryTreeBuilder>(_ => new SpCategoryTreeBuilder(connectionString));
services.AddSingleton<ITreePrinter, ConsoleTreePrinter>();


var sp = services.BuildServiceProvider();
using var scope = sp.CreateScope();

var db = scope.ServiceProvider.GetRequiredService<TaskOneDbContext>();
await DbBootstrapper.EnsureCreatedAndSeededAsync(db, connectionString);

var printer = scope.ServiceProvider.GetRequiredService<ITreePrinter>();
var efBuilder = scope.ServiceProvider.GetRequiredService<ICategoryTreeBuilder>();
var spBuilder = scope.ServiceProvider.GetRequiredService<SpCategoryTreeBuilder>();

Console.WriteLine("=== EF + LINQ ===");
var efResult = await BenchmarkRunner.RunOnceAsync(efBuilder, printer);

Console.WriteLine();
Console.WriteLine("=== Stored Procedure ===");
var spResult = await BenchmarkRunner.RunOnceAsync(spBuilder, printer);

Console.WriteLine("\n=== Summary ===");
Console.WriteLine($"EF+LINQ time:     {efResult.elapsed}");
Console.WriteLine($"StoredProc time:  {spResult.elapsed}");

if (efResult.elapsed > spResult.elapsed)
{
    Console.WriteLine($"Difference:        {(efResult.elapsed - spResult.elapsed)} faster by StoredProc");
}
else if (spResult.elapsed > efResult.elapsed)
{
    Console.WriteLine($"Difference:        {(spResult.elapsed - efResult.elapsed)} faster by EF+LINQ");
}
else
{
    Console.WriteLine("Both approaches performed equally.");
}

Console.WriteLine("\nDone.");