using MiDNico2API.Contract;

namespace MiDNico2API.Windows
{
    public static class Extensions
    {
        public static System.Drawing.Color ToRGB(this Nico2Color color)
        {
            return System.Drawing.Color.FromArgb((int)color);
        }
    }
}
