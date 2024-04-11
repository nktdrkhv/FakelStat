using FakelStat.Models;
using PetaPoco;

namespace FakelStat.Repositories;

public class SqlMomentumLoadRepository : SqlRepository<MomentumLoad>, IMomentumLoadRepository
{
    public SqlMomentumLoadRepository(IDatabase database) : base(database) { }

    public ValueTask<IEnumerable<MomentumLoad>> GetDaysAsync(DayOfWeek day, DateOnly start, DateOnly end)
    {
        var result = new List<MomentumLoad>();
        foreach (var momentumLoad in Database.Fetch<MomentumLoad>(
            $"SELECT * FROM {nameof(MomentumLoad)} WHERE Day >= @0 AND Day <= @1",
            start.ToString("O"),
            end.ToString("O")))
            if (momentumLoad.Day.DayOfWeek == day)
                result.Add(momentumLoad);
        return ValueTask.FromResult<IEnumerable<MomentumLoad>>(result);
    }
}