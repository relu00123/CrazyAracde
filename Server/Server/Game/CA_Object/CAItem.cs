using Server.Game.CA_Object;
using Server.Game;
using System;
using System.Collections.Generic;
using System.Text;
using Google.Protobuf.Protocol;



public class CAItem : InGameObject
{
    public CAItemType itemType { get; set; }
    public Vector2Int position { get; set; } = new Vector2Int();
    public CAItem(int id, string name, Transform transform, int layer)
       : base(id, name, transform, layer)
    {
        // Collider 부분은 나중에 작업 
        //_collider = new Collider(this, Vector2.Zero, new Vector2(0.95f, 0.95f));
        //lastUpdateTime = DateTime.Now;
    }
}