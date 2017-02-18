// ユーザのセッション情報
// TcpListenerにアクセス要求が来たときに作成されます
using Network;
using System.Net.Sockets;
using System;

namespace Server
{
    public class Session
    {
        TcpClient client;
        NetworkStream stream;                
        PacketHelper packet;
        Action<Session, byte[]> commandExec;

        public UserInfo userInfo { get; private set; }

        public Session(TcpClient client, Action<Session, byte[]> commandExec)
        {
            this.client = client;
            this.stream = client.GetStream();
            this.packet = new PacketHelper(this.stream);
            this.userInfo = new UserInfo();
            this.commandExec = commandExec;
        }
        public void Poll()
        {
            if (client == null) return;
            if (!client.Connected) return;

            while (client.Available > 0)
            {
                byte[] bytes;
                if (packet.Receive(out bytes))
                {
                    commandExec(this, bytes);
                }
            }
        }
        public void Send(byte[] bytes)
        {
            try
            {
                stream.Write(bytes, 0, bytes.Length);
                stream.Flush();
            } catch(Exception e)
            {
                client.Close();
                client = null;
            }
        }
        public bool Disconnected
        {
            get { return client == null; }
        }
    }
}
