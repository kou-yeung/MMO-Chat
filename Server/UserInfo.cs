// ユーザ情報を保持される
using System;

namespace Server
{
    public class UserInfo
    {
        public string Username { get; set; }

        public void Auth()
        {
            Console.WriteLine("Auth!!!");
        }
    }
}
