﻿using Common.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ClientCommunicators
{
    public class DotNetRemotingClientCommunicator : ClientCommunicatorBase
    {
        public DotNetRemotingClientCommunicator(ILogger logger) : base(logger)
        {
        }

        public override void Dispose()
        {
            throw new NotImplementedException();
        }

        public override string ReceiveResponse()
        {
            throw new NotImplementedException();
        }

        public override void SendRequest(string dataToSend)
        {
            throw new NotImplementedException();
        }
    }
}
