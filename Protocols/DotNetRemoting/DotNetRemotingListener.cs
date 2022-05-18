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
            try
            {
                OnConnect?.Invoke(new DotNetRemotingCommunicator(logger));
            }
            catch (Exception ex)
            {
                logger?.LogError($"[{Protocol}] failed to start .NetRemoting listener: {ex.Message}");
            }
        }

        public void Stop()
        {

        }
    }
}
