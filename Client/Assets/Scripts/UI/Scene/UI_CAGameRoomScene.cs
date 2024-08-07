using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_CAGameRoomScene : UI_Scene
{
    enum Buttons
    {
        ToLobby,
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
    }
    public void OnToLobbyButtonClicked(PointerEventData evt)
    {
        Debug.Log("To Lobby Button Clicked!");

        C_EnterLobby enterLobbyPacket = new C_EnterLobby();
        enterLobbyPacket.Player = Managers.UserInfo.myLobbyPlayerInfo;

        Managers.Scene.LoadScene(Define.Scene.CAMainLobby);

        Managers.Network.Send(enterLobbyPacket);
    }

    public UI_UsersGridPanel GetUIUserGridPanel()
    {
        return GetGridPanel((int)GridPanels.UsersGridPanel).gameObject.GetComponent<UI_UsersGridPanel>();
    }


}
