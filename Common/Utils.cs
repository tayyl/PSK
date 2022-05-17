using Common.Logger;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class Utils
    {
        public static string TryReadAllText(string filePath)
        {
            try
            {
                return File.ReadAllText(filePath);
            }
            catch (IOException ex)
            {
                return null;
            }

        }
        public static bool IsFileLocked(this FileInfo file)
        {
            try
            {
                using(var fileStream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    fileStream.Close();
                }
            }catch(IOException ex)
            {
                return true;
            }
            return false;
        }
        public static string GenerateRandomText(this Random random, int length)
        {
            var answerString = new StringBuilder();
            for (var i = 0; i < length; i++)
            {
                answerString.Append((char)random.Next(99, 123));
            }
            return answerString.ToString();
        }
        public static byte[] TryConvertFromBase64(this string data)
        {
            try
            {
                return Convert.FromBase64String(data);
            }catch (Exception ex)
            {
                return null;
            }
        }
        public static string SetPortName(string defaultPortName,ILogger logger)
        {
            if(!string.IsNullOrEmpty(defaultPortName)) return defaultPortName;
            string portName;

            logger.LogInfo("Available serial ports:");
            foreach (string s in SerialPort.GetPortNames())
            {
                logger.LogInfo($"   {s}");
            }

            logger.LogInfo($"Enter COM port value (Default: {defaultPortName}): ");
            portName = Console.ReadLine();

            if (portName == "" || !(portName.ToLower()).StartsWith("com"))
            {
                portName = defaultPortName;
            }
            return portName;
        }
    }
}
