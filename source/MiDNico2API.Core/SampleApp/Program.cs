using MiDNico2API.Core;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Linq;

namespace SampleApp
{
    /// <summary>
    /// ----------------
    /// サンプル説明
    /// ----------------
    /// MiDNico2API.Coreの利用方法を紹介するためのサンプルプログラムです.
    /// 
    /// === 動作環境 ===
    /// ○ .NET Framework 4.6.1
    /// ○ C# 7.0
    /// 
    /// ----------------
    /// 注意事項
    /// ----------------
    /// 全提供APIのサンプルコードを記述する上で必要なデータマッピング等の処理が入っております.
    /// ただし, サンプルのためのモックコードのため正常動作しない場合がございます.
    /// あくまでもサンプルと割り切ってご利用ください.
    /// 
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            /* ===================================================================== *
             * 001 : ニコニコ動画にログインするサンプルコード
             * (公式API : login)
             * ===================================================================== */

            // 1 - login を利用するために必要なパラメータを用意.
            #region 個人情報
            const string email    = "********";    // ログイン用メールアドレス
            const string password = "********";    // ログイン用パスワード
            #endregion

            // 2 - login の実行.
            // ---------------------------------------
            // Cookie情報を取得可能.
            // 取得したCookie情報を利用して他のAPIを使用する.
            CookieContainer cookie = Nico2Auth.Login(email, password);



            /* ===================================================================== *
             * 002 : ニコニコ生放送番組とのセッションを維持するサンプルコード
             * (公式API : heartbeat)
             * ===================================================================== */

            // 1 - heartbeat を利用するために必要なパラメータを用意.
            CookieContainer cookie001 = cookie;       // Nico2Auth.Login()で取得したCookie情報.
            const int nico2liveId001  = 314630813;    // 接続先の生放送番組ID. (lv○○の○○部分).

            // 2 - heartbeat の実行.
            // ---------------------------------------
            // heartbeatは一度だけではなく, 定期的に実行しなければならないので注意.
            // また, XML形式でレスポンスが返ってくる.
            Stream responseHeartbeat = Nico2Auth.HeartBeat(cookie001, nico2liveId001);



            /* ===================================================================== *
             * 003 : ニコニコ生放送の番組情報を取得するサンプルコード
             * (公式API : getplayerstatus)
             * ===================================================================== */

            // 1 - getplayerstatus を利用するために必要なパラメータを用意.
            CookieContainer cookie002 = cookie;          // Nico2Auth.Login()で取得したCookie情報.
            const int nico2liveId002  = nico2liveId001;  // 接続先の生放送番組ID. (lv○○の○○部分).

