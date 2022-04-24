using Common;
using Common.Enums;
using Common.Logger;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Protocols.TCP {
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
            var remoteEndPoint = (IPEndPoint)tcpClient.Client.RemoteEndPoint;
            port = remoteEndPoint.Port;
            iPAddress = remoteEndPoint.Address;
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
        public void Start(Func<string, string> OnCommand, Action<ICommunicator> OnDisconnect)
        {
            try
            {
                cts = new CancellationTokenSource();
                Task.Factory.StartNew(() =>  ReceiveCommand(OnCommand, OnDisconnect), cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            }
            catch (Exception ex)
            {
                logger?.LogError($"[{ProtocolEnum.tcp}] communicator failed to start. Exception: {ex.Message}");
                OnDisconnect?.Invoke(this);
            }
        }
        public void Send(string data)
        {
            try
            {
                var buffer = Encoding.UTF8.GetBytes($"{data}\n");
                tcpClient.GetStream().Write(buffer, 0, buffer.Length);
            }
            catch (Exception e)
            {
                logger?.LogError($"[{ProtocolEnum.tcp}] failed to send data to {iPAddress}:{port}. Exception: {e.Message}");
            }
        }
        void ReceiveCommand(Func<string,string> OnCommand, Action<ICommunicator> OnDisconnect)
        {
            try
            {
                var reader = new StreamReader(tcpClient.GetStream());
                var writer = new StreamWriter(tcpClient.GetStream());
                var command = string.Empty;
                while (!cts.IsCancellationRequested)
                {
                    command = reader.ReadLine();
                    logger?.LogSuccess($"[{Protocol}] received command from client: {command}");
                    var answer = OnCommand?.Invoke(command);
                    writer.WriteLine(answer);
                    writer.Flush();
                }
                reader.Close();
                writer.Close();
            }
            catch (Exception e)
            {
                logger?.LogError($"[{Protocol}] Failed to receive data. Exception {e.Message}");
                Send(e.Message);
            }
            finally
            {
                OnDisconnect?.Invoke(this);
            }
        }
    }
}