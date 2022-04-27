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
    //schowek na pakiety - slownik
    //rozrozniac na \n
    
    //klient powinien miec jedną kolejkę - moze byc lista 
    public class UdpClientCommunicator : ClientCommunicatorBase
    {
        UdpClient udpClient;
        IPEndPoint iPEndPoint;
        const int port = Consts.UdpPort + 1;
        public UdpClientCommunicator(ILogger logger) : base(logger)
        {
            var rand = new Random();
           
            iPEndPoint = new System.Net.IPEndPoint(Consts.IpAddress, Consts.UdpPort);
            udpClient = new UdpClient(port + rand.Next(2, 1000));
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

                //fragmentacja i wysylanie wielu rzeczy na raz
                //konczymy kazdy fragment \n
                //ostatniego nie konczymy \n - server bedzie wiedzial ze ma wszystko
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
