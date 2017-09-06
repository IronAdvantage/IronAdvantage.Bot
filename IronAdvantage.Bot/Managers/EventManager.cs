namespace IronAdvantage.Bot.Managers
{
	using System;
	using System.Linq;
	using System.Threading.Tasks;

	using DSharpPlus;
	using DSharpPlus.EventArgs;

	internal class EventManager
	{
		#region Public Methods and Operators

		public Task ClientError(ClientErrorEventArgs e)
		{
			e.Client.DebugLogger.LogMessage(LogLevel.Error,
				"Client Error",
				$"Exception occured: {e.Exception.GetType()}: {e.Exception.Message}",
				DateTime.Now);
			return Task.CompletedTask;
		}

		public Task OnMessage(MessageCreateEventArgs e)
		{
			return Task.CompletedTask;
		}

		public Task Ready(ReadyEventArgs e)
		{
			e.Client.DebugLogger.LogMessage(LogLevel.Info, "Ready", "Client is ready to process events.", DateTime.Now);
			return Task.CompletedTask;
		}

		public Task SocketError(SocketErrorEventArgs e)
		{
			e.Client.DebugLogger.LogMessage(LogLevel.Error,
				"Socket Error",
				$"Exception occured: {e.Exception.GetType()}: {e.Exception.Message}",
				DateTime.Now);
			return Task.CompletedTask;
		}

		#endregion

		public Task OnNewUser(GuildMemberAddEventArgs e)
		{
			var unverified = e.Guild.Roles.First(x => x.Name == "Unverified User");
			e.Member.GrantRoleAsync(unverified);

			return Task.CompletedTask;
		}
	}
}