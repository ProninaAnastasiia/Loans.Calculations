namespace Loans.Calculations.Kafka.Events;

public record AllValuesCalculatedEvent(
    Guid ContractId, Guid ScheduleId, decimal MonthlyPaymentAmount, decimal TotalPaymentAmount, 
    decimal TotalInterestPaid, decimal FullLoanValue, Guid OperationId) : EventBase;