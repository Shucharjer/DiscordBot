using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DiscordBot.Modules
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        public async Task Ping()
        {
            await ReplyAsync("Pong");
        }
    }
}
