using Common.Enums;

namespace Common
{
    public interface IServiceModule
    {
        ServiceModuleEnum ServiceModule { get; }
        string AnswerCommand(string command);
    }
}