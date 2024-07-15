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
    }



    public UI_UserListPannel UserListUI { get; private set; }

    public UI_ChattingPannel ChattingUI { get; private set; }

    public override void Init()
    {
        base.Init();

        Bind<Button>(typeof(Buttons));

        GetButton((int)Buttons.LeftButton).gameObject.BindEvent(OnPreviousPageButtonClicked);
        GetButton((int)Buttons.RightButton).gameObject.BindEvent(OnNextPageButtonClicked);

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
}
