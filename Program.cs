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
            
            await Task.Delay(-1); //needed to keep bot running on pc until you stop it manually
        }
        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
        public async Task Client_Ready()
        {
            //guild variable needed to create commands as guildcommands
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

            var rolld4Command = new SlashCommandBuilder();
            rolld4Command.WithName("roll-d4");
            rolld4Command.WithDescription("Roll a four sided die");

            var roll2d4Command = new SlashCommandBuilder();
            roll2d4Command.WithName("roll-2d4");
            roll2d4Command.WithDescription("Roll 2 four sided dice");
            
            try
            {
                //Generate the different commands
                await guild.CreateApplicationCommandAsync(rolld6Command.Build());
                await guild.CreateApplicationCommandAsync(rolld20Command.Build());
                await guild.CreateApplicationCommandAsync(roll2d6Command.Build());
                await guild.CreateApplicationCommandAsync(rolld4Command.Build());
            }
            catch (HttpException exception)
            {
                // If our command was invalid, we should catch an ApplicationCommandException. This exception contains the path of the error as well as the error message. You can serialize the Error field in the exception to get a visual of where your error is.
                var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);
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
                case "roll-d4":
                    int roll4 = rollDie(5);
                    await command.RespondAsync($"You have rolled a {roll4}");
                    break;
                case "roll-2d4":
                    int firstRoll4 = rollDie(5);
                    int secondRoll4 = rollDie(5);
                    await command.RespondAsync($"You have rolled a {firstRoll4} and a {secondRoll4}");
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
