using Discord.Commands;
using Discord.Interactions;
using DiscordBot.Handlers;

namespace DiscordBot.Modules;

[Name("TechSupport")]
[Discord.Commands.Summary("Not a very useful Tech Support Module")]
public sealed class TechSupportModule : InteractionModuleBase<SocketInteractionContext>
{
   public InteractionService InteractionService { get; set; }
   
   private readonly InteractionHandler _interactionHandler;
   private List<string> _possibleResponses;

   public TechSupportModule(InteractionHandler interactionHandler)
   {
      
      _interactionHandler = interactionHandler;
      
      _possibleResponses = new List<string>();
      LoadResponses();
      Console.WriteLine($"There are {_possibleResponses.Count} possible responses");
      
      Console.WriteLine("Tech Support Module Loaded");
   }

   [SlashCommand("support", "Does something cool")]
   public async Task SupportAsync() => await RespondAsync(GetSupportText());

   public void LoadResponses()
   {
      var responseFile = File.ReadAllLines("responses.txt");
      _possibleResponses = responseFile.ToList();
   }

   public string GetSupportText()
   {
      if (_possibleResponses.Count == 0)
         return "I've checked the manual and there's nothing in here for that.";
      
      var rnd = new Random();
      var randomNumber = rnd.Next(0, _possibleResponses.Count);

      return _possibleResponses[randomNumber];
   }
}