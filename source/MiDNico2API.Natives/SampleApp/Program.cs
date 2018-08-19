using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Nico2APIを利用するためには下記をincludeする必要があります.
using MiDNico2API.Windows;
using MiDNico2API.Contract;
using System;
using System.Net;

namespace SampleApp
{
    /// <summary>
    /// ----------------
    /// サンプル説明
    /// ----------------
    /// MiDNico2API.Nativesの利用方法を紹介するためのサンプルプログラムです.
    /// 今回はMiDNico2API.Windowsを利用していますが, MiDNico2API.DotNetも利用方法は全く同じです.
    /// 
    /// === 動作環境 ===
    /// ○ .NET Framework 4.7.1
    /// ○ C# 7.0
    /// 
    /// === 依存関係 ===
    /// 下記ライブラリをNugetよりインストールする必要があります.
    /// ├ System.Buffers
    /// └ System.Memory
    /// 
    /// </summary>
    class Program
    {
        static async Task Main(string[] args)
        {
            /* ===================================================================== *
             * 001 : ニコニコ動画からCookie情報を取得する方法.
             * ===================================================================== */

            // 1 - login を利用するために必要なパラメータを用意.
            #region
            string email = "";      // ニコニコログイン用メールアドレス
            string pass  = "";      // ニコニコログイン用パスワード
            #endregion
            // 2 - login の実行.
            //   - Cookie情報を取得可能.
            CookieContainer cookie001 = Nico2API.Login(email, pass);



            /* ===================================================================== *
             * 002 : ニコニコAPIを利用するためのクライアントを生成する.
             * ===================================================================== */

            // 1 - クライアント生成に必要なパラメータを用意.
            CookieContainer cookie002 = cookie001;  // ニコニコ動画のCookie情報
            int             liveId    = 314829021;  // ニコニコ生放送の番組ID

            // 2 - クライアントの生成.
            Nico2API client001 = new Nico2API(cookie002, liveId);       // [方法 A] newを使用してのインスタンス化. 
            Nico2API client002 = Nico2API.Create(cookie002, liveId);    // [方法 B] Createメソッドを使用してのインスタンス化.



            /* ===================================================================== *
             * 003 : ニコニコ生放送番組情報を取得する.
             * ===================================================================== */

            // 1 - 生放送番組情報を取得.
            PlayerStatus info = client001.GetPlayerStatus();

            // 2 - 取得した情報を表示してみる.
            Console.WriteLine($"コミュニティID      : {info.CommunityID}");
            Console.WriteLine($"生放送番組ID        : {info.LiveID}");
            Console.WriteLine($"生放送番組タイトル  : {info.Title}");
            Console.WriteLine($"生放送番組概要      : {info.Description}");
            Console.WriteLine($"生放送番組閲覧者数  : {info.WatchCount}");
            Console.WriteLine($"生放送番組コメント数: {info.CommentCount}");



            /* ===================================================================== *
             * 004 : ニコニコ生放送からコメントを取得する.
             * ===================================================================== */

            await client001.ConsumeCommentAsync(
                comment =>  // ここにコメント取得時の操作を記述する.
                {
                    int         no            = comment.Number;         // コメント投稿番号.
                    string      userId        = comment.UserID;         // コメント投稿ユーザID.
                    string      text          = comment.Text;           // コメント内容.
                    bool        is184         = comment.Anonymity;      // 184コメントの場合, true.
                    bool        isPremium     = comment.IsPremium;      // コメント投稿ユーザがプレミアム会員の場合, true.
                    Nico2Color  color         = comment.Color;          // コメント色.
                    Nico2Size   commentSize   = comment.Size;           // コメントサイズ.
                    Nico2Locate commentLocate = comment.Locate;         // コメントの位置.
                    long        pos           = comment.VideoPosition;  // コメント投稿時間(放送開始からの経過秒).
                    DateTime    postedDate    = comment.PostDate;       // コメント投稿時刻.

                    System.Drawing.Color sysColor = color.ToRGB();      // MiDNico2API.Windows版限定. コメント色をRGBに変換する.

                    // コメントを表示してみる.
                    Console.WriteLine($"[{no.ToString("000")}] {text}\t({userId})");
                });


            //await Nico2API.Create(cookie001, liveId).ConsumeCommentAsync(c=>{Console.WriteLine($"[{c.Number.ToString("000")}] {c.Text}");});


            Console.ReadKey();
        }
    }
}
