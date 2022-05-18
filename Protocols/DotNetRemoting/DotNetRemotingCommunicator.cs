using Common;
using Common.Enums;
using Common.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocols.DotNetRemoting
{
    public class DotNetRemotingCommunicator : ICommunicator
    {
        public ProtocolEnum Protocol => ProtocolEnum.dotnetremoting;
        ILogger logger;
        DotNetRemotingObject remoteObject;
        public DotNetRemotingCommunicator(ILogger logger)
        {
            this.logger = logger;
        }
        public void Start(Func<string, string> OnCommand, Action<ICommunicator> OnDisconnect)
        {
            try
            {
                remoteObject = new DotNetRemotingObject(OnCommand);
            }catch (Exception ex)
            {
                OnDisconnect?.Invoke(this);
            }
        }

        public void Stop()
        {
            try
            {

            }catch (Exception ex)
            {

            }
        }
    }
}
