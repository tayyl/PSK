using Common;
using Common.Enums;
using Common.Logger;
using System;
using System.IO;

namespace Protocols.Filesystem
{
    public class FilesystemListener : IListener
    {
        public ProtocolEnum Protocol => ProtocolEnum.filesystem;
        FileSystemWatcher fileSystemWatcher;
        ILogger logger;
        public FilesystemListener(ILogger logger)
        {
            if (!Directory.Exists(Consts.FilesystemPath))
            {
                Directory.CreateDirectory(Consts.FilesystemPath);
            }
            this.logger = logger;
        }

        public void Start(Action<ICommunicator> OnConnect)
        {
            try
            {
                fileSystemWatcher = new FileSystemWatcher(Consts.FilesystemPath);
                fileSystemWatcher.EnableRaisingEvents = true;

                fileSystemWatcher.Created += (sender, e) =>
                {
                    if (e.FullPath.Contains("Client") && Directory.Exists(e.FullPath))
                    {
                        OnConnect(new FilesystemCommunicator(e.FullPath, logger));
                    }
                };
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
                fileSystemWatcher.Dispose();
                Directory.Delete(Consts.FilesystemPath, true);
            }
            catch (Exception ex)
            {
                logger.LogError($"[{Protocol}] listener stop failed. Exception: {ex.Message}");
            }
        }
    }
}
