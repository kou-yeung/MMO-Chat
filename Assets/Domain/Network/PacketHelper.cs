using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
namespace Network
{
    class PacketHelper
    {
        static public readonly int BufferSize = 1024;

        int? available;  // 残りデータ量
        List<byte> buffer = new List<byte>();
        byte[] bytes = new byte[BufferSize];
        Stream stream;
        StringBuilder sb = new StringBuilder();

        public PacketHelper(Stream stream)
        {
            this.stream = stream;
        }

        public bool Receive(out byte[] res)
        {
            res = default(byte[]);

            int readBytes = 0; // 実際に読みだせたデータ量

            if (available == null)
            {
                readBytes = stream.Read(bytes, 0, BufferSize);
                available = BitConverter.ToInt32(bytes, 0);     // データの長さ取得
                readBytes -= 4;                                 // サイズを減らす
                buffer.AddRange(bytes.Skip(4).Take(readBytes)); // 残りデータは buffer に追加
            }
            else
            {
                // 複数の通信が一緒に来ると想定し、残りデータ量以上読みだせないよう最大値制限
                readBytes = stream.Read(bytes, 0, Math.Min(BufferSize, available.Value));
                buffer.AddRange(bytes.Take(readBytes));
            }

            // 読み込んだ量を引きます
            available -= readBytes;

            // すべて読みだした
            if (available <= 0)
            {
                res = buffer.ToArray();
                available = null;
                buffer.Clear();
                return true;
            }
            return false;
        }
    }
}
