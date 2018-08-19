using System.IO;
using System.Net.Http;

namespace MiDNico2API.Core
{
    /// <summary>
    /// ニコニコのユーザ情報に関するAPIと通信するクラス
    /// </summary>
    public static class Nico2UserInfo
    {
        /// <summary>
        /// ユーザ情報を取得するメソッド.
        /// </summary>
        /// <param name="userId">ユーザID</param>
        /// <returns>ユーザ情報</returns>
        public static Stream Take(
            in string userId
        )
        {
            var client = new HttpClient();
            var url = $"http://api.ce.nicovideo.jp/api/v1/user.info?user_id={userId}";
            var res = Nico2Signal.Post(url);
            return res.Content.ReadAsStreamAsync().Result;
        }
    }
}
