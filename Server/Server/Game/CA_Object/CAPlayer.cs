using Google.Protobuf.Protocol;
using Server.Game;
using Server.Game.CA_Object;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;


public class CAPlayer  : InGameObject
{
    public CAPlayer(int id, string name, Transform transform, int layer)
        : base(id, name, transform, layer)
    {

    }

    IJob _job;

     

    public override void Update()
    {
        Console.WriteLine($"Update Function Called! (Object ID : {Id}) (CurPosition : {_transform.Position})");

        if (_inGame != null)
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

             _job =  _inGame._gameRoom.PushAfter(500, Update);
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

