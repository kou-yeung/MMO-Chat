// ソケット通信に必要なインタフェース
using System;

namespace Network
{
    interface ISocketService
    {
        void Connect(string hostname, int port);
        void Poll();
        void Send(byte[] bytes);
        bool Connected();
        void AddReceivedEvent(Action<byte[]> cb);
        void RemoveReceivedEvent(Action<byte[]> cb);
    }

    class SocketService : ServiceLocator<ISocketService> { }
}
