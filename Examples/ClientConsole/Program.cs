using Binance.Net;
using Binance.Net.Enums;
using Binance.Net.Objects.Spot;
using BinanceAPI.ClientConsole.Helper;
//using BinanceAPI.ClientConsole.Helper;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BinanceAPI.ClientConsole
{
    class Program
    {
        private static StreamWriter _writer;
        private static BinanceClient _client;
        private static BinanceSocketClient _socketClient;

        static void Main(string[] args)
        {
            SettingConfig();



            var tradeCostAveraging = TradeCostAveraging(symbol: "SOLBTC", limit: 500);
            LogHelper.WriteFreeLog(tradeCostAveraging);

            Console.ReadLine();
            _writer.Close();
        }

        private static object TradeCostAveraging(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? limit = 500)
        {
            var trades = _client.Spot.Order.GetAllOrders(symbol: symbol, limit: limit);
            if (trades.Success)
            {
                var tradesFilled = trades.Data.Where(c => c.Status == OrderStatus.Filled);
                decimal averaging = tradesFilled.Average(a => a.AverageFillPrice).Value;
                decimal totalQuantityFilled = 0;
                decimal totalCost = 0;
                foreach (var item in tradesFilled)
                {
                    if (item.Status == OrderStatus.Filled)
                    {
                        totalQuantityFilled += item.QuantityFilled;
                        totalCost += item.QuoteQuantityFilled;
                    }
                }
                averaging = totalCost / totalQuantityFilled;
                string result = $"You have {tradesFilled.Count()} trades ${symbol} filled."
                    +$"- Total buy: ${totalQuantityFilled}, total cost: ${totalCost}"
                    +$"------ DETAILS ------";
                return new 
                {
                    Summary = result,
                    Detail = trades.Data
                };
            }
            return null;
        }


        private static void SettingConfig()
        {
            string rootPath = $@"{Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName}\\log";
            string logDebug = Path.Combine(rootPath, $@"log-debug");
            DirectoryInfo diDebug = Directory.CreateDirectory(logDebug);
            string logTrade = Path.Combine(rootPath, $@"log-trade");
            DirectoryInfo diTrade = Directory.CreateDirectory(logTrade);

            _writer = new StreamWriter($"{logDebug}\\log-debug-{DateTime.Now.ToString("yyyyMMddHHmm")}.txt", true);
            BinanceClient.SetDefaultOptions(new BinanceClientOptions()
            {
                ApiCredentials = new ApiCredentials("dlgSlybqJTZ2zCTjf2sT97mWbcTRJbuYa5GtDPue6x3JJsulVt1gmZ3oGttfkQzJ", "Q6fjmKXHMHpVQqYXIrU9fdMVayRTAYcYGVE0x35W9Im3cRhjkIEl3oWYYpkBkaNp"),
                LogVerbosity = LogVerbosity.Debug,
                LogWriters = new List<TextWriter> { _writer, Console.Out }
            });
            BinanceSocketClient.SetDefaultOptions(new BinanceSocketClientOptions()
            {
                ApiCredentials = new ApiCredentials("dlgSlybqJTZ2zCTjf2sT97mWbcTRJbuYa5GtDPue6x3JJsulVt1gmZ3oGttfkQzJ", "Q6fjmKXHMHpVQqYXIrU9fdMVayRTAYcYGVE0x35W9Im3cRhjkIEl3oWYYpkBkaNp"),
                LogVerbosity = LogVerbosity.Debug,
                LogWriters = new List<TextWriter> { _writer, Console.Out }
            });


            _client = new BinanceClient();
            var startResult = _client.Spot.UserStream.StartUserStream();
            //var accountSnapshot = client.General.GetDailySpotAccountSnapshot(DateTime.Now.AddDays(-3), DateTime.Now);
            //LogHelper.WriteFreeLog(accountSnapshot.Data);

            if (!startResult.Success)
                throw new Exception($"Failed to start user stream: {startResult.Error}");

            _socketClient = new BinanceSocketClient();

            _socketClient.Spot.SubscribeToUserDataUpdates(startResult.Data,
                orderUpdate =>
                {
                    // Handle order update
                    LogHelper.WriteLogOderUpdate(orderUpdate);
                },
                ocoUpdate =>
                {
                    // Handle oco order update
                },
                positionUpdate =>
                {
                    // Handle account position update
                },
                balanceUpdate =>
                {
                    // Handle balance update
                });

            //// Spot.Market | Spot market info endpoints
            //// Spot.Order | Spot order info endpoints
            //client.Spot.Order.GetAllOrders("BTCUSDT");
            //// Spot.System | Spot system endpoints
            //client.Spot.System.GetExchangeInfo();
            //// Spot.UserStream | Spot user stream endpoints. Should be used to subscribe to a user stream with the socket client
            //client.Spot.UserStream.StartUserStream();
            //// Spot.Futures | Transfer to/from spot from/to the futures account + cross-collateral endpoints
            //client.Spot.Futures.TransferFuturesAccount("ASSET", 1, FuturesTransferType.FromSpotToUsdtFutures);

            //// FuturesCoin | Coin-M general endpoints
            //client.FuturesCoin.GetPositionInformation();
            //// FuturesCoin.Market | Coin-M futures market endpoints
            //client.FuturesCoin.Market.GetBookPrices("BTCUSD");
            //// FuturesCoin.Order | Coin-M futures order endpoints
            //client.FuturesCoin.Order.GetMyTrades();
            //// FuturesCoin.Account | Coin-M account info
            //client.FuturesCoin.Account.GetAccountInfo();
            //// FuturesCoin.System | Coin-M system endpoints
            //client.FuturesCoin.System.GetExchangeInfo();
            //// FuturesCoin.UserStream | Coin-M user stream endpoints. Should be used to subscribe to a user stream with the socket client
            //client.FuturesCoin.UserStream.StartUserStream();

            //// FuturesUsdt | USDT-M general endpoints
            //client.FuturesUsdt.GetPositionInformation();
            //// FuturesUsdt.Market | USDT-M futures market endpoints
            //client.FuturesUsdt.Market.GetBookPrices("BTCUSDT");
            //// FuturesUsdt.Order | USDT-M futures order endpoints
            //client.FuturesUsdt.Order.GetMyTrades("BTCUSDT");
            //// FuturesUsdt.Account | USDT-M account info
            //client.FuturesUsdt.Account.GetAccountInfo();
            //// FuturesUsdt.System | USDT-M system endpoints
            //client.FuturesUsdt.System.GetExchangeInfo();
            //// FuturesUsdt.UserStream | USDT-M user stream endpoints. Should be used to subscribe to a user stream with the socket client
            //client.FuturesUsdt.UserStream.StartUserStream();

            //// General | General/account endpoints
            //client.General.GetAccountInfo();

            //// Lending | Lending endpoints
            //client.Lending.GetFlexibleProductList();

            //// Margin | Margin general/account info
            //client.Margin.GetMarginAccountInfo();
            //// Margin.Market | Margin market endpoints
            //client.Margin.Market.GetMarginPairs();
            //// Margin.Order | Margin order endpoints
            //client.Margin.Order.GetAllMarginAccountOrders("BTCUSDT");
            //// Margin.UserStream | Margin user stream endpoints. Should be used to subscribe to a user stream with the socket client
            //client.Margin.UserStream.StartUserStream();
            //// Margin.IsolatedUserStream | Isolated margin user stream endpoints. Should be used to subscribe to a user stream with the socket client
            //client.Margin.IsolatedUserStream.StartIsolatedMarginUserStream("BTCUSDT");

            //// Mining | Mining endpoints
            //client.Mining.GetMiningCoinList();

            //// SubAccount | Sub account management
            //client.SubAccount.TransferSubAccount("fromEmail", "toEmail", "asset", 1);

            //// Brokerage | Brokerage management
            //client.Brokerage.CreateSubAccountAsync();

            //// WithdrawDeposit | Withdraw and deposit endpoints
            //client.WithdrawDeposit.GetWithdrawalHistory();

        }

    }
}
