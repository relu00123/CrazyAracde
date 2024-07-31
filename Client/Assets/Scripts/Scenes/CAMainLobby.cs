using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CAMainLobby : BaseScene
{
    UI_CAMainLobbyScene _sceneUI { get; set; }

    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.CAMainLobby;

        Screen.SetResolution(640, 480, false);

        _sceneUI = Managers.UI.ShowSceneUI<UI_CAMainLobbyScene>();

    }

    public override void Clear()
    {
    }

    public void HandleReceivedChatMessage(S_Chatting chatpacket)
    {
        _sceneUI.ChattingUI.AddToScrollView(chatpacket);
    }

    public void HandleAlterRoom(S_AlterRoom alterRoomPacket)
    {
        switch (alterRoomPacket.Altertype)
        {
            case RoomAlterType.Add:
                _sceneUI.GameRoomGridPanel.AddRoom(alterRoomPacket.Roominfo);
                break;

            case RoomAlterType.Delete:
                _sceneUI.GameRoomGridPanel.RemoveRoom(alterRoomPacket.Roominfo.RoomNumber);
                break;

            case RoomAlterType.Alter:
                _sceneUI.GameRoomGridPanel.ChangeRoomInfo(alterRoomPacket.Roominfo);
                break;
        }
    }


}
