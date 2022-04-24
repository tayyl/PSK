using Common;
using Common.Logger;
using Protocols;
using Protocols.TCP;
using Protocols.UDP;
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
                new UdpListener(Consts.UdpPort, new IPEndPoint(Consts.IpAddress,Consts.UdpPort+1), consoleLogger)
            };
            var server = new Server.Server(listeners, consoleLogger);

            server.Start();

            await Task.Delay(-1);
        }
    }
}