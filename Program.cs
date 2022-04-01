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
        ulong guildID = ulong.Parse(Environment.GetEnvironmentVariable("guildID"));

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
            var guildCommand = new SlashCommandBuilder();
            guildCommand.WithName("roll");
            guildCommand.WithDescription("Roll the die");

            var globalCommand = new SlashCommandBuilder();
            globalCommand.WithName("first-command");
            globalCommand.WithDescription("This is my first global slash command");

            var dice20 = new SlashCommandBuilder();
            dice20.WithName("roll20");
            dice20.WithDescription("Roll a 20 sided die");
            

            try
            {
                await guild.CreateApplicationCommandAsync(guildCommand.Build());
                await guild.CreateApplicationCommandAsync(dice20.Build());
                // With global commands we don't need the guild.
                await _client.CreateGlobalApplicationCommandAsync(globalCommand.Build());
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
                case "roll":
                    int roll = rollDie(7);
                    await command.RespondAsync($"You have rolled a {roll}");
                    break;
                case "roll20":
                    int roll20 = rollDie(21);
                    await command.RespondAsync($"you have rolled a {roll20}");
                    break;
                default:
                    await command.RespondAsync($"You have run {command.Data.Name}");
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
