using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
//using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class RoomManager : MonoBehaviour
{
    private GameObject characterPrefab;
    public CACharacter[] characters { get; private set; } = new CACharacter[8];
    private UI_UsersGridPanel userGridPanel;
    private CAGameRoomScene curGameRoomScene;

    public int rows = 2;
    public int columns = 4;
    private int clientSlotidx = -1;

    public MapType SelectedMap { get; private set; }

    public MapTeamType SelectedMapTeamType { get; set; }

    public GameModeType GameMode { get; private set; }

    private bool _host = false;
    public bool host
    {
        get { return _host; }
        private set
        {
            if (_host != value)
            {
                _host = value;
                OnHostChanged(clientSlotidx);
            }
        }
    }
    
    //public bool host { get; private set; }

    private S_JoinRoom currentJoinRoomPakcet;

    public void SetCurrentGameRoomScene(CAGameRoomScene gameRoomScene)
    {
        curGameRoomScene = gameRoomScene;
    }

    public void OnHostChanged(int slotidx)
    {
        if (curGameRoomScene == null)
            return;

        curGameRoomScene._sceneUI.SetHost(host);
    }
 
    public void HandleJoinRoom(S_JoinRoom joinRoomPacket)
    {
        currentJoinRoomPakcet = joinRoomPacket;


        //foreach ( var slotInfo in joinRoomPacket.SlotInfos)
        //{
        //    Debug.Log("===================");
        //    Debug.Log(slotInfo.SlotIndex);
        //    Debug.Log(slotInfo.IsAvailable);
        //    Debug.Log(slotInfo.PlayerId);
        //    Debug.Log(slotInfo.Character);
        //    Debug.Log("===================");
        //}

        switch (joinRoomPacket.Joinresult)
        {
            case JoinResultType.Success:

                SceneManager.sceneLoaded += OnSceneLoaded;
                SceneManager.sceneUnloaded += OnSceneUnloaded;
                Managers.Scene.LoadScene(Define.Scene.CAGameRoom);

                break;

            case JoinResultType.RoomNotExist:

                break;

            case JoinResultType.GameAlreadyStarted:

                break;

            case JoinResultType.RoomFull:

                break;
        } 
    }

    #region PacketHandle

    public void HandleJoinRoomBroadcast(S_JoinRoomBroadcast joinRoomBroadcastPacket)
    {
        SlotInfo slotInfo = joinRoomBroadcastPacket.SlotInfo;

        characters[slotInfo.SlotIndex] = InstantiateCharacter(slotInfo);
    }

    public void HandleExitRoomBroadcast(S_ExitRoomBroadcast exitRoomBroadcastPacket)
    {
        int slotid = exitRoomBroadcastPacket.SlotId;

        if (0 <= slotid && slotid < characters.Length && characters[slotid] != null)
        {
            // 해당 캐릭터를 삭제한다.
            Destroy(characters[slotid].gameObject);

            // 해당 Slot을 Empty로 바꾼다. (이름, 배경 포함)
            curGameRoomScene._sceneUI.GetUIUserGridPanel().ClearSlot(slotid);
            curGameRoomScene._sceneUI.GetUIUserGridPanel().SetCharState(slotid, GameRoomCharacterStateType.NotReady);
        }
    }

    public void HandleGameroomCharState(S_GameroomCharState pkt)
    {
        curGameRoomScene._sceneUI.GetUIUserGridPanel().SetCharState(pkt.SlotId, pkt.Charstate);

    }

    public void HandleAlterHost(S_AlterHost pkt)
    {
        if (pkt.Previousidx == clientSlotidx)
            host = false;
        else if (pkt.Nowidx == clientSlotidx && clientSlotidx != -1) 
            host = true;
       

       curGameRoomScene._sceneUI.GetUIUserGridPanel().SetCharState(pkt.Previousidx, GameRoomCharacterStateType.NotReady);
       curGameRoomScene._sceneUI.GetUIUserGridPanel().SetCharState(pkt.Nowidx, GameRoomCharacterStateType.Host);

    }

    
    public void HandleChangeSlotState(S_ChangeSlotStateBroadcast pkt)
    {
        if (pkt.Isopen)
            curGameRoomScene._sceneUI.GetUIUserGridPanel().OpenSlot(pkt.Slotidx);
        else
            curGameRoomScene._sceneUI.GetUIUserGridPanel().CloseSlot(pkt.Slotidx);

    }

    public void HandleCharacterSelectResponse(S_CharacterSelectResponse pkt)
    {
        if (pkt.IsSuccess == true)
        {
            curGameRoomScene._sceneUI.CharacterSelect(pkt.Chartype);
        }

        else
        {
            // 해당모드에서는 배찌만 가능하다는 메세지를 출력해주는 팝업을 만들어준다. 
            // GetObject((int)Panels.GameRoomCreatePanel).SetActive(true);
            // UI_GameRoomCreatePopup Popup = Managers.UI.ShowPopupUI<UI_GameRoomCreatePopup>(GetObject((int)Panels.GameRoomCreatePanel));

            //.UI.ShowPopupUI<UI_Popup>()

            //UI_GameRoomCreatePop Popupup = Managers.UI.ShowPopupUI<UI_GameRoomCreatePopup>(GetObject((int)Panels.GameRoomCreatePanel));

            UI_NotificationPopup popup =  Managers.UI.ShowPopupUI<UI_NotificationPopup>();
            popup.SetMainText("Only Team A Available in this mode!");
            popup.AddDefaultCloseEventOnConfirmBtn();
        }
    }


    public void HandleCharacterSelectBroadcast(S_CharacterSelectBroadcast pkt)
    {
        // 예외처리
        if (pkt.Slotid < 0 || pkt.Slotid >= characters.Length || characters[pkt.Slotid] == null) return;

        string animationName = pkt.Chartype.ToString() + "_Idle";
        characters[pkt.Slotid].animator.Play(animationName);

    }

    public void HandleStartGameRes(S_StartGameRes pkt)
    {
        // Noti Popup하나 보여준다. 
        if (pkt.IsSuccess == false)
        {
            UI_NotificationPopup popup = Managers.UI.ShowPopupUI<UI_NotificationPopup>();
            popup.SetMainText("It's not Fair Team!");
            popup.AddDefaultCloseEventOnConfirmBtn();
        }
    }

    public void HandleStartGameBroadcast(S_StartGameBroadcast pkt)
    {
        // GameScene 전환 필요! (일단 팝업으로 해보자)
        UI_NotificationPopup popup = Managers.UI.ShowPopupUI<UI_NotificationPopup>();
        popup.SetMainText("GameStart!!!");
        popup.AddDefaultCloseEventOnConfirmBtn();


        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
        Managers.Scene.LoadScene(Define.Scene.CAGameRoom);
    }

    public void HandleMapSelectBroadcast(S_MapSelectBroadcast pkt)
    {
        // Select한 Map 바꿔주기. (관리용)
        SelectedMap = pkt.Maptype;
        SelectedMapTeamType = pkt.MapTeamType;


        // Highlight UI 바꿔주기 (UI용)
        curGameRoomScene._sceneUI.SelectMap(SelectedMap, SelectedMapTeamType);


        if (SelectedMapTeamType == MapTeamType.TwoTeam)
        {
            C_CharacterSelect characterSelect = new C_CharacterSelect();
            characterSelect.Chartype = CharacterType.Bazzi;

            Managers.Network.Send(characterSelect);
        }
    }


    #endregion



    public void OnSceneUnloaded(Scene scene)
    {
        SceneManager.sceneUnloaded -= OnSceneUnloaded;

        curGameRoomScene = null;
        clientSlotidx = -1;
        host = false;


        for (int i = 0; i < characters.Length; i++)
        {
            characters[i] = null;
        }
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        characterPrefab = Resources.Load<GameObject>("Prefabs/Creature/CACharacter");
       
        Debug.Log("Hello");
        SceneManager.sceneLoaded -= OnSceneLoaded;
        host = false;

        clientSlotidx = currentJoinRoomPakcet.ClientslotIdx;
        int clientcount = 0;

        // 게임 모드 설정
        GameMode = currentJoinRoomPakcet.Gamemode;

        foreach (var slotInfo in currentJoinRoomPakcet.SlotInfos)
        {
            if (slotInfo.PlayerId != -1)
            {
                characters[slotInfo.SlotIndex] = InstantiateCharacter(slotInfo);
                clientcount++;

                // 해당하는 플레이어 State에 맞는 CharState UI를 보여준다. 
                curGameRoomScene._sceneUI.GetUIUserGridPanel().SetCharState(slotInfo.SlotIndex, slotInfo.CharacterState);
            }

            else if (slotInfo.PlayerId == - 1 && slotInfo.IsAvailable == false)
            {
               Debug.Log($"SlotIndex : {slotInfo.SlotIndex}");
                curGameRoomScene._sceneUI.GetUIUserGridPanel().CloseSlot(slotInfo.SlotIndex);
            }
        }

        // 자기 자신이 Host일 경우 host의 권한을 줘야 한다.  
        if (clientcount == 1)
        {
            host = true;
            curGameRoomScene._sceneUI.GetUIUserGridPanel().SetCharState(0, GameRoomCharacterStateType.Host);
        }

        // 테스트중
        if (currentJoinRoomPakcet.HostIdx == clientSlotidx)
        {
            host = true;
            curGameRoomScene._sceneUI.GetUIUserGridPanel().SetCharState(clientSlotidx, GameRoomCharacterStateType.Host);
        }



        // 선택된 맵을 보여주도록 한다.
        curGameRoomScene._sceneUI.SelectMap(currentJoinRoomPakcet.Maptype, MapTeamType.FourTeam);


         

    }

    public CACharacter InstantiateCharacter(SlotInfo slotinfo) // 나중에는 PlayerInfo로 바꿔줘야 한다.
    {
        GameRoomCamera camera =  Managers.Scene.CurrentScene.gameObject.GetComponent<CAGameRoomScene>().gameRoomCamera;
        Camera mainCamera = camera.GetComponent<Camera>();

        float width = 1.0f / columns;
        float height = 1.0f / rows;


        int char_row = 1 - (slotinfo.SlotIndex / columns);
        int char_col = slotinfo.SlotIndex % columns;


        // Viewport 좌표를 World 좌표로 변환
        Vector3 viewportPos = new Vector3((char_col + 0.5f) * width, char_row * height, mainCamera.nearClipPlane);
        Vector3 worldPos = mainCamera.ViewportToWorldPoint(viewportPos);

        // 캐릭터를 해당 World 좌표에 Instantiate
        GameObject go = Instantiate(characterPrefab, worldPos, Quaternion.identity);
        CACharacter caCharacter = go.GetComponent<CACharacter>();

        if (caCharacter != null)
            caCharacter.characterType = slotinfo.Character;

        LobbyPlayerInfo playerinfo =  Managers.UserInfo.Find(slotinfo.PlayerId);
        curGameRoomScene._sceneUI.GetUIUserGridPanel().SetName(slotinfo.SlotIndex, playerinfo.Name);

        return caCharacter;
    }
}


 