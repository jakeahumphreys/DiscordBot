using System.Reflection;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace DiscordBot.Handlers;

public sealed class InteractionHandler
{
    private readonly IServiceProvider _serviceProvider;
    private readonly DiscordSocketClient _discordClient;
    private readonly InteractionService _interactionService;
    private readonly IConfigurationRoot _configuration;

    public InteractionHandler(DiscordSocketClient discordClient, InteractionService interactionService, IServiceProvider serviceProvider, IConfigurationRoot configuration)
    {
        _discordClient = discordClient;
        _interactionService = interactionService;
        _serviceProvider = serviceProvider;
        _configuration = configuration;
    }

    public async Task InitialiseAsync()
    {
        _discordClient.Ready += ReadyAsync;
        
        await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), _serviceProvider);
        
        _discordClient.InteractionCreated += HandleInteraction;
    }
    
    private async Task ReadyAsync()
    {
        if (Program.IsDebug())
        {
            var testGuild = Convert.ToUInt64(_configuration["testGuild"]);
            await _interactionService.RegisterCommandsToGuildAsync(testGuild, true);
        }
        else
        {
            await _interactionService.RegisterCommandsGloballyAsync(true);
        }
    }
 
    private async Task HandleInteraction(SocketInteraction arg)
    {
        try
        {
            var context = new SocketInteractionContext(_discordClient, arg);
            var result = await _interactionService.ExecuteCommandAsync(context, _serviceProvider);
            
            if(!result.IsSuccess)
                switch (result.Error)
                {
                    case InteractionCommandError.UnmetPrecondition:
                        break;
                    default:
                        break;
                }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}