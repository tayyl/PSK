using Common;
using Common.Enums;
using Common.Logger;
using Protocols.Common;
using Protocols.TCP;
using Protocols.UDP;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Client;
public class Client
{
    //w tasku powinna byc analiza komunikacji z jednym klientem, a nie kazdego polecenia
    //raczej w ramach jednego "polaczenia" powinnismy wysylac jedno - lepiej czekac
    //klient powinien byc prosty 
    //testy danej uslugi - np. dla pinga 10 testow, na jednym protokole + statystyki
    //kazda funkcja testujaca powinna byc prosta i jednowatkowa - dsotaje medium 
    //rozdzielic protokol od tego co ma robic, czyli np. tcp "enter" i wtedy dzialamy w ramach tcp
    readonly ILogger logger;
    readonly Stopwatch stopwatch;
    ICommunicator communicator;
    public Client(ILogger logger)
    {
        stopwatch = new Stopwatch();
        this.logger = logger;
    }
    public async Task Start()
    {
        logger.LogInfo("Client started");
        logger?.LogInfo("Type 'help' to list commands");
        while (true)
        {
            try
            {
                logger?.LogInfo("Type your command");
                var userInput = Console.ReadLine();
                var command = ParseCommands(userInput);
                switch (command.command)
                {
                    case CommandEnum.help:
                        ShowHelp();
                        break;
                    case CommandEnum.test:
                        if(communicator == null)
                        {
                            logger?.LogInfo("Communicator is not picked, type: protocol protocolName");
                            break;
                        }
                        await TestProtocol(command.dataToSend);
                        break;
                    case CommandEnum.protocol:
                        communicator = PickCommunicator(command.protocol);
                        await communicator.Start(OnCommand, OnDisconnect);
                        logger?.LogInfo($"{communicator.Protocol} has been picked");
                        break;
                }
            }
            catch (Exception ex)
            {
                logger?.LogError($"{ex.Message}");
                logger?.LogInfo("Type 'help' to list commands");
            }
        }
    }
    async Task TestProtocol(string dataToSend)
    {
        for(var i = 0; i < 10; i++)
        {
            stopwatch.Restart();
            await communicator.Send(dataToSend);
        }
    }
    ICommunicator PickCommunicator(ProtocolEnum protocol)
    {
        switch (protocol)
        {
            case ProtocolEnum.tcp:
                return new TcpCommunicator(Consts.IpAddress, Consts.TcpPort, logger);
            case ProtocolEnum.udp:
                return new UdpCommunicator(Consts.UdpPort + 1, new System.Net.IPEndPoint(Consts.IpAddress, Consts.UdpPort), logger);
            default: throw new NotImplementedException();
        }
    }
    (ProtocolEnum protocol, CommandEnum command, string dataToSend) ParseCommands(string userInput)
    {
        var commands = userInput?.Split(' ');
        var commandAsString = commands?.ElementAtOrDefault(0);
        var actionAsString = commands?.ElementAtOrDefault(1);
        var dataToSendAsString = string.Join(' ', commands?.Skip(1));

        if (Enum.TryParse(actionAsString, out CommandEnum help) && help == CommandEnum.help)
        {
            return (default, CommandEnum.help, default);
        }
        
        if (!Enum.TryParse(commandAsString, out CommandEnum command))
        {
            throw new Exception($"Command not recognized");
        }
        var protocol = ProtocolEnum.none;
        if (command == CommandEnum.protocol && !Enum.TryParse(actionAsString, out protocol))
        {
            throw new Exception($"Protocol not recognized");
        }
        return (protocol, command, dataToSendAsString);
    }
    void ShowHelp()
    {
        logger?.LogInfo("Using the client: protocol protocolName");
        logger?.LogInfo("When protocol is picked: command service data");
        logger?.LogInfo("ex. protocol tcp");
        logger?.LogInfo("ex. test ping 1024");

        var availableCommands = string.Join(", ", Enum.GetValues(typeof(CommandEnum)).Cast<CommandEnum>().Select(x => x.ToString()));
        logger?.LogInfo($"Available commands: {availableCommands}");

        var availableProtocols = string.Join(", ", Enum.GetValues(typeof(ProtocolEnum)).Cast<ProtocolEnum>().Select(x => x.ToString()));
        logger?.LogInfo($"Available protocols: {availableProtocols}");

        var availableServices = string.Join(", ", Enum.GetValues(typeof(ServiceModuleEnum)).Cast<ServiceModuleEnum>().Select(x => x.ToString()));
        logger?.LogInfo($"Available services: {availableServices}");
    }
    
    async Task<string> OnCommand(ICommunicator communicator, string data)
    {
        stopwatch.Stop();
        logger?.LogSuccess($"[{communicator.Protocol}] received answer from server:{data} ({stopwatch.ElapsedMilliseconds}ms)");
        return string.Empty;//??
    }
    void OnDisconnect(ICommunicator communicator)
    {
        logger?.LogInfo($"[{communicator.Protocol}] disconnected.");
    }
}
