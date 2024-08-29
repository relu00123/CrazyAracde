using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameObject : Entity
{
     public InGameObject(int id, string name) : base(id, name)
    {
 
    }


    public Vector3? CurrentPos
    {
        get
        {
            if (UnityObject != null && GetComponentFromUnityObject<Transform>() != null)
                return GetComponentFromUnityObject<Transform>().position;
            else
                return null;
        }

        set
        {
            if (value == null) return;

            if (UnityObject != null && GetComponentFromUnityObject<Transform>() != null)
                GetComponentFromUnityObject<Transform>().position = (Vector3)value;
        }
    }
}
