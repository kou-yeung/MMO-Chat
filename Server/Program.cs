// MMORPGゲームサーバプログラミング
// 第２章:チャットサーバ
// C# & Unity アレンジサンプル
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Network.Proto;
using System.Linq;

namespace Server
{
    class Program
    {
        static List<Session> sessions = new List<Session>();

        static void ReceivedEvent(Session session, byte[] bytes)
        {
            switch (ProtoMaker.GetProtoType(bytes))
            {
                case ProtoType.Admin:
                    var admin = ProtoMaker.Unpack<Client2Server.Admin>(bytes);
                    Console.WriteLine(admin.Message);
                    break;
                case ProtoType.UserAuth:
                    var auth = ProtoMaker.Unpack<Client2Server.UserAuth>(bytes);
                    Console.WriteLine("Username : {0}, Password : {1}", auth.Username, auth.Password);
                    // TODO : DBから認証情報をチェック
                    session.userInfo.Auth();
                    session.userInfo.Username = auth.Username;
                    session.Send(ProtoMaker.Pack(ProtoType.UserAuth, new Server2Client.UserAuth
                    {
                        Username = session.userInfo.Username,
                    }));
                    break;
                case ProtoType.UserInfo:
                    var info = new Server2Client.UserInfo();
                    info.numUser = sessions.Count;
                    info.Username = new string[sessions.Count];
                    for (var i = 0; i < sessions.Count; ++i)
                    {
                        info.Username[i] = sessions[i].userInfo.Username;
                    }
                    session.Send(ProtoMaker.Pack(ProtoType.UserInfo, info));
                    break;
                case ProtoType.UserChat:
                    var c2sChat = ProtoMaker.Unpack<Client2Server.UserChat>(bytes);
                    var s2CChat = ProtoMaker.Pack(ProtoType.UserChat, new Server2Client.UserChat
                    {
                        Username = session.userInfo.Username,
                        Message = c2sChat.Message
                    });
                    // 全員配信
                    for (var i = 0; i < sessions.Count; ++i)
                    {
                        sessions[i].Send(s2CChat);
                    }
                    break;
                case ProtoType.CmdMove:
                    // 全員配信
                    var s2CMove = ProtoMaker.Pack(ProtoType.UserMove);
                    for (var i = 0; i < sessions.Count; ++i)
                    {
                        sessions[i].Send(s2CMove);
                    }
                    break;
            }
        }

        static void TaskReceive(TcpListener listener)
        {
            while (true)
            {
                // 接続要求あるかどうか確認
                if (listener.Pending())
                {
                    // 接続要求を処理する
                    var client = listener.AcceptTcpClient();
                    sessions.Add(new Session(client, ReceivedEvent));
                    Console.WriteLine("AcceptTcpClient : {0}", client.Client.RemoteEndPoint );
                }
                // 受信処理
                foreach (var session in sessions)
                {
                    session.Poll();
                }

                // 接続切断したものがあれば削除
                if (sessions.RemoveAll(s => s.Disconnected) > 0)
                {
                    // TODO : 以下コピペーなのでリファクタリング
                    var info = new Server2Client.UserInfo();
                    info.numUser = sessions.Count;
                    info.Username = new string[sessions.Count];
                    for (var i = 0; i < sessions.Count; ++i)
                    {
                        info.Username[i] = sessions[i].userInfo.Username;
                    }
                    // 削除されたものがあるので情報を再送信
                    foreach (var session in sessions)
                    {
                        session.Send(ProtoMaker.Pack(ProtoType.UserInfo, info));
                    }
                }
                Task.Delay(16).Wait();
            }
        }

        static void TaskSend()
        {
            while (true)
            {
                Task.Delay(16).Wait();
            }
        }

        static void Main(string[] args)
        {
            // ローカルコンピューターのホスト名を取得。
            //IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
            //IPAddress ipAddress = ipHostInfo.AddressList[0];

            var ipString = "127.0.0.1";     // IPアドレス
            var port = 2001;                // Listenするポート番号

            IPAddress ipAddress = IPAddress.Parse(ipString);
            TcpListener listener = new TcpListener(ipAddress, port);

            listener.Start(100);

            List<Task> tasks = new List<Task>();

            tasks.Add(Task.Run(() => TaskReceive(listener)));
            tasks.Add(Task.Run(() => TaskSend()));

            Task.WaitAll(tasks.ToArray());
        }
    }
}

