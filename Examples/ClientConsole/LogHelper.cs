using Binance.Net.Objects.Spot.SpotData;
using Binance.Net.Objects.Spot.UserStream;
using Newtonsoft.Json;
using System;
using System.IO;

namespace BinanceAPI.ClientConsole.Helper
{
    public static class LogHelper
    {
        public static void WriteLogOderUpdate(BinanceStreamOrderUpdate content, string name = "_unknow")
        {
            string rootPath = $@"{Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName}\\log";
            string logTrade = Path.Combine(rootPath, $@"log-trade");
            DirectoryInfo diTrade = Directory.CreateDirectory(logTrade);
            name = content.Symbol;

            using (StreamWriter outputFile = new StreamWriter(Path.Combine(logTrade, $"{name}.txt"), true))
            {
                outputFile.WriteLine($"{DateTime.Now.ToString()}----------------------------");
                outputFile.WriteLine(JsonConvert.SerializeObject(content, Formatting.Indented));
            }
        }

        public static void WriteFreeLog(object content, string name = "_free")
        {
            if ("_free".Equals(name))
            {
                name = $"_{DateTime.Now.ToString("yyyyMMddHHmm")}_free";
            }
            string rootPath = $@"{Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName}\\log";

            using (StreamWriter outputFile = new StreamWriter(Path.Combine(rootPath, $"{name}.txt"), true))
            {
                outputFile.WriteLine($"{DateTime.Now.ToString()}----------------------------");
                outputFile.WriteLine(JsonConvert.SerializeObject(content, Formatting.Indented));
            }
        }

        internal static void WriteLogOderUpdate(BinancePlacedOrder content, string name = "_unknow")
        {
            string rootPath = $@"{Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName}\\log";
            string logTrade = Path.Combine(rootPath, $@"log-trade");
            DirectoryInfo diTrade = Directory.CreateDirectory(logTrade);
            name = content.Symbol;

            using (StreamWriter outputFile = new StreamWriter(Path.Combine(logTrade, $"{name}.txt"), true))
            {
                outputFile.WriteLine($"{DateTime.Now.ToString()}----------------------------");
                outputFile.WriteLine(JsonConvert.SerializeObject(content, Formatting.Indented));
            }
        }
    }
}
