using Microsoft.EntityFrameworkCore;
using Refactoring.Application.Models.Settings;
using Refactoring.Application.Services.Payments;
using Refactoring.Domain.Interfaces;
using Refactoring.Domain.Interfaces.DB;
using Refactoring.Infrastructure;
using Refactoring.Infrastructure.DB;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("Email"));

builder.Services.AddHttpClient();
builder.Services.AddControllers();

builder.Services.AddDbContext<OrdersDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

builder.Services.AddScoped<IEmailClient, HttpEmailClient>();
builder.Services.AddScoped<IOutputWriter, ConsoleOutputWriter>();
builder.Services.AddScoped<IProductsRepository, ProductsRepository>();

builder.Services.AddTransient<CreditCardPaymentHandler>();
builder.Services.AddTransient<PayPalPaymentHandler>();
builder.Services.AddTransient<FallbackPaymentHandler>();

builder.Services.AddTransient<Func<IPaymentHandler>>(provider =>
{
    var creditCardPaymentHandler = provider.GetRequiredService<CreditCardPaymentHandler>();
    var paypalPaymentHandler = provider.GetRequiredService<PayPalPaymentHandler>();
    var fallbackPaymentHandler = provider.GetRequiredService<FallbackPaymentHandler>();

    creditCardPaymentHandler.SetNextHandler(paypalPaymentHandler);
    paypalPaymentHandler.SetNextHandler(fallbackPaymentHandler);

    return () => creditCardPaymentHandler;
});

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
