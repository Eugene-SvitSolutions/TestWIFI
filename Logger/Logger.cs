using System;
using System.IO;
using System.Text;
using System.Threading;

namespace Logger
{
    public class Logger
    {
        private Stream _stream;
        

        public Logger(Stream LogStream)
        {
            _stream = LogStream;
        }

        public void WriteLine(string Message)
        {
            var buffer = Encoding.UTF8.GetBytes(Message+'\n');
            _stream.Write(buffer, 0, buffer.Length);
            _stream.Flush();
            Thread.Sleep(10);
        }
    }
}
