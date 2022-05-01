using Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocols.DotNetRemoting
{
    public class DotNetRemotingListener : IListener
    {
        public ProtocolEnum Protocol => ProtocolEnum.dotnetremoting;

        public void Start(Action<ICommunicator> OnConnect)
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}
