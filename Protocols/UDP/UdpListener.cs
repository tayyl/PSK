using Common.Enums;
using Common.Logger;
using Protocols.Common;
using System.Net;
using System.Net.Sockets;

namespace Protocols.UDP;
public class UdpListener : IListener
{
    public ProtocolEnum Protocol => ProtocolEnum.udp;

    readonly ILogger logger;
    readonly int port;
    CancellationTokenSource cts;
    IPEndPoint remoteIpEndpoint;
    public UdpListener(int port, IPEndPoint iPEndPoint, ILogger logger)
    {
        this.logger = logger;
        this.port = port;
        this.remoteIpEndpoint = iPEndPoint;
    }
    public void Start(Action<ICommunicator> OnConnect)
    {
        try
        {
            var udpCommunicator = new UdpCommunicator(port, remoteIpEndpoint, logger);

            OnConnect(udpCommunicator);
        }
        catch (Exception ex)
        {
            logger.LogError($"[{Protocol}] listener start failed. Exception: {ex.Message}");
        }
    }
    
    public void Stop()
    {
        try
        {
            cts?.Cancel();
        }
        catch (Exception ex)
        {
            logger.LogError($"[{Protocol}] listener stop failed. Exception: {ex.Message}");
        }
    }
}
