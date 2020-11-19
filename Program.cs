using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

// ReSharper disable HeapView.ObjectAllocation

// ReSharper disable HeapView.DelegateAllocation
// ReSharper disable HeapView.ObjectAllocation.Evident
// ReSharper disable HeapView.ClosureAllocation
// ReSharper disable HeapView.ObjectAllocation.Possible
// ReSharper disable HeapView.BoxingAllocation

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable PossibleMultipleEnumeration
// ReSharper disable MemberCanBeMadeStatic.Local
// ReSharper disable ConvertToConstant.Local

// ReSharper disable InvertIf
// ReSharper disable ArrangeThisQualifier
// ReSharper disable SuggestVarOrType_BuiltInTypes
// ReSharper disable UnusedMember.Global
// ReSharper disable RedundantAssignment
// ReSharper disable RedundantStringInterpolation
// ReSharper disable RedundantArgumentDefaultValue
// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable SuggestVarOrType_SimpleTypes
// ReSharper disable RedundantToStringCall

namespace GamerBot
{
    internal class Program
    {
        private static void Main()
        {
            new Bot().RunAsync().GetAwaiter().GetResult();
        }

        public class Bot
        {
            public DiscordClient Client { get; private set; }
            public InteractivityExtension Interactivity { get; private set; }
            public CommandsNextExtension Commands { get; private set; }

            public async Task RunAsync()
            {
                var json = string.Empty;

                await using (var fs = File.OpenRead("config.json"))
                using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                    json = await sr.ReadToEndAsync().ConfigureAwait(false);

                var cfgjson = JsonConvert.DeserializeObject<ConfigJson>(json);

                var config = new DiscordConfiguration
                {
                    Token = cfgjson.Token,
                    TokenType = TokenType.Bot,
                    AutoReconnect = true,
                    MinimumLogLevel = LogLevel.Debug,
                    Intents = DiscordIntents.AllUnprivileged | DiscordIntents.GuildMembers
                };
                this.Client = new DiscordClient(config);

                this.Client.GuildDownloadCompleted += Client_Ready;

                Client.UseInteractivity(new InteractivityConfiguration
                {
                    Timeout = TimeSpan.FromMinutes(10)
                });

                var commandsConfig = new CommandsNextConfiguration
                {
                    StringPrefixes = new[] { cfgjson.CommandPrefix },
                    EnableDms = true,
                    EnableMentionPrefix = true,
                    CaseSensitive = false,
                    DmHelp = false
                };

                Commands = Client.UseCommandsNext(commandsConfig);

                Commands.RegisterCommands<Youtube>();
                
                AppDomain.CurrentDomain.ProcessExit += async (s, ev) =>
                {
                    await Client.DisconnectAsync();
                };

                Console.CancelKeyPress += async (s, ev) =>
                {
                    await Client.DisconnectAsync();
                };
                
                // update status
                Timer timer = new Timer
                {
                    AutoReset = true,
                    Interval = 15 * 60000,
                    Enabled = true
                };
                timer.Elapsed += StatusUpdate;
                GC.KeepAlive(timer);
                timer.Start();

                await Client.ConnectAsync();

                await Task.Delay(-1);
            }

            private void StatusUpdate(object sender, ElapsedEventArgs e)
            {
                _ = new Task(async () =>
                {
                    await Client.UpdateStatusAsync(new DiscordActivity(
                            $"over {Client.Guilds.Values.First(x => x.Name.Contains("Coins")).MemberCount} members",
                            ActivityType.Watching),
                        UserStatus.Online);
                });
            }

            private async Task Client_Ready(DiscordClient sender, GuildDownloadCompletedEventArgs e)
            {
                await Client.UpdateStatusAsync(new DiscordActivity(
                        $"over {e.Guilds.Values.First(x => x.Name.Contains("Coins")).MemberCount} members",
                        ActivityType.Watching),
                    UserStatus.Online);
            }
        }

        public struct ConfigJson
        {
            [JsonProperty("token")]
            public string Token { get; private set; }

            [JsonProperty("prefix")]
            public string CommandPrefix { get; private set; }
        }
    }
}