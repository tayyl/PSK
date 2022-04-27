using Client.ClientCommunicators;
using Client.QAClients;
using Common;
using Common.Enums;
using Common.Logger;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Client
{
    public class Client
    {
        readonly ILogger logger;
        ClientCommunicatorBase communicator;
        public Client(ILogger logger)
        {
            this.logger = logger;
        }
        public void Start()
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
                        case CommandEnum.ping:
                        case CommandEnum.ftp:
                        case CommandEnum.chat:
                            ProcessCommand(command.command, command.dataToSend);
                            break;
                        case CommandEnum.protocol:
                            communicator?.Dispose();
                            communicator = PickProtocol(command.protocol);
                            logger?.LogInfo($"{command.protocol} has been picked");
                            break;
                        default:
                            logger?.LogInfo($"Command not recognized");
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
        void ProcessCommand(CommandEnum command, string dataToSend)
        {
            try
            {
                if (communicator == null) { 
                    logger?.LogError("Protocol is not picked, use: protocol NAME, ex. protocol tcp");
                    return;
                }
                QAClientBase qaClient = null;
                var answer = string.Empty;
                switch (command)
                {
                    case CommandEnum.ping:
                        qaClient = new PingClient(communicator, logger);
                        break;
                    case CommandEnum.ftp:
                        qaClient = new FTPClient(communicator, logger);
                        break;
                    case CommandEnum.chat:
                        qaClient = new ChatClient(communicator, logger);
                        break;
                    default:
                        logger?.LogInfo($"Missing service {command}");
                        return;
                }
                logger?.LogSuccess(qaClient.QA(dataToSend));
            }
            catch (Exception ex)
            {
                logger?.LogError($"Processing command failed, exception: {ex.Message}");
            }
        }
        ClientCommunicatorBase PickProtocol(ProtocolEnum protocol)
        {
            switch (protocol)
            {
                case ProtocolEnum.tcp:
                    return new TcpClientCommunicator(logger);
                case ProtocolEnum.udp:
                    return new UdpClientCommunicator(logger);
                default: throw new NotImplementedException();
            }
        }
        (ProtocolEnum protocol, CommandEnum command, string dataToSend) ParseCommands(string userInput)
        {
            var commands = userInput?.Split(' ');
            var commandAsString = commands?.ElementAtOrDefault(0);
            var actionAsString = commands?.ElementAtOrDefault(1);
            var dataToSendAsString = string.Join(" ", commands);

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

    }
}