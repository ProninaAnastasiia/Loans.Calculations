namespace Loans.Calculations.Kafka.Events;

public record ContractScheduleCalculatedEvent(Guid ContractId, Guid ScheduleId, Guid OperationId) : EventBase;