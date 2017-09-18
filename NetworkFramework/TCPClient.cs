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
            string server = GenerateIp();
            Console.WriteLine(server);
            Console.Write("On port: ");
            int port = int.Parse(Console.ReadLine());
            Console.Write("Initializing connection... ");
            TcpClient client = new TcpClient(server, port);
            NetworkStream stream = client.GetStream();
            Console.WriteLine("done!");
            _stream = new TCPStream(stream);
        }

        private string GenerateIp()
        {
            string input = Console.ReadLine();
            if (input=="localhost")
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
