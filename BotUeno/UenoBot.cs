using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Discord.Audio;
using System.Diagnostics;

namespace BotUeno
{
    public class UenoBot : ModuleBase
    {
        [Command("Help")]
        [Alias("help")]
        public async Task Help()
        {
            var embed = new EmbedBuilder()
            {
                Color = new Color(255, 68, 224)
            };
            embed.Description = (":heart:**My Commands**:heart:\n" +
                ":eye:**$status** - To view your current status.\n" +
                ":loudspeaker: **$say** - Im going to repeat whatever you say.\n" +
                ":joy:**$meme** - Send some random meme.\n" +
                ":dancer:**$bestgirl** - Best images of best girl.\n" +
                ":video_game:**$flip** *heads/tails points* - Choose between Heads or Tail (You need to bet your *Points minimum of 5* to play)\n" +
                ":fist::hand_splayed::v:**$rps** *rock/paper/scissor points* - Choose between Rock, Paper or Scissor (You need to bet your *Points minimum of 5* to play)");
            await Context.Channel.SendMessageAsync("", false, embed);
        }

        [Command("Status")]
        [Alias("status","stats","Stats")]
        public async Task Status([Remainder] IUser user = null)
        {
            var database = new UenoDB("UenoDB");
            var embed = new EmbedBuilder();
            embed.WithColor(new Color(255, 68, 224));
            if (user == null)
            {
                user = Context.User;
            }

            if (UenoDB.CheckExistingUser(user).Count() <= 0)
            {
                UenoDB.EnterUser(user);
            }
 
            var userinfo = UenoDB.GetUserStatus(user);

            embed.Description = (Context.User.Mention + ", \n\n" + "***Status*** \n" +
                            ":small_blue_diamond:" + "**UserID:** *" + userinfo.FirstOrDefault().UserId + "*\n" +
                            ":small_orange_diamond:" + "**Level:** *" + userinfo.FirstOrDefault().Lvl + "*\n" +
                            ":small_blue_diamond:" + "**XP:** *" + userinfo.FirstOrDefault().XP + "*\n" +
                            ":small_orange_diamond:" + "**Role:** *" + userinfo.FirstOrDefault().GuildRole + "*\n" +
                            ":small_blue_diamond:" + "**Points:** *" + userinfo.FirstOrDefault().Points + "*");
            await Context.Channel.SendMessageAsync("", false, embed);
            database.CloseConnection();
        }

        [Command("Statusof")]
        [RequireUserPermission(GuildPermission.Administrator)] ///Needed User Permissions ///
        [Alias("statusof", "statsof", "Statsof")]
        public async Task StatusOf(SocketGuildUser user)
        {
            var database = new UenoDB("UenoDB");
            var embed = new EmbedBuilder();
            embed.WithColor(new Color(255, 68, 224));

            var userinfo = UenoDB.GetUserStatus(user);

            embed.Description = ("***Status of*** " + user.Username + " \n" +
                            ":small_blue_diamond:" + "**UserID:** *" + userinfo.FirstOrDefault().UserId + "*\n" +
                            ":small_orange_diamond:" + "**Level:** *" + userinfo.FirstOrDefault().Lvl + "*\n" +
                            ":small_blue_diamond:" + "**XP:** *" + userinfo.FirstOrDefault().XP + "*\n" +
                            ":small_orange_diamond:" + "**Role:** *" + userinfo.FirstOrDefault().GuildRole + "*\n" +
                            ":small_blue_diamond:" + "**Points:** *" + userinfo.FirstOrDefault().Points + "*");
            await Context.Channel.SendMessageAsync("", false, embed);
            database.CloseConnection();
        }

