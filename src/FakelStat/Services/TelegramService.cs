using Telegram.Bot;
using Telegram.Bot.Types;

namespace FakelStat.Services;

public class TelegramService
{
    private readonly ITelegramBotClient _client;

    public TelegramService(ITelegramBotClient client) => _client = client;

    public async ValueTask HandleUpdate(Update update)
    {

    }
}