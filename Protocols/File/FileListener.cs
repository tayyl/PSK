using Common.Enums;
using System;

namespace Protocols.File
{
    public class FileListener : IListener
    {
        public ProtocolEnum Protocol => ProtocolEnum.file;

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
