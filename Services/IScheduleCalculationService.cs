namespace Loans.Calculations.Services;

public interface IScheduleCalculationService
{
    Task<Guid> CalculateRepaymentAsync(Guid contractId, decimal loanAmount, int loanTermMonths, decimal interestRate,
        string paymentType, CancellationToken cancellationToken);
}