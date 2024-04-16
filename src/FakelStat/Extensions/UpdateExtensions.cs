using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace FakelStat.Extensions;

public static class UpdateExtensions
{
    public static long GetId(this Update update) => update.Type switch
    {
        UpdateType.Message or
        UpdateType.EditedMessage => update.Message!.Chat!.Id,
        UpdateType.BusinessMessage or
        UpdateType.EditedBusinessMessage => update.Message!.From!.Id,
        UpdateType.ChannelPost or
        UpdateType.EditedChannelPost => update.Message!.SenderChat!.Id,
        UpdateType.BusinessConnection => update.BusinessConnection!.UserChatId,
        UpdateType.DeletedBusinessMessages => update.DeletedBusinessMessages!.Chat.Id,
        UpdateType.MessageReaction => update.MessageReaction!.Chat.Id,
        UpdateType.MessageReactionCount => update.MessageReactionCount!.Chat.Id,
        UpdateType.InlineQuery => update.InlineQuery!.From.Id,
        UpdateType.ChosenInlineResult => update.ChosenInlineResult!.From.Id,
        UpdateType.CallbackQuery => update.CallbackQuery!.From.Id,
        UpdateType.ShippingQuery => update.ShippingQuery!.From.Id,
        UpdateType.PreCheckoutQuery => update.PreCheckoutQuery!.From.Id,
        UpdateType.PollAnswer => update.PollAnswer!.VoterChat?.Id ?? update.PollAnswer!.User!.Id,
        UpdateType.MyChatMember => update.MyChatMember!.Chat.Id,
        UpdateType.ChatMember => update.ChatMember!.Chat.Id,
        UpdateType.ChatJoinRequest => update.ChatJoinRequest!.Chat.Id,
        UpdateType.ChatBoost => update.ChatBoost!.Chat.Id,
        UpdateType.RemovedChatBoost => update.RemovedChatBoost!.Chat.Id,
        _ => 0
    };
}