            // 2 - getplayerstatus の実行.
            // ---------------------------------------
            // getplayerstatusのみ, lv○○ or co○○ で指定可能.
            // また, 他のAPIと違いlvやcoなどのプレフィクスが必須となっている点に注意.
            // ---------------------------------------
            // レスポンスは以下のような形式で返ってくる.
            //
            #region getplayerstatus レスポンス
            //<?xml version="1.0" encoding="utf-8"?>
            //<getplayerstatus status="ok" time="1531825142">
            //	<stream>
            //		<id>lv314492471</id>
            //		<title>【C#】楽しくプログラミング！	</title>
            //		<description>C#を使って楽しくプログラミン グしていきます。</description>
            //		<provider_type>community</provider_type>
            //		<default_community>co3823389</default_community>
            //		<international>13</international>
            //		<is_owner>1</is_owner>
            //		<owner_id>13882986</owner_id>
            //		<owner_name>Midori</owner_name>
            //		<is_reserved>0</is_reserved>
            //		<is_niconico_enquete_enabled>1</is_niconico_enquete_enabled>
            //		<watch_count>15</watch_count>
            //		<comment_count>3</comment_count>
            //		<base_time>1531824016</base_time>
            //		<open_time>1531824016</open_time>
            //		<start_time>1531824041</start_time>
            //		<end_time>1531825841</end_time>
            //		<is_rerun_stream>0</is_rerun_stream>
            //		<bourbon_url>http://live.nicovideo.jp/gate/lv314492471?sec=nicolive_crowded&amp;sub=watch_crowded_0_community_lv314492471_onair</bourbon_url>
            //		<full_video>http://live.nicovideo.jp/gate/lv314492471?sec=nicolive_crowded&amp;sub=watch_crowded_0_community_lv314492471_onair</full_video>
            //		<after_video></after_video>
            //		<before_video></before_video>
            //		<kickout_video>http://live.nicovideo.jp/gate/lv314492471?sec=nicolive_oidashi&amp;sub=watchplayer_oidashialert_0_community_lv314492471_onair</kickout_video>
            //		<twitter_tag>#co3823389</twitter_tag>
            //		<danjo_comment_mode>0</danjo_comment_mode>
            //		<infinity_mode>0</infinity_mode>
            //		<archive>0</archive>
            //		<press>
            //			<display_lines>-1</display_lines>
            //			<display_time>-1</display_time>
            //			<style_conf></style_conf>
            //		</press>
            //		<plugin_delay></plugin_delay>
            //		<plugin_url></plugin_url>
            //		<plugin_urls />
            //		<allow_netduetto>0</allow_netduetto>
            //		<broadcast_token>ca3fdbffcfcbc2f9a9991e8c457b98bbdd6b8c09</broadcast_token>
            //		<ng_scoring>0</ng_scoring>
            //		<is_nonarchive_timeshift_enabled>0</is_nonarchive_timeshift_enabled>
            //		<is_timeshift_reserved>0</is_timeshift_reserved>
            //		<header_comment>0</header_comment>
            //		<footer_comment>0</footer_comment>
            //		<split_bottom>0</split_bottom>
            //		<split_top>0</split_top>
            //		<background_comment>0</background_comment>
            //		<font_scale></font_scale>
            //		<comment_lock>0</comment_lock>
            //		<telop>
            //			<enable>0</enable>
            //		</telop>
            //		<contents_list>
            //			<contents id="main" disableAudio="0" disableVideo="0" start_time="1531824042">
            //				rtmp:rtmp://nlpoca118.live.nicovideo.jp:1935/publicorigin/180717_19_0/,lv314492471?1531825142:30:80bb1416f9f6ab7e
            //			</contents>
            //		</contents_list>
            //		<picture_url>http://dcdn.cdn.nimg.jp/comch/community-icon/128x128/co3823389.jpg?1531823792</picture_url>
            //		<thumb_url>http://dcdn.cdn.nimg.jp/comch/community-icon/64x64/co3823389.jpg?1531823792</thumb_url>
            //		<is_priority_prefecture></is_priority_prefecture>
            //	</stream>
            //	<user>
            //		<user_id>13882986</user_id>
            //		<nickname>Midori</nickname>
            //		<is_premium>1</is_premium>
            //		<userAge>28</userAge>
            //		<userSex>1</userSex>
            //		<userDomain>jp</userDomain>
            //		<userPrefecture>27</userPrefecture>
            //		<userLanguage>ja-jp</userLanguage>
            //		<room_label>co3823389</room_label>
            //		<room_seetno>2525025</room_seetno>
            //		<is_join>1</is_join>
            //		<twitter_info>
            //			<status>disabled</status>
            //			<screen_name></screen_name>
            //			<followers_count>0</followers_count>
            //			<is_vip>0</is_vip>
            //			<profile_image_url>http://abs.twimg.com/sticky/default_profile_images/default_profile_6_normal.png</profile_image_url>
            //			<after_auth>0</after_auth>
            //			<tweet_token>7165d886048b84b72364f058f25ed763a3da0049</tweet_token>
            //		</twitter_info>
            //	</user>
            //	<rtmp is_fms="1" rtmpt_port="80">
            //		<url>rtmp://nleu23.live.nicovideo.jp:1935/liveedge/live_180717_19_7</url>
            //		<ticket>15882654:lv314492471:4:1533822142:av5ebe8dcbe9e982</ticket>
            //	</rtmp>
            //	<ms>
            //		<addr>msg102.live.nicovideo.jp</addr>
            //		<port>2819</port>
            //		<thread>1627842355</thread>
            //	</ms>
            //	<tid_list>
            //		<tid>1628842350</tid>
            //		<tid>1628842351</tid>
            //	</tid_list>
            //	<ms_list>
            //		<ms>
            //			<addr>msg102.live.nicovideo.jp</addr>
            //			<port>2819</port>
            //			<thread>1628842350</thread>
            //		</ms>
            //		<ms>
            //			<addr>msg103.live.nicovideo.jp</addr>
            //			<port>2829</port>
            //			<thread>1628842351</thread>
            //		</ms>
            //	</ms_list>
            //	<twitter>
            //		<live_enabled>1</live_enabled>
            //		<vip_mode_count>10000</vip_mode_count>
            //		<live_api_url>http://watch.live.nicovideo.jp/api/</live_api_url>
            //	</twitter>
            //	<player>
            //		<qos_analytics>0</qos_analytics>
            //		<dialog_image>
            //			<oidashi>http://nicolive.cdn.nimg.jp/live/simg/img/201311/281696.a29344.png</oidashi>
            //		</dialog_image>
            //		<is_notice_viewer_balloon_enabled>1</is_notice_viewer_balloon_enabled>
            //		<error_report>1</error_report>
            //	</player>
            //	<marquee>
            //		<category>一般(その他)</category>
            //		<game_key>53895a18</game_key>
            //		<game_time>1531825142</game_time>
            //		<force_nicowari_off>1</force_nicowari_off>
            //	</marquee>
            //</getplayerstatus>
            #endregion

