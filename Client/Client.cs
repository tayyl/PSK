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
    //jak liczyc czas odpowiedzi - dla kazdego utworzonego taska tworzyc stopwatch?
    readonly ILogger logger;
    readonly List<ICommunicator> communicators = new List<ICommunicator>();
    public Client(ILogger logger)
    {
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
                    case CommandEnum.send:
                        await Send(command.protocol, command.dataToSend);
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
    (ProtocolEnum protocol, CommandEnum command, string dataToSend) ParseCommands(string userInput)
    {
        var commands = userInput?.Split(' ');
        var protocolAsString = commands?.ElementAtOrDefault(0);
        var commandAsString = commands?.ElementAtOrDefault(1);
        var dataToSendAsString = string.Join(' ', commands?.Skip(2));

        if (Enum.TryParse(protocolAsString, out CommandEnum help) && help == CommandEnum.help)
        {
            return (default, CommandEnum.help, default);
        }

        if (!Enum.TryParse(protocolAsString, out ProtocolEnum protocol))
        {
            throw new Exception($"Protocol not recognized");
        }
        if (!Enum.TryParse(commandAsString, out CommandEnum command))
        {
            throw new Exception($"Command not recognized");
        }

        return (protocol, command, dataToSendAsString);
    }
    void ShowHelp()
    {
        logger?.LogInfo("Using the client: protocol command service dataToSend");
        logger?.LogInfo("ex. tcp send ping 1024");

        var availableCommands = string.Join(", ", Enum.GetValues(typeof(CommandEnum)).Cast<CommandEnum>().Select(x => x.ToString()));
        logger?.LogInfo($"Available commands: {availableCommands}");

        var availableProtocols = string.Join(", ", Enum.GetValues(typeof(ProtocolEnum)).Cast<ProtocolEnum>().Select(x => x.ToString()));
        logger?.LogInfo($"Available protocols: {availableProtocols}");

        var availableServices = string.Join(", ", Enum.GetValues(typeof(ServiceModuleEnum)).Cast<ServiceModuleEnum>().Select(x => x.ToString()));
        logger?.LogInfo($"Available services: {availableServices}");
    }
    async Task Send(ProtocolEnum protocol, string dataToSend)
    {
        switch (protocol)
        {
            case ProtocolEnum.tcp:
                if (communicators.FirstOrDefault(x => x.Protocol == ProtocolEnum.tcp) is not TcpCommunicator tcpCommunicator)
                {
                    tcpCommunicator = new TcpCommunicator(Consts.IpAddress, Consts.TcpPort, logger);
                    communicators.Add(tcpCommunicator);
                    await tcpCommunicator.Start(OnCommand, OnDisconnect);
                }

                await tcpCommunicator.Send(dataToSend);
                break;
            case ProtocolEnum.udp:
                if (communicators.FirstOrDefault(x => x.Protocol == ProtocolEnum.udp) is not UdpCommunicator udpCommunicator)
                {
                    udpCommunicator = new UdpCommunicator(Consts.UdpPort + 1, new System.Net.IPEndPoint(Consts.IpAddress, Consts.UdpPort), logger);
                    communicators.Add(udpCommunicator);
                    await udpCommunicator.Start(OnCommand, OnDisconnect);
                }
                await udpCommunicator.Send(dataToSend);
                break;
        }
    }
    Task OnCommand(ICommunicator communicator, string data)
    {
        logger?.LogSuccess($"[{communicator.Protocol}] received answer from server:{data}");
        return Task.CompletedTask;
    }
    void OnDisconnect(ICommunicator communicator)
    {
        logger?.LogInfo($"[{communicator.Protocol}] disconnected.");
    }
}
