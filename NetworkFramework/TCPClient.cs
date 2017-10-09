using System;
using System.Net.Sockets;

namespace NetworkFramework
{
    public sealed class TCPClient : Channel<string>
    {
        private Channel<string> _stream;
        public TCPClient()
        {
            Console.Write("Connect to (IP): ");
            string server = GetIp();
            Console.WriteLine("Connecting to " + server);
            int port = GetPort();
            Console.Write("Initializing connection... ");
            TcpClient client = new TcpClient(server, port);
            NetworkStream stream = client.GetStream();
            Console.WriteLine("done!");
            _stream = new TCPStream(stream);
        }

        private int GetPort()
        {
            int port;
            bool success;
            do
            {
                Console.Write("On port: ");
                success = int.TryParse(Console.ReadLine(), out port);
            } while (!success);
            return port;
        }

        private string GetIp()
        {
            string input = Console.ReadLine();
            if (input == "localhost")
                return input;
            string[] inp = input.Split('.');
            string[] local = TCPServer.GetLocalIPAddress().Split('.');
            for (int i = inp.Length - 1; i >= 0; i--)
                local[i + local.Length - inp.Length] = inp[i];
            return String.Join(".", local);
        }

        public string Receive()
        {
            return _stream.Receive();
        }

        public void Send(string msg)
        {
            _stream.Send(msg);
        }

        public void SetVerbose(bool val)
        {
            _stream.SetVerbose(val);
        }

    }
}
