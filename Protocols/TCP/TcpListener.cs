using Common.Enums;
using Common.Logger;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Protocols.TCP
{
    public class TcpListener : IListener
    {
        public ProtocolEnum Protocol => ProtocolEnum.tcp;

        readonly ILogger logger;
        System.Net.Sockets.TcpListener listener;
        readonly int port;
        readonly IPAddress ipAddress;
        CancellationTokenSource cts;
        public TcpListener(int port, IPAddress ipAddress, ILogger logger)
        {
            this.logger = logger;
            this.port = port;
            this.ipAddress = ipAddress;
        }
        public void Start(Action<ICommunicator> OnConnect)
        {
            try
            {
                listener = new System.Net.Sockets.TcpListener(ipAddress, port);
                listener.Start();
                cts = new CancellationTokenSource();

                void CommandListener()
                {
                    while (!cts.IsCancellationRequested)
                    {
                        try
                        {
                            var client = listener.AcceptTcpClient();
                            var tcpCommunicator = new TcpCommunicator(client, logger);

                            OnConnect(tcpCommunicator);
                        }
                        catch (Exception e)
                        {
                            logger.LogError($"[{Protocol}] running failed. Exception: {e.Message}");
                        }
                    }
                }

                Task.Factory.StartNew(CommandListener, cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
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
                listener.Stop();
                cts.Cancel();
            }
            catch (Exception ex)
            {
                logger.LogError($"[{Protocol}] listener stop failed. Exception: {ex.Message}");
            }
        }
    }
}