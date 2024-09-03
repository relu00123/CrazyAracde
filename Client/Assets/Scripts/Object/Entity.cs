using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Entity
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public GameObject UnityObject { get; private set; }

    public Entity(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public virtual void AttachUnityObject(GameObject obj)
    {
        UnityObject = obj;
        UnityObject.name = $"{Name} (Id: {Id})";

         

    }

    public T GetComponentFromUnityObject<T>() where T : Component
    {
        return UnityObject != null ? UnityObject.GetComponent<T>() : null;
    }

}
