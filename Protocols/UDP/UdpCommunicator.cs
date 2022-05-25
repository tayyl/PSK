using Common;
using Common.Enums;
using Common.Logger;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Protocols.UDP
{
    public class UdpCommunicator : ICommunicator
    {
        public ProtocolEnum Protocol => ProtocolEnum.udp;

        CancellationTokenSource cts;
        readonly ILogger logger;
        UdpClient udpClient;
        IPEndPoint iPEndPoint;
        readonly int port;
        MessageQueue<string, string> queue;
        public UdpCommunicator(int port, IPEndPoint iPEndPoint, ILogger logger)
        {
            this.logger = logger;
            this.iPEndPoint = iPEndPoint;
            this.port = port;
            queue = new MessageQueue<string, string>();
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

        public void Start(Func<string, string> OnCommand, Action<ICommunicator> OnDisconnect)
        {
            try
            {
                cts = new CancellationTokenSource();
                udpClient = new UdpClient(port);
                Task.Factory.StartNew(() => ReceiveCommand(OnCommand, OnDisconnect), cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            }
            catch (Exception ex)
            {
                logger?.LogError($"[{Protocol}] communicator failed to start. Exception: {ex.Message}");
                OnDisconnect?.Invoke(this);
            }
        }
        public void Send(string data)
        {
            try
            {
                var buffer = Encoding.UTF8.GetBytes($"{data}\n").AsSpan();
                var lastIndex = 0;

                do
                {
                    var fragmentBuffer = buffer.Slice(lastIndex, Math.Min(buffer.Length - lastIndex, Consts.DatagramSize)).ToArray();
                    udpClient.Send(fragmentBuffer, fragmentBuffer.Length, iPEndPoint);
                    lastIndex += Consts.DatagramSize;
                } while (lastIndex < buffer.Length);
            }
            catch (Exception e)
            {
                logger?.LogError($"[{Protocol}] failed to send data to {iPEndPoint}. Exception: {e.Message}");

            }
        }
        void ReceiveCommand(Func<string, string> OnCommand, Action<ICommunicator> OnDisconnect)
        {
            while (!cts.IsCancellationRequested)
            {
                try
                {
                    var response = string.Empty;
                    do
                    {
                        response = Encoding.UTF8.GetString(udpClient.Receive(ref iPEndPoint));
                        queue.Add(iPEndPoint.ToString(), response);
                    } while (response.Last() != '\n');

                    var result = queue.GetAllMessages(iPEndPoint.ToString());
                    queue.RemoveAllMessages(iPEndPoint.ToString());

                    logger?.LogSuccess($"[{Protocol}] received command from client[{iPEndPoint}]: ");
                    var res = OnCommand.Invoke(result);
                    Send(res);
                }
                catch (SocketException e)
                {
                    if(e.SocketErrorCode != SocketError.Interrupted)
                    logger?.LogError($"[{Protocol}] Failed to receive data. Exception {e.Message}");
                }
                catch (Exception e)
                {
                    logger?.LogError($"[{Protocol}] Failed to receive data. Exception {e.Message}");
                }
            }
            OnDisconnect?.Invoke(this);
        }
    }
}