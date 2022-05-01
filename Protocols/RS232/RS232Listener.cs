using Common;
using Common.Enums;
using Common.Logger;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Protocols.RS232
{
    public class RS232Listener : IListener
    {
        public ProtocolEnum Protocol => ProtocolEnum.rs232;
        SerialPort serialPort;
        CancellationTokenSource cts;
        readonly ILogger logger;
        public RS232Listener(ILogger logger)
        {
            this.logger = logger;
        }
        public void Start(Action<ICommunicator> OnConnect)
        {
            try
            {
                serialPort = new SerialPort
                {
                    PortName = Utils.SetPortName("COM1", logger),
                    ReadTimeout = 50000,
                    WriteTimeout = 50000,
                };
                cts = new CancellationTokenSource();
                void CommandListener()
                {

                    while (!cts.IsCancellationRequested)
                    {

                        if (!serialPort.IsOpen)
                        {
                            serialPort.Open();
                            var res = new RS232Communicator(serialPort, logger);
                            OnConnect?.Invoke(res);
                        }

                    }
                }
                Task.Factory.StartNew(CommandListener, cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            }
            catch (Exception ex)
            {
                logger.LogError($"[{Protocol}] listener start failed. Exception: {ex.Message}");
            }
        }

        public void Stop()
        {
            try
            {
                serialPort?.Dispose();
                cts?.Cancel();
            }
            catch (Exception ex)
            {
                logger.LogError($"[{Protocol}] listener stop failed. Exception: {ex.Message}");
            }
        }
    }
}
