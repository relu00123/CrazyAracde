using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;



public class CAPlayerRender : MonoBehaviour
{
    public void CAPlayerRenderTest()
    {

    }

    public void ToIdleAnim()
    {
        GameObject parentObject = transform.parent.gameObject;
        if (parentObject == null)
        {
            Debug.Log("Parent Object is Null!");
            return;
        }

        var basecontroller =  parentObject.GetComponent<CABaseController>(); 
        if (basecontroller == null)
        {
            Debug.Log("Base Controller is Null");
            return;
        }

        basecontroller.Dir = MoveDir.Down;
        basecontroller.ChangeAnimation(PlayerAnimState.PlayerAnimIdle);


    }
}
