using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MiDNico2API.Core
{
    /// <summary>
    /// ニコニコの番組情報に関するAPIと通信するクラス
    /// </summary>
    public sealed class Nico2LiveInfo
    {
        private readonly CookieContainer _cookie;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="cookie">ニコニコとのCookie情報</param>
        public Nico2LiveInfo(
            CookieContainer cookie
        )
        {
            if (cookie == default) throw new ArgumentNullException(nameof(cookie));
            _cookie = cookie;
        }

        /// <summary>
        /// ニコニコの番組情報に関するAPIと通信するインスタンスを生成するメソッド.
        /// </summary>
        /// <param name="cookie">ニコニコとのCookie情報</param>
        /// <returns>Nico2LiveInfoインスタンス</returns>
        public static Nico2LiveInfo Access(
            in CookieContainer cookie
        )
        {
            if (cookie == default) throw new ArgumentNullException(nameof(cookie));
            return new Nico2LiveInfo(cookie);
        }

        /// <summary>
        /// 指定したニコニコ生放送の番組情報を取得するメソッド.
        /// </summary>
        /// <param name="nico2LiveId">番組情報を取得する生放送ID</param>
        /// <returns>番組情報</returns>
        public Stream GetInfo(
            in string nico2LiveId
        )
        {
            var url = $"http://live.nicovideo.jp/api/getplayerstatus?v={nico2LiveId}";
            var result = Nico2Signal.Get(url, _cookie);
            return result.Content.ReadAsStreamAsync().Result;
        }

        /// <summary>
        /// 指定したニコニコ生放送の番組にコメント投稿するための投稿キーを取得するメソッド.
        /// </summary>
        /// <param name="nico2Thread">ニコニコ生放送のスレッド番号</param>
        /// <param name="commentBlockNumber">(最新コメント番号 / 100) した値</param>
        /// <returns>コメント投稿用のキー値</returns>
        public string GetPostKey(
            in long nico2Thread,
            in int commentBlockNumber
        )
        {
            if (nico2Thread < 0) throw new ArgumentOutOfRangeException(nameof(nico2Thread));
            if (commentBlockNumber < 0) throw new ArgumentOutOfRangeException(nameof(commentBlockNumber));

            var url = $"http://live.nicovideo.jp/api/getpostkey?thread={nico2Thread}&block_no={commentBlockNumber}";
            var result = Nico2Signal.Get(url, _cookie);
            return result?.Content?.ReadAsStringAsync().Result.Replace("postkey=", "");
        }

        /// <summary>
        /// 指定したニコニコ生放送の概要情報を取得するメソッド.
        /// </summary>
        /// <param name="ipendpoint">ニコニコのIPEndPoint.</param>
        /// <param name="nico2Thread">ニコニコ生放送のスレッド番号.</param>
        /// <param name="received">生放送概要情報を取得した際の挙動.</param>
        /// <returns>このインスタンスを返す.</returns>
        public async Task<Nico2LiveInfo> GetThreadInfoAsync(
            IPEndPoint ipendpoint,
            long nico2Thread,
            Action<Stream> received
        )
        {
            if (ipendpoint == default) throw new ArgumentNullException(nameof(ipendpoint));
            if (nico2Thread == default) throw new ArgumentOutOfRangeException(nameof(nico2Thread));

            var client = Nico2Comment.Create(_cookie, ipendpoint);

            await client.ConnectAsync(nico2Thread,
                res =>
                {
                    received?.Invoke(res);
                    client.DisConnect();
                }
                , 512, 0);

            return this;
        }

        /// <summary>
        /// 現在放送中の生放送番組リスト取得するメソッド.
        /// </summary>
        /// <param name="mode">検索モード([index] or [recent])</param>
        /// <param name="pageNumber">ページ番号</param>
        /// <param name="category">カテゴリ([common] or [try] or [live] or [req] or [face] or [totu] or [r18])</param>
        /// <param name="sort">ソート方法([start_time] or [view_couter] or [comment_num] or [community_level] or [community_create_time])</param>
        /// <param name="order">順番([desc] or [asc])</param>
        /// <param name="tags">検索するタグ情報</param>
        /// <returns>現在生放送中の生放送番組リスト</returns>
        public Stream GetZappingList(
            in string       mode,
            in int          pageNumber,
            in string       category    = "",
            in string       sort        = "",
            in string       order       = "asc",
            params string[] tags
        )
        {
            if (mode       == default) throw new ArgumentNullException(nameof(mode));
            if (pageNumber <  0      ) throw new ArgumentOutOfRangeException(nameof(pageNumber));
            if (category   == default) throw new ArgumentNullException(nameof(category));
            if (sort       == default) throw new ArgumentNullException(nameof(sort));
            if (order      == default) throw new ArgumentNullException(nameof(order));

            var pMode  = $"zroute={mode}";
            var pPage  = $"&zpage={pageNumber}";
            var pCat   = category    == "" ? string.Empty : $"&tab={category}";
            var pSort  = sort        == "" ? string.Empty : $"&sort={sort}";
            var pOrder = order       == "" ? "&order=asc" : $"&order={order}";
            var pTags  = tags.Length == 0  ? string.Empty : $"&tags={string.Join("&tags=", tags)}";

            string      api      = $"http://live.nicovideo.jp/api/getzappinglist?{pMode}{pPage}{pCat}{pSort}{pOrder}{pTags}";
            HttpContent response = Nico2Signal.Get(api, _cookie).Content;

            return response.ReadAsStreamAsync().Result;
        }
    }
}
