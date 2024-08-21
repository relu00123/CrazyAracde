using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameManager : MonoBehaviour
{
    private CAGameScene curGameScene;
    private S_StartGameBroadcast currentStartGamePacket;

    public void HandleStartGameBroadcast(S_StartGameBroadcast pkt)
    {
        currentStartGamePacket = pkt;

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
        Managers.Scene.LoadScene(Define.Scene.CAGameScene);
        
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Managers.CaMap.LoadMap(currentStartGamePacket.Maptype);

        // 서버에게 Scene이 Load되었으니 Instantiate할 Object들을 달라고 해야함 
        Debug.Log("On Scene Load Function Called! Please Load Map");
    }

    public void OnSceneUnloaded(Scene scene)
    {
    }

    public void SetCurrentGameRoomScene(CAGameScene gameRoomScene)
    {
        curGameScene = gameRoomScene;
    }
}