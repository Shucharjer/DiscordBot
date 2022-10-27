using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using System.Web;
using Discord.Commands;
using Discord.WebSocket;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Sockets;
using System.Net;

namespace DiscordBot
{
    public class Program
    {
        static void Main(string[] args) => new Program().RunBotAsync().GetAwaiter().GetResult();

        //discord在国内使用需要挂VPN，考虑sock5代理或者是把这个bot挂在国外
        private DiscordSocketClient client;
        private CommandService commands;
        private IServiceProvider services;

        

        public async Task RunBotAsync()
        {
            client = new DiscordSocketClient();
            commands = new CommandService();
            services = new ServiceCollection()
                .AddSingleton(client)
                .AddSingleton(commands)
                .BuildServiceProvider();
            string token = "MTAzNDcxOTM4Nzk3NTk1NDQzMg.GRdYpx.HV15HTgn8wduFbcIkDwic6kE1EFhPbGg2pQo4Y";
            client.Log += client_Log;
            await RegisterCommandsAsync();
            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();
            await Task.Delay(-1);
        }
        private Task client_Log(LogMessage arg)
        {
            Console.WriteLine(arg);
            return Task.CompletedTask;
        }

        public async Task RegisterCommandsAsync()
        {
            client.MessageReceived += HandleCommandAsync;
            await commands.AddModulesAsync(Assembly.GetEntryAssembly(), services);
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;
            var context = new SocketCommandContext(client, message);
            if (message.Author.IsBot)
            {
                return;
            }
            int argPos = 0;
            if (message.HasStringPrefix("!", ref argPos))
            {
                var result = await commands.ExecuteAsync(context, argPos, services);

            }
        }
    }
}
