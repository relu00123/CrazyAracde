using Google.Protobuf.Protocol;
using Server.Game;
using Server.Game.CA_Object;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Numerics;
using System.Text;


public class CAPlayer  : InGameObject
{
    public CAPlayer(int id, string name, Transform transform, int layer)
        : base(id, name, transform, layer)
    {
        // Player에 필요한 Collider 생성
        _collider = new Collider(this, Vector2.Zero, new Vector2(0.95f, 0.95f));
    }

    IJob _job;

     

    public override void Update()
    {
        //Console.WriteLine($"Update Function Called! (Object ID : {Id}) (CurPosition : {_transform.Position})");

        if (_possessGame != null)
        {
            switch (_state)
            {
                case CreatureState.Idle:
                    UpdateIdle();
                    break;
                case CreatureState.Moving:
                    UpdateMoving();
                    break;
                
            }

             _job =  _possessGame._gameRoom.PushAfter(10, Update);
        }
    }

    public override void UpdateIdle()
    {

    }

    public override void UpdateMoving()
    {
        base.UpdateMoving();
    }
}

