using Client.ClientCommunicators;
using Common;
using Common.Logger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.QAClients
{
    public class FileClient : QAClientBase
    {
        public FileClient(ClientCommunicatorBase clientCommunicator, ILogger logger) : base(clientCommunicator, logger)
        {
            if (!Directory.Exists(Consts.FileServiceClientPath))
                Directory.CreateDirectory(Consts.FileServiceClientPath);
        }

        public override string QA(string dataToSend)
        {
            var commands = dataToSend.Split(' ');
            var action = commands.ElementAtOrDefault(1);
            var fileName = commands.ElementAtOrDefault(2);
            var request = string.Empty;
            try
            {
                switch (action)
                {
                    case "list":
                    case "delete":
                    case "get":
                        request = dataToSend;
                        break;
                    case "put":
                        var file = File.ReadAllBytes(Path.Combine(Consts.FileServiceClientPath, fileName));
                        request = $"{dataToSend} {Convert.ToBase64String(file)}";
                        break;
                    default:
                        logger?.LogError($"Not recognized action: {action}. Try: list, delete, get, put");
                        break;
                }

                ClientCommunicator.WriteLine(request);
                var response = ClientCommunicator.ReadLine();

                if (action == "get")
                {
                    var file = response.TryConvertFromBase64();
                    if (file != null)
                    {
                        File.WriteAllBytes(Path.Combine(Consts.FileServiceClientPath, fileName), file);
                        response = $"Successfully downloaded file '{fileName}' to path: {Consts.FileServiceClientPath}";
                    }
                }
                return response;
            }
            catch (Exception ex)
            {
                logger?.LogError($"Error while processing command: {ex.Message}");
                return string.Empty;
            }
        }
    }
}
