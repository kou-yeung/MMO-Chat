
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using Network;
using Network.Proto;
using Logger;

public class Login : MonoBehaviour
{
    public InputField Id;
    public InputField Pw;

    void Start()
    {
        SocketService.Locator.AddReceivedEvent(Received);
    }
    void OnDestroy()
    {
        SocketService.Locator.RemoveReceivedEvent(Received);
    }
    public void OnLoginClick()
    {
        if (String.IsNullOrEmpty(Id.text)) return;
        if (String.IsNullOrEmpty(Pw.text)) return;

        var client = SocketService.Locator;
        if (!client.Connected()) client.Connect("127.0.0.1", 2001);
        client.Send(ProtoMaker.Pack(new Client2Server.UserAuth
        {
            Username = Id.text,
            Password = Pw.text,
        }));

        Pw.text = ""; // パスワードリセット
    }

    void Received(byte[] bytes)
    {
        switch (ProtoMaker.GetProtoType(bytes))
        {
            case ProtoType.Admin:
                var admin = ProtoMaker.Unpack<Server2Client.Admin>(bytes);
                LoggerService.Locator.Log(admin.Message);
                break;
            case ProtoType.UserAuth:
                var auth = ProtoMaker.Unpack<Server2Client.UserAuth>(bytes);
                LoggerService.Locator.Log("Username : {0}", auth.Username);
                SceneManager.LoadScene("Room"); // 入室
                break;
        }
    }
}
