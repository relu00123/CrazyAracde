using Server.Game;
using Server.Game.CA_Object;
using System;
using System.Collections.Generic;
using System.Text;
using static Server.Game.ObjectsManager;

public class InGameObjectFactory<T> : IInGameObjectFactory<T> where T : InGameObject
{
    public T Create(int id, string name, Transform transform, int layer)
    {
        return (T)Activator.CreateInstance(typeof(T), id, name, transform, layer);
    }
}