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
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace DiscordBot
{
    public class Program
    {
        static void Main(string[] args) => new Program().RunBotAsync().GetAwaiter().GetResult();

        public string getData()
        {
            string stream = File.OpenText(Directory.GetCurrentDirectory() + "\\config.json").ReadToEnd();
            //这里需要把config.json放在了与生成出来的.exe文件相同的位置
            Data data = JsonConvert.DeserializeObject<Data>(stream);
            string[] str = stream.Split('"');
            string token = str[str.Length - 2];
            //莫名起码地反序列化出整个json文本，被迫采用这种方法，之后再改
            Console.WriteLine("获取到的token：" + token);
            return token;
        }

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

            string token = "";
            token = getData();
            //由于之前不小心泄露token了
            //把token存放在config.json里，同时在.gitignore中添加了相关内容
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
