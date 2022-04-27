using Common;
using Common.Enums;
using System;

namespace ServiceModules {
    public class PingServiceModule : IServiceModule
    {
        public ServiceModuleEnum ServiceModule => ServiceModuleEnum.ping;
        private Random random = new Random();
        public string AnswerCommand(string command)
        {
            var commands = command.Split(' ');
            if (!int.TryParse(commands[0], out var length))
            {
                return "Invalid number of bytes.";
            }

            return random.GenerateRandomText(length);
        }

    }
}