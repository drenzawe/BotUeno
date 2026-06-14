using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Reflection;

namespace BotUeno
{
    class Program
    {
        private CommandService commands;
        private DiscordSocketClient client;
        private IServiceProvider service;

        string token = "YOUR_DISCORD_BOT_TOKEN_HERE";


        static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            client = new DiscordSocketClient();
            commands = new CommandService();
            service = new ServiceCollection().BuildServiceProvider();

            await InstallCommands();

            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            client.Log += Log;

            ////////////////////////////////////////////////////////Starto////////////////////////////////////////////////////////

            client.UserJoined += UserJoined;
            client.UserLeft += UserLeft;
            client.MessageReceived += giveXP;
            client.MessageReceived += checkLVL;



            await client.SetGameAsync("Beating Up Nishimiya", "https://www.vblankz.blogspot.com", StreamType.Twitch);

            ////////////////////////////////////////////////////////Endo////////////////////////////////////////////////////////

            await Task.Delay(-1);
        }

        ////////////////////////////////////////////////////////Starto////////////////////////////////////////////////////////

        public async Task UserJoined(SocketGuildUser user)
        {
            var role = user.Guild.Roles.Where(has => has.Name.ToUpper() == "PEASANT".ToUpper());

            var channel = client.GetChannel(226648459778719744) as SocketTextChannel;
            await channel.SendMessageAsync(":confetti_ball:**Welcome!**:tada: " + user.Mention + " **To the :skull_crossbones:Dark Brotherhood:skull_crossbones:** **Hail Sithis!**");
            var database = new UenoDB("UenoDB");
            if (UenoDB.CheckExistingUser(user).Count() <= 0)
            {
                if (user.IsBot != true)
                {
                    UenoDB.EnterUser(user);
                }
            }
            else
            {
                var useriden = UenoDB.GetUserStatus(user).FirstOrDefault();
                var lvl = useriden.Lvl;
                if (lvl <= 10)
                {
                    GuildRole.rolePeasant(user);
                    await user.RemoveRolesAsync(role);
                    role = user.Guild.Roles.Where(has => has.Name.ToUpper() == "PEASANT".ToUpper());
                    await user.AddRolesAsync(role);
                }
                else if (lvl >= 11 && lvl <= 20)
                {
                    GuildRole.rolePlebian(user);
                    await user.RemoveRolesAsync(role);
                    role = user.Guild.Roles.Where(has => has.Name.ToUpper() == "PLEBIAN".ToUpper());
                    await user.AddRolesAsync(role);
                }
                else if (lvl >= 21 && lvl <= 50)
                {
                    GuildRole.roleCommoner(user);
                    await user.RemoveRolesAsync(role);
                    role = user.Guild.Roles.Where(has => has.Name.ToUpper() == "COMMONER".ToUpper());
                    await user.AddRolesAsync(role);
                }
                else if (lvl >= 51 && lvl <= 99)
                {
                    GuildRole.roleLoyalist(user);
                    await user.RemoveRolesAsync(role);
                    role = user.Guild.Roles.Where(has => has.Name.ToUpper() == "LOYALIST".ToUpper());
                    await user.AddRolesAsync(role);
                }
                else if (lvl >= 100)
                {
                    GuildRole.roleElite(user);
                    await user.RemoveRolesAsync(role);
                    role = user.Guild.Roles.Where(has => has.Name.ToUpper() == "ELITE".ToUpper());
                    await user.AddRolesAsync(role);
                }
            }
            database.CloseConnection();
        }
        public async Task UserLeft(SocketGuildUser user)
        {
            var role = user.Guild.Roles.Where(has => has.Name.ToUpper() == "PEASANT".ToUpper());

            var channel = client.GetChannel(226648459778719744) as SocketTextChannel;
            await channel.SendMessageAsync(user.Mention + " **has left :skull_crossbones:Dark Brotherhood:skull_crossbones:**");

            var database = new UenoDB("UenoDB");
            if (UenoDB.CheckExistingUser(user).Count() <= 0)
            {
                if (user.IsBot != true)
                {
                    UenoDB.EnterUser(user);
                }
            }
            else
            {
                var useriden = UenoDB.GetUserStatus(user).FirstOrDefault();
                var lvl = useriden.Lvl;
                if (lvl <= 10)
                {
                    GuildRole.rolePeasant(user);
                    await user.RemoveRolesAsync(role);
                    role = user.Guild.Roles.Where(has => has.Name.ToUpper() == "PEASANT".ToUpper());
                    await user.AddRolesAsync(role);
                }
                else if (lvl >= 11 && lvl <= 20)
                {
                    GuildRole.rolePlebian(user);
                    await user.RemoveRolesAsync(role);
                    role = user.Guild.Roles.Where(has => has.Name.ToUpper() == "PLEBIAN".ToUpper());
                    await user.AddRolesAsync(role);
                }
                else if (lvl >= 21 && lvl <= 50)
                {
                    GuildRole.roleCommoner(user);
                    await user.RemoveRolesAsync(role);
                    role = user.Guild.Roles.Where(has => has.Name.ToUpper() == "COMMONER".ToUpper());
                    await user.AddRolesAsync(role);
                }
                else if (lvl >= 51 && lvl <= 99)
                {
                    GuildRole.roleLoyalist(user);
                    await user.RemoveRolesAsync(role);
                    role = user.Guild.Roles.Where(has => has.Name.ToUpper() == "LOYALIST".ToUpper());
                    await user.AddRolesAsync(role);
                }
                else if (lvl >= 100)
                {
                    GuildRole.roleElite(user);
                    await user.RemoveRolesAsync(role);
                    role = user.Guild.Roles.Where(has => has.Name.ToUpper() == "ELITE".ToUpper());
                    await user.AddRolesAsync(role);
                }
            }
            database.CloseConnection();
        }

