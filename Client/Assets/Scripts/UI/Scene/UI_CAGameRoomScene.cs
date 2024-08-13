using Google.Protobuf.Protocol;
using System;
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
        Bazzi,
        Kefi,
        Dao,
        Marid,
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
        GetButton((int)Buttons.Bazzi).gameObject.BindEvent(OnCharacterSelect);
        GetButton((int)Buttons.Kefi).gameObject.BindEvent(OnCharacterSelect);
        GetButton((int)Buttons.Dao).gameObject.BindEvent(OnCharacterSelect);
        GetButton((int)Buttons.Marid).gameObject.BindEvent(OnCharacterSelect);
       
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

    public void OnCharacterSelect(PointerEventData evt)
    {
        GameObject selectedBtn = evt.pointerClick;
        string buttonName = selectedBtn.name;

       // Debug.Log($"Selected BtnName : {buttonName}");

       if (Enum.TryParse(buttonName, out CharacterType characterType))
       {
            Debug.Log($"Selected Char Type : {characterType}");
            // 해당 캐릭터를 골랐다고 서버에게 전달을 해야한다. 
            // 서버에서는 해당 캐릭터를 고를 수 있으면 서버의 Character상태를 바꿔주고 클라한테는 Success를 보낸다.
            // Success를 받은 클라는 자신이 선택한 Character에 Check표시를 달아준다.
            // 몬스터 모드나 AI모드에서는 UnSuccess를 보낸다. 

            C_CharacterSelect characterSelect = new C_CharacterSelect();
            characterSelect.Chartype = characterType;

            Managers.Network.Send(characterSelect);
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


    public void CharacterSelect(CharacterType charType)
    {
        // 초기화 
        GetButton((int)Buttons.Dao).transform.Find("Check").gameObject.SetActive(false);
        GetButton((int)Buttons.Marid).transform.Find("Check").gameObject.SetActive(false);
        GetButton((int)Buttons.Kefi).transform.Find("Check").gameObject.SetActive(false);
        GetButton((int)Buttons.Bazzi).transform.Find("Check").gameObject.SetActive(false);

        GetButton((int)((Buttons)Enum.Parse(typeof(Buttons), charType.ToString()))).transform.Find("Check").gameObject.SetActive(true);
    }


}
