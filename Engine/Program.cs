//#define CustomLogger
//#define ENGINE_SIDE

using nanoFramework.Networking;
using System;
using System.Diagnostics;
using System.Threading;
using System.IO.Ports;
using System.Text;
using nanoFramework.Logging;
using nanoFramework.Logging.Serial;
using Microsoft.Extensions.Logging;
using nanoFramework.Json;
using System.IO;
using System.Reflection;



//using Logger;

namespace Engine
{
    public class Program
    {
        private const int DEBUG_DELAY = 2000;
#if ENGINE_SIDE
        private const string VALUE_SSID = ""; //PUT HERE YOUR WIFI CREDENTIALS
        private const string VALUE_PASSWORD = "";
#endif

#if CustomLogger
        private static Logger.Logger _logger;
#else
        private static ILogger _logger;
#endif

        public static void Main()
        {
            Thread.Sleep(DEBUG_DELAY);
            try
            {
                WriteToConsole("Starting...");

                InitLog();
                WriteToConsole($"ENGINE VER.: {Assembly.GetExecutingAssembly().GetName().Version}");

#if ENGINE_SIDE
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
#else
                LoadAssembly("I:\\System.Device.Wifi.pe");
                Assembly assemblyItem = LoadAssembly("I:\\Plugin.pe");

                ActivateAssembly(assemblyItem, "Plugin.Program");



#endif
                //serialPort.Close();
                _logger = null;
            }
            catch (Exception ex)
            {
                WriteToConsole("-->"+ex.ToString());
                WriteToConsole("-->"+ex.StackTrace);
            }
            WriteToConsole("Go to sleep");

            Thread.Sleep(Timeout.InfiniteTimeSpan);
            WriteToConsole("done");
        }

        private static Assembly LoadAssembly(string FileName)
        {
            var buffer = ReadAssemblyFromDisk(FileName, _logger);

            var result = Assembly.Load(buffer);

            return result ?? throw new Exception($"Assembly not loaded: [{FileName}]. Empty assembly instanse");
        }

        private static void InitLog()
        {
#if CustomLogger
                var serialPort = new SerialPort("COM1", 115200, Parity.None, 8, StopBits.One);
                serialPort.WriteLine("\r\n\r\n");
                Thread.Sleep(10);
                if (!serialPort.IsOpen)
                {
                    serialPort.Open();
                };
                WriteToConsole("COM1 opened");
                _logger = new Logger.Logger(serialPort.BaseStream);
#else

            LogDispatcher.LoggerFactory = new SerialLoggerFactory("COM1", 115200, 8, Parity.None, StopBits.One);
            Thread.Sleep(10);
            _logger = LogDispatcher.GetLogger("Engine");
            Thread.Sleep(10);
#endif




            WriteToConsole("Logger created");
        }

        private static void ActivateAssembly(Assembly AssemblyItem, string TypeName)
        {
            var classTypeStr = TypeName;

            Type typeToRun = AssemblyItem.GetType(classTypeStr);
            if (typeToRun == null)
            {
                WriteToConsole($"Assembly with type do not exist: {classTypeStr}");
                return;
            }
            var start = typeToRun.GetMethod("Start");
            //var stop = typeToRun.GetMethod("Stop");
            //twinUpated = typeToRun.GetMethod("TwinUpdated");

            if (start == null)
            {
                WriteToConsole($"Assembly do not have start method: {AssemblyItem.GetName()}");
                return;
            }

            try
            {
                //var container = DataEntitiesGroup.GetContainer(DeviceDomain.CONTAINER_NAME_SERIAL_FOR_COMMANDS);
                //var names = DataEntitiesGroup.GetNames();
                // See if all goes right
                start.Invoke(null, new object[] { _logger});

            }
            catch (Exception ex)
            {
                WriteToConsole($"Error during starting assembly: {AssemblyItem.GetName()}. Error: {ex.Message}");
                WriteToConsole(ex.StackTrace.ToString());
            }
        }
#if ENGINE_SIDE
        private static bool ConnectToDHCP(int TimeOut = 60_000)
        {
            WriteToConsole("Connecting to WIFI from engine....");
            var ssid = "";
            var password = "";

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
            WriteToConsole("Disconnecting...");
            WifiNetworkHelper.Disconnect();
            WriteToConsole($"Disconnected. Status: {WifiNetworkHelper.Status}");
        }
#endif


        private static byte[] ReadAssemblyFromDisk(string File, ILogger Logger)
        {
            using (var fspe = new FileStream(File, FileMode.Open, FileAccess.Read))
            {
                var buff = new byte[fspe.Length];
                fspe.Read(buff, 0, buff.Length);

                return buff;
                // Needed as so far, there seems to be an issue when loading them too fast
            }
        }


        private static void WriteToConsole(string Message)
        {
            if(_logger != null)
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
