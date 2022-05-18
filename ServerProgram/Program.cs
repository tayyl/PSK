using Common;
using Common.Logger;
using Protocols;
using Protocols.DotNetRemoting;
using Protocols.Filesystem;
using Protocols.RS232;
using Protocols.TCP;
using Protocols.UDP;
using Server;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace ServerProgram
{

    public class Program
    {

        public static async Task Main(string[] args)
        {
            var consoleLogger = new ConsoleLogger();
            var listeners = new List<IListener>
            {
                new TcpListener(Consts.TcpPort, Consts.IpAddress, consoleLogger),
                new UdpListener(Consts.UdpPort, new IPEndPoint(Consts.IpAddress,Consts.UdpPort+1), consoleLogger),
                new RS232Listener(consoleLogger,"COM1"),
                new FilesystemListener(consoleLogger),
                new DotNetRemotingListener(consoleLogger)
            };
            var server = new Server.Server(listeners, consoleLogger);

            server.Start();

            await Task.Delay(-1);
        }
    }
}