using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

 


public class UI_NotificationPopup : UI_Popup
{
    private UI_RaycastBlock BlockPopup;

    enum Buttons
    {
        ConfirmBtn,
    }

    enum Texts
    {
        MainText,
    }


    public override void Init()
    {
        BlockPopup = Managers.UI.ShowPopupUI<UI_RaycastBlock>();

        Managers.UI.SetCanvas(gameObject, true);


        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Button>(typeof(Buttons));

    }

    public void SetMainText(string text)
    {
        

        GetTextMeshPro((int)Texts.MainText).text = text;
    }

    public void AddDefaultCloseEventOnConfirmBtn()
    {
        GetButton((int)Buttons.ConfirmBtn).gameObject.BindEvent(ClosePopup);
    }

    public void ClosePopup(PointerEventData evt)
    {
        ClosePopupUI();

        if (BlockPopup != null) 
            BlockPopup.ClosePopupUI();

    }

    

}
