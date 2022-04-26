using Common;
using Common.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client.ClientCommunicators
{
    public class UdpClientCommunicator : ClientCommunicatorBase
    {
        UdpClient udpClient;
        IPEndPoint iPEndPoint;
        readonly int port = Consts.UdpPort + 1;
        public UdpClientCommunicator(ILogger logger) : base(logger)
        {
            iPEndPoint = new System.Net.IPEndPoint(Consts.IpAddress, Consts.UdpPort);
            udpClient = new UdpClient(port);
        }

        public override string ReadLine()
        {
            string response = null;
            try
            {
                response = Encoding.UTF8.GetString(udpClient.Receive(ref iPEndPoint));
            }
            catch (Exception e)
            {
                logger?.LogError($"[UdpClientCommunicator] Failed to receive data. Exception {e.Message}");
            }
            return response;
        }

        public override void WriteLine(string dataToSend)
        {
            try
            {
                var buffer = Encoding.UTF8.GetBytes($"{dataToSend}\n");
                udpClient.Send(buffer, buffer.Length, iPEndPoint);
            }
            catch (Exception e)
            {
                logger?.LogError($"[UdpClientCommunicator] failed to send data to {iPEndPoint}. Exception: {e.Message}");
            }
        }

        public override void Dispose()
        {
            udpClient.Close();
            udpClient.Dispose();
        }
    }
}
