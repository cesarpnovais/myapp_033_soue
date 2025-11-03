using PayFlow.Api.Services;
using PayFlow.Api.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<FastPayService>();
builder.Services.AddHttpClient<SecurePayService>();
builder.Services.AddScoped<IPaymentProvider, FastPayService>();
builder.Services.AddScoped<IPaymentProvider, SecurePayService>();
builder.Services.AddScoped<PaymentService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
