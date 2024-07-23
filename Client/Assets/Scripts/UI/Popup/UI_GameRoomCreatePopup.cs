using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_GameRoomCreatePopup : UI_Popup
{
     

    enum Buttons
    {
        CreateBtn,
        CancelBtn
    }


    public override void Init()
    {
        base.Init();

        Bind<Button>(typeof(Buttons));

        GetButton((int)Buttons.CreateBtn).gameObject.BindEvent(OnCreateBtnClicked);
        GetButton((int)Buttons.CancelBtn).gameObject.BindEvent(OnCancelBtnClicked);


        //         Bind<Button>(typeof(Buttons));

        //GetButton((int)Buttons.LeftButton).gameObject.BindEvent(OnPreviousPageButtonClicked);
        //GetButton((int)Buttons.RightButton).gameObject.BindEvent(OnNextPageButtonClicked);
        //GetButton((int)Buttons.ToStoreButton).gameObject.BindEvent(OnToStoreButtonClicked);
    }

    public void OnCreateBtnClicked(PointerEventData evt)
    {
        Debug.Log("OnClickBtnClicked!");
    }

    public void OnCancelBtnClicked(PointerEventData evt)
    {
        Debug.Log("OnCancelBtnClicked!");
        Managers.UI.ClosePopupUI();
    }




}
