using Loans.Calculations.Kafka.Events;
using Loans.Calculations.Services;
using Newtonsoft.Json;

namespace Loans.Calculations.Kafka.Handlers;

public class CalculateContractValuesHandler : IEventHandler<CalculateContractValuesEvent>
{
    private readonly ILogger<CalculateContractValuesHandler> _logger;
    private readonly IScheduleCalculationService _calculationSchedule;
    private readonly ICalculationService<CalculateContractValuesEvent, ContractValuesCalculatedEvent> _calculationService;
    

    private readonly IConfiguration _config;
    private KafkaProducerService _producer;

    public CalculateContractValuesHandler(
        ICalculationService<CalculateContractValuesEvent, ContractValuesCalculatedEvent> calculationService, IScheduleCalculationService calculationSchedule,
        ILogger<CalculateContractValuesHandler> logger, IConfiguration config, KafkaProducerService producer)
    {
        _logger = logger;
        _calculationSchedule = calculationSchedule;
        _calculationService = calculationService;
        _config = config;
        _producer = producer;
    }

    public async Task HandleAsync(CalculateContractValuesEvent calculationEvent, CancellationToken cancellationToken)
    {
        try
        {
            var otherCalcs = await _calculationService.CalculateAsync(calculationEvent, cancellationToken);
            var scheduleId = await _calculationSchedule.CalculateRepaymentAsync(calculationEvent.ContractId, calculationEvent.LoanAmount,
                calculationEvent.LoanTermMonths, calculationEvent.InterestRate, calculationEvent.PaymentType, cancellationToken);

            var @event = new AllValuesCalculatedEvent(otherCalcs.ContractId, scheduleId, otherCalcs.MonthlyPaymentAmount, 
                otherCalcs.TotalPaymentAmount, otherCalcs.TotalInterestPaid, otherCalcs.FullLoanValue, otherCalcs.OperationId);
            var jsonMessage = JsonConvert.SerializeObject(@event);
            var topic = _config["Kafka:Topics:CalculateAllContractValues"];

            await _producer.PublishAsync(topic, jsonMessage);
        }
        catch (Exception e)
        {
            _logger.LogError(
                "Failed to handle CalculateContractValuesEvent. ContractId: {ContractId} OperationId: {OperationId}. Exception: {e}", calculationEvent.ContractId, calculationEvent.OperationId, e.Message);
        }
    }
}