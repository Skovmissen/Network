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
            string server = Console.ReadLine();
            Console.Write("On port: ");
            int port = int.Parse(Console.ReadLine());
            Console.Write("Initializing connection... ");
            TcpClient client = new TcpClient(server, port);
            NetworkStream stream = client.GetStream();
            Console.WriteLine("done!");
            _stream = new TCPStream(stream);
        }

        public string Receive()
        {
            return _stream.Receive();
        }

        public void Send(string msg)
        {
            _stream.Send(msg);
        }

    }
}
