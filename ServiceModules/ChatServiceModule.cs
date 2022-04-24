using Common.Enums;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceModules
{
    public class ChatServiceModule : IServiceModule
    {
        ConcurrentDictionary<string, ConcurrentBag<string>> messageQueue = new ConcurrentDictionary<string, ConcurrentBag<string>>();
        public ServiceModuleEnum ServiceModule => ServiceModuleEnum.chat;

        public string AnswerCommand(string command)
        {
            var commands = command.Split(' ');
            var action = commands.ElementAtOrDefault(0);
            var sender = commands.ElementAtOrDefault(1);
            var message = commands.ElementAtOrDefault(commands.Length - 1);
            var commandsWithoutActionSender = commands.Skip(2);
            var users = commandsWithoutActionSender.Take(commandsWithoutActionSender.Count() - 1);

            if (sender != null && !messageQueue.ContainsKey(sender))
            {
                messageQueue.TryAdd(sender, new ConcurrentBag<string>());
            }

            switch (action)
            {
                case "get":
                    var messages = new StringBuilder();
                    foreach(var msg in messageQueue[sender])
                    {
                        messages.Append($"\n{msg}");
                    }
                    var result = messages.ToString();
                    return !string.IsNullOrEmpty(result) ? result : $"There are no messages for user: {sender}";
                case "msg":
                    try
                    {
                        foreach (var user in users)
                        {
                            var msg = $"[{DateTime.Now:dd-MM-yyyy}][{sender}]: {message}";
                            if (!messageQueue.ContainsKey(user))
                            {
                                messageQueue.TryAdd(user, new ConcurrentBag<string>());
                            }
                            messageQueue[user].Add(msg);
                        }
                        return $"Message send from {sender} to {string.Join(" ", users)}";
                    }
                    catch (Exception ex)
                    {
                        return $"Failed to save message from {sender} to {string.Join(" ", users)}, ex: {ex.Message}";
                    }
                case "who":
                    var availableUsers = messageQueue.Select(x => x.Key);
                    return availableUsers.Count() > 0 ? string.Join(" ", availableUsers) : "List of users is empty";
                default:
                    return "Unrecognized command, available commands are: get, msg, who";
            }
        }
    }
}
