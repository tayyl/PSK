using Common.Enums;
using Common.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;

namespace Protocols.DotNetRemoting
{
    public class DotNetRemotingListener : IListener
    {
        public ProtocolEnum Protocol => ProtocolEnum.dotnetremoting;
        readonly ILogger logger;
        public DotNetRemotingListener(ILogger logger)
        {
            this.logger = logger;
        }
        public void Start(Action<ICommunicator> OnConnect)
        {
            RemotingConfiguration.Configure("RemotingServer.xml");
            OnConnect?.Invoke(new DotNetRemotingCommunicator(logger));

        }

        public void Stop()
        {
            try
            {
                
            }catch(Exception e)
            {

            }
        }
    }
}
