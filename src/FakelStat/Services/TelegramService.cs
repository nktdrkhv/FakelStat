using FakelStat.Repositories;
using Telegram.Bot;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace FakelStat.Services;

public class TelegramService
{
    private readonly ITelegramBotClient _client;
    private readonly PlotService _plotService;
    private readonly IGeneratedPlotRepository _generatedPlotRepository;
    private readonly IMomentumLoadRepository _momentumLoadRepository;
    private readonly IWorkoutRepository _workoutRepository;

    public TelegramService(
        ITelegramBotClient client,
        PlotService plotService,
        IGeneratedPlotRepository generatedPlotRepository,
        IMomentumLoadRepository momentumLoadRepository,
        IWorkoutRepository workoutRepository)
    {
        _client = client;
        _plotService = plotService;
        _generatedPlotRepository = generatedPlotRepository;
        _momentumLoadRepository = momentumLoadRepository;
        _workoutRepository = workoutRepository;
    }

    public ValueTask HandleUpdate(Update update) => update switch
    {
        { Message.Text: { } text } when text!.StartsWith("/stat") => HandleStatCommand(update.Message),
        { CallbackQuery.Data: { } query } when query!.StartsWith("stat:") => HandleStatCallback(update.CallbackQuery),
        _ => ValueTask.CompletedTask
    };

    public async ValueTask HandleStatCommand(Message message)
    {
        var markup = new InlineKeyboardMarkup(new[]{
            new[] { InlineKeyboardButton.WithCallbackData("За все дни", "stat:average-day") },
            new[] { InlineKeyboardButton.WithCallbackData("За день недели", "stat:average-day") },
        });
        var sendRequest = new SendMessageRequest()
        {
            ChatId = message.Chat.Id,
            Text = "Выберите формат графика, на котором будет показана средняя посещаемость по часам:",
            ReplyMarkup = markup
        };
        await _client.SendMessageAsync(sendRequest);
    }

    public async ValueTask HandleStatCallback(CallbackQuery callbackQuery)
    {
        //stat:average-day:3
        //stat:week-day:2
        //
    }
}