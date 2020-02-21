using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using SpeldesignBotCore.Contests;
using SpeldesignBotCore.Entities;
using SpeldesignBotCore.Loggers;
using SpeldesignBotCore.Storage;

namespace SpeldesignBotCore
{
    public class Connection
    {
        private readonly DiscordSocketClient _client;
        private readonly StatusLogger _statusLogger;
        private readonly DiscordMessageHandler _messageHandler;
        private readonly ContestHandler _reactionHandler;
        private readonly IDataStorage _storage;

        public Connection(DiscordSocketClient client, StatusLogger statusLogger, DiscordMessageHandler messageHandler, 
                          ContestHandler reactionHandler, IDataStorage storage)
        {
            _client = client;
            _statusLogger = statusLogger;
            _messageHandler = messageHandler;
            _reactionHandler = reactionHandler;
            _storage = storage;
        }

        internal async Task ConnectAsync(BotConfiguration config)
        {
            _client.Log += _statusLogger.LogToConsole;
            await _client.LoginAsync(TokenType.Bot, config.Token);
            await _client.StartAsync();

            _client.MessageReceived += _messageHandler.HandleMessageSentAsync;
            _client.MessageUpdated += _messageHandler.HandleMessageEditedAsync;
            _client.ReactionAdded += _reactionHandler.HandleReactionAdded;

            _client.Ready += ReportStartupTime;

            await _client.SetGameAsync($"\"{config.Prefix}help\"", type: ActivityType.Listening);

            await Task.Delay(-1);
        }

        private Task ReportStartupTime()
        {
            if (!_storage.HasObject("Info/BotInformation"))
            {
                var newBotInformation = new BotInformation(startupTime: System.DateTime.Now);
                _storage.StoreObject(newBotInformation, "Info/BotInformation");
                return Task.CompletedTask;
            }

            var botInformation = _storage.RestoreObject<BotInformation>("Info/BotInformation");
            botInformation.TimeOfPreviousStartup = System.DateTime.Now;
            _storage.StoreObject(botInformation, "Info/BotInformation");

            return Task.CompletedTask;
        }
    }
}
