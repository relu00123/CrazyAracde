using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_CAGameRoomScene : UI_Scene
{
    [SerializeField] private Sprite GameStartBtnImage;
    [SerializeField] private Sprite GameReadyBtnImage;

    enum Buttons
    {
        ToLobby,
        StartBtn,
    }

    enum GridPanels
    {
        UsersGridPanel,
    }

    public override void Init()
    {
        base.Init();

        Bind<Button>(typeof(Buttons));
        Bind<GridLayoutGroup>(typeof(GridPanels));

        GetButton((int)Buttons.ToLobby).gameObject.BindEvent(OnToLobbyButtonClicked);
        GetButton((int)Buttons.StartBtn).gameObject.BindEvent(OnStartBtnClicked);
    }
    public void OnToLobbyButtonClicked(PointerEventData evt)
    {
        Debug.Log("To Lobby Button Clicked!");

        C_EnterLobby enterLobbyPacket = new C_EnterLobby();
        enterLobbyPacket.Player = Managers.UserInfo.myLobbyPlayerInfo;

        Managers.Scene.LoadScene(Define.Scene.CAMainLobby);

        Managers.Network.Send(enterLobbyPacket);
    }


    public void OnStartBtnClicked(PointerEventData evt)
    {
        if (Managers.Room.host)
        {
            // 게임을 시작하자는 패킷을 Server에 보낸다.
            C_StartGame startgamepacket = new C_StartGame();
            Managers.Network.Send(startgamepacket);
        }

        else
        {
            //레디 상태로 변경하라는 패킷을 Server에 보낸다. 
            C_ReadybtnClicked readybtnClickedPacket = new C_ReadybtnClicked();
            Managers.Network.Send(readybtnClickedPacket);
        }
    }


    public UI_UsersGridPanel GetUIUserGridPanel()
    {
        return GetGridPanel((int)GridPanels.UsersGridPanel).gameObject.GetComponent<UI_UsersGridPanel>();
    }

    public void SetHost(bool ishost)
    {
        if (ishost)
            GetButton((int)Buttons.StartBtn).image.sprite = GameStartBtnImage;

        else
            GetButton((int)Buttons.StartBtn).image.sprite = GameReadyBtnImage; 
    }


}
