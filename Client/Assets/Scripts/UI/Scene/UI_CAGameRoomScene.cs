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
        MapSelectBtn
    }

    enum Images
    {
        SelectedMap
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
        Bind<Image>(typeof(Images));

        GetButton((int)Buttons.ToLobby).gameObject.BindEvent(OnToLobbyButtonClicked);
        GetButton((int)Buttons.StartBtn).gameObject.BindEvent(OnStartBtnClicked);
        GetButton((int)Buttons.Bazzi).gameObject.BindEvent(OnCharacterSelect);
        GetButton((int)Buttons.Kefi).gameObject.BindEvent(OnCharacterSelect);
        GetButton((int)Buttons.Dao).gameObject.BindEvent(OnCharacterSelect);
        GetButton((int)Buttons.Marid).gameObject.BindEvent(OnCharacterSelect);
        GetButton((int)Buttons.MapSelectBtn).gameObject.BindEvent(OnMapSelectBtn);
    }

    public void OnMapSelectBtn(PointerEventData evt)
    {
        //Host�϶��� Popup�� ������ �Ѵ�. 
        if (Managers.Room.host)
            Managers.UI.ShowPopupUI<UI_CAMapSelect>();
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
            // ������ �������ڴ� ��Ŷ�� Server�� ������.
            C_StartGame startgamepacket = new C_StartGame();
            Managers.Network.Send(startgamepacket);
        }

        else
        {
            //���� ���·� �����϶�� ��Ŷ�� Server�� ������. 
            C_ReadybtnClicked readybtnClickedPacket = new C_ReadybtnClicked();
            Managers.Network.Send(readybtnClickedPacket);
        }
    }

    public void OnCharacterSelect(PointerEventData evt)
    {
        GameObject selectedBtn = evt.pointerClick;
        string buttonName = selectedBtn.name;

       // Debug.Log($"Selected BtnName : {buttonName}");

       // 2���� �����ϴ� ��쿡�� MaridȤ�� Dao�� �� ���� ����. 
       if (Managers.Room.SelectedMapTeamType == MapTeamType.TwoTeam)
        {
            if (buttonName == "Marid" || buttonName == "Dao")
                return;
        }



       if (Enum.TryParse(buttonName, out CharacterType characterType))
       {
            Debug.Log($"Selected Char Type : {characterType}");
            // �ش� ĳ���͸� ����ٰ� �������� ������ �ؾ��Ѵ�. 
            // ���������� �ش� ĳ���͸� �� �� ������ ������ Character���¸� �ٲ��ְ� Ŭ�����״� Success�� ������.
            // Success�� ���� Ŭ��� �ڽ��� ������ Character�� Checkǥ�ø� �޾��ش�.
            // ���� ��峪 AI��忡���� UnSuccess�� ������. 

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
        // �ʱ�ȭ 
        GetButton((int)Buttons.Dao).transform.Find("Check").gameObject.SetActive(false);
        GetButton((int)Buttons.Marid).transform.Find("Check").gameObject.SetActive(false);
        GetButton((int)Buttons.Kefi).transform.Find("Check").gameObject.SetActive(false);
        GetButton((int)Buttons.Bazzi).transform.Find("Check").gameObject.SetActive(false);

        GetButton((int)((Buttons)Enum.Parse(typeof(Buttons), charType.ToString()))).transform.Find("Check").gameObject.SetActive(true);
    }

    public void SelectMap(MapType mapType, MapTeamType mapTeamType)
    {
        Sprite mapImage = Managers.Data.MapPreviewImageDict[mapType];
        GetImage((int)Images.SelectedMap).sprite = mapImage;


        Color color = new Color(1, 1, 1, mapTeamType == MapTeamType.TwoTeam ? 1 : 0);
        GetButton((int)Buttons.Dao).image.color = color;
        GetButton((int)Buttons.Marid).image.color = color;

    }


}
