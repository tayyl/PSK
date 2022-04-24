using Common.Enums;

namespace ServiceModules
{
    public interface IServiceModule
    {
        ServiceModuleEnum ServiceModule { get; }
        string AnswerCommand(string command);
    }
}