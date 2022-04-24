using Common;
using Common.Enums;
using System;

namespace ServiceModules {
    public class PingServiceModule : IServiceModule
    {
        public ServiceModuleEnum ServiceModule => ServiceModuleEnum.ping;
        private Random random => new Random();
        public string AnswerCommand(string command)
        {
            var answerString = string.Empty;
            if (!int.TryParse(command, out var length))
            {
                return "Invalid number of bytes.";
            }
            for (var i = 0; i < length; i++)
            {
                answerString += (char)random.Next(99, 123);
            }

            return answerString;
        }

    }
}