        [Command("rps",RunMode = RunMode.Async)]
        public async Task RockPaperScissor(string rps = null, int points = 0,[Remainder] IUser user = null)
        {
            int rockpaperscissor = rnd.Next(1,4);
            if (rps == null) throw new ArgumentException("```You must choose either Rock, Paper or Scissor```");
            if (points < 5) throw new ArgumentException("```You must enter the amount of points minimum of 5 points```");

            if (user == null)
            {
                user = Context.User;
            }

            var embed = new EmbedBuilder();
            embed.WithColor(new Color(255, 68, 224));

            var database = new UenoDB("UenoDB");

            var userinfo = UenoDB.GetUserStatus(user);

            
            if(userinfo.FirstOrDefault().Points < points)
            {
                embed.Description = (Context.User.Mention + $" You don't have enough **Points** to bet!");
                await Context.Channel.SendMessageAsync("", false, embed);
            }
            else
            {
                if (rockpaperscissor == 1)
                {
                    if (rps == "Rock" || rps == "rock" || rps == "R" || rps == "r")
                    {
                        embed.Description = (Context.User.Mention + $" :fist:vs:fist:\nit's a DRAW!:sweat_smile::sweat_smile:");
                        await Context.Channel.SendMessageAsync("", false, embed);
                        await Context.Channel.SendFileAsync("Game/RPSRock.png");
                    }
                    else if (rps == "Paper" || rps == "paper" || rps == "P" || rps == "p")
                    {
                        embed.Description = (Context.User.Mention + $" :hand_splayed:vs:fist:\nYou WON! **{points} Points**:money_mouth::money_mouth::money_mouth:");
                        await Context.Channel.SendMessageAsync("", false, embed);
                        UenoDB.RockPaperScissorWin(user, points);
                        await Context.Channel.SendFileAsync("Game/RPSRock.png");
                    }
                    else if (rps == "Scissors" || rps == "scissor" || rps == "S" || rps == "s")
                    {
                        embed.Description = (Context.User.Mention + $" :v:vs:fist:\nYou LOSE **{points} Points**:dizzy_face::scream: Better luck next time!");
                        await Context.Channel.SendMessageAsync("", false, embed);
                        UenoDB.RockPaperScissor(user, points);
                        await Context.Channel.SendFileAsync("Game/RPSRock.png");
                    }
                    else
                    {
                        embed.Description = (Context.User.Mention + " **Invalid input**");
                        await Context.Channel.SendMessageAsync("", false, embed);
                    }
                }
                else if (rockpaperscissor == 2)
                {
                    if (rps == "Rock" || rps == "rock" || rps == "R" || rps == "r")
                    {
                        embed.Description = (Context.User.Mention + $" :fist:vs:hand_splayed:\nYou LOSE **{points} Points**:dizzy_face::scream: Better luck next time!");
                        await Context.Channel.SendMessageAsync("", false, embed);
                        UenoDB.RockPaperScissor(user, points);
                        await Context.Channel.SendFileAsync("Game/RPSPaper.png");
                    }
                    else if (rps == "Paper" || rps == "paper" || rps == "P" || rps == "p")
                    {
                        embed.Description = (Context.User.Mention + $" :hand_splayed:vs:hand_splayed:\nit's a DRAW!:sweat_smile::sweat_smile:");
                        await Context.Channel.SendMessageAsync("", false, embed);
                        await Context.Channel.SendFileAsync("Game/RPSPaper.png");
                    }
                    else if (rps == "Scissors" || rps == "scissor" || rps == "S" || rps == "s")
                    {
                        embed.Description = (Context.User.Mention + $" :v:vs:hand_splayed:\nYou WON! **{points} Points**:money_mouth::money_mouth::money_mouth:");
                        await Context.Channel.SendMessageAsync("", false, embed);
                        UenoDB.RockPaperScissorWin(user, points);
                        await Context.Channel.SendFileAsync("Game/RPSPaper.png");
                    }
                    else
                    {
                        embed.Description = (Context.User.Mention + " **Invalid input**");
                        await Context.Channel.SendMessageAsync("", false, embed);
                    }
                }
                else
                {
                    if (rps == "Rock" || rps == "rock" || rps == "R" || rps == "r")
                    {
                        embed.Description = (Context.User.Mention + $" :fist:vs:v:\nYou WON! **{points} Points**:money_mouth::money_mouth::money_mouth:");
                        await Context.Channel.SendMessageAsync("", false, embed);
                        UenoDB.RockPaperScissorWin(user, points);
                        await Context.Channel.SendFileAsync("Game/RPSScissor.png");
                    }
                    else if (rps == "Paper" || rps == "paper" || rps == "P" || rps == "p")
                    {
                        embed.Description = (Context.User.Mention + $" :hand_splayed:vs:v:\nYou LOSE **{points} Points**:dizzy_face::scream: Better luck next time!");
                        await Context.Channel.SendMessageAsync("", false, embed);
                        UenoDB.RockPaperScissor(user, points);
                        await Context.Channel.SendFileAsync("Game/RPSScissor.png");
                    }
                    else if (rps == "Scissors" || rps == "scissor" || rps == "S" || rps == "s")
                    {
                        embed.Description = (Context.User.Mention + $" :v:vs:v:\nit's a DRAW!:sweat_smile::sweat_smile:");
                        await Context.Channel.SendMessageAsync("", false, embed);
                        await Context.Channel.SendFileAsync("Game/RPSScissor.png");
                    }
                    else
                    {
                        embed.Description = (Context.User.Mention + " **Invalid input**");
                        await Context.Channel.SendMessageAsync("", false, embed);
                    }
                }
            }
        }

