using Common;
using Common.Enums;
using Common.Logger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Protocols.Filesystem
{
    public class FilesystemCommunicator : ICommunicator
    {
        public ProtocolEnum Protocol => ProtocolEnum.filesystem;
        readonly string filesystemPath;
        ILogger logger;
        FileSystemWatcher fileSystemWatcher;
        CancellationTokenSource cts;
        public FilesystemCommunicator(string filesystemPath, ILogger logger)
        {
            this.logger = logger;
            this.filesystemPath = filesystemPath;
        }
        public void Start(Func<string, string> OnCommand, Action<ICommunicator> OnDisconnect)
        {
            try
            {
                cts = new CancellationTokenSource();
                fileSystemWatcher = new FileSystemWatcher(filesystemPath);
                fileSystemWatcher.EnableRaisingEvents = true;
                fileSystemWatcher.Created += (sender, e) =>
                {
                    try
                    {
                        if (!e.FullPath.Contains("-Answer"))
                        {
                            string req; 
                            while ((req = Utils.TryReadAllText(e.FullPath)) == null) { } //probuje odczytac, plik moze byc zajety przez proces klienta
                           
                            var res = OnCommand?.Invoke(req);
                            logger?.LogSuccess($"[{Protocol}] received command from client [{filesystemPath}]: {req}");
                            File.Delete(e.FullPath);
                            File.WriteAllText(e.FullPath + "-Answer", res);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger?.LogError($"[{Protocol}] failed to respond to filesystem client [{filesystemPath}]: {ex.Message}");
                    }
                };

                Task.Factory.StartNew(() =>
                {
                    while (Directory.Exists(filesystemPath)) { }
                }, cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            }
            catch (Exception ex)
            {
                logger?.LogError($"[{Protocol}] failed to start filesystem communicator [{filesystemPath}]: {ex.Message}");
                OnDisconnect(this);
            }

        }

        public void Stop()
        {
            try
            {
                fileSystemWatcher.Dispose();
                Directory.Delete(Consts.FilesystemPath, true);
                cts.Cancel();
            }
            catch (Exception ex)
            {
                logger.LogError($"[{Protocol}] listener stop failed. Exception: {ex.Message}");
            }
        }
    }
}
