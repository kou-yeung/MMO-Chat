using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Network;
using Network.Proto;
using System;

public class Room : MonoBehaviour
{
    public Text messages;
    public Text usernames;
    public InputField input;

	void Start ()
    {
        SocketService.Locator.AddReceivedEvent(Received);
        // 入室
        SocketService.Locator.Send(ProtoMaker.Pack(ProtoType.CmdMove));
	}
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (!String.IsNullOrEmpty(input.text))
            {
                SocketService.Locator.Send(ProtoMaker.Pack(new Client2Server.UserChat
                {
                    Message = input.text
                }));
                input.text = "";
            }
        }
    }
    void OnDestroy()
    {
        SocketService.Locator.RemoveReceivedEvent(Received);
    }
    void Received(byte[] bytes)
    {
        switch (ProtoMaker.GetProtoType(bytes))
        {
            case ProtoType.UserInfo:
                var info = ProtoMaker.Unpack<Server2Client.UserInfo>(bytes);
                usernames.text = String.Join("\n", info.Username);
                break;
            case ProtoType.UserChat:
                var chat = ProtoMaker.Unpack<Server2Client.UserChat>(bytes);
                AddMessage(String.Format("{0} : {1}", chat.Username, chat.Message));
                break;
            case ProtoType.UserMove: // 誰か入室した
                // ユーザ一覧取得:手軽なのでこのサンプルは一覧すべて取得する
                SocketService.Locator.Send(ProtoMaker.Pack(ProtoType.UserInfo));
                break;
        }
    }

    void AddMessage(string str)
    {
        if (String.IsNullOrEmpty(messages.text))
        {
            messages.text = str;
        }
        else
        {
            messages.text = String.Format("{0}\n{1}", str, messages.text);
        }
    }
}