        [Command("flip", RunMode = RunMode.Async)]
        public async Task GameFlip(string coin = null, int points = 0, [Remainder] IUser user = null)
        {
            int flip = rnd.Next(0, 2);
            if (coin == null) throw new ArgumentException("```You must choose either Heads or Tail```");
            if (points < 5) throw new ArgumentException("```You must enter the amount of points minimum of 5 points```");

            if (user == null)
            {
                user = Context.User;
            }

            var embed = new EmbedBuilder();
            embed.WithColor(new Color(255, 68, 224));

            var database = new UenoDB("UenoDB");



            var userinfo = UenoDB.GetUserStatus(user);


            if (userinfo.FirstOrDefault().Points < points)
            {
                embed.Description = (Context.User.Mention + $" You don't have enough **Points** to bet!");
                await Context.Channel.SendMessageAsync("", false, embed);
            }
            else
            {
                UenoDB.FlipCoin(user, points);
                if (flip == 0)
                {
                    if (coin == "Head" || coin == "head" || coin == "Heads" || coin == "heads" || coin == "h")
                    {
                        embed.Description = (Context.User.Mention + $" You WON! **{points} Points**:money_mouth::money_mouth::money_mouth:");
                        await Context.Channel.SendMessageAsync("", false, embed);
                        UenoDB.FlipCoinWon(user, points);
                    }
                    else
                    {
                        embed.Description = (Context.User.Mention + $" You LOSE **{points} Points**:dizzy_face::scream: Better luck next time!");
                        await Context.Channel.SendMessageAsync("", false, embed);
                    }
                    await Context.Channel.SendFileAsync("Game/FlipHead.png");
                }
                else
                {
                    if (coin == "Tail" || coin == "tail" || coin == "Tails" || coin == "tails" || coin == "t")
                    {
                        embed.Description = (Context.User.Mention + $" You WON! **{points} Points**:money_mouth::money_mouth::money_mouth:");
                        await Context.Channel.SendMessageAsync("", false, embed);
                        UenoDB.FlipCoinWon(user, points);
                    }
                    else
                    {
                        embed.Description = (Context.User.Mention + $" You LOSE **{points} Points**:dizzy_face::scream: Better luck next time!");
                        await Context.Channel.SendMessageAsync("", false, embed);
                    }
                    await Context.Channel.SendFileAsync("Game/FlipTail.png");
                }
            }
        }
        //

        //Kick/Ban
        [Command("Ban")]
        [Alias("ban")]
        [Summary("Ban @Username")]
        [RequireUserPermission(GuildPermission.BanMembers)] ///Needed User Permissions ///
        [RequireBotPermission(GuildPermission.BanMembers)] ///Needed Bot Permissions ///
        public async Task BanAsync(SocketGuildUser user = null, [Remainder] string reason = null)
        {
            if (user == null) throw new ArgumentException("```You must mention a user```");
            if (string.IsNullOrWhiteSpace(reason)) throw new ArgumentException("```You must provide a reason```");

            var gld = Context.Guild as SocketGuild;
            var embed = new EmbedBuilder(); ///starts embed///
            embed.WithColor(new Color(255, 68, 224)); ///hexacode colours ///
            embed.Title = $"**{user.Mention}** was BANNED:pepeGun::pepeGun::pepeGun: from **{user.Guild.Name}**";///Who was banned///
            embed.Description = $"**Username: **{user.Username}\n**Guild Name: **{user.Guild.Name}\n**Banned by: **{Context.User.Mention}!\n**Reason: **{reason}"; ///Embed values///

            await gld.AddBanAsync(user);///bans selected user///
            await Context.Channel.SendMessageAsync("", false, embed);///sends embed///
        }

