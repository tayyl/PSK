using Common;
using Common.Logger;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ClientCommunicators
{
    public class RS232ClientCommunicator : ClientCommunicatorBase
    {
        SerialPort serialPort;
        public RS232ClientCommunicator(ILogger logger) : base(logger)
        {
            serialPort = new SerialPort
            {
                PortName = Utils.SetPortName("COM2", logger),
                ReadTimeout = 50000,
                WriteTimeout = 50000
            };
        }

        public override void Dispose()
        {
            serialPort.Dispose();
        }

        public override string ReadLine()
        {
            var response = serialPort.ReadLine();
            serialPort.Close();
            return response;
        }

        public override void WriteLine(string dataToSend)
        {
            if(!serialPort.IsOpen)
            serialPort.Open();
            serialPort.WriteLine(dataToSend);
        }
    }
}
