using Common;
using Common.Logger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ClientCommunicators
{
    public class FTPClientCommunicator : ClientCommunicatorBase
    {
        FileSystemWatcher fileSystemWatcher;
        Random random = new Random();
        bool canRead = false;
        string fileName = string.Empty;

        public FTPClientCommunicator(ILogger logger) : base(logger)
        {
            if (!Directory.Exists(Consts.FTPPath))
            {
                Directory.CreateDirectory(Consts.FTPPath);
            }
            fileSystemWatcher = new FileSystemWatcher(Consts.FTPPath);
            fileSystemWatcher.EnableRaisingEvents = true;
            fileSystemWatcher.Created += FileSystemWatcher_Created;
        }

        private void FileSystemWatcher_Created(object sender, FileSystemEventArgs e)
        {
            if (e.FullPath.Contains(fileName + "-Ready"))
            {
                canRead = true;
            }
        }
        public override void Dispose()
        {
            Directory.Delete(Consts.FTPPath, true);
        }

        public override string ReadLine()
        {
            while (!canRead) { }

            var response = File.ReadAllText(Path.Combine(Consts.FTPPath, fileName + "-Answer"));
         
            File.Delete(Path.Combine(Consts.FTPPath, fileName + "-Answer"));
            File.Delete(Path.Combine(Consts.FTPPath,  fileName + "-Ready"));
            canRead = false;

            return response;
        }

        public override void WriteLine(string dataToSend)
        {
            fileName = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + random.Next();
            File.WriteAllText(Path.Combine(Consts.FTPPath, fileName), dataToSend);
        }
    }
}