        [Command("Kick")]
        [Alias("kick")]
        [Summary("Kick @Username")]
        [RequireUserPermission(GuildPermission.KickMembers)] ///Needed User Perms///
        [RequireBotPermission(GuildPermission.KickMembers)] ///Needed BotPerms///
        public async Task KickAsync(SocketGuildUser user = null, [Remainder] string reason = null)
        {
            if (user == null) throw new ArgumentException("```You must mention a user```");
            if (string.IsNullOrWhiteSpace(reason)) throw new ArgumentException("```You must provide a reason```");

            var gld = Context.Guild as SocketGuild;
            var embed = new EmbedBuilder(); ///starts embed///
            embed.WithColor(new Color(255, 68, 224)); ///hexacode colours ///
            embed.Title = $" {user.Mention} has been KICKED:gun::gun::gun: from **{user.Guild.Name}**"; ///who was kicked///
            embed.Description = $"**Username: **{user.Username}\n**Guild Name: **{user.Guild.Name}\n**Kicked by: **{Context.User.Mention}!\n**Reason: **{reason}"; ///embed values///

            await user.KickAsync(); ///kicks selected user///
            await Context.Channel.SendMessageAsync("", false, embed); ///sends embed///
        }
        //End
        //

        [Command("award", RunMode = RunMode.Async)]
        [Alias("Award")]
        [RequireUserPermission(GuildPermission.Administrator)] ///Needed User Permissions ///
        public async Task Award(SocketGuildUser user, [Remainder] int points)
        {
            var embed = new EmbedBuilder();
            embed.WithColor(new Color(255, 68, 224));
            embed.Description = (user.Mention + ", was awarded ***" + points + " points!***:money_mouth::money_mouth::gift_heart:");
            if (user == null) throw new ArgumentException("```You must mention a user```");
            UenoDB.ChangePoints(user, points);
            await ReplyAsync("", false, embed);
        }


        [Command("Say")]
        [Alias("say")]
        [Summary("have the bot repeat whatever you say")]
        public async Task Say([Remainder, Summary("echo for the bot")]string echo = null)
        {
            var embed = new EmbedBuilder(); ///starts embed///
            embed.WithColor(new Color(255, 68, 224)); ///hexacode colours ///
            if (echo == null)
            {
                embed.Description = ("```Say something with the command to have the repeat it!```");
                await ReplyAsync("",false, embed);
            }
            else
            {
                embed.Description = (Context.User.Mention + ":loudspeaker: *" + echo + "*");
                await ReplyAsync("",false, embed);
            }
        }

        //Purge Chat
        [Command("purge")]
        [RequireBotPermission(GuildPermission.ManageMessages)]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        [Alias("clear", "delete")]
        public async Task Purge([Remainder] int num = 0)
        {
            if (num <= 100)
            {
                var DeleteMessages = await Context.Channel.GetMessagesAsync(num + 1).Flatten();
                await Context.Channel.DeleteMessagesAsync(DeleteMessages);
                if (num == 1)
                {
                    await Context.Channel.SendMessageAsync(Context.User.Username + " *deleted 1 message.*");
                }
                else
                {
                    await Context.Channel.SendMessageAsync(Context.User.Username + " *deleted " + num + " message.*");
                }
            }
            else
            {
                await ReplyAsync("You cannot delete more than *100 messages*");
            }
        }
        Random rnd = new Random();
        string[] bestgirllist = new string[]
            {
                "Ueno/u0.png", "Ueno/u1.png","Ueno/u2.png","Ueno/u3.jpg","Ueno/u4.jpg","Ueno/u5.jpg","Ueno/u6.png","Ueno/u7.jpg"
            };
        string[] memelist = new string[] 
            {"Meme/m0.jpg","Meme/m1.jpg","Meme/m2.jpg","Meme/m3.jpeg","Meme/m4.jpg","Meme/m5.jpg","Meme/m6.jpg","Meme/m7.jpg","Meme/m8.jpg","Meme/m9.png",
                "Meme/m10.jpg","Meme/m11.png","Meme/m12.jpg","Meme/m13.jpg","Meme/m14.jpg","Meme/m15.jpg"
            };


        //Best Girl
        [Command("Bestgirl")]
        [Alias("bestgirl")]
        public async Task BestGirl()
        {
            int randomBestGirlIndex = rnd.Next(bestgirllist.Length);
            string BestGirlPost = bestgirllist[randomBestGirlIndex];
            await Context.Channel.SendFileAsync(BestGirlPost, Context.Message.Author.Mention);
        }

