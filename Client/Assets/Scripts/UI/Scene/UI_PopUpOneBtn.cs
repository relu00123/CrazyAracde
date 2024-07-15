using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

 
public class UI_PopUpOneBtn : UI_Base
{
    enum Images
    {
        Window
    }

    enum Texts
    {
        Message
    }

    enum Buttons
    {
        CancelBtn
    }



    public override void Init()
    {
        //base.Init();

        Bind<Image>(typeof(Images));
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Button>(typeof(Buttons));

        //GetButton((int)Buttons.CancelBtn).gameObject.BindEvent(OnClickCancelButton);
    }

    //public void OnClickCancelButton(PointerEventData evt)
    //{
    //    Debug.Log("Cancel Button ´©¸§");
    //    this.gameObject.SetActive(false);
    //}

    public Button GetCancelButton() { return GetButton((int)Buttons.CancelBtn); }
    public TextMeshProUGUI GetMessageText() { return GetTextMeshPro((int)Texts.Message); }
    public void SetMessageText(string _text) { GetTextMeshPro((int)Texts.Message).text = _text; }

}

