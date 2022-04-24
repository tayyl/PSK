using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class Consts
    {
        public const int TcpPort = 4000;
        public const int UdpPort = 6000;
        public static IPAddress IpAddress => IPAddress.Loopback;
    }
}
