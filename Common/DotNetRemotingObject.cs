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
        public DotNetRemotingObject() { }
        public DotNetRemotingObject(Func<string,string> onCommand)
        {
            this.OnCommand = onCommand;
        }
        public string QA(string command)
        {
            return OnCommand?.Invoke(command);
        }
    }
}
