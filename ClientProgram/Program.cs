
using Common.Logger;
using System.Threading.Tasks;

namespace ClientProgram
{

    public class Program
    {

        public static async Task Main(string[] args)
        {
            var consoleLogger = new ConsoleLogger();
            var client = new Client.Client(consoleLogger);

            await client.Start();
        }
    }
}