using Client.ClientCommunicators;
using Common.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.QAClients
{
    public abstract class QAClientBase
    {
        protected readonly ILogger logger;
        protected ClientCommunicatorBase ClientCommunicator;
        public QAClientBase(ClientCommunicatorBase clientCommunicator, ILogger logger)
        {
            this.logger = logger;
            ClientCommunicator = clientCommunicator;
        }
        public abstract string QA(string dataToSend);
    }
}
