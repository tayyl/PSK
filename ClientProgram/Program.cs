
using Common.Logger;

var consoleLogger = new ConsoleLogger();
var client = new Client.Client(consoleLogger);

await client.Start();