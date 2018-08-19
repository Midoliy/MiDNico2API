using System.Net;
using System.Net.Http;

namespace MiDNico2API.Core
{
    public sealed class Nico2Signal
    {
        /// <summary>
        /// (通信プロトコル:GET)
        /// 対象URLにGET通信する.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public static HttpResponseMessage Get(
            in string url,
            in CookieContainer cookie
        )
        {
            using (var handler = new HttpClientHandler() { UseCookies = true, CookieContainer = cookie })
            using (var client = new HttpClient(handler))
            {
                return client.GetAsync(url).Result;
            }
        }

        /// <summary>
        /// (通信プロトコル:POST)
        /// 対象URLにPOST通信する.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="cookie"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static HttpResponseMessage Post(
            in string url,
            in CookieContainer cookie = null,
            in HttpContent param = null
        )
        {
            using (var handler = new HttpClientHandler() { UseCookies = true, CookieContainer = cookie ?? new CookieContainer() })
            using (var client = new HttpClient(handler))
            {
                return client.PostAsync(url, param).Result;
            }
        }

        /// <summary>
        /// (通信プロトコル:SEND)
        /// 対象URLにSEND通信する.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="cookie"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static HttpResponseMessage Send(
            in string url,
            in CookieContainer cookie,
            in HttpRequestMessage param = null
        )
        {
            using (var handler = new HttpClientHandler() { UseCookies = true, CookieContainer = cookie })
            using (var client = new HttpClient(handler))
            {
                return client.SendAsync(param).Result;
            }
        }

        /// <summary>
        /// (通信プロトコル:PUT)
        /// 対象URLにPUT通信する.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="cookie"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static HttpResponseMessage Put(
            in string url,
            in CookieContainer cookie,
            in HttpContent param = null
        )
        {
            using (var handler = new HttpClientHandler() { UseCookies = true, CookieContainer = cookie })
            using (var client = new HttpClient(handler))
            {
                return client.PutAsync(url, param).Result;
            }
        }

        /// <summary>
        /// (通信プロトコル:DELETE)
        /// 対象URLにDELETE通信する.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public static HttpResponseMessage Delete(
            in string url,
            in CookieContainer cookie
        )
        {
            using (var handler = new HttpClientHandler() { CookieContainer = cookie })
            using (var client = new HttpClient(handler))
            {
                return client.DeleteAsync(url).Result;
            }
        }

        /// <summary>
        /// クッキーを取得する
        /// </summary>
        /// <param name="url"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static CookieContainer TakeCookie(
            in string url,
            in HttpContent param
        )
        {
            using (var handler = new HttpClientHandler() { UseCookies = true, CookieContainer = new CookieContainer() })
            using (var client = new HttpClient(handler))
            {
                var response = client.PostAsync(url, param).Result;
                return handler.CookieContainer;
            }
        }
    }
}
