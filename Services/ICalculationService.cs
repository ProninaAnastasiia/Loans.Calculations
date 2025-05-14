namespace Loans.Calculations.Services;

public interface ICalculationService<TRequest, TResult>
{
    Task<TResult> CalculateAsync(TRequest request, CancellationToken cancellationToken);
}