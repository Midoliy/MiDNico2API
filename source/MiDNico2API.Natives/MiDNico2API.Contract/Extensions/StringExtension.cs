using System.Linq;
using System.Text;

namespace MiDNico2API.Contract.Extensions
{
    internal static class StringExtension
    {
        internal static byte[] ToBinary(
           this string str
        ) => Encoding.UTF8.GetBytes(str);

        internal static string[] ToLines(
            this string str
        ) => str.Split('\0')?.Where(l => 0 < l?.Length)?.ToArray();
    }
}