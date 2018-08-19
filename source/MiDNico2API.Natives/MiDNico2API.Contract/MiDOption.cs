using System;

namespace MiDNico2API.Contract
{
    public class MiDOption
    {
        /// <summary>デフォルト値</summary>
        public static readonly MiDOption Default = new MiDOption();

        /// <summary>コメント色(default: White)</summary>
        public Nico2Color  Color  { get; }
        /// <summary>コメントサイズ(default: Medium)</summary>
        public Nico2Size   Size   { get; }
        /// <summary>コメント位置(default: Middle)</summary>
        public Nico2Locate Locate { get; }

        public MiDOption(
            Nico2Color  color  = Nico2Color.White,
            Nico2Size   size   = Nico2Size.Medium,
            Nico2Locate locate = Nico2Locate.Middle
        )
        {
            this.Color  = color;
            this.Size   = size;
            this.Locate = locate;
        }
    }

    public enum Nico2Color
    {
        White       = 0x00ffffff,
        White2      = 0x00cccc99,
        Red         = 0x00ff0000,
        Red2        = 0x00cc0033,
        Pink        = 0x00ff8080,
        Pink2       = 0x00ff33cc,
        Orange      = 0x00ffc000,
        Orange2     = 0x00ff6600,
        Yellow      = 0x00ffff00,
        Yellow2     = 0x00999900,
        Green       = 0x0000ff00,
        Green2      = 0x0000cc66,
        Cyan        = 0x0000ffff,
        Cyan2       = 0x0000cccc,
        Blue        = 0x000000ff,
        Blue2       = 0x003399ff,
        Purple      = 0x00c000ff,
        Purple2     = 0x006633cc,
        Black       = 0x00000000,
        Black2      = 0x00666666,
    }

    public enum Nico2Size
    {
        Big,
        Medium,
        Small,
    }

    public enum Nico2Locate
    {
        Top,
        Middle,
        Under,
    }

    public static class MiDOptionEx
    {
        private static readonly Array _colors = Enum.GetValues(typeof(Nico2Color));
        
        public static Nico2Color ToNico2Color(this string color)
        {
            var condition = color?.ToLower();

            switch (condition)
            {
                case "white"    : return Nico2Color.White;
                case "white2"   : return Nico2Color.White2;
                case "red"      : return Nico2Color.Red;
                case "red2"     : return Nico2Color.Red2;
                case "pink"     : return Nico2Color.Pink;
                case "pink2"    : return Nico2Color.Pink2;
                case "orange"   : return Nico2Color.Orange;
                case "orange2"  : return Nico2Color.Orange2;
                case "yellow"   : return Nico2Color.Yellow;
                case "yellow2"  : return Nico2Color.Yellow2;
                case "green"    : return Nico2Color.Green;
                case "green2"   : return Nico2Color.Green2;
                case "cyan"     : return Nico2Color.Cyan;
                case "cyan2"    : return Nico2Color.Cyan2;
                case "blue"     : return Nico2Color.Blue;
                case "blue2"    : return Nico2Color.Blue2;
                case "purple"   : return Nico2Color.Purple;
                case "purple2"  : return Nico2Color.Purple2;
                case "black"    : return Nico2Color.Black;
                case "black2"   : return Nico2Color.Black2;
                default         : return Nico2Color.White;
            }
        }

        public static Nico2Size ToNico2Size(this string size)
        {
            var condition = size?.ToLower();
            switch (condition)
            {
                case "big"   : return Nico2Size.Big;
                case "medium": return Nico2Size.Medium;
                case "small" : return Nico2Size.Small;
                default      : return Nico2Size.Medium;
            }
        }

        public static Nico2Locate ToNico2Locate(this string locate)
        {
            var condition = locate?.ToLower();
            switch (condition)
            {
                case "top"   : return Nico2Locate.Top;
                case "ue"    : return Nico2Locate.Top;
                case "middle": return Nico2Locate.Middle;
                case "naka"  : return Nico2Locate.Middle;
                case "under" : return Nico2Locate.Under;
                case "shita" : return Nico2Locate.Under;
                default      : return Nico2Locate.Middle;
            }
        }

        internal static string ToValue(this Nico2Color color)
        {
            return color.ToString().ToLower();
        }

        internal static string ToValue(this Nico2Size size)
        {
            return size.ToString().ToLower();
        }

        internal static string ToValue(this Nico2Locate locate)
        {
            switch (locate)
            {
                case Nico2Locate.Top:    return "ue";
                case Nico2Locate.Middle: return "naka";
                case Nico2Locate.Under:  return "shita";
                default:                 return "naka";
            }
        }
    }
}
