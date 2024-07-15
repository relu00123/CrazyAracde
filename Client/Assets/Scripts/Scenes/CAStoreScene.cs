using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CAStoreScene : BaseScene
{
    UI_CAStoreScene _sceneUI { get; set; }

    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.CAStore;

        _sceneUI = Managers.UI.ShowSceneUI<UI_CAStoreScene>();
    }

    public override void Clear()
    {
    }
}
