using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace GradesTrackingBot;

internal static partial class Program
{
    private static async Task Main(string[] args)
    {
        using IHost host = Host.CreateDefaultBuilder(args).Build();

        var config = host.Services.GetRequiredService<IConfiguration>();

        IConfigurationSection databaseConfig = config.GetSection("Database");
        ConfigureDatabase(databaseConfig);  

        var token = config.GetValue<string>("Telegram:BotToken");

        TelegramBotClient botClient;
        try
        {
            botClient = new TelegramBotClient(token);
        }
        catch (ArgumentException)
        {
            await Console.Error.WriteLineAsync("Invalid token.");
            return;
        }

        using var cts = new CancellationTokenSource();

        botClient.StartReceiving(HandleUpdateAsync, HandleErrorAsync, cancellationToken: cts.Token);

        User me = await botClient.GetMeAsync(cts.Token);
        Console.WriteLine($"Connected as @{me.Username}, Id {me.Id}.");
        Console.WriteLine("Bot is running, Ctrl+C to exit.");
        Console.ReadLine();
        cts.Cancel();

        await host.RunAsync(cts.Token);
    }
}