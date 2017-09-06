namespace IronAdvantage.Bot.Commands
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	using DSharpPlus.CommandsNext;
	using DSharpPlus.CommandsNext.Attributes;
	using DSharpPlus.Entities;

	using IronAdvantage.Bot.Utility;

	[RequireOwner]
	internal class OwnerCommands
	{
		#region Public Methods and Operators

		[Command("verify")]
		[Description("Verifies a user to the Discord")]
		public async Task Verify(CommandContext ctx)
		{
			await ctx.Message.DeleteAsync();

			var user = Utility.GetUser(ctx, ctx.Message.MentionedUsers[0].Id);
			if (user == null) return;

			var generalChannel = Utility.GetChannel(ctx, "general");
			var memberRole = Utility.GetRole(ctx, "Verified Member");

			await user.ReplaceRolesAsync(new List<DiscordRole> { memberRole });

			var embed = new DiscordEmbedBuilder
				            { Color = new DiscordColor(0x09C100), Title = "🙋 User Joined - Give them a warm welcome!" };

			embed.AddField("Username", $"{user.Username}#{user.Discriminator}", true);
			embed.AddField("ID", $"{user.Id}", true);
			embed.Build();

			await generalChannel.SendMessageAsync("", embed: embed);
		}

		[Command("print")]
		[Hidden]
		public async Task Print(CommandContext ctx)
		{
			await Utility.PrintEmbed(ctx);
		}

		#endregion
	}
}