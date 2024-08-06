using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
 

public class UI_GameRoomCreatePopup : UI_Popup
{
    [SerializeField] private Sprite CheckedImage;

    enum Buttons
    {
        CreateBtn,
        CancelBtn,
        MannerModeBtn,
        FreeModeBtn,
        RandomModeBtn,
        SecretRoomBtn,

        // ModeUncheckedBtn
        NormalModeUncheckedBtn,
        MonsterModeUncheckedBtn,
        AIModeUncheckedBtn,

        // ModeCheckedBtn
        NormalMode,
        MonsterMode,
        AIMode,
    }

    enum GameObjects
    {
        RoomName,
        Password,
    }

    Dictionary<TeamModeType, Buttons> TeamModeAssistDic = new Dictionary<TeamModeType, Buttons>
    {
        { TeamModeType.MannerMode, Buttons.MannerModeBtn },
        { TeamModeType.FreeMode, Buttons.FreeModeBtn },
        { TeamModeType.RandomMode, Buttons.RandomModeBtn }
    };

    Dictionary<Buttons, (List<Buttons> activateButtons, List<Buttons> deactivateButtons)> GameModeButtons = new Dictionary<Buttons, (List<Buttons>, List<Buttons>)>
    {
        { Buttons.NormalMode,  (new List<Buttons> {Buttons.NormalMode, Buttons.MonsterModeUncheckedBtn , Buttons.AIModeUncheckedBtn} ,
                                new List<Buttons> {Buttons.NormalModeUncheckedBtn, Buttons.MonsterMode, Buttons.AIMode})},

        { Buttons.MonsterMode,  (new List<Buttons> {Buttons.NormalModeUncheckedBtn, Buttons.MonsterMode, Buttons.AIModeUncheckedBtn} ,
                            new List<Buttons> {Buttons.NormalMode, Buttons.MonsterModeUncheckedBtn, Buttons.AIMode})},

        { Buttons.AIMode,  (new List<Buttons> {Buttons.NormalModeUncheckedBtn, Buttons.MonsterModeUncheckedBtn , Buttons.AIMode } ,
                            new List<Buttons> {Buttons.NormalMode, Buttons.MonsterMode, Buttons.AIModeUncheckedBtn})},
    };