            Stream liveInfo = Nico2LiveInfo.Access(cookie002).GetInfo($"lv{nico2liveId002}");

            // ---------------------------------------------------------------------
            // [サンプルコード] getplayerstatusのparse :: LINQ to XML
            #region サンプル
            var liveInfoDoc = XDocument.Load(liveInfo);

            // ---------------------------------------------------------------------
            // getplayerstatus の属性
            var res_status = liveInfoDoc.Root.Attribute("status")?.Value;    // ok:取得成功, fail:取得失敗
            var res_time   = liveInfoDoc.Root.Attribute("time")?.Value;      // 取得時刻

            // ---------------------------------------------------------------------
            // stream 要素 : 生放送番組情報
            var res_stream = liveInfoDoc.Root.Element("stream");
            #region stream の子要素
            var stream_id                                   = res_stream?.Element("id")?.Value;                      // 生放送番組ID
            var stream_title                                = res_stream?.Element("title")?.Value;                   // 生放送番組タイトル
            var stream_description                          = res_stream?.Element("description")?.Value;             // 生放送番組説明
            var stream_provider_type                        = res_stream?.Element("provider_type")?.Value;
            var stream_default_community                    = res_stream?.Element("default_community")?.Value;
            var stream_international                        = res_stream?.Element("international")?.Value;
            var stream_is_owner                             = res_stream?.Element("is_owner")?.Value;
            var stream_owner_id                             = res_stream?.Element("owner_id")?.Value;
            var stream_owner_name                           = res_stream?.Element("owner_name")?.Value;
            var stream_is_reserved                          = res_stream?.Element("is_reserved")?.Value;
            var stream_is_niconico_enquete_enabled          = res_stream?.Element("is_niconico_enquete_enabled")?.Value;
            var stream_watch_count                          = res_stream?.Element("watch_count")?.Value;
            var stream_comment_count                        = res_stream?.Element("comment_count")?.Value;
            var stream_base_time                            = res_stream?.Element("base_time")?.Value;
            var stream_open_time                            = res_stream?.Element("open_time")?.Value;
            var stream_start_time                           = res_stream?.Element("start_time")?.Value;
            var stream_end_time                             = res_stream?.Element("end_time")?.Value;
            var stream_is_rerun_stream                      = res_stream?.Element("is_rerun_stream")?.Value;
            var stream_bourbon_url                          = res_stream?.Element("bourbon_url")?.Value;
            var stream_full_video                           = res_stream?.Element("full_video")?.Value;
            var stream_after_video                          = res_stream?.Element("after_video")?.Value;
            var stream_before_video                         = res_stream?.Element("before_video")?.Value;
            var stream_kickout_video                        = res_stream?.Element("kickout_video")?.Value;
            var stream_twitter_tag                          = res_stream?.Element("twitter_tag")?.Value;
            var stream_danjo_comment_mode                   = res_stream?.Element("danjo_comment_mode")?.Value;
            var stream_infinity_mode                        = res_stream?.Element("infinity_mode")?.Value;
            var stream_archive                              = res_stream?.Element("archive")?.Value;
            var stream_press                                = res_stream?.Element("press");
            // --- start : stream_pressの子要素
            var stream_press_display_lines                  = stream_press?.Element("display_lines")?.Value;
            var stream_press_display_time                   = stream_press?.Element("display_time")?.Value;
            var stream_press_style_conf                     = stream_press?.Element("display_lines")?.Value;
            // --- end   : stream_pressの子要素
            var stream_plugin_delay                         = res_stream?.Element("plugin_delay")?.Value;
            var stream_plugin_url                           = res_stream?.Element("plugin_url")?.Value;
            var stream_presplugin_urls                      = res_stream?.Element("plugin_urls")?.Value;
            var stream_allow_netduetto                      = res_stream?.Element("allow_netduetto")?.Value;
            var stream_broadcast_token                      = res_stream?.Element("broadcast_token")?.Value;
            var stream_ng_scoring                           = res_stream?.Element("ng_scoring")?.Value;
            var stream_is_nonarchive_timeshift_enabled      = res_stream?.Element("is_nonarchive_timeshift_enabled")?.Value;
            var stream_is_timeshift_reserved                = res_stream?.Element("is_timeshift_reserved")?.Value;
            var stream_header_comment                       = res_stream?.Element("header_comment")?.Value;
            var stream_footer_comment                       = res_stream?.Element("footer_comment")?.Value;
            var stream_split_bottom                         = res_stream?.Element("split_bottom")?.Value;
            var stream_split_top                            = res_stream?.Element("split_top")?.Value;
            var stream_background_comment                   = res_stream?.Element("background_comment")?.Value;
            var stream_font_scale                           = res_stream?.Element("font_scale")?.Value;
            var stream_comment_lock                         = res_stream?.Element("comment_lock")?.Value;
            var stream_telop                                = res_stream?.Element("telop");
            // --- start : stream_telopの子要素
            var stream_telop_enable                         = stream_telop?.Element("enable")?.Value;
            // --- end   : stream_telopの子要素
            var stream_contents_list                        = res_stream?.Element("contents_list");
            // --- start : stream_contents_listの子要素
            var stream_contents_list_contents               = stream_contents_list?.Elements("contents");
            // --- end   : stream_contents_listの子要素
            var stream_picture_url                          = res_stream?.Element("picture_url")?.Value;
            var stream_thumb_url                            = res_stream?.Element("thumb_url")?.Value;
            var stream_is_priority_prefecture               = res_stream?.Element("is_priority_prefecture")?.Value;
            #endregion  stream の子要素

