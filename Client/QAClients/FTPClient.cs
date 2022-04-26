using Client.ClientCommunicators;
using Common.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.QAClients
{
    public class FTPClient : QAClientBase
    {
        public FTPClient(ClientCommunicatorBase clientCommunicator, ILogger logger) : base(clientCommunicator, logger)
        {
        }

        public override string QA(string dataToSend)
        {
            throw new NotImplementedException();
        }
    }
}
