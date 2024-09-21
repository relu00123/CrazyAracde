using Google.Protobuf.Protocol;
using Server.Game.CA_Object;
using System;
using System.Collections.Generic;
using System.Text;

public class Player_IdleState : AbstractPlayerState
{
    public override void ApplyMove(InGameObject gameObject, MoveDir dir)
    {
        if (dir == MoveDir.MoveNone)
        {
            Console.Write("Stop Walking (Move Key detached)");

           // 아무 행동도 안해도 될 것 같다. 어처피 Idle상태였으니까
           // gameObject.ChangeState(CreatureState.Idle);
            return;
        }

        base.ApplyMove(gameObject, dir);
        gameObject.ChangeState(new Player_MovingState());
    }

    public override void EnterState(InGameObject obj)
    {

    }

    public override void ExitState(InGameObject obj)
    {

    }
}
