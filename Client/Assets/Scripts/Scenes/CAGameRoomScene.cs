using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CAGameRoomScene : BaseScene
{
    [SerializeField] GameRoomCamera gameRoomCameraPrefab;
    public GameRoomCamera gameRoomCamera { get; private set; }

    public UI_CAGameRoomScene _sceneUI { get; set; }


    protected override void Init()
    {
        base.Init();

        SceneType =  Define.Scene.CAGameRoom;

        gameRoomCamera = Instantiate(gameRoomCameraPrefab);
        gameRoomCamera.gameObject.SetActive(true);

        _sceneUI = Managers.UI.ShowSceneUI<UI_CAGameRoomScene>();

        Managers.Room.SetCurrentGameRoomScene(this);
    }

    public override void Clear()
    {
    }
}