        private async Task giveXP(SocketMessage msg) /*Creates the method*/
        {
            var database = new UenoDB("UenoDB");
            var user = msg.Author; /*Sets variable user to msg.author*/
            var result = UenoDB.CheckExistingUser(user); /*We check if the database contains the user*/
            if (result.Count <= 0 && user.IsBot != true) /*Checks if result contains anyone and checks if the user is not a bot*/
            {
                UenoDB.EnterUser(user);  /*Enters the user*/
            }

            var userData = UenoDB.GetUserStatus(user).FirstOrDefault(); /*Gets the users Data from the database*/
            var xp = XP.returnXP(msg); /*sets variable xp to the number returnXP returns we also pass msg through it*/

            if (userData.XP >= 100) /*Checks if the users xp is greater or equal to the xp required to level up*/
            {
                XP.levelUp(user);  /*Levels up the user using the levelUp method we created */
                var embed = new EmbedBuilder(); /*Creates a embedBuilder*/
                embed.WithColor(new Color(0x4d006d)).AddField(y =>            /*Creates a embed with a colour then we add a field*/
                {
                    var userData2 = UenoDB.GetUserStatus(user).FirstOrDefault(); /*We create a new variable for userData because we just updated the users level im unsure if this is required I just did it to be on the safe side*/
                    y.Name = "***Leveled up!***"; /*sets the fields name to leveled up*/       
                   y.Value = $"{user.Mention} has LEVELED UP to **Level {userData2.Lvl}**!:tada::confetti_ball:";  /*We set the value to this string*/
                });
                await msg.Channel.SendMessageAsync("", embed: embed); /*Sends the embed*/
            }
            else if (user.IsBot != true)/*else if the user is not a bot then its just gonna add xp*/
            {
                XP.addXP(user, xp); /*adds the xp to the user*/
            }
            database.CloseConnection();
        }
        public async Task checkLVL(SocketMessage msg)
        {
            var cuser = msg.Author;

            var user = cuser as SocketGuildUser;
            var role = user.Guild.Roles.Where(has => has.Name.ToUpper() == "PEASANT".ToUpper());
 
            await user.AddRolesAsync(role);


            var channel = client.GetChannel(226648459778719744) as SocketTextChannel;

            var database = new UenoDB("UenoDB");
            var useriden = UenoDB.GetUserStatus(user).FirstOrDefault();
            var lvl = useriden.Lvl;
            var xp = useriden.XP;
            if (lvl <= 10)
            {
                GuildRole.rolePeasant(user);
                await user.RemoveRolesAsync(role);
                role = user.Guild.Roles.Where(has => has.Name.ToUpper() == "PEASANT".ToUpper());
            }
            else if (lvl >= 11 && lvl <= 20)
            {
                GuildRole.rolePlebian(user);
                await user.RemoveRolesAsync(role);
                role = user.Guild.Roles.Where(has => has.Name.ToUpper() == "PLEBIAN".ToUpper());
                await user.AddRolesAsync(role);
                if(lvl == 11 && xp == 0)
                {
                    await channel.SendMessageAsync(user.Mention + " :tada:**Congrats! You are now a** ***PLEBIAN***");
                }
            }
            else if (lvl >= 21 && lvl <= 50)
            {
                GuildRole.roleCommoner(user);
                await user.RemoveRolesAsync(role);
                role = user.Guild.Roles.Where(has => has.Name.ToUpper() == "COMMONER".ToUpper());
                await user.AddRolesAsync(role);
                if (lvl == 21 && xp == 0)
                {
                    await channel.SendMessageAsync(user.Mention + " :tada:**Congrats! You are now a** ***COMMONER***");
                }
            }
            else if (lvl >= 51 && lvl <= 99)
            {
                GuildRole.roleLoyalist(user);
                await user.RemoveRolesAsync(role);
                role = user.Guild.Roles.Where(has => has.Name.ToUpper() == "LOYALIST".ToUpper());
                await user.AddRolesAsync(role);
                if (lvl == 51 && xp == 0)
                {
                    await channel.SendMessageAsync(user.Mention + " :tada:**Congrats! You are now a** ***LOYALIST***");
                }
            }
            else if (lvl >= 100)
            {
                GuildRole.roleElite(user);
                await user.RemoveRolesAsync(role);
                role = user.Guild.Roles.Where(has => has.Name.ToUpper() == "ELITE".ToUpper());
                await user.AddRolesAsync(role);
                if (lvl == 100 && xp == 0)
                {
                    await channel.SendMessageAsync(user.Mention + " :tada:**Congrats! You are now a** ***LOYALIST***");
                }
            }
            database.CloseConnection();
        }

        ////////////////////////////////////////////////////////Endo////////////////////////////////////////////////////////

        public async Task InstallCommands()
        {
            // Hook the MessageReceived Event into our Command Handler
            client.MessageReceived += HandleCommand;
            // Discover all of the commands in this assembly and load them.
            await commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        public async Task HandleCommand(SocketMessage messageParam)
        {
            // Don't process the command if it was a System Message
            var message = messageParam as SocketUserMessage;
            if (message == null) return;
            // Create a number to track where the prefix ends and the command begins
            int argPos = 0;
            // Determine if the message is a command, based on if it starts with '!' or a mention prefix
            if (!(message.HasCharPrefix('$', ref argPos) || message.HasMentionPrefix(client.CurrentUser, ref argPos))) return;
            // Create a Command Context
            var context = new CommandContext(client, message);
            // Execute the command. (result does not indicate a return value, 
            // rather an object stating if the command executed successfully)
            var result = await commands.ExecuteAsync(context, argPos, service);
            if (!result.IsSuccess)
                await context.Channel.SendMessageAsync(result.ErrorReason);
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
