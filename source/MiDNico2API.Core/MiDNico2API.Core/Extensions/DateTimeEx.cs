using System;

namespace MiDNico2API.Core.Extensions
{
    internal static class DateTimeEx
    {
        public static long ToUnixTime(
            this DateTime time
        )
        {
            var offset = new DateTimeOffset(time.Ticks, new TimeSpan(+09, 00, 00));
            return offset.ToUnixTimeSeconds() * 100;
        }
    }
}
