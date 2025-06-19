using Microsoft.Extensions.Caching.Memory;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace webb_tst_site3.Services
{
    public class TelegramBotService : IHostedService
    {
        private readonly ITelegramBotClient _botClient;
        private readonly IConfiguration _config;
        private readonly IMemoryCache _cache;

        public TelegramBotService(IConfiguration config, IMemoryCache cache)
        {
            _config = config;
            _cache = cache;
            _botClient = new TelegramBotClient(_config["TelegramBot:Token"]);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _botClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                errorHandler: HandleErrorAsync,
                receiverOptions: new ReceiverOptions { AllowedUpdates = Array.Empty<UpdateType>() },
                cancellationToken: cancellationToken);

            return Task.CompletedTask;
        }

        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            if (update.Message is not { Text: { } text } message)
                return;

            if (text.StartsWith("/start auth_"))
            {
                var telegramId = message.From.Id;
                var token = Guid.NewGuid().ToString();
                _cache.Set($"telegram_auth_{token}", telegramId, TimeSpan.FromMinutes(5));

                var authUrl = $"{_config["SiteUrl"]}/auth/telegram-callback?token={token}";
                var keyboard = new InlineKeyboardMarkup(InlineKeyboardButton.WithUrl("Войти на сайт", authUrl));

                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Нажмите кнопку для автоматического входа:",
                    replyMarkup: keyboard,
                    cancellationToken: ct);
            }
        }

        private Task HandleErrorAsync(ITelegramBotClient botClient, Exception error, CancellationToken ct)
        {
            Console.WriteLine($"Telegram Bot Error: {error.Message}");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}