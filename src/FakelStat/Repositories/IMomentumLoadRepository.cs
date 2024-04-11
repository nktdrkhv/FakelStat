using FakelStat.Models;

namespace FakelStat.Repositories;

public interface IMomentumLoadRepository : IRepository<MomentumLoad>
{
    ValueTask<IEnumerable<MomentumLoad>> GetDaysAsync(DayOfWeek day, DateOnly start, DateOnly end);
}