using Loans.Calculations.Data.Models;

namespace Loans.Calculations.Data.Repositories;

public interface IScheduleRepository
{
    Task<ScheduleEntity?> GetByIdAsync(Guid scheduleId);
    Task SaveAsync(ScheduleEntity scheduleId);
    Task UpdateAsync(ScheduleEntity scheduleId);
}