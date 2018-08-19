# MiDNico2API.Core
## 1. 説明
[ニコニコ生放送用](http://live.nicovideo.jp/)のAPIを利用するためのライブラリです。

コメントを取得するまでの各APIの利用法については下記をご参照ください。

さらに詳細なサンプルにつきましては, [サンプルアプリ](https://github.com/Midoliy/MiDNico2API/blob/master/source/MiDNico2API.Core/SampleApp/Program.cs)をご参照ください。

### ***[ login ]***
ニコニコにログインするAPI.

ここで取得したCookie情報を他のAPIに利用する.

```cs
using MiDNico2API.Core;
using System;
using System.Net;

string email_address = "hoge@fuga.com"; // ニコニコログイン用メールアドレス.
string password      = "piyopiyo";      // ニコニコログイン用パスワード.

/**
 * ニコニコにログインしてCookie情報を取得する.
 */
CookieContainer cookie = Nico2Auth.Login(email_address, password);
```

---

### ***[ getplayerstatus ]***
指定したニコニコ生放送の番組情報を取得するAPI.

ここで取得した番組情報を元にコメントを取得するAPIを利用する.

```cs
using MiDNico2API.Core;
using System;
using System.Net;
using System.IO;

CookieContainer cookie; // ニコニコのCookie情報. 通常, Nico2Auth.Login()で取得したものを利用する.
string nico2liveId = "lv314408231"; // lvから始まる生放送番組ID.

/**
 * Cookie情報を利用して, 生放送番組情報を取得する.
 * 生放送番組情報はStreamとして取得できる.
 */
Nico2LiveInfo client    = Nico2LiveInfo.Access(cookie); // 生放送番組情報に関するAPIと通信するクラスを利用.
Stream        live_info = client.GetInfo(nico2liveId);  // 番組情報を取得.
```

---

### ***[ getcomment ]***
*getplayerstatus*で取得した番組情報を元に生放送コメントを取得する.

※ *getplayerstatus* で取得した情報はStream型のため、適宜クラスにマッピングするコードが必要になるため各自で用意してください。
※ もしくは現在、MiDNico2API.Nativesソリューションの方でMiDNico2API.Coreをラッピングしたライブラリを作成していますので、独自で用意することが難しい or 面倒な場合はそちらを利用してください。

```cs
using MiDNico2API.Core;
using System;
using System.Net;
using System.IO;

CookieContainer cookie;         // ニコニコのCookie情報. 通常, Nico2Auth.Login()で取得したものを利用する.
IpEndPoint ipendpoint;          // getplayerstatusで取得した番組情報から生成する.
int        nico2thread;         // getplayerstatusで取得した番組情報から取得する.
int        buf_size     = 4096; // 1度に取得してくるコメントの最大バイト数.
short      history_from = 500;  // 何件前のコメントから取得するかの設定.


/**
 * 対象の生放送からコメントを取得する.
 */
Nico2Comment client    = Nico2Comment.Create(cookie, ipendpoint);

await client.ConnectAsync(
    nico2thread,   // [必須]対象生放送番組のスレッドID.
    data => {      // [必須]コメントを受信した際の処理.
        /**
         * data は Stream 形式で来るので適宜ココに処理を記述してください. 
         * なお data は1件分のコメントデータとなります.
         */
    },
    buf_size,      // [オプション] default= 1024
    history_from   // [オプション] default= 100
);
```

---

## 2. ダウンロード
Nugetにて公開していますので、Visual StudioのNugetパッケージからの導入か下記からダウンロードしてお使いください。

ダウンロードはこちらから → [MiDNico2API](https://www.nuget.org/packages/MiDNico2API.Core/)

---

## 3. 動作環境
(1) 以下のいずれかで動作.

    .NET Framework 4.6.1 以上.
    .NET Standard  2.0   以上.    
    .NET Core      2.0   以上.
    
(2) C# 7.3 以上.
