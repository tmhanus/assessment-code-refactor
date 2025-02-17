using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Refactoring.Application.Models.Settings;
using Refactoring.Application.Services.Payments;
using Refactoring.Domain.Interfaces;
using Refactoring.Domain.Interfaces.DB;
using Refactoring.Infrastructure;
using Refactoring.Infrastructure.DB;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("Email"));

builder.Services.AddHttpClient();
builder.Services.AddControllers();

builder.Services.AddDbContext<OrdersDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

builder.Services.AddScoped<IEmailClient, HttpEmailClient>();
builder.Services.AddScoped<IOutputWriter, ConsoleOutputWriter>();
builder.Services.AddScoped<IProductsRepository, ProductsRepository>();

foreach (var strategyType in Assembly.GetExecutingAssembly()
             .GetTypes()
             .Where(t => t.GetInterfaces().Contains(typeof(IPaymentStrategy))))
{
    builder.Services.AddScoped(typeof(IPaymentStrategy), strategyType);
}

builder.Services.AddScoped<IPaymentStrategyFactory, PaymentStrategyFactory>();
builder.Services.AddScoped<IPaymentProcessor, PaymentProcessor>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
