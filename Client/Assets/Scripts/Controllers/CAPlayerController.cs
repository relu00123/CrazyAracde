using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class CAPlayerController : MonoBehaviour
{
    public InGameObject Player { get; set; }


   // bool _moveKeyPressed = false;

    //CACreatureState characterstate;

    public virtual void Test()
    {
        Debug.Log("TestFunction Called from PlayerController");
    }

    //void Update()
    //{
    //    UpdateController();
    //}

    //public void UpdateController()
    //{
    //    switch (characterstate)
    //    {
    //        case CACreatureState.CaIdle:
    //            break;
    //        case CACreatureState.CaMoving:
    //            break;
    //        case CACreatureState.CaDead:
    //            break;
    //    }
    //}

    //void GetDirInput()
    //{
    //    _moveKeyPressed = true;


    //}
}
 
