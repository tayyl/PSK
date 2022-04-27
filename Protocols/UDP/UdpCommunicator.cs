using Common.Enums;
using Common.Logger;
using System;
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
                //fragmentacja
                var buffer = Encoding.UTF8.GetBytes($"{data}\n");
                udpClient.Send(buffer, buffer.Length, iPEndPoint);
            }
            catch (Exception e)
            {
                logger?.LogError($"[{Protocol}] failed to send data to {iPEndPoint}. Exception: {e.Message}");

            }
        }
        void ReceiveCommand(Func<string, string> OnCommand, Action<ICommunicator> OnDisconnect)
        {
            string data = null;
            while (!cts.IsCancellationRequested)
            {
                try
                {
                    
                    data = Encoding.UTF8.GetString(udpClient.Receive(ref iPEndPoint));
                    //jezeli nie konczy sie \n to do kolejki i continue

                    if (data[data.Length - 1] == '\n') continue;

                    //skonczyl sie \n - zlepiamy wiadomosc i obslugujemy
                    //zlepiona wiadomosc usuwana z slownika (concurrent)

                    logger?.LogSuccess($"[{Protocol}] received command from client[{iPEndPoint}]: {data}");
                    var res = OnCommand.Invoke(data);
                    Send(res);
                }
                catch (Exception e)
                {
                    logger?.LogError($"[{Protocol}] Failed to receive data. Exception {e.Message}");
                    Send(e.Message);
                }
            }
            OnDisconnect?.Invoke(this);
        }
    }
}