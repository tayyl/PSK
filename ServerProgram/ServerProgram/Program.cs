using Common;
using Common.Logger;
using Protocols.Common;
using Protocols.TCP;

var consoleLogger = new ConsoleLogger();
var listeners = new List<IListener>
{
    new TcpListener(Consts.TcpPort, Consts.IpAddress, consoleLogger)
};
var server = new Server.Server(listeners, consoleLogger);

server.Start();

await Task.Delay(-1);