        //Posting Meme
        [Command("Meme")]
        [Alias("meme")]
        public async Task Meme()
        {
            var embed = new EmbedBuilder(); ///starts embed///
            embed.WithColor(new Color(255, 68, 224));
            int randomText = rnd.Next(1, 6);
            int randomMemeIndex = rnd.Next(memelist.Length);
            string MemePost = memelist[randomMemeIndex];
            if (randomText == 1)
            {
                embed.Description = ("A meme for you...");
                await ReplyAsync("", false, embed);
            }
            else if (randomText == 2)
            {
                embed.Description = ("Your request...");
                await ReplyAsync("",false, embed);
            }
            else if (randomText == 3)
            {
                embed.Description = ("Here is the meme...");
                await ReplyAsync("", false, embed);
            }
            else if (randomText == 4)
            {
                embed.Description = ("Slice of meme...");
                await ReplyAsync("", false, embed);
            }
            else if (randomText == 5)
            {
                embed.Description = ("Here :heart:...");
                await ReplyAsync("", false, embed);
            }
            //await (MemePost);
            await Context.Channel.SendFileAsync(MemePost, Context.Message.Author.Mention);
        }

        [Command("play", RunMode = RunMode.Async)]
        public async Task play(string url)
        {
            IVoiceChannel channel = (Context.User as IVoiceState).VoiceChannel;
            IAudioClient client = await channel.ConnectAsync();

            var output = CreateStream(url).StandardOutput.BaseStream;
            var stream = client.CreatePCMStream(AudioApplication.Music, 128 * 1024);
            await output.CopyToAsync(stream);
            await stream.FlushAsync().ConfigureAwait(false);
        }

        private Process CreateStream(string url)
        {
            Process currentsong = new Process();

            currentsong.StartInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/C youtube-dl.exe -o - {url} | ffmpeg -i pipe:0 -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };

            currentsong.Start();
            return currentsong;
        }
        [Command("info")]
        [Summary("Shows All Bot Info.")]
        public async Task Info()
        {
            using (var process = Process.GetCurrentProcess())
            { ///this is required for up time
                var embed = new EmbedBuilder();
                var application = await Context.Client.GetApplicationInfoAsync(); ///for lib version


                embed.ImageUrl = application.IconUrl; ///pulls bot Avatar. Not needed can be removed
                embed.WithColor(new Color(0x4900ff)) ///Hexacode colours

                .AddField(y => { ///new embed field
                    y.Name = "Author."; ///Field name here
                    y.Value = application.Owner.Username; application.Owner.Id.ToString(); ///Code here. If INT convert to string
                    y.IsInline = false;
                }).AddField(y => /// add new field, rinse and repeat
                {
                    y.Name = "Uptime.";
                    var time = DateTime.Now - process.StartTime; /// Subtracts current time and start time to get Uptime
                    var sb = new StringBuilder();
                    if (time.Days > 0)
                    {
                        sb.Append($"{time.Days}d ");
                    }
                    if (time.Hours > 0)
                    {
                        sb.Append($"{time.Hours}h ");
                    }
                    if (time.Minutes > 0)
                    {
                        sb.Append($"{time.Minutes}m ");
                    }
                    sb.Append($"{time.Seconds}s ");
                    y.Value = sb.ToString();
                    y.IsInline = true;
                }).AddField(y =>
                {
                    y.Name = "Discord.net version.";///pulls discord lib version
                    y.Value = DiscordConfig.Version;
                    y.IsInline = true;
                }).AddField(y => {
                    y.Name = "Server Amount.";
                    y.Value = (Context.Client as DiscordSocketClient).Guilds.Count.ToString(); ///Numbers of servers the bot is in
                    y.IsInline = false;
                }).AddField(y => {
                    y.Name = "Heap Size";///pulls ram usage of modules/heaps
                    y.Value = GetHeapSize();
                    y.IsInline = false;
                }).AddField(y => {
                    y.Name = "Number Of Users";
                    y.Value = (Context.Client as DiscordSocketClient).Guilds.Sum(g => g.Users.Count).ToString(); ///Counts users
                    y.IsInline = false;
                });

                await ReplyAsync("", embed: embed);
            }
        }
        private static string GetHeapSize() => Math.Round(GC.GetTotalMemory(true) / (1024.0 * 1024.0), 2).ToString();
        ///required for GetHeapSize
    }
}
