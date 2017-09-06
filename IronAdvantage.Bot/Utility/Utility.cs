namespace IronAdvantage.Bot.Utility
{
	using System;
	using System.Linq;
	using System.Threading.Tasks;

	using DSharpPlus;
	using DSharpPlus.CommandsNext;
	using DSharpPlus.Entities;
	using DSharpPlus.EventArgs;
	using DSharpPlus.Interactivity;

	internal class Utility
	{
		#region Public Methods and Operators

		public static async Task CreatePrivateChannel(CommandContext ctx, DiscordMember user, string message)
		{
			var existingChannel = GetChannel(ctx, "text-" + user.DisplayName.Replace(" ", "-"));
			if (existingChannel != null)
			{
				var exist = ctx.RespondAsync($"<#{existingChannel.Id}> already exists.").Result;
				await Task.Delay(5000);
				await exist.DeleteAsync();
				return;
			}
			var channel = ctx.Guild.CreateChannelAsync("text-" + user.DisplayName.Replace(" ", "-"), ChannelType.Text).Result;
			var everyone = ctx.Guild.EveryoneRole;
			await channel.AddOverwriteAsync(everyone,
				Permissions.None,
				Permissions.ReadMessages | Permissions.ReadMessageHistory);
			await channel.AddOverwriteAsync(ctx.Member,
				Permissions.ReadMessages | Permissions.ReadMessageHistory | Permissions.SendMessages,
				Permissions.None);
			await channel.AddOverwriteAsync(user,
				Permissions.ReadMessages | Permissions.ReadMessageHistory | Permissions.SendMessages,
				Permissions.None);
			await Task.Delay(250);
			await channel.SendMessageAsync(message);
		}

		public static async Task CreateVoiceChannel(CommandContext ctx, DiscordMember user, string message)
		{
			var existingChannel = GetChannel(ctx, "voice-" + user.DisplayName.Replace(" ", "-"));
			if (existingChannel != null)
			{
				var exist = ctx.RespondAsync($"Voice channel already exists.").Result;
				await Task.Delay(5000);
				await exist.DeleteAsync();
				return;
			}
			var channel = ctx.Guild.CreateChannelAsync("voice-" + user.DisplayName.Replace(" ", "-"), ChannelType.Voice).Result;
			var everyone = ctx.Guild.EveryoneRole;
			await channel.AddOverwriteAsync(everyone, Permissions.None, Permissions.UseVoice | Permissions.Speak);
			await channel.AddOverwriteAsync(ctx.Member,
				Permissions.UseVoice | Permissions.Speak | Permissions.UseVoiceDetection,
				Permissions.None);
			await channel.AddOverwriteAsync(user,
				Permissions.UseVoice | Permissions.Speak | Permissions.UseVoiceDetection,
				Permissions.None);
		}

		public static async Task EndChannel(CommandContext ctx, string type)
		{
			var user = GetUser(ctx, ctx.Message.MentionedUsers[0].Id);
			var channel = GetChannel(ctx, type + "-" + user.DisplayName.Replace(" ", "-"));

			if (channel == null) return;

			await ctx.RespondAsync("Are you sure you want to end session? Yes/No");
			var interactivity = ctx.Client.GetInteractivityModule();
			var confirmation =
				await interactivity.WaitForMessageAsync(xm => xm.Author.Id == ctx.Member.Id, TimeSpan.FromSeconds(60));

			if (confirmation.Message.Content.ToLower().Contains("yes")) await channel.DeleteAsync($"{type} chat closed.");
			else if (confirmation.Message.Content.ToLower().Contains("no")) await ctx.RespondAsync("Session Resumed.");
		}

		public static DiscordMember GetUser(CommandContext ctx, string user)
		{
			return ctx.Guild.Members.FirstOrDefault(
				x => string.Equals(x.DisplayName, user, StringComparison.CurrentCultureIgnoreCase));
		}

		public static DiscordMember GetUser(CommandContext ctx, ulong id)
		{
			return ctx.Guild.GetMemberAsync(id).Result;
		}

		public static DiscordMember GetUser(MessageCreateEventArgs e, string user)
		{
			return e.Guild.Members.FirstOrDefault(x => string.Equals(x.DisplayName,
				user,
				StringComparison.CurrentCultureIgnoreCase));
		}

		public static DiscordMember GetUser(MessageCreateEventArgs e, ulong id)
		{
			return e.Guild.GetMemberAsync(id).Result;
		}

		public static DiscordChannel GetChannel(CommandContext ctx, ulong id)
		{
			return ctx.Guild.GetChannel(id);
		}

		public static DiscordChannel GetChannel(CommandContext ctx, string channelName)
		{
			return ctx.Guild.Channels.FirstOrDefault(x => x.Name == channelName);
		}

		public static DiscordRole GetRole(CommandContext ctx, ulong id)
		{
			return ctx.Guild.GetRole(id);
		}

		public static DiscordRole GetRole(CommandContext ctx, string roleName)
		{
			return ctx.Guild.Roles.FirstOrDefault(x => string.Equals(x.Name,
				roleName,
				StringComparison.InvariantCultureIgnoreCase));
		}

		public static async Task PrintEmbed(CommandContext ctx)
		{
			await ctx.Message.DeleteAsync();
			var embed = new DiscordEmbedBuilder
				            {
					            Color = new DiscordColor(0x9B59B6), Title = "Welcome to IronAdvantage. Your advantage for PUBG.",
					            Author = new DiscordEmbedBuilder.EmbedAuthor
						                     { IconUrl = ctx.Member.AvatarUrl, Name = ctx.Message.Author.Username, Url = "http://IronAdvantage.pro"},
								ThumbnailUrl = ctx.Guild.IconUrl,
					            Description = "To access this Discord Server you must first be authenticated by an Administrator. "
					                          + "This is to keep security at a maximum, for you and I.\n"
											  + "If you haven't already, check out the website for a full list of features: "
					                          + "[IronAdvantage](http://IronAdvantage.pro)",
								Footer = new DiscordEmbedBuilder.EmbedFooter
									         {
													IconUrl = ctx.Guild.IconUrl,
													Text = "IronAdvantage"
									         },
								Timestamp = new DateTimeOffset(DateTime.Now)
				            };

			embed.AddField("I would like to join.",
				"Please type: `.join` in order to setup a private group chat with the administration team.",
				true);
			embed.AddField("I'm lost, get me out of here.", "Please type: `.leave` to leave the server completely.", true);

			await ctx.RespondAsync("", embed: embed);
		}

		public static async Task UserEmbed(CommandContext ctx, DiscordUser discordUser, int color, string title, string reason)
		{
			var embed = new DiscordEmbedBuilder { Color = new DiscordColor(color), Title = title };
			embed.AddField("Username", $"{discordUser.Username}#{discordUser.Discriminator}", true);
			embed.AddField("ID", $"{discordUser.Id}", true);
			embed.Description = reason ?? "No reason";

			embed.Build();
			await ctx.RespondAsync("", embed: embed);
		}

		#endregion
	}
}