using Common.Logger;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ClientCommunicators
{
    public abstract class ClientCommunicatorBase : IDisposable
    {
        protected readonly ILogger logger;
        public ClientCommunicatorBase(ILogger logger)
        {
            this.logger = logger;
        }
        public abstract void SendRequest(string dataToSend);
        public abstract string ReceiveResponse();
        public string Ping(string dataToSend)
        {
            var stopwatch = new Stopwatch();
            var meanTime = 0d;
            const int requestAmount = 10;
            for (var i = 0; i < requestAmount; i++)
            {
                logger?.LogInfo("Sending request to server");
                stopwatch.Restart();
                SendRequest(dataToSend);
                var res = ReceiveResponse();
                stopwatch.Stop();
                meanTime += stopwatch.ElapsedMilliseconds;
                logger?.LogSuccess($"Received response from server: {res} [{stopwatch.ElapsedMilliseconds}ms]");
            }
            return $"Send {requestAmount} requests, mean time to receive response: {meanTime}ms";
        }
        public string FTP(string dataToSend)
        {
            throw new NotImplementedException();
        }
        public string Configuration(string dataToSend)
        {
            throw new NotImplementedException();
        }
        public string Chat(string dataToSend)
        {
            SendRequest(dataToSend);
            return ReceiveResponse();
        }

        public abstract void Dispose();
    }
}
