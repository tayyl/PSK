using Common.Enums;
using System;
using System.Threading.Tasks;

namespace Protocols
{
    public interface ICommunicator
    {
        ProtocolEnum Protocol { get; }
        void Start(Func<string, string> OnCommand, Action<ICommunicator> OnDisconnect);
        void Stop();
    }

}