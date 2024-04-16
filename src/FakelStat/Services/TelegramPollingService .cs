using System.Collections.Concurrent;
using System.Timers;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using FakelStat.Extensions;

namespace FakelStat.Services;

public class TelegramPollingService : BackgroundService
{
    private readonly ILogger<TelegramPollingService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ITelegramBotClient _client;

    private readonly QueuedUpdateReceiver _updateReciever;
    private readonly ConcurrentDictionary<long, UserUpdates> _userUpdates = new();
    private readonly System.Timers.Timer _cleanerTimer;

    public TelegramPollingService(
        IServiceScopeFactory scopeFactory,
        ILogger<TelegramPollingService> logger,
        ITelegramBotClient client)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _client = client;

        var recieverOptions = new ReceiverOptions
        {
            AllowedUpdates = { }
        };
        _updateReciever = new QueuedUpdateReceiver(_client, recieverOptions);

        _cleanerTimer = new(TimeSpan.FromMinutes(10))
        {
            AutoReset = true,
            Enabled = true
        };
        _cleanerTimer.Elapsed += OnCleanCache;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await foreach (var update in _updateReciever.WithCancellation(stoppingToken))
                {
                    if (_userUpdates.TryGetValue(update.GetId(), out var user))
                    {
                        // добавить 
                        lock (user.Updates)
                        {
                            //if (user.Mission != null)
                        }
                    }
                    else
                    {
                        // создать и запустить
                    }

                }
            }
            catch { }
        }

        _cleanerTimer.Stop();
        _cleanerTimer.Dispose();
    }

    protected void OnCleanCache(object? sender, ElapsedEventArgs args)
    {
        var moment = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        foreach (var user in _userUpdates)
            if (moment - user.Value.LastActivity > 600)
            {
                user.Value.Cts.Cancel();
                _userUpdates.TryRemove(user);
            }
    }

    public record struct UserUpdates(
        Task? Mission,
        Queue<Update> Updates,
        long LastActivity,
        CancellationTokenSource Cts);
}