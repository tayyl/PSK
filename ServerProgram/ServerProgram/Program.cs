using Common;
using Common.Logger;
using Protocols.Common;
using Protocols.TCP;
using Protocols.UDP;
using System.Net;

var consoleLogger = new ConsoleLogger();
var listeners = new List<IListener>
{
    new Protocols.TCP.TcpListener(Consts.TcpPort, Consts.IpAddress, consoleLogger),
    new UdpListener(Consts.UdpPort, new IPEndPoint(Consts.IpAddress,Consts.UdpPort+1), consoleLogger)
};
var server = new Server.Server(listeners, consoleLogger);

server.Start();

await Task.Delay(-1);