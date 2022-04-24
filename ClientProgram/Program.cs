
using Common.Logger;
using System.Threading.Tasks;

namespace ClientProgram
{

    public class Program
    {

        public static void Main(string[] args)
        {
            var consoleLogger = new ConsoleLogger();
            var client = new Client.Client(consoleLogger);

            client.Start();
        }
    }
}