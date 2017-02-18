// プロトコールの定義
// Client と Server 共通に使います。
using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Net.Sockets;
using MsgPack.Serialization;
using System.IO;

namespace Network.Proto
{
    public enum ProtoType : int
    {
        Admin,          // 管理者
        UserAuth,       // 認証
        UserChat,       // ユーザチャット
        UserInfo,       // ユーザ情報
        Loction,        // ロケーション取得
        CmdMove,        // 移動実行コマンド
        UserMove,       // ユーザが移動
    }

    static public class ProtoMaker
    {
        static public byte[] Pack(ProtoType type)
        {
            byte[] proto = BitConverter.GetBytes((int)type);
            byte[] size = BitConverter.GetBytes(proto.Length);
            return size.Concat(proto).ToArray();
        }
        static public byte[] Pack<T>(ProtoType type, T obj)
        {
            var serializer = SerializationContext.Default.GetSerializer<T>();
            using (var ms = new MemoryStream())
            {
                serializer.Pack(ms, obj);
                byte[] data = ms.ToArray();
                byte[] proto = BitConverter.GetBytes((int)type);
                byte[] size = BitConverter.GetBytes(data.Length + proto.Length);
                return size.Concat(proto).Concat(data).ToArray();
            }
        }
        static public ProtoType GetProtoType(byte[] bytes)
        {
            return (ProtoType)BitConverter.ToInt32(bytes, 0);
        }
        static public T Unpack<T>(byte[] bytes)
        {
            var serializer = SerializationContext.Default.GetSerializer<T>();
            using (var ms = new MemoryStream(bytes.Skip(4).ToArray()))
            {
                return serializer.Unpack(ms);
            }
        }
    }
}

// From Client send to Server
namespace Network.Proto
{
    static public class Client2Server
    {
        public class Admin
        {
            public string Message;
        }
        public class UserAuth
        {
            public string Username;    // ユーザ名
            public string Password;    // パスワード
        }
        public class UserChat
        {
            public string Message;      // メッセージ
        }
    }
}

// From Server send to Client
namespace Network.Proto
{
    static public class Server2Client
    {
        public class Admin
        {
            public string Message;
        }
        public class UserAuth
        {
            public string Username;     // ユーザ名
        }
        public class UserInfo
        {
            public int numUser;         // ユーザ数
            public string[] Username;   // ユーザ名一覧
        }
        public class UserChat
        {
            public string Username;     // ユーザ名
            public string Message;      // メッセージ
        }
    }
}
