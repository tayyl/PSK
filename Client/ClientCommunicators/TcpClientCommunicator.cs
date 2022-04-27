using Common;
using Common.Logger;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client.ClientCommunicators
{
    public class TcpClientCommunicator : ClientCommunicatorBase
    {

        readonly TcpClient tcpClient;
        readonly IPAddress iPAddress = Consts.IpAddress;
        readonly int port = Consts.TcpPort;

        public TcpClientCommunicator(ILogger logger) : base(logger)
        {
            tcpClient = new TcpClient();
            tcpClient.Connect(iPAddress, port);
        }

        public override void Dispose()
        {
            tcpClient.Close();
            tcpClient.Dispose();
        }

        public override string ReadLine() 
        {
            string response = null;
            try
            {

                var responseBytes = new byte[1024];
                var bytes = 0;
                var stream = tcpClient.GetStream();
                do
                {
                    bytes = stream.Read(responseBytes, 0, responseBytes.Length);
                    response += Encoding.ASCII.GetString(responseBytes, 0, bytes);
                }
                while (stream.DataAvailable);

            }
            catch (Exception e)
            {
                logger?.LogError($"[TcpClientCommunicator] Failed to receive data. Exception {e.Message}");
            }
            return response;
        }

        public override void WriteLine(string dataToSend)
        {
            try
            {
                var buffer = Encoding.UTF8.GetBytes($"{dataToSend}\n");
                tcpClient.GetStream().Write(buffer, 0, buffer.Length);
            }
            catch (Exception e)
            {
                logger?.LogError($"[TcpClientCommunicator] failed to send data to {iPAddress}:{port}. Exception: {e.Message}");
            }
        }
    }
}
