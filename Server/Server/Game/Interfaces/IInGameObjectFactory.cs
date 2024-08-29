using Server.Game.CA_Object;
using Server.Game;
using System;
using System.Collections.Generic;
using System.Text;

public interface IInGameObjectFactory<T> where T : InGameObject
{
    T Create(int id, string name, Transform transform, int layer);
}