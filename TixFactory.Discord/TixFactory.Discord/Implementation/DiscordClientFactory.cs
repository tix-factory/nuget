using Discord;
using Discord.WebSocket;
using System.Threading;
using System.Threading.Tasks;

namespace TixFactory.Discord
{
	/// <inheritdoc cref="IDiscordClientFactory.CreateBotClient"/>
	public class DiscordClientFactory : IDiscordClientFactory
	{
		/// <inheritdoc cref="IDiscordClientFactory.CreateBotClient"/>
		public DiscordSocketClient CreateBotClient(string botToken)
		{
			var client = new DiscordSocketClient();
			var waitLock = new SemaphoreSlim(0, 1);

			client.Ready += () =>
			{
				waitLock.Release();
				return Task.CompletedTask;
			};

			client.LoginAsync(TokenType.Bot, botToken).Wait();
			client.StartAsync().Wait();

			waitLock.Wait();

			return client;
		}
	}
}
