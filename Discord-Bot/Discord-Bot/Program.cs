using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Discord_Bot
{
    class Program
    {
        private CommandService _commands;
        private DiscordSocketClient _client;
        private IServiceProvider _services;
        private RegistrationService _registrationServive;

        private const string Token = "NDk4ODU5NTIzNDYxNjExNTIy.Dp4sdA.3zdONPf19HE3_vvsD3bZAaSr38k";

        static void Main() => new Program().Start().GetAwaiter().GetResult();

        public async Task Start()
        {
            _client = new DiscordSocketClient();
            _commands = new CommandService();
            _services = new ServiceCollection().BuildServiceProvider();
            _registrationServive = new RegistrationService();

            await InstallCommands();

            await _client.LoginAsync(TokenType.Bot, Token);
            await _client.StartAsync();

            await Task.Delay(-1);
        }

        public async Task InstallCommands()
        {
            // Hook the MessageReceived Event into our Command Handler
            _client.MessageReceived += ClientOnMessageReceived;
            _client.UserJoined += ClientOnUserJoined;
            // Discover all of the commands in this assembly and load them.
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        private async Task ClientOnUserJoined(SocketGuildUser socketGuildUser)
        {
            if (_registrationServive.Users.Any(user => user.SocketGuildUser.Username == socketGuildUser.Username && user.IsRegistrationComplete))
            {
                //todo welcome back
            }
            else
            {
                var user = new User(socketGuildUser);

                await StartRegistration(user);
            }
        }

        private static async Task StartRegistration(User user)
        {
            await user.SocketGuildUser.SendMessageAsync(Messages.WelcomeMessage);

            await user.SocketGuildUser.SendMessageAsync(Messages.AskName);
        }

        public async Task ClientOnMessageReceived(SocketMessage socketMessage)
        {
            // Don't process the command if it was a System Message
            var message = socketMessage as SocketUserMessage;
            if (message == null)
            {
                return;
            }

            // Check if direct channel
            if (message.Channel is SocketDMChannel)
            {

                await message.Author.SendMessageAsync("Hi there");
            }
            else
            {
                // Don't process the command if it was outside the test channel
                if (socketMessage.Channel.ToString() != "test_lab_1")
                {
                    return;
                }
                // Create a number to track where the prefix ends and the command begins
                var argPos = 0;
                // Determine if the message is a command, based on if it starts with '!' or a mention prefix
                if (!(message.HasCharPrefix('!', ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos)))
                {
                    return;
                }
                // Create a Command Context
                var context = new CommandContext(_client, message);
                // Execute the command. (result does not indicate a return value, 
                // rather an object stating if the command executed successfully)
                var result = await _commands.ExecuteAsync(context, argPos, _services);
                if (!result.IsSuccess)
                {
                    await context.Channel.SendMessageAsync(result.ErrorReason);
                }
            }
        }
    }

    internal class RegistrationService
    {
        public enum RegistrationStep
        {
            NotStarted,
            Name,
            Surname,
            Sex,
            Done
        }

        public RegistrationService()
        {
            Users = new List<User>();
            //todo load users from json
        }

        public List<User> Users { get; set; }
    }

    internal class User
    {
        private RegistrationService.RegistrationStep _registrationStep;

        public User(SocketGuildUser socketGuildUser)
        {
            SocketGuildUser = socketGuildUser;
            _registrationStep = RegistrationService.RegistrationStep.NotStarted;
        }

        public SocketGuildUser SocketGuildUser { get; set; }
        public bool IsRegistrationComplete { get; set; }
    }

    internal class Messages
    {

        public static string WelcomeMessage = "Hallo!\n" +
                                              "Ich bin der UltimateLinz-Bot und werde dich durch die Anmeldung führen.\n" +
                                              "Bitte beantworte die folgenden Fragen wahrheitsgemäß.\n" +
                                              "Ein Admin wird sich deine Anmeldung dann ansehen und dich freischalten.";

        public static string AskName = "Wie lautet dein Vorname?";
        public static string AskSurName = "Wie lautet dein Nachname?";
        public static string AskSex = "Was is dein Geschlecht?";
    }

    public class Info : ModuleBase
    {
        // ~say hello -> hello
        [Command("say"), Summary("Echos a message.")]
        public async Task Say([Remainder, Summary("The text to echo")] string echo)
        {
            // ReplyAsync is a method on ModuleBase
            await ReplyAsync(echo);
        }

        [Command("clear"), Summary("Clears current channel.")]
        public async Task Clear()
        {
            var messages = await Context.Channel.GetMessagesAsync().Flatten();

            await Context.Channel.DeleteMessagesAsync(messages);
            const int delay = 5000;
            var m = await ReplyAsync($"Purge completed. _This message will be deleted in {delay / 1000} seconds._");
            await Task.Delay(delay);
            await m.DeleteAsync();
        }

        [Command("spam"), Summary("Spams the channel.")]
        public async Task Square([Summary("The number of messages to spam.")] int num)
        {
            // We can also access the channel from the Command Context.
            for (var i = 0; i < num; i++)
            {
                await Context.Channel.SendMessageAsync($"spam message #{i + 1}");
            }
        }
    }

    // Create a module with the 'sample' prefix
    [Group("sample")]
    public class Sample : ModuleBase
    {
        // ~sample square 20 -> 400
        [Command("square"), Summary("Squares a number.")]
        public async Task Square([Summary("The number to square.")] int num)
        {
            // We can also access the channel from the Command Context.
            await Context.Channel.SendMessageAsync($"{num}^2 = {Math.Pow(num, 2)}");
        }

        // ~sample userinfo --> foxbot#0282
        // ~sample userinfo @Khionu --> Khionu#8708
        // ~sample userinfo Khionu#8708 --> Khionu#8708
        // ~sample userinfo Khionu --> Khionu#8708
        // ~sample userinfo 96642168176807936 --> Khionu#8708
        // ~sample whois 96642168176807936 --> Khionu#8708
        [Command("userinfo"), Summary("Returns info about the current user, or the user parameter, if one passed.")]
        [Alias("user", "whois")]
        public async Task UserInfo([Summary("The (optional) user to get info for")] IUser user = null)
        {
            var userInfo = user ?? Context.Client.CurrentUser;
            await ReplyAsync($"{userInfo.Username}#{userInfo.Discriminator}");
        }
    }


}