    // ���߿� CreateGameRoom Info ��Ŷ�� �������� ��Ƽ� ��������. 
    TeamModeType SelectedTeamMode = TeamModeType.MannerMode;
    GameModeType SelectedGameMode = GameModeType.NormalMode;
    bool         IsPasswordUsed = false;

    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));

        GetButton((int)Buttons.CreateBtn).gameObject.BindEvent(OnCreateBtnClicked);
        GetButton((int)Buttons.CancelBtn).gameObject.BindEvent(OnCancelBtnClicked);
        GetButton((int)Buttons.MannerModeBtn).gameObject.BindEvent(OnTeamModeSelectBtnClicked);
        GetButton((int)Buttons.FreeModeBtn).gameObject.BindEvent(OnTeamModeSelectBtnClicked);
        GetButton((int)Buttons.RandomModeBtn).gameObject.BindEvent(OnTeamModeSelectBtnClicked);
        GetButton((int)Buttons.SecretRoomBtn).gameObject.BindEvent(OnSecretRoomBtnClicked);

        // ModeUnchecked
        GetButton((int)Buttons.NormalModeUncheckedBtn).gameObject.BindEvent(OnGameModeSelectBtnClicked);
        GetButton((int)Buttons.MonsterModeUncheckedBtn).gameObject.BindEvent(OnGameModeSelectBtnClicked);
        GetButton((int)Buttons.AIModeUncheckedBtn).gameObject.BindEvent(OnGameModeSelectBtnClicked);

        // ModeChecked
        GetButton((int)Buttons.NormalMode).gameObject.BindEvent(OnGameModeSelectBtnClicked);
        GetButton((int)Buttons.MonsterMode).gameObject.BindEvent(OnGameModeSelectBtnClicked);
        GetButton((int)Buttons.AIMode).gameObject.BindEvent(OnGameModeSelectBtnClicked);


        GetObject((int)GameObjects.Password).gameObject.GetComponent<TMP_InputField>().interactable = false;
        string DefaultGameMode = "NormalMode";
        GameModeSelect(DefaultGameMode);
        CheckUIApply(GetButton((int)Buttons.MannerModeBtn).gameObject);
    }

    public void OnGameModeSelectBtnClicked(PointerEventData evt)
    {
        string ClickedBtnName = evt.pointerPress.name.Replace("UncheckedBtn", "");
        GameModeSelect(ClickedBtnName);
    }

    public void GameModeSelect(string ClickedBtnName)
    {
        if (Enum.TryParse(ClickedBtnName, out Buttons btnName))
        {
            if (GameModeButtons.TryGetValue(btnName, out var buttonLists))
            {
                if (Enum.TryParse(btnName.ToString(), out GameModeType gamemode))
                {
                    //Debug.Log($"GameMode Select called successfully {gamemode.ToString()}");
                    SelectedGameMode = gamemode;
                }

                // ��Ȱ��ȭ�� ��ư�� ó��
                foreach (Buttons button in buttonLists.deactivateButtons)
                {
                    GetButton((int)button).gameObject.SetActive(false);
                }

                // Ȱ��ȭ�� ��ư�� ó��
                foreach (Buttons button in buttonLists.activateButtons)
                {
                    GetButton((int)button).gameObject.SetActive(true);
                }

                UpdateTeamMode();
            }
        }
    }

    public void OnTeamModeSelectBtnClicked(PointerEventData evt)
    {
        Debug.Log("OnTeamModeSelectBtnClicked!");
        TeamModeSelect(evt.pointerPress.gameObject);
        UpdateTeamMode();
    }

    private void TeamModeSelect(GameObject selectedObj)
    {
        // ������ ���õǾ� �ִ��� �ʱ�ȭ (Image�� ������ �����ϰ� �ٲ۴�
        if (TeamModeAssistDic.TryGetValue(SelectedTeamMode, out Buttons MappingButton))
        {
            CheckUIApply(GetButton((int)MappingButton).gameObject, false);
        }

        // Ŭ���� ��ư�� �̸��� ������� TeamModeType ��ȯ
        string clickedButtonName = selectedObj.name.Replace("Btn", "").ToUpper();
        if (Enum.TryParse(clickedButtonName, true, out TeamModeType teamMode))
        {
            SelectedTeamMode = teamMode;
            CheckUIApply(selectedObj, true);
        }
    }

    public void OnSecretRoomBtnClicked(PointerEventData evt)
    {
        Debug.Log("SecretRoom Button Clicked!");

        // ��й�ȣ �������� ����
        if (IsPasswordUsed)
        {
            // ��й�ȣ�� ���̻� ������� �ʱ� ������ üũ ǥ�ø� ������Ѵ�. 
            CheckUIApply(GetButton((int)Buttons.SecretRoomBtn).gameObject, false);


            // Password  ������ �ϴû����� ������ �Ѵ�.
            // Input Field�� ���̻� Ŭ���� �� ������ ������ �Ѵ�. 
            GetObject((int)GameObjects.Password).gameObject.GetComponent<TMP_InputField>().interactable = false;
        }

        // ��й�ȣ �������� ����
        else
        {
            // ��й�ȣ�� ����Ѵٴ� üũ ǥ�ø� �޾���� �Ѵ�. 
            CheckUIApply(GetButton((int)Buttons.SecretRoomBtn).gameObject, true);

            // Password Panel�� ������ �Ͼ������ ������ �Ѵ�.
            // Input Field�� Ŭ���� �� �ֵ��� ������ �Ѵ�. 
            GetObject((int)GameObjects.Password).gameObject.GetComponent<TMP_InputField>().interactable = true;
        }

        IsPasswordUsed = !IsPasswordUsed;
        GetObject((int)GameObjects.Password).gameObject.GetComponent<TMP_InputField>().text = "";
    }

    public void OnCreateBtnClicked(PointerEventData evt)
    {
        // ���⼭ Game Room�� Info�� ������ GameRoom�� Create �ش޶�� ������ ��û�� �ؾ��Ѵ�. 
        Debug.Log("OnClickBtnClicked!");
        //Debug.Log("=========== Room Info ============");

        //string RoomName = GetObject((int)GameObjects.RoomName).gameObject.GetComponent<TMP_InputField>().text;
        //Debug.Log($"Room Name : {RoomName}");
        //Debug.Log($"Game Mode : {SelectedGameMode.ToString()}");
        //Debug.Log($"Team Mode : {SelectedTeamMode.ToString()}");
        //Debug.Log($"Pwd  Use  : {IsPasswordUsed}");
        //if (IsPasswordUsed)
        //Debug.Log($"Password  : {GetObject((int)GameObjects.Password).gameObject.GetComponent<TMP_InputField>().text}");
        //Debug.Log("==========  Room Info ============");

        RoomInfo roominfo = new RoomInfo();
        roominfo.RoomNumber = -1; // �̺κ��� Client�� �����ϴ� ���� �ƴ� �������� �����ϴ� ���� 
        roominfo.RoomName = GetObject((int)GameObjects.RoomName).gameObject.GetComponent<TMP_InputField>().text;
        roominfo.MapImagePath = ""; // ���߿� RandomMap���� ������ ����. DataManager���� �۾��ؾ��ҵ� 
        roominfo.CurPeopleCnt = 0;
        roominfo.MaxPeopleCnt = -1; // �̰Ŵ� Create�� ��帶�� Maximum People�μ� Server���� ���� ����
        roominfo.RoomState = RoomStateType.Waiting;
        roominfo.TeamMode = SelectedTeamMode;
        roominfo.GameMode = SelectedGameMode;
        roominfo.IsSecretRoom = IsPasswordUsed;
 
        C_CreateRoom createRoomPacket = new C_CreateRoom();
        createRoomPacket.Roominfo = roominfo;
        
        

        CloseCreatePopup(evt);


        // Scene ��ȯ GameRoom ���� �ϴ� Server�� ����� ���� �ʰ� Scene Change�� �غ���. 
        // �̰� ���������� �ȴٸ� �������� Room�� ����� ���Ŀ� Client�� Room�� ������ �� �ִ� ������� �ٲپ�� �� 
        //Managers.Scene.LoadScene(Define.Scene.CAGameRoom);

      
        // �������� ���� ����� �޶�� ��û
        Managers.Network.Send(createRoomPacket);

    }


    public void OnCancelBtnClicked(PointerEventData evt)
    {
        Debug.Log("OnCancelBtnClicked!");

        // Server���� �����͸� ��������. 

        CloseCreatePopup(evt);

        // Ŭ���� ��ư ��ü ��������
        //GameObject clickedButton = evt.pointerPress;
        //if (clickedButton != null)
        //{
        //    Debug.Log($"Clicked Button: {clickedButton.name}");

        //    // �θ� ��ü ��������
        //    Transform parentTransform = clickedButton.transform.parent;

        //    // �θ��� �θ� ��ü ��������
        //    Transform grandParentTransform = parentTransform.parent;

        //    grandParentTransform.gameObject.SetActive(false);

        //}
        //Managers.UI.ClosePopupUI();
    }

    private void CloseCreatePopup(PointerEventData evt)
    {
        GameObject clickedButton = evt.pointerPress;
        if (clickedButton != null)
        {
            //Debug.Log($"Clicked Button: {clickedButton.name}");

            // �θ� ��ü ��������
            Transform parentTransform = clickedButton.transform.parent;

            // �θ��� �θ� ��ü ��������
            Transform grandParentTransform = parentTransform.parent;

            grandParentTransform.gameObject.SetActive(false);

        }
        Managers.UI.ClosePopupUI();
    }

    private void CheckUIApply(GameObject targetObject,  bool isAssigning = true)
    {
        if (isAssigning == true)  // �̹����� �����ϴ� ���
        {
            targetObject.GetComponent<Image>().sprite = CheckedImage;
            targetObject.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
        }

        else // �̹����� ���� ���
        {
            targetObject.GetComponent<Image>().sprite = null;
            targetObject.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
        }
    }

    private void UpdateTeamMode()
    {
        switch (SelectedGameMode)
        {
            case GameModeType.NormalMode:
            {
                    break;
            }

            case GameModeType.MonsterMode:
            {
               TeamModeSelect(GetButton((int)Buttons.MannerModeBtn).gameObject);
                break;
            }

            case GameModeType.AIMode:
            {
                if (SelectedTeamMode == TeamModeType.RandomMode)
                    TeamModeSelect(GetButton((int)Buttons.MannerModeBtn).gameObject);
                break;
            }
        }
             
    }
}
