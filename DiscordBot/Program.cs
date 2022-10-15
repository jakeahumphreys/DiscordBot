using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Handlers;
using DiscordBot.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot;

public class Program
{
    private readonly IConfigurationRoot _configuration;
    private readonly IServiceProvider _services;

    private readonly DiscordSocketConfig _socketConfig = new()
    {
        MessageCacheSize = 1000
    };

    public Program()
    {
        _configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", false, true)
            .Build();

        _services = new ServiceCollection()
            .AddSingleton(_configuration)
            .AddSingleton(_socketConfig)
            .AddSingleton(x => new DiscordSocketClient(x.GetRequiredService<DiscordSocketConfig>()))
            .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
            .AddSingleton<InteractionHandler>()
            .AddSingleton<LoggingService>()
            .BuildServiceProvider();
    }

    static void Main(string[] args) => new Program().RunAsync().GetAwaiter().GetResult();

    private async Task RunAsync()
    {
        var client = _services.GetRequiredService<DiscordSocketClient>();

        await _services.GetRequiredService<InteractionHandler>().InitialiseAsync();
        _services.GetRequiredService<LoggingService>();

        await client.LoginAsync(TokenType.Bot, _configuration["token"]);
        await client.StartAsync();

        await client.SetGameAsync("Pondering the universe");
        
        await Task.Delay(-1);
    }
    
    public static bool IsDebug()
    {
        #if DEBUG
                return true;
        #endif
    }
}
