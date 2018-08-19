using System.Text;

namespace MiDNico2API.Contract.Extensions
{
    internal static class ByteExtension
    {
        internal static string ToUTF8String(
             this byte[] binaries
        ) => Encoding.UTF8.GetString(binaries);
    }
}