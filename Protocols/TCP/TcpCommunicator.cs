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

namespace Protocols.TCP;
public class TcpCommunicator : ICommunicator
{
    public ProtocolEnum Protocol => ProtocolEnum.tcp;

    CancellationTokenSource cts;
    readonly ILogger logger;
    readonly TcpClient tcpClient;
    readonly IPAddress iPAddress;
    readonly int port;
    public TcpCommunicator(TcpClient tcpClient, ILogger logger)
    {
        this.logger = logger;
        this.tcpClient = tcpClient;
        var remoteEndPoint = (IPEndPoint)tcpClient.Client.RemoteEndPoint!;
        port = remoteEndPoint.Port;
        iPAddress = remoteEndPoint.Address;
    }
    public TcpCommunicator(IPAddress iPAddress, int port, ILogger logger)
    {
        this.logger = logger;
        this.iPAddress = iPAddress;
        this.port = port;
        tcpClient = new TcpClient();
    }
    public void Stop()
    {
        try
        {
            cts?.Cancel();
            tcpClient.Close();
        }
        catch (Exception ex)
        {
            logger?.LogError($"[{Protocol}] communicator failed to stop. Exception: {ex.Message}");
        }
    }
    public async Task Start(Func<ICommunicator, string, Task<string>> OnCommand, Action<ICommunicator> OnDisconnect)
    {
        try
        {
            cts = new CancellationTokenSource();
            if (!tcpClient.Connected) 
            {
                await tcpClient.ConnectAsync(iPAddress, port); 
            }

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
            await tcpClient.GetStream().WriteAsync(buffer, cts.Token);
        }
        catch (Exception e)
        {
            logger?.LogError($"[{Protocol}] failed to send data to {iPAddress}:{port}. Exception: {e.Message}");
        }
    }
    async Task ReceiveCommand(Func<ICommunicator, string, Task<string>> OnCommand, Action<ICommunicator> OnDisconnect)
    {
        try
        {
            var reader = PipeReader.Create(tcpClient.GetStream());
            while (!cts.IsCancellationRequested)
            {
                var result = await reader.ReadAsync(cts.Token);
                if (result.IsCompleted)
                {
                    break;
                }
                var buffer = result.Buffer;

                while (TryReadLine(ref buffer, out var line))
                {
                    var stringBuilder = new StringBuilder();
                    foreach (var segment in line)
                    {
                        stringBuilder.Append(Encoding.UTF8.GetString(segment.Span.ToArray()));
                    }
                    var answer = await OnCommand.Invoke(this, stringBuilder.ToString());
                    //tutaj powinienem wyslac odpowiedz po oncommand
                    //await Send(answer); ??: jezeli tutaj bede zwracal to wpadne w petle, bo klient i serwer korzystaja z tego samego komunikatora
                }

                reader.AdvanceTo(buffer.Start, buffer.End);
            }
            await reader.CompleteAsync();
        }
        catch (Exception e)
        {
            logger?.LogError($"[{Protocol}] Failed to receive data. Exception {e.Message}");
        }
        finally
        {
            OnDisconnect?.Invoke(this);
        }
    }
    bool TryReadLine(ref ReadOnlySequence<byte> buffer, out ReadOnlySequence<byte> line)
    {
        var position = buffer.PositionOf((byte)'\n');
        if (!position.HasValue)
        {
            line = default;
            return false;
        }

        line = buffer.Slice(0, position.Value);
        buffer = buffer.Slice(buffer.GetPosition(1, position.Value));
        return true;
    }
}
