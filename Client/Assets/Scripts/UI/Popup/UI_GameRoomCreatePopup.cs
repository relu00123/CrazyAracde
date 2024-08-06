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

    // 나중에 CreateGameRoom Info 패킷에 차곡차곡 모아서 보내야함. 
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

                // 비활성화할 버튼들 처리
                foreach (Buttons button in buttonLists.deactivateButtons)
                {
                    GetButton((int)button).gameObject.SetActive(false);
                }

                // 활성화할 버튼들 처리
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
        // 기존에 선택되어 있던것 초기화 (Image를 없에고 투명하게 바꾼다
        if (TeamModeAssistDic.TryGetValue(SelectedTeamMode, out Buttons MappingButton))
        {
            CheckUIApply(GetButton((int)MappingButton).gameObject, false);
        }

        // 클릭한 버튼의 이름을 기반으로 TeamModeType 변환
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

        // 비밀번호 없음으로 변경
        if (IsPasswordUsed)
        {
            // 비밀번호를 더이상 사용하지 않기 때문에 체크 표시를 빼줘야한다. 
            CheckUIApply(GetButton((int)Buttons.SecretRoomBtn).gameObject, false);


            // Password  색깔을 하늘색으로 만들어야 한다.
            // Input Field를 더이상 클릭할 수 없도록 만들어야 한다. 
            GetObject((int)GameObjects.Password).gameObject.GetComponent<TMP_InputField>().interactable = false;
        }

        // 비밀번호 있음으로 변경
        else
        {
            // 비밀번호를 사용한다는 체크 표시를 달아줘야 한다. 
            CheckUIApply(GetButton((int)Buttons.SecretRoomBtn).gameObject, true);

            // Password Panel의 색깔을 하얀색으로 만들어야 한다.
            // Input Field를 클릭할 수 있도록 만들어야 한다. 
            GetObject((int)GameObjects.Password).gameObject.GetComponent<TMP_InputField>().interactable = true;
        }

        IsPasswordUsed = !IsPasswordUsed;
        GetObject((int)GameObjects.Password).gameObject.GetComponent<TMP_InputField>().text = "";
    }

    public void OnCreateBtnClicked(PointerEventData evt)
    {
        // 여기서 Game Room의 Info를 가지고 GameRoom을 Create 해달라고 서버에 요청을 해야한다. 
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
        roominfo.RoomNumber = -1; // 이부분은 Client가 결정하는 것이 아닌 서버에서 결정하는 것임 
        roominfo.RoomName = GetObject((int)GameObjects.RoomName).gameObject.GetComponent<TMP_InputField>().text;
        roominfo.MapImagePath = ""; // 나중에 RandomMap으로 변경할 것임. DataManager에서 작업해야할듯 
        roominfo.CurPeopleCnt = 0;
        roominfo.MaxPeopleCnt = -1; // 이거는 Create시 모드마다 Maximum People로서 Server에서 강제 세팅
        roominfo.RoomState = RoomStateType.Waiting;
        roominfo.TeamMode = SelectedTeamMode;
        roominfo.GameMode = SelectedGameMode;
        roominfo.IsSecretRoom = IsPasswordUsed;
 
        C_CreateRoom createRoomPacket = new C_CreateRoom();
        createRoomPacket.Roominfo = roominfo;
        
        

        CloseCreatePopup(evt);


        // Scene 전환 GameRoom 으로 일단 Server의 허락을 받지 않고 Scene Change를 해보자. 
        // 이게 정상적으로 된다면 서버에서 Room을 만들고 이후에 Client가 Room에 참여할 수 있는 방식으로 바꾸어야 함 
        //Managers.Scene.LoadScene(Define.Scene.CAGameRoom);

      
        // 서버에게 방을 만들어 달라고 요청
        Managers.Network.Send(createRoomPacket);

    }


    public void OnCancelBtnClicked(PointerEventData evt)
    {
        Debug.Log("OnCancelBtnClicked!");

        // Server에다 데이터를 보내보자. 

        CloseCreatePopup(evt);

        // 클릭한 버튼 객체 가져오기
        //GameObject clickedButton = evt.pointerPress;
        //if (clickedButton != null)
        //{
        //    Debug.Log($"Clicked Button: {clickedButton.name}");

        //    // 부모 객체 가져오기
        //    Transform parentTransform = clickedButton.transform.parent;

        //    // 부모의 부모 객체 가져오기
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

            // 부모 객체 가져오기
            Transform parentTransform = clickedButton.transform.parent;

            // 부모의 부모 객체 가져오기
            Transform grandParentTransform = parentTransform.parent;

            grandParentTransform.gameObject.SetActive(false);

        }
        Managers.UI.ClosePopupUI();
    }

    private void CheckUIApply(GameObject targetObject,  bool isAssigning = true)
    {
        if (isAssigning == true)  // 이미지를 대입하는 경우
        {
            targetObject.GetComponent<Image>().sprite = CheckedImage;
            targetObject.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
        }

        else // 이미지를 빼는 경우
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
