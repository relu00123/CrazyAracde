using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameObject : Entity
{
     public InGameObject(int id, string name) : base(id, name)
    {
 
    }

    public override void AttachUnityObject(GameObject obj)
    {
        base.AttachUnityObject(obj);

        CABaseController controller = UnityObject.GetComponent<CABaseController>();

        if (controller != null)
            controller.InGameObj = this;
    }


    public virtual void OnDestroyObject()
    {
        Managers.InGame._objectLayerManager.RemoveObjectFromLayer(this);
    }


    //public Vector3? CurrentPos
    //{
    //    get
    //    {
    //        if (UnityObject != null && GetComponentFromUnityObject<Transform>() != null)
    //            return GetComponentFromUnityObject<Transform>().position;
    //        else
    //            return null;
    //    }

    //    set
    //    {
    //        if (value == null) return;

    //        if (UnityObject != null && GetComponentFromUnityObject<Transform>() != null)
    //            GetComponentFromUnityObject<Transform>().position = (Vector3)value;
    //    }
    //}
}
