using System;
using System.Net;
using System.Net.Sockets;

namespace NetworkFramework
{
    public sealed class TCPServer
    {
        private TcpListener server;

        public TCPServer()
        {
            Console.WriteLine("Local ip: " + GetLocalIPAddress());
            Console.Write("Listen on port: ");
            int port = int.Parse(Console.ReadLine());
            Console.Write("Initializing server... ");
            server = new TcpListener(IPAddress.Any, port);
            Console.WriteLine("done!");
            Console.Write("Starting server... ");
            server.Start();
            Console.WriteLine("done!");
        }

        public Channel<string> Accept()
        {
            Console.Write("Waiting for client... ");
            TcpClient client = server.AcceptTcpClient();
            Console.WriteLine("connected!");
            NetworkStream stream = client.GetStream();
            return new TCPStream(stream);
        }

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("Local IP Address Not Found!");
        }


    }
}
