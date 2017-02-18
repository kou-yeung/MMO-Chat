using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using Network;
using Network.Proto;
using Logger;

namespace Network
{
    public class TcpClientService : ISocketService
    {
        TcpClient client;
        NetworkStream stream;
        PacketHelper packet;
        Action<byte[]> ReceviedEvent;

        public void Connect(string hostname, int port)
        {
            client = new TcpClient(hostname, port);
            stream = client.GetStream();
            packet = new PacketHelper(stream);
        }

        public bool Connected()
        {
            if (client == null) return false;
            return client.Connected;
        }

        public void Poll()
        {
            if (!Connected()) return;

            while (client.Available > 0)
            {
                byte[] bytes;
                if (packet.Receive(out bytes))
                {
                    if (ReceviedEvent != null)
                    {
                        ReceviedEvent(bytes);
                    }
                }
            }
        }

        public void Send(byte[] bytes)
        {
            if (stream != null)
            {
                stream.Write(bytes, 0, bytes.Length);
                stream.Flush();
            }
        }

        public void AddReceivedEvent(Action<byte[]> cb)
        {
            ReceviedEvent += cb;
        }

        public void RemoveReceivedEvent(Action<byte[]> cb)
        {
            ReceviedEvent -= cb;
        }
    }
}
