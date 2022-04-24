using Common;
using Common.Enums;
using Common.Logger;
using Protocols.Common;
using ServiceModules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server {
    public class Server
    {
        readonly ILogger logger;
        List<ICommunicator> communicators = new List<ICommunicator>();
        List<IServiceModule> services = new List<IServiceModule>()
        {
            new PingServiceModule()
        };
        readonly List<IListener> listeners;
        public Server(List<IListener> listeners, ILogger logger)
        {
            this.logger = logger;
            this.listeners = listeners;
        }
        public void Start()
        {
            foreach (var listener in listeners)
            {
                listener.Start(OnConnect);
                logger.LogInfo($"{listener.Protocol} listener has started");
            }
            logger.LogInfo("Server started");
        }
        //to powinienem przekazac do komunikatora

        public async Task OnCommand(ICommunicator communicator, string data)
        {
            logger?.LogSuccess($"[{communicator.Protocol}] received command from client: {data}");
            var serviceAsString = data.Split(' ').ElementAtOrDefault(0);
            var answer = string.Empty;
            if (!Enum.TryParse(serviceAsString, out ServiceModuleEnum serviceModule))
            {
                answer = $"Unknown serivce: {serviceAsString}";
            }
            else
            {
                var service = services.FirstOrDefault(x => x.ServiceModule == serviceModule);
                if (service == null)
                {
                    answer = $"Required service is not turned on: {serviceModule}";
                }
                else
                {

                    answer = service.AnswerCommand(data.Split(' ').ElementAtOrDefault(1));
                }
            }
            await communicator.Send(answer);
        }
        public void Stop()
        {
            foreach (var listener in listeners)
            {
                listener.Stop();
            }
            foreach (var communicator in communicators)
            {
                communicator.Stop();
            }
            logger?.LogInfo("Server stopped");
        }
        void OnConnect(ICommunicator communicator)
        {
            logger?.LogInfo($"[{communicator.Protocol}] client connected");
            communicators.Add(communicator);
            communicator.Start(OnCommand, OnDisconnect);
        }
        void OnDisconnect(ICommunicator communicator)
        {
            logger?.LogInfo($"[{communicator.Protocol}] client disconnected");
            communicator.Stop();
            communicators.Remove(communicator);
        }
    }
}