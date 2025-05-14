using Loans.Calculations.Data;
using Loans.Calculations.Data.Dto;
using Loans.Calculations.Data.Repositories;
using Loans.Calculations.Kafka;
using Loans.Calculations.Kafka.Consumers;
using Loans.Calculations.Kafka.Events;
using Loans.Calculations.Kafka.Handlers;
using Loans.Calculations.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Postgres");
builder.Services.AddDbContext<SchedulesDbContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddScoped<IScheduleRepository, ScheduleRepository>();

builder.Services.AddScoped<IScheduleCalculationService, ScheduleCalculationService>();
builder.Services.AddScoped<IEventHandler<CalculateContractValuesEvent>, CalculateContractValuesHandler>();
builder.Services.AddScoped<IEventHandler<CalculateRepaymentScheduleEvent>, CalculateScheduleRequestedHandler>();
builder.Services.AddScoped<IEventHandler<CalculateContractValuesEvent>, CalculateContractValuesHandler>();
builder.Services.AddScoped<IEventHandler<CalculateFullLoanValueEvent>, CalculateFullLoanValueHandler>();
builder.Services.AddScoped<ICalculationService<CalculateContractValuesEvent, decimal>, FullLoanValueCalculationService>();
builder.Services.AddScoped<ICalculationService<CalculateContractValuesEvent, ContractValuesCalculatedEvent>, LoanCalculationService>();

builder.Services.AddHostedService<CalculateContractValuesConsumer>();
builder.Services.AddHostedService<CalculateScheduleConsumer>();
builder.Services.AddHostedService<CalculateIndebtednessConsumer>();
builder.Services.AddSingleton<KafkaProducerService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

// Метрики HTTP
app.UseHttpMetrics();

// Экспонирование метрик на /metrics
app.MapMetrics();

app.MapPost("/api/calculate-all", async ([FromBody] CalculateContractValues request, CancellationToken cancellationToken, IEventHandler<CalculateContractValuesEvent> handler) =>
{
    var @event = new CalculateContractValuesEvent(request.ContractId, request.LoanAmount, request.LoanTermMonths, request.InterestRate, request.PaymentType, request.OperationId);
    await handler.HandleAsync(@event, cancellationToken);
});

app.Run();