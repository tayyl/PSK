using Common;
using Common.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.Text;
using System.Threading.Tasks;

namespace Client.ClientCommunicators
{
    public class DotNetRemotingClientCommunicator : ClientCommunicatorBase
    {
        DotNetRemotingObject remoteObject;
        HttpClientChannel channel;
        public DotNetRemotingClientCommunicator(ILogger logger) : base(logger)
        {
            channel = new HttpClientChannel();
            ChannelServices.RegisterChannel(channel, false);

            remoteObject = (DotNetRemotingObject)Activator.GetObject(typeof(DotNetRemotingObject), $@"http://localhost:{Consts.HttpPort}/{Consts.RemoteServiceName}");
        }

        public override void Dispose()
        {
            ChannelServices.UnregisterChannel(channel);
            remoteObject = null;
        }

        public override string QA(string dataToSend)
        {
            return remoteObject.QA(dataToSend);
        }
    }
}
