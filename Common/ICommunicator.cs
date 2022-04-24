using Common.Enums;
using System;
using System.Threading.Tasks;

namespace Protocols.Common
{
    public interface ICommunicator
    {
        ProtocolEnum Protocol { get; }
        Task Start(Func<ICommunicator, string, Task> OnCommand, Action<ICommunicator> OnDisconnect);
        void Stop();
        Task Send(string data);
    }

}