            // ---------------------------------------------------------------------
            // user 要素 : 自分自身の情報
            var res_user = liveInfoDoc.Root.Element("user");
            #region user の子要素
            var user_user_id                            = res_user?.Element("user_id")?.Value;
            var user_nickname                           = res_user?.Element("nickname")?.Value;
            var user_is_premium                         = res_user?.Element("is_premium")?.Value;
            var user_userAge                            = res_user?.Element("userAge")?.Value;
            var user_userSex                            = res_user?.Element("userSex")?.Value;
            var user_userDomain                         = res_user?.Element("userDomain")?.Value;
            var user_userPrefecture                     = res_user?.Element("userPrefecture")?.Value;
            var user_userLanguage                       = res_user?.Element("userLanguage")?.Value;
            var user_room_label                         = res_user?.Element("room_label")?.Value;
            var user_room_seetno                        = res_user?.Element("room_seetno")?.Value;
            var user_is_join                            = res_user?.Element("is_join")?.Value;
            var user_twitter_info                       = res_user?.Element("twitter_info");
            // --- start : user_twitter_infoの子要素
            var user_twitter_info_status                = user_twitter_info?.Element("status")?.Value;
            var user_twitter_info_screen_name           = user_twitter_info?.Element("screen_name")?.Value;
            var user_twitter_info_followers_count       = user_twitter_info?.Element("followers_count")?.Value;
            var user_twitter_info_is_vip                = user_twitter_info?.Element("is_vip")?.Value;
            var user_twitter_info_profile_image_url     = user_twitter_info?.Element("profile_image_url")?.Value;
            var user_twitter_info_after_auth            = user_twitter_info?.Element("after_auth")?.Value;
            var user_twitter_info_tweet_token           = user_twitter_info?.Element("tweet_token")?.Value;
            // --- end   : user_twitter_infoの子要素
            #endregion user の子要素

