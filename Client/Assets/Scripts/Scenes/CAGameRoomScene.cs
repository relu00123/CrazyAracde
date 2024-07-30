using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CAGameRoomScene : BaseScene
{
    public UI_CAGameRoomScene _sceneUI { get; set; }

    protected override void Init()
    {
        base.Init();

        SceneType =  Define.Scene.CAGameRoom;

        _sceneUI = Managers.UI.ShowSceneUI<UI_CAGameRoomScene>();
    }

    public override void Clear()
    {
    }
}
