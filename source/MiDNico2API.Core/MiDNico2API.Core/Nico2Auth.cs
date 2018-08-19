using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;

namespace MiDNico2API.Core
{
    /// <summary>
    /// ニコニコとの認証に関するAPIと通信するクラス
    /// </summary>
    public static class Nico2Auth
    {
        /// <summary>
        /// ニコニコにログインするメソッド.
        /// </summary>
        /// <param name="email">ニコニコログイン用メールアドレス</param>
        /// <param name="password">ニコニコログイン用パスワード</param>
        /// <returns>ニコニコとのCookie情報</returns>
        public static CookieContainer Login(
            in string email,
            in string password
        )
        {
            if (string.IsNullOrWhiteSpace(email)   ) throw new ArgumentNullException(nameof(email));
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentNullException(nameof(password));

            var param = new Dictionary<string, string>
                             {
                                 { "next_url", ""       },
                                 { "mail"    , email    },
                                 { "password", password },
                             };
            var content = new FormUrlEncodedContent(param);
            string api  = @"https://secure.nicovideo.jp/secure/login?site=nicolive";

            var cookie  = Nico2Signal.TakeCookie(api, content);

            // Cookieの取得に失敗した場合, Exceptionをスローする.
            if (cookie.GetCookies(new Uri(@"http://nicovideo.jp"))["user_session"] == null)
            {
                throw new Exception("ログインに失敗しました.");
            }
                
            return cookie;
        }

        /// <summary>
        /// セッション情報を維持するためのメソッド.
        /// 定期的に実行する必要がある.
        /// </summary>
        /// <param name="cookie">ニコニコとのCookie情報</param>
        /// <param name="nico2liveId">ニコニコ生放送の番組ID</param>
        /// <returns>ニコニコからのレスポンスメッセージ</returns>
        public static Stream HeartBeat(
            in CookieContainer cookie,
            in int             nico2liveId
        )
        {
            if (cookie == default) throw new ArgumentNullException(nameof(cookie));
            if (nico2liveId < 0  ) throw new ArgumentOutOfRangeException(nameof(nico2liveId));

            string      api      = $"http://live.nicovideo.jp/api/heartbeat?v={nico2liveId}";
            HttpContent response = Nico2Signal.Get(api, cookie).Content;

            return response.ReadAsStreamAsync().Result;
        }
    }
}
