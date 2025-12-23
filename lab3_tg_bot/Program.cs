using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Polling;

Dictionary<long, string> userNames = new Dictionary<long, string>();

var botClient = new TelegramBotClient("8538696177:AAG5eQrj5wVIHRSptCir2RV9I2fUgXmf4bM");
var cts = new CancellationTokenSource();

ReceiverOptions receiverOptions = new ReceiverOptions
{
    AllowedUpdates = Array.Empty<UpdateType>()
};

var updateHandler = new DefaultUpdateHandler(HandleUpdateAsync, HandlePollingErrorAsync);

botClient.StartReceiving(
    updateHandler: updateHandler,
    receiverOptions: receiverOptions,
    cancellationToken: cts.Token
);

var me = await botClient.GetMe();
Console.WriteLine($"Бот @{me.Username} запущен");
Console.ReadLine();

async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    if (update.Message is not { } message || message.Text is null)
        return;

    var chatId = message.Chat.Id;
    var messageText = message.Text;

    Console.WriteLine($"В чат {chatId} получено: '{messageText}'");

    if (messageText == "/start" || messageText == "В главное меню")
    {
        if (!userNames.ContainsKey(chatId))
        {
            await botClient.SendMessage(
                chatId: chatId,
                text: "Привет! Я бот-коуч по становлению топовым блогером в нише котов и еды! Как мне к тебе обращаться?",
                cancellationToken: cancellationToken);
            return;
        }

        var mainMenuKeyboard = new ReplyKeyboardMarkup(new[]
        {
            new KeyboardButton[] { "📚 Основы мастерства" },
            new KeyboardButton[] { "😻 Съемка котов", "🍕 Съемка еды" },
            new KeyboardButton[] { "⚙️ Инструменты", "💡 Генератор идей" },
            new KeyboardButton[] { "🧪 Тестовые задания" }
        })
        {
            ResizeKeyboard = true
        };

        await botClient.SendMessage(
            chatId: chatId,
            text: $"Выбери раздел, {userNames[chatId]}!",
            replyMarkup: mainMenuKeyboard,
            cancellationToken: cancellationToken);
        return;
    }

    if (!userNames.ContainsKey(chatId))
    {
        string name = messageText.Trim();
        if (string.IsNullOrWhiteSpace(name) || name.Length > 50)
        {
            await botClient.SendMessage(chatId, "Имя не может быть пустым или таким длинным. Попробуй еще раз!", cancellationToken: cancellationToken);
            return;
        }
        userNames[chatId] = name;
        await botClient.SendMessage(chatId, $"Отлично, {name}! Теперь я буду тебя так звать. Давай начнем обучение!", cancellationToken: cancellationToken);

        await botClient.SendMessage(
            chatId: chatId,
            text: $"Выбери раздел, {userNames[chatId]}!",
            replyMarkup: new ReplyKeyboardMarkup(new[]
            {
                new KeyboardButton[] { "📚 Основы мастерства" },
                new KeyboardButton[] { "😻 Съемка котов", "🍕 Съемка еды" },
                new KeyboardButton[] { "⚙️ Инструменты", "💡 Генератор идей" },
                new KeyboardButton[] { "🧪 Тестовые задания" }
            })
            {
                ResizeKeyboard = true
            },
            cancellationToken: cancellationToken);
        return;
    }

    switch (messageText)
    {
        case "📚 Основы мастерства":
            var basicsText = $"<b>Основы мастерства для {userNames[chatId]}:</b>\n\n" +
                             "1. <u>Найди свою суперсилу</u>: Ты снимаешь Котов или Еду? А может, их тайную дружбу?\n" +
                             "2. <u>Контент-план</u>: Понедельник — Кот в тарелке с пастой. Четверг — Еда в форме кота.\n" +
                             "3. <u>Постоянство</u>: Выкладывай stories каждый день. Даже если кот спит, а еда остыла.";
            await botClient.SendMessage(chatId, basicsText, parseMode: ParseMode.Html, cancellationToken: cancellationToken);
            break;

        case "😻 Съемка котов":
            var catTips = $"<b>Секреты съемки котов, {userNames[chatId]}:</b>\n\n" +
                          "• Используй <i>игрушки-удочки</i>, чтобы привлечь внимание.\n" +
                          "• Лови момент 'мяу' — это хит ленты!\n" +
                          "• Снимай с уровня глаз кота для эффекта погружения.";
            await botClient.SendMessage(chatId, catTips, parseMode: ParseMode.Html, cancellationToken: cancellationToken);
            break;

        case "🍕 Съемка еды":
            var foodTips = $"<b>Как снять еду аппетитно:</b>\n\n" +
                           "• <u>Естественный свет</u> — твой лучший друг.\n" +
                           "• Добавь движение: капающий соус или поднимающийся пар.\n" +
                           "• Цветовой акцент: зеленый базилик на красном томатном супе.";
            await botClient.SendMessage(chatId, foodTips, parseMode: ParseMode.Html, cancellationToken: cancellationToken);
            break;

        case "⚙️ Инструменты":
            var toolsText = "Рекомендуемый набор блогера:\n\n" +
                            "• <b>Смартфон</b> с хорошей камерой (iPhone/Google Pixel).\n" +
                            "• Кольцевой свет для идеального освещения.\n" +
                            "• Штатив для ровных кадров и hands-free съемки.\n" +
                            "• Приложения: VSCO, Lightroom, InShot.";
            await botClient.SendMessage(chatId, toolsText, parseMode: ParseMode.Html, cancellationToken: cancellationToken);
            break;

        case "💡 Генератор идей":
            string[] ideas = {
                "Сними, как кот пробует огурец.",
                "Приготовь печенье в форме кошачьей лапки.",
                "Сделай таймлапс поедания котом завтрака.",
                "Сравни реакцию кота на разные виды сыра."
            };
            Random rnd = new Random();
            string randomIdea = ideas[rnd.Next(ideas.Length)];
            await botClient.SendMessage(chatId, $"<b>Идея для тебя:</b>\n{randomIdea}", parseMode: ParseMode.Html, cancellationToken: cancellationToken);
            break;

        case "🧪 Тестовые задания":
            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                InlineKeyboardButton.WithCallbackData(text: "✅ Я выполнил задание!", callbackData: "task_done")
            });
            await botClient.SendMessage(
                chatId: chatId,
                text: $"<b>Задание дня для {userNames[chatId]}:</b>\nСними короткое видео (15 сек), где кот пытается украсть кусочек еды с тарелки. Выложи в Reels/TikTok с хештегом #котоедоблогер.",
                parseMode: ParseMode.Html,
                replyMarkup: inlineKeyboard,
                cancellationToken: cancellationToken);
            break;

        case "Привет":
            await botClient.SendMessage(chatId, $"Здравствуй, {userNames[chatId]}!", cancellationToken: cancellationToken);
            break;
        case "Картинка":
            await using (var stream = System.IO.File.OpenRead("cat_food.jpg"))
            {
                await botClient.SendPhoto(chatId, new InputFileStream(stream), caption: "Вот идеальный кадр для вдохновения!", cancellationToken: cancellationToken);
            }
            break;
        case "Кнопки":
            var quickKeyboard = new ReplyKeyboardMarkup(new[]
            {
                new KeyboardButton[] { "Лайк", "Дизлайк" },
                new KeyboardButton[] { "Комментарий" }
            })
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = true
            };
            await botClient.SendMessage(chatId, "Оцени мой совет:", replyMarkup: quickKeyboard, cancellationToken: cancellationToken);
            break;
        default:
            await botClient.SendMessage(chatId, "Пока я понимаю только команды из меню 😿 Нажми /start, чтобы вернуться.", cancellationToken: cancellationToken);
            break;
    }
}

Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    Console.WriteLine($"Ошибка при работе бота: {exception.Message}");
    return Task.CompletedTask;
}