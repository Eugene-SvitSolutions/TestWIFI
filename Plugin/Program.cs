using Microsoft.Extensions.Logging;
using nanoFramework.Logging;
using nanoFramework.Networking;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;

namespace Plugin
{
    public class Program
    {
        private const string VALUE_SSID = ""; //PUT HERE YOUR WIFI CREDENTIALS
        private const string VALUE_PASSWORD = "";
        private static ILogger _logger;
        public static void Main()
        {
            Debug.WriteLine("Hello from nanoFramework!");

            Thread.Sleep(Timeout.Infinite);

            // Browse our samples repository: https://github.com/nanoframework/samples
            // Check our documentation online: https://docs.nanoframework.net/
            // Join our lively Discord community: https://discord.gg/gCyBu8T
        }

        public static void Start(ILogger Logger)
        {
            try
            {
                if (Logger == null) { return; }
                _logger = Logger;

                WriteToConsole($"PLUGIN ver.:{Assembly.GetExecutingAssembly().GetName().Version}");

                WriteToConsole("Disconnectiong from wifi...");
                Thread.Sleep(1);
                DisconnectFromWIFI();
                Thread.Sleep(1);
                var isDHCOConnected = ConnectToDHCP();
                if (!isDHCOConnected)
                {
                    WriteToConsole($"Not connected.Status :{WifiNetworkHelper.Status}");
                    if (WifiNetworkHelper.HelperException != null)
                    {
                        WriteToConsole($"Message: {WifiNetworkHelper.HelperException.Message}");
                        WriteToConsole($"Stack: {WifiNetworkHelper.HelperException.StackTrace}");

                    }
                }
                else
                {
                    WriteToConsole($"Connected!!!!. Status: {WifiNetworkHelper.Status}");
                    WriteToConsole(DateTime.UtcNow.ToString());
                }

                DisconnectFromWIFI();
            }
            catch (Exception Ex)
            {
                WriteToConsole(Ex.Message);
                WriteToConsole(Ex.StackTrace);
            }



            
        }

        private static bool ConnectToDHCP(int TimeOut = 60_000)
        {
            WriteToConsole("Connectiong to WIFI from engine....");
            var ssid = VALUE_SSID;
            var password = VALUE_PASSWORD;

            if (string.IsNullOrEmpty(ssid)) { throw new Exception("Empty SID"); }

            return WifiNetworkHelper.ConnectDhcp(
                           ssid,
                           password,
                           requiresDateTime: true,
                           token: new CancellationTokenSource(TimeOut).Token
                       );
        }

        private static void DisconnectFromWIFI()
        {
            WriteToConsole("Disconnectiong...");
            WifiNetworkHelper.Disconnect();
            WriteToConsole($"Disconnected. Status: {WifiNetworkHelper.Status}");
        }

        private static void WriteToConsole(string Message)
        {
            if (_logger != null)
            {
#if CustomLogger
                _logger.WriteLine(Message);
#else
                _logger.LogError(Message);
#endif
            }
            else
            {
                Debug.WriteLine(Message);
            }
        }
    }
}
