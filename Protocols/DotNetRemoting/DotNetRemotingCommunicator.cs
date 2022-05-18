using Common;
using Common.Enums;
using Common.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.Text;
using System.Threading.Tasks;

namespace Protocols.DotNetRemoting
{
    public class DotNetRemotingCommunicator : ICommunicator
    {
        public ProtocolEnum Protocol => ProtocolEnum.dotnetremoting;
        ILogger logger;
        HttpChannel httpChannel;
        DotNetRemotingObject remotingObject;
        public DotNetRemotingCommunicator(ILogger logger)
        {
            this.logger = logger;
        }
        public void Start(Func<string, string> OnCommand, Action<ICommunicator> OnDisconnect)
        {
            try
            {
                httpChannel = new HttpChannel(Consts.HttpPort);
                ChannelServices.RegisterChannel(httpChannel, ensureSecurity: false);

                remotingObject = new DotNetRemotingObject(OnCommand, logger);
                RemotingServices.Marshal(remotingObject, Consts.RemoteServiceName);
            }
            catch (Exception ex)
            {
                OnDisconnect?.Invoke(this);
            }
        }

        public void Stop()
        {
            try
            {
                RemotingServices.Disconnect(remotingObject);
                ChannelServices.UnregisterChannel(httpChannel);
            }
            catch (Exception ex)
            {
                logger.LogError($"[{Protocol}] communicator stop failed. Exception: {ex.Message}");
            }
        }
    }
}