            // ---------------------------------------------------------------------
            // rtmp 要素
            var res_rtmp = liveInfoDoc.Root.Element("rtmp");
            #region rtmp の属性・子要素
            // rtmp の属性
            var rtmp_is_fms     = res_rtmp?.Attribute("is_fms")?.Value;
            var rtmp_rtmpt_port = res_rtmp?.Attribute("rtmpt_port")?.Value;
            // rtmp の子要素
            var rtmp_url        = res_rtmp?.Element("url")?.Value;
            var rtmp_ticket     = res_rtmp?.Element("ticket")?.Value;
            #endregion rtmp の属性・子要素

            // ---------------------------------------------------------------------
            // ms 要素
            var res_ms = liveInfoDoc.Root.Element("ms");
            #region ms の子要素
            var ms_addr   = res_ms?.Element("addr")?.Value;
            var ms_port   = res_ms?.Element("port")?.Value;
            var ms_thread = res_ms?.Element("thread")?.Value;
            #endregion ms の子要素

            // ---------------------------------------------------------------------
            // tid_list 要素
            var res_tid_list = liveInfoDoc.Root.Element("tid_list");
            #region tid_list の子要素
            var tid_list_tid = res_ms?.Elements("tid");
            #endregion tid_list の子要素

            // ---------------------------------------------------------------------
            // ms_list 要素
            var res_ms_list = liveInfoDoc.Root.Element("ms_list");
            #region ms_list の子要素
            var ms_list_ms  = res_ms_list?.Elements("ms");
            #endregion ms_list の子要素

            // ---------------------------------------------------------------------
            // twitter 要素
            var res_twitter = liveInfoDoc.Root.Element("twitter");
            #region twitter の子要素
            var twitter_live_enabled   = res_ms_list?.Element("live_enabled")?.Value;
            var twitter_vip_mode_count = res_ms_list?.Element("vip_mode_count")?.Value;
            var twitter_live_api_url   = res_ms_list?.Element("live_api_url")?.Value;
            #endregion twitter の子要素

            // ---------------------------------------------------------------------
            // player 要素
            var res_player = liveInfoDoc.Root.Element("player");
            #region player の子要素
            var player_qos_analytics                    = res_player?.Element("qos_analytics")?.Value;
            var player_dialog_image                     = res_player?.Element("dialog_image");
            // --- start : player_dialog_imageの子要素
            var player_dialog_image_oidashi             = player_dialog_image?.Element("oidashi")?.Value;
            // --- end   : player_dialog_imageの子要素
            var player_is_notice_viewer_balloon_enabled = res_player?.Element("is_notice_viewer_balloon_enabled")?.Value;
            var player_error_report                     = res_player?.Element("error_report")?.Value;
            #endregion player の子要素

            // ---------------------------------------------------------------------
            // marquee 要素
            var res_marquee = liveInfoDoc.Root.Element("marquee");
            #region marquee の子要素
            var marquee_category            = res_marquee?.Element("category")?.Value;
            var marquee_game_key            = res_marquee?.Element("game_key")?.Value;
            var marquee_game_time           = res_marquee?.Element("game_time")?.Value;
            var marquee_force_nicowari_off  = res_marquee?.Element("force_nicowari_off")?.Value;
            #endregion marquee の子要素

            #endregion サンプル


            
            /* ===================================================================== *
             * 004 : ニコニコ生放送の番組情報を取得するサンプルコード
             * (非公式API : consumeComment)
             * ===================================================================== */
             
