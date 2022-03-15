using Common.Enums;

namespace Common;
public interface IServiceModule
{
    ServiceModuleEnum ServiceModule { get; }
    Task<string> AnswerCommand(string command); 
}