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
    public class FileClientCommunicator : ClientCommunicatorBase
    {
        Random random = new Random();
        string fileName = string.Empty;
        readonly string clientId;
        readonly string clientPath;
        public FileClientCommunicator(ILogger logger) : base(logger)
        {
            clientId = Guid.NewGuid().ToString();
            clientPath = Path.Combine(Consts.FilesystemPath, $"Client-{clientId}");
            if (!Directory.Exists(Consts.FilesystemPath))
            {
                Directory.CreateDirectory(Consts.FilesystemPath);
            }
            if (!Directory.Exists(clientPath))
            {
                Directory.CreateDirectory(clientPath);
            }
        }

        public override void Dispose()
        {
            Directory.Delete(clientPath, true);
        }

        public override string ReadLine()
        {
            var filePath = Path.Combine(clientPath, fileName + "-Answer");

            string response;
            while ((response = Utils.TryReadAllText(filePath)) == null) { }//probuje odczytac, plik moze byc zajety przez proces serwera
                    
            File.Delete(filePath);

            return response;
        }

        public override void WriteLine(string dataToSend)
        {
            fileName = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + random.Next();
            File.WriteAllText(Path.Combine(clientPath, fileName), dataToSend);
        }
    }
}
