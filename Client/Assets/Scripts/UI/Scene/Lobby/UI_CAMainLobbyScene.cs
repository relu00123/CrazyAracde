using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_CAMainLobbyScene : UI_Scene
{
    enum Buttons
    {
        LeftButton,
        RightButton,
        ToStoreButton,
        CreateRoomButton,
    }

    enum Panels
    {
        GameRoomCreatePanel,
    }



    public UI_UserListPannel UserListUI { get; private set; }

    public UI_ChattingPannel ChattingUI { get; private set; }

    public UI_GameRoomGridPanel GameRoomGridPanel { get; private set; }

    public override void Init()
    {
        base.Init();

        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(Panels));

        GetButton((int)Buttons.LeftButton).gameObject.BindEvent(OnPreviousPageButtonClicked);
        GetButton((int)Buttons.RightButton).gameObject.BindEvent(OnNextPageButtonClicked);
        GetButton((int)Buttons.ToStoreButton).gameObject.BindEvent(OnToStoreButtonClicked);
        GetButton((int)Buttons.CreateRoomButton).gameObject.BindEvent(OnCreateRoomButtonClicked);
        GetObject((int)Panels.GameRoomCreatePanel).SetActive(false);

        Transform userListPanelTransform = transform.Find("MainLobbyPannel/UserListPannel");
        Transform chattingPannelTransform = transform.Find("MainLobbyPannel/ChattingPannel");
        Transform gameRoomGridPanelTransform = transform.Find("MainLobbyPannel/GameRoomGridPanel");

        UserListUI = userListPanelTransform.GetComponent<UI_UserListPannel>();
        ChattingUI = chattingPannelTransform.GetComponent<UI_ChattingPannel>();
        GameRoomGridPanel = gameRoomGridPanelTransform.GetComponent<UI_GameRoomGridPanel>();

        // GameRoom GridPanel에 RoomItem 8개를 추가해야한다.  Grid Panel 이 Init할때 자동으로 해주도록 했음. 
    }

    // 이게 Lobby Scene에서 관리해야함 
    public void OnNextPageButtonClicked(PointerEventData evt)
    {
        Managers.UserList.NextPage();
    }

    public void OnPreviousPageButtonClicked(PointerEventData evt)
    {
        Managers.UserList.PreviousPage();
    }

    public void OnToStoreButtonClicked(PointerEventData evt)
    {
        // Store Scene으로 이동.
        Debug.Log("To Store!");
        Managers.Scene.LoadScene(Define.Scene.CAStore);

        C_EnterStore EnterStorePacket = new C_EnterStore();
        EnterStorePacket.Player = Managers.UserInfo.myLobbyPlayerInfo;

        Managers.Network.Send(EnterStorePacket);
    }

    public void OnCreateRoomButtonClicked(PointerEventData evt)
    {
        Debug.Log("OnCreateRoomButtonClicked!!");

        // CreateRoom Popup UI를 보여줘야 한다.
        // 이런 식으로 사용하는 것 맞는지 확인하기.
        GetObject((int)Panels.GameRoomCreatePanel).SetActive(true);
        UI_GameRoomCreatePopup Popup = Managers.UI.ShowPopupUI<UI_GameRoomCreatePopup>(GetObject((int)Panels.GameRoomCreatePanel));
    }
}
