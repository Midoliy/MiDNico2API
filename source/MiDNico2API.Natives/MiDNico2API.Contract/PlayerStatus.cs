using System;
using System.IO;
using System.Xml.Linq;
using System.Linq;
using System.Net;

namespace MiDNico2API.Contract
{
    public class PlayerStatus
    {
        /// <summary>生放送番組ID</summary>
        public string     LiveID        { get; }
        /// <summary>生放送番組タイトル</summary>
        public string     Title         { get; }
        /// <summary>生放送番組説明</summary>
        public string     Description   { get; }
        /// <summary>コミュニティID</summary>
        public string     CommunityID   { get; }
        /// <summary>生放送番組閲覧者数</summary>
        public int        WatchCount    { get; }
        /// <summary>生放送コメント投稿数</summary>
        public int        CommentCount  { get; }
        /// <summary>生放送開始時間</summary>
        public long       OpenTimeUTC   { get; }
        /// <summary>生放送開始時間</summary>
        public DateTime   OpenTime      { get; }
        /// <summary>コミュニティサムネイル画像URL</summary>
        public string     ThumbUrl      { get; }
        /// <summary>ログインユーザID</summary>
        public string     UserID        { get; }
        /// <summary>ログインユーザのプレミアムフラグ</summary>
        public bool       IsPremium     { get; }
        /// <summary>生放送IPEndPoint</summary>
        public IPEndPoint IpEndPoint    { get; }
        /// <summary>生放送スレッドID</summary>
        public long       Thread        { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="data"></param>
        public PlayerStatus(
            Stream data
        )
        {
            if (data == default || data.Length <= 0)
            {
                throw new InvalidDataException("PlayerStatusの取得に失敗しました.");
            }

            // XMLデータを読み込む.
            var doc = XDocument.Load(data);
            if(doc.Elements("getplayerstatus").Count() == 0)
            {
                throw new InvalidDataException($"{nameof(data)}: getplayerstatusデータではありません.");
            }

            //　rootデータの生成.
            var root = doc.Root;

            // getplayerstatusから返ってきた値が成功値かチェック.
            var status = root.Attribute("status")?.Value;
            if (status == default || status != "ok")
            {
                throw new InvalidDataException($"{nameof(data)}: getplayerstatusデータが不正です.");
            }


            // streamデータを取得.
            var stream   = root.Element("stream");
            // stream内にある生放送番組情報を取得する.
            LiveID       = stream?.Element("id")?.Value;
            Title        = stream?.Element("title")?.Value;
            Description  = stream?.Element("description")?.Value;
            CommunityID  = stream?.Element("default_community")?.Value;
            WatchCount   = int.Parse(stream?.Element("watch_count")?.Value ?? "0");
            CommentCount = int.Parse(stream?.Element("comment_count")?.Value ?? "0");
            OpenTimeUTC  = long.Parse(stream?.Element("open_time")?.Value ?? "0");
            OpenTime     = DateTimeOffset.FromUnixTimeSeconds(OpenTimeUTC).LocalDateTime;
            ThumbUrl     = stream?.Element("thumb_url")?.Value;


            // userデータを取得.
            var user = root.Element("user");
            // user内にあるユーザ情報を取得する.
            UserID    = user?.Element("user_id")?.Value;
            IsPremium = (user?.Element("user_id")?.Value != null) && (user.Element("user_id").Value == "1");


            // msデータを取得.
            var ms     = root.Element("ms");
            // ms内にあるユーザ情報を取得する.
            IpEndPoint = CreateIPEndPoint(ms?.Element("addr")?.Value, ms?.Element("port")?.Value);
            Thread     = long.Parse(ms?.Element("thread")?.Value ?? "0");
        }

        /// <summary>
        /// IPアドレスとポート番号からIPEndPointを生成する.
        /// </summary>
        /// <param name="ms_addr">IPアドレス</param>
        /// <param name="ms_port">ポート番号</param>
        /// <returns>IPEndPointインスタンス</returns>
        private IPEndPoint CreateIPEndPoint(string ms_addr, string ms_port)
        {
            IPHostEntry host = Dns.GetHostEntry(ms_addr);
            IPAddress ipaddress = host?.AddressList?.FirstOrDefault();
            int.TryParse(ms_port, out int port);

            return new IPEndPoint(ipaddress, port);
        }
    }
}
