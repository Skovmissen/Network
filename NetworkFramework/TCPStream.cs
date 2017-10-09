using System;
using System.Net.Sockets;
using System.Text;

namespace NetworkFramework
{
    sealed class TCPStream : Channel<string>
    {
        private NetworkStream _stream;
        private bool _verbose;

        public TCPStream(NetworkStream stream)
        {
            _stream = stream;
            _verbose = true;
        }

        public void Send(string msg)
        {
            byte[] data = Encoding.ASCII.GetBytes(msg);
            Send(data);
        }
        public void Send(byte[] data)
        {
            if (_verbose)
                Console.Write("Sending data... ");
            _stream.Write(data, 0, data.Length);
            if (_verbose)
                Console.WriteLine("sent {0} bytes", data.Length);
        }

        public string Receive()
        {
            StringBuilder accumulatedMessage = new StringBuilder();
            byte[] buffer = new byte[256];
            int numberOfBytesRead = 0;
            if (_verbose)
                Console.Write("Waiting for data... ");
            do
            {
                numberOfBytesRead = _stream.Read(buffer, 0, buffer.Length);
                if (_verbose)
                    Console.Write("received {0} bytes... ", numberOfBytesRead);
                string bufferAsString = Encoding.ASCII.GetString(buffer, 0, numberOfBytesRead);
                accumulatedMessage.AppendFormat("{0}", bufferAsString);
            } while (_stream.DataAvailable);
            if (_verbose)
                Console.WriteLine("done");
            return accumulatedMessage.ToString();
        }

        public void SetVerbose(bool val)
        {
            _verbose = val;
        }

    }
}