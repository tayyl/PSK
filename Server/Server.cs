using Common;
using Common.Enums;
using Common.Logger;
using Protocols.Common;
using ServiceModules;

namespace Server;
public class Server
{
    readonly ILogger logger;
    List<ICommunicator> communicators = new();
    List<IServiceModule> services = new()
    {
        new PingServiceModule()
    };
    readonly List<IListener> listeners;
    CancellationTokenSource cts;
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
        cts = new CancellationTokenSource();
        logger.LogInfo("Server started");
    }
    async Task<string> AnswerCommand(string data)
    {
        var serviceAsString = data.Split(' ').ElementAtOrDefault(0);

        if(!Enum.TryParse(serviceAsString, out ServiceModuleEnum serviceModule))
        {
            return $"Unknown serivce: {serviceAsString}";
        }
        var service = services.FirstOrDefault(x => x.ServiceModule == serviceModule);
        if(service == null)
        {
            return $"Missing service {serviceModule}";
        }

        return await service.AnswerCommand(data.Split(' ').ElementAtOrDefault(1));
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
    async Task OnCommand(ICommunicator communicator, string command)
    {
        logger?.LogSuccess($"[{communicator.Protocol}] received command from client: {command}");
        await communicator.Send(await AnswerCommand(command));
    }
}
