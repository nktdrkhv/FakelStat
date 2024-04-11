using FakelStat.Models;

namespace FakelStat.Repositories;

public interface IWorkoutRepository : IRepository<Workout>
{
    ValueTask<IEnumerable<Workout>> GetDayScheduleAsync(DateOnly day);
}