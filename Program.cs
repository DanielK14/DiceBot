using System;
using System.Threading.Tasks;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace DiceBot
{
    class Program
    {
        private DiscordSocketClient _client;
        readonly ulong guildID = ulong.Parse(Environment.GetEnvironmentVariable("guildID"));

        public static void Main(string[] args)
        {
            new Program().MainAsync().GetAwaiter().GetResult();
            
        }
        public async Task MainAsync()
        {
            _client = new DiscordSocketClient();
            _client.Log += Log;
            await _client.LoginAsync(TokenType.Bot,
                Environment.GetEnvironmentVariable("DiscordToken"));
            await _client.StartAsync();
            _client.Ready += Client_Ready;
            _client.SlashCommandExecuted += SlashCommandHandler;

            await Task.Delay(-1);
        }
        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
        public async Task Client_Ready()
        {
            var guild = _client.GetGuild(guildID);
            var rolld6Command = new SlashCommandBuilder();
            rolld6Command.WithName("roll-d6");
            rolld6Command.WithDescription("Roll the die");

            var rolld20Command = new SlashCommandBuilder();
            rolld20Command.WithName("roll-d20");
            rolld20Command.WithDescription("Roll a 20 sided die");

            var roll2d6Command = new SlashCommandBuilder();
            roll2d6Command.WithName("roll-2d6");
            roll2d6Command.WithDescription("Roll two six sided dice");
            
            try
            {
                await guild.CreateApplicationCommandAsync(rolld6Command.Build());
                await guild.CreateApplicationCommandAsync(rolld20Command.Build());
                await guild.CreateApplicationCommandAsync(roll2d6Command.Build());
                
                // Using the ready event is a simple implementation for the sake of the example. Suitable for testing and development.
                // For a production bot, it is recommended to only run the CreateGlobalApplicationCommandAsync() once for each command.
            }
            catch (HttpException exception)
            {
                // If our command was invalid, we should catch an ApplicationCommandException. This exception contains the path of the error as well as the error message. You can serialize the Error field in the exception to get a visual of where your error is.
                var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);

                // You can send this error somewhere or just print it to the console, for this example we're just going to print it.
                Console.WriteLine(json);
            }
        }
        private async Task SlashCommandHandler(SocketSlashCommand command)
        {
            switch (command.Data.Name)
            {
                case "roll-d6":
                    int roll = rollDie(7);
                    await command.RespondAsync($"You have rolled a {roll}");
                    break;
                case "roll-d20":
                    int roll20 = rollDie(21);
                    await command.RespondAsync($"you have rolled a {roll20}");
                    break;
                case "roll-2d6":
                    int firstDie = rollDie(7);
                    int secondDie = rollDie(7);
                    await command.RespondAsync($"you have rolled a {firstDie} and a {secondDie}");
                    break;
                default:
                    await command.RespondAsync($"You have run {command.Data.Name}, which does nothing.");
                    break;
            }
            
        }
        public int rollDie(int top)
        {
            Random rnd = new Random();
            int roll = rnd.Next(1, top);
            return roll;
        }
    }
}
