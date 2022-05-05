using Common.Enums;
using Common.Logger;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Protocols.RS232
{
    public class RS232Communicator : ICommunicator
    {
        public ProtocolEnum Protocol => ProtocolEnum.rs232;
        ILogger logger;
        SerialPort serialPort;
        CancellationTokenSource cts;
        public RS232Communicator(SerialPort serialPort, ILogger logger)
        {
            this.serialPort = serialPort;
            this.logger = logger;
        }
        public void Start(Func<string, string> OnCommand, Action<ICommunicator> OnDisconnect)
        {
            try
            {
                cts = new CancellationTokenSource();
                Task.Factory.StartNew(() => ReceiveCommand(OnCommand, OnDisconnect), cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            }
            catch (Exception ex)
            {
                logger?.LogError($"[{Protocol}] communicator failed to start. Exception: {ex.Message}");
                OnDisconnect?.Invoke(this);
            }
        }
        void ReceiveCommand(Func<string, string> OnCommand, Action<ICommunicator> OnDisconnect)
        {
            try
            {
                while (!cts.IsCancellationRequested)
                {
                    var command = serialPort.ReadLine();
                    if (!string.IsNullOrEmpty(command))
                    {

                        logger?.LogSuccess($"[{Protocol}] received command from client[{serialPort.PortName}]: {command}");
                        var response = OnCommand(command);
                        serialPort.WriteLine(response);
                    }
                }
            }
            catch (IOException e)
            {
                logger?.LogError($"[{Protocol}] Failed to receive data. Exception {e.Message}");
            }
            catch (Exception e)
            {
                logger?.LogError($"[{Protocol}] Failed to receive data. Exception {e.Message}");
            }
            finally
            {
                serialPort.Close();
                OnDisconnect?.Invoke(this);
            }
        }

        public void Stop()
        {
            try
            {
                serialPort.Close();
                serialPort.Dispose();
            }
            catch (Exception ex)
            {
                logger?.LogError($"[{Protocol}] communicator failed to stop. Exception: {ex.Message}");
            }
        }
    }
}
