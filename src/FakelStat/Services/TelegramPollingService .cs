using System.Collections.Concurrent;
using Telegram.Bot.Types;

namespace FakelStat.Services;

public class TelegramPollingService : BackgroundService
{
    private readonly ConcurrentDictionary<long, UserUpdates> _userUpdates = new();
    private readonly ILogger<TelegramPollingService> _logger;

    public TelegramPollingService(ILogger<TelegramPollingService> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }
            await Task.Delay(1000, stoppingToken);
        }
    }

    public record struct UserUpdates(Task Mission, Queue<Update> Updates, long LastActivity, CancellationTokenSource Cts);
}