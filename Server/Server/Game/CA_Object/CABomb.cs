using Server.Game;
using Server.Game.CA_Object;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

 public class CABomb : InGameObject
{
    public CABomb(int id, string name, Transform transform, int layer)
       : base(id, name, transform, layer)
    {
        // Bomb에 필요한 Collider 생성 ?  아직 필요한지 정확하게 파악안됨.
        // 폭탄이 터질때 Collision System을 어떻게 만들지에 따라서 갈릴듯. 
        _collider = new Collider(this, Vector2.Zero, new Vector2(0.95f, 0.95f));
    }
}