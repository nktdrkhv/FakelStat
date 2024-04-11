using System.Security.Cryptography;
using FakelStat.Models;
using FakelStat.Repositories;
using FakelStat.Services;
using LinkDotNet.NCronJob;

namespace FakelStat.Jobs;

public class GetTodaySchedule(FakelService fakelService, IWorkoutRepository repository) : IJob
{
    public async Task RunAsync(JobExecutionContext context, CancellationToken ct)
    {
        var todayWorkouts = await fakelService.GetDayScheduleAsync(DateTime.Today, ct);
        foreach (var workout in todayWorkouts)
            _ = await repository.InsertAsync(workout);
    }
}