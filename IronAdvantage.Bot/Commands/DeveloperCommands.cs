namespace IronAdvantage.Bot.Commands
{
	using System;
	using System.Diagnostics;
	using System.Linq;
	using System.Threading.Tasks;

	using DSharpPlus;
	using DSharpPlus.CommandsNext;
	using DSharpPlus.CommandsNext.Attributes;

	using IronAdvantage.Bot.Utility;

	[RequireUserPermissions(Permissions.Administrator)]
	internal class DeveloperCommands
	{
		#region Public Methods and Operators

		[Command("ban")]
		[Description("Bans the user")]
		public async Task Ban(CommandContext ctx, [Description("User to be banned")] string user, string reason = null)
		{
			if (ctx.Message.MentionedUsers.Count != 0)
			{
				var discordUser = ctx.Message.MentionedUsers.FirstOrDefault();
				if (discordUser != null)
				{
					await ctx.Guild.BanMemberAsync(discordUser.Id, int.MaxValue, reason);
					await Utility.UserEmbed(ctx, discordUser, 0xFF0000, "⛔️ User Banned", reason);
				}
			}
			else
			{
				var discordUser = Utility.GetUser(ctx, user);
				if (discordUser != null)
				{
					await ctx.Guild.BanMemberAsync(discordUser, int.MaxValue, reason);
					await Utility.UserEmbed(ctx, discordUser, 0xFF0000, "⛔️ User Banned", reason);
				}
			}
		}

		[Command("kick")]
		[Description("Kicks the user")]
		public async Task Kick(CommandContext ctx, string user, string reason = null)
		{
			if (ctx.Message.MentionedUsers.Count != 0)
			{
				var discordUser = Utility.GetUser(ctx, ctx.Message.MentionedUsers.FirstOrDefault()?.Username);
				if (discordUser != null)
				{
					await ctx.Guild.RemoveMemberAsync(discordUser, reason);
					await Utility.UserEmbed(ctx, discordUser, 0xFFD400, "🏈 User Kicked", reason);
				}
			}
			else
			{
				var discordUser = Utility.GetUser(ctx, user);
				if (discordUser != null)
				{
					await ctx.Guild.RemoveMemberAsync(discordUser, reason);
					await Utility.UserEmbed(ctx, discordUser, 0xFFD400, "🏈 User Kicked", reason);
				}
			}
		}

		[Command("prune")]
		[Description("Deletes and X amount of messages")]
		public async Task Prune(CommandContext ctx, [Description("Amount of messages to delete")] int number = 1)
		{
			await ctx.Channel.DeleteMessagesAsync(await ctx.Channel.GetMessagesAsync(number < 100 && number > 0 ? number + 1 : 100));
		}

		[Command("restart")]
		[Description("Restarts the bot")]
		public async Task Restart(CommandContext ctx)
		{
			try
			{
				Process.Start("IronAdvantage.Bot.exe");
				await ctx.RespondAsync("Restarted.");
			}
			catch (Exception e)
			{
				ctx.Client.DebugLogger.LogMessage(LogLevel.Error,
					"Restart Command",
					$"Unable to start the program: {e}",
					DateTime.Now);
			}
			Process.GetProcessById(Process.GetCurrentProcess().Id).CloseMainWindow();
			await Task.CompletedTask;
		}

		#endregion
	}
}