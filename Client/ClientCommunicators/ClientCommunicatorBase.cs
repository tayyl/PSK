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
        public abstract string QA(string dataToSend);
        public abstract void Dispose();
    }
}
