using Common.Enums;
using Common.Logger;
using Protocols.Common;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Protocols.UDP;
public class UdpCommunicator : ICommunicator
{
    public ProtocolEnum Protocol => ProtocolEnum.udp;

    CancellationTokenSource cts;
    readonly ILogger logger;
    UdpClient udpClient;
    readonly IPEndPoint iPEndPoint;
    readonly int port;
    public UdpCommunicator(int port, IPEndPoint iPEndPoint, ILogger logger)
    {
        this.logger = logger;
        this.iPEndPoint = iPEndPoint;
        this.port = port;
    }
    public void Stop()
    {
        try
        {
            cts?.Cancel();
            udpClient.Close();
        }
        catch (Exception ex)
        {
            logger?.LogError($"[{Protocol}] communicator failed to stop. Exception: {ex.Message}");
        }
    }

    public async Task Start(Func<ICommunicator, string, Task> OnCommand, Action<ICommunicator> OnDisconnect)
    {
        try
        {
            cts = new CancellationTokenSource();
            udpClient = new UdpClient(port);
            await Task.Factory.StartNew(async () => await ReceiveCommand(OnCommand, OnDisconnect), cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }
        catch (Exception ex)
        {
            logger?.LogError($"[{Protocol}] communicator failed to start. Exception: {ex.Message}");
            OnDisconnect?.Invoke(this);
        }
    }
    public async Task Send(string data)
    {
        try
        {
            var buffer = Encoding.UTF8.GetBytes($"{data}\n");
            await udpClient.SendAsync(buffer, buffer.Length, iPEndPoint);
        }
        catch (Exception e)
        {
            logger?.LogError($"[{Protocol}] failed to send data to {iPEndPoint}. Exception: {e.Message}");
        }
    }
    async Task ReceiveCommand(Func<ICommunicator, string, Task> OnCommand, Action<ICommunicator> OnDisconnect)
    {
        while (!cts.IsCancellationRequested)
        {
            try
            {
                var data = await udpClient.ReceiveAsync();
                if (OnCommand != null)
                {
                    await OnCommand.Invoke(this, Encoding.UTF8.GetString(data.Buffer));
                }
            }
            catch (Exception e)
            {
                logger?.LogError($"[{Protocol}] Failed to receive data. Exception {e.Message}");
            }
        }
        OnDisconnect?.Invoke(this);
    }
}
