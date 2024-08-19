using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public class CAGameScene : BaseScene
{
    [SerializeField] InGameCamera InGameCameraPrefab;

    public UI_CAGameScene _sceneUI { get; set; }
   
    public InGameCamera InGameCamera { get; private set; }

    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.CAGame;

        //InGameCamera = Instantiate(InGameCameraPrefab);
        //InGameCamera.gameObject.SetActive(true);

        _sceneUI = Managers.UI.ShowSceneUI<UI_CAGameScene>();

        Managers.CaMap.LoadMap("Pirate_test", "Pirate_test");

    }

    public override void Clear()
    {

    }
}
