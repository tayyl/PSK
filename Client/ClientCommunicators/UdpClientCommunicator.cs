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
        MessageQueue<string, string> queue;
        public UdpClientCommunicator(ILogger logger) : base(logger)
        {
            queue = new MessageQueue<string, string>();
            var rand = new Random();

            iPEndPoint = new System.Net.IPEndPoint(Consts.IpAddress, Consts.UdpPort);
            udpClient = new UdpClient(Consts.UdpPort + rand.Next(2, 1000));
        }

        public override string ReadLine()
        {
            var result = string.Empty;
            try
            {
                var response = string.Empty;
                do
                {
                    response = Encoding.UTF8.GetString(udpClient.Receive(ref iPEndPoint));
                    queue.Add(iPEndPoint.ToString(), response);
                } while (response.Last() != '\n');
                result = queue.GetAllMessages(iPEndPoint.ToString());
                queue.RemoveAllMessages(iPEndPoint.ToString());
            }
            catch (Exception e)
            {
                logger?.LogError($"[UdpClientCommunicator] Failed to receive data. Exception {e.Message}");
            }
            return result;
        }

        public override void WriteLine(string dataToSend)
        {
            try
            {
                var buffer = Encoding.UTF8.GetBytes($"{dataToSend}\n").AsSpan();
                var lastIndex = 0;

                do
                {
                    var fragmentBuffer = buffer.Slice(lastIndex, Math.Min(buffer.Length - lastIndex, Consts.DatagramSize + lastIndex)).ToArray();
                    udpClient.Send(fragmentBuffer, fragmentBuffer.Length, iPEndPoint);
                    lastIndex += Consts.DatagramSize;
                } while (lastIndex < buffer.Length);
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
