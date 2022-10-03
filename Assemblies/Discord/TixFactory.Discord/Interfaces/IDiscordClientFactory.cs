using Discord.WebSocket;

namespace TixFactory.Discord
{
    public interface IDiscordClientFactory
    {
        DiscordSocketClient CreateBotClient(string botToken);
    }
}
