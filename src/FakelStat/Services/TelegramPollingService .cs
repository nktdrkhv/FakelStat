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
                    UserUpdates user;
                    long id = update.GetId();
                    if (!_userUpdates.TryGetValue(id, out user!))
                        _userUpdates[id] = user = new UserUpdates()
                        {
                            Cts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken)
                        };
                    lock (user)
                    {
                        user.LastActivity = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                        if (user.Mission == null)
                            user.Mission = Run(user, update, user.Cts.Token);
                        else
                            user.Updates.Enqueue(update);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during polling");
            }
        }
        _cleanerTimer.Stop();
        _cleanerTimer.Dispose();
    }

    public Task Run(UserUpdates user, Update update, CancellationToken stoppingToken) =>
        Task.Run(() => ProcessUpdateAsync(update), stoppingToken).ContinueWith(_ =>
            {
                Update? nextUpdate;
                lock (user)
                    if (user.Updates.Count == 0)
                    {
                        user.Mission = null;
                        return;
                    }
                    else
                    {
                        nextUpdate = user.Updates.Dequeue();
                        user.Mission = Run(user, update, user.Cts.Token);
                    }
            }, stoppingToken);

    public async Task ProcessUpdateAsync(Update update)
    {
        using var scope = _scopeFactory.CreateScope();
        var telegramService = scope.ServiceProvider.GetRequiredService<TelegramService>();
        await telegramService.HandleUpdate(update);
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

    public class UserUpdates
    {
        public Task? Mission { get; set; }
        public Queue<Update> Updates { get; set; } = new();
        public long LastActivity { get; set; }
        public CancellationTokenSource Cts { get; set; } = null!;
    }
}