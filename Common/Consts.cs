﻿using System;
using System.Collections.Generic;
using System.IO;
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
        public const int DatagramSize = 60000;
        public const string FTPClientPath = "FTPClient";
        public const string FTPServerPath = "FTPServer";
        public static IPAddress IpAddress => IPAddress.Loopback;
    }
}
