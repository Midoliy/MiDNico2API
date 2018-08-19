using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MiDNico2API.Core
{
    public sealed class Nico2Socket : Socket
    {
        private static readonly Encoding _enc = Encoding.UTF8;

        private Nico2Socket(
            AddressFamily addressFamily,
            SocketType socketType,
            ProtocolType protocolType
        ) : base(addressFamily, socketType, protocolType)
        {
        }

        public static Nico2Socket Create()
        {
            return new Nico2Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public int Send(
            IPEndPoint ipEndPoint,
            string xml
        )
        {
            this.Connect(ipEndPoint);
            var data = Encoding.UTF8.GetBytes(xml);
            return this.Send(data, data.Length, SocketFlags.None);
        }
    }
}
