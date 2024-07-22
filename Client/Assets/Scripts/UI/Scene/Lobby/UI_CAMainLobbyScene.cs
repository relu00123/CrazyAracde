using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
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
    }

    public UI_UserListPannel UserListUI { get; private set; }

    public UI_ChattingPannel ChattingUI { get; private set; }

    public override void Init()
    {
        base.Init();

        Bind<Button>(typeof(Buttons));

        GetButton((int)Buttons.LeftButton).gameObject.BindEvent(OnPreviousPageButtonClicked);
        GetButton((int)Buttons.RightButton).gameObject.BindEvent(OnNextPageButtonClicked);
        GetButton((int)Buttons.ToStoreButton).gameObject.BindEvent(OnToStoreButtonClicked);

        Transform userListPanelTransform = transform.Find("MainLobbyPannel/UserListPannel");
        Transform chattingPannelTransform = transform.Find("MainLobbyPannel/ChattingPannel");

        UserListUI = userListPanelTransform.GetComponent<UI_UserListPannel>();
        ChattingUI = chattingPannelTransform.GetComponent<UI_ChattingPannel>();

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
}
