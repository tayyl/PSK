using Common.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class DotNetRemotingObject : MarshalByRefObject
    {
        readonly Func<string, string> OnCommand;
        readonly ILogger logger;
        public DotNetRemotingObject() { }
        public DotNetRemotingObject(Func<string, string> onCommand, ILogger logger)
        {
            this.OnCommand = onCommand;
            this.logger = logger;
        }
        public string QA(string command)
        {
            logger?.LogSuccess($"[.NetRemoting] received command from client: {command}");
            return OnCommand?.Invoke(command);
        }
    }
}
