using Common.Enums;
using System;

namespace Protocols.Common
{
    public interface IListener
    {
        ProtocolEnum Protocol { get; }
        void Start(Action<ICommunicator> OnConnect);
        void Stop();
    }
}