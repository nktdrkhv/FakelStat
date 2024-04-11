using FakelStat.Models;
using PetaPoco;

namespace FakelStat.Repositories;

public class SqlWorkoutRepository : SqlRepository<Workout>, IWorkoutRepository
{
    public SqlWorkoutRepository(IDatabase database) : base(database) { }

    public ValueTask<IEnumerable<Workout>> GetDayScheduleAsync(DateOnly day)
    {
        throw new NotImplementedException();
    }
}