using Common;
using Common.Enums;
using Common.Logger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocols.Filesystem
{
    public class FilesystemCommunicator : ICommunicator
    {
        public ProtocolEnum Protocol => ProtocolEnum.filesystem;
        readonly string filePath;
        ILogger logger;
        public FilesystemCommunicator(string filePath, ILogger logger)
        {
            this.logger = logger;
            this.filePath = filePath;
        }
        public void Start(Func<string, string> OnCommand, Action<ICommunicator> OnDisconnect)
        {
            try
            {
                var req = File.ReadAllText(filePath);
                var res = OnCommand?.Invoke(req);
                logger?.LogSuccess($"[{Protocol}] received command from FTP client [{filePath}]: {req}");
                File.Delete(filePath);
                File.WriteAllText(filePath+"-Answer", res);
                File.WriteAllText(filePath + "-Ready", "Ready");
            }
            catch (Exception ex)
            {
                logger?.LogError($"[{Protocol}] failed to respond to FTP client [{filePath}]: {ex.Message}");
            }
            finally
            {
                OnDisconnect(this);
            }
        }

        public void Stop()
        {
            //??
        }
    }
}
