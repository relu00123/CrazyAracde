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

    public ObjectLayerManager _objectLayerManager { get; private set; }

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

        _objectLayerManager = new ObjectLayerManager();

        // 서버에게 Scene이 Load되었으니 Instantiate할 Object들을 달라고 해야함 
        Debug.Log("On Scene Load Function Called! Please Load Map");

        C_GameSceneLoadFinished LoadFinishedPkt = new C_GameSceneLoadFinished();
        Managers.Network.Send(LoadFinishedPkt);

        // 이부분에서 GameStart SoundEffect를 Play해도 상관없을 것 같다. 확인해보기 (10.08)
        GameObject prefab = Resources.Load<GameObject>($"Prefabs/Effect/SoundEffect");
        prefab.GetComponent<SoundEffect>().SetAudioClip(SoundEffectType.GameStartSoundEffect);
        GameObject.Instantiate(prefab);

        // Game BGM 틀기! 
        Managers.Scene.CurrentScene.SetMapBGM(currentStartGamePacket.Maptype);

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void HandleSpawnObject(S_SpawnObject spawnObjectPacket)
    {
        _objectLayerManager.HandleSpawnObject(spawnObjectPacket);
    }

    public void HandleDestroyObject(S_DestroyObject destroyObjectPacket)
    {
        _objectLayerManager.HandleDestroyObject(destroyObjectPacket);
    }

    public void OnSceneUnloaded(Scene scene)
    {
        //SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void SetCurrentGameRoomScene(CAGameScene gameRoomScene)
    {
        curGameScene = gameRoomScene;
    }

}