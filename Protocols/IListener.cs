using Common.Enums;
using System;

namespace Protocols
{
    public interface IListener
    {
        ProtocolEnum Protocol { get; }
        void Start(Action<ICommunicator> OnConnect);
        void Stop();
    }
}