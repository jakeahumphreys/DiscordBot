using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace DiscordBot.Services;

public sealed class LoggingService
{
    public LoggingService(DiscordSocketClient discordClient, InteractionService interactionService)
    {
        discordClient.Log += LogAsync;
        interactionService.Log += LogAsync;
    }

    private Task LogAsync(LogMessage message)
    {
        return Console.Out.WriteLineAsync(message.Message);
    }
}