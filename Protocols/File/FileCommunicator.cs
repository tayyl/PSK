using Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocols.File
{
    public class FileCommunicator : ICommunicator
    {
        public ProtocolEnum Protocol =>ProtocolEnum.file;

        public void Start(Func<string, string> OnCommand, Action<ICommunicator> OnDisconnect)
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}
