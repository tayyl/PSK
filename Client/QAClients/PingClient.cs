using Client.ClientCommunicators;
using Common;
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
        Random random = new Random();
        public PingClient(ClientCommunicatorBase clientCommunicator, ILogger logger) : base(clientCommunicator, logger)
        {
        }

        public override string QA(string dataToSend)
        {
            var stopwatch = new Stopwatch();
            var requestsTimes = new List<long>();
            const int requestAmount = 1;

            var data = dataToSend.Split(' ');
            var clientBytesToSendAmount = data.ElementAtOrDefault(2);
            int.TryParse(clientBytesToSendAmount, out var requestSize);
            var dataToSendToServer = $"{data[0]} {data[1]}";
            if (requestSize > 0 && requestSize > dataToSendToServer.Length)
            {
                dataToSendToServer += $" {random.GenerateRandomText(requestSize - dataToSendToServer.Length - 1)}";
            }

            for (var i = 0; i < requestAmount; i++)
            {
                logger?.LogInfo($"Sending request with size: {dataToSendToServer.Length} to server");

                stopwatch.Restart();
                ClientCommunicator.WriteLine(dataToSendToServer);
                var res = ClientCommunicator.ReadLine();
                stopwatch.Stop();
                requestsTimes.Add(stopwatch.ElapsedMilliseconds);
                logger?.LogSuccess($"Received response from server:  [{stopwatch.ElapsedMilliseconds}ms]");
            }
            return $"Send {requestAmount} requests\n mean time to receive response: {requestsTimes.Average()}ms" +
                   $"\n max time to response: {requestsTimes.Max()}ms" +
                   $"\n min time to response: {requestsTimes.Min()}ms";
        }
    }
}
