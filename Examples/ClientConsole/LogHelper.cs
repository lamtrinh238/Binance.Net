using Newtonsoft.Json;
using System;
using System.IO;

namespace BinanceAPI.ClientConsole.Helper
{
    public static class LogHelper
    {
        public static void WriteLogTrade(object content, string name = "_free")
        {
            string rootPath = $@"{Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName}\\log";
            string logTrade = Path.Combine(rootPath, $@"log-trade");
            DirectoryInfo diTrade = Directory.CreateDirectory(logTrade);
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(logTrade, $"{name}.txt"), true))
            {
                outputFile.WriteLine($"{DateTime.Now.ToString()}----------------------------");
                outputFile.WriteLine(JsonConvert.SerializeObject(content, Formatting.Indented));
            }
        }
    }
}
