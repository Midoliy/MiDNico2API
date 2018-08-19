using MiDNico2API.Core.Extensions;
using System;
using System.Buffers;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MiDNico2API.Core
{
    /// <summary>
    /// ニコニコのコメントに関するAPIと通信をするクラス
    /// </summary>
    public sealed class Nico2Comment
    {
        private        readonly CookieContainer _cookie;
        private        readonly IPEndPoint      _ipendpoint;
        private                 bool            _doStop = false;
        private static readonly byte            NULL    = 0;
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="cookie">ニコニコとのCookie情報</param>
        /// <param name="ipEndPoint">ニコニコのIPEndPoint</param>
        public Nico2Comment(
            CookieContainer cookie,
            IPEndPoint ipendpoint
        )
        {
            _cookie = cookie;
            _ipendpoint = ipendpoint;
        }

        /// <summary>
        /// ニコニコのコメントに関するAPIと通信をするインスタンスを生成するメソッド.
        /// </summary>
        /// <param name="cookie">ニコニコとのCookie情報</param>
        /// <param name="ipendpoint">ニコニコのIPEndPoint</param>
        /// <returns>Nico2Commentインスタンス</returns>
        public static Nico2Comment Create(
            CookieContainer cookie,
            IPEndPoint ipendpoint
        )
        {
            if (cookie == default) throw new ArgumentNullException(nameof(cookie));
            if (ipendpoint == default) throw new ArgumentNullException(nameof(ipendpoint));

            return new Nico2Comment(cookie, ipendpoint);
        }


        /// <summary>
        /// ニコニコ生放送からコメントを取得する非同期メソッド.
        /// </summary>
        /// <param name="nico2Thread">ニコニコ生放送のスレッド番号</param>
        /// <param name="observer">コメント受信時の処理</param>
        /// <param name="bufSize">一度に取得するバイトサイズ</param>
        /// <param name="historyFrom">過去何件に遡ってコメントを取得するかの設定</param>
        /// <returns>Nico2Commentインスタンス</returns>
        public async Task<Nico2Comment> ConnectAsync(
            long nico2Thread,
            Action<Stream> observer,
            int bufSize = 1024,
            short historyFrom = 100
        )
        {
            if (observer == null) throw new ArgumentNullException(nameof(observer));
            if (bufSize <= 0) throw new ArgumentOutOfRangeException(nameof(bufSize));
            if (historyFrom < 0) throw new ArgumentOutOfRangeException(nameof(historyFrom));

            var xml = $"<thread thread=\"{nico2Thread}\" version=\"20061206\" res_from=\"-{historyFrom}\"/>\0";
            return await this.ConnectAsync(xml, observer, bufSize);
        }

        /// <summary>
        /// ニコニコ生放送からコメントを取得する非同期メソッド.
        /// </summary>
        /// <param name="xmlParam">ニコニコ生放送接続用パラメータ</param>
        /// <param name="observer">コメント受信時の処理</param>
        /// <param name="bufSize">一度に取得するバイトサイズ</param>
        /// <returns>Nico2Commentインスタンス</returns>
        public async Task<Nico2Comment> ConnectAsync(
            string xmlParam,
            Action<Stream> observer,
            int bufSize
        )
        {
            if (string.IsNullOrWhiteSpace(xmlParam)) throw new ArgumentNullException(nameof(xmlParam));
            if (observer == null) throw new ArgumentNullException(nameof(observer));
            if (bufSize <= 0) throw new ArgumentOutOfRangeException(nameof(bufSize));
            
            async void recieve(
                Nico2Socket socket,
                int bsize
            )
            {
                _doStop = false;
                bool   waiting = false;
                byte[] buffer  = ArrayPool<byte>.Shared.Rent(bsize);
                byte[] prev    = new byte[0];

                do
                {
                    if (_doStop) break;

                    if (waiting)
                    {
                        await Task.Delay(1);
                        continue;
                    }

                    var arg = new SocketAsyncEventArgs();
                    arg.SetBuffer(buffer, 0, bsize);
                    arg.SocketFlags = SocketFlags.None;
                    arg.Completed += (_, e) =>
                    {
                        byte[] src       = prev.Concat(e.Buffer).ToArray();
                        int    srcLength = prev.Length + e.BytesTransferred;
                               prev      = new byte[0];
                        Span<byte> span  = src.AsSpan(0, srcLength);

                        int start = 0;
                        for (int i = 0; i < span.Length; i++)
                        {
                            if (span[i] != NULL) continue;                  // コメントデータはNULL文字区切りのため, NULL文字でない場合 continue.

                            int length = i - start;                         // xml データ幅を計算.
                            var xml    = span.Slice(start, length);         // xml データを抽出.
                                start  = i + 1;                             // 読み取り開始位置を更新.
                            
                            using (MemoryStream ms = new MemoryStream(xml.ToArray()))
                            {
                                observer?.Invoke(ms);                // 取得情報をobserverに通知.
                            }                                
                        }

                        // データが途切れてしまっている場合, 一時データに保存しておく.
                        if (start < span.Length)
                        {
                            int length = span.Length - start;
                            prev = span.Slice(start, length).ToArray();
                        }

                        // bufferを初期化.
                        ArrayPool<byte>.Shared.Return(buffer, clearArray:true);
                        waiting = false;
                    };

                    socket.ReceiveAsync(arg);
                    waiting = true;
                }
                while (true);
            }

            var client = Nico2Socket.Create();
            var res    = client.Send(_ipendpoint, xmlParam);
            await Task.Run(() => recieve(client, bufSize));

            return this;
        }

        /// <summary>
        /// ニコニコ生放送からコメントを取得を停止するメソッド.
        /// </summary>
        public void DisConnect()
        {
            _doStop = true;
        }

        /// <summary>
        /// ニコニコ生放送にコメントを投稿するメソッド.
        /// </summary>
        /// <param name="message">投稿するコメント</param>
        /// <param name="userId">コメント投稿するユーザID</param>
        /// <param name="nico2Thread">ニコニコ生放送のスレッド番号</param>
        /// <param name="livecastStartTime">ニコニコ生放送の番組開始時間</param>
        /// <param name="isPremium">プレミアム会員の場合, true を指定</param>
        /// <param name="mails">コメントに付与する追加コマンドリスト</param>
        /// <returns>送信できたデータバイト数</returns>
        public void Send(
            in string message,
            in string userId,
            in long  nico2Thread,
            in long livecastStartTime,
            in bool isPremium = false,
            params string[] mails
        )
        {
            var msg       = message;
            var thread    = nico2Thread;
            var user      = userId;
            var premium   = isPremium;
            var startDate = livecastStartTime * 100;

            this.ConnectAsync(thread, 
            data => 
            {
                var root     = XDocument.Load(data).Root;
                var ticket   = root.Attribute("ticket")?.Value;                         // コメント投稿用チケット
                int.TryParse(root.Attribute("last_res")?.Value, out int last_res);
                var block    = last_res / 100;                      
                var postkey  = Nico2LiveInfo.Access(_cookie).GetPostKey(thread, block); // コメント投稿用キー
                var now      = DateTime.Now.ToUnixTime();

                var pThread  = $"thread=\"{thread}\"";
                var pDate    = $" date=\"{now}\"";
                var pTicket  = $" ticket=\"{ticket}\"";
                var pVpos    = $" vpos=\"{(now - startDate)}\"";
                var pPostkey = $" postkey=\"{postkey}\"";
                var pMail    = $" mail=\"{string.Join(" ", mails)}\"";
                var pUserId  = $" user_id=\"{user}\"";
                var pPremium = $" premium=\"{(premium ? "1" : "0")}\"";

                var param    = $"<chat {pThread}{pDate}{pTicket}{pVpos}{pPostkey}{pMail}{pUserId}{pPremium}>{msg}</chat>\0";
                var a = this.Send(param);
                this.DisConnect();
            },
            bufSize: 512, historyFrom: 0).Wait();

        }

        /// <summary>
        /// ニコニコ生放送にコメントを投稿するメソッド.
        /// </summary>
        /// <param name="xmlParam">コメント投稿用パラメータ</param>
        /// <returns>送信できたデータバイト数</returns>
        public int Send(
            in string xmlParam
        )
        {
            using (var client = Nico2Socket.Create())
            {
                return client.Send(_ipendpoint, xmlParam);
            }
        }

        /// <summary>
        /// ニコニコ生放送に生放送主としてコメントを投稿するメソッド.
        /// </summary>
        /// <param name="message">投稿するコメント</param>
        /// <param name="nico2LiveId">ニコニコ生放送の番組ID</param>
        /// <param name="isPermanent">投稿したコメントを生主コメント欄に固定する場合, true を指定</param>
        /// <param name="color">コメントの色を指定</param>
        /// <param name="nickname">コメント投稿時のニックネーム</param>
        /// <returns>ニコニココメントサーバからのレスポンス</returns>
        public Stream SendAsAdmin(
            in string message,
            in int nico2LiveId,
            in bool isPermanent = false,
            in string color = "white",
            in string nickname = ""
        )
        {
            if (string.IsNullOrWhiteSpace(message)) throw new ArgumentNullException(nameof(message));
            if (nico2LiveId < 0) throw new ArgumentOutOfRangeException(nameof(nico2LiveId));

            var pText  = $"\"text\":\"{message}\"";
            var pColor = $"\"color\":\"{color}\"";
            var pPerm  = $"\"isPermanent\":{isPermanent.ToString().ToLower()}";
            var pName  = $"\"userName\":\"{nickname}\"";

            var msg = $"{{{pText},{pColor},{pPerm},{pName}}}";  // 投稿コメントをJSON配列形式の文字列にする.

            return this.SendAsAdmin(nico2LiveId, msg);
        }

        /// <summary>
        /// ニコニコ生放送に生放送主としてコメントを投稿するメソッド.
        /// </summary>
        /// <param name="nico2LiveId">ニコニコ生放送の番組ID</param>
        /// <param name="xmlParam">コメント投稿用パラメータ</param>
        /// <returns>ニコニココメントサーバからのレスポンス</returns>
        public Stream SendAsAdmin(
            in int nico2LiveId,
            in string xmlParam
        )
        {
            if (nico2LiveId <= 0) throw new ArgumentOutOfRangeException(nameof(nico2LiveId));
            if (string.IsNullOrWhiteSpace(xmlParam)) throw new ArgumentNullException(nameof(xmlParam));

            var url     = $"http://live2.nicovideo.jp/watch/lv{nico2LiveId}/operator_comment";
            var content = new StringContent(xmlParam, Encoding.UTF8, @"application/json");

            var response = Nico2Signal.Put(url, _cookie, content);
            return response?.Content?.ReadAsStreamAsync().Result;
        }

        /// <summary>
        /// 生主コメント欄に固定されているコメントを削除するメソッド.
        /// </summary>
        /// <param name="nico2LiveId">ニコニコ生放送の番組ID</param>
        public Stream DeletePermComment(
            in int nico2LiveId
        )
        {
            if (nico2LiveId < 0) throw new ArgumentOutOfRangeException(nameof(nico2LiveId));

            var url      = $"http://live2.nicovideo.jp/watch/lv{nico2LiveId}/operator_comment";
            var response = Nico2Signal.Delete(url, _cookie);
            return response?.Content?.ReadAsStreamAsync().Result;
        }
    }
}