            // 1 - comment 取得用のクライアントを生成するために必要なパラメータを用意.
            CookieContainer cookie003 = cookie;                // Nico2Auth.Login()で取得したCookie情報.
            IPEndPoint      ipendpoint = default(IPEndPoint);  // ニコニコ生放送番組へのIPEndPoint.
            {
                #region IPEndPoint 生成サンプル
                IPHostEntry host      = Dns.GetHostEntry(ms_addr);
                IPAddress   ipaddress = host?.AddressList?.FirstOrDefault();
                int.TryParse(ms_port, out int port);

                ipendpoint = new IPEndPoint(ipaddress, port);
                #endregion
            }
            // comment 取得用クライアントをインスタンス化.
            var commentClient = Nico2Comment.Create(cookie003, ipendpoint);
            
            // 2 - consumeComment を利用するために必要なパラメータを用意.
            // ニコニコ生放送番組のスレッドIDを取得.
            int.TryParse(ms_thread, out int nico2thread);

            // 3 - consumeComment の実行.
            var res = commentClient.ConnectAsync(
                nico2thread,        // 生放送番組のスレッドID
                data =>             // コメント取得時の処理
                {               
                    #region コメント取得時の処理
                    // ----------------------------------------------
                    // xmlデータからコメントデータを抽出する
                    var commentDoc = XDocument.Load(data);

                    var comment_text      = commentDoc.Root?.Value;                         // コメント.
                    var comment_thread    = commentDoc.Root.Attribute("thread")?.Value;     // 生放送番組スレッドID:立ち見やアリーナ分け用.
                    var comment_number    = commentDoc.Root.Attribute("no")?.Value;         // コメント番号.
                    var comment_vpos      = commentDoc.Root.Attribute("vpos")?.Value;       // コメントした時間(long型:放送開始からの経過時間. 100で割ると秒単位になる.)
                    var comment_date      = commentDoc.Root.Attribute("date")?.Value;       // コメントした日時(long型:1970/1/1 00:00:00 からの経過時間. 秒単位.)
                    var comment_date_usec = commentDoc.Root.Attribute("date_usec")?.Value;  // └ dateの小数点以下の秒数.
                    var comment_user_id   = commentDoc.Root.Attribute("user_id")?.Value;    // コメントしたユーザのID.
                    var comment_anonymity = commentDoc.Root.Attribute("anonymity")?.Value;  // コメントしたユーザが184でコメントした場合のみ送られてくる.
                    var comment_premium   = commentDoc.Root.Attribute("premium")?.Value;    // コメントしたユーザがプレミアム会員の場合のみ送られてくる.
                    var comment_mail      = commentDoc.Root.Attribute("mail")?.Value;       // コメントに対する付加コマンド.

                    var usr_id = comment_anonymity == null ? comment_user_id : "184";
                    Console.WriteLine($"{comment_number}::{comment_text} [{usr_id}]");
                    #endregion
                },
                bufSize: 1024,      // [オプション] 一度に取得するコメントサイズ
                historyFrom: 100    // [オプション] 過去何件まで遡ってコメントを取得するかの指定
            );

            

            /* ===================================================================== *
             * 005 : ニコニコ生放送の番組にコメントを投稿するサンプルコード
             * (非公式API : postComment)
             * ===================================================================== */
             
            // 1 - comment 投稿用クライアントを生成する.
            // 1 - 1 : comment 投稿用クライアント生成用のパラメータを準備する.
            CookieContainer cookie004     = cookie;                 // Nico2Auth.Login()で取得したCookie情報.
            IPEndPoint      ipendpoint002 = default(IPEndPoint);    // ニコニコ生放送番組へのIPEndPoint.
            {
                #region IPEndPoint 生成サンプル
                IPHostEntry host = Dns.GetHostEntry(ms_addr);
                IPAddress ipaddress = host?.AddressList?.FirstOrDefault();
                int.TryParse(ms_port, out int port);

                ipendpoint002 = new IPEndPoint(ipaddress, port);
                #endregion
            }
            // 1 - 2 : ニコニコ生放送コメント投稿用クライアントを生成する.
            Nico2Comment client = Nico2Comment.Create(cookie004, ipendpoint002);

