using System.Net;

namespace NetworkFramework
{
    public sealed class UDPPacket
    {
        public readonly string data;
        public readonly IPAddress address;
        public readonly int port;
        public UDPPacket(string data, IPAddress address, int port) {
            this.data = data;
            this.address = address;
            this.port = port;
        }
    }
}