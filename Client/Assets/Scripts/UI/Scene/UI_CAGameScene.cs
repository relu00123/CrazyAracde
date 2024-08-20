using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_CAGameScene : UI_Scene
{
    enum RawImages
    {
        GameImage
    }

    public override void Init()
    {
        base.Init();

        Bind<RawImage>(typeof(RawImages));
    }
}
