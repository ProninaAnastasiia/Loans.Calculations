namespace Loans.Calculations.Kafka.Events;

public record FullLoanValueCalculatedEvent(decimal FullLoanValue, Guid OperationId) : EventBase;