            // 2 - comment 投稿用の必須パラメータを生成する.
            string message = "test message";                        // 投稿するコメント内容.
            string userid  = user_user_id;                          // 自分自身のユーザID.
            int .TryParse(ms_thread, out int nico2thread002);       // ニコニコ生放送番組のスレッドID.
            long.TryParse(stream_base_time, out long startTime);    // ニコニコ生放送番組が開始した時間.
            bool isPremium = user_is_premium != "0";                // プレミアム会員か否か.

            // 3 - commentを投稿する.
            // ---------------------------
            // [オプション] の順番はどちらが先でも問題ない.
            client.Send(
                message, 
                userid, 
                nico2thread002, 
                startTime, 
                isPremium, 
                "white",        // [オプション] コメント色を指定.
                "small"         // [オプション] コメントのサイズを指定.
            );



            /* ===================================================================== *
             * 006 : ニコニコ生放送の番組に生主コメントを投稿するサンプルコード
             * (非公式API : postCommentAsAdmin)
             * -------------------------------------
             * 生放送主以外のユーザで使用しても動作しないので注意.
             * ===================================================================== */
             
            // 1 - comment 投稿用クライアントを生成する.
            // 1 - 1 : comment 投稿用クライアント生成用のパラメータを準備する.
            CookieContainer cookie005     = cookie;                 // Nico2Auth.Login()で取得したCookie情報.
            IPEndPoint      ipendpoint003 = default(IPEndPoint);    // ニコニコ生放送番組へのIPEndPoint.
            {
                #region IPEndPoint 生成サンプル
                IPHostEntry host      = Dns.GetHostEntry(ms_addr);
                IPAddress   ipaddress = host?.AddressList?.FirstOrDefault();
                int.TryParse(ms_port, out int port);

                ipendpoint003 = new IPEndPoint(ipaddress, port);
                #endregion
            }
            // 1 - 2 : ニコニコ生放送コメント投稿用クライアントを生成する.
            Nico2Comment client002 = Nico2Comment.Create(cookie005, ipendpoint003);

            // 2 - comment 投稿用の必須パラメータを生成する.
            string adminMsg       = "message";          // 投稿するコメント内容.
            int    nico2liveId003 = nico2liveId001;     // 接続先の生放送番組ID.

            // 3 - commentを投稿する.
            client002.SendAsAdmin(
                adminMsg,
                nico2liveId003,
                false,          // [オプション] 投稿したコメントを生主コメント欄に固定する場合, true を指定.
                "white",        // [オプション] コメント色を指定.
                "midori"        // [オプション] コメントに付与するニックネームを指定.
            );



            /* ===================================================================== *
             * 007 : ユーザ情報を取得するサンプルコード
             * (公式API : user/info)
             * ===================================================================== */

            // 1 - ユーザ情報取得用パラメータを準備する.
            string targetUserId = "13882986";

            // 2 - ユーザ情報を取得する.
            Stream userInfo = Nico2UserInfo.Take(targetUserId);

            // ---------------------------------------------------------------------
            // [サンプルコード] user/infoのparse :: LINQ to XML
            var userInfoDoc = XDocument.Load(userInfo);
            #region サンプル
            var userInfo_status = liveInfoDoc.Root.Attribute("status")?.Value;                      // ok:取得成功, fail:取得失敗
            
            var userInfo_stream             = userInfoDoc.Root.Element("user");                     // ユーザ情報
            var userInfo_user_id            = userInfo_stream?.Element("id")?.Value;                // ユーザID
            var userInfo_user_nickname      = userInfo_stream?.Element("nickname")?.Value;          // ユーザ名
            var userInfo_user_thumbnail_url = userInfo_stream?.Element("thumbnail_url")?.Value;     // ユーザサムネイル画像URL

            Console.WriteLine($"STATUS::{userInfo_status}");
            Console.WriteLine($"USER_ID::{userInfo_user_id}");
            Console.WriteLine($"USER_NAME::{userInfo_user_nickname}");
            Console.WriteLine($"USER_THUMBNAIL::{userInfo_user_thumbnail_url}");
            #endregion サンプル

            Console.WriteLine("end");
            Console.ReadKey();
        }
    }
}
