namespace IronAdvantage.Bot.Commands
{
	using System;
	using System.Threading.Tasks;

	using DSharpPlus.CommandsNext;
	using DSharpPlus.CommandsNext.Attributes;
	using DSharpPlus.Entities;
	using DSharpPlus.Interactivity;

	using IronAdvantage.Bot.Utility;

	[RequireRole("Unverified User")]
	internal class UnverifiedCommands
	{
		#region Public Methods and Operators

		[Command("join")]
		[Hidden]
		public async Task Join(CommandContext ctx)
		{
			var user = ctx.Member;
			var message = $"<@{user.Id}>, use this channel to speak to the admin staff in order to verify your account.\n"
			              + $"<@{Utility.GetUser(ctx, "Iron").Id}> <@{Utility.GetUser(ctx, "Atlas").Id}";
			await Utility.CreatePrivateChannel(ctx, user, message);
		}

		[Command("leave")]
		[Hidden]
		public async Task Leave(CommandContext ctx)
		{
			if (ctx.Channel.Id != Utility.GetChannel(ctx, "landing-page").Id) return;
			var user = ctx.User;

			await ctx.RespondAsync("Please type `GET ME OUT OF HERE` to leave the server or `LET ME STAY` to stay.");
			var interactivity = ctx.Client.GetInteractivityModule();
			var confirmation =
				await interactivity.WaitForMessageAsync(xm => xm.Author.Id == ctx.Member.Id, TimeSpan.FromSeconds(60));

			if (confirmation.Message.Content.ToLower().Contains("get me out of here")) await ctx.Guild.RemoveMemberAsync((DiscordMember)user);
			else if (confirmation.Message.Content.ToLower().Contains("let me stay"))
			{
				var message = ctx.RespondAsync("Fine. You can stay.").Result;
				await Task.Delay(5000).ContinueWith(task => message.DeleteAsync());
			}
		}

		#endregion
	}
}