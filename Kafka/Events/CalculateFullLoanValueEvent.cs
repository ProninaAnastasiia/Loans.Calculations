namespace Loans.Calculations.Kafka.Events;

public record CalculateFullLoanValueEvent(
    decimal LoanAmount,
    int LoanTermMonths,
    decimal InterestRate,
    string PaymentType,
    Guid OperationId) : EventBase;