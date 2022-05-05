using Client.ClientCommunicators;
using Common.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.QAClients
{
    public class ConfigurationClient : QAClientBase
    {
        public ConfigurationClient(ClientCommunicatorBase clientCommunicator, ILogger logger) : base(clientCommunicator, logger)
        {
        }

        public override string QA(string dataToSend)
        {
            ClientCommunicator.WriteLine(dataToSend);
            return ClientCommunicator.ReadLine();
        }
    }
}
