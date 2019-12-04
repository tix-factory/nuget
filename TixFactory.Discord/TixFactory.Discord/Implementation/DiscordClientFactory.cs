using Discord;
using Discord.WebSocket;
using System.Collections.Generic;
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
			var readyLock = new SemaphoreSlim(0, 1);
			var guildLock = new SemaphoreSlim(0, 1);
			var availableGuilds = new Dictionary<ulong, bool>();

			void checkGuildLock()
			{
				if (availableGuilds.Count == client.Guilds.Count)
				{
					guildLock.Release();
				}
			}

			client.GuildAvailable += (guild) =>
			{
				availableGuilds[guild.Id] = true;
				checkGuildLock();

				return Task.CompletedTask;
			};

			client.GuildUnavailable += (guild) =>
			{
				availableGuilds[guild.Id] = false;
				checkGuildLock();

				return Task.CompletedTask;
			};

			client.Ready += () =>
			{
				readyLock.Release();
				checkGuildLock();

				return Task.CompletedTask;
			};

			client.LoginAsync(TokenType.Bot, botToken).Wait();
			client.StartAsync().Wait();

			readyLock.Wait();
			guildLock.Wait();

			return client;
		}
	}
}
