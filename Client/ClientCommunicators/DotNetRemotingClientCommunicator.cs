using Common;
using Common.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;

namespace Client.ClientCommunicators
{
    public class DotNetRemotingClientCommunicator : ClientCommunicatorBase
    {
        DotNetRemotingObject remoteObject;
        public DotNetRemotingClientCommunicator(ILogger logger) : base(logger)
        {
            RemotingConfiguration.Configure("RemotingClient.xml");
            
            remoteObject = (DotNetRemotingObject)Activator.GetObject(typeof(DotNetRemotingObject), "http://localhost:12343/RemoteObject");
        }

        public override void Dispose()
        {
            try
            {
                
            }catch(Exception ex)
            {

            }
        }

        public override string QA(string dataToSend)
        {
            return remoteObject.QA(dataToSend);
        }
    }
}
