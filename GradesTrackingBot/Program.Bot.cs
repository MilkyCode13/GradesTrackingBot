using GradesTrackingBot.Model;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace GradesTrackingBot;

internal static partial class Program
{
    private static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Type != UpdateType.Message)
        {
            return;
        }

        if (update.Message!.Type != MessageType.Text)
        {
            return;
        }

        long chatId = update.Message.Chat.Id;
        long userId = update.Message.From?.Id ?? 0;
        if (userId == 0)
        {
            return;
        }
        string messageText = update.Message.Text ?? string.Empty;

        var newDiscipline = new Discipline(messageText, userId);

        if (s_database == null)
        {
            return;
        }
        
        await s_database.AddDiscipline(newDiscipline);

        List<Discipline> disciplines = await s_database.GetDisciplines(userId);
        
        Console.WriteLine($"{userId} {messageText}");
        string reply = string.Join('\n', disciplines.Select(discipline => discipline.Name));
        await botClient.SendTextMessageAsync(chatId, reply, cancellationToken: cancellationToken);
    }

    private static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        string errorMessage = exception switch
        {
            ApiRequestException apiRequestException =>
                $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(errorMessage);
        return Task.CompletedTask;
    }
}