using Common;
using Common.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceModules
{
    public class FileServiceModule : IServiceModule
    {
        public ServiceModuleEnum ServiceModule => ServiceModuleEnum.file;
        public FileServiceModule()
        {
            if (!Directory.Exists(Consts.FilesystemPath))
                Directory.CreateDirectory(Consts.FilesystemPath);
        }
        public string AnswerCommand(string command)
        {
            var commands = command.Split(' ');
            var action = commands.ElementAtOrDefault(0);
            var fileName = commands.ElementAtOrDefault(1);
            var fileAsBase64 = commands.ElementAtOrDefault(2);

            try
            {
                switch (action)
                {
                    case "list":
                        var files = Directory.GetFiles(Consts.FilesystemPath);
                        return files.Length == 0 ? "There are no files on server" : string.Join(",", files);
                    case "delete":
                        File.Delete(Path.Combine(Consts.FilesystemPath, fileName));
                        return $"File {fileName} deleted successfully";
                    case "get":
                        var file = File.ReadAllBytes(Path.Combine(Consts.FilesystemPath, fileName));
                        return Convert.ToBase64String(file);
                    case "put":
                        var fileAsBytes = Convert.FromBase64String(fileAsBase64);
                        File.WriteAllBytes(Path.Combine(Consts.FilesystemPath, fileName), fileAsBytes);
                        return $"Successfully saved file {fileName}";
                    default:
                        return $"Not recognized action: {action}. Try: list, delete, get, put";
                }
            }
            catch (Exception ex)
            {
                return $"Error while processing command: {ex.Message}";
            }
        }
    }
}
