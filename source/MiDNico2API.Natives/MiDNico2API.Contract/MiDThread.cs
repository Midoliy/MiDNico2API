using System;
using System.IO;
using System.Xml.Linq;
using System.Linq;

namespace MiDNico2API.Contract
{
    public sealed class MiDThread
    {
        /// <summary>ニコニコ生放送番組のスレッドID</summary>
        public long     Thread        { get; }
        /// <summary>ニコニコ生放送番組に投稿されているコメント数</summary>
        public int      CommentCount  { get; }
        /// <summary>コメント投稿用チケット</summary>
        public string   Ticket        { get; }
        /// <summary>サーバ時間</summary>
        public long     ServerTimeUTC { get; }
        /// <summary>サーバ時間</summary>
        public DateTime ServerTime    { get; }

        public MiDThread(
            in Stream data
        )
        {
            if (data == default || data.Length <= 0)
            {
                throw new InvalidDataException("Threadデータの取得に失敗しました.");
            }

            // XMLデータを読み込む.
            data.Seek(0, SeekOrigin.Begin);
            var doc = XDocument.Load(data);
            if (doc.Elements("thread").Count() == 0)
            {
                throw new InvalidDataException($"{nameof(data)}: threadデータではありません.");
            }

            //　rootデータの生成.
            var root = XDocument.Load(data).Root;

            Thread        = long.Parse(root.Attribute("thread")?.Value ?? "0");
            CommentCount  = int.Parse(root.Attribute("resultcode")?.Value ?? "-1");
            Ticket        = root.Attribute("ticket")?.Value;
            ServerTimeUTC = long.Parse(root.Attribute("server_time")?.Value ?? "0");
            ServerTime    = DateTimeOffset.FromUnixTimeSeconds(ServerTimeUTC).LocalDateTime;
        }
    }
}
