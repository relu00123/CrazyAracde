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
    private CACharacter[] characters { get; set; } = new CACharacter[8];
    private UI_UsersGridPanel userGridPanel;
    private CAGameRoomScene curGameRoomScene;

    public int rows = 2;
    public int columns = 4;

    private S_JoinRoom currentJoinRoomPakcet;

    public void SetCurrentGameRoomScene(CAGameRoomScene gameRoomScene)
    {
        curGameRoomScene = gameRoomScene;
    }
 
    public void HandleJoinRoom(S_JoinRoom joinRoomPacket)
    {

        currentJoinRoomPakcet = joinRoomPacket;

        foreach ( var slotInfo in joinRoomPacket.SlotInfos)
        {
            Debug.Log("===================");
            Debug.Log(slotInfo.SlotIndex);
            Debug.Log(slotInfo.IsAvailable);
            Debug.Log(slotInfo.PlayerId);
            Debug.Log(slotInfo.Character);
            Debug.Log("===================");
        }

        switch (joinRoomPacket.Joinresult)
        {
            case JoinResultType.Success:

                SceneManager.sceneLoaded += OnSceneLoaded;
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

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        characterPrefab = Resources.Load<GameObject>("Prefabs/Creature/CACharacter");
       
        Debug.Log("Hello");
        SceneManager.sceneLoaded -= OnSceneLoaded;

        foreach (var slotInfo in currentJoinRoomPakcet.SlotInfos)
        {
            if (slotInfo.PlayerId != -1)
            {
              characters[slotInfo.SlotIndex] = InstantiateCharacter(slotInfo);
            }
        }
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


 