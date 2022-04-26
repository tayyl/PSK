using Client.ClientCommunicators;
using Common.Logger;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.QAClients
{
    public class PingClient : QAClientBase
    {
        public PingClient(ClientCommunicatorBase clientCommunicator, ILogger logger) : base(clientCommunicator, logger)
        {
        }

        public override string QA(string dataToSend)
        {
            var stopwatch = new Stopwatch();
            var meanTime = 0d;
            const int requestAmount = 10;
            for (var i = 0; i < requestAmount; i++)
            {
                logger?.LogInfo("Sending request to server");

                stopwatch.Restart();
                ClientCommunicator.WriteLine(dataToSend);
                var res = ClientCommunicator.ReadLine();
                stopwatch.Stop();

                meanTime += stopwatch.ElapsedMilliseconds;
                logger?.LogSuccess($"Received response from server: {res} [{stopwatch.ElapsedMilliseconds}ms]");
            }
            return $"Send {requestAmount} requests, mean time to receive response: {meanTime}ms";
        }
    }
}
