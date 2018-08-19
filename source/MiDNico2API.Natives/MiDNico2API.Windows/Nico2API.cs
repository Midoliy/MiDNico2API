using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using MiDNico2API.Core;
using MiDNico2API.Contract;
using System.Xml.Linq;
using System.Linq;

namespace MiDNico2API.Windows
{
    public class Nico2API
    {
        private static readonly Uri             _nico2uri      = new Uri(@"http://nicovideo.jp");
        private        readonly CookieContainer _cookie;
        private        readonly int             _nico2liveId;
        private                 PlayerStatus    _status        = default;
        private                 Nico2Comment    _commentClient = default;


        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="cookie">ニコニコのCookie情報</param>
        /// <param name="nico2liveId">ニコニコ生放送番組ID</param>
        public Nico2API(
            in CookieContainer cookie,
            in int             nico2liveId
        )
        {
            if (cookie.GetCookies(_nico2uri)?["user_session"] == null)
            {
                throw new InvalidDataException("無効なCookie情報.");
            }

            if (nico2liveId < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(nico2liveId));
            }

            _cookie = cookie;
            _nico2liveId = nico2liveId;
        }

        /// <summary>
        /// Createメソッド
        /// </summary>
        /// <param name="cookie">ニコニコのCookie情報</param>
        /// <param name="nico2liveId">ニコニコ生放送番組ID</param>
        public static Nico2API Create(
            in CookieContainer cookie,
            in int             nico2liveId
        )
        {
            return new Nico2API(cookie, nico2liveId);
        }

        /// <summary>
        /// (API: login)
        /// ニコニコにログインする.
        /// </summary>
        /// <param name="email">ニコニコログイン用メールアドレス</param>
        /// <param name="password">ニコニコログイン用パスワード</param>
        /// <returns>ニコニコのログインCookie情報</returns>
        public static CookieContainer Login(
            in string email,
            in string password
        )
        {
            var cookie = Nico2Auth.Login(email, password);
            return cookie;
        }

        /// <summary>
        /// (API: heartbeat)
        /// ニコニコAPIとの接続を維持するために定期的に実行する必要あり.
        /// 基本的にレスポンス値を気にする必要はない.
        /// </summary>
        /// <returns>ニコニコAPIからのレスポンス値</returns>
        public Stream Heartbeat()
        {
            var data = Nico2Auth.HeartBeat(_cookie, _nico2liveId);
            return data;
        }

        /// <summary>
        /// (API: getplayerstatus)
        /// プレーヤー情報を取得する.
        /// </summary>
        public PlayerStatus GetPlayerStatus()
        {
            var data = Nico2LiveInfo.Access(_cookie).GetInfo($"lv{_nico2liveId}");
            if (data.Length <= 0)
            {
                throw new InvalidDataException("PlayerStatusの取得に失敗しました.");
            }

            // プレーヤー情報を取得する.
            _status = new PlayerStatus(data);
            return _status;
        }

        /// <summary>
        /// (API: consumeComment)
        /// コメントを非同期で取得する.
        /// </summary>
        /// <param name="recievedComment">コメントを取得した際の処理</param>
        /// <param name="recievedThread">生放送スレッド情報を取得した際の処理</param>
        /// <returns>Nico2APIオブジェクト</returns>
        public async Task<Nico2API> ConsumeCommentAsync(
            Action<MiDComment> recievedComment,
            Action<MiDThread>  recievedThread = default
        )
        {
            if (_status == default)
            {
                _status = this.GetPlayerStatus();
            }

            if(_commentClient == default)
            {
                _commentClient = Nico2Comment.Create(_cookie, _status.IpEndPoint);
            }

            await _commentClient.ConnectAsync(
                                  (int)_status.Thread,
                                  data =>
                                  {
                                      var doc = XDocument.Load(data);
                                      if (doc.Elements("chat").Count() != 0)
                                      {
                                          recievedComment?.Invoke(new MiDComment(data));
                                      }
                                      else if (doc.Elements("thread").Count() != 0)
                                      {
                                          recievedThread?.Invoke(new MiDThread(data));
                                      }
                                  },
                                  1024,
                                  1000
                               );
            return this;
        }

        /// <summary>
        /// (非公式API: stopComment)
        /// 生放送番組とのプレーヤー情報からコメントを取得するを停止するAPI.
        /// </summary>
        /// <param name="status">プレーヤー情報</param>
        public void StopComment()
        {
            if (_commentClient == default)
            {   // コメントを取得していない場合, 何もせずにreturn.
                return;
            }

            if (_status == default)
            {
                _status = this.GetPlayerStatus();
            }

            Nico2Comment.Create(_cookie, _status.IpEndPoint).DisConnect();
        }

        /// <summary>
        /// (非公式API : postComment)
        /// ニコニコ生放送の番組にコメントを投稿する.
        /// </summary>
        /// <param name="message">投稿するコメント内容</param>
        /// <param name="command">コメントに付与する追加属性</param>
        public void PostComment(
            in string    message,
               MiDOption command = default
        )
        {
            if (_status == default)
            {
                _status = this.GetPlayerStatus();
            }

            if (command == default)
            {
                command = MiDOption.Default;
            }

            Nico2Comment.Create(_cookie, _status.IpEndPoint)
                        .Send(message,
                                 _status.UserID,
                                 (int)_status.Thread,
                                 _status.OpenTimeUTC,
                                 _status.IsPremium,
                                 command.Color.ToValue(),
                                 command.Size.ToValue(),
                                 command.Locate.ToValue());
        }

        /// <summary>
        /// (非公式API : postCommentAsAdmin)
        /// 生主コメントを投稿する.
        /// </summary>
        /// <param name="message">投稿するコメント内容</param>
        /// <param name="color">コメント色</param>
        public void PostCommentAsAdmin(
            in string     message,
            in Nico2Color color = Nico2Color.White
        )
        {
            if (_status == default)
            {
                _status = this.GetPlayerStatus();
            }

            Nico2Comment.Create(_cookie, _status.IpEndPoint)
                        .SendAsAdmin(message, _nico2liveId, color: color.ToValue());
        }

        /// <summary>
        /// (非公式API : postCommentAsAdmin)
        /// 生主コメントを投稿する.
        /// その際, コメントを生主コメント投稿欄に固定する.
        /// </summary>
        /// <param name="message">投稿するコメント内容</param>
        /// <param name="color">コメント色</param>
        public void PostPermComment(
            in string     message,
            in Nico2Color color = Nico2Color.White
        )
        {
            if (_status == default)
            {
                _status = this.GetPlayerStatus();
            }

            Nico2Comment.Create(_cookie, _status.IpEndPoint)
                        .SendAsAdmin(message, _nico2liveId, color: color.ToValue(), isPermanent: true);
        }

        /// <summary>
        /// (非公式API : stopPermComment)
        /// 生主コメント欄に固定しているコメントを削除する.
        /// </summary>
        public void StopPermComment()
        {
            if (_status == default)
            {
                _status = this.GetPlayerStatus();
            }

            Nico2Comment.Create(_cookie, _status.IpEndPoint)
                        .DeletePermComment(_nico2liveId);
        }

        /// <summary>
        /// (API: userInfo)
        /// ユーザIDを元にユーザ情報を取得する.
        /// </summary>
        /// <param name="userId">ユーザID</param>
        /// <returns>ユーザ情報</returns>
        public MiDUser GetUserInfo(
            in string userId
        )
        {
            var data = Nico2UserInfo.Take(userId);
            var info = new MiDUser(data);
            return info;
        }

    }
}
