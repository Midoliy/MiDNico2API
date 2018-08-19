using System;
using System.IO;
using System.Xml.Linq;
using System.Linq;

namespace MiDNico2API.Contract
{
    public sealed class MiDComment
    {
        /// <summary>コメント内容</summary>
        public string      Text          { get; }
        /// <summary>コメント番号</summary>
        public int         Number        { get; }
        /// <summary>コメント投稿したタイミング(番組からの経過時刻/秒単位)</summary>
        public long        VideoPosition { get; }
        /// <summary>コメント投稿時刻(秒単位)</summary>
        public long        PostDateSec   { get; }
        /// <summary>コメント投稿時刻</summary>
        public DateTime    PostDate      { get; }
        /// <summary>コメント投稿したユーザのユーザID</summary>
        public string      UserID        { get; }
        /// <summary>コメント投稿者が184投稿した場合, true</summary>
        public bool        Anonymity     { get; }
        /// <summary>コメント投稿者がプレミアム会員の場合, true</summary>
        public bool        IsPremium     { get; }
        /// <summary>コメント表示位置</summary>
        public Nico2Locate Locate        { get; }
        /// <summary>コメントサイズ</summary>
        public Nico2Size   Size          { get; }
        /// <summary>コメント色</summary>
        public Nico2Color  Color         { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="data"></param>
        public MiDComment(
            Stream data
        )
        {
            if (data == default || data.Length <= 0)
            {
                throw new InvalidDataException("Commentデータの取得に失敗しました.");
            }

            // XMLデータを読み込む.
            data.Seek(0, SeekOrigin.Begin);
            var doc = XDocument.Load(data);
            if (doc.Elements("chat").Count() == 0)
            {
                throw new InvalidDataException($"{nameof(data)}: commentデータではありません.");
            }

            //　rootデータの生成.
            var root = doc.Root;

            Text         = root.Value;
            Number       = int.Parse(root.Attribute("no")?.Value ?? "0");
            PostDateSec  = long.Parse(root.Attribute("date")?.Value ?? "0");
            PostDate     = DateTimeOffset.FromUnixTimeSeconds(PostDateSec).LocalDateTime;
            UserID       = root.Attribute("user_id")?.Value;
            Anonymity    = (root.Attribute("anonymity")?.Value == default) || (root.Attribute("anonymity").Value != "1");
            IsPremium    = (root.Attribute("premium")?.Value   == default) || (root.Attribute("premium").Value   != "1");
            var commands = root.Attribute("mail")?.Value?.Split(' ');
            if (commands != default)
            {
                foreach (var cmd in commands)
                {
                    var c = cmd.ToLower();

                    if (c == "ue" || c == "naka" || c == "shita")
                    {
                        Locate = c.ToNico2Locate();
                    }
                    else if (c == "big" || c == "medium" || c == "small")
                    {
                        Size = c.ToNico2Size();
                    }
                    else if (c.Contains("white") || c.Contains("red")
                          || c.Contains("pink") || c.Contains("orange")
                          || c.Contains("yellow") || c.Contains("green")
                          || c.Contains("cyan") || c.Contains("blue")
                          || c.Contains("purple") || c.Contains("black"))
                    {
                        Color = c.ToNico2Color();
                    }
                }
            }
        }
    }
}
