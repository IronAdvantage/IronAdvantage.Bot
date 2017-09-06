namespace IronAdvantage.Bot
{
	using System.Threading.Tasks;

	using DSharpPlus;
	using DSharpPlus.CommandsNext;
	using DSharpPlus.Interactivity;

	using IronAdvantage.Bot.Commands;
	using IronAdvantage.Bot.Managers;

	using Newtonsoft.Json;

	internal class Program
	{
		#region Public Properties

		public DiscordClient Client { get; set; }

		public CommandManager CommandManager { get; set; }

		public CommandsNextModule Commands { get; set; }

		public EventManager EventManager { get; set; }

		public InteractivityModule Interactivity { get; set; }

		#endregion

		#region Public Methods and Operators

		public async Task RunBot()
		{
			var json = JsonManager.ParseJsonAsync(@"..\config.json").Result;
			var cfgjson = JsonConvert.DeserializeObject<ConfigJson>(json);
			var cfg = new DiscordConfiguration
				          {
					          Token = cfgjson.Token, TokenType = TokenType.Bot, AutoReconnect = true, LogLevel = LogLevel.Info,
					          UseInternalLogHandler = true
				          };

			var ccfg = new CommandsNextConfiguration
				           { StringPrefix = cfgjson.Prefix, EnableDms = true, EnableMentionPrefix = true };

			this.Client = new DiscordClient(cfg);
			this.Commands = this.Client.UseCommandsNext(ccfg);
			this.EventManager = new EventManager();
			this.CommandManager = new CommandManager();

			this.Client.UseInteractivity();

			this.Client.Ready += this.EventManager.Ready;
			this.Client.GuildMemberAdded += this.EventManager.OnNewUser;
			this.Client.MessageCreated += this.EventManager.OnMessage;
			this.Client.SocketErrored += this.EventManager.SocketError;
			this.Client.ClientErrored += this.EventManager.ClientError;

			this.Commands.RegisterCommands<CommandManager>();
			this.Commands.RegisterCommands<OwnerCommands>();
			this.Commands.RegisterCommands<DeveloperCommands>();
			this.Commands.RegisterCommands<MemberCommands>();
			this.Commands.RegisterCommands<UnverifiedCommands>();

			await this.Client.ConnectAsync();
			await Task.Delay(-1);
		}

		#endregion

		#region Methods

		private static void Main()
		{
			new Program().RunBot().GetAwaiter().GetResult();
		}

		#endregion
	}
}