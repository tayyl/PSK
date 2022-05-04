using Common;
using Common.Enums;
using Common.Logger;
using Protocols;
using Protocols.Filesystem;
using Protocols.RS232;
using Protocols.TCP;
using Protocols.UDP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ServiceModules
{
    //    conf start-service name
    // conf stop-service name
    // conf start-medium name
    // conf stop-medium name
    public class ConfigurationServiceModule : IServiceModule
    {
        public ServiceModuleEnum ServiceModule => ServiceModuleEnum.configuration;
        IList<IServiceModule> serviceModules;
        IList<IListener> listeners;
        Action<ICommunicator> onConnect;
        ILogger logger;
        public ConfigurationServiceModule(IList<IServiceModule> serviceModules, IList<IListener> listeners, Action<ICommunicator> onConnect, ILogger logger)
        {
            this.serviceModules = serviceModules;
            this.listeners = listeners;
            this.onConnect = onConnect;
            this.logger = logger;
        }
        public string AnswerCommand(string command)
        {
            var commands = command.Split(' ');
            var action = commands.ElementAtOrDefault(0);
            var name = commands.ElementAtOrDefault(1);
            switch (action)
            {
                case "start-service":
                    {
                        if (!Enum.TryParse(name, out ServiceModuleEnum serviceModule))
                        {
                            return "Unrecongnized service.";
                        }
                        if (serviceModules.Any(x => x.ServiceModule == serviceModule))
                        {
                            return $"Service {name} is already started";
                        }
                        var added = AddServiceModule(serviceModule);
                        return added ? $"{name} service started" : $"Cannot start {name}";
                    }
                case "stop-service":
                    {
                        if (!Enum.TryParse(name, out ServiceModuleEnum serviceModule))
                        {
                            return "Unrecongnized service.";
                        }
                        if (!serviceModules.Any(x => x.ServiceModule == serviceModule))
                        {
                            return $"Service {name} is already stopped";
                        }
                        var toRemove = serviceModules.First(x => x.ServiceModule == serviceModule);
                        serviceModules.Remove(toRemove);
                        logger?.LogInfo($"{serviceModule} service disabled");
                        return $"{name} service stopped";
                    }
                case "start-medium":
                    {
                        if (!Enum.TryParse(name, out ProtocolEnum protocol))
                        {
                            return "Unrecongnized medium.";
                        }
                        if (listeners.Any(x => x.Protocol == protocol))
                        {
                            return $"Medium {name} is already started";
                        }
                        var added = AddMedium(protocol);
                        return added ? $"{name} medium started successfully" : $"Cannot start {name}";
                    }
                case "stop-medium":
                    {
                        if (!Enum.TryParse(name, out ProtocolEnum protocol))
                        {
                            return "Unrecongnized medium.";
                        }
                        if (!listeners.Any(x => x.Protocol == protocol))
                        {
                            return $"Medium {name} is already stopped";
                        }
                        var toRemove = listeners.First(x => x.Protocol == protocol);
                        toRemove.Stop();
                        listeners.Remove(toRemove);
                        logger?.LogInfo($"{protocol} listener stopped");
                        return $"{name} medium stopped sucessfully";
                    }
                    default: return "Unrecognized command, try: start-service, stop-service, start-medium, stop-medium";
            }
        }
        bool AddServiceModule(ServiceModuleEnum serviceModule)
        {
            switch (serviceModule)
            {
                case ServiceModuleEnum.chat:
                    serviceModules.Add(new ChatServiceModule());
                    break;
                case ServiceModuleEnum.ping:
                    serviceModules.Add(new PingServiceModule());
                    break;
                case ServiceModuleEnum.file:
                    serviceModules.Add(new FileServiceModule());
                    break;
                default:
                    return false;
            }
            logger?.LogInfo($"{serviceModule} service enabled");
            return true;
        }
        bool AddMedium(ProtocolEnum protocol)
        {
            IListener listenerToAdd = null;
            switch (protocol)
            {
                case ProtocolEnum.filesystem:
                    listenerToAdd = new FilesystemListener(logger);
                    break;
                case ProtocolEnum.rs232:
                    listenerToAdd = new RS232Listener(logger);
                    break;
                case ProtocolEnum.tcp:
                    listenerToAdd = new TcpListener(Consts.TcpPort, Consts.IpAddress, logger);
                    break;
                case ProtocolEnum.udp:
                    listenerToAdd = new UdpListener(Consts.UdpPort, new IPEndPoint(Consts.IpAddress, Consts.UdpPort + 1), logger);
                    break;
                default:
                    return false;
            }
            listenerToAdd.Start(onConnect);
            logger?.LogInfo($"{protocol} listener has started");
            listeners.Add(listenerToAdd);
            return true;
        }
    }
}