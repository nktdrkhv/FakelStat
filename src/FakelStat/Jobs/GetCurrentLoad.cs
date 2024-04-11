using FakelStat.Models;
using FakelStat.Repositories;
using FakelStat.Services;
using LinkDotNet.NCronJob;

namespace FakelStat.Jobs;

public class GetCurrentLoad(FakelService fakelService, IMomentumLoadRepository repository) : IJob
{
    public async Task RunAsync(JobExecutionContext context, CancellationToken ct)
    {
        var currentLoad = await fakelService.GetCurrentLoadAsync(ct);
        var moment = DateTime.Now;
        var momentumLoad = new MomentumLoad()
        {
            Amount = currentLoad,
            Day = DateOnly.FromDateTime(moment),
            Time = TimeOnly.FromDateTime(moment)
        };
        _ = await repository.InsertAsync(momentumLoad);